using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using tmg.equinox.core.logging.Logging;

namespace tmg.equinox.generatecollateral
{
    public class HTMLComplianceFormatter
    {
        private static readonly ILog _logger = LogProvider.For<HTMLComplianceFormatter>();
        public applicationservices.viewmodels.FolderVersion.FormInstanceExportPDF FormInstanceDetails { get; set; }
        private bool _allowFix = false;

        public HTMLComplianceFormatter(bool allowFix)
        {
            _allowFix = allowFix;
        }
        public void FixAllHtmlFormat(string strInputHtml)
        {
            string strOutputHtml = strInputHtml;
            try
            {
                if (_allowFix == true)
                {
                    strOutputHtml = FixHeadTableFormatter(strInputHtml);
                    strOutputHtml = FixImageFormatter(strOutputHtml);
                    strOutputHtml = FixLinkFormatter(strOutputHtml);
                }
            }
            catch (Exception e)
            {
                _logger.ErrorException("Unable to fix HTML", e);
            }
            //return strOutputHtml;
        }

        public string FixHeadTableFormatter(string strInputHtml)
        {
            string strOutputHtml = strInputHtml;

            // create html document
            var doc = new HtmlDocument();
            bool isModify = false;
            strInputHtml = strInputHtml.Replace("\n", "");
            doc.LoadHtml(strInputHtml);
            var tableTag = doc.DocumentNode.Descendants("table").FirstOrDefault();
            if (tableTag != null)
            {
                var attrNode = tableTag.ChildNodes.Where(a => a.Name == "thead").FirstOrDefault();
                if (attrNode == null)
                {
                    var tbodyNode = tableTag.ChildNodes.Where(a => a.Name == "tbody").FirstOrDefault();
                    if (tbodyNode != null)
                    {


                        if (tbodyNode.ChildNodes.Count > 0)
                        {
                            isModify = true;
                            var firstRowNode = tbodyNode.ChildNodes.First();
                            tbodyNode.ChildNodes.RemoveAt(0);

                            HtmlNode node = new HtmlNode(HtmlNodeType.Element, doc, tableTag.InnerStartIndex + 1);
                            node.Name = "thead";
                            node.ChildNodes.Insert(0, firstRowNode);
                            tableTag.ChildNodes.Insert(0, node);
                            LogEntry("Error in Table html", strInputHtml);
                        }

                    }
                }
                if (isModify == true)
                {
                    strOutputHtml = doc.DocumentNode.InnerHtml;
                }
            }
            return strOutputHtml;
        }
        public string FixImageFormatter(string strInputHtml)
        {
            string strOutputHtml = strInputHtml;
            var doc = new HtmlDocument();
            doc.LoadHtml(strInputHtml);
            var imageTag = doc.DocumentNode.Descendants("img").FirstOrDefault();
            if (imageTag != null)
            {
                var attrImg = imageTag.Attributes.Where(a => a.Name == "alt").FirstOrDefault();
                if (attrImg == null)
                {
                    imageTag.Attributes.Append("alt", "figure");
                    LogEntry("Error in Image html", strInputHtml);
                    strOutputHtml = doc.DocumentNode.InnerHtml;
                }

            }
            return strOutputHtml;
        }
        public string FixLinkFormatter(string strInputHtml)
        {
            string strOutputHtml = strInputHtml;
            var doc = new HtmlDocument();
            doc.LoadHtml(strInputHtml);
            var linkTag = doc.DocumentNode.Descendants("a").FirstOrDefault();

            if (doc.DocumentNode.Descendants("a").Count() > 1)
            {
                int cout = doc.DocumentNode.Descendants("a").Count();
            }

            if (linkTag != null)
            {
                var attrTitle = linkTag.Attributes.Where(a => a.Name == "title").FirstOrDefault();
                if (linkTag.InnerHtml == "")
                {
                    LogEntry("Error in Link html:NoText", strInputHtml);
                    strOutputHtml = strOutputHtml.Replace(linkTag.OuterHtml, "");
                    return strOutputHtml;
                }
                if (attrTitle == null)
                {
                    var attrHref = linkTag.Attributes.Where(a => a.Name == "href").FirstOrDefault();

                    var title = "title";
                    if (attrHref != null)
                    {
                        title = attrHref.Value;
                        if (string.IsNullOrEmpty(title))
                        {
                            title = "title";
                        }
                        if (attrHref.Value.StartsWith("www") == true)
                        {
                            LogEntry("Error in Link html:Nohttp", strInputHtml);
                            attrHref.Value = string.Format("http://{0}", attrHref.Value);
                        }
                    }
                    else
                    {
                        LogEntry("Error in Link html:NoHref", strInputHtml);
                    }
                    linkTag.Attributes.Append("title", title);
                    LogEntry("Error in Link html:NoTitle", strInputHtml);
                    strOutputHtml = doc.DocumentNode.InnerHtml;
                }

            }
            return strOutputHtml;
        }
        public void LogEntry(string strErrorMessage, string strInputHtml)
        {
            if (FormInstanceDetails != null)
            {
                strErrorMessage = strErrorMessage + " Folder:" + FormInstanceDetails.FolderName + ", PlanName: " + FormInstanceDetails.FormName + ", FolderVersion: " + FormInstanceDetails.FolderVersionNumber;
            }
            _logger.ErrorException(strErrorMessage, new Exception(strInputHtml));
        }
    }
    public class ProcessJsonFormatter
    {
        public string GetJosnFormatter(string strJson)
        {
            string strJosnData = strJson;
            JToken objJtokenvar = JToken.Parse(strJson);
            ProcessJtoken(objJtokenvar);

            return JsonConvert.SerializeObject(objJtokenvar);
        }
        public void ProcessJtoken(JToken jTokenObj)
        {
            if (jTokenObj.Count() > 0)
            {
                if (jTokenObj.Type == JTokenType.Array)
                {
                    JArray tokenArray = (JArray)jTokenObj;
                    foreach (var item in tokenArray)
                    {
                        var richVal = item["RichText"];
                        if (richVal != null)
                        {
                            string strRichTextInput = item["RichText"].ToString();
                            if (!string.IsNullOrEmpty(strRichTextInput))
                            {
                                strRichTextInput = strRichTextInput.Replace("\n", "");
                                HTMLComplianceFormatter objHTMLComplianceFormatter = new HTMLComplianceFormatter(true);
                                strRichTextInput = objHTMLComplianceFormatter.FixHeadTableFormatter(strRichTextInput);
                                item["RichText"] = strRichTextInput;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var jTokenObjChild in jTokenObj.Children())
                    {
                        ProcessJtoken(jTokenObjChild);
                    }
                }
            }
        }
    }
}