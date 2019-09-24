using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.documentcomparer.RepeaterCompareUtils;

namespace tmg.equinox.documentcomparer
{
    public class DocumentSynchronizer
    {
        private string sourceDocument;
        private string targetDocument;
        private JObject sourceDocumentJSON;
        private JObject targetDocumentJSON;
        private DocumentCompareResult compareResult;

        public DocumentSynchronizer(string sourceDocument, string targetDocument, DocumentCompareResult compareResult)
        {
            this.sourceDocument = sourceDocument;
            this.targetDocument = targetDocument;
            this.compareResult = compareResult;
        }

        public string Synchronize()
        {
            //this.sourceDocumentJSON = JObject.Parse(sourceDocument);
            this.targetDocumentJSON = JObject.Parse(targetDocument);
            foreach (CompareResult result in compareResult.Results) 
            {
                if (result is SectionCompareResult) 
                {
                    SyncSection((SectionCompareResult)result);
                }
                else 
                {
                    SyncRepeater((RepeaterCompareResult)result);
                }
            }
            return JsonConvert.SerializeObject(this.targetDocumentJSON);
        }

        private void SyncSection(SectionCompareResult result) 
        {
            if (result.Fields != null && result.Fields.Count > 0) 
            {
                string path = result.Path;
                JToken targetSection = targetDocumentJSON.SelectToken(path);
                foreach (SectionCompareField field in result.Fields) 
                {
                    if (field.IsMissingInSource == false && field.IsMissingInTarget == false && field.IsMatch == false) 
                    {
                        targetSection[field.FieldName] = field.SourceValue;
                    }
                }
            }
        }

        private void SyncRepeater(RepeaterCompareResult result) 
        {
            string childContainerName = result.ChildContainerName;
            if (result.Rows != null && result.Rows.Count > 0)
            {
                string path = result.Path;
                JToken targetRepeater = targetDocumentJSON.SelectToken(path);
                foreach (RepeaterCompareRow row in result.Rows)
                {
                    if (row.CanSync == true && row.IsMatch == false)  
                    {
                        //get target row from JSON
                        JToken matchRow = RepeaterMatchUtil.GetMatchingRowInRepeater(targetRepeater, row.Keys, "Target");
                        if (row.Fields != null && row.Fields.Count > 0) 
                        {
                            //set fields
                            foreach (RepeaterCompareField field in row.Fields) 
                            {
                                matchRow[field.FieldName] = field.SourceValue;
                            }
                        }
                        if (row.ChildRows != null && row.ChildRows.Count > 0) 
                        {
                            foreach (RepeaterCompareRow childRow in row.ChildRows) 
                            {
                                //get target row from JSON
                                JToken childMatchRow = RepeaterMatchUtil.GetMatchingRowInRepeater(matchRow[childContainerName], childRow.Keys, "Target");
                                //set fields
                                if (childRow.Fields != null && childRow.Fields.Count > 0) 
                                {
                                    if (childRow.CanSync == true && childRow.IsMatch == false)
                                    {
                                        foreach (RepeaterCompareField field in childRow.Fields)
                                        {
                                            childMatchRow[field.FieldName] = field.SourceValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
