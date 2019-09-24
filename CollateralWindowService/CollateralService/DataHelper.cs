using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.CollateralWindowService;
using tmg.equinox.dependencyresolution;
using tmg.equinox.web.CollateralHelper;

namespace CollateralService
{
    public class DataHelper
    {
        private int _userId = 1228;
        private string _userName = "superuser";
        private IUIElementService _uiElementService;
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceService _formInstanceService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IMasterListService _masterListService;
        private int _sbFormDesignVersionID;

        public DataHelper(int formDesignVersionID)
        {
            _uiElementService = UnityConfig.Resolve<IUIElementService>();
            _formDesignService = UnityConfig.Resolve<IFormDesignService>();
            _folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
            _formInstanceService = UnityConfig.Resolve<IFormInstanceService>();
            _formInstanceDataService = UnityConfig.Resolve<IFormInstanceDataServices>();
            _masterListService = UnityConfig.Resolve<IMasterListService>();
            _sbFormDesignVersionID = formDesignVersionID;
        }

        public string ProcessSBDesign(CollateralProcessQueueViewModel processQueue, List<SBCollateralProcessQueueViewModel> sbCollateralQueue)
        {
            List<SBConfig> configuraiton = GetSBConfiguration(processQueue, sbCollateralQueue);
            string formData = this.GetSBDesignJSONData(configuraiton);
            return formData;
        }

        private List<SBConfig> GetSBConfiguration(CollateralProcessQueueViewModel processQueue, List<SBCollateralProcessQueueViewModel> sbCollateralQueue)
        {
            List<SBConfig> configs = new List<SBConfig>();

            //Add base document
            configs.Add(new SBConfig() { FormInstanceID = Convert.ToInt32( processQueue.FormInstanceID), FolderVersionID = Convert.ToInt32(processQueue.FolderVersionID) });

            //Add additional anchor documents
            foreach (var document in sbCollateralQueue)
            {
                configs.Add(new SBConfig() { FormInstanceID = document.FormInstanceID, FolderVersionID = document.FolderVersionID });
            }

            return configs;
        }

        private string GetSBDesignJSONData(List<SBConfig> configuraiton)
        {
            SBDesignDataHelper dataHelper = new SBDesignDataHelper(configuraiton[0].FormInstanceID, configuraiton[0].FolderVersionID, _sbFormDesignVersionID, _userId, _userName,
                       _uiElementService, _formDesignService, _folderVersionService, _formInstanceService, _formInstanceDataService, _masterListService);
            string sbDesingJsonData = dataHelper.ProcessSBDesign(configuraiton);
            return sbDesingJsonData;
        }
    }
}
