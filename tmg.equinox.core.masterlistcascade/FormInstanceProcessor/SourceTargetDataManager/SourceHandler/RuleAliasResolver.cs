using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.caching;
using tmg.equinox.ruleprocessor.formdesignmanager;
using tmg.equinox.rulesprocessor;

namespace tmg.equinox.expressionbuilder
{
    public class RuleAliasResolver
    {
        private int _currentDocumentID { get; set; }
        private string _currentDocumentName { get; set; }
        private int _folderVersionID { get; set; }
        private IFormInstanceService _formInstanceService { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private FormInstanceSectionDataCacheHandler _handler;
        private int? _userId;
        Dictionary<string, bool> formDesignList = new Dictionary<string, bool>();
        public RuleAliasResolver(string formName, int formInstanceID, int folderVersionID, IFormInstanceService formInstanceService, IFormDesignService formDesignService,int? userid)
        {
            this._currentDocumentName = formName;
            this._currentDocumentID = formInstanceID;
            this._folderVersionID = folderVersionID;
            this._formInstanceService = formInstanceService;
            this._formDesignService = formDesignService;
            _handler = new FormInstanceSectionDataCacheHandler();
            this._userId = userid;
        }

        public int Resolve(string ruleAlias)
        {
            int formInstanceID = 0;

            if (string.Equals(ruleAlias, this._currentDocumentName, StringComparison.OrdinalIgnoreCase))
            {
                formInstanceID = this._currentDocumentID;
            }
            else
            {
                var design = FormDesignGroupMapManager.Get(ruleAlias, _formDesignService);
                if (!design.AllowMultiple)
                {
                    formInstanceID = _formInstanceService.GetDocumentID(this._folderVersionID, design.FormDesignID);
                }
                else if (design.ParentFormDesignID != 0)
                {
                    formInstanceID = _formInstanceService.GetViewDocumentID(this._currentDocumentID, design.FormDesignID);
                }
                else
                {
                    formInstanceID = _formInstanceService.GetAnchorDocumentID(this._currentDocumentID);
                }

                if (formInstanceID != 0)
                {
                    _handler.AddTargetFormInstanceIdToCache(_currentDocumentID, formInstanceID, _userId);
                }

            }

            return formInstanceID;
        }

        public bool IsMasterListFormDesign(string formName)
        {
            formDesignList = FormDesignsManager.GetFormDesignList(1, _formDesignService);
            if (formDesignList.Count > 0 && formDesignList.ContainsKey(formName))
            {
               bool isMasterList = formDesignList[formName];
               return isMasterList;               
            }
            return false;
        }
    }
}