using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;

namespace tmg.equinox.web.sourcehandler
{
    public  class RuleTargetDataAdapter
    {
        public JToken GetTargetJSONToken(int folderVersionId, Dictionary<string, int> ruleAliasFormInstances, string targetPath, SourceHandlerDBManager sourceHandlerManager, FormDesignVersionDetail sourceDesign, CurrentRequestContext requestContext)
        {
            Dictionary<string, string> targetPartDic = new Dictionary<string, string>();
            targetPartDic.Add(targetPath, targetPath);

            JToken targetToken = null;
            SourceGroupingWrapper sourceGrouping = new SourceGroupingWrapper(targetPartDic, ruleAliasFormInstances);
            List<SourceHandlerInput> handlerInputs = sourceGrouping.GetSourceHandlerInput();

            foreach (var handlerInput in handlerInputs)
            {
                SourceType sourceType = RuleSourceFactory.GetSourceType(handlerInput.FormInstanceId, handlerInput.RuleAlias, sourceHandlerManager, requestContext);
                if (sourceType == SourceType.MasterList)
                {
                    handlerInput.SourceType = "MasterList";
                }
                else
                {
                    handlerInput.SourceType = "NonMasterList";
                }
                IRuleSourceHandler sourceHandler = RuleSourceFactory.GetSourceHandler(folderVersionId, handlerInput.FormInstanceId, handlerInput, sourceHandlerManager, sourceDesign.IsAliasDesignMasterList,requestContext,"");
                Dictionary<string, JToken> sourceData = sourceHandler.GetSourceData();
                targetToken = sourceData.First().Value;
            }
            return targetToken;
        }
    }
}
