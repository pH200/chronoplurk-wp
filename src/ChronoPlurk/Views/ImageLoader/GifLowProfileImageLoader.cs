// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using ImageTools;
using ImageTools.Controls;
using ImageTools.IO.Gif;

namespace ChronoPlurk.Views.ImageLoader
{
    /// <summary>
    /// Provides access to the Image.UriSource attached property which allows
    /// Images to be loaded by Windows Phone with less impact to the UI thread.
    /// </summary>
    public static class GifLowProfileImageLoader
    {
        private const int WorkItemQuantum = 5;
        private static readonly Thread _thread = new Thread(WorkerThreadProc);
        private static readonly Queue<PendingRequest> _pendingRequests = new Queue<PendingRequest>();
        private static readonly Queue<IAsyncResult> _pendingResponses = new Queue<IAsyncResult>();
        private static readonly object _syncBlock = new object();
        private static bool _exiting;

        /// <summary>
        /// Gets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <returns>Uri to use for providing the contents of the Source property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static Uri GetUriSource(Border obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            return (Uri)obj.GetValue(UriSourceProperty);
        }

        /// <summary>
        /// Sets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <param name="value">Uri to use for providing the contents of the Source property.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static void SetUriSource(Border obj, Uri value)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(UriSourceProperty, value);
        }

        /// <summary>
        /// Identifies the UriSource attached DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.RegisterAttached(
            "UriSource", typeof(Uri), typeof(GifLowProfileImageLoader), new PropertyMetadata(OnUriSourceChanged));


        #region StretchProperty (Attached DependencyProperty)

        public static readonly DependencyProperty StretchProperty = DependencyProperty.RegisterAttached(
            "Stretch", typeof(Stretch), typeof(GifLowProfileImageLoader),
            new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

        public static void SetStretch(DependencyObject o, Stretch value)
        {
            o.SetValue(StretchProperty, value);
        }

        public static Stretch GetStretch(DependencyObject o)
        {
            return (Stretch)o.GetValue(StretchProperty);
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var border = d as Border;
            if (border == null)
            {
                return;
            }
            var image = border.Child as Image;
            if (image != null)
            {
                image.Stretch = (Stretch)e.NewValue;
            }
            else
            {
                var animatedImage = border.Child as AnimatedImage;
                if (animatedImage != null)
                {
                    animatedImage.Stretch = (Stretch)e.NewValue;
                }
            }
        }

        #endregion

        private static readonly StreamResourceInfo _emoticonZip;

        /// <summary>
        /// Gets or sets a value indicating whether low-profile image loading is enabled.
        /// </summary>
        public static bool IsEnabled { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Static constructor performs additional tasks.")]
        static GifLowProfileImageLoader()
        {
            _emoticonZip = Application.GetResourceStream(new Uri("Resources/CachedEmoticons/default_all.zip",
                                                                 UriKind.Relative));
            // Start worker thread
            _thread.Start();
            Application.Current.Exit += new EventHandler(HandleApplicationExit);
            IsEnabled = true;
        }

        private static void HandleApplicationExit(object sender, EventArgs e)
        {
            // Tell worker thread to exit
            _exiting = true;
            if (Monitor.TryEnter(_syncBlock, 100))
            {
                Monitor.Pulse(_syncBlock);
                Monitor.Exit(_syncBlock);
            }
            if (_emoticonZip.Stream != null)
            {
                _emoticonZip.Stream.Dispose();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Relevant exceptions don't have a common base class.")]
        private static void WorkerThreadProc(object unused)
        {
            Queue<PendingRequest> pendingRequests = new Queue<PendingRequest>();
            Queue<IAsyncResult> pendingResponses = new Queue<IAsyncResult>();
            while (!_exiting)
            {
                lock (_syncBlock)
                {
                    // Wait for more work if there's nothing left to do
                    if ((0 == _pendingRequests.Count) && (0 == _pendingResponses.Count) && (0 == pendingRequests.Count) && (0 == pendingResponses.Count))
                    {
                        Monitor.Wait(_syncBlock);
                        if (_exiting)
                        {
                            return;
                        }
                    }
                    // Copy work items to private collections
                    while (0 < _pendingRequests.Count)
                    {
                        pendingRequests.Enqueue(_pendingRequests.Dequeue());
                    }
                    while (0 < _pendingResponses.Count)
                    {
                        pendingResponses.Enqueue(_pendingResponses.Dequeue());
                    }
                }
                Queue<PendingCompletion> pendingCompletions = new Queue<PendingCompletion>();
                // Process pending requests
                for (var i = 0; (i < pendingRequests.Count) && (i < WorkItemQuantum); i++)
                {
                    var pendingRequest = pendingRequests.Dequeue();

                    //AndreasHammar 2011-01-25: is getting this in the Tre.News app
                    if (pendingRequest.Uri == null) continue;

                    var isCachedGif = false;
                    Uri emoticonUri = null;
                    if (pendingRequest.Uri.IsAbsoluteUri)
                    {
                        if (EmoticonsDictionary.TryGetValue(pendingRequest.Uri.OriginalString, out emoticonUri))
                        {
                            isCachedGif = true;
                        }
                    }
                    
                    if (!isCachedGif && pendingRequest.Uri.IsAbsoluteUri)
                    {
                        // Download from network
                        var webRequest = HttpWebRequest.CreateHttp(pendingRequest.Uri);
                        webRequest.AllowReadStreamBuffering = true; // Don't want to block this thread or the UI thread on network access
                        webRequest.BeginGetResponse(HandleGetResponseResult, new ResponseState(webRequest, pendingRequest.Image, pendingRequest.Uri));
                    }
                    else
                    {
                        // Load from application (must have "Build Action"="Content")
                        var originalUriString = pendingRequest.Uri.OriginalString;
                        // Trim leading '/' to avoid problems
                        var resourceStreamUri = originalUriString.StartsWith("/", StringComparison.Ordinal) ? new Uri(originalUriString.TrimStart('/'), UriKind.Relative) : pendingRequest.Uri;
                        // Enqueue resource stream for completion
                        var streamResourceInfo = !isCachedGif
                                                     ? Application.GetResourceStream(resourceStreamUri)
                                                     : Application.GetResourceStream(_emoticonZip, emoticonUri);
                        if (null != streamResourceInfo)
                        {
                            pendingCompletions.Enqueue(new PendingCompletion(pendingRequest.Image, pendingRequest.Uri, streamResourceInfo.Stream));
                        }
                    }
                    // Yield to UI thread
                    Thread.Sleep(5);
                }
                // Process pending responses
                for (var i = 0; (i < pendingResponses.Count) && (i < WorkItemQuantum); i++)
                {
                    var pendingResponse = pendingResponses.Dequeue();
                    var responseState = (ResponseState)pendingResponse.AsyncState;
                    try
                    {
                        var response = responseState.WebRequest.EndGetResponse(pendingResponse);
                        pendingCompletions.Enqueue(new PendingCompletion(responseState.Image, responseState.Uri, response.GetResponseStream()));
                    }
                    catch (WebException)
                    {
                        // Ignore web exceptions (ex: not found)
                    }
                    // Yield to UI thread
                    Thread.Sleep(5);
                }
                // Process pending completions
                if (0 < pendingCompletions.Count)
                {
                    // Get the Dispatcher and process everything that needs to happen on the UI thread in one batch
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        while (0 < pendingCompletions.Count)
                        {
                            // Decode the image and set the source
                            var pendingCompletion = pendingCompletions.Dequeue();
                            if (GetUriSource(pendingCompletion.Image) == pendingCompletion.Uri)
                            {
                                try
                                {
                                    if (pendingCompletion.Uri.OriginalString.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        var decoder = new GifDecoder();
                                        var extendedImage = new ExtendedImage();
                                        decoder.Decode(extendedImage, pendingCompletion.Stream);
                                        var image = new AnimatedImage() { Source = extendedImage, Stretch = Stretch.None };
                                        // image.Stretch = (Stretch)pendingCompletion.Image.GetValue(StretchProperty);
                                        pendingCompletion.Image.Child = image;
                                    }
                                    else
                                    {
                                        var bitmapImage = new BitmapImage();
                                        bitmapImage.SetSource(pendingCompletion.Stream);
                                        var image = new Image() { Source = bitmapImage };
                                        image.Stretch = (Stretch)pendingCompletion.Image.GetValue(StretchProperty);
                                        pendingCompletion.Image.Child = image;
                                    }
                                }
                                catch
                                {
                                    // Ignore image decode exceptions (ex: invalid image)
                                }
                            }
                            // Dispose of response stream
                            pendingCompletion.Stream.Dispose();
                        }
                    });
                }
            }
        }

        private static void OnUriSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var image = o as Border;
            if (image == null)
            {
                return;
            }
            var uri = (Uri)e.NewValue;

            if (!IsEnabled || DesignerProperties.IsInDesignTool)
            {
                // Avoid handing off to the worker thread (can cause problems for design tools)
                image.Child = new Image() { Source = new BitmapImage(uri) };
            }
            else
            {
                lock (_syncBlock)
                {
                    // Enqueue the request
                    _pendingRequests.Enqueue(new PendingRequest(image, uri));
                    Monitor.Pulse(_syncBlock);
                }
            }
        }

        private static void HandleGetResponseResult(IAsyncResult result)
        {
            lock (_syncBlock)
            {
                // Enqueue the response
                _pendingResponses.Enqueue(result);
                Monitor.Pulse(_syncBlock);
            }
        }

        private class PendingRequest
        {
            public Border Image { get; private set; }
            public Uri Uri { get; set; }
            public PendingRequest(Border image, Uri uri)
            {
                Image = image;
                Uri = uri;
            }
        }

        private class ResponseState
        {
            public WebRequest WebRequest { get; private set; }
            public Border Image { get; private set; }
            public Uri Uri { get; private set; }
            public ResponseState(WebRequest webRequest, Border image, Uri uri)
            {
                WebRequest = webRequest;
                Image = image;
                Uri = uri;
            }
        }

        private class PendingCompletion
        {
            public Border Image { get; private set; }
            public Uri Uri { get; private set; }
            public Stream Stream { get; private set; }
            public PendingCompletion(Border image, Uri uri, Stream stream)
            {
                Image = image;
                Uri = uri;
                Stream = stream;
            }
        }
    }
}
