using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using tmg.equinox.core.masterlistcascade.filter;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.viewmodels;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.mlcascade.documentcomparer;

namespace tmg.equinox.core.masterlistcascade.filter
{
    //Implementation specific
    public class PlanCodeFilterExpressionProcessor : IFilterExpressionProcessor
    {

        public List<DocumentFilterResult> ProcessExpression(IMasterListCascadeService mlcService, IFolderVersionServices fvService, JObject queryData, FilterExpression filter, MasterListCascadeViewModel cascadeModel, MasterListVersions versions,DateTime effectiveDate,int folderVersionID)
        {
            List<DocumentFilterResult> results = new List<DocumentFilterResult>();
            //get the distinct values for field for Source
            List<string> inList = GetSourceINList(fvService, queryData,cascadeModel,versions, filter);
            //invoke SP and get the list of documents to process
            results = GetDocumentList(mlcService, inList, filter.SourceFields,cascadeModel.TargetDesignID,cascadeModel.TargetDesignVersionID, effectiveDate,folderVersionID,cascadeModel.MasterListCascadeID);
            return results;
        }

        private List<string> GetSourceINList(IFolderVersionServices fvService,JObject queryData, MasterListCascadeViewModel cascadeModel, MasterListVersions versions,FilterExpression filter)
        {
            List<string> sourceList = new List<string>();
            //if (versions.PreviousFormInstanceID > 0)
            //{
            //    DocumentCompareResult result = GetCompareResult(fvService, cascadeModel, versions);
            //    //find list based on result - assumption is that the result will have only one repeater compare
            //    string sourcePath = filter.SourcePath;
            //    sourceList = GetFilteredList(result, sourcePath, filter.SourceFields);
            //}
            //else
            //{
            sourceList = queryData.SelectToken(filter.SourcePath).Values<string>(filter.SourceFields).Distinct().ToList();
            //}
            return sourceList;
        }

        private List<string> GetFilteredList(DocumentCompareResult result,string filterPath,string filterField)
        {
            List<string> sourceList = new List<string>();
            if(result.Results != null && result.Results.Count > 0)
            {
                foreach(var compareResult in result.Results)
                {
                    RepeaterCompareResult res = (RepeaterCompareResult)compareResult;
                    foreach(var row in res.Rows)
                    {
                        if(row.IsMatch == false)
                        {
                            var keyValue = row.Keys.Find(a=>a.KeyName == filterField);
                            if(keyValue != null && !sourceList.Contains(keyValue.TargetKey))
                            {
                                sourceList.Add(keyValue.TargetKey);
                            }
                        }
                    }
                }
            }
            return sourceList;
        }

        private List<DocumentFilterResult> GetDocumentList(IMasterListCascadeService mlcService,List<string> inList,string filterPath, int formDesignID,int formDesignVersionID,DateTime effectiveDate,int folderVersionID,int masterListCascadeID)
        {
            List<DocumentFilterResult> results = new List<DocumentFilterResult>();
            var planCodes = from cd in inList
                            select new MLPlanCode
                            {
                                PlanCode = cd
                            };
            if(planCodes != null && planCodes.Count() > 0)
            {
                results = mlcService.FilterDocuments(effectiveDate, filterPath, formDesignID,formDesignVersionID, planCodes.ToList(),folderVersionID,masterListCascadeID);
            }
            return results;
        }

        private DocumentCompareResult GetCompareResult(IFolderVersionServices fvService,MasterListCascadeViewModel model,MasterListVersions versions)
        {
            string sourceDocument = fvService.GetFormInstanceDataCompressed(1, versions.CurrentFormInstanceID);
            string targetDocument = fvService.GetFormInstanceDataCompressed(1, versions.PreviousFormInstanceID);
            CompareDocument source = GetCompareDocument();
            CompareDocument target = GetCompareDocument();
            string macro = model.MasterListCompareJSON;
            string matchType = "";
            DocumentComparer comparer = new DocumentComparer(sourceDocument, targetDocument, source, target, macro, matchType, CompareDocumentSource.GENERATEREPORT);
            DocumentCompareResult result = comparer.Compare();
            return result;
        }

        private CompareDocument GetCompareDocument()
        {
            CompareDocument doc = new CompareDocument();
            doc.AccountName = "";
            doc.DocumentName = "";
            doc.DocumentType = "";
            doc.EffectiveDate = DateTime.Now.ToShortDateString();
            doc.FolderName = "";
            doc.FolderVerionNumber = "";
            return doc;
        }
    }
}
