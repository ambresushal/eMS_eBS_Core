using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using tmg.equinox.web.sourcehandler;
using tmg.equinox.ruleinterpreter.pathhelper;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class TargetSectionSyncManager
    {
        string _targetPath;
        JToken _ruleTargetData;
        SourceHandlerDBManager _sourceHandlerDBManager;
        int _FormInstanceId;
        CurrentRequestContext _requestContext;

        public TargetSectionSyncManager(string targetPath, int formInstanceId, JToken ruleTargetData, SourceHandlerDBManager sourceHandlerDBManager,CurrentRequestContext requestContext)
        {
            _targetPath = targetPath;
            _ruleTargetData = ruleTargetData;
            _sourceHandlerDBManager = sourceHandlerDBManager;
            _FormInstanceId = formInstanceId;
            _requestContext = requestContext;
        }

        public string GetUpdatedSection(string jsonData, RuleEventType eventType)
        {
            //Get section Inputs
            string sectionDataString = string.Empty;
            string sectionName = _targetPath.GetSectionName();
            if (eventType == RuleEventType.SECTIONLOAD)
            {
                sectionDataString = jsonData;
            }
            else
            {
                sectionDataString = _sourceHandlerDBManager.GetSectionData(sectionName, _FormInstanceId, _requestContext);

            }

            //parse section string to JObject
            JObject sectionData = (JObject)JObject.Parse((sectionDataString));

            //This will support updates only to the parent  and needs to enhance for childs
            string targetPathElement = _targetPath.GetElementPath();

            //Update section with Ruleprocess output fot the target path 
            sectionData.SelectToken(targetPathElement).Replace(_ruleTargetData);

            //serailized section data and return
            return JsonConvert.SerializeObject(sectionData);
        }
    }
}