using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.FindnReplace
{
    public class AnnotationExtractor
    {
        private string _content;
        private HtmlDocument _rtDocument;

        public AnnotationExtractor(string richTextContent)
        {
            this._content = richTextContent;
            this._rtDocument = new HtmlDocument();
            this._rtDocument.LoadHtml(richTextContent);
        }

        public List<AnnotationViewModel> GetAnnotation()
        {
            List<AnnotationViewModel> annotations = new List<AnnotationViewModel>();
            List<HtmlNode> nodes = new List<HtmlNode>();
            HtmlNode node = _rtDocument.DocumentNode;
            nodes = node.SelectNodes("//span").Where(n => n.Attributes.Any(a => a.Name == "class" && a.Value == "annotation")).ToList();

            foreach (var annNode in nodes)
            {
                string authorName = annNode.GetAttributeValue("data-author", "");
                string text = annNode.GetAttributeValue("data-annotation", "");
                annotations.Add(new AnnotationViewModel() { Annotation = text, AuthorName = authorName, AnnotationFor = annNode.InnerText });
            }

            return annotations;
        }

    }
}
