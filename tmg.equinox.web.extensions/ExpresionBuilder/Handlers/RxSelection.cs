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
    public class RxSelectionRules
    {
        private FormDesignVersionDetail _formDetail;
        private FormInstanceDataManager _formDataInstanceManager;
        private int _formInstanceId;
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private int _folderVersionId;
        private JObject _sectionData;
        public RxSelectionRules(FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId)
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
            BasePlanChange();
        }

        private void BasePlanChange()
        {
            string planInfoData = _formDataInstanceManager.GetSectionData(_formInstanceId, COMMERCIALMEDICAL.ProductRules, false, _formDetail, false, false);
            JObject planInfoDataObj = JObject.Parse(planInfoData);
            string basePlan = COMMERCIALMEDICALGETSET.GetBasePlan(planInfoDataObj).ToString();
            JObject riderDataObj = new JObject();

            string sourceData = GetMasterListRider();
            List<JToken> rxRiderList = MASTERLISTMEDICALRIDERGETSET.GetRXRider(JObject.Parse(sourceData));

            if (rxRiderList != null && rxRiderList.Count > 0)
            {
                rxRiderList = rxRiderList.Where(a => a[MASTERLISTRIDEROPTION.BasePlan].ToString() == basePlan).Select(sel=>sel[MASTERLISTRIDEROPTION.RXridername]).Distinct().ToList();

                string[] rxRiderArry = COMMERCIALMEDICAL.SelectRxRiderPath.Split('.');

                List<SectionDesign> sectionDesign = _formDetail.Sections.Where(a => a.GeneratedName.ToString() == rxRiderArry[0].ToString()).ToList();
                List<ElementDesign> elment = sectionDesign[0].Elements.Where(a => a.GeneratedName.ToString() == rxRiderArry[1].ToString()).ToList();
                for (int i = 2; i < rxRiderArry.Length; i++)
                {
                    if (elment.Count != 0)
                    {
                        elment = elment[0].Section.Elements.Where(a => a.GeneratedName.ToString() == rxRiderArry[i].ToString()).ToList();
                    }
                }
                
                List<Item> riderItem = new List<Item>();

                foreach (JToken rxRider in rxRiderList)
                {
                    Item obj = new Item();
                    obj.ItemID = 0;
                    obj.Enabled = "";
                    obj.ItemText = rxRider.ToString();
                    obj.ItemValue = rxRider.ToString();
                    riderItem.Add(obj);
                }

                if (riderItem.Count > 0)
                {
                    elment[0].Items = riderItem;
                }
            }
            
        }

        public string GetMasterListRider()
        {
            int sourceFormInstanceID = this._folderVersionServices.GetSourceFormInstanceID(_formInstanceId, _formDetail.FormDesignVersionId, _folderVersionId, ExpressionBuilderConstants.COMMERCIALRIDEROPTION);
            string sourceFormInstanceData = "";

            if (sourceFormInstanceID != 0)
            {
                int sourceformDesignVersionId = this._folderVersionServices.GetSourceFormDesignVersionId(sourceFormInstanceID);
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(_formDetail.TenantID, sourceformDesignVersionId, _formDesignServices);
                FormDesignVersionDetail sourceDetail = formDesignVersionMgr.GetFormDesignVersion(true);
                sourceFormInstanceData = _formDataInstanceManager.GetSectionData(sourceFormInstanceID, MASTERLISTRIDEROPTION.RxRiderOption, false, sourceDetail, false, true);
            }
            return sourceFormInstanceData;
        }
    }
}