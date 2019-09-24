using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.web.FindnReplace
{
    public class FindnReplace
    {
        private string _content;
        private string _findText;
        private string _replaceWith;
        private bool _matchEntire;
        private HtmlDocument _rtDocument;

        public FindnReplace(string richTextContent, string textToFind, string replaceText, bool matchEntire)
        {
            this._content = richTextContent;
            this._findText = textToFind;
            this._replaceWith = replaceText;
            this._matchEntire = matchEntire;
            this._rtDocument = new HtmlDocument();
            this._rtDocument.LoadHtml(richTextContent);
        }

        #region Find
        public string FindText()
        {
            this._findText = this._findText.TrimEnd();
            int count = MarkAllMatches();
            if (count > 0)
            {
                this.ReplaceText();
            }
            return _rtDocument.DocumentNode.InnerHtml;
        }

        private int MarkAllMatches()
        {
            var node = this._content;
            var marker = "<span data-mce-bogus='1' class='mce-match-marker'></span>";
            return FindAndReplaceText(marker);
        }

        private int FindAndReplaceText(string marker)
        {
            var count = 0;
            var text = this.GetText();
            if (string.IsNullOrEmpty(text))
            {
                return count;
            }

            if (this._matchEntire)
            {
                if (!string.Equals(text, _findText))
                {
                    return count;
                }
            }

            var matches = this.GetMatches(text, this._findText);

            if (matches.Count > 0)
            {
                count = matches.Count;
                this.IterateThroughMatches(matches);
            }

            return count;
        }

        private List<MatchLocation> GetMatches(string text, string findText)
        {
            List<MatchLocation> matchLocations = new List<MatchLocation>();
            var match = Regex.Matches(text, findText);
            foreach (var m in match)
            {
                Match mt = ((Match)m);
                MatchLocation loc = new MatchLocation();
                loc.Text = mt.Value;
                loc.StartIndex = mt.Index;
                loc.EndIndex = mt.Index + mt.Length;
                matchLocations.Add(loc);
            }

            return matchLocations;
        }

        private void IterateThroughMatches(List<MatchLocation> matches)
        {
            HtmlNode startNode = null;
            int startNodeIndex = 0;
            HtmlNode endNode = null;
            int endNodeIndex = 0;
            List<HtmlNode> innerNodes = new List<HtmlNode>();
            int atIndex = 0;
            HtmlNode curNode = _rtDocument.DocumentNode;
            int matchIndex = 0;

            while (true)
            {
                if (curNode.NodeType == HtmlNodeType.Text)
                {
                    if (endNode == null && curNode.InnerText.Length + atIndex >= matches[matchIndex].EndIndex)
                    {
                        endNode = curNode;
                        endNodeIndex = matches[matchIndex].EndIndex - atIndex;
                    }
                    else if (startNode != null)
                    {
                        innerNodes.Add(curNode);
                    }

                    if (startNode == null && curNode.InnerText.Length + atIndex > matches[matchIndex].StartIndex)
                    {
                        startNode = curNode;
                        startNodeIndex = matches[matchIndex].StartIndex - atIndex;
                    }

                    atIndex += curNode.InnerText.Length;
                }

                if (startNode != null && endNode != null)
                {
                    curNode = this.GenerateReplacer(startNode, startNodeIndex, endNode, endNodeIndex, innerNodes, _findText, matchIndex);

                    atIndex -= (endNode.InnerText.Length - endNodeIndex);
                    startNode = null;
                    endNode = null;
                    innerNodes = new List<HtmlNode>();
                    matchIndex++;

                    if (matchIndex >= matches.Count)
                    {
                        break;
                    }
                }
                else if (curNode.FirstChild != null)
                {
                    curNode = curNode.FirstChild;
                    continue;
                }
                else if (curNode.NextSibling != null)
                {
                    curNode = curNode.NextSibling;
                    continue;
                }

                while (true)
                {
                    if (curNode.NextSibling != null)
                    {
                        curNode = curNode.NextSibling;
                        break;
                    }
                    else if (curNode.ParentNode != _rtDocument.DocumentNode)
                    {
                        curNode = curNode.ParentNode;
                    }
                    else
                    {
                        break;
                    }
                }

            }


        }

        private HtmlNode GenerateReplacer(HtmlNode startNode, int startNodeIndex, HtmlNode endNode, int endNodeIndex, List<HtmlNode> innerNodes, string findText, int matchIndex)
        {
            if (startNode == endNode)
            {
                var node = startNode;
                var parentNode = node.ParentNode;

                if (startNodeIndex > 0)
                {
                    var before = HtmlNode.CreateNode(node.InnerText.Substring(0, startNodeIndex));
                    parentNode.InsertBefore(before, node);
                }

                var element = CreateReplacerNode(findText, matchIndex);
                parentNode.InsertBefore(element, node);

                if (endNodeIndex < node.InnerText.Length)
                {
                    var after = HtmlNode.CreateNode(node.InnerText.Substring(endNodeIndex));
                    parentNode.InsertBefore(after, node);
                }

                node.ParentNode.RemoveChild(node);
                return element;
            }

            var before1 = this.CreateTextNode(startNode.InnerText.Substring(0, startNodeIndex));
            var after1 = this.CreateTextNode(endNode.InnerText.Substring(endNodeIndex));
            var repElement = CreateReplacerNode(startNode.InnerText.Substring(startNodeIndex), matchIndex);
            //var innerEls = [];

            for (int i = 0, l = innerNodes.Count; i < l; ++i)
            {
                var innerNode = innerNodes[i];
                var innerEl = CreateReplacerNode(innerNode.InnerText, matchIndex);
                innerNode.ParentNode.ReplaceChild(innerEl, innerNode);
                //innerEls.push(innerEl);
            }

            var elB = CreateReplacerNode(endNode.InnerText.Substring(0, endNodeIndex), matchIndex);

            var parentNode1 = startNode.ParentNode;
            parentNode1.InsertBefore(before1, startNode);
            parentNode1.InsertBefore(repElement, startNode);
            parentNode1.RemoveChild(startNode);

            parentNode1 = endNode.ParentNode;
            parentNode1.InsertBefore(elB, endNode);
            parentNode1.InsertBefore(after1, endNode);
            parentNode1.RemoveChild(endNode);

            return elB;
        }

        private HtmlNode CreateTextNode(string innerText)
        {
            if (string.IsNullOrEmpty(innerText))
            {
                innerText = " ";
            }
            return HtmlNode.CreateNode(innerText);
        }
        private HtmlNode CreateReplacerNode(string innerText, int index)
        {
            return HtmlNode.CreateNode("<span data-mce-bogus='1' class='mce-match-marker' data-mce-index='" + index + "'>" + innerText + "</span>");
        }

        private string GetText()
        {
            string rtText = _rtDocument.DocumentNode.InnerText;
            return rtText;
        }

        #endregion

        #region Replace
        public void ReplaceText()
        {
            List<HtmlNode> nodes = new List<HtmlNode>();
            HtmlNode node = _rtDocument.DocumentNode;
            int matchIndex = 0;
            int currentIndex = 0;
            int currentMatchIndex = 0;

            nodes = node.SelectNodes("//span").Where(n => n.Attributes.Any(a => a.Name == "class" && a.Value == "mce-match-marker")).ToList();

            for (int i = 0; i < nodes.Count; i++)
            {
                int n = 0;
                var nodeIndex = nodes[i].Attributes["data-mce-index"].Value;
                if (int.TryParse(nodeIndex, out n) && matchIndex == n)
                {
                    if (this._replaceWith.Length > 0)
                    {
                        nodes[i].FirstChild.InnerHtml = this._replaceWith;
                        RemoveDummyParent(nodes[i]);
                    }
                    else
                    {
                        RemoveNode(nodes[i]);
                    }

                    while (true)
                    {
                        ++i;
                        if (i >= nodes.Count)
                        {
                            i--;
                            break;
                        }
                        matchIndex = Convert.ToInt32(nodes[i].Attributes["data-mce-index"].Value);
                        if (matchIndex == currentMatchIndex)
                        {
                            RemoveNode(nodes[i]);
                        }
                        else
                        {
                            i--;
                            break;
                        }
                    }
                }
                else if (currentMatchIndex > currentIndex)
                {
                    nodes[i].SetAttributeValue("data-mce-index", Convert.ToString(currentMatchIndex - 1));
                }
            }
        }

        private void RemoveDummyParent(HtmlNode node)
        {
            var parentNode = node.ParentNode;
            if (node.FirstChild != null)
            {
                parentNode.InsertBefore(node.FirstChild, node);
            }

            node.ParentNode.RemoveChild(node);
        }

        private void RemoveNode(HtmlNode node)
        {
            var parentNode = node.ParentNode;
            node.Remove();

            string parentText = parentNode.InnerText;
            if (string.IsNullOrWhiteSpace(parentText))
            {
                parentNode.Remove();
            }
        }
        #endregion
    }
}
