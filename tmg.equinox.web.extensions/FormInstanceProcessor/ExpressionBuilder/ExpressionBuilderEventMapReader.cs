using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.rulecompiler;
using tmg.equinox.web.DataSource;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class ExpressionBuilderEventMapReader
    {
        IUIElementService _uiElementService;
        CompiledDcoumentCacheHandler compiledDocumentHandler;
        private int _tenantId;
        private int _formDesignVersionId;
        private IFormDesignService _formDesignService;
        private ExpressionBuilderEventMapCacheHandler _eventMapHandler;

        public ExpressionBuilderEventMapReader(int tenantId, int formDesignVersionID, IUIElementService uiElementService, IFormDesignService formDesignService)
        {
            this._tenantId = tenantId;
            this._formDesignVersionId = formDesignVersionID;
            this._formDesignService = formDesignService;
            this._eventMapHandler = new ExpressionBuilderEventMapCacheHandler();
            _uiElementService = uiElementService;
            this.compiledDocumentHandler = new CompiledDcoumentCacheHandler();
        }

        // Get Rules for the input section from UI.FormDesignVersion 
        //This will return multiple rules associated with input section
        public List<JToken> GetSourceSectionRules(string sectionName)
        {
            List<JObject> rule = new List<JObject>();
            List<JToken> sectionrules = new List<JToken>();
            string eventMapJSON = _eventMapHandler.IsExists(_formDesignVersionId);
            if (eventMapJSON == null || eventMapJSON == "")
            {
                eventMapJSON = this._formDesignService.GetEventMapJSON(_tenantId, _formDesignVersionId);
                _eventMapHandler.Add(_formDesignVersionId, eventMapJSON);
            }
            if (eventMapJSON != null && eventMapJSON != "")
            {
                JObject eventMapJObject = JObject.Parse(eventMapJSON);

                if (eventMapJObject.SelectToken("documentrules.sourcesectionrules") != null)
                {
                    sectionrules = ((JArray)eventMapJObject["documentrules"]["sourcesectionrules"]).Where(a => a.Count() > 0).ToList();
                    sectionrules = sectionrules.Where(a => a["section"].ToString() == sectionName).ToList();
                    if (sectionrules != null && sectionrules.Count > 0)
                    {
                        sectionrules = ((JArray)sectionrules[0]["rules"]).ToList();
                    }
                }
            }
            return sectionrules;
        }

        public List<JToken> GetDocumentRules()
        {
            List<JObject> rule = new List<JObject>();
            List<JToken> documentRules = new List<JToken>();
            string eventMapJSON = _eventMapHandler.IsExists(_formDesignVersionId);
            if (eventMapJSON == null || eventMapJSON == "")
            {
                eventMapJSON = this._formDesignService.GetEventMapJSON(_tenantId, _formDesignVersionId);
                _eventMapHandler.Add(_formDesignVersionId, eventMapJSON);
            }
            if (eventMapJSON != null && eventMapJSON != "")
            {
                JObject eventMapJObject = JObject.Parse(eventMapJSON);

                if (eventMapJObject.SelectToken("documentrules.targetsectionrules") != null)
                {
                    documentRules = ((JArray)eventMapJObject["documentrules"]["targetsectionrules"]).Where(a => a.Count() > 0).ToList();
                }
            }
            return documentRules;
        }

        public List<JToken> GetTargetSectionRules(string sectionName)
        {
            List<JObject> rule = new List<JObject>();
            string eventMapJSON = _eventMapHandler.IsExists(_formDesignVersionId);
            if (eventMapJSON == null || eventMapJSON == "")
            {
                eventMapJSON = this._formDesignService.GetEventMapJSON(_tenantId, _formDesignVersionId);
                _eventMapHandler.Add(_formDesignVersionId, eventMapJSON);
            }

            List<JToken> sectionrules = new List<JToken>();
            if (eventMapJSON != null && eventMapJSON != "")
            {
                JObject eventMapJObject = JObject.Parse(eventMapJSON);
                if (eventMapJObject.SelectToken("documentrules.sourcesectionrules") != null)
                {
                    sectionrules = ((JArray)eventMapJObject["documentrules"]["targetsectionrules"]).Where(a => a.Count() > 0).ToList();
                    sectionrules = sectionrules.Where(a => a["section"].ToString() == sectionName).ToList();
                    if (sectionrules != null && sectionrules.Count > 0)
                    {
                        sectionrules = ((JArray)sectionrules[0]["rules"]).ToList();
                    }

                }
            }
            return sectionrules;
        }

        public List<JToken> GetTargetSectionRules()
        {
            List<JObject> rule = new List<JObject>();
            string eventMapJSON = _eventMapHandler.IsExists(_formDesignVersionId);
            if (eventMapJSON == null || eventMapJSON == "")
            {
                eventMapJSON = this._formDesignService.GetEventMapJSON(_tenantId, _formDesignVersionId);
                _eventMapHandler.Add(_formDesignVersionId, eventMapJSON);
            }

            List<JToken> sectionrules = new List<JToken>();
            if (eventMapJSON != null && eventMapJSON != "")
            {
                JObject eventMapJObject = JObject.Parse(eventMapJSON);
                if (eventMapJObject.SelectToken("documentrules.sourcesectionrules") != null)
                {
                    sectionrules = ((JArray)eventMapJObject["documentrules"]["targetsectionrules"]).Where(a => a.Count() > 0).ToList();
                }
            }
            return sectionrules;
        }


        public CompiledDocumentRule GetCompiledRule(int documentRuleId)
        {
            string compiledJSON = string.Empty;
            compiledJSON = compiledDocumentHandler.IsExists(documentRuleId);

            if (compiledJSON == string.Empty || compiledJSON == null)
            {
                compiledJSON = _uiElementService.GetDocumentRule(documentRuleId);
                compiledDocumentHandler.Add(documentRuleId, compiledJSON);

            }
            CompiledDocumentRule compileDocumentRule = null;
            if (compiledJSON != null)
            {
                compileDocumentRule = DocumentRuleSerializer.DeseralizedToCompiledRule(compiledJSON);
            }
            return compileDocumentRule;
        }
    }
}