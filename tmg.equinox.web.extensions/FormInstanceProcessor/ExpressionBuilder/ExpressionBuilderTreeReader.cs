﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.DataSource;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class ExpressionBuilderTreeReader
    {
        private int _tenantId;
        private IFormDesignService _formDesignService;
        private int _formDesignVersionID;
        private ExpressionBuilderTreeCacheHandler _treeHandler;
        public ExpressionBuilderTreeReader(int tenantId, int formDesignVersionID, IUIElementService uiElementService, IFormDesignService formDesignService)
        {
            this._formDesignVersionID = formDesignVersionID;
            this._tenantId = tenantId;
            this._formDesignService = formDesignService;
            this._treeHandler = new ExpressionBuilderTreeCacheHandler();
        }

        public JToken GetRuleTree(int ruleID)
        {
            string ruleTreeJSON = _treeHandler.IsExists(_formDesignVersionID);            
            if (ruleTreeJSON == null || ruleTreeJSON == "")
            {
                ruleTreeJSON = this._formDesignService.GetExecutionTreeJSON(_tenantId, _formDesignVersionID);
                _treeHandler.Add(_formDesignVersionID, ruleTreeJSON);
            }

            JToken rule = "";
            if (ruleTreeJSON != null && ruleTreeJSON != "")
            {
                JObject ruleTreeJObject = JObject.Parse(ruleTreeJSON);

                List<JToken> ruleTree = ((JArray)ruleTreeJObject["rule"]).ToList();
                rule = ruleTree.Where(a => a["id"].ToString() == ruleID.ToString()).FirstOrDefault();
            }
            return rule;
        }
    }
}