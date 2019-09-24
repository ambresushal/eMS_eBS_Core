using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.jsonhelper;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.forminstanceprocessor.expressionbuilder;

namespace tmg.equinox.expressionbuilder
{
    public class MasterListSourceHandler : IRuleSourceHandler
    {
        SourceHandlerInput _sourceHandlerInput;
        Dictionary<string, JToken> _sourceData;
        JObject _sectionData;
        int _formInstanceId;
        int _folderVersionId;
        string _ruleAlias;
        CurrentRequestContext _requestContext;
        SourceHandlerDBManager _handlerDBManager;
        public MasterListSourceHandler(int folderVersionId, int formInstanceId, SourceHandlerInput sourceHandlerInput, SourceHandlerDBManager handlerDBManager, FormDesignVersionDetail sourceDesign, CurrentRequestContext requestContext)
        {
            _sourceHandlerInput = sourceHandlerInput;
            _folderVersionId = folderVersionId;
            _ruleAlias = sourceHandlerInput.RuleAlias;
            _handlerDBManager = handlerDBManager;
            _formInstanceId = formInstanceId;
            _requestContext = requestContext;
            GetMasterListSections(sourceDesign);
        }

        public Dictionary<string, JToken> GetSourceData()
        {
            _sourceData = new Dictionary<string, JToken>();

            foreach (var item in _sourceHandlerInput.SourceSection)
            {
                GetSectionData(item.SectionName, item.SourceInput);
            }
            return _sourceData;
        }

        private void GetSectionData(string sectionName, List<SourceInput> sourceInput)
        {
            foreach (var item in sourceInput)
            {
                JToken sourceJToken = GetSource(item.SectionElementPaths);
                _sourceData.Add(item.SourceName, sourceJToken);
            }
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
            JToken sourceJToken = sourceToken.SelectToken(tokenKey);
            return sourceJToken;
        }

        private void GetMasterListSections(FormDesignVersionDetail sourceDesign)
        {
            List<string> sections = _sourceHandlerInput.SourceSection.Select(sel => sel.SectionName).ToList();
            string ruleAlias = _sourceHandlerInput.RuleAlias;
            List<string> newSections = new List<string>();
            if (_requestContext.RuleAliasesMasterListDataMaps.ContainsKey(ruleAlias) == false) 
            {
                _requestContext.RuleAliasesMasterListDataMaps.Add(ruleAlias, new Dictionary<string, JToken>()); 
            }
            Dictionary<string, JToken> ruleAliasDictionary = _requestContext.RuleAliasesMasterListDataMaps[ruleAlias];
            _sectionData = new JObject();
            foreach (string section in sections)
            {
                if (ruleAliasDictionary.ContainsKey(section) == false)
                {
                    newSections.Add(section);
                }
                else 
                {
                    _sectionData.Add(section, ruleAliasDictionary[section]);
                }   
            }
            if (newSections.Count > 0)
            {
                JObject sectionArray = _handlerDBManager.GetMasterListData(_folderVersionId, newSections, ruleAlias, sourceDesign);
                foreach (var item in sectionArray.Children())
                {
                    JProperty sectionProp = ((JProperty)item);
                    ruleAliasDictionary.Add(sectionProp.Name, sectionProp.Value);
                    _sectionData.Add(sectionProp.Name, sectionProp.Value);
                }
            }
        }
    }
}