using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.web.sourcehandler;

namespace tmg.equinox.web.sourcehandler
{
    public static class RuleSourceFactory
    {
        public static IRuleSourceHandler GetSourceHandler(int folderVerionId, int formInstanceId, SourceHandlerInput sourceHandlerInput, SourceHandlerDBManager sourceHandlerManager, bool isMasterList, CurrentRequestContext requestContext,string mlDocumentFilter)
        {
            IRuleSourceHandler sourceHandlerObject = null;
            //SourceType sourceType = GetSourceType(formInstanceId,sourceHandlerInput.RuleAlias, sourceHandlerManager, requestContext);

            switch (sourceHandlerInput.SourceType)
            {
                case "MasterList":
                    sourceHandlerObject = new MasterListSourceHandler(folderVerionId, formInstanceId, sourceHandlerInput, sourceHandlerManager, isMasterList,requestContext, mlDocumentFilter);
                    break;
                case "NonMasterList":
                    sourceHandlerObject = new NonMasterListSourceHandler(formInstanceId, sourceHandlerInput, sourceHandlerManager,isMasterList,requestContext);
                    break;
                default:
                    break;
            }
            return sourceHandlerObject;
        }

        public static SourceType GetSourceType(int formInstanceID,string ruleAlias, SourceHandlerDBManager sourceHandlerManager,CurrentRequestContext requestContext)
        {
            SourceType type = SourceType.None;
            foreach (string key in requestContext.RuleAliasesMasterListDataMaps.Keys)
            {
                string ruleAliasPart = key.Split('-')[0];
                if (ruleAlias == ruleAliasPart)
                {
                    type = SourceType.MasterList;
                    break;
                }
            }
            if (type == SourceType.None)
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