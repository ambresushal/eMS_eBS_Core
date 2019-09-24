using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.ExpresionBuilder.Handlers
{
    public class AncillarySelectionRules
    {
        private FormDesignVersionDetail _formDetail;
        private FormInstanceDataManager _formDataInstanceManager;
        private int _formInstanceId;
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private int _folderVersionId;
        private JObject _sectionData;
        public AncillarySelectionRules(FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId)
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

            List<DVHPackages> dvhObjectList = new List<Handlers.DVHPackages>();
            dvhObjectList.Add(new Handlers.DVHPackages { mlPackageName = "Dental", mlSectionName = "DentalPackages", mlBasePlanAssociationName = "DentalPackageBasePlanAssociation", mlColumnName = "DentalPackageName", anchorDDPath = "AncillarySelection.Dental.SelectDentalPackage" });
            dvhObjectList.Add(new Handlers.DVHPackages { mlPackageName = "Vision", mlSectionName = "VisionPackages", mlBasePlanAssociationName = "VisionPackageBasePlanAssociation", mlColumnName = "VisionPackageName", anchorDDPath = "AncillarySelection.Vision.SelectVisionPackage" });
            dvhObjectList.Add(new Handlers.DVHPackages { mlPackageName = "Hearing", mlSectionName = "HearingPackages", mlBasePlanAssociationName = "HearingPackageBasePlanAssociation", mlColumnName = "HearingPackageName", anchorDDPath = "AncillarySelection.Hearing.SelectHearingPackage" });

            foreach (DVHPackages dvhObject in dvhObjectList)
            {
                string sourceData = GetMasterListSectionData(dvhObject.mlSectionName);
                List<JToken> packageOptionList = MASTERLISTMEDICALRIDERGETSET.GetDVHPackage(JObject.Parse(sourceData), dvhObject.mlSectionName, dvhObject.mlBasePlanAssociationName);

                if (packageOptionList != null && packageOptionList.Count > 0)
                {
                    packageOptionList = packageOptionList.Where(a => a[MASTERLISTRIDEROPTION.BasePlan].ToString() == basePlan).Select(sel => sel[dvhObject.mlColumnName]).Distinct().ToList();

                    string[] ddPathArry = dvhObject.anchorDDPath.Split('.');

                    List<SectionDesign> sectionDesign = _formDetail.Sections.Where(a => a.GeneratedName.ToString() == ddPathArry[0].ToString()).ToList();
                    List<ElementDesign> elment = sectionDesign[0].Elements.Where(a => a.GeneratedName.ToString() == ddPathArry[1].ToString()).ToList();
                    for (int i = 2; i < ddPathArry.Length; i++)
                    {
                        if (elment.Count != 0)
                        {
                            elment = elment[0].Section.Elements.Where(a => a.GeneratedName.ToString() == ddPathArry[i].ToString()).ToList();
                        }
                    }

                    List<Item> ddItemList = new List<Item>();

                    foreach (JToken option in packageOptionList)
                    {
                        Item obj = new Item();
                        obj.ItemID = 0;
                        obj.Enabled = "";
                        obj.ItemText = option.ToString();
                        obj.ItemValue = option.ToString();
                        ddItemList.Add(obj);
                    }

                    if (ddItemList.Count > 0)
                    {
                        elment[0].Items = ddItemList;
                    }
                }
            }
        }

        public string GetMasterListSectionData(string sectionName)
        {
            int sourceFormInstanceID = this._folderVersionServices.GetSourceFormInstanceID(_formInstanceId, _formDetail.FormDesignVersionId, _folderVersionId, ExpressionBuilderConstants.COMMERCIALRIDEROPTION);
            string sourceFormInstanceData = "";

            if (sourceFormInstanceID != 0)
            {
                int sourceformDesignVersionId = this._folderVersionServices.GetSourceFormDesignVersionId(sourceFormInstanceID);
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(_formDetail.TenantID, sourceformDesignVersionId, _formDesignServices);
                FormDesignVersionDetail sourceDetail = formDesignVersionMgr.GetFormDesignVersion(true);
                sourceFormInstanceData = _formDataInstanceManager.GetSectionData(sourceFormInstanceID, sectionName, false, sourceDetail, false, true);
            }
            return sourceFormInstanceData;
        }
    }

    public class DVHPackages
    {
        public string mlPackageName { get; set; }
        public string mlSectionName { get; set; }
        public string mlBasePlanAssociationName { get; set; }
        public string mlColumnName { get; set; }
        public string anchorDDPath { get; set; }
    }
}
