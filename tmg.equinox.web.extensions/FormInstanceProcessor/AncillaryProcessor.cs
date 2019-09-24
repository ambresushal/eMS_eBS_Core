using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class AncillaryProcessor
    {
        private FormDesignVersionDetail _detail;
        private FormInstanceDataManager _formDataInstanceManager;
        private string _sectionName;
        private int _formInstanceId;
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private int _folderVersionId;
        private int _ancillaryDesignId = 0;
        private int _ancillaryFormDesignVersionId = 0;
        private int _ancillaryFormInstanceId = 0;
        private string _ancillaryProductName;
        private string _FolderName;
        private string _FolderEffectiveDate;
        private string _FolderVersionNumber;
        public AncillaryProcessor(FormDesignVersionDetail detail, FormInstanceDataManager formDataInstanceManager, string sectionName, int formInstanceId, int folderVersionId, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService, string ancillaryProductName)
        {
            this._formDataInstanceManager = formDataInstanceManager;
            this._sectionName = sectionName;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
            this._folderVersionId = folderVersionId;
            this._ancillaryProductName = ancillaryProductName;
        }
        public bool IsAncillarySection()
        {
            IEnumerable<FormDesignRowModel> anchorList = _formDesignServices.GetAnchorDesignList(1);

            foreach (FormDesignRowModel anchor in anchorList)
            {
                if (anchor.FormDesignName == _sectionName)
                {
                    _ancillaryDesignId = anchor.FormDesignId;
                    return true;
                }
            }
            return false;
        }
        public void GetAncillaryProductList()
        {
            DateTime dtObj = new DateTime();

            List<FormInstanceViewModel> productList = _folderVersionServices.GetAncillaryProductList(_folderVersionId, _sectionName);

            SectionDesign sectionDesign = _detail.Sections.Where(a => a.GeneratedName == _sectionName).FirstOrDefault();

            if (sectionDesign != null)
            {
                ElementDesign secElm = sectionDesign.Elements.Where(a => a.GeneratedName.ToString() == _sectionName + "ProductSelection").FirstOrDefault();

                ElementDesign elm = secElm.Section.Elements.Where(a => a.GeneratedName.ToString() == "Select" + _sectionName + "Product").FirstOrDefault();

                foreach (FormInstanceViewModel anchorProd in productList)
                {
                    if (anchorProd.Name == "") { continue; }
                    Item itm = new Item();
                    itm.ItemText = anchorProd.Name;
                    itm.ItemValue = anchorProd.Name;
                    elm.Items.Add(itm);
                    if (_ancillaryProductName == anchorProd.Name)
                    {
                        _ancillaryFormInstanceId = anchorProd.FormInstanceID;
                        _ancillaryFormDesignVersionId = anchorProd.FormDesignVersionID;
                        _FolderName = anchorProd.FolderName;
                        dtObj = anchorProd.FolderEffectiveDate;
                        _FolderEffectiveDate = dtObj.ToShortDateString();
                        _FolderVersionNumber = anchorProd.FolderVersionNumber;
                    }
                }
            }

            if (_ancillaryProductName != null && _ancillaryProductName != "" && _ancillaryProductName != "undefined")
            {
                GetAncilarySectionData();
            }
        }
        public void GetAncilarySectionData()
        {

            if (_ancillaryProductName == "[Select One]")
            {
                _detail.JSONData = _detail.GetDefaultJSONDataObject();
                JObject formData = JObject.Parse(_detail.JSONData);
                formData.SelectToken(_sectionName + "." + _sectionName + "ProductSelection.Select" + _sectionName + "Product").Replace(_ancillaryProductName);
                formData.SelectToken(_sectionName + "." + _sectionName + "ProductSelection.RequestNew" + _sectionName + "Product").Replace("false");
                _detail.JSONData = JsonConvert.SerializeObject(formData);
            }
            else
            {
                if (_ancillaryFormInstanceId != 0)
                {
                    JObject formData = JObject.Parse(_detail.JSONData);
                    FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, _ancillaryFormDesignVersionId, _formDesignServices);
                    FormDesignVersionDetail ancilaryDetail = formDesignVersionMgr.GetFormDesignVersion(false);

                    string ancillarydata = _formDataInstanceManager.GetSectionData(_ancillaryFormInstanceId, _sectionName + "ProductInformation", true, ancilaryDetail, false, false);
                    JObject ancillarydataJData = JObject.Parse(ancillarydata);
                    JToken ancillarydataValue = ancillarydataJData.SelectToken(_sectionName + "ProductInformation");

                    formData.SelectToken(_sectionName + "." + _sectionName + "ProductInformation").Replace(ancillarydataValue);

                    formData.SelectToken(_sectionName + "." + _sectionName + "ProductSelection.Select" + _sectionName + "Product").Replace(_ancillaryProductName);
                    formData.SelectToken(_sectionName + "." + _sectionName + "ProductSelection.RequestNew" + _sectionName + "Product").Replace("false");

                    formData.SelectToken(_sectionName + "." + _sectionName + "ProductSelection.FolderName").Replace(_FolderName);
                    formData.SelectToken(_sectionName + "." + _sectionName + "ProductSelection.Version").Replace(_FolderVersionNumber);
                    formData.SelectToken(_sectionName + "." + _sectionName + "ProductSelection.EffectiveDate").Replace(_FolderEffectiveDate);
                   
                    _detail.JSONData = JsonConvert.SerializeObject(formData);
                }
            }
        }
    }
}