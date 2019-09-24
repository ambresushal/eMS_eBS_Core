using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.domain.Models;
using tmg.equinox.integration.translator.dao.Models;
using tmg.equinox.integration.domain;
using tmg.equinox.integration.domain.services.Enums;
using tmg.equinox.integration.infrastructure.exceptionhandling;
using tmg.equinox.integration.infrastructure.Util;

namespace tmg.equinox.integration.domain.services.Impl
{
    public class TransmitterService : ITransmitterService
    {
        private IUnitOfWork _unitOfWork { get; set; }
        
        public TransmitterService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public TranslatorRowModel GetTransmissionQueueProducts()
        {
            TranslatorRowModel translatorRowModel = new TranslatorRowModel();
            List<TranslatorProductRowModel> translatorModelList;
            //List<PluginRowModel> pluginList;
            //List<VersionRowModel> versionList;
            try
            {               
                translatorModelList = (from c in this._unitOfWork.Repository<PluginTransmissionProcessQueue>()
                                                                        .Query()
                                                                        .Get()
                                       select new TranslatorProductRowModel
                                       {
                                           Id = c.ProcessQueueId,
                                           Batch = c.BatchId,
                                           Status = c.PluginVersionProcessorStatus.Status,
                                           PluginId = c.PluginVersionProcessorId.Value,
                                           Plugin = c.PluginVersionProcessor.PluginVersion.Plugin.Name,
                                           Product = c.ProductId.Value.ToString(),
                                           Version = c.PluginVersionProcessor.PluginVersion.Description,
                                           VersionId = c.PluginVersionProcessor.PluginVersionId.Value.ToString(),
                                           PluginVersionProcessorId = c.PluginVersionProcessorId.Value,
                                           FolderVersionNumber = c.FolderVersionNumber,
                                           FormInstanceName = c.FormInstanceName,
                                           FolderName = c.FolderName,
                                           TrasmittedFilePath = c.TrasmittedFilePath
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

        public IList<Models.TranslatorProductRowModel> GetPluginList()
        {
            return null;
        }

        public IList<Models.TranslatorProductRowModel> GetVersionList()
        {
            return null;
        }

        public IList<Models.TranslatorProductRowModel> GetProductList()
        {
            return null;
        }

        private Facet481Model GetStagingEntities(string batchId)
        {
            Facet481Model facetModel481 = new Facet481Model();
            try
            {
                facetModel481.PDPD481 = this._unitOfWork.Repository<PDPD481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<PDPD481Model, PDPD481Stg>();
                facetModel481.PDDS481 = this._unitOfWork.Repository<PDDS481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<PDDS481Model, PDDS481Stg>();
                facetModel481.ACCM481 = this._unitOfWork.Repository<ACCM481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<ACCM481Model, ACCM481Stg>();
                facetModel481.ACDE481 = this._unitOfWork.Repository<ACDE481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<ACDE481Model, ACDE481Stg>();
                facetModel481.DEDE481 = this._unitOfWork.Repository<DEDE481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<DEDE481Model, DEDE481Stg>();
                facetModel481.IPMC481 = this._unitOfWork.Repository<IPMC481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<IPMC481Model, IPMC481Stg>();
                facetModel481.LTID481 = this._unitOfWork.Repository<LTID481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<LTID481Model, LTID481Stg>();
                facetModel481.LTIP481 = this._unitOfWork.Repository<LTIP481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<LTIP481Model, LTIP481Stg>();
                facetModel481.LTLT481 = this._unitOfWork.Repository<LTLT481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<LTLT481Model, LTLT481Stg>();
                facetModel481.LTPR481 = this._unitOfWork.Repository<LTPR481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<LTPR481Model, LTPR481Stg>();
                facetModel481.LTSE481 = this._unitOfWork.Repository<LTSE481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<LTSE481Model, LTSE481Stg>();
                facetModel481.PDBC481 = this._unitOfWork.Repository<PDBC481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<PDBC481Model, PDBC481Stg>();
                facetModel481.PDVC481 = this._unitOfWork.Repository<PDVC481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<PDVC481Model, PDVC481Stg>();
                facetModel481.SEDF481 = this._unitOfWork.Repository<SEDF481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<SEDF481Model, SEDF481Stg>();
                facetModel481.SEPY481 = this._unitOfWork.Repository<SEPY481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<SEPY481Model, SEPY481Stg>();
                facetModel481.SERL481 = this._unitOfWork.Repository<SERL481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<SERL481Model, SERL481Stg>();
                facetModel481.SESE481 = this._unitOfWork.Repository<SESE481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<SESE481Model, SESE481Stg>();
                facetModel481.SESP481 = this._unitOfWork.Repository<SESP481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<SESP481Model, SESP481Stg>();
                facetModel481.SESR481 = this._unitOfWork.Repository<SESR481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<SESR481Model, SESR481Stg>();
                facetModel481.SETR481 = this._unitOfWork.Repository<SETR481Stg>().Get().Where(c => c.BatchID == batchId).ToList().MapToListEntity<SETR481Model, SETR481Stg>();

                return facetModel481;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;    
            }

            return null;
        }

        private string GetFilePath(string batchId, string pluginVersion, string plugin)
        {
            string dirPath = Util.GetKeyValue("TransmitterOutputPath");
            string filePath = dirPath + plugin + "_" + pluginVersion + "\\" + batchId + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return filePath;
        }

        private List<TranslatorProductRowModel> GetQueuedEntries(string plugin, string pluginVersion)
        {
            try
            {
                var statusQueued = (int)Status.Queued;
                List<TranslatorProductRowModel> pluginProcessor = (from q in this._unitOfWork.Repository<PluginTransmissionProcessQueue>()
                                            .Get()
                                            .Where(c => c.PluginVersionStatusId == statusQueued
                                                     && c.PluginVersionProcessor.PluginVersion.Plugin.Name == plugin
                                                     && c.PluginVersionProcessor.PluginVersion.Description == pluginVersion)
                                                                   select new TranslatorProductRowModel
                                                                   {
                                                                       Id = q.ProcessQueueId,
                                                                       Plugin = q.PluginVersionProcessor.PluginVersion.Plugin.Name,
                                                                       Batch = q.BatchId,
                                                                       PluginId = (int)q.ProductId,
                                                                       PluginVersionProcessorId = (int)q.PluginVersionProcessorId,
                                                                       Version = q.PluginVersionProcessor.PluginVersion.Description,
                                                                       VersionId = q.PluginVersionProcessor.PluginVersionId.ToString(),
                                                                       Product = q.ProductId.ToString(),
                                                                       Status = q.PluginVersionProcessorStatus.Status,
                                                                       OutPutFormat = q.PluginVersionProcessor.OutPutFormat
                                                                   }).ToList();

                return pluginProcessor;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;    
            }
            return null;
        }

        public string AddProductstoTransmissionQueue(IList<TranslatorProductRowModel> productstoTransmit)
        {
            string msg = string.Empty;
            try
            {
                if (productstoTransmit != null && productstoTransmit.Count > 0)
                {
                    foreach (TranslatorProductRowModel producttoTransmit in productstoTransmit)
                    {
                        if (producttoTransmit.IsIncluded)
                        {
                            PluginTransmissionProcessQueue newProducttoTransmit = new PluginTransmissionProcessQueue();
                            newProducttoTransmit.BatchId = producttoTransmit.Batch;
                            newProducttoTransmit.ProductId = Convert.ToInt32(producttoTransmit.Product);
                            newProducttoTransmit.PluginVersionProcessorId = producttoTransmit.PluginVersionProcessorId;
                            newProducttoTransmit.PluginVersionStatusId = 1;
                            newProducttoTransmit.CreatedBy = "WebAdmin";
                            newProducttoTransmit.CreatedDate = DateTime.Now;
                            newProducttoTransmit.FolderVersionNumber = producttoTransmit.FolderVersionNumber;
                            newProducttoTransmit.FormInstanceName = producttoTransmit.FormInstanceName;
                            newProducttoTransmit.FolderName = producttoTransmit.FolderName;

                            this._unitOfWork.Repository<PluginTransmissionProcessQueue>().Insert(newProducttoTransmit);
                            this._unitOfWork.Save();
                        }
                    }
                    msg = "Product successfully queued for Transmission";
                }
            }
            catch (Exception ex)
            {
                msg = "Error Queuing product for transmission";
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;    
            }
            return msg;
        }

        private void UpdateProduct(List<TranslatorProductRowModel> producttoTransmitList, DateTime startTime, DateTime endTime, int status, string CSVFilePath)
        {
            string msg = string.Empty;
            try
            {
                if (producttoTransmitList != null)
                {
                    foreach (TranslatorProductRowModel producttoTransmit in producttoTransmitList)
                    {
                        PluginTransmissionProcessQueue pluginTransProcessQueue = this._unitOfWork.Repository<PluginTransmissionProcessQueue>()
                                                                   .FindById(producttoTransmit.Id);

                        pluginTransProcessQueue.StartTime = startTime;
                        pluginTransProcessQueue.EndTime = endTime;
                        pluginTransProcessQueue.PluginVersionStatusId = status;
                        pluginTransProcessQueue.TrasmittedFilePath = CSVFilePath;

                        this._unitOfWork.Repository<PluginTransmissionProcessQueue>().Update(pluginTransProcessQueue);
                        
                    }
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

        bool ITransmitterService.DeleteProcessQueue(int Id)
        {
            try
            {
                PluginTransmissionProcessQueue processQueue = this._unitOfWork.Repository<PluginTransmissionProcessQueue>().FindById(Id);

                this._unitOfWork.Repository<PluginTransmissionProcessQueue>().Delete(processQueue);
                this._unitOfWork.Save();
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

        private string CreateCSVFiles(string batchId, string pluginVersion, string plugin, Facet481Model facet481Model)
        {
            string filePath = "";
            try
            {
                filePath = GetFilePath(batchId, pluginVersion, plugin); //"_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + 
                FileWritter writter = null;

                FileConvertor<PDPD481Model> pdpd481 = new FileConvertor<PDPD481Model>();
                string pdpdContent = pdpd481.CSV(facet481Model.PDPD481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.PDPD + "." + FileExtension.csv, pdpdContent);

                //FileConvertor<ACCM481Model> accm481 = new FileConvertor<ACCM481Model>();
                //string accmContent = accm481.CSV(facet481Model.ACCM481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.ACCM + "." + FileExtension.csv, accmContent);

                //FileConvertor<ACDE481Model> acde481 = new FileConvertor<ACDE481Model>();
                //string acdeContent = acde481.CSV(facet481Model.ACDE481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.ACDE + "." + FileExtension.csv, acdeContent);

                FileConvertor<DEDE481Model> dede481 = new FileConvertor<DEDE481Model>();
                string dedeContent = dede481.CSV(facet481Model.DEDE481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.DEDE + "." + FileExtension.csv, dedeContent);

                //FileConvertor<IPMC481Model> ipmc481 = new FileConvertor<IPMC481Model>();
                //string ipmcContent = ipmc481.CSV(facet481Model.IPMC481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.IPMC + "." + FileExtension.csv, ipmcContent);

                FileConvertor<LTID481Model> ltid481 = new FileConvertor<LTID481Model>();
                string ltidContent = ltid481.CSV(facet481Model.LTID481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTID + "." + FileExtension.csv, ltidContent);

                FileConvertor<LTIP481Model> ltip481 = new FileConvertor<LTIP481Model>();
                string ltipContent = ltip481.CSV(facet481Model.LTIP481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTIP + "." + FileExtension.csv, ltipContent);

                FileConvertor<LTLT481Model> ltlt481 = new FileConvertor<LTLT481Model>();
                string ltltContent = ltlt481.CSV(facet481Model.LTLT481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTLT + "." + FileExtension.csv, ltltContent);

                FileConvertor<LTPR481Model> ltpr481 = new FileConvertor<LTPR481Model>();
                string ltprContent = ltpr481.CSV(facet481Model.LTPR481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTPR + "." + FileExtension.csv, ltprContent);

                FileConvertor<PDBC481Model> pdbc481 = new FileConvertor<PDBC481Model>();
                string pdbcContent = pdbc481.CSV(facet481Model.PDBC481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.PDBC + "." + FileExtension.csv, pdbcContent);

                FileConvertor<LTSE481Model> ltse481 = new FileConvertor<LTSE481Model>();
                string ltseContent = ltse481.CSV(facet481Model.LTSE481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTSE + "." + FileExtension.csv, ltseContent);

                FileConvertor<PDDS481Model> pdds481 = new FileConvertor<PDDS481Model>();
                string pddsContent = pdds481.CSV(facet481Model.PDDS481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.PDDS + "." + FileExtension.csv, pddsContent);

                FileConvertor<PDVC481Model> pdvc481 = new FileConvertor<PDVC481Model>();
                string pdvcContent = pdvc481.CSV(facet481Model.PDVC481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.PDVC + "." + FileExtension.csv, pdvcContent);

                //FileConvertor<SEDF481Model> sedf481 = new FileConvertor<SEDF481Model>();
                //string sedfContent = sedf481.CSV(facet481Model.SEDF481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.SEDF + "." + FileExtension.csv, sedfContent);

                FileConvertor<SEPY481Model> sepy481 = new FileConvertor<SEPY481Model>();
                string sepyContent = sepy481.CSV(facet481Model.SEPY481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.SEPY + "." + FileExtension.csv, sepyContent);

                FileConvertor<SERL481Model> serl481 = new FileConvertor<SERL481Model>();
                string serlContent = serl481.CSV(facet481Model.SERL481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.SERL + "." + FileExtension.csv, serlContent);

                FileConvertor<SESE481Model> sese481 = new FileConvertor<SESE481Model>();
                string seseContent = sese481.CSV(facet481Model.SESE481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.SESE + "." + FileExtension.csv, seseContent);

                //FileConvertor<SESP481Model> sesp481 = new FileConvertor<SESP481Model>();
                //string sespContent = sesp481.CSV(facet481Model.SESP481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.SESP + "." + FileExtension.csv, sespContent);

                //FileConvertor<SESR481Model> sesr481 = new FileConvertor<SESR481Model>();
                //string sesrContent = sesr481.CSV(facet481Model.SESR481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.SESR + "." + FileExtension.csv, sesrContent);

                FileConvertor<SETR481Model> setr481 = new FileConvertor<SETR481Model>();
                string setrContent = setr481.CSV(facet481Model.SETR481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.SETR + "." + FileExtension.csv, setrContent);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                throw ex;   
            }
            return filePath;
        }

        private void CreateXMLFiles(string batchId,  string pluginVersion, string plugin, Facet481Model facet481Model)
        {
            try
            {
                string filePath = GetFilePath(batchId, pluginVersion, plugin);
                FileWritter writter = null;

                FileConvertor<PDPD481Model> pdpd481 = new FileConvertor<PDPD481Model>();
                string pdpdContent = pdpd481.XML(facet481Model.PDPD481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.PDPD + "." + FileExtension.xml, pdpdContent);

                //FileConvertor<ACCM481Model> accm481 = new FileConvertor<ACCM481Model>();
                //string accmContent = accm481.XML(facet481Model.ACCM481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.ACCM + "." + FileExtension.xml, accmContent);

                //FileConvertor<ACDE481Model> acde481 = new FileConvertor<ACDE481Model>();
                //string acdeContent = acde481.XML(facet481Model.ACDE481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.ACDE + "." + FileExtension.xml, acdeContent);

                FileConvertor<DEDE481Model> dede481 = new FileConvertor<DEDE481Model>();
                string dedeContent = dede481.XML(facet481Model.DEDE481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.DEDE + "." + FileExtension.xml, dedeContent);

                //FileConvertor<IPMC481Model> ipmc481 = new FileConvertor<IPMC481Model>();
                //string ipmcContent = ipmc481.XML(facet481Model.IPMC481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.IPMC + "." + FileExtension.xml, ipmcContent);

                FileConvertor<LTID481Model> ltid481 = new FileConvertor<LTID481Model>();
                string ltidContent = ltid481.XML(facet481Model.LTID481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTID + "." + FileExtension.xml, ltidContent);

                FileConvertor<LTIP481Model> ltip481 = new FileConvertor<LTIP481Model>();
                string ltipContent = ltip481.XML(facet481Model.LTIP481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTIP + "." + FileExtension.xml, ltipContent);

                FileConvertor<LTLT481Model> ltlt481 = new FileConvertor<LTLT481Model>();
                string ltltContent = ltlt481.CSV(facet481Model.LTLT481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTLT + "." + FileExtension.xml, ltltContent);

                FileConvertor<LTPR481Model> ltpr481 = new FileConvertor<LTPR481Model>();
                string ltprContent = ltpr481.XML(facet481Model.LTPR481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTPR + "." + FileExtension.xml, ltprContent);

                FileConvertor<PDBC481Model> pdbc481 = new FileConvertor<PDBC481Model>();
                string pdbcContent = pdbc481.CSV(facet481Model.PDBC481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.PDBC + "." + FileExtension.xml, pdbcContent);

                FileConvertor<LTSE481Model> ltse481 = new FileConvertor<LTSE481Model>();
                string ltseContent = ltse481.XML(facet481Model.LTSE481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.LTSE + "." + FileExtension.xml, ltseContent);

                FileConvertor<PDDS481Model> pdds481 = new FileConvertor<PDDS481Model>();
                string pddsContent = pdds481.XML(facet481Model.PDDS481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.PDDS + "." + FileExtension.xml, pddsContent);

                FileConvertor<PDVC481Model> pdvc481 = new FileConvertor<PDVC481Model>();
                string pdvcContent = pdvc481.XML(facet481Model.PDVC481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.PDVC + "." + FileExtension.xml, pdvcContent);

                //FileConvertor<SEDF481Model> sedf481 = new FileConvertor<SEDF481Model>();
                //string sedfContent = sedf481.XML(facet481Model.SEDF481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.SEDF + "." + FileExtension.xml, sedfContent);

                FileConvertor<SEPY481Model> sepy481 = new FileConvertor<SEPY481Model>();
                string sepyContent = sepy481.XML(facet481Model.SEPY481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.SEPY + "." + FileExtension.xml, sepyContent);

                FileConvertor<SERL481Model> serl481 = new FileConvertor<SERL481Model>();
                string serlContent = serl481.XML(facet481Model.SERL481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.SERL + "." + FileExtension.xml, serlContent);

                FileConvertor<SESE481Model> sese481 = new FileConvertor<SESE481Model>();
                string seseContent = sese481.XML(facet481Model.SESE481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.SESE + "." + FileExtension.xml, seseContent);

                //FileConvertor<SESP481Model> sesp481 = new FileConvertor<SESP481Model>();
                //string sespContent = sesp481.XML(facet481Model.SESP481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.SESP + "." + FileExtension.xml, sespContent);

                //FileConvertor<SESR481Model> sesr481 = new FileConvertor<SESR481Model>();
                //string sesrContent = sesr481.XML(facet481Model.SESR481);
                //writter = new FileWritter();
                //writter.CreateFile(filePath, Component.SESR + "." + FileExtension.xml, sesrContent);

                FileConvertor<SETR481Model> setr481 = new FileConvertor<SETR481Model>();
                string setrContent = setr481.XML(facet481Model.SETR481);
                writter = new FileWritter();
                writter.CreateFile(filePath, Component.SETR + "." + FileExtension.xml, setrContent);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;   
            }
        }

        public bool Transmitfiles(string plugin, string pluginVersion)
        {
            try
            {               
                List<TranslatorProductRowModel> transProcessQueueList = GetQueuedEntries(plugin, pluginVersion);
                List<string> batchIds = transProcessQueueList.Select(c => c.Batch).Distinct().ToList();
                string CSVFilePath = "";

                foreach (string batchId in batchIds)
                {
                    Facet481Model facet481Model = GetStagingEntities(batchId);
                    List<TranslatorProductRowModel> pluginVerProcessQueueList = transProcessQueueList.Where(c => c.Batch == batchId).ToList();
                    //foreach (TranslatorProductRowModel processQueue in pluginVerProcessQueueList)
                    //{

                    DateTime startTime = DateTime.Now;

                    string outFileFormat = Util.GetKeyValue("OutputFileFormat");

                    // if (pluginVerProcessQueueList.FirstOrDefault().OutPutFormat.Trim() == "CSV")
                    if (outFileFormat != "" && outFileFormat.ToLower() == "csv")
                        CSVFilePath = CreateCSVFiles(batchId, pluginVersion, plugin, facet481Model);

                    //if (pluginVerProcessQueueList.FirstOrDefault().OutPutFormat.Trim() == "XML")
                    if (outFileFormat != "" && outFileFormat.ToLower() == "xml")
                        CreateXMLFiles(batchId, pluginVersion, plugin, facet481Model);


                    DateTime endTime = DateTime.Now;
                    int status = 2;
                    string filePath = CSVFilePath.Split('\\').Last();
                    UpdateProduct(pluginVerProcessQueueList, startTime, endTime, status, filePath);
                    //}
                }

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
    }
}
