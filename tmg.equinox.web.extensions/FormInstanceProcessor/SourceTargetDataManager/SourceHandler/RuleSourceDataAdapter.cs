using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpretercollateral;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.web.sourcehandler;

namespace tmg.equinox.web.sourcehandler
{
    public class RuleSourceDataAdapter
    {
        public Dictionary<string, JToken> GetRuleSourceData(int folderVersionId, Dictionary<string, int> ruleAliasFormInstances, RuleSourcesContainer sourceContainer, SourceHandlerDBManager dbHandlerManager, FormDesignVersionDetail sourceDesign, CurrentRequestContext requestContext)
        {
            Dictionary<string, string> sources = sourceContainer.RuleSources.Select(sel => sel).ToDictionary(x => x.SourceName, x => x.SourcePath);

            SourceGroupingWrapper sourceGrouping = new SourceGroupingWrapper(sources, ruleAliasFormInstances);
            List<SourceHandlerInput> handlerInputs = sourceGrouping.GetSourceHandlerInput();
            Dictionary<string, JToken> sourceOutputs = new Dictionary<string, JToken>();
            foreach (var handlerInput in handlerInputs)
            {
                SourceType sourceType = RuleSourceFactory.GetSourceType(handlerInput.FormInstanceId, handlerInput.RuleAlias, dbHandlerManager, requestContext);
                if(sourceType == SourceType.MasterList)
                {
                    handlerInput.SourceType = "MasterList";
                }
                else
                {
                    handlerInput.SourceType = "NonMasterList";
                }
            }
            var nmlHandlerInputs = handlerInputs.Where(a => a.SourceType == "NonMasterList").ToList();
            foreach (var handlerInput in nmlHandlerInputs)
            {
                string docFilter = sourceContainer.RuleSources.Where(a=> a.SourcePath.Split('[')[0] == handlerInput.RuleAlias).Select(sel => sel.SourceDocumentFilter).FirstOrDefault();
                string mlDocumentInstanceFilter = "";
                IRuleSourceHandler sourceHandler = RuleSourceFactory.GetSourceHandler(folderVersionId, handlerInput.FormInstanceId, handlerInput, dbHandlerManager, sourceDesign.IsMasterList, requestContext, mlDocumentInstanceFilter);
                Dictionary<string, JToken> sourceData = sourceHandler.GetSourceData();
                var sourceKeys = sourceData.Keys.ToList();
                sourceKeys.Sort();

                foreach (var item in sourceKeys)
                {
                    if (!sourceOutputs.ContainsKey(item))
                    {
                        sourceOutputs.Add(item, sourceData[item]);
                    }
                }
            }
            var mlHandlerInputs = handlerInputs.Where(a => a.SourceType == "MasterList").ToList();
            foreach (var handlerInput in mlHandlerInputs)
            {
                string docFilter = sourceContainer.RuleSources.Where(a => a.SourcePath.Split('[')[0] == handlerInput.RuleAlias).Select(sel => sel.SourceDocumentFilter).FirstOrDefault();
                if (String.IsNullOrEmpty(docFilter))
                {
                    IRuleSourceHandler sourceHandler = RuleSourceFactory.GetSourceHandler(folderVersionId, handlerInput.FormInstanceId, handlerInput, dbHandlerManager, sourceDesign.IsMasterList, requestContext, "");
                    Dictionary<string, JToken> sourceData = sourceHandler.GetSourceData();
                    var sourceKeys = sourceData.Keys.ToList();
                    sourceKeys.Sort();

                    foreach (var item in sourceKeys)
                    {
                        if (!sourceOutputs.ContainsKey(item))
                        {
                            sourceOutputs.Add(item, sourceData[item]);
                        }
                    }
                }
            }
            var mlFilterHandlerInputs = handlerInputs.Where(a => a.SourceType == "MasterList").ToList();
            foreach (var handlerInput in mlFilterHandlerInputs)
            {
                string docFilter = sourceContainer.RuleSources.Where(a => a.SourcePath.Split('[')[0] == handlerInput.RuleAlias).Select(sel => sel.SourceDocumentFilter).FirstOrDefault();
                if (!String.IsNullOrEmpty(docFilter))
                {
                    docFilter = GetDocumentFilter(folderVersionId, handlerInput.FormInstanceId, handlerInput.RuleAlias, dbHandlerManager, sourceDesign, requestContext, sourceOutputs);
                    IRuleSourceHandler sourceHandler = RuleSourceFactory.GetSourceHandler(folderVersionId, handlerInput.FormInstanceId, handlerInput, dbHandlerManager, sourceDesign.IsMasterList, requestContext, docFilter);
                    Dictionary<string, JToken> sourceData = sourceHandler.GetSourceData();
                    var sourceKeys = sourceData.Keys.ToList();
                    sourceKeys.Sort();
                    foreach (var item in sourceKeys)
                    {
                        if (!sourceOutputs.ContainsKey(item))
                        {
                            sourceOutputs.Add(item, sourceData[item]);
                        }
                    }
                }
            }

            Dictionary<string, JToken> FinalSourceOutputs = new Dictionary<string, JToken>();
            foreach (var item in sourceOutputs)
            {
                if (item.Value != null)
                    FinalSourceOutputs.Add(item.Key, item.Value);
            }

            return FinalSourceOutputs;
        }

        private string GetDocumentFilter(int folderVersionId, int formInstanceId, string ruleAlias,SourceHandlerDBManager dbHandlerManager , FormDesignVersionDetail sourceDesign, CurrentRequestContext requestContext, Dictionary<string,JToken> sourceOutputs)
        {
            string filter = "";
            //if document filter is true, get the list of keys from the system configuration repeater
            //find the first matching key, and get the instance name for the document
            SourceHandlerInput configHandlerInput = new SourceHandlerInput();
            configHandlerInput.FormInstanceId = formInstanceId;
            configHandlerInput.SourceType = "MasterList";
            configHandlerInput.RuleAlias = "SystemConfiguration";
            configHandlerInput.SourceSection = new List<SourceSection>();
            SourceSection section = new SourceSection();
            configHandlerInput.SourceSection.Add(section);
            section.SectionName = "MasterListSelectionRules";
            section.SourceInput = new List<SourceInput>();
            SourceInput input = new SourceInput();
            section.SourceInput.Add(input);
            input.SourceName = "MasterListSelection";
            List<string> sectionPaths = new List<string>();
            sectionPaths.Add("MasterListSelectionRules.MasterListSelection");
            input.SectionElementPaths = sectionPaths;
            IRuleSourceHandler sourceHandler = RuleSourceFactory.GetSourceHandler(folderVersionId, configHandlerInput.FormInstanceId, configHandlerInput, dbHandlerManager, true, requestContext, "");
            Dictionary<string, JToken> sourceData = sourceHandler.GetSourceData();

            //resolve keys
            ExpressionParser parser = new ExpressionParser(sourceOutputs);
            List<ExpressionTemplate> templates =  parser.ParseExpressions(sourceData[input.SourceName],ruleAlias);
            List<ExpressionTemplate> nonStaticTemplates = templates.Where(a => !a.TemplateExp.Trim().StartsWith("STATIC") && !a.TemplateExp.Trim().StartsWith("CUSTOM") && a.IsValid == true).ToList();
            List<ExpressionTemplate> staticTemplates = templates.Where(a => a.TemplateExp.Trim().StartsWith("STATIC")).ToList();
            List<string> expressions = new List<string>();
            if (nonStaticTemplates.Count > 0)
            {
                expressions = parser.EvaluateExpressions(nonStaticTemplates);
            }
            if(staticTemplates != null && staticTemplates.Count > 0)
            {
                expressions.Add("STATIC");
            }
            var results = sourceData[input.SourceName].Where(a => expressions.Contains(a["Key"].ToString()));
            if(results != null && results.Count() > 0)
            {
                var result = results.First();
                filter = result["InstanceName"].ToString();
                object val = System.Threading.Thread.GetData(System.Threading.Thread.GetNamedDataSlot("SBCOUNT"));
                if(val != null)
                {
                    int sbCount;
                    if (int.TryParse(val.ToString(), out sbCount) == true)
                    {
                        if(sbCount > 1)
                        {
                            filter = filter + "-" + sbCount.ToString();
                        }
                    }
                }
            }
            return filter;
        }
    }
}
