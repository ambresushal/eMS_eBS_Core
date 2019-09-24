using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.forminstanceprocessor.expressionbuilder;

namespace tmg.equinox.expressionbuilder
{
    public static class RuleSourceFactory
    {
        public static IRuleSourceHandler GetSourceHandler(int folderVerionId, int formInstanceId, SourceHandlerInput sourceHandlerInput, SourceHandlerDBManager sourceHandlerManager, FormDesignVersionDetail sourceDesign, CurrentRequestContext requestContext)
        {
            IRuleSourceHandler sourceHandlerObject = null;
            SourceType sourceType = GetSourceType(formInstanceId,sourceHandlerInput.RuleAlias, sourceHandlerManager, requestContext);

            switch (sourceType)
            {
                case SourceType.MasterList:
                    sourceHandlerObject = new MasterListSourceHandler(folderVerionId, formInstanceId, sourceHandlerInput, sourceHandlerManager, sourceDesign,requestContext);
                    break;
                case SourceType.NonMasterList:
                    sourceHandlerObject = new NonMasterListSourceHandler(formInstanceId, sourceHandlerInput, sourceHandlerManager,sourceDesign,requestContext);
                    break;
                default:
                    break;
            }
            return sourceHandlerObject;
        }

        public static SourceType GetSourceType(int formInstanceID,string ruleAlias, SourceHandlerDBManager sourceHandlerManager,CurrentRequestContext requestContext)
        {
            SourceType type = SourceType.None;
            if (requestContext.RuleAliasesMasterListDataMaps.ContainsKey(ruleAlias) == true)
            {
                type = SourceType.MasterList;
            }
            if(type == SourceType.None)
            {
                foreach(string key in requestContext.RuleAliasesNonMasterListDataMaps.Keys)
                {
                    string formInstance = key.Split('-')[0];
                    if(formInstance ==  formInstanceID.ToString())
                    {
                        type = SourceType.NonMasterList;
                        break;
                    }
                }
            }
            if(type == SourceType.None)
            {
                type = sourceHandlerManager.IsMasterList(ruleAlias) ? SourceType.MasterList : SourceType.NonMasterList;
            }
            return type;
        }
    }
}