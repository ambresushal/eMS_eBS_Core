using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace tmg.equinox.generatehtml
{
    public class GenerateHtml
    {
        private string _dataJSON, _designJSON;

        public GenerateHtml(string dataJSON, string designJSON)
        {
            _dataJSON = dataJSON;
            _designJSON = designJSON;
        }

        public string GetElementHTML(string elementPath)
        {
            JObject formData = JObject.Parse(_dataJSON);
            dynamic elementData = formData.SelectToken(elementPath);
            string html = elementData;
            return html;
        }

        public string GetRepeaterHTML(string elementPath)
        {
            string[] pathElements = elementPath.Split('.');
            string elementType = string.Empty, html = string.Empty;
            dynamic designData = null;
            dynamic columnsNames = null;
            JObject formData = JObject.Parse(_dataJSON);
            dynamic elementData = formData.SelectToken(elementPath);

            JObject formDesign = JObject.Parse(_designJSON);
            var sections = formDesign["Sections"];

            foreach (var section in sections)
            {
                if (section["GeneratedName"].ToString() == pathElements[0])
                {
                    var elemets = section["Elements"];

                    foreach (var ele in elemets)
                    {
                        for (int index = 1; index < pathElements.Length; index++)
                        {
                            if (ele["GeneratedName"].ToString() == pathElements[index])
                            {
                                if (ele["Type"].ToString() == "section")
                                {
                                    var elemnts = ele["Section"]["Elements"];
                                    foreach (var elemnt in elemnts)
                                    {
                                        if (elemnt["GeneratedName"].ToString() == pathElements[index + 1])
                                        {
                                            if (elemnt["Type"].ToString().ToLower() == "repeater")
                                            {
                                                elementType = "repeater";
                                                //if (ele["Repeater"]["LayoutClass"].ToString() == "customLayout")
                                                //{
                                                designData = elemnt["Repeater"]["RepeaterUIElementProperties"];

                                                columnsNames = elemnt["Repeater"]["Elements"];
                                                //}
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (ele["Type"].ToString().ToLower() == "repeater")
                                    {
                                        elementType = "repeater";
                                        designData = ele["Repeater"]["RepeaterUIElementProperties"];
                                        columnsNames = ele["Repeater"]["Elements"];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (elementType == "repeater")
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic arrElements = js.Deserialize<dynamic>(elementData.ToString());

                string rowTemp = designData.RowTemplate; var headers = designData.HeaderTemplate; var footer = designData.FooterTemplate;
                if (rowTemp == null) rowTemp = "";
                if (headers == null) headers = "";
                if (footer == null) footer = "";

                Regex regex = new Regex(@"\{\d+\}");

                var arrLength = arrElements.Length;

                for (int index = 0; index < arrLength; index++)
                {
                    Match match = regex.Match(rowTemp);
                    var columnNumber = 0;
                    do
                    {
                        if (match.ToString() != "")
                        {
                            var columnName = columnsNames[columnNumber].GeneratedName;
                            var value = arrElements[index][columnName.Value];
                            rowTemp = rowTemp.Replace(match.ToString(), value);
                            match = regex.Match(rowTemp);
                            columnNumber = columnNumber + 1;
                        }
                    }
                    while (match.ToString() != "");
                    html = html + rowTemp;
                    rowTemp = designData.RowTemplate;
                }
                html = "<table><tr>" + headers + "</tr><tr>" + html + "</tr><tr>" + footer + "</tr></table>";
            }
            else
                html = GetElementHTML(elementPath);
            return html;
        }
    }
}
