using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.FormInstanceProcessor.SourceTargetDataManager.SourceHandler
{
    public class RuleAliasResolver
    {
        private int _currentDocumentID { get; set; }

        private int _currentDocumentDesignID { get; set; }
        private string _currentDocumentName { get; set; }
        private int _folderVersionID { get; set; }
        private IFormInstanceService _formInstanceService { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private FormInstanceSectionDataCacheHandler _handler;
        private int? _userId;
        Dictionary<string, bool> formDesignList = new Dictionary<string, bool>();
        private FormDesignGroupMapModel _currentDesign;
        public RuleAliasResolver(string formName, int formInstanceID, int formDesignID, int folderVersionID, IFormInstanceService formInstanceService, IFormDesignService formDesignService, int? userid)
        {
            this._currentDocumentName = formName;
            this._currentDocumentID = formInstanceID;
            this._currentDocumentDesignID = formDesignID;
            this._folderVersionID = folderVersionID;
            this._formInstanceService = formInstanceService;
            this._formDesignService = formDesignService;
            _handler = new FormInstanceSectionDataCacheHandler();
            _currentDesign = FormDesignGroupMapManager.Get(formName, _formDesignService);
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
                if (design.IsMasterList == false)
                {
                    if (design.ParentFormDesignID != 0)
                    {
                        int anchorInstanceID = _formInstanceService.GetAnchorDocumentID(this._currentDocumentID);
                        formInstanceID = _formInstanceService.GetViewDocumentID(anchorInstanceID, design.FormDesignID);
                    }
                    else if (!design.AllowMultiple)
                    {
                        formInstanceID = _formInstanceService.GetDocumentID(this._folderVersionID, design.FormDesignID);
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
            }
            return formInstanceID;
        }

        public List<int> ResolveMultiple(string ruleAlias)
        {
            List<int> formInstanceIds = new List<int>();

            var design = FormDesignGroupMapManager.Get(ruleAlias, _formDesignService);
            if (design.IsMasterList == false)
            {
                if (design.ParentFormDesignID != 0)
                {
                    int anchorInstanceID = _formInstanceService.GetAnchorDocumentID(this._currentDocumentID);
                    formInstanceIds = _formInstanceService.GetViewDocumentIDs(anchorInstanceID, design.FormDesignID);
                }
                if (formInstanceIds.Count > 0)
                {
                    foreach(var formInstanceId in formInstanceIds)
                    {
                        _handler.AddTargetFormInstanceIdToCache(_currentDocumentID, formInstanceId, _userId);
                    }
                }
            }
            return formInstanceIds;
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