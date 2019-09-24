using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.CustomRule;

using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

using tmg.equinox.web.Framework.MasterList;


namespace tmg.equinox.web.Controllers
{
    public class CustomRuleController : AuthenticatedController
    {
        #region Private Members
        private IFolderVersionServices _folderVersionServices;
        private ICustomRuleService _customRuleServices;
        private IFormDesignService _formDesignServices;
        private IMasterListService _masterListService;
        #endregion Private Members

        #region Constructor
        public CustomRuleController(IFolderVersionServices folderVersionServices, ICustomRuleService customRuleServices, IFormDesignService formDesignServices, IMasterListService masterListService)
        {
            this._folderVersionServices = folderVersionServices;
            this._customRuleServices = customRuleServices;
            this._formDesignServices = formDesignServices;
            this._masterListService = masterListService;
        }
        #endregion Constructor

    }
}