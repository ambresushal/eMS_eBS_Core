using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.integration.domain.Models;
using tmg.equinox.integration.domain.services.MasterListServiceReference;
using Service = tmg.equinox.integration.domain.services.ProductServiceReference;
using Data = tmg.equinox.integration.translator.dao.Models;
using tmg.equinox.integration.infrastructure.exceptionhandling;
using tmg.equinox.integration.translator.dao.Models;
using Newtonsoft.Json.Linq;
using tmg.equinox.integration.infrastructure.Util;
using System.Data;
using System.Data.SqlClient;
using tmg.equinox.integration.domain.services.Enums;

namespace tmg.equinox.integration.domain.services.Impl
{
    public class TranslatorService : ITranslatorService
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private IEquinoxProductService _equinoxProductService;
        public Service.ProductFacetsQHPService productService;
        public tmg.equinox.integration.domain.services.MasterListServiceReference.MasterListService masterListService;
        public Facet481Model facet481Model;

        public EquinioxProduct getProductResponse;
        public MasterList getMasterListResponse;
        dynamic masterlist;
        List<NetworkTypeModel> networkTypePrefixList = new List<NetworkTypeModel>();
        public DateTime PDPD_EFF_DT = DateTime.Now;
        public string PDPD_ID = string.Empty;
        public DateTime PDPD_TERM_DT = DateTime.Now;

        public readonly short TOKEN = 1;
        public readonly DateTime ATXR_SOURCE_ID = Convert.ToDateTime("1753-01-01");

        public string BatchId { get; set; }
        public int ProductId { get; set; }

        public TranslatorService(IUnitOfWork unitOfWork, IEquinoxProductService equinoxProductService)
        {
            _unitOfWork = unitOfWork;
            _equinoxProductService = equinoxProductService;
        }

        public TranslatorRowModel GetAllProductsInTranslationQueue()
        {
            TranslatorRowModel translatorRowModel = new TranslatorRowModel();
            List<TranslatorProductRowModel> translatorModelList;
            List<PluginRowModel> pluginList;
            List<VersionRowModel> versionList;

            try
            {

                translatorModelList = (from c in this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>()
                                                                       .Query()
                                                                       .Get()
                                       select new TranslatorProductRowModel
                                       {
                                           Id = c.ProcessQueueId,
                                           Batch = c.BatchId,
                                           Status = c.PluginVersionProcessorStatus.Status,
                                           PluginId = c.PluginVersionProcessor.PluginVersion.PluginId.Value,
                                           Plugin = c.PluginVersionProcessor.PluginVersion.Plugin.Name,
                                           ProductId = c.ProductId.Value.ToString(),
                                           Product = c.Product,
                                           Version = c.PluginVersionProcessor.PluginVersion.Description,
                                           VersionId = c.PluginVersionProcessor.PluginVersion.PluginVersionId.ToString(),
                                           PluginVersionProcessorId = c.PluginVersionProcessorId.Value,
                                           FolderVersionNumber = c.FolderVersionNumber,
                                           FormInstanceName = c.FormInstanceName,
                                           FolderName = c.FolderName
                                       }).ToList();

                translatorRowModel.translatorProducts = translatorModelList;

                pluginList = (from plgn in this._unitOfWork.Repository<Data.Plugin>()
                                                                        .Query()
                                                                        .Get()
                              select new PluginRowModel
                              {
                                  Id = plgn.PluginId,
                                  Name = plgn.Name
                              }).ToList();

                translatorRowModel.plugins = pluginList;

                versionList = (from ver in this._unitOfWork.Repository<Data.PluginVersion>()
                                                                        .Query()
                                                                        .Get()
                               select new VersionRowModel
                               {
                                   Id = ver.PluginVersionId,
                                   PluginId = ver.PluginId.Value,
                                   Name = ver.Description
                               }).ToList();

                translatorRowModel.versions = versionList;

                if (translatorModelList.Count() == 0)
                    translatorModelList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return translatorRowModel;
        }

        public TranslatorRowModel GetCompletedProductsInTranslationQueue()
        {
            TranslatorRowModel translatorRowModel = new TranslatorRowModel();
            List<TranslatorProductRowModel> translatorModelList;
            try
            {
                int statusComplete = (int)Status.Completed;
                translatorModelList = (from c in this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>()
                                                                        .Query()
                                                                        .Filter(c => c.PluginVersionStatusId == statusComplete)
                                                                        .Get()
                                       select new TranslatorProductRowModel
                                       {
                                           Id = c.ProcessQueueId,
                                           Batch = c.BatchId,
                                           Status = c.PluginVersionProcessorStatus.Status,
                                           PluginId = c.PluginVersionProcessor.PluginVersion.PluginId.Value,
                                           Plugin = c.PluginVersionProcessor.PluginVersion.Plugin.Name,
                                           Product = c.ProductId.Value.ToString(),
                                           Version = c.PluginVersionProcessor.PluginVersion.Description,
                                           VersionId = c.PluginVersionProcessor.PluginVersion.PluginVersionId.ToString(),
                                           PluginVersionProcessorId = c.PluginVersionProcessorId.Value,
                                           FolderVersionNumber = c.FolderVersionNumber,
                                           FormInstanceName = c.FormInstanceName,
                                           FolderName = c.FolderName
                                       }).ToList();

                translatorRowModel.translatorProducts = translatorModelList;

                if (translatorModelList.Count() == 0)
                    translatorModelList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return translatorRowModel;
        }

        public IList<PluginRowModel> GetPluginList()
        {
            IList<PluginRowModel> pluginList = null;
            try
            {
                pluginList = (from c in this._unitOfWork.Repository<Data.Plugin>()
                                                                            .Query()
                                                                            .Get()
                              select new PluginRowModel
                                   {
                                       Id = c.PluginId,
                                       Name = c.Name
                                   }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return pluginList;
        }

        public IList<VersionRowModel> GetVersionList()
        {
            IList<VersionRowModel> pluginVersionList = null;
            try
            {
                pluginVersionList = (from c in this._unitOfWork.Repository<Data.PluginVersion>()
                                                                            .Get()
                                     select new VersionRowModel
                                   {
                                       Id = c.PluginVersionId,
                                       PluginId = (int)c.PluginId,
                                       Version = c.Description
                                   }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return pluginVersionList;
        }

        public IList<TranslatorProductRowModel> GetProductList(TranslatorProductRowModel prodreqmodel)
        {
            IList<TranslatorProductRowModel> productList = null;
            try
            {
                List<PDPD481Model> prodModel = _equinoxProductService.GetAllProducts(prodreqmodel.Plugin, prodreqmodel.Version);

                productList = new List<TranslatorProductRowModel>();
                if (prodModel != null && prodModel.Count > 0)
                {

                    TranslatorProductRowModel product;
                    foreach (var item in prodModel)
                    {
                        product = new TranslatorProductRowModel();
                        product.Id = (int)item.productId;
                        product.Product = item.PDPD_ID;
                        product.Version = prodreqmodel.Version;
                        product.Plugin = prodreqmodel.Plugin;
                        product.FolderVersionNumber = item.FolderVersionNumber;
                        product.FormInstanceName = item.FormInstanceName;
                        product.FolderName = item.FolderName;

                        productList.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return productList;
        }

        string ITranslatorService.AddProducttoTranslate(IList<TranslatorProductRowModel> producttoTranslate)
        {
            string msg = string.Empty;
            try
            {
                if (producttoTranslate != null)
                {
                    string batchId = Guid.NewGuid().ToString().ToUpper();
                    foreach (var product in producttoTranslate)
                    {
                        if (product != null && product.Id != null && product.Id != 0)
                        {
                            Data.PluginVersionProcessQueueCommon newProducttoTranslate = new Data.PluginVersionProcessQueueCommon();
                            newProducttoTranslate.BatchId = batchId;
                            newProducttoTranslate.ProductId = Convert.ToInt32(product.Id);
                            newProducttoTranslate.Product = product.Product;
                            newProducttoTranslate.PluginVersionProcessorId = 1;
                            newProducttoTranslate.PluginVersionStatusId = 1;
                            newProducttoTranslate.IsActive = true;
                            newProducttoTranslate.CreatedBy = "WebAdmin";
                            newProducttoTranslate.CreatedDate = DateTime.Now;
                            newProducttoTranslate.FolderVersionNumber = product.FolderVersionNumber;
                            newProducttoTranslate.FormInstanceName = product.FormInstanceName;
                            newProducttoTranslate.FolderName = product.FolderName;

                            this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>().Insert(newProducttoTranslate);
                            this._unitOfWork.Save();
                        }
                    }
                    //Return success resul
                    msg = "Product successfully queued";
                }
            }
            catch (Exception ex)
            {
                msg = "Error Queuing product for translation";
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return msg;
        }

        string ITranslatorService.UpdateProducttoTranslate(TranslatorProductRowModel producttoTranslate)
        {
            string msg = string.Empty;
            try
            {
                if (producttoTranslate != null)
                {
                    Data.PluginVersionProcessQueueCommon processQueue = this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>().FindById(producttoTranslate.Id);

                    processQueue.ProductId = Convert.ToInt32(producttoTranslate.Product);
                    processQueue.PluginVersionProcessorId = 1;
                    processQueue.UpdatedBy = "WebAdmin";
                    processQueue.UpdatedDate = DateTime.Now;

                    this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>().Update(processQueue);
                    this._unitOfWork.Save();

                    //Return success resul
                    msg = "Product successfully queued";
                }
            }
            catch (Exception ex)
            {
                msg = "Error Queuing product for translation";
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return msg;
        }

        bool ITranslatorService.DeleteProcessQueue(int Id)
        {
            try
            {
                Data.PluginVersionProcessQueueCommon processQueue = this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>().FindById(Id);

                this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>().Delete(processQueue);
                this._unitOfWork.Save();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return true;
        }

        private void UpdateProduct(TranslatorProductRowModel producttoTranslate, DateTime startTime, DateTime endTime, int status, bool hasError)
        {
            string msg = string.Empty;
            try
            {
                if (producttoTranslate != null)
                {
                    Data.PluginVersionProcessQueueCommon pluginTransProcessQueue = this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>()
                                                               .FindById(producttoTranslate.Id);

                    pluginTransProcessQueue.StartTime = startTime;
                    pluginTransProcessQueue.EndTime = endTime;
                    pluginTransProcessQueue.PluginVersionStatusId = status;
                    pluginTransProcessQueue.HasError = hasError;
                    this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>().Update(pluginTransProcessQueue);
                    this._unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        /// <summary>
        /// Execute the translators
        /// </summary>
        /// <returns></returns>
        public bool ExecuteTranslator(string plugin, string pluginVersion)
        {
            try
            {

                var facet481Model = GetServiceResponse(plugin, pluginVersion);
                return true;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return false;
        }

        /// <summary>
        /// Get the servcie responses and insert in respective DB
        /// </summary>
        /// <returns></returns>
        private Facet481Model GetServiceResponse(string plugin, string pluginVersion)
        {
            facet481Model = null;
            try
            {
                facet481Model = new Facet481Model();

                DateTime startTime;
                DateTime endTime = DateTime.MinValue;
                int status = 0;
                bool hasError = false;
                int statusQueued = (int)Status.Queued;
                List<TranslatorProductRowModel> processQueueList;
                processQueueList = (from c in this._unitOfWork.Repository<Data.PluginVersionProcessQueueCommon>()
                                       .Get()
                                       .Where(c => c.PluginVersionStatusId == statusQueued
                                       && c.PluginVersionProcessor.PluginVersion.Plugin.Name == plugin
                                       && c.PluginVersionProcessor.PluginVersion.Description == pluginVersion)
                                    select new TranslatorProductRowModel
                                    {
                                        Id = c.ProcessQueueId,
                                        Batch = c.BatchId,
                                        Plugin = c.PluginVersionProcessor.PluginVersion.Plugin.Name,
                                        PluginId = (int)c.PluginVersionProcessor.PluginVersion.PluginId,
                                        PluginVersionProcessorId = (int)c.PluginVersionProcessorId,
                                        Product = c.ProductId.ToString(),
                                        Status = c.PluginVersionProcessorStatus.Status,
                                        Version = c.PluginVersionProcessor.PluginVersion.Description,
                                        VersionId = c.PluginVersionProcessor.PluginVersionId.ToString(),
                                        FolderVersionNumber = c.FolderVersionNumber,
                                        FormInstanceName = c.FormInstanceName,
                                        FolderName = c.FolderName
                                    }).ToList();

                if (processQueueList != null)
                {
                    foreach (var processQueue in processQueueList)
                    {
                        BatchId = processQueue.Batch;
                        ProductId = Convert.ToInt32(processQueue.Product);
                        startTime = DateTime.Now;

                        int prdId = Convert.ToInt32(processQueue.Product);
                        EquinioxProduct getProductRes = _equinoxProductService.GetProductJson(prdId);

                        int masterlistInstanceId = _equinoxProductService.GetMasterListInstanceId(prdId);
                        masterlist = _equinoxProductService.GetMasterListJson(masterlistInstanceId);

                        if (getProductRes != null)
                        {
                            getProductResponse = getProductRes;

                            facet481Model.PDPD481Model = new PDPD481Model();
                            facet481Model = GetPDPDDetails(facet481Model);

                            facet481Model.PDDS481Model = new PDDS481Model();
                            facet481Model = GetPDDSDetails(facet481Model);

                            facet481Model.PDBC481Model = new PDBC481Model();
                            facet481Model = GetPDBCDetails(facet481Model);

                            facet481Model.DEDE481Model = new DEDE481Model();
                            facet481Model = GetDEDEDetails(facet481Model);

                            facet481Model.LTLT481Model = new LTLT481Model();
                            facet481Model = GetLTLTDetails(facet481Model);

                            facet481Model.SERL481Model = new SERL481Model();
                            facet481Model = GetSERLDetails(facet481Model);

                            facet481Model.SEPY481Model = new SEPY481Model();
                            facet481Model = GetSEPYDetails(facet481Model);

                            facet481Model.PDVC481Model = new PDVC481Model();
                            facet481Model = GetPDVCDetails(facet481Model);

                            if (!facet481Model.HasError)
                            {
                                string res = InsertServiceResponse(facet481Model);
                                if (res == "Error")
                                {
                                    facet481Model.HasError = true;
                                }
                                else
                                {
                                    endTime = DateTime.Now;
                                    status = 2;
                                    hasError = false;

                                    InsertProcessTranmitterQueue(processQueue);
                                }
                            }

                            if (facet481Model.HasError)
                            {
                                endTime = DateTime.Now;
                                status = (int)Status.Errored;
                                hasError = true;
                            }

                            UpdateProduct(processQueue, startTime, endTime, status, hasError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        /// <summary>
        /// Insert service response in DB
        /// </summary>
        /// <param name="facet481PDPDModel"></param>
        /// <returns></returns>
        private string InsertServiceResponse(Facet481Model facet481Model)
        {
            try
            {
                if (facet481Model.PDPD481Model != null)
                {

                    Data.PDPD481Stg pdpd481 = facet481Model.PDPD481Model.MapToEntity<Data.PDPD481Stg, PDPD481Model>();
                    this._unitOfWork.Repository<Data.PDPD481Stg>().Insert(pdpd481);
                }

                if (facet481Model.PDDS481Model != null)
                {
                    Data.PDDS481Stg pdds481 = facet481Model.PDDS481Model.MapToEntity<Data.PDDS481Stg, PDDS481Model>();
                    this._unitOfWork.Repository<Data.PDDS481Stg>().Insert(pdds481);
                }

                if (facet481Model.PDBC481 != null)
                {
                    List<Data.PDBC481Stg> pdbc481 = facet481Model.PDBC481.MapToListEntity<Data.PDBC481Stg, PDBC481Model>();
                    this._unitOfWork.Repository<Data.PDBC481Stg>().InsertRange(pdbc481);

                    UpdatePrefixCounter("PDBC", pdbc481[0].PDBC_PFX);
                }

                if (facet481Model.DEDE481 != null)
                {
                    List<Data.DEDE481Stg> dede481 = facet481Model.DEDE481.MapToListEntity<Data.DEDE481Stg, DEDE481Model>();
                    this._unitOfWork.Repository<Data.DEDE481Stg>().InsertRange(dede481);

                    UpdatePrefixCounter("DEDE", dede481[0].DEDE_PFX);
                }

                if (facet481Model.LTLT481 != null)
                {
                    List<Data.LTLT481Stg> ltlt481 = facet481Model.LTLT481.MapToListEntity<Data.LTLT481Stg, LTLT481Model>();
                    this._unitOfWork.Repository<Data.LTLT481Stg>().InsertRange(ltlt481);

                    UpdatePrefixCounter("LTLT", ltlt481[0].LTLT_PFX);
                }

                if (facet481Model.LTIP481 != null)
                {
                    List<Data.LTIP481Stg> ltlt481 = facet481Model.LTIP481.MapToListEntity<Data.LTIP481Stg, LTIP481Model>();
                    this._unitOfWork.Repository<Data.LTIP481Stg>().InsertRange(ltlt481);
                }

                if (facet481Model.LTID481 != null)
                {
                    List<Data.LTID481Stg> ltlt481 = facet481Model.LTID481.MapToListEntity<Data.LTID481Stg, LTID481Model>();
                    this._unitOfWork.Repository<Data.LTID481Stg>().InsertRange(ltlt481);
                }

                if (facet481Model.LTPR481 != null)
                {
                    List<Data.LTPR481Stg> ltlt481 = facet481Model.LTPR481.MapToListEntity<Data.LTPR481Stg, LTPR481Model>();
                    this._unitOfWork.Repository<Data.LTPR481Stg>().InsertRange(ltlt481);
                }

                if (facet481Model.LTSE481 != null)
                {
                    List<Data.LTSE481Stg> ltse481 = facet481Model.LTSE481.MapToListEntity<Data.LTSE481Stg, LTSE481Model>();
                    this._unitOfWork.Repository<Data.LTSE481Stg>().InsertRange(ltse481);
                }

                if (facet481Model.SESE481 != null)
                {
                    List<Data.SESE481Stg> sese481 = facet481Model.SESE481.MapToListEntity<Data.SESE481Stg, SESE481Model>();
                    this._unitOfWork.Repository<Data.SESE481Stg>().InsertRange(sese481);
                }

                if (facet481Model.SEPY481 != null)
                {
                    List<Data.SEPY481Stg> sepy481 = facet481Model.SEPY481.MapToListEntity<Data.SEPY481Stg, SEPY481Model>();
                    this._unitOfWork.Repository<Data.SEPY481Stg>().InsertRange(sepy481);
                    UpdatePrefixCounter("SEPY", sepy481.Select(c => c.SEPY_PFX).Max());
                }

                if (facet481Model.PDVC481 != null)
                {
                    List<Data.PDVC481Stg> pdvc481 = facet481Model.PDVC481.MapToListEntity<Data.PDVC481Stg, PDVC481Model>();
                    this._unitOfWork.Repository<Data.PDVC481Stg>().InsertRange(pdvc481);

                }

                if (facet481Model.SERL481 != null)
                {
                    List<Data.SERL481Stg> serl481 = facet481Model.SERL481.MapToListEntity<Data.SERL481Stg, SERL481Model>();
                    this._unitOfWork.Repository<Data.SERL481Stg>().InsertRange(serl481);
                }

                this._unitOfWork.Save();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return null;
        }

        /// <summary>
        /// Get PDPD Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetPDPDDetails(Facet481Model facet481Model)
        {
            try
            {

                var pdpdData = getProductResponse.ProductRules.FacetsProductInformation;
                var pdpdGeneralInfo = getProductResponse.ProductRules.GeneralInformation;
                if (pdpdData != null && pdpdGeneralInfo != null)
                {
                    facet481Model.PDPD481Model.PDPD_ID = pdpdData.ProductID.ToString();
                    PDPD_ID = pdpdData.ProductID.ToString();
                    facet481Model.PDPD481Model.PDPD_RISK_IND = pdpdData.LineofBusinessSwitchIndicator.ToString();
                    facet481Model.PDPD481Model.PDPD_RISK_IND = facet481Model.PDPD481Model.PDPD_RISK_IND != "" ? facet481Model.PDPD481Model.PDPD_RISK_IND.Substring(0, 1) : "";

                    facet481Model.PDPD481Model.LOBD_ID = pdpdData.ProductLineofBusiness.ToString();
                    facet481Model.PDPD481Model.LOBD_ID = facet481Model.PDPD481Model.LOBD_ID != "" ? facet481Model.PDPD481Model.LOBD_ID.Substring(0, 4) : "";

                    facet481Model.PDPD481Model.LOBD_ALT_RISK_ID = pdpdData.ProductAlternateLineofBusiness.ToString();
                    facet481Model.PDPD481Model.LOBD_ALT_RISK_ID = facet481Model.PDPD481Model.LOBD_ALT_RISK_ID != "" ? facet481Model.PDPD481Model.LOBD_ALT_RISK_ID.Substring(0, 4) : "";

                    facet481Model.PDPD481Model.PDPD_ACC_SFX = pdpdData.ProductAccumulatorSuffix.ToString();
                    facet481Model.PDPD481Model.PDPD_ACC_SFX = facet481Model.PDPD481Model.PDPD_ACC_SFX != "" ? facet481Model.PDPD481Model.PDPD_ACC_SFX.Substring(0, 4) : "";

                    facet481Model.PDPD481Model.PDPD_CAP_POP_LVL = pdpdData.CapitationPercentofPremiumLevel.ToString();
                    facet481Model.PDPD481Model.PDPD_CAP_POP_LVL = facet481Model.PDPD481Model.PDPD_CAP_POP_LVL != "" ? facet481Model.PDPD481Model.PDPD_CAP_POP_LVL.Substring(0, 1) : "";

                    facet481Model.PDPD481Model.PDPD_CAP_RET_MOS = 1;
                    facet481Model.PDPD481Model.PDPD_MCTR_CCAT = pdpdData.CapitationCategory.ToString();
                    facet481Model.PDPD481Model.PDPD_MCTR_CCAT = facet481Model.PDPD481Model.PDPD_MCTR_CCAT != "" ? facet481Model.PDPD481Model.PDPD_MCTR_CCAT.Substring(0, 4) : "";

                    facet481Model.PDPD481Model.PDPD_OPTS = "";
                    facet481Model.PDPD481Model.PDPD_LOCK_TOKEN = TOKEN;
                    facet481Model.PDPD481Model.ATXR_SOURCE_ID = ATXR_SOURCE_ID;
                    facet481Model.PDPD481Model.SYS_LAST_UPD_DTM = DateTime.Now;
                    facet481Model.PDPD481Model.SYS_USUS_ID = "";
                    facet481Model.PDPD481Model.SYS_DBUSER_ID = "";

                    facet481Model.PDPD481Model.PDPD_EFF_DT = Convert.ToDateTime(pdpdGeneralInfo.ClaimsEffectiveDate);
                    facet481Model.PDPD481Model.PDPD_TERM_DT = Convert.ToDateTime(pdpdGeneralInfo.ClaimsCancelDate);
                    PDPD_EFF_DT = Convert.ToDateTime(pdpdGeneralInfo.ClaimsEffectiveDate);
                    PDPD_TERM_DT = Convert.ToDateTime(pdpdGeneralInfo.ClaimsCancelDate);
                }

                facet481Model.PDPD481Model.BatchID = BatchId;
                facet481Model.PDPD481Model.BenefitOfferingID = ProductId;
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        /// <summary>
        /// Get PDDS Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetPDDSDetails(Facet481Model facet481Model)
        {
            try
            {
                var productData = getProductResponse.ProductRules.FacetsProductInformation;
                var productDataGeneralInfo = getProductResponse.ProductRules.GeneralInformation;
                if (productData != null && productDataGeneralInfo != null)
                {

                    facet481Model.PDDS481Model.PDPD_ID = productData.ProductID.ToString();
                    facet481Model.PDDS481Model.PDDS_UM_IND = (Convert.ToBoolean(productData.ReferralsPreauthorizations) == true ? "Y" : "N");
                    facet481Model.PDDS481Model.PDDS_MED_PRICE_IND = (Convert.ToBoolean(productData.PrePricing) == true ? "Y" : "N");
                    facet481Model.PDDS481Model.PDDS_MED_CLMS_IND = (Convert.ToBoolean(productData.MedicalClaimsProcessing) == true ? "Y" : "N"); //TODO: Both Medical and Dental claimsprocessing are there, which need to take?
                    facet481Model.PDDS481Model.PDDS_PREM_IND = productData.PremiumIndicator;
                    facet481Model.PDDS481Model.PDDS_PREM_IND = facet481Model.PDDS481Model.PDDS_PREM_IND != "" ? facet481Model.PDDS481Model.PDDS_PREM_IND.Substring(0, 1) : "";

                    facet481Model.PDDS481Model.PDDS_CLED_IND = (Convert.ToBoolean(productData.ClinicalEdits) == true ? "Y" : "N");
                    facet481Model.PDDS481Model.PDDS_CAP_IND = (Convert.ToBoolean(productData.CapitationRiskAllocation) == true ? "Y" : "N");
                    facet481Model.PDDS481Model.PDDS_INT_STATE_IND = productData.StateDeterminationforClaimsInterest.ToString();
                    facet481Model.PDDS481Model.PDDS_INT_STATE_IND = facet481Model.PDDS481Model.PDDS_INT_STATE_IND != "" ? facet481Model.PDDS481Model.PDDS_INT_STATE_IND.Substring(0, 1) : "";

                    facet481Model.PDDS481Model.PDDS_MCTR_BCAT = productData.ProductBusinessCategory.ToString();
                    facet481Model.PDDS481Model.PDDS_MCTR_BCAT = facet481Model.PDDS481Model.PDDS_MCTR_BCAT != "" ? facet481Model.PDDS481Model.PDDS_MCTR_BCAT.Substring(0, 4) : "";
                    facet481Model.PDDS481Model.PDDS_MCTR_VAL1 = productData.ProductValueCode1.ToString();
                    facet481Model.PDDS481Model.PDDS_MCTR_VAL1 = facet481Model.PDDS481Model.PDDS_MCTR_VAL1 != "" ? facet481Model.PDDS481Model.PDDS_MCTR_VAL1.Substring(0, 4) : "";
                    facet481Model.PDDS481Model.PDDS_MCTR_VAL2 = productData.ProductValueCode2.ToString();
                    facet481Model.PDDS481Model.PDDS_MCTR_VAL2 = facet481Model.PDDS481Model.PDDS_MCTR_VAL2 != "" ? facet481Model.PDDS481Model.PDDS_MCTR_VAL2.Substring(0, 4) : "";
                    facet481Model.PDDS481Model.PDDS_OPTOUT_IND = (Convert.ToBoolean(productData.POSOptout) == true ? "Y" : "N");
                    facet481Model.PDDS481Model.PDDS_DEN_PD_IND = (Convert.ToBoolean(productData.DentalPreDeterminations) == true ? "Y" : "N");

                    facet481Model.PDDS481Model.PDDS_DESC = productDataGeneralInfo.ProductName.ToString();
                    facet481Model.PDDS481Model.PDDS_APP_TYPE = productDataGeneralInfo.ApplicationType.ToString();
                    facet481Model.PDDS481Model.PDDS_APP_TYPE = facet481Model.PDDS481Model.PDDS_APP_TYPE != "" ? facet481Model.PDDS481Model.PDDS_APP_TYPE.Substring(0, 1) : "";
                    facet481Model.PDDS481Model.PDDS_PROD_TYPE = productDataGeneralInfo.ProductType.ToString();
                    facet481Model.PDDS481Model.PDDS_PROD_TYPE = facet481Model.PDDS481Model.PDDS_PROD_TYPE != "" ? facet481Model.PDDS481Model.PDDS_PROD_TYPE.Substring(0, 1) : "";
                }

                facet481Model.PDDS481Model.BenefitOfferingID = 1;
                facet481Model.PDDS481Model.PDDS_LOCK_TOKEN = TOKEN;
                facet481Model.PDDS481Model.PDDS_DOFR_IND = "";
                facet481Model.PDDS481Model.SYS_LAST_UPD_DTM = DateTime.Now; //TODO : need to confirm.
                facet481Model.PDDS481Model.SYS_USUS_ID = "";
                facet481Model.PDDS481Model.SYS_DBUSER_ID = "";
                facet481Model.PDDS481Model.PDDS_DEN_UM_IND = "";
                facet481Model.PDDS481Model.PDDS_DEN_PRICE_IND = "";
                facet481Model.PDDS481Model.PDDS_DEN_CLMS_IND = "";
                facet481Model.PDDS481Model.ATXR_SOURCE_ID = DateTime.Now; //TODO : need to confirm.

                facet481Model.PDDS481Model.BatchID = BatchId;
                facet481Model.PDDS481Model.BenefitOfferingID = ProductId;
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        /// <summary>
        /// Get PDBC Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetPDBCDetails(Facet481Model facet481Model)
        {
            try
            {
                if (getProductResponse.ProductRules.FacetsProductInformation.FacetProductComponentsPDBC != null)
                {
                    List<PDBC481Model> PDBCList = new List<PDBC481Model>();
                    foreach (var item in getProductResponse.ProductRules.FacetsProductInformation.FacetProductComponentsPDBC.PDBCPrefixList.ToList())
                    {
                        PDBC481Model model = new PDBC481Model();
                        model.PDPD_ID = facet481Model.PDPD481Model.PDPD_ID; //TODO : Need to check.
                        model.PDBC_TYPE = item.PDBCType;
                        model.PDBC_EFF_DT = Convert.ToDateTime(item.EffectiveDate);
                        model.PDBC_TERM_DT = Convert.ToDateTime(item.CancelDate);//TODO : if input is null/invalie use the default value
                        model.PDBC_PFX = item.PDBCPrefix;
                        model.PDBC_OPTS = "";
                        model.PDBC_LOCK_TOKEN = TOKEN;
                        model.ATXR_SOURCE_ID = ATXR_SOURCE_ID;
                        model.SYS_LAST_UPD_DTM = DateTime.Now;
                        model.SYS_USUS_ID = "";
                        model.SYS_DBUSER_ID = "";

                        model.BatchID = BatchId;
                        model.BenefitOfferingID = ProductId;
                        PDBCList.Add(model);
                    }
                    facet481Model.PDBC481 = PDBCList.Distinct().ToList();
                }
                return facet481Model;
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return null;
        }

        /// <summary>
        /// Get PDVC Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetPDVCDetails(Facet481Model facet481Model)
        {
            var pdvcDetails = getProductResponse.NetworkDS.FacetProductVariableComponent;
            List<PDVC481Model> PDVCList = new List<PDVC481Model>();
            if (pdvcDetails != null)
            {

                foreach (var item in pdvcDetails.ToList())
                {
                    PDVC481Model model = new PDVC481Model();
                    model.PDPD_ID = PDPD_ID;
                    model.PDVC_TIER = Convert.ToInt16(item.ProductVariableComponentTier != "" ? item.ProductVariableComponentTier : "0");
                    model.PDVC_TYPE = item.ProductVariableComponentType != "" ? item.ProductVariableComponentType.Substring(0, 1) : "";
                    model.PDVC_EFF_DT = item.EffectiveDate != "" ? Convert.ToDateTime(item.EffectiveDate) : PDPD_EFF_DT;
                    model.PDVC_SEQ_NO = Convert.ToInt16(item.SequenceNumber != "" ? item.SequenceNumber : "0");
                    model.PDVC_TERM_DT = item.TerminationDate != "" ? Convert.ToDateTime(item.TerminationDate) : PDPD_TERM_DT;
                    model.PDVC_PR_PCP = item.PrimaryCareProvider.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_PR_IN = item.InNetworkProvider.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_PR_PAR = item.ParticipatingProvider.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_PR_NONPAR = item.NonParticipatingProvider.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_PC_NR = item.PreauthorizationNotRequired.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_PC_OBT = item.PreauthorizationObtained.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_PC_VIOL = item.PreauthorizationViolation.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_REF_NR = item.ReferralNotRequired.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_REF_OBT = item.ReferralObtained.ToLower() == "yes" ? "Y" : "N";
                    model.PDVC_REF_VIOL = item.ReferralViolation.ToLower() == "yes" ? "Y" : "N";

                    model.PDVC_LOBD_PTR = item.LineofBusinessIndicator != "" ? item.LineofBusinessIndicator.Substring(0, 1) : "";
                    model.SEPY_PFX = networkTypePrefixList.Where(c => c.NetworkType == item.NetworkType).Select(c => c.Prefix).FirstOrDefault();
                    var pfx = Convert.ToInt32(GetPrefixCounter("DEDE"));
                    model.DEDE_PFX = Util.ConvertToBase36(pfx);
                    pfx = Convert.ToInt32(GetPrefixCounter("LTLT"));
                    model.LTLT_PFX = Util.ConvertToBase36(pfx);

                    model.DPPY_PFX = "";
                    model.CGPY_PFX = "";
                    model.PDVC_LOCK_TOKEN = TOKEN;
                    model.ATXR_SOURCE_ID = DateTime.Now;
                    model.SYS_LAST_UPD_DTM = DateTime.Now;
                    model.SYS_USUS_ID = "";
                    model.SYS_DBUSER_ID = "";

                    model.BatchID = BatchId;
                    model.BenefitOfferingID = ProductId;


                    PDVCList.Add(model);
                }
            }
            facet481Model.PDVC481 = PDVCList.Distinct().ToList();


            return facet481Model;
        }

        /// <summary>
        /// Get SEPY Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetSEPYDetails(Facet481Model facet481Model)
        {
            try
            {
                // Get 
                var pfx = GetPrefixCounter("SEPY");
                var newPfx = Convert.ToInt32(pfx) + 1;
                var benefitReviewGridDataList = getProductResponse.BenefitReview.BenefitReviewGrid.ToList();
                var benefitReviewAltGridDataList = getProductResponse.BenefitReview.BenefitReviewGridAltRulesData.ToList();
                var benefitReviewTierGridDataList = getProductResponse.BenefitReview.BenefitReviewGridTierData.ToList();
                Models.Deductible benefitProductData = getProductResponse.CostShare.Deductible;
                List<Models.MessageServiceList> benefitMessageDataList = getProductResponse.Messages.MessageServiceList;

                List<SEPY481Model> sepy481ModelList = new List<SEPY481Model>();
                var networkList = getProductResponse.NetworkDS.NetworkList; //based on above conditon
                var networkTypes = networkList.Select(c => c.NetworkType).Distinct();

                foreach (var networkType in networkTypes)
                {
                    NetworkTypeModel networkTypeModel = new NetworkTypeModel();
                    networkTypeModel.NetworkType = networkType;
                    pfx = (Convert.ToInt32(pfx) + 1).ToString();
                    networkTypeModel.Prefix = Util.ConvertToBase36(newPfx);
                    networkTypePrefixList.Add(networkTypeModel);
                }

                foreach (var benefitReviewGridData in benefitReviewGridDataList)
                {
                    string cat1 = benefitReviewGridData.BenefitCat1;
                    string cat2 = benefitReviewGridData.BenefitCat2;
                    string cat3 = benefitReviewGridData.BenefitCat3;
                    string pos = benefitReviewGridData.POS;
                    TierModel tierModel = new TierModel();
                    FacetRuleModel facetRuleModel = GetFacetRuleModel(cat1, cat2, cat3, pos);
                    string acac_Acc_No = GetDEDE_ACC_ACAC_No(cat1, cat2, cat3, pos);

                    // Get SErl ID
                    List<NetworkTypeModel> networkserl_Ids = GetNetworkSERLIds(cat1, cat2, cat3, pos, benefitMessageDataList);

                    foreach (var network in networkList)
                    {
                        var networkData = benefitReviewGridData.QHPFACETSNetworkDS.Where(c => c.NetworkType == network.NetworkType).FirstOrDefault();
                        bool isDisallowedMsg = IsDisAllowedMessagePresent(cat1, cat2, cat3, pos, network.NetworkType, tierModel, benefitMessageDataList);

                        if (networkData != null && facetRuleModel != null)
                        {
                            if (!isDisallowedMsg)
                            {
                                // for SESE 

                                tierModel.SERLId = networkserl_Ids.Where(c => c.NetworkType == network.NetworkType).Select(c => c.id).FirstOrDefault();
                                tierModel.AccumNumber = GetDEDEAccumNo(cat1, cat2, cat3, pos, network.NetworkType, benefitProductData);
                                tierModel.TypeIndicator = 'I';

                                TierDataModel tierZeroModel = new TierDataModel();
                                tierZeroModel.AllowedCounter = networkData.AllowedCounter.TrimStart('$').TrimEnd('%');
                                tierZeroModel.AllowedAmount = networkData.AllowedAmount.TrimStart('$').TrimEnd('%');
                                tierZeroModel.Coinsurance = networkData.Coinsurance.TrimStart('$').TrimEnd('%');
                                tierZeroModel.Copay = networkData.Copay.TrimStart('$').TrimEnd('%');
                                tierZeroModel.TierNo = "0";
                                tierModel.TierDataModelList.Add(tierZeroModel);

                                tierModel.TierDataModelList = benefitReviewTierGridDataList.Where(c => c.BenefitCategory1 == cat1
                                                               && c.BenefitCategory2 == cat2
                                                               && c.BenefitCategory3 == cat3
                                                               && c.PlaceofService == pos
                                                               && c.NetworkType == network.NetworkType)
                                                               .Select(c =>
                                                               new TierDataModel
                                                               {
                                                                   AllowedAmount = c.AllowedAmount.TrimStart('$').TrimEnd('%'),
                                                                   AllowedCounter = c.AllowedCounter.TrimStart('$').TrimEnd('%'),
                                                                   Coinsurance = c.Coinsurance.TrimStart('$').TrimEnd('%'),
                                                                   Copay = c.Copay.TrimStart('$').TrimEnd('%'),
                                                                   TierNo = c.TierNo
                                                               }
                                                               ).ToList();


                                // normal Flow

                                SEPY481Model sepy = GenerateSEPYModel(networkData.Copay, networkData.Coinsurance, facetRuleModel.SESE_ID, network.NetworkType, false, tierModel, facetRuleModel);
                                sepy481ModelList.Add(sepy);

                                // in case of alt rule condition
                                var benefitReviewAltGrid = benefitReviewAltGridDataList
                                                            .Where(c => c.BenefitCategory1 == cat1
                                                                && c.BenefitCategory2 == cat2
                                                                && c.BenefitCategory3 == cat3
                                                                && c.PlaceofService == pos
                                                                && c.NetworkType == network.NetworkType).FirstOrDefault();

                                if (benefitReviewAltGrid != null)
                                {
                                    tierModel.TierDataModelList = benefitReviewAltGridDataList
                                                                 .Where(c => c.BenefitCategory1 == cat1
                                                                        && c.BenefitCategory2 == cat2
                                                                        && c.BenefitCategory3 == cat3
                                                                        && c.PlaceofService == pos
                                                                        && c.NetworkType == network.NetworkType).Select(c =>
                                                                new TierDataModel
                                                                {
                                                                    AllowedAmount = c.AllowedAmount.TrimStart('$').TrimEnd('%'),
                                                                    AllowedCounter = c.AllowedCounter.TrimStart('$').TrimEnd('%'),
                                                                    Coinsurance = c.Coinsurance.TrimStart('$').TrimEnd('%'),
                                                                    Copay = c.Copay.TrimStart('$').TrimEnd('%'),
                                                                    TierNo = c.TierNo
                                                                }
                                                                ).ToList();

                                    SEPY481Model sepyModel = GenerateSEPYModel(benefitReviewAltGrid.Copay, benefitReviewAltGrid.Coinsurance, facetRuleModel.SESE_ID, network.NetworkType, true, tierModel, facetRuleModel);
                                    sepy481ModelList.Add(sepyModel);
                                }
                            }
                        }
                    }
                }

                facet481Model.SEPY481 = sepy481ModelList.Distinct().ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return facet481Model;
        }

        private bool IsDisAllowedMessagePresent(string benefitCategory1, string benefitCategory2, string benefitCategory3, string placeOfService, string networkType, TierModel tierModel, List<Models.MessageServiceList> messageList)
        {
            try
            {
                var networkSerlList = messageList.Where(c => c.BenefitCategory1 == benefitCategory1
                                      && c.BenefitCategory2 == benefitCategory2
                                      && c.BenefitCategory3 == benefitCategory3
                                      && c.PlaceofService == placeOfService
                                      && c.Include.ToLower() == "yes"
                                      ).Select(c => c.QHPFACETSNetworkDS)
                                      .FirstOrDefault();

                if (networkSerlList != null)
                {
                    var DisAllowedMessages = networkSerlList.Where(c => !String.IsNullOrEmpty(c.DisallowedMessage) && c.NetworkType == networkType);
                    if (DisAllowedMessages != null && DisAllowedMessages.Count() > 0)
                    {
                        tierModel.MessageDescription = DisAllowedMessages.FirstOrDefault().DisallowedMessage;
                        tierModel.TypeIndicator = 'M';
                        TierDataModel dataModel = new TierDataModel();
                        dataModel.AllowedAmount = "0";
                        dataModel.AllowedCounter = "0";
                        dataModel.Coinsurance = "0";
                        dataModel.Copay = "0";
                        dataModel.TierNo = "0";
                        tierModel.TierDataModelList.Add(dataModel);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return false;
        }

        private List<NetworkTypeModel> GetNetworkSERLIds(string benefitCategory1, string benefitCategory2, string benefitCategory3, string placeOfService, List<Models.MessageServiceList> msgServiceList)
        {
            List<NetworkTypeModel> networkSerlIds = null;
            try
            {
                networkSerlIds = new List<NetworkTypeModel>();
                var networkSerlList = msgServiceList.Where(c => c.BenefitCategory1 == benefitCategory1
                                      && c.BenefitCategory2 == benefitCategory2
                                      && c.BenefitCategory3 == benefitCategory3
                                      && c.PlaceofService == placeOfService
                                      && c.Include.ToLower() == "yes"
                                      ).Select(c => c.QHPFACETSNetworkDS)
                                      .FirstOrDefault();
                if (networkSerlList != null)
                {
                    JArray serviceList = masterlist.Messages.RelatedServiceMessagesSERL;

                    foreach (var item in serviceList.Children())
                    {
                        var itemProperty = item.Children<JProperty>();
                        var myElement = itemProperty.ToList();

                        string description = myElement.Where(c => c.Name == "Description").FirstOrDefault().Value.ToString();
                        description = description != "[Select One]" ? description : "";

                        foreach (var network in networkSerlList)
                        {
                            if (network.MessageSERL == description)
                            {
                                NetworkTypeModel networkModel = new NetworkTypeModel();
                                networkModel.id = myElement.Where(c => c.Name == "RelatedServiceID").FirstOrDefault().Value.ToString();
                                description = description != "[Select One]" ? description : "";
                                networkModel.NetworkType = network.NetworkType;
                                networkSerlIds.Add(networkModel);
                                break;
                            }
                        }

                    }
                }
                //return networkSerlIds;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return networkSerlIds;
        }

        private string GetDEDEAccumNo(string benefitCategory1, string benefitCategory2, string benefitCategory3, string placeOfService, string networkType, Models.Deductible deductible)
        {
            string dedeAccumNo = string.Empty;
            try
            {
                //string dedeAccumNo = string.Empty;
                var dedeAccum = deductible.FacetServiceSpecificDeductible.FacetServiceSpecificDeductibleList
                      .Where(c => c.BenefitCategory1 == benefitCategory1
                          && c.BenefitCategory2 == benefitCategory2
                          && c.BenefitCategory3 == benefitCategory3
                          && c.PlaceofService == placeOfService       // check for networkType
                          && c.Include.ToLower() == "yes").ToList();
                if (dedeAccum != null && dedeAccum.Count > 0)
                {
                    dedeAccumNo = dedeAccum.FirstOrDefault().DeductibleRulesAccumulatorNumber;
                }
                else
                {
                    dedeAccumNo = deductible.DeductibleRules.DeductibleRulesAccumulatorNumber;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return dedeAccumNo;
        }

        private string GetDEDE_ACC_ACAC_No(string benefitCategory1, string benefitCategory2, string benefitCategory3, string placeOfService)
        {
            var dedeList = getProductResponse.CostShare.Deductible.FacetServiceSpecificDeductible.FacetServiceSpecificDeductibleList.ToList();

            string acac_Acc_No = dedeList.Where(c => c.BenefitCategory1 == benefitCategory1
                 && c.BenefitCategory2 == benefitCategory2
                 && c.BenefitCategory3 == benefitCategory3
                 && c.PlaceofService == placeOfService).FirstOrDefault().DeductibleRulesAccumulatorNumber;

            return acac_Acc_No;
        }

        private FacetRuleModel GetFacetRuleModel(string benefitCategory1, string benefitCategory2, string benefitCategory3, string placeOfService)
        {
            FacetRuleModel ruleModel = null;
            try
            {
                ruleModel = new FacetRuleModel();
                JArray serviceList = masterlist.Services.ServiceList;
                foreach (var item in serviceList.Children())
                {
                    var itemProperty = item.Children<JProperty>();
                    var myElement = itemProperty.ToList();
                    string benefitCat1 = myElement.Where(c => c.Name == "BenefitCategory1").FirstOrDefault().Value.ToString();
                    benefitCat1 = benefitCat1 != "[Select One]" ? benefitCat1 : "";

                    string benefitCat2 = myElement.Where(c => c.Name == "BenefitCategory2").FirstOrDefault().Value.ToString();
                    benefitCat2 = benefitCat2 != "[Select One]" ? benefitCat2 : "";

                    string benefitCat3 = myElement.Where(c => c.Name == "BenefitCategory3").FirstOrDefault().Value.ToString();
                    benefitCat3 = benefitCat3 != "[Select One]" ? benefitCat3 : "";

                    string pos = myElement.Where(c => c.Name == "PlaceofService").FirstOrDefault().Value.ToString();
                    pos = pos != "[Select One]" ? pos : "";


                    if (benefitCategory1 == benefitCat1 && benefitCategory2 == benefitCat2 && benefitCategory3 == benefitCat3 && placeOfService == pos)
                    {

                        ruleModel.SESE_ID = myElement.Where(c => c.Name == "SESEID").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_ID = ruleModel.SESE_ID != "[Select One]" ? ruleModel.SESE_ID : "";

                        ruleModel.SESE_CM_IND = myElement.Where(c => c.Name == "DisplayCaseManagementWarningMessage").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_CM_IND = ruleModel.SESE_CM_IND != "[Select One]" ? ruleModel.SESE_CM_IND : "";
                        ruleModel.SESE_CM_IND = ruleModel.SESE_CM_IND.Length > 0 ? ruleModel.SESE_CM_IND.Substring(0, 1) : "";


                        ruleModel.SESE_PA_AMT_REQ = myElement.Where(c => c.Name == "PreAuthChargeRequired").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_PA_AMT_REQ = ruleModel.SESE_PA_AMT_REQ != "[Select One]" ? ruleModel.SESE_PA_AMT_REQ : "";
                        ruleModel.SESE_PA_AMT_REQ = ruleModel.SESE_PA_AMT_REQ.Length > 0 ? ruleModel.SESE_PA_AMT_REQ.Substring(0, 1) : "";

                        ruleModel.SESE_PA_UNIT_REQ = myElement.Where(c => c.Name == "PreAuthUnitsRequired").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_PA_UNIT_REQ = ruleModel.SESE_PA_UNIT_REQ.Length > 0 ? ruleModel.SESE_PA_UNIT_REQ.Substring(0, 1) : "";

                        ruleModel.SESE_PA_PROC_REQ = myElement.Where(c => c.Name == "PreAuthProcedureRequired").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_PA_PROC_REQ = ruleModel.SESE_PA_PROC_REQ != "[Select One]" ? ruleModel.SESE_PA_PROC_REQ : "";
                        ruleModel.SESE_PA_PROC_REQ = ruleModel.SESE_PA_PROC_REQ.Length > 0 ? ruleModel.SESE_PA_PROC_REQ.Substring(0, 1) : "";


                        ruleModel.SESE_VALID_SEX = myElement.Where(c => c.Name == "Gender").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_VALID_SEX = ruleModel.SESE_VALID_SEX != "[Select One]" ? ruleModel.SESE_VALID_SEX : "";
                        ruleModel.SESE_VALID_SEX = ruleModel.SESE_VALID_SEX.Length > 0 ? ruleModel.SESE_VALID_SEX.Substring(0, 1) : "";


                        ruleModel.SESE_SEX_EXCD_ID = myElement.Where(c => c.Name == "GenderExplanationCode").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_SEX_EXCD_ID = ruleModel.SESE_SEX_EXCD_ID != "[Select One]" ? ruleModel.SESE_SEX_EXCD_ID : "";
                        ruleModel.SESE_VALID_SEX = ruleModel.SESE_VALID_SEX.Length > 3 ? ruleModel.SESE_VALID_SEX.Substring(0, 3) : ruleModel.SESE_VALID_SEX;


                        string minAge = myElement.Where(c => c.Name == "AgeFrom").FirstOrDefault().Value.ToString();
                        minAge = minAge == "" ? "0" : minAge;
                        ruleModel.SESE_MIN_AGE = minAge != "[Select One]" ? Convert.ToInt32(minAge) : 0;

                        string maxAge = myElement.Where(c => c.Name == "AgeTo").FirstOrDefault().Value.ToString();
                        maxAge = maxAge == "" ? "0" : maxAge;
                        ruleModel.SESE_MAX_AGE = maxAge != "[Select One]" ? Convert.ToInt32(maxAge) : 0;

                        ruleModel.SESE_AGE_EXCD_ID = myElement.Where(c => c.Name == "AgeExplanationCode").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_AGE_EXCD_ID = ruleModel.SESE_AGE_EXCD_ID != "[Select One]" ? ruleModel.SESE_AGE_EXCD_ID : "";
                        ruleModel.SESE_AGE_EXCD_ID = ruleModel.SESE_AGE_EXCD_ID.Length > 3 ? ruleModel.SESE_AGE_EXCD_ID.Substring(0, 3) : ruleModel.SESE_VALID_SEX;


                        ruleModel.SESE_COV_TYPE = myElement.Where(c => c.Name == "CoveredMembers").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_COV_TYPE = ruleModel.SESE_COV_TYPE != "[Select One]" ? ruleModel.SESE_COV_TYPE : "";
                        ruleModel.SESE_COV_TYPE = ruleModel.SESE_COV_TYPE.Length > 0 ? ruleModel.SESE_COV_TYPE.Substring(0, 1) : "";

                        ruleModel.SESE_COV_EXCD_ID = myElement.Where(c => c.Name == "CoveredMembersExplanationCode").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_COV_EXCD_ID = ruleModel.SESE_COV_EXCD_ID != "[Select One]" ? ruleModel.SESE_COV_EXCD_ID : "";
                        ruleModel.SESE_COV_EXCD_ID = ruleModel.SESE_COV_EXCD_ID.Length > 3 ? ruleModel.SESE_COV_EXCD_ID.Substring(0, 3) : ruleModel.SESE_VALID_SEX;

                        ruleModel.SESE_RULE_TYPE = "";

                        ruleModel.SESE_CALC_IND = myElement.Where(c => c.Name == "CalculationIndicator").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_CALC_IND = ruleModel.SESE_CALC_IND != "[Select One]" ? ruleModel.SESE_CALC_IND : "";
                        ruleModel.SESE_CALC_IND = ruleModel.SESE_CALC_IND.Length > 0 ? ruleModel.SESE_CALC_IND.Substring(0, 1) : "";

                        string seqNo = myElement.Where(c => c.Name == "AgeTo").FirstOrDefault().Value.ToString();
                        seqNo = seqNo == "" ? "0" : seqNo;
                        ruleModel.WMDS_SEQ_NO = minAge != "[Select One]" ? Convert.ToInt32(seqNo) : 0;

                        ruleModel.SESE_DIS_EXCD_ID = "";

                        ruleModel.SESE_MAX_CPAY_PCT = 0;

                        ruleModel.SESE_FSA_REIMB_IND = myElement.Where(c => c.Name == "EligibleforFSAReimbursement").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_FSA_REIMB_IND = ruleModel.SESE_FSA_REIMB_IND != "[Select One]" ? ruleModel.SESE_FSA_REIMB_IND : "";
                        ruleModel.SESE_FSA_REIMB_IND = ruleModel.SESE_FSA_REIMB_IND.Length > 0 ? ruleModel.SESE_FSA_REIMB_IND.Substring(0, 1) : "";

                        ruleModel.SESE_HSA_REIMB_IND = myElement.Where(c => c.Name == "EligibleforHRAReimbursement").FirstOrDefault().Value.ToString() != null ? "" : myElement.Where(c => c.Name == "EligibleforHRAReimbursement").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_HSA_REIMB_IND = ruleModel.SESE_HSA_REIMB_IND != "[Select One]" ? ruleModel.SESE_HSA_REIMB_IND : "";
                        ruleModel.SESE_HSA_REIMB_IND = ruleModel.SESE_HSA_REIMB_IND.Length > 0 ? ruleModel.SESE_HSA_REIMB_IND.Substring(0, 1) : "";

                        ruleModel.SESE_HRA_DED_IND = myElement.Where(c => c.Name == "HRADeductibleApplies").FirstOrDefault().Value.ToString();
                        ruleModel.SESE_HRA_DED_IND = ruleModel.SESE_HRA_DED_IND != "[Select One]" ? ruleModel.SESE_HRA_DED_IND : "";
                        ruleModel.SESE_HRA_DED_IND = ruleModel.SESE_HRA_DED_IND.Length > 0 ? ruleModel.SESE_HRA_DED_IND.Substring(0, 1) : "";

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return ruleModel;
        }

        private SEPY481Model GenerateSEPYModel(string copay, string coinsurance, string sese_ID, string networkType, bool is_sese_Alt_Rule, TierModel tierModel, FacetRuleModel facetRuleModel)
        {
            SEPY481Model model = null;
            try
            {
                string setr_Rule = getSESERule(copay, coinsurance, tierModel, facetRuleModel);
                model = new SEPY481Model();
                string sepy_Pfx = networkTypePrefixList.Where(c => c.NetworkType == networkType).Select(c => c.Prefix).FirstOrDefault();
                model.SEPY_PFX = sepy_Pfx != null ? sepy_Pfx : "";
                model.SESE_ID = sese_ID;
                model.SEPY_EFF_DT = DateTime.Now;
                model.SEPY_TERM_DT = DateTime.Now;
                model.SESE_RULE = setr_Rule != "" ? setr_Rule : "PS0"; //sese_Rule; 
                model.SEPY_EXP_CAT = "";
                model.SEPY_ACCT_CAT = "";
                model.SEPY_OPTS = "";
                model.SESE_RULE_ALT = "";
                model.SESE_RULE_ALT_COND = "";
                if (is_sese_Alt_Rule)
                {
                    model.SESE_RULE_ALT = setr_Rule != "" ? setr_Rule : "PS0";
                    model.SESE_RULE_ALT_COND = "M";
                }
                model.SEPY_LOCK_TOKEN = 0;
                model.ATXR_SOURCE_ID = DateTime.Now;
                model.BatchID = BatchId;
                model.BenefitOfferingID = ProductId;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return model;
        }

        private string getSESERule(string copay, string coinsurance, TierModel tierModel, FacetRuleModel facetRuleModel)
        {
            try
            {
                DataTable tierDT = GetTierTableType(tierModel);
                DataTable facetruleDT = GetFacetRuleTableType(facetRuleModel);

                SqlParameter tierTypeData = new SqlParameter("@Tierdata", SqlDbType.Structured);
                tierTypeData.Value = tierDT;
                tierTypeData.TypeName = "[common].TierDataType";

                SqlParameter facetRuleType = new SqlParameter("@FacetRule", SqlDbType.Structured);
                facetRuleType.Value = facetruleDT;
                facetRuleType.TypeName = "[common].FacetRuleType";

                SqlParameter typeInd = new SqlParameter("TypeIndicator", tierModel.TypeIndicator == null ? ' ' : tierModel.TypeIndicator);
                SqlParameter serlId = new SqlParameter("SERLId", tierModel.SERLId == null ? "" : tierModel.SERLId);
                SqlParameter accNumber = new SqlParameter("AccumNumber", tierModel.AccumNumber == null ? "" : tierModel.AccumNumber);
                SqlParameter msgDesc = new SqlParameter("MessageDescription", tierModel.MessageDescription == null ? "" : tierModel.MessageDescription);

                string sese_Rule = this._unitOfWork.Repository<SESE481Stg>()
                                                        .SqlQuery("[common].SeseRuleMatch @TypeIndicator, @SERLId, @AccumNumber, @MessageDescription, @Tierdata, @FacetRule",
                                                         typeInd, serlId, accNumber, msgDesc, tierTypeData, facetRuleType);


                return sese_Rule;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return null;
        }

        private DataTable GetTierTableType(TierModel tierModel)
        {
            DataTable tierdataDT = new DataTable();
            if (tierModel.TierDataModelList != null)
            {
                tierdataDT.Columns.Add("AllowedAmount", typeof(string));
                tierdataDT.Columns.Add("AllowedCounter", typeof(string));
                tierdataDT.Columns.Add("Coinsurance", typeof(string));
                tierdataDT.Columns.Add("Copay", typeof(string));
                tierdataDT.Columns.Add("TierNo", typeof(string));

                foreach (var tModel in tierModel.TierDataModelList)
                {
                    DataRow dr = tierdataDT.NewRow();
                    dr["AllowedAmount"] = tModel.AllowedAmount;
                    dr["AllowedCounter"] = tModel.AllowedCounter;
                    dr["Coinsurance"] = tModel.Coinsurance;
                    dr["Copay"] = tModel.Copay;
                    dr["TierNo"] = tModel.TierNo;

                    tierdataDT.Rows.Add(dr);
                }
            }
            return tierdataDT;
        }


        private DataTable GetFacetRuleTableType(FacetRuleModel facetRuleModel)
        {

            DataTable facetRuledataDT = new DataTable();
            if (facetRuleModel != null)
            {
                facetRuledataDT.Columns.Add("SESE_ID", typeof(string));
                facetRuledataDT.Columns.Add("SESE_CM_IND", typeof(string));
                facetRuledataDT.Columns.Add("SESE_PA_AMT_REQ", typeof(string));
                facetRuledataDT.Columns.Add("SESE_PA_UNIT_REQ", typeof(string));
                facetRuledataDT.Columns.Add("SESE_PA_PROC_REQ", typeof(string));
                facetRuledataDT.Columns.Add("SESE_VALID_SEX", typeof(string));
                facetRuledataDT.Columns.Add("SESE_SEX_EXCD_ID", typeof(string));
                facetRuledataDT.Columns.Add("SESE_MIN_AGE", typeof(int));
                facetRuledataDT.Columns.Add("SESE_MAX_AGE", typeof(int));
                facetRuledataDT.Columns.Add("SESE_AGE_EXCD_ID", typeof(string));
                facetRuledataDT.Columns.Add("SESE_COV_TYPE", typeof(string));
                facetRuledataDT.Columns.Add("SESE_COV_EXCD_ID", typeof(string));
                facetRuledataDT.Columns.Add("SESE_RULE_TYPE", typeof(string));
                facetRuledataDT.Columns.Add("SESE_CALC_IND", typeof(string));
                facetRuledataDT.Columns.Add("SERL_REL_ID", typeof(string));
                facetRuledataDT.Columns.Add("WMDS_SEQ_NO", typeof(int));
                facetRuledataDT.Columns.Add("SESE_ID_XLOW", typeof(string));
                facetRuledataDT.Columns.Add("SESE_DESC_XLOW", typeof(string));
                facetRuledataDT.Columns.Add("SESE_DIS_EXCD_ID", typeof(string));
                facetRuledataDT.Columns.Add("SESE_MAX_CPAY_PCT", typeof(string));
                facetRuledataDT.Columns.Add("SESE_FSA_REIMB_IND", typeof(string));
                facetRuledataDT.Columns.Add("SESE_HSA_REIMB_IND", typeof(string));
                facetRuledataDT.Columns.Add("SESE_HRA_DED_IND", typeof(string));


                DataRow dr = facetRuledataDT.NewRow();
                dr["SESE_ID"] = facetRuleModel.SESE_ID;
                dr["SESE_CM_IND"] = facetRuleModel.SESE_CM_IND;
                dr["SESE_PA_AMT_REQ"] = facetRuleModel.SESE_PA_AMT_REQ;
                dr["SESE_PA_UNIT_REQ"] = facetRuleModel.SESE_PA_UNIT_REQ;
                dr["SESE_PA_PROC_REQ"] = facetRuleModel.SESE_PA_PROC_REQ;
                dr["SESE_VALID_SEX"] = facetRuleModel.SESE_VALID_SEX;
                dr["SESE_SEX_EXCD_ID"] = facetRuleModel.SESE_SEX_EXCD_ID;
                dr["SESE_MIN_AGE"] = facetRuleModel.SESE_MIN_AGE;
                dr["SESE_MAX_AGE"] = facetRuleModel.SESE_MAX_AGE;
                dr["SESE_AGE_EXCD_ID"] = facetRuleModel.SESE_AGE_EXCD_ID;
                dr["SESE_COV_TYPE"] = facetRuleModel.SESE_COV_TYPE;
                dr["SESE_COV_EXCD_ID"] = facetRuleModel.SESE_COV_EXCD_ID;
                dr["SESE_RULE_TYPE"] = facetRuleModel.SESE_RULE_TYPE;
                dr["SESE_CALC_IND"] = facetRuleModel.SESE_CALC_IND;
                dr["SERL_REL_ID"] = facetRuleModel.SERL_REL_ID;
                dr["WMDS_SEQ_NO"] = facetRuleModel.WMDS_SEQ_NO;
                dr["SESE_ID_XLOW"] = facetRuleModel.SESE_ID_XLOW;
                dr["SESE_DESC_XLOW"] = facetRuleModel.SESE_DESC_XLOW;
                dr["SESE_DIS_EXCD_ID"] = facetRuleModel.SESE_DIS_EXCD_ID;
                dr["SESE_MAX_CPAY_PCT"] = facetRuleModel.SESE_MAX_CPAY_PCT;
                dr["SESE_FSA_REIMB_IND"] = facetRuleModel.SESE_FSA_REIMB_IND;
                dr["SESE_HSA_REIMB_IND"] = facetRuleModel.SESE_HSA_REIMB_IND;
                dr["SESE_HRA_DED_IND"] = facetRuleModel.SESE_HRA_DED_IND;

                facetRuledataDT.Rows.Add(dr);

            }
            return facetRuledataDT;
        }


        /// <summary>
        /// Get LTLT Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetLTLTDetails(Facet481Model facet481Model)
        {
            try
            {
                if (getProductResponse.Limits.LimitsList != null)
                {
                    List<LTLT481Model> ltLTist = new List<LTLT481Model>();
                    List<LTIP481Model> ltIPList = new List<LTIP481Model>();
                    List<LTID481Model> ltIDList = new List<LTID481Model>();
                    List<LTPR481Model> ltPRList = new List<LTPR481Model>();
                    List<LTSE481Model> ltSEList = new List<LTSE481Model>();

                    foreach (var limitlist in getProductResponse.Limits.LimitsList.ToList())
                    {
                        bool isIncluded = (limitlist.SelectLimit == "Yes" ? true : false);
                        if (isIncluded)
                        {
                            var ltltdata = GetLTLTData(limitlist.LimitDescription, limitlist.Value);
                            if (ltltdata != null)
                            {
                                ltLTist.Add(ltltdata);

                                var ltIPdata = GetLTIPData(limitlist.LimitDescription, ltltdata.ACAC_ACC_NO);

                                if (ltIPdata != null)
                                {
                                    ltIPdata.LTLT_PFX = ltltdata.LTLT_PFX;
                                    ltIPList.Add(ltIPdata);
                                }

                                var ltIDdata = GetLTIDData(limitlist.LimitDescription, ltltdata.ACAC_ACC_NO);
                                if (ltIDdata != null)
                                {
                                    ltIDdata.LTLT_PFX = ltltdata.LTLT_PFX;
                                    ltIDList.Add(ltIDdata);
                                }

                                var ltPRdata = GetLTPRData(limitlist.LimitDescription, ltltdata.ACAC_ACC_NO);
                                if (ltPRdata != null)
                                {
                                    ltPRdata.LTLT_PFX = ltltdata.LTLT_PFX;
                                    ltPRList.Add(ltPRdata);
                                }

                                var ltSEdata = GetLTSEData(limitlist.LimitDescription, ltltdata.ACAC_ACC_NO, ltltdata.LTLT_PFX);
                                if (ltSEdata != null)
                                {
                                    ltSEList.AddRange(ltSEdata);
                                }
                            }

                        }
                    }

                    facet481Model.LTLT481 = ltLTist;
                    facet481Model.LTIP481 = ltIPList;
                    facet481Model.LTID481 = ltIDList;
                    facet481Model.LTPR481 = ltPRList;
                    facet481Model.LTSE481 = ltSEList;
                }
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private LTLT481Model GetLTLTData(string desc, string value)
        {
            LTLT481Model model = null;
            try
            {
                var limitlistdata = getProductResponse.Limits.FacetsLimits.LimitRulesLTLT.Where(x => x.LimitDescription == desc).FirstOrDefault();

                if (limitlistdata != null)
                {
                    model = new LTLT481Model();

                    model.ACAC_ACC_NO = (short)Convert.ToInt16(limitlistdata.AccumulatorNo != "" ? limitlistdata.AccumulatorNo : "0");
                    model.ATXR_SOURCE_ID = ATXR_SOURCE_ID;

                    model.EXCD_ID = limitlistdata.ExplanationCode;
                    model.EXCD_ID = model.EXCD_ID != "" ? model.EXCD_ID.Substring(0, 3) : "";

                    model.LTLT_AMT1 = Convert.ToDecimal(limitlistdata.LimitAmountSalaryPct != "" ? limitlistdata.LimitAmountSalaryPct : "0");
                    model.LTLT_AMT2 = Convert.ToDecimal(limitlistdata.AlternateAmount != "" ? limitlistdata.AlternateAmount : "0");

                    model.LTLT_CAT = limitlistdata.Category;
                    model.LTLT_CAT = model.LTLT_CAT != "" ? model.LTLT_CAT.Substring(0, 1) : "";

                    model.LTLT_DAYS = limitlistdata.Days == "" ? (short)0 : (short)Convert.ToInt16(limitlistdata.Days);
                    model.LTLT_DESC = limitlistdata.LimitDescription;

                    model.LTLT_IX_IND = limitlistdata.Relations;
                    model.LTLT_IX_IND = model.LTLT_IX_IND != "" ? model.LTLT_IX_IND.Substring(0, 1) : "";

                    model.LTLT_IX_TYPE = limitlistdata.Subsection;
                    model.LTLT_IX_TYPE = model.LTLT_IX_TYPE != "" ? model.LTLT_IX_TYPE.Substring(0, 1) : "";

                    model.LTLT_LEVEL = limitlistdata.Level;
                    model.LTLT_LEVEL = model.LTLT_LEVEL != "" ? model.LTLT_LEVEL.Substring(0, 1) : "";

                    model.LTLT_LOCK_TOKEN = TOKEN;
                    model.LTLT_OPTS = "";

                    model.LTLT_PERIOD_IND = limitlistdata.Period;
                    model.LTLT_PERIOD_IND = model.LTLT_PERIOD_IND != "" ? model.LTLT_PERIOD_IND.Substring(0, 1) : "";

                    model.LTLT_RULE = limitlistdata.Rule;
                    model.LTLT_RULE = model.LTLT_RULE != "" ? model.LTLT_RULE.Substring(0, 1) : "";

                    var pfx = GetPrefixCounter("LTLT");
                    var newPfx = Convert.ToInt32(pfx) + 1;
                    model.LTLT_PFX = Util.ConvertToBase36(newPfx);
                    model.LTLT_SAL_IND = limitlistdata.SalaryBased;
                    model.LTLT_SAL_IND = model.LTLT_SAL_IND != "" ? model.LTLT_SAL_IND.Substring(0, 1) : "";

                    model.SYS_DBUSER_ID = "";
                    model.SYS_LAST_UPD_DTM = DateTime.Now;
                    model.SYS_USUS_ID = "";
                    model.WMDS_SEQ_NO = (short)Convert.ToInt16(limitlistdata.UserMessage != "" ? limitlistdata.UserMessage : "0");

                    model.BatchID = BatchId;
                    model.BenefitOfferingID = ProductId;

                    return model;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return model;
        }

        private LTIP481Model GetLTIPData(string desc, int acac_ACC_NO)
        {
            LTIP481Model model = null;
            try
            {
                var limitlistdata = getProductResponse.Limits.FacetsLimits.LimitProcedureTableLTIP.Where(x => x.LimitDescription == desc).FirstOrDefault();
                if (limitlistdata != null)
                {
                    model = new LTIP481Model();

                    model.ACAC_ACC_NO = acac_ACC_NO;
                    model.LTIP_IPCD_ID_LOW = limitlistdata.RelatedProcedureCodeLow;
                    model.LTIP_IPCD_ID_HIGH = limitlistdata.RelatedProcedureCodeHigh;

                    model.ATXR_SOURCE_ID = ATXR_SOURCE_ID;
                    model.BatchID = BatchId;
                    model.BenefitOfferingID = ProductId;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return model;
        }

        private LTID481Model GetLTIDData(string desc, int acac_ACC_NO)
        {
            LTID481Model model = null;
            try
            {
                var limitlistdata = getProductResponse.Limits.FacetsLimits.LimitDiagnosisTableLTID.Where(x => x.LimitDescription == desc).FirstOrDefault();
                if (limitlistdata != null)
                {
                    model = new LTID481Model();

                    model.ACAC_ACC_NO = acac_ACC_NO;
                    model.IDCD_ID_REL = limitlistdata.RelatedDiagnosisCode != null ? limitlistdata.RelatedDiagnosisCode : "";

                    model.LTID_LOCK_TOKEN = TOKEN;
                    model.ATXR_SOURCE_ID = ATXR_SOURCE_ID;
                    model.SYS_DBUSER_ID = "Admin";
                    model.SYS_USUS_ID = "Admin";
                    model.SYS_LAST_UPD_DTM = DateTime.Now;
                    model.BatchID = BatchId;
                    model.BenefitOfferingID = ProductId;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return model;
        }

        private LTPR481Model GetLTPRData(string desc, int acac_ACC_NO)
        {
            LTPR481Model model = null;
            try
            {
                var limitlistdata = getProductResponse.Limits.FacetsLimits.LimitProviderTableLTPR.Where(x => x.LimitDescription == desc).FirstOrDefault();
                if (limitlistdata != null)
                {
                    model = new LTPR481Model();

                    model.ACAC_ACC_NO = acac_ACC_NO;
                    model.PRPR_MCTR_TYPE = "0";

                    model.LTPR_LOCK_TOKEN = TOKEN;
                    model.ATXR_SOURCE_ID = ATXR_SOURCE_ID;
                    model.SYS_DBUSER_ID = "Admin";
                    model.SYS_USUS_ID = "Admin";
                    model.SYS_LAST_UPD_DTM = DateTime.Now;
                    model.BatchID = BatchId;
                    model.BenefitOfferingID = ProductId;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return model;
        }

        private List<LTSE481Model> GetLTSEData(string desc, int acac_ACC_NO, string pfx)
        {
            List<LTSE481Model> modelList = null;
            try
            {

                JArray serviceList = masterlist.Limits.LimitServices;
                if (serviceList != null)
                {
                    JArray masterServiceListData = masterlist.Services.ServiceList;

                    modelList = new List<LTSE481Model>();
                    LTSE481Model model;

                    //iterate service list to found the matched description : TODO : can be optimize
                    foreach (var ltse in serviceList.Children())
                    {
                        var itemProperty = ltse.Children<JProperty>().ToList();

                        var lmtDesc = itemProperty.Where(c => c.Name == "LimitDescription").FirstOrDefault().Value.ToString();

                        if (lmtDesc == desc)
                        {
                            //iterate service list to found the selected one : TODO : can be optimize
                            foreach (var childOfService in ltse["MasterListServices"])
                            {

                                //var lmtProperties = childOfService.FirstOrDefault().Children<JProperty>().ToList();
                                var isSelect = childOfService["SelectService"].ToString();

                                if (isSelect == "Yes")
                                {
                                    var bc1 = childOfService["BenefitCategory1"].ToString();
                                    var bc2 = childOfService["BenefitCategory2"].ToString();
                                    var bc3 = childOfService["BenefitCategory3"].ToString();
                                    var pos = childOfService["PlaceofService"].ToString();

                                    //iterate service list to found the matched bc1, bc2m bc3 and pos : TODO : can be optimize
                                    foreach (var masterService in masterServiceListData)
                                    {
                                        if (masterService["BenefitCategory1"].ToString() == bc1 && masterService["BenefitCategory2"].ToString() == bc2
                                            && masterService["BenefitCategory3"].ToString() == bc3 && masterService["PlaceofService"].ToString() == pos)
                                        {

                                            model = new LTSE481Model();

                                            model.ACAC_ACC_NO = acac_ACC_NO;
                                            model.SESE_ID = masterService["SESEID"].ToString();
                                            model.LTSE_WT_CTR = 0;
                                            model.LTLT_PFX = pfx;

                                            model.LTSE_LOCK_TOKEN = TOKEN;
                                            model.ATXR_SOURCE_ID = ATXR_SOURCE_ID;
                                            model.SYS_DBUSER_ID = "Admin";
                                            model.SYS_USUS_ID = "Admin";
                                            model.SYS_LAST_UPD_DTM = DateTime.Now;
                                            model.BatchID = BatchId;
                                            model.BenefitOfferingID = ProductId;

                                            modelList.Add(model);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return modelList;
        }

        /// <summary>
        /// Get DEDE Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetDEDEDetails(Facet481Model facet481Model)
        {
            try
            {
                var dedeDataList = getProductResponse.CostShare.Deductible.FacetServiceSpecificDeductible.FacetServiceSpecificDeductibleList;
                bool isServiceSpecificDeducitbleDefined = false;
                DEDE481Model modelDEDE481;
                List<DEDE481Model> dede481ModelList = new List<DEDE481Model>();

                if (dedeDataList != null)
                {

                    foreach (var dedeData in dedeDataList)
                    {
                        bool isIncluded = (dedeData.Include == "Yes" ? true : false);
                        if (isIncluded)
                        {
                            isServiceSpecificDeducitbleDefined = true;
                            modelDEDE481 = new DEDE481Model();
                            var pfx = GetPrefixCounter("DEDE");
                            var newPfx = Convert.ToInt32(pfx) + 1;
                            modelDEDE481.DEDE_PFX = Util.ConvertToBase36(newPfx);
                            modelDEDE481.ACAC_ACC_NO = dedeData.DeductibleRulesAccumulatorNumber == "" ? 0 : Convert.ToInt32(dedeData.DeductibleRulesAccumulatorNumber);
                            modelDEDE481.DEDE_DESC = dedeData.DeductibleRulesDescription;
                            modelDEDE481.DEDE_RULE = dedeData.DeductibleRule == "" ? 0 : Convert.ToInt32(dedeData.DeductibleRule.Substring(0, 1));
                            modelDEDE481.DEDE_REL_ACC_ID = 0;

                            modelDEDE481.DEDE_COB_OOP_IND = dedeData.COBMedicareOutofPocketIndicator;
                            modelDEDE481.DEDE_COB_OOP_IND = modelDEDE481.DEDE_COB_OOP_IND != "" ? modelDEDE481.DEDE_COB_OOP_IND.Substring(0, 1) : "";

                            modelDEDE481.DEDE_SL_IND = dedeData.DeductibleStoplossAccumulation;
                            modelDEDE481.DEDE_SL_IND = modelDEDE481.DEDE_SL_IND != "" ? modelDEDE481.DEDE_SL_IND.Substring(0, 1) : "";

                            modelDEDE481.DEDE_PERIOD_IND = dedeData.PlanYearorCalendarYear;
                            modelDEDE481.DEDE_PERIOD_IND = modelDEDE481.DEDE_PERIOD_IND != "" ? modelDEDE481.DEDE_PERIOD_IND.Substring(0, 1) : "";

                            modelDEDE481.DEDE_AGG_PERSON = 0;
                            modelDEDE481.DEDE_AGG_PERSON_CO = 0;
                            modelDEDE481.DEDE_FAM_AMT = Convert.ToDecimal(dedeData.Family.Replace("$", "").Trim());
                            modelDEDE481.DEDE_FAM_AMT_CO = Convert.ToDecimal(dedeData.FamilyCarryOver.Replace("$", "").Trim());
                            modelDEDE481.DEDE_MEME_AMT = Convert.ToDecimal(dedeData.Single.Replace("$", "").Trim());
                            modelDEDE481.DEDE_MEME_AMT_CO = Convert.ToDecimal(dedeData.SingleCarryOver.Replace("$", "").Trim()); ;
                            modelDEDE481.DEDE_OPTS = "";

                            modelDEDE481.DEDE_CO_BYPASS = (dedeData.Apply4thQuarterCarryOver == "Yes" ? "Y" : "N");
                            modelDEDE481.DEDE_MEM_SAL_IND = (dedeData.MemberDeductibleisSalaryBased == "Yes" ? "Y" : "N");
                            modelDEDE481.DEDE_FAM_SAL_IND = (dedeData.FamilyDeductibleisSalaryBased == "Yes" ? "Y" : "N");
                            modelDEDE481.DEDE_LOCK_TOKEN = TOKEN;
                            modelDEDE481.ATXR_SOURCE_ID = ATXR_SOURCE_ID;
                            modelDEDE481.SYS_LAST_UPD_DTM = DateTime.Now;
                            modelDEDE481.SYS_USUS_ID = "";
                            modelDEDE481.SYS_DBUSER_ID = "";
                            modelDEDE481.BatchID = BatchId;
                            modelDEDE481.BenefitOfferingID = ProductId;

                            dede481ModelList.Add(modelDEDE481);
                        }
                    }
                    facet481Model.DEDE481 = dede481ModelList;
                }


                //if service specific deductible is not  defined take the default deductible defined
                if (!isServiceSpecificDeducitbleDefined)
                {
                    var singleAmount1 = 0.0;
                    var singleAmount2 = 0.0;
                    var singleCarryover1 = 0.0;
                    var singleCarryover2 = 0.0;
                    var familyAmount1 = 0.0;
                    var familyCarryover1 = 0.0;
                    var familyAmount2 = 0.0;
                    var familyCarryover2 = 0.0;

                    //Get  the individual and  family amount for both the networks
                    var defaultDeductibleAmount = getProductResponse.CostShare.Deductible.DeductibleAmountDS.DeductibleAmount;
                    if (defaultDeductibleAmount != null && defaultDeductibleAmount.Count > 0)
                    {
                        foreach (var amt in defaultDeductibleAmount)
                        {
                            if (amt.DeductibleType != null && amt.DeductibleType == "Individual")
                            {
                                var dedeNetworks = amt.QHPFACETSNetworkDS;
                                if (dedeNetworks != null && dedeNetworks.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(dedeNetworks[0].Amount))
                                        singleAmount1 = Convert.ToDouble(dedeNetworks[0].Amount.Replace("$", ""));

                                    if (!string.IsNullOrEmpty(dedeNetworks[1].Amount))
                                        singleAmount2 = Convert.ToDouble(dedeNetworks[1].Amount.Replace("$", ""));
                                }
                            }
                            else if (amt.DeductibleType != null && amt.DeductibleType == "Family")
                            {
                                var dedeNetworks = amt.QHPFACETSNetworkDS;
                                if (dedeNetworks != null && dedeNetworks.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(dedeNetworks[0].Amount))
                                        familyAmount1 = Convert.ToDouble(dedeNetworks[0].Amount.Replace("$", ""));

                                    if (!string.IsNullOrEmpty(dedeNetworks[1].Amount))
                                        familyAmount2 = Convert.ToDouble(dedeNetworks[1].Amount.Replace("$", ""));
                                }
                            }
                        }
                    }

                    //Get the individual and family  carryover amount for both the networks
                    var defaultDeductibleCarryover = getProductResponse.CostShare.Deductible.DeductibleCarryOverAmountforFacets.DeductibleCarryOverAmount;
                    if (defaultDeductibleCarryover != null && defaultDeductibleCarryover.Count > 0)
                    {
                        foreach (var amt in defaultDeductibleCarryover)
                        {
                            if (amt.DeductibleType != null && amt.DeductibleType == "Individual")
                            {
                                var dedeNetworks = amt.QHPFACETSNetworkDS;
                                if (dedeNetworks != null && dedeNetworks.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(dedeNetworks[0].Amount))
                                        singleCarryover1 = Convert.ToDouble(dedeNetworks[0].Amount.Replace("$", ""));

                                    if (!string.IsNullOrEmpty(dedeNetworks[1].Amount))
                                        singleCarryover2 = Convert.ToDouble(dedeNetworks[1].Amount.Replace("$", ""));
                                }
                            }
                            else if (amt.DeductibleType != null && amt.DeductibleType == "Family")
                            {
                                var dedeNetworks = amt.QHPFACETSNetworkDS;
                                if (dedeNetworks != null && dedeNetworks.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(dedeNetworks[0].Amount))
                                        familyCarryover1 = Convert.ToDouble(dedeNetworks[0].Amount.Replace("$", ""));

                                    if (!string.IsNullOrEmpty(dedeNetworks[1].Amount))
                                        familyCarryover2 = Convert.ToDouble(dedeNetworks[1].Amount.Replace("$", ""));
                                }
                            }
                        }
                    }

                    //Create Deductible Set
                    var defaultDeductibleRule = getProductResponse.CostShare.Deductible.DeductibleRules;
                    if (defaultDeductibleRule != null)
                    {
                        //Create deductible 1 for first network
                        modelDEDE481 = new DEDE481Model();
                        var modelDEDE481_2 = new DEDE481Model();
                        var pfx = GetPrefixCounter("DEDE");
                        var newPfx = Convert.ToInt32(pfx) + 1;
                        //Create facet deductible object for first network
                        modelDEDE481 = GetFacetDeductible(newPfx, defaultDeductibleRule, singleAmount1, singleCarryover1, familyAmount1, familyCarryover1, BatchId);
                        dede481ModelList.Add(modelDEDE481);

                        //Create  deductible 2 for second network                          
                        modelDEDE481_2 = GetFacetDeductible(newPfx, defaultDeductibleRule, singleAmount2, singleCarryover2, familyAmount2, familyCarryover2, BatchId);
                        dede481ModelList.Add(modelDEDE481_2);
                    }
                }
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private DEDE481Model GetFacetDeductible(int pfx, DeductibleRules defaultDeductibleRule, double singleAmount, double singleCarryover, double familyAmount, double familyCarryover, string BatchId)
        {
            var dedeModel2 = new DEDE481Model();
            dedeModel2.DEDE_PFX = Util.ConvertToBase36(pfx);
            dedeModel2.ACAC_ACC_NO = defaultDeductibleRule.DeductibleRulesAccumulatorNumber == "" ? 0 : Convert.ToInt32(defaultDeductibleRule.DeductibleRulesAccumulatorNumber);
            dedeModel2.DEDE_DESC = defaultDeductibleRule.DeductibleRulesDescription;
            dedeModel2.DEDE_RULE = defaultDeductibleRule.DeductibleRule == "" ? 0 : Convert.ToInt32(defaultDeductibleRule.DeductibleRule.Substring(0, 1));
            dedeModel2.DEDE_REL_ACC_ID = 0;

            dedeModel2.DEDE_COB_OOP_IND = defaultDeductibleRule.COBMedicareOutofPocketIndicator;
            dedeModel2.DEDE_COB_OOP_IND = dedeModel2.DEDE_COB_OOP_IND != "" ? dedeModel2.DEDE_COB_OOP_IND.Substring(0, 1) : "";

            dedeModel2.DEDE_SL_IND = defaultDeductibleRule.DeductibleStoplossAccumulation;
            dedeModel2.DEDE_SL_IND = dedeModel2.DEDE_SL_IND != "" ? dedeModel2.DEDE_SL_IND.Substring(0, 1) : "";

            dedeModel2.DEDE_PERIOD_IND = defaultDeductibleRule.PlanYearorCalendarYear;
            dedeModel2.DEDE_PERIOD_IND = dedeModel2.DEDE_PERIOD_IND != "" ? dedeModel2.DEDE_PERIOD_IND.Substring(0, 1) : "";

            dedeModel2.DEDE_AGG_PERSON = 0;
            dedeModel2.DEDE_AGG_PERSON_CO = 0;
            dedeModel2.DEDE_FAM_AMT = Convert.ToDecimal(familyAmount);
            dedeModel2.DEDE_FAM_AMT_CO = Convert.ToDecimal(familyCarryover);
            dedeModel2.DEDE_MEME_AMT = Convert.ToDecimal(singleAmount);
            dedeModel2.DEDE_MEME_AMT_CO = Convert.ToDecimal(singleCarryover);
            dedeModel2.DEDE_OPTS = "";

            dedeModel2.DEDE_CO_BYPASS = (defaultDeductibleRule.Apply4thQuarterCarryOver == "Yes" ? "Y" : "N");
            dedeModel2.DEDE_MEM_SAL_IND = (defaultDeductibleRule.MemberDeductibleisSalaryBased == "Yes" ? "Y" : "N");
            dedeModel2.DEDE_FAM_SAL_IND = (defaultDeductibleRule.FamilyDeductibleisSalaryBased == "Yes" ? "Y" : "N");
            dedeModel2.DEDE_LOCK_TOKEN = TOKEN;
            dedeModel2.ATXR_SOURCE_ID = ATXR_SOURCE_ID;
            dedeModel2.SYS_LAST_UPD_DTM = DateTime.Now;
            dedeModel2.SYS_USUS_ID = "";
            dedeModel2.SYS_DBUSER_ID = "";
            dedeModel2.BatchID = BatchId;
            dedeModel2.BenefitOfferingID = ProductId;

            return dedeModel2;
        }

        private Facet481Model GetSERLDetails(Facet481Model facet481Model)
        {
            try
            {
                var msgListData = getProductResponse.Messages.MessageServiceList;
                bool isIncluded = false;
                if (msgListData != null)
                {
                    List<SERL481Model> msgModelList = new List<SERL481Model>();
                    SERL481Model msgModel;
                    foreach (var msgData in msgListData)
                    {
                        isIncluded = (msgData.Include == "Yes" ? true : false);
                        if (isIncluded)
                        {


                            var fds = masterlist.Messages.RelatedServiceMessagesSERL;
                            var serlMessages = msgData.QHPFACETSNetworkDS.ToList().Select(c => c.MessageSERL).Distinct().ToList();
                            foreach (var message in serlMessages)
                            {
                                JArray serviceList = masterlist.Messages.RelatedServiceMessagesSERL;

                                foreach (var item in serviceList.Children())
                                {
                                    var itemProperty = item.Children<JProperty>();
                                    var myElement = itemProperty.ToList();

                                    string description = myElement.Where(c => c.Name == "Description").FirstOrDefault().Value.ToString();
                                    description = description != "[Select One]" ? description : "";

                                    if (description == message)
                                    {
                                        msgModel = new SERL481Model();
                                        msgModel.SERL_REL_ID = myElement.Where(c => c.Name == "RelatedServiceID").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_REL_ID = msgModel.SERL_REL_ID != "[Select One]" ? msgModel.SERL_REL_ID : "";

                                        msgModel.SERL_REL_TYPE = myElement.Where(c => c.Name == "ServiceRelationType").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_REL_TYPE = msgModel.SERL_REL_TYPE != "[Select One]" && msgModel.SERL_REL_TYPE != "" ? msgModel.SERL_REL_TYPE.Substring(0, 1) : "";

                                        msgModel.SERL_REL_PER_IND = myElement.Where(c => c.Name == "ServiceRelatedPeriodIndicator").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_REL_PER_IND = msgModel.SERL_REL_PER_IND != "[Select One]" && msgModel.SERL_REL_PER_IND != "" ? msgModel.SERL_REL_PER_IND.Substring(0, 1) : "";

                                        msgModel.SERL_DIAG_IND = myElement.Where(c => c.Name == "RelateServicebyDiagnosis").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_DIAG_IND = msgModel.SERL_DIAG_IND != "[Select One]" && msgModel.SERL_DIAG_IND != "" ? msgModel.SERL_DIAG_IND.ToLower() == "yes" ? "Y" : "N" : "";

                                        msgModel.SERL_NTWK_IND = myElement.Where(c => c.Name == "ServiceRelatedParameterNetworkIndicator").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_NTWK_IND = msgModel.SERL_NTWK_IND != "[Select One]" && msgModel.SERL_NTWK_IND != "" ? msgModel.SERL_NTWK_IND : "";

                                        msgModel.SERL_PC_IND = myElement.Where(c => c.Name == "ServiceRelatedParameterPreauthorizationIndicator").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_PC_IND = msgModel.SERL_PC_IND != "[Select One]" && msgModel.SERL_PC_IND != "" ? msgModel.SERL_PC_IND.Substring(0, 1) : "";

                                        msgModel.SERL_REF_IND = myElement.Where(c => c.Name == "ServiceParameterReferralIndicator").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_REF_IND = msgModel.SERL_REF_IND != "[Select One]" && msgModel.SERL_REF_IND != "" ? msgModel.SERL_REF_IND.Substring(0, 1) : "";

                                        string serl_Per = myElement.Where(c => c.Name == "RelatedPeriod").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_PER = msgModel.SERL_REF_IND != "[Select One]" ? Convert.ToInt16(serl_Per) : (short)0;

                                        msgModel.SERL_COPAY_IND = myElement.Where(c => c.Name == "ServiceParameterReferralIndicator").FirstOrDefault().Value.ToString();
                                        msgModel.SERL_COPAY_IND = msgModel.SERL_COPAY_IND != "[Select One]" && msgModel.SERL_COPAY_IND != "" ? msgModel.SERL_COPAY_IND.Substring(0, 1) : "";

                                        msgModel.SERL_OPTS = "";

                                        msgModel.SERL_DESC = description;

                                        msgModel.SERL_LOCK_TOKEN = TOKEN;
                                        msgModel.ATXR_SOURCE_ID = DateTime.Now;
                                        msgModel.SYS_LAST_UPD_DTM = DateTime.Now;
                                        msgModel.SYS_USUS_ID = "";
                                        msgModel.SYS_DBUSER_ID = "";
                                        msgModel.BatchID = BatchId;
                                        msgModel.BenefitOfferingID = ProductId;

                                        msgModelList.Add(msgModel);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    facet481Model.SERL481 = msgModelList;
                }
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return facet481Model;
        }

        private string GetPrefixCounter(string entityName)
        {
            try
            {
                var preFixEnity = (from c in this._unitOfWork.Repository<Data.PrefixCounter>()
                                                                           .Get()
                                                                           .Where(x => x.Entity == entityName)
                                   select new
                                 {
                                     preFix = c.Prefix
                                 }).FirstOrDefault();

                return preFixEnity.preFix;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return null;
        }

        private void UpdatePrefixCounter(string entityName, string preFix)
        {
            try
            {
                Data.PrefixCounter preFixEnity = this._unitOfWork.Repository<Data.PrefixCounter>().Get().Where(x => x.Entity == entityName).FirstOrDefault();
                int calPrefix = Util.ConvertBase10(preFix);
                preFixEnity.Prefix = calPrefix.ToString();
                this._unitOfWork.Repository<Data.PrefixCounter>().Update(preFixEnity);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        /// <summary>
        /// Get SESE_SETR Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetSESE_SETRDetails(Facet481Model facet481Model)
        {
            try
            {
                if (getMasterListResponse.ServicesData.ServiceListData != null)
                {
                    List<SESE481Model> SESEList = new List<SESE481Model>();
                    int count = 1;
                    foreach (var item in getMasterListResponse.ServicesData.ServiceListData.ToList())
                    {
                        SESE481Model model = new SESE481Model();
                        model.ATXR_SOURCE_ID = DateTime.Now;
                        model.BatchID = BatchId;
                        model.BenefitOfferingID = 1;
                        model.SERL_REL_ID = "";

                        model.SESE_AGE_EXCD_ID = item.AgeExplanationCode;
                        model.SESE_AGE_EXCD_ID = model.SESE_AGE_EXCD_ID != "" ? model.SESE_AGE_EXCD_ID.Substring(0, 3) : "";

                        model.SESE_CALC_IND = item.CalculationIndicator;
                        model.SESE_CALC_IND = model.SESE_CALC_IND != "" ? model.SESE_CALC_IND.Substring(0, 1) : "";

                        model.SESE_CM_IND = item.DisplayCaseManagementWarningMessage ? "1" : "0";

                        model.SESE_COV_EXCD_ID = item.CoveredMembersExplanationCode;
                        model.SESE_COV_EXCD_ID = model.SESE_COV_EXCD_ID != "" ? model.SESE_COV_EXCD_ID.Substring(0, 3) : "";

                        model.SESE_COV_TYPE = item.CoveredMembers;
                        model.SESE_COV_TYPE = model.SESE_COV_TYPE != "" ? model.SESE_COV_TYPE.Substring(0, 1) : "";

                        model.SESE_DESC = "";
                        model.SESE_DESC_XLOW = "";
                        model.SESE_DIS_EXCD_ID = "";
                        model.SESE_FSA_REIMB_IND = item.EligibleforFSAReimbursement ? "1" : "0";

                        model.SESE_HRA_DED_IND = item.HRADeductibleApplies ? "1" : "0";

                        model.SESE_HSA_REIMB_IND = item.EligibleforHRAReimbursement;
                        model.SESE_HSA_REIMB_IND = model.SESE_HSA_REIMB_IND != "" ? model.SESE_HSA_REIMB_IND.Substring(0, 1) : "";

                        model.SESE_ID = item.SESEID;
                        model.SESE_ID = model.SESE_ID == "" ? count.ToString() : model.SESE_ID.Length > 4 ? model.SESE_ID.Substring(0, 4) : model.SESE_ID;

                        model.SESE_ID_XLOW = "";
                        model.SESE_LOCK_TOKEN = TOKEN;
                        model.SESE_MIN_AGE = (short)Convert.ToInt16(item.AgeFrom != "" ? item.AgeFrom : "0");
                        model.SESE_MAX_AGE = (short)Convert.ToInt16(item.AgeTo != "" ? item.AgeTo : "0");
                        model.SESE_OPTS = "";

                        model.SESE_PA_AMT_REQ = item.PreAuthChargeRequired ? "1" : "0";

                        model.SESE_PA_PROC_REQ = item.PreAuthProcedureRequired ? "1" : "0";

                        model.SESE_PA_UNIT_REQ = item.PreAuthUnitsRequired ? "1" : "0";

                        model.SESE_RULE = "";

                        model.SESE_RULE_TYPE = "";

                        model.SESE_SEX_EXCD_ID = item.GenderExplanationCode;
                        model.SESE_SEX_EXCD_ID = model.SESE_SEX_EXCD_ID != "" ? model.SESE_SEX_EXCD_ID.Substring(0, 3) : "";

                        model.SESE_VALID_SEX = item.Gender;
                        model.SESE_VALID_SEX = model.SESE_VALID_SEX != "" ? model.SESE_VALID_SEX.Substring(0, 1) : "";

                        model.SYS_DBUSER_ID = "";
                        model.SYS_LAST_UPD_DTM = DateTime.Now;
                        model.SYS_USUS_ID = "";
                        model.WMDS_SEQ_NO = (short)Convert.ToInt16(item.UserMessage != "" ? item.UserMessage : "0");
                        SESEList.Add(model);
                        count++;
                    }
                    facet481Model.SESE481 = SESEList;
                }
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        /// <summary>
        /// Get LTSE Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetLTSEDetails(Facet481Model facet481Model)
        {
            try
            {
                if (getMasterListResponse.LimitsData.LTLTListData != null)
                {
                    List<LTSE481Model> LTSEList = new List<LTSE481Model>();
                    int count = 1;
                    foreach (var item in getMasterListResponse.LimitsData.LTSEListData.ToList())
                    {
                        LTSE481Model model = new LTSE481Model();
                        model.ACAC_ACC_NO = Convert.ToInt32(item.LTSEAccumulator);
                        model.ATXR_SOURCE_ID = DateTime.Now;
                        model.SESE_ID = item.ServiceID;
                        model.SESE_ID = model.SESE_ID != "" ? model.SESE_ID : "";
                        model.BatchID = BatchId;
                        model.BenefitOfferingID = 1;
                        model.LTLT_PFX = count.ToString();
                        model.LTSE_LOCK_TOKEN = TOKEN;
                        model.LTSE_WT_CTR = Convert.ToInt32(item.WeightedCounter);
                        model.SYS_DBUSER_ID = "";
                        model.SYS_LAST_UPD_DTM = DateTime.Now;
                        model.SYS_USUS_ID = "";
                        LTSEList.Add(model);
                        count++;
                    }
                    facet481Model.LTSE481 = LTSEList;
                }
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        /// <summary>
        /// Get LTID Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetLTIDDetails(Facet481Model facet481Model)
        {
            try
            {
                if (getMasterListResponse.LimitsData.LTLTListData != null)
                {
                    List<LTID481Model> LTIDList = new List<LTID481Model>();
                    int count = 1;
                    foreach (var item in getMasterListResponse.LimitsData.LTIDListData.ToList())
                    {
                        LTID481Model model = new LTID481Model();
                        model.ACAC_ACC_NO = Convert.ToInt32(item.LTIDAccumulator);
                        model.ATXR_SOURCE_ID = DateTime.Now;
                        model.BatchID = BatchId;
                        model.BenefitOfferingID = 1;
                        model.LTLT_PFX = count.ToString();
                        model.IDCD_ID_REL = "";
                        model.LTID_LOCK_TOKEN = TOKEN;
                        model.SYS_DBUSER_ID = "";
                        model.SYS_LAST_UPD_DTM = DateTime.Now;
                        model.SYS_USUS_ID = "";
                        LTIDList.Add(model);
                        count++;
                    }

                    facet481Model.LTID481 = LTIDList;
                }
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        /// <summary>
        /// Get LTIP Details
        /// </summary>
        /// <param name="facet481Model"></param>
        /// <returns></returns>
        private Facet481Model GetLTIPDetails(Facet481Model facet481Model)
        {
            try
            {
                if (getMasterListResponse.LimitsData.LTIPListData != null)
                {
                    List<LTIP481Model> LTIPList = new List<LTIP481Model>();
                    int count = 1;
                    foreach (var item in getMasterListResponse.LimitsData.LTIPListData.ToList())
                    {
                        LTIP481Model model = new LTIP481Model();
                        model.ACAC_ACC_NO = Convert.ToInt32(item.LTIPAccumulator);
                        model.ATXR_SOURCE_ID = DateTime.Now;
                        model.BatchID = BatchId;
                        model.BenefitOfferingID = 1;
                        model.LTLT_PFX = count.ToString();
                        model.LTIP_IPCD_ID_HIGH = item.ProcedureTo;
                        model.LTIP_IPCD_ID_HIGH = model.LTIP_IPCD_ID_HIGH != "" ? model.LTIP_IPCD_ID_HIGH.Length > 7 ? model.LTIP_IPCD_ID_HIGH.Substring(0, 7) : model.LTIP_IPCD_ID_HIGH : "";
                        model.LTIP_IPCD_ID_LOW = item.ProcedureFrom;
                        model.LTIP_IPCD_ID_LOW = model.LTIP_IPCD_ID_LOW != "" ? model.LTIP_IPCD_ID_LOW.Length > 7 ? model.LTIP_IPCD_ID_LOW.Substring(0, 7) : model.LTIP_IPCD_ID_LOW : "";
                        model.LTIP_LOCK_TOKEN = TOKEN;
                        //item.DiagnosisID -- need to ask 
                        //item.InstanceIDSpecified -- need to ask
                        LTIPList.Add(model);
                        count++;
                    }

                    facet481Model.LTIP481 = LTIPList;
                }
            }
            catch (Exception ex)
            {
                facet481Model.HasError = true;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private void InsertProcessTranmitterQueue(TranslatorProductRowModel processQueue)
        {
            try
            {
                Data.PluginTransmissionProcessQueue pTransProcessQu = new Data.PluginTransmissionProcessQueue();
                pTransProcessQu.BatchId = processQueue.Batch;
                pTransProcessQu.CreatedBy = "TranslatorProcess";
                pTransProcessQu.HasError = false;
                pTransProcessQu.IsActive = true;
                pTransProcessQu.PluginVersionProcessorId = processQueue.PluginVersionProcessorId;
                pTransProcessQu.ProductId = Convert.ToInt32(processQueue.Product);
                pTransProcessQu.UpdatedBy = null;
                pTransProcessQu.UpdatedDate = null;
                pTransProcessQu.CreatedDate = DateTime.Now;
                pTransProcessQu.StartTime = null;
                pTransProcessQu.EndTime = null;
                pTransProcessQu.PluginVersionStatusId = 1;
                pTransProcessQu.FolderVersionNumber = processQueue.FolderVersionNumber;
                pTransProcessQu.FormInstanceName = processQueue.FormInstanceName;
                pTransProcessQu.FolderName = processQueue.FolderName;

                this._unitOfWork.Repository<Data.PluginTransmissionProcessQueue>().Insert(pTransProcessQu);
                this._unitOfWork.Save();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        public TranslatorRowModel GetAllRecordsFromProcessQueue(int productId)
        {
            TranslatorRowModel translatorRowModel = new TranslatorRowModel();
            List<TranslatorProductRowModel> translatorModelList;
            List<PluginRowModel> pluginList;
            List<VersionRowModel> versionList;

            try
            {

                translatorModelList = (from c in this._unitOfWork.Repository<Data.PluginVersionProcessQueueArc>()
                                                                            .Query()
                                                                            .Get()
                                       where c.ProductId == productId
                                       select new TranslatorProductRowModel
                                       {
                                           Id = c.ProcessQueueId,
                                           Batch = c.BatchId,
                                           Status = c.PluginVersionProcessorStatus.Status,
                                           PluginId = c.PluginVersionProcessor.PluginVersion.PluginId.Value,
                                           Plugin = c.PluginVersionProcessor.PluginVersion.Plugin.Name,
                                           ProductId = c.ProductId.Value.ToString(),
                                           Product = c.Product,
                                           Version = c.PluginVersionProcessor.PluginVersion.Description,
                                           VersionId = c.PluginVersionProcessor.PluginVersion.PluginVersionId.ToString(),
                                           PluginVersionProcessorId = c.PluginVersionProcessorId.Value,
                                           FolderVersionNumber = c.FolderVersionNumber,
                                           FormInstanceName = c.FormInstanceName,
                                           FolderName = c.FolderName
                                       }).ToList();

                translatorRowModel.translatorProducts = translatorModelList;

                pluginList = (from plgn in this._unitOfWork.Repository<Data.Plugin>()
                                                                        .Query()
                                                                        .Get()
                              select new PluginRowModel
                              {
                                  Id = plgn.PluginId,
                                  Name = plgn.Name
                              }).ToList();

                translatorRowModel.plugins = pluginList;

                versionList = (from ver in this._unitOfWork.Repository<Data.PluginVersion>()
                                                                        .Query()
                                                                        .Get()
                               select new VersionRowModel
                               {
                                   Id = ver.PluginVersionId,
                                   PluginId = ver.PluginId.Value,
                                   Name = ver.Description
                               }).ToList();

                translatorRowModel.versions = versionList;

                if (translatorModelList.Count() == 0)
                    translatorModelList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return translatorRowModel;
        }

        public Facet481Model GetFacetTableEntries(string tableName, int productId, string batchId, string product)
        {
            Facet481Model facet481Model = new Facet481Model();

            if (tableName == "ACCM")
            {
                facet481Model.ACCM481Model = new ACCM481Model();
                facet481Model = GetACCMHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "DEDE")
            {
                facet481Model.DEDE481Model = new DEDE481Model();
                facet481Model = GetDEDEHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "LTID")
            {
                facet481Model.LTID481Model = new LTID481Model();
                facet481Model = GetLTIDHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "LTIP")
            {
                facet481Model.LTIP481Model = new LTIP481Model();
                facet481Model = GetLTIPHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "LTLT")
            {
                facet481Model.LTLT481Model = new LTLT481Model();
                facet481Model = GetLTLTHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "LTPR")
            {
                facet481Model.LTPR481Model = new LTPR481Model();
                facet481Model = GetLTPRHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "LTSE")
            {
                facet481Model.LTSE481Model = new LTSE481Model();
                facet481Model = GetLTSEHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "PDBC")
            {
                facet481Model.PDBC481Model = new PDBC481Model();
                facet481Model = GetPDBCHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "PDDS")
            {
                facet481Model.PDDS481Model = new PDDS481Model();
                facet481Model = GetPDDSHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "PDPD")
            {
                facet481Model.PDPD481Model = new PDPD481Model();
                facet481Model = GetPDPDHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "PDVC")
            {
                facet481Model.PDVC481Model = new PDVC481Model();
                facet481Model = GetPDVCHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "SEPY")
            {
                facet481Model.SEPY481Model = new SEPY481Model();
                facet481Model = GetSEPYHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "SERL")
            {
                facet481Model.SERL481Model = new SERL481Model();
                facet481Model = GetSERLHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "SESE")
            {
                facet481Model.SESE481Model = new SESE481Model();
                facet481Model = GetSESEHistoryData(facet481Model, productId, batchId);
            }
            else if (tableName == "SETR")
            {
                facet481Model.SETR481Model = new SETR481Model();
                facet481Model = GetSETRHistoryData(facet481Model, productId, batchId);
            }
            return facet481Model;
        }

        public dynamic ReturnList(string tableName, Facet481Model facet481Model)
        {
            if (tableName == "ACCM")
            {
                return facet481Model.ACCM481;
            }
            else if (tableName == "ACDE")
            {
                return facet481Model.ACDE481;
            }
            else if (tableName == "DEDE")
            {
                return facet481Model.DEDE481;
            }

            else if (tableName == "LTID")
            {
                return facet481Model.LTID481;
            }
            else if (tableName == "LTIP")
            {
                return facet481Model.LTIP481;
            }
            else if (tableName == "LTLT")
            {
                return facet481Model.LTLT481;
            }
            else if (tableName == "LTPR")
            {
                return facet481Model.LTPR481;
            }
            else if (tableName == "LTSE")
            {
                return facet481Model.LTSE481;
            }
            else if (tableName == "PDBC")
            {
                return facet481Model.PDBC481;
            }
            else if (tableName == "PDDS")
            {
                return facet481Model.PDDS481;
            }
            else if (tableName == "PDPD")
            {
                return facet481Model.PDPD481;
            }
            else if (tableName == "PDVC")
            {
                return facet481Model.PDVC481;
            }
            else if (tableName == "SEPY")
            {
                return facet481Model.SEPY481;
            }
            else if (tableName == "SERL")
            {
                return facet481Model.SERL481;
            }
            else if (tableName == "SESE")
            {
                return facet481Model.SESE481;
            }
            else if (tableName == "SETR")
            {
                return facet481Model.SETR481;
            }
            else
            {
                return null;
            }
        }

        private Facet481Model GetACCMHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<ACCM481Model> ACCM481DataEntriesList;
            try
            {
                ACCM481DataEntriesList = (from c in this._unitOfWork.Repository<Data.ACCM481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new ACCM481Model
                                          {
                                              PDPD_ID = c.PDPD_ID,
                                              ACCM_TYPE = c.ACCM_TYPE,
                                              ACCM_EFF_DT = c.ACCM_EFF_DT ?? DateTime.Now,
                                              ACCM_SEQ_NO = c.ACCM_SEQ_NO ?? 0,
                                              ACCM_TERM_DT = c.ACCM_TERM_DT ?? DateTime.Now,
                                              ACAC_ACC_NO = c.ACAC_ACC_NO ?? 0,
                                              ACCM_DESC = c.ACCM_DESC,
                                              ACCM_PFX = c.ACCM_PFX,
                                              ACCM_LOCK_TOKEN = c.ACCM_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.ACCM481 = ACCM481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetDEDEHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<DEDE481Model> DEDE481DataEntriesList;
            try
            {
                DEDE481DataEntriesList = (from c in this._unitOfWork.Repository<Data.DEDE481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new DEDE481Model
                                          {
                                              DEDE_PFX = c.DEDE_PFX,
                                              ACAC_ACC_NO = c.ACAC_ACC_NO ?? 0,
                                              DEDE_DESC = c.DEDE_DESC,
                                              DEDE_RULE = c.DEDE_RULE ?? 0,
                                              DEDE_REL_ACC_ID = c.DEDE_REL_ACC_ID ?? 0,
                                              DEDE_COB_OOP_IND = c.DEDE_COB_OOP_IND,
                                              DEDE_SL_IND = c.DEDE_SL_IND,
                                              DEDE_PERIOD_IND = c.DEDE_PERIOD_IND,
                                              DEDE_AGG_PERSON = c.DEDE_AGG_PERSON ?? 0,
                                              DEDE_AGG_PERSON_CO = c.DEDE_AGG_PERSON_CO ?? 0,
                                              DEDE_FAM_AMT = c.DEDE_FAM_AMT ?? 0,
                                              DEDE_FAM_AMT_CO = c.DEDE_FAM_AMT_CO ?? 0,
                                              DEDE_MEME_AMT = c.DEDE_MEME_AMT ?? 0,
                                              DEDE_MEME_AMT_CO = c.DEDE_MEME_AMT_CO ?? 0,
                                              DEDE_OPTS = c.DEDE_OPTS,
                                              DEDE_CO_BYPASS = c.DEDE_CO_BYPASS,
                                              DEDE_MEM_SAL_IND = c.DEDE_MEM_SAL_IND,
                                              DEDE_FAM_SAL_IND = c.DEDE_FAM_SAL_IND,
                                              DEDE_LOCK_TOKEN = c.DEDE_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.DEDE481 = DEDE481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetLTIDHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<LTID481Model> LTID481DataEntriesList;
            try
            {
                LTID481DataEntriesList = (from c in this._unitOfWork.Repository<Data.LTID481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new LTID481Model
                                          {
                                              LTLT_PFX = c.LTLT_PFX,
                                              ACAC_ACC_NO = c.ACAC_ACC_NO ?? 0,
                                              IDCD_ID_REL = c.IDCD_ID_REL,
                                              LTID_LOCK_TOKEN = c.LTID_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.LTID481 = LTID481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetLTIPHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<LTIP481Model> LTIP481DataEntriesList;
            try
            {
                LTIP481DataEntriesList = (from c in this._unitOfWork.Repository<Data.LTIP481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new LTIP481Model
                                          {
                                              LTLT_PFX = c.LTLT_PFX,
                                              ACAC_ACC_NO = c.ACAC_ACC_NO ?? 0,
                                              LTIP_IPCD_ID_LOW = c.LTIP_IPCD_ID_LOW,
                                              LTIP_IPCD_ID_HIGH = c.LTIP_IPCD_ID_HIGH,
                                              LTIP_LOCK_TOKEN = c.LTIP_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.LTIP481 = LTIP481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetLTLTHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<LTLT481Model> LTLT481DataEntriesList;
            try
            {
                LTLT481DataEntriesList = (from c in this._unitOfWork.Repository<Data.LTLT481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new LTLT481Model
                                          {
                                              LTLT_PFX = c.LTLT_PFX,
                                              ACAC_ACC_NO = c.ACAC_ACC_NO ?? 0,
                                              LTLT_DESC = c.LTLT_DESC,
                                              LTLT_CAT = c.LTLT_CAT,
                                              LTLT_LEVEL = c.LTLT_LEVEL,
                                              LTLT_PERIOD_IND = c.LTLT_PERIOD_IND,
                                              LTLT_RULE = c.LTLT_RULE,
                                              LTLT_IX_IND = c.LTLT_IX_IND,
                                              LTLT_IX_TYPE = c.LTLT_IX_TYPE,
                                              EXCD_ID = c.EXCD_ID,
                                              LTLT_AMT1 = c.LTLT_AMT1 ?? 0,
                                              LTLT_AMT2 = c.LTLT_AMT2 ?? 0,
                                              LTLT_OPTS = c.LTLT_OPTS,
                                              LTLT_SAL_IND = c.LTLT_SAL_IND,
                                              LTLT_DAYS = c.LTLT_DAYS ?? 0,
                                              WMDS_SEQ_NO = c.WMDS_SEQ_NO ?? 0,
                                              LTLT_LOCK_TOKEN = c.LTLT_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.LTLT481 = LTLT481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetLTPRHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<LTPR481Model> LTPR481DataEntriesList;
            try
            {
                LTPR481DataEntriesList = (from c in this._unitOfWork.Repository<Data.LTPR481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new LTPR481Model
                                          {
                                              LTLT_PFX = c.LTLT_PFX,
                                              ACAC_ACC_NO = c.ACAC_ACC_NO ?? 0,
                                              PRPR_MCTR_TYPE = c.PRPR_MCTR_TYPE,
                                              LTPR_LOCK_TOKEN = c.LTPR_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.LTPR481 = LTPR481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetLTSEHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<LTSE481Model> LTSE481DataEntriesList;
            try
            {
                LTSE481DataEntriesList = (from c in this._unitOfWork.Repository<Data.LTSE481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new LTSE481Model
                                          {
                                              LTLT_PFX = c.LTLT_PFX,
                                              ACAC_ACC_NO = c.ACAC_ACC_NO ?? 0,
                                              SESE_ID = c.SESE_ID,
                                              LTSE_WT_CTR = c.LTSE_WT_CTR ?? 0,
                                              LTSE_LOCK_TOKEN = c.LTSE_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.LTSE481 = LTSE481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetPDBCHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<PDBC481Model> PDBC481DataEntriesList;
            try
            {
                PDBC481DataEntriesList = (from c in this._unitOfWork.Repository<Data.PDBC481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new PDBC481Model
                                          {
                                              PDBC_PFX = c.PDBC_PFX,
                                              PDPD_ID = c.PDPD_ID,
                                              PDBC_TYPE = c.PDBC_TYPE,
                                              PDBC_EFF_DT = c.PDBC_TERM_DT ?? DateTime.Now,
                                              PDBC_TERM_DT = c.PDBC_TERM_DT ?? DateTime.Now,
                                              PDBC_OPTS = c.PDBC_OPTS,
                                              PDBC_LOCK_TOKEN = c.PDBC_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.PDBC481 = PDBC481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetPDDSHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<PDDS481Model> PDDS481DataEntriesList;
            try
            {
                PDDS481DataEntriesList = (from c in this._unitOfWork.Repository<Data.PDDS481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new PDDS481Model
                                          {
                                              PDPD_ID = c.PDPD_ID,
                                              PDDS_DESC = c.PDDS_DESC,
                                              PDDS_UM_IND = c.PDDS_UM_IND,
                                              PDDS_MED_PRICE_IND = c.PDDS_MED_PRICE_IND,
                                              PDDS_MED_CLMS_IND = c.PDDS_MED_CLMS_IND,
                                              PDDS_DEN_UM_IND = c.PDDS_DEN_UM_IND,
                                              PDDS_DEN_PD_IND = c.PDDS_DEN_PD_IND,
                                              PDDS_DEN_PRICE_IND = c.PDDS_DEN_PRICE_IND,
                                              PDDS_DEN_CLMS_IND = c.PDDS_DEN_CLMS_IND,
                                              PDDS_PREM_IND = c.PDDS_PREM_IND,
                                              PDDS_CLED_IND = c.PDDS_CLED_IND,
                                              PDDS_CAP_IND = c.PDDS_CAP_IND,
                                              PDDS_INT_STATE_IND = c.PDDS_INT_STATE_IND,
                                              PDDS_MCTR_BCAT = c.PDDS_MCTR_BCAT,
                                              PDDS_MCTR_VAL1 = c.PDDS_MCTR_VAL1,
                                              PDDS_MCTR_VAL2 = c.PDDS_MCTR_VAL2,
                                              PDDS_APP_TYPE = c.PDDS_APP_TYPE,
                                              PDDS_PROD_TYPE = c.PDDS_PROD_TYPE,
                                              PDDS_DOFR_IND = c.PDDS_DOFR_IND,
                                              PDDS_OPTOUT_IND = c.PDDS_OPTOUT_IND,
                                              PDDS_LOCK_TOKEN = c.PDDS_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_DBUSER_ID,
                                              SYS_DBUSER_ID = c.SYS_USUS_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.PDDS481 = PDDS481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetPDPDHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<PDPD481Model> PDPD481DataEntriesList;
            try
            {
                PDPD481DataEntriesList = (from c in this._unitOfWork.Repository<Data.PDPD481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new PDPD481Model
                                          {
                                              PDPD_ID = c.PDPD_ID,
                                              PDPD_EFF_DT = c.PDPD_EFF_DT ?? DateTime.Now,
                                              PDPD_TERM_DT = c.PDPD_TERM_DT ?? DateTime.Now,
                                              PDPD_RISK_IND = c.PDPD_RISK_IND,
                                              LOBD_ID = c.LOBD_ID,
                                              LOBD_ALT_RISK_ID = c.LOBD_ALT_RISK_ID,
                                              PDPD_ACC_SFX = c.PDPD_ACC_SFX,
                                              PDPD_OPTS = c.PDPD_OPTS,
                                              PDPD_CAP_POP_LVL = c.PDPD_CAP_POP_LVL,
                                              PDPD_CAP_RET_MOS = c.PDPD_CAP_RET_MOS ?? 0,
                                              PDPD_MCTR_CCAT = c.PDPD_MCTR_CCAT,
                                              PDPD_LOCK_TOKEN = c.PDPD_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.PDPD481 = PDPD481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetPDVCHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<PDVC481Model> PDVC481DataEntriesList;
            try
            {
                PDVC481DataEntriesList = (from c in this._unitOfWork.Repository<Data.PDVC481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new PDVC481Model
                                          {
                                              PDPD_ID = c.PDPD_ID,
                                              PDVC_TIER = c.PDVC_TIER ?? 0,
                                              PDVC_TYPE = c.PDVC_TYPE,
                                              PDVC_EFF_DT = c.PDVC_EFF_DT ?? DateTime.Now,
                                              PDVC_SEQ_NO = c.PDVC_SEQ_NO ?? 0,
                                              PDVC_TERM_DT = c.PDVC_TERM_DT ?? DateTime.Now,
                                              PDVC_PR_PCP = c.PDVC_PR_PCP,
                                              PDVC_PR_IN = c.PDVC_PR_IN,
                                              PDVC_PR_PAR = c.PDVC_PR_PAR,
                                              PDVC_PR_NONPAR = c.PDVC_PR_NONPAR,
                                              PDVC_PC_NR = c.PDVC_PC_NR,
                                              PDVC_PC_OBT = c.PDVC_PC_OBT,
                                              PDVC_PC_VIOL = c.PDVC_PC_VIOL,
                                              PDVC_REF_NR = c.PDVC_REF_NR,
                                              PDVC_REF_OBT = c.PDVC_REF_OBT,
                                              PDVC_REF_VIOL = c.PDVC_REF_VIOL,
                                              PDVC_LOBD_PTR = c.PDVC_LOBD_PTR,
                                              SEPY_PFX = c.SEPY_PFX,
                                              DEDE_PFX = c.DEDE_PFX,
                                              LTLT_PFX = c.LTLT_PFX,
                                              DPPY_PFX = c.DPPY_PFX,
                                              CGPY_PFX = c.CGPY_PFX,
                                              PDVC_LOCK_TOKEN = c.PDVC_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.PDVC481 = PDVC481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetSEPYHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<SEPY481Model> SEPY481DataEntriesList;
            try
            {
                SEPY481DataEntriesList = (from c in this._unitOfWork.Repository<Data.SEPY481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new SEPY481Model
                                          {
                                              SEPY_PFX = c.SEPY_PFX,
                                              SEPY_EFF_DT = c.SEPY_EFF_DT ?? DateTime.Now,
                                              SESE_ID = c.SESE_ID,
                                              SEPY_TERM_DT = c.SEPY_TERM_DT ?? DateTime.Now,
                                              SESE_RULE = c.SESE_RULE,
                                              SEPY_EXP_CAT = c.SEPY_EXP_CAT,
                                              SEPY_ACCT_CAT = c.SEPY_ACCT_CAT,
                                              SEPY_OPTS = c.SEPY_OPTS,
                                              SESE_RULE_ALT = c.SESE_RULE_ALT,
                                              SESE_RULE_ALT_COND = c.SESE_RULE_ALT_COND,
                                              SEPY_LOCK_TOKEN = c.SEPY_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.SEPY481 = SEPY481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetSERLHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<SERL481Model> SERL481DataEntriesList;
            try
            {
                SERL481DataEntriesList = (from c in this._unitOfWork.Repository<Data.SERL481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new SERL481Model
                                          {
                                              SERL_REL_ID = c.SERL_REL_ID,
                                              SERL_REL_TYPE = c.SERL_REL_TYPE,
                                              SERL_REL_PER_IND = c.SERL_REL_PER_IND,
                                              SERL_DIAG_IND = c.SERL_DIAG_IND,
                                              SERL_NTWK_IND = c.SERL_NTWK_IND,
                                              SERL_PC_IND = c.SERL_PC_IND,
                                              SERL_REF_IND = c.SERL_REF_IND,
                                              SERL_PER = c.SERL_PER ?? 0,
                                              SERL_OPTS = c.SERL_OPTS,
                                              SERL_COPAY_IND = c.SERL_COPAY_IND,
                                              SERL_DESC = c.SERL_DESC,
                                              SERL_LOCK_TOKEN = c.SERL_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.SERL481 = SERL481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetSESEHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<SESE481Model> SESE481DataEntriesList;
            try
            {
                SESE481DataEntriesList = (from c in this._unitOfWork.Repository<Data.SESE481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new SESE481Model
                                          {
                                              SESE_ID = c.SESE_ID,
                                              SESE_RULE = c.SESE_RULE,
                                              SESE_DESC = c.SESE_DESC,
                                              SESE_CM_IND = c.SESE_CM_IND,
                                              SESE_PA_AMT_REQ = c.SESE_PA_AMT_REQ,
                                              SESE_PA_UNIT_REQ = c.SESE_PA_UNIT_REQ,
                                              SESE_PA_PROC_REQ = c.SESE_PA_PROC_REQ,
                                              SESE_VALID_SEX = c.SESE_VALID_SEX,
                                              SESE_SEX_EXCD_ID = c.SESE_SEX_EXCD_ID,
                                              SESE_MIN_AGE = c.SESE_MIN_AGE ?? 0,
                                              SESE_MAX_AGE = c.SESE_MAX_AGE ?? 0,
                                              SESE_AGE_EXCD_ID = c.SESE_AGE_EXCD_ID,
                                              SESE_COV_TYPE = c.SESE_COV_TYPE,
                                              SESE_COV_EXCD_ID = c.SESE_COV_EXCD_ID,
                                              SESE_RULE_TYPE = c.SESE_RULE_TYPE,
                                              SESE_CALC_IND = c.SESE_ID,
                                              SERL_REL_ID = c.SERL_REL_ID,
                                              SESE_OPTS = c.SESE_OPTS,
                                              WMDS_SEQ_NO = c.WMDS_SEQ_NO ?? 0,
                                              SESE_ID_XLOW = c.SESE_ID_XLOW,
                                              SESE_DESC_XLOW = c.SESE_DESC_XLOW,
                                              SESE_DIS_EXCD_ID = c.SESE_DIS_EXCD_ID,
                                              SESE_MAX_CPAY_PCT = c.SESE_MAX_CPAY_PCT ?? 0,
                                              SESE_FSA_REIMB_IND = c.SESE_FSA_REIMB_IND,
                                              SESE_HSA_REIMB_IND = c.SESE_HSA_REIMB_IND,
                                              SESE_HRA_DED_IND = c.SESE_HRA_DED_IND,
                                              SESE_LOCK_TOKEN = c.SESE_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.SESE481 = SESE481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }

        private Facet481Model GetSETRHistoryData(Facet481Model facet481Model, int productId, string batchId)
        {
            List<SETR481Model> SETR481DataEntriesList;
            try
            {
                SETR481DataEntriesList = (from c in this._unitOfWork.Repository<Data.SETR481Arc>()
                                                            .Query()
                                                            .Get()
                                          where c.BenefitOfferingID == productId && c.BatchID == batchId
                                          select new SETR481Model
                                          {
                                              SESE_ID = c.SESE_ID,
                                              SESE_RULE = c.SESE_RULE,
                                              SETR_TIER_NO = c.SETR_TIER_NO ?? 0,
                                              SETR_ALLOW_AMT = c.SETR_ALLOW_AMT ?? 0,
                                              SETR_ALLOW_CTR = c.SETR_ALLOW_CTR ?? 0,
                                              SETR_COPAY_AMT = c.SETR_COPAY_AMT ?? 0,
                                              SETR_COIN_PCT = c.SETR_COIN_PCT ?? 0,
                                              ACAC_ACC_NO = c.ACAC_ACC_NO ?? 0,
                                              SETR_OPTS = c.SETR_OPTS,
                                              SETR_LOCK_TOKEN = c.SETR_LOCK_TOKEN ?? 0,
                                              ATXR_SOURCE_ID = c.ATXR_SOURCE_ID ?? DateTime.Now,
                                              SYS_LAST_UPD_DTM = c.SYS_LAST_UPD_DTM,
                                              SYS_USUS_ID = c.SYS_USUS_ID,
                                              SYS_DBUSER_ID = c.SYS_DBUSER_ID,
                                              BatchID = c.BatchID,
                                              BenefitOfferingID = c.BenefitOfferingID
                                          }).ToList();

                facet481Model.SETR481 = SETR481DataEntriesList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return facet481Model;
        }
    }
}
