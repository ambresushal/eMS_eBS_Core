using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.ruleinterpreter.jsonhelper;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.sourcehandler;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;

namespace tmg.equinox.web.sourcehandler
{
    public class NonMasterListSourceHandler : IRuleSourceHandler
    {

        SourceHandlerInput _sourceHandlerInput;
        Dictionary<string, JToken> _sourceData;
        JObject _sectionData;
        string _ruleAlias;
        SourceHandlerDBManager _handlerDBManager;
        int _formInstanceId;
        CurrentRequestContext _requestContext;
		bool _isMasterList;
        public NonMasterListSourceHandler(int formInstanceId, SourceHandlerInput sourceHandlerInput, SourceHandlerDBManager handlerDBManager, bool isMasterList, CurrentRequestContext requestContext)
        {
            _sourceHandlerInput = sourceHandlerInput;
            _ruleAlias = sourceHandlerInput.RuleAlias;
            _handlerDBManager = handlerDBManager;
            _formInstanceId = formInstanceId;
            _requestContext = requestContext;
            _isMasterList = isMasterList;
        }

        public Dictionary<string, JToken> GetSourceData()
        {
            _sourceData = new Dictionary<string, JToken>();
            if(_sourceHandlerInput.SourceSection.Count == 1 && _sourceHandlerInput.SourceSection[0].SectionName == "ROOT")
            {
                _sourceData = GetFormInstanceData();
            }
            else
            {
                foreach (var item in _sourceHandlerInput.SourceSection)
                {
                    GetSectionData(item.SectionName, item.SourceInput);
                }
            }
            return _sourceData;
        }

        public Dictionary<string, JToken> GetFormInstanceData()
        {
            Dictionary<string, JToken> sourceData = new Dictionary<string, JToken>();
            string cacheKey = _formInstanceId + "-" + "ROOT";
                        
            if (_requestContext.RuleAliasesNonMasterListDataMaps.ContainsKey(cacheKey) == false)
            {
                JObject result = _handlerDBManager.GetCompleteFormData(_formInstanceId);    
                _requestContext.RuleAliasesNonMasterListDataMaps.Add(cacheKey, result);
                sourceData.Add(_ruleAlias, result);
            }
            else
            {
                sourceData.Add(_ruleAlias,_requestContext.RuleAliasesNonMasterListDataMaps[cacheKey]);
            }
            return sourceData;
        }

        private Dictionary<string, JToken> GetSectionData(string sectionName, List<SourceInput> sourceInput)
        {
            string key = _formInstanceId + "-" + sectionName;
            if (_requestContext.RuleAliasesNonMasterListDataMaps.ContainsKey(key) == false)
            {
                string sectionData = _handlerDBManager.GetSectionData(sectionName,_formInstanceId,_requestContext);
                _sectionData = (JObject)JObject.Parse((sectionData));
                _requestContext.RuleAliasesNonMasterListDataMaps.Add(key, _sectionData);
            }
            else
            {
                _sectionData = _requestContext.RuleAliasesNonMasterListDataMaps[key];
            }

            foreach (var item in sourceInput)
            {
                JToken sourceJToken = GetSource(item.SectionElementPaths);
                _sourceData.Add(item.SourceName, sourceJToken);
            }
            return _sourceData;
        }

        private JToken GetSource(List<string> sourceElements)
        {
            JToken sourceJToken = null;

            for (int i = 0; i < sourceElements.Count; i++)
            {
                if (i == 0 || (sourceJToken != null && sourceJToken.GetType() != typeof(JArray))) //TODO: Need to configured for more Generic Condition
                {
                    sourceJToken = GetSourceToken(_sectionData, sourceElements[i]);
                }
                else
                {
                    sourceJToken = GetChildJToken(sourceJToken.ToList(), sourceElements[i]);
                }
            }
            return sourceJToken;
        }

        private JToken GetChildJToken(List<JToken> parentJTokens, string tokenKey)
        {
            List<JToken> childJtokenList = new List<JToken>();
            JToken childJToken = null;

            foreach (JToken parentJToken in parentJTokens)
            {
                List<JToken> tokenResult = parentJToken.SelectToken(tokenKey).ToList();
                childJtokenList.AddRange(tokenResult);
            }
            childJToken = childJtokenList.ConvertJtokenListToJToken();
            return childJToken;
        }

        private JToken GetSourceToken(JToken sourceToken, string tokenKey)
        {
            if (tokenKey.EndsWith("."))
            {
                tokenKey = tokenKey.TrimEnd('.');
            }
            JToken sourceJToken = sourceToken.SelectToken(tokenKey.TrimEnd('.'));
            return sourceJToken;
        }
    }
}