using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.CustomRule;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.ExpresionBuilder
{
    public class CascadingCostShare
    {
        private FormDesignVersionDetail _formDetail;
        private FormInstanceDataManager _formDataInstanceManager;
        private int _formInstanceId;
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private int _folderVersionId;
        private JObject _sectionData;
        public CascadingCostShare(FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId)
        {

            this._formDataInstanceManager = formDataInstanceManager;
            this._formInstanceId = formInstanceId;
            this._formDetail = detail;
            this._sectionData = JObject.Parse(detail.JSONData);
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._folderVersionId = folderVersionId;
        }

        public void RunOnSectionOnLoad()
        {
            filterNetworkTierDropdown();
        }

        private void filterNetworkTierDropdown()
        {
            string networkSectionData = _formDataInstanceManager.GetSectionData(_formInstanceId, COMMERCIALMEDICAL.Network, false, _formDetail, false, false);

            JObject netObj = JObject.Parse(networkSectionData);
            List<JToken> networkList = netObj.SelectToken("Network.NetworkTierList").ToList();

            SectionDesign sectionDesign = _formDetail.Sections.Where(a => a.GeneratedName.ToString() == COMMERCIALMEDICAL.CascadingCostShare).FirstOrDefault();
            ElementDesign costSharSection = sectionDesign.Elements.Where(a => a.GeneratedName.ToString() == "CostShareGroup").FirstOrDefault();
            
            ElementDesign sameAsNetworkTierElem = costSharSection.Section.Elements[0].Repeater.Elements.Where(a => a.GeneratedName == "SameAsNetworkTier").FirstOrDefault();
           
            List<Item> ddItemList = new List<Item>();

            foreach (JToken option in networkList)
            {
                Item obj = new Item();
                obj.ItemID = 0;
                obj.Enabled = "";
                obj.ItemText = option[COMMERCIALMEDICAL.NetworkTier].ToString();
                obj.ItemValue = option[COMMERCIALMEDICAL.NetworkTier].ToString();
                ddItemList.Add(obj);
            }

            if (ddItemList.Count > 0)
            {
                Item obj = new Item();
                obj.ItemID = 0;
                obj.Enabled = "";
                obj.ItemText = "Not Applicable";
                obj.ItemValue = "Not Applicable";
                ddItemList.Add(obj);
                sameAsNetworkTierElem.Items = ddItemList;
            }
        }
    }
}