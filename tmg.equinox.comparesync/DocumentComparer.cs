using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.documentcomparer
{
    public class DocumentComparer
    {
        private string sourceDocument;
        public string targetDocument;
        private string macro;
        private JObject sourceDocumentJSON;
        private JObject targetDocumentJSON;
        private CompareDocument source;
        private CompareDocument target;
        private JObject macroJSON;
        private List<CompareResult> results;
        private string matchType;
        CompareDocumentSource compareSource;
        public DocumentComparer(string sourceDocument, string targetDocument, CompareDocument source, CompareDocument target, string macro, string matchType,CompareDocumentSource compareSource) 
        {
            this.sourceDocument = sourceDocument;
            this.targetDocument = targetDocument;
            this.source = source;
            this.target = target;
            this.macro = macro;
            this.matchType = matchType;
            this.compareSource = compareSource;
        }

        public DocumentCompareResult Compare() 
        {
            DocumentCompareResult result = new DocumentCompareResult();
            result.SourceDocument = source;
            result.TargetDocument = target;
            results = new List<CompareResult>();
            InitiaizeJSONObjects();
            result.Results = ProcessMacroCompare();
            SetSyncAndMatch(result);
            return result;
        }
        private void InitiaizeJSONObjects()
        {
            sourceDocumentJSON = JObject.Parse(sourceDocument);
            targetDocumentJSON = JObject.Parse(targetDocument);
            macroJSON = JObject.Parse(macro);
        }

        private List<CompareResult> ProcessMacroCompare()
        {
            Dictionary<string, string> labelPathMap = new Dictionary<string, string>();
            List<CompareResult> results = new List<CompareResult>();
            List<JProperty> rootSections = macroJSON.Properties().ToList();
            foreach(var section in rootSections)
            {
                ProcessContainer(section, ref results, ref labelPathMap);
            }
            return results;
        }

        private void ProcessContainer(JProperty container, ref List<CompareResult> results,ref Dictionary<string,string> labelPathMap)
        {
            List<JToken> tokens = container.Values().ToList();
            bool hasFields = false;
            bool hasKeys = false;
            JToken fields = null;
            JToken keys = null;
            bool hasMacroSelected = false;
            string path = "";
            foreach (JToken token in tokens)
            {
                if (token.Path.EndsWith(".IsMacroSelected") == true)
                {
                    if (((JProperty)token).Value.ToString() == "true")
                    {
                        hasMacroSelected = true;
                    }
                }
                if (token.Path.EndsWith(".Fields") == true)
                {
                    if (token.First().ToString() != "[]")
                    {
                        hasFields = true;
                        fields = token;
                    }
                }
                if (token.Path.EndsWith(".Keys") == true)
                {
                    hasKeys = true;
                    keys = token;
                }
                if (token.Path.EndsWith(".Label") == true)
                {
                    if (((JProperty)token).Value != null)
                    {
                        string label = ((JProperty)token).Value.ToString();
                        labelPathMap.Add(token.Parent.Path, label);
                        path = token.Parent.Path;
                    }
                }
            }
            if (hasMacroSelected)
            {
                if (hasKeys == true)
                {
                    JToken sourceSection = null, targetSection = null;
                    if (hasFields == false)
                    {
                        sourceSection = sourceDocumentJSON.SelectToken(path);
                        targetSection = targetDocumentJSON.SelectToken(path);
                    }
                    else
                    {
                        sourceSection = DocumentJSONReaderHelper.GetSectionToken(sourceDocumentJSON, fields);
                        targetSection = DocumentJSONReaderHelper.GetSectionToken(targetDocumentJSON, fields);
                    }
                    RepeaterComparer comparer = new RepeaterComparer(fields, keys, sourceSection, targetSection, labelPathMap, matchType);
                    results.Add(comparer.Compare());

                }
            }
            if (hasFields == true && hasKeys == false)
            {
                JToken sourceSection = DocumentJSONReaderHelper.GetSectionToken(sourceDocumentJSON, fields);
                JToken targetSection = DocumentJSONReaderHelper.GetSectionToken(targetDocumentJSON, fields);
                SectionComparer comparer = new SectionComparer(fields, sourceSection, targetSection, labelPathMap, matchType, compareSource);
                results.Add(comparer.Compare());
            }

            foreach (JToken token in tokens)
            {
                if (!token.Path.EndsWith(".Fields") && !token.Path.EndsWith(".Keys") && !token.Path.EndsWith(".Label") && !token.Path.EndsWith(".IsMacroSelected"))
                {
                        ProcessContainer((JProperty)token, ref results, ref labelPathMap);
                }
            }
        }

        private void SetSyncAndMatch(DocumentCompareResult result)
        {
            if(result.Results != null && result.Results.Count > 0)
            {
                foreach (CompareResult row in result.Results) 
                {
                    if (row is SectionCompareResult) 
                    {
                        SectionCompareResult sectionResult = (SectionCompareResult)row;
                        if (sectionResult.CanSync == true && sectionResult.IsMatch == false) 
                        {
                            result.CanSync = true;
                            result.IsMatch = false;
                            break;
                        }
                    }
                    else 
                    {
                        RepeaterCompareResult repeaterResult = (RepeaterCompareResult)row;
                        if (repeaterResult.CanSync == true && repeaterResult.IsMatch == false)
                        {
                            result.CanSync = true;
                            result.IsMatch = false;
                            break;
                        }
                    }
                }
            }
        }

    }
}
