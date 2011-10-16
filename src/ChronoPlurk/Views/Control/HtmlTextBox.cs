using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ChronoPlurk.Helpers;
using ChronoPlurk.Views.ImageLoader;
using HtmlAgilityPack;

namespace ChronoPlurk.Views.PlurkControls
{
    [TemplatePart(Name = "RichTextBoxElement", Type = typeof(RichTextBox))]
    public class HtmlTextBox : Control
    {
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

            if (!string.IsNullOrEmpty(html))
            {
                var document = new HtmlDocument();
                document.LoadHtml(html);

                var paragraph = new Paragraph();
                var inlines = ProcessNodeInternal(document.DocumentNode);
                foreach (var inline in inlines)
                {
                    paragraph.Inlines.Add(inline);
                }
                RichTextBox.Blocks.Add(paragraph);
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
                                var run = new Run() { Text = childNode.InnerText };
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
                                    var nextNode = CreateInlineNode(childNode);
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
                return new Run() { Text = node.Node.InnerText };
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

        private InlineNode CreateInlineNode(HtmlNode node)
        {
            var type = GetHtmlType(node);
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
                default:
                    return new InlineNode(node, null);
            }
        }

        private static InlineNode CreateHyperlinkInlineNode(HtmlNode node)
        {
            var href = node.Attributes.FirstOrDefault(attr => attr.Name == "href");
            var hyperlink = new Hyperlink() { TargetName = "_blank", Foreground = PlurkResources.PhoneAccentBrush };
            if (href != null)
            {
                hyperlink.NavigateUri = new Uri(href.Value, UriKind.Absolute);
            }
            return new InlineNode(node, hyperlink);
        }

        private static Inline CreateImageInline(HtmlNode node)
        {
            var src = node.Attributes.FirstOrDefault(attr => attr.Name == "src");
            if (src != null)
            {
                var image = new Image();
                var heightAttr = node.Attributes.FirstOrDefault(attr => attr.Name == "height");
                if (heightAttr != null)
                {
                    double height;
                    if (double.TryParse(heightAttr.Value, out height))
                    {
                        image.Height = height;
                        image.Stretch = Stretch.UniformToFill;
                    }
                }
                LowProfileImageLoader.SetUriSource(image, new Uri(src.Value, UriKind.Absolute));
                var container = new InlineUIContainer { Child = image };
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
                default:
                    return HtmlType.Text;
            }
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
        }
    }
}
