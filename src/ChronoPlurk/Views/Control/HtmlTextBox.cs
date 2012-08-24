using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ChronoPlurk.Helpers;
using ChronoPlurk.Views.ImageLoader;
using HtmlAgilityPack;
using Microsoft.Phone.Tasks;

namespace ChronoPlurk.Views.PlurkControls
{
    [TemplatePart(Name = "RichTextBoxElement", Type = typeof(RichTextBox))]
    public class HtmlTextBox : Control
    {
        private readonly CompositeDisposable _imageClickEvents = new CompositeDisposable();

        #region Html (DependencyProperty)

        /// <summary>
        /// Gets or sets HTML content.
        /// </summary>
        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.Register("Html", typeof(string), typeof(HtmlTextBox),
            new PropertyMetadata(null, new PropertyChangedCallback(OnHtmlChanged)));

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HtmlTextBox)d).OnHtmlChanged(e);
        }

        protected virtual void OnHtmlChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (RichTextBox != null)
                {
                    ConvertHtml(e.NewValue as string);
                }
            }
        }

        #endregion


        #region EnableHyperlink (DependencyProperty)

        /// <summary>
        /// Gets or sets whether to enable hyperlink in HtmlTextBox.
        /// </summary>
        public bool EnableHyperlink
        {
            get { return (bool)GetValue(EnableHyperlinkProperty); }
            set { SetValue(EnableHyperlinkProperty, value); }
        }

        public static readonly DependencyProperty EnableHyperlinkProperty =
            DependencyProperty.Register("EnableHyperlink", typeof(bool), typeof(HtmlTextBox),
              new PropertyMetadata(false));

        #endregion


        #region EnableOrignialSizeImage (DependencyProperty)

        /// <summary>
        /// Gets or sets whether to use original size for images.
        /// </summary>
        public bool EnableOrignialSizeImage
        {
            get { return (bool)GetValue(EnableOrignialSizeImageProperty); }
            set { SetValue(EnableOrignialSizeImageProperty, value); }
        }
        public static readonly DependencyProperty EnableOrignialSizeImageProperty =
            DependencyProperty.Register("EnableOrignialSizeImage", typeof(bool), typeof(HtmlTextBox),
              new PropertyMetadata(false));

        #endregion
        

        private RichTextBox _richTextBox;
        protected RichTextBox RichTextBox
        {
            get { return _richTextBox; }
            set
            {
                _richTextBox = value;
                if (_richTextBox != null)
                {
                    ConvertHtml(Html);
                }
            }
        }

        public HtmlTextBox()
        {
            DefaultStyleKey = typeof(HtmlTextBox);
        }

        public override void OnApplyTemplate()
        {
            RichTextBox = GetTemplateChild("RichTextBoxElement") as RichTextBox;

            base.OnApplyTemplate();
        }

        private void ConvertHtml(string html)
        {
            RichTextBox.Blocks.Clear();
            _imageClickEvents.Clear();

            if (!string.IsNullOrEmpty(html))
            {
                var sanitized = Regex.Replace(html, @"<(.|\n)*?>", "");
                var rawParagraph = new Paragraph();
                rawParagraph.Inlines.Add(sanitized);
                var imgCount = Regex.Matches(html, "<img").Count;
                for (var i = 0; i < imgCount; i++)
                {
                    rawParagraph.Inlines.Add(new LineBreak() { FontSize = 20.0 });
                }
                RichTextBox.Blocks.Add(rawParagraph);

                double recommendHeight; // Get recommend height for LongListSelector scrolling
                if (RecommendHeightCache.Instance.TryGetValue(html, out recommendHeight))
                {
                    this.Height = recommendHeight;
                }

                ThreadEx.OnThreadPool(() =>
                {
                    var document = new HtmlDocument();
                    document.LoadHtml(html);
                    return document;
                }, document =>
                {
                    RichTextBox.Blocks.Remove(rawParagraph);

                    var paragraph = new Paragraph();
                    var inlines = ProcessNodeInternal(document.DocumentNode);
                    foreach (var inline in inlines)
                    {
                        paragraph.Inlines.Add(inline);
                    }
                    RichTextBox.Blocks.Add(paragraph);

                    this.Height = double.NaN; // Reset height to auto.
                    RecommendHeightCache.Instance.Add(html, this.ActualHeight); // Cache current height.
                });
            }
        }

        private IEnumerable<Inline> ProcessNodeInternal(HtmlNode htmlNode)
        {
            var queue = new Queue<Inline>();
            var stack = new Stack<InlineNode>();
            stack.Push(new InlineNode(htmlNode, null));

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                Action<Inline> addInlineOrQueue = inline => AddInlineOrQueue(inline, node, queue);

                if (node.Node.HasChildNodes)
                {
                    foreach (var childNode in node.Node.ChildNodes)
                    {
                        switch (childNode.NodeType)
                        {
                            case HtmlNodeType.Text:
                                var run = new Run() { Text = HtmlDecode(childNode.InnerText) };
                                addInlineOrQueue(run);
                                break;
                            case HtmlNodeType.Element:
                                var type = GetHtmlType(childNode);
                                if (type == HtmlType.Image)
                                {
                                    var image = CreateImageInline(childNode);
                                    if (image != null)
                                    {
                                        addInlineOrQueue(image);
                                    }
                                }
                                else if (type == HtmlType.Newline)
                                {
                                    addInlineOrQueue(new LineBreak());
                                }
                                else
                                {
                                    var nextNode = CreateInlineNode(childNode, type);
                                    if (nextNode.Span != null)
                                    {
                                        addInlineOrQueue(nextNode.Span);
                                    }
                                    stack.Push(nextNode);
                                }
                                break;
                        }
                    }
                }
                else
                {
                    var inline = CreateTextInline(node);
                    if (inline != null)
                    {
                        addInlineOrQueue(inline);
                    }
                }
            }
            return queue;
        }

        private static Inline CreateTextInline(InlineNode node)
        {
            if (node.Node.NodeType == HtmlNodeType.Text)
            {
                return new Run() { Text = HtmlDecode(node.Node.InnerText) };
            }
            else
            {
                if (node.Node.Name == "br")
                {
                    return new LineBreak();
                }
                else
                {
                    return null;
                }
            }
        }

        private void AddInlineOrQueue(Inline inline, InlineNode currentNode, Queue<Inline> inlineQueue)
        {
            if (inline == null)
            {
                throw new ArgumentNullException("inline");
            }
            if (currentNode.Span != null)
            {
                var inlineUi = inline as InlineUIContainer;
                if (inlineUi != null) // this is image
                {
                    var nextSpan = CreateInlineNode(currentNode.Node).Span;
                    if (nextSpan != null)
                    {
                        currentNode.Span = nextSpan;
                        inlineQueue.Enqueue(nextSpan);
                    }
                    var nextHyperlink = nextSpan as Hyperlink;
                    if (nextHyperlink != null)
                    {
                        SubscribeImageClick(nextHyperlink, inlineUi);
                    }
                    inlineQueue.Enqueue(inlineUi);
                }
                else
                {
                    currentNode.Span.Inlines.Add(inline);
                }
            }
            else
            {
                inlineQueue.Enqueue(inline);
            }
        }

        private void SubscribeImageClick(Hyperlink nextHyperlink, InlineUIContainer inlineUi)
        {
            if (inlineUi.Child != null)
            {
                var mouseDownEvent =
                Observable.FromEventPattern<GestureEventArgs>(
                    handler => inlineUi.Child.Tap += handler,
                    handler => inlineUi.Child.Tap -= handler);
                var click = mouseDownEvent
                    .ObserveOnDispatcher().SubscribeOnDispatcher()
                    .Select(e => new { Args = e.EventArgs, Uri = nextHyperlink.NavigateUri })
                    .Subscribe(e =>
                    {
                        e.Args.Handled = true;
                        if (e.Uri.IsAbsoluteUri)
                        {
                            var webBrowserTask = new WebBrowserTask() { Uri = e.Uri };
                            webBrowserTask.Show();
                        }
                        else
                        {
                            var frame = Application.Current.RootVisual as Microsoft.Phone.Controls.PhoneApplicationFrame;
                            if (frame != null)
                            {
                                frame.Navigate(e.Uri);
                            }
                        }
                    });
                _imageClickEvents.Add(click);
            }
        }

        private InlineNode CreateInlineNode(HtmlNode node)
        {
            return CreateInlineNode(node, GetHtmlType(node));
        }

        private InlineNode CreateInlineNode(HtmlNode node, HtmlType type)
        {
            switch (type)
            {
                case HtmlType.Hyperlink:
                    if (EnableHyperlink)
                    {
                        return CreateHyperlinkInlineNode(node);
                    }
                    else
                    {
                        return new InlineNode(node, new Underline() { Foreground = PlurkResources.PhoneAccentBrush });
                    }
                case HtmlType.Decoration:
                    var decoration = GetDecorationType(node);
                    switch (decoration)
                    {
                        case DecorationType.B:
                            return new InlineNode(node, new Bold());
                        case DecorationType.I:
                            return new InlineNode(node, new Italic());
                        case DecorationType.U:
                            return new InlineNode(node, new Underline());
                        case DecorationType.Strike:
                            return new InlineNode(node, new Strikethrough());
                        default:
                            return new InlineNode(node, new Span());
                    }
            }
            return new InlineNode(node, null);
        }

        private static InlineNode CreateHyperlinkInlineNode(HtmlNode node)
        {
            var href = node.Attributes.FirstOrDefault(attr => attr.Name == "href");
            var hyperlink = new Hyperlink() { Foreground = PlurkResources.PhoneAccentBrush };
            if (href != null)
            {
                hyperlink.NavigateUri = UrlRemapper.RemapUrl(href.Value);
                if (hyperlink.NavigateUri.IsAbsoluteUri)
                {
                    hyperlink.TargetName = "_blank";
                }
            }
            return new InlineNode(node, hyperlink);
        }

        private Inline CreateImageInline(HtmlNode node)
        {
            Func<string, bool> isntEmoticon = filename => !EmoticonsDictionary.Contains(filename);

            var src = node.Attributes.FirstOrDefault(attr => attr.Name == "src");
            if (src != null)
            {
                var imageContainer = new Border();
                if (EnableOrignialSizeImage)
                {
                    GifLowProfileImageLoader.SetStretch(imageContainer, Stretch.Uniform);
                }
                else if (isntEmoticon(src.Value))
                {
                    var heightAttr = node.Attributes.FirstOrDefault(attr => attr.Name == "height");
                    if (heightAttr != null)
                    {
                        // NOTE: Some imgs may have attributes like height="40px"
                        var match = Regex.Match(heightAttr.Value, "[0-9]+");
                        if (match.Success)
                        {
                            double height;
                            if (double.TryParse(match.Value, out height))
                            {
                                var adjustedHeight = height == 30.0 ? 40.0 : height;
                                imageContainer.Height = adjustedHeight;
                                GifLowProfileImageLoader.SetStretch(imageContainer, Stretch.Uniform);
                            }
                        }
                    }
                }
                GifLowProfileImageLoader.SetUriSource(imageContainer, new Uri(src.Value, UriKind.Absolute));
                var container = new InlineUIContainer { Child = imageContainer };
                return container;
            }
            else
            {
                return null;
            }
        }

        private static HtmlType GetHtmlType(HtmlNode node)
        {
            var name = node.Name;
            switch (name)
            {
                case "a":
                    return HtmlType.Hyperlink;
                case "img":
                    return HtmlType.Image;
                case "br":
                    return HtmlType.Newline;
                case "b":
                case "i":
                case "u":
                case "strike":
                    return HtmlType.Decoration;
                default:
                    return HtmlType.Text;
            }
        }

        private static DecorationType GetDecorationType(HtmlNode node)
        {
            switch (node.Name)
            {
                case "b":
                    return DecorationType.B;
                case "i":
                    return DecorationType.I;
                case "u":
                    return DecorationType.U;
                case "strike":
                    return DecorationType.Strike;
                default:
                    return DecorationType.None;
            }
        }

        private static string HtmlDecode(string value)
        {
            return Plurto.Converters.HtmlEntity.HtmlDecode(value);
        }

        private class InlineNode
        {
            public HtmlNode Node { get; private set; }
            public Span Span { get; set; }

            public InlineNode(HtmlNode node, Span span)
            {
                Node = node;
                Span = span;
            }
        }

        private enum HtmlType
        {
            Text, Hyperlink, Image, Newline,
            Decoration,
        }

        private enum DecorationType
        {
            None, B, I, U, Strike,
        }

        /// <summary>
        /// Simulate class for strike tag span.
        /// </summary>
        private class Strikethrough : Span
        {
            public Strikethrough()
            {
                // Use fade color instead (Strike is not natively supported)
                this.Foreground = PlurkResources.PhoneDisabledBrush;
            }
        }
    }
}
