using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.services.api.Framework;
using tmg.equinox.services.api.Models;
using tmg.equinox.services.api.Validators;
using tmg.equinox.web.FormInstanceManager;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using System.Collections.Generic;
using tmg.equinox.web.Validator;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using System;
using tmg.equinox.web.extensions;
using tmg.equinox.queueprocess.exitvalidate;
using System.Text;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class DocumentsController : BaseApiController
    {
        private IFormInstanceService _formInstanceService;
        private IFolderVersionServices _folderversionservices;
        private IFormInstanceDataServices _formInstanceDataService;
        private IWorkFlowStateServices _workflowstateservice;
        private IFormDesignService _formDesignServices;
        private IUIElementService _uiElementService;
        private IMasterListService _masterListService;
        private IConsumerAccountService _consumerAccountService;
        private ILoggingService _loggingService;
        private IExitValidateService _evService;
        private IExitValidateEnqueueService _evEnqueueService;

        public DocumentsController(IWorkFlowStateServices workflowversionservices, IFormInstanceService formInstanceService, IFormInstanceDataServices formInstanceDataService, IFolderVersionServices folderversionservices, IFormDesignService formDesignService, IUIElementService uiElementService, IMasterListService masterListService, IConsumerAccountService consumerAccountService, ILoggingService loggingService, IBackgroundJobManager backgroundJobManager, IExitValidateService evService, IExitValidateEnqueueService evEnqueueService)
        {
            this._formInstanceService = formInstanceService;
            this._formInstanceDataService = formInstanceDataService;
            this._folderversionservices = folderversionservices;
            this._workflowstateservice = workflowversionservices;
            this._formDesignServices = formDesignService;
            this._uiElementService = uiElementService;
            this._masterListService = masterListService;
            this._consumerAccountService = consumerAccountService;
            this._loggingService = loggingService;
            this._evService = evService;
            this._evEnqueueService = evEnqueueService;
        }

        /// <summary>
        /// Get a Document by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a document with data.</returns>
        [HttpGet]
        [Route("api/v1/Documents/{id}")]
        [ResponseType(typeof(FormInstanceViewModel))]
        public HttpResponseMessage Get(int id)
        {
            var document = _folderversionservices.GetFormInstance(1, id);
            if (document == null)
            {
                HttpError myCustomError = new HttpError(Constants.DocumentNotExist);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, myCustomError);
            }
            return Request.CreateResponse(HttpStatusCode.OK, document);
        }

        /// <summary>
        /// Get a DocumentJSON by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a document with JSON data.</returns>
        [HttpGet]
        [Route("api/v1/Documents/GetDocumentJSON/{id}")]
        [ResponseType(typeof(FormInstanceViewModel))]
        public HttpResponseMessage GetDocumentJSON(int id)
        {
            var document = _folderversionservices.GetFormInstanceData(1, id);            
            if (document == null)
            {
                HttpError myCustomError = new HttpError(Constants.DocumentNotExist);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, myCustomError);
            }
            return Request.CreateResponse(HttpStatusCode.OK, JObject.Parse(document));            
        }
        
        /// <summary>
        /// Get the list of Documents by a folder version.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns the list of documents without data.</returns>
        [HttpGet]
        [Route("api/v1/Documents/{id}/Documents")]
        [ResponseType(typeof(FormInstanceViewModel))]
        public HttpResponseMessage GetDocumentsByFolderVersion(int id)
        {
            var documents = _folderversionservices.GetFormList(1, id);
            List<FormInstanceModel> documentsList = new List<FormInstanceModel>();
            if (documents == null || documents.Count() > 0)
            {
                documentsList  = (from c in documents
                                     select new FormInstanceModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FormInstanceName = c.FormInstanceName,
                                         FolderVersionID = c.FolderVersionID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = c.FormDesignName,                                         
                                         FormDesignVersionID = c.FormDesignVersionID,
                                         EffectiveDate = c.EffectiveDate
                                     }).ToList();
            } else if (documents == null || documents.Count() == 0)
            {
                HttpError myCustomError = new HttpError(tmg.equinox.services.api.Validators.Constants.DocumentNotActive);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, myCustomError);
            }
            return Request.CreateResponse(HttpStatusCode.OK, documentsList);
        }

        /// <summary>
        /// Update Document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="document"></param>
        /// <param name="showWarning"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/Documents/{id}")]
        public async Task<HttpResponseMessage> Update(int id, [FromBody]JObject document, bool showWarning = true)
        {
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataService, base.CurrentUserName, _folderversionservices, null, _masterListService);
            ServiceResult result = formDataInstanceManager.SaveTargetSectionsData(id, _folderversionservices, _formDesignServices);
            return await Task.Run(() => CreateResponse(result, true));
        }

        /// <summary>
        /// Add a new Document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Documents")]
        public async Task<HttpResponseMessage> Add([FromBody]Documentadd document)
        {
            string resultMessage = string.Empty;
            ServiceResult result = null;
            int folderid = 0;
            int formInstanceId = 0;
            int formDesignVersionId = 0;
            var folderversion = _folderversionservices.GetFolderVersionById(document.FolderversionID);
            folderid = folderversion.FolderId;
            var formdesigns = _formDesignServices.GetFormDesignList(1);
            if (formdesigns != null && formdesigns.Count() > 0)
            {
                var fromDesign = formdesigns.Where(row => row.FormDesignName == document.TemplateName).FirstOrDefault();
                if (fromDesign != null)
                {
                    var fromDesignVersions = _formDesignServices.GetFormDesignVersionList(1, fromDesign.FormDesignId);
                    if (fromDesignVersions != null && fromDesignVersions.Count() > 0)
                    {
                        formDesignVersionId = fromDesignVersions.OrderByDescending(row => row.FormDesignVersionId).FirstOrDefault().FormDesignVersionId;
                    }
                    else
                    {
                        resultMessage = "Design version does not exist.";
                        return Request.CreateResponse(HttpStatusCode.BadRequest, resultMessage);
                    }

                }
                else
                {
                    resultMessage = "Design does not exist.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultMessage);
                }
            }

            result = await Task.Run(() => _folderversionservices.CreateFormInstance(base.TenantId, document.FolderversionID, formDesignVersionId, formInstanceId, document.IsCopy, document.DocumentName, CurrentUserName));
            if (result.Result == ServiceResultStatus.Success)
            {
                return await Task.Run(() => Request.CreateResponse(HttpStatusCode.OK, Validators.Constants.Success));
            }
            else
            {
                return await Task.Run(() => Request.CreateResponse(HttpStatusCode.OK, Validators.Constants.Failure));
            }
        }

        /// <summary>
        /// Delete a Document.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/Documents/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            var document = _folderversionservices.GetFormInstance(1, id);
            if (document == null)
            {
                return CreateResponse(document, Constants.DocumentNotExist, HttpStatusCode.NotFound);
            }
            var result = _folderversionservices.DeleteFormInstance(document.FolderID, 1, document.FolderVersionID, id, CurrentUserName);
            return Request.CreateResponse(HttpStatusCode.OK, Validators.Constants.Success);
        }

        /// <summary>
        /// Validate Document.
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Documents/Validate/{formInstanceId}")]
        public HttpResponseMessage Validate(int formInstanceId)
        {
            List<FormInstanceViewModel> formInstances = new List<FormInstanceViewModel>();
            formInstances.Add(this._folderversionservices.GetFormInstance(1, formInstanceId));
            var document = _folderversionservices.GetFormInstance(1, formInstanceId);
            if (document == null)
            {
                return CreateResponse(document, Constants.DocumentNotExist, HttpStatusCode.NotFound);
            }
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataService, base.CurrentUserName, _folderversionservices);
            FolderVersionViewModel model = this._folderversionservices.GetFolderVersion(CurrentUserId, base.CurrentUserName, 1, document.FolderVersionID, document.FolderID);
            DocumentValidatorManager validateMgr = new DocumentValidatorManager(formInstances, model, null, formDataInstanceManager, _folderversionservices, _formDesignServices, _formInstanceService);
            var validationDataList = validateMgr.ExecuteValidation(1, _formDesignServices, _folderversionservices, CurrentUserId, false, model.IsPortfolio, null);
            return Request.CreateResponse(HttpStatusCode.OK, Validators.Constants.Success);
        }


        /// <summary>
        /// Exit Validate Document.
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/Documents/ExitValidate/{formInstanceId}")]
        public HttpResponseMessage ExitValidate(int formInstanceId)
        {
            try
            {
                var document = _folderversionservices.GetFormInstance(1, formInstanceId);
                if (document == null)
                {
                    return CreateResponse(document, Constants.DocumentNotExist, HttpStatusCode.NotFound);
                }
                ExitValidateViewModel model = new ExitValidateViewModel();
                model.AddedBy = CurrentUserName;
                model.AddedDate = DateTime.Now;
                model.FolderVersionID = document.FolderVersionID;
                model.FormInstanceID = document.FormInstanceID;
                model.FormDesignVersionID = document.FormDesignVersionID;
                model.UserID = (Int32)CurrentUserId;
                model.ProductID = document.Name;
                model.Name = document.Name;
                model.FormName = "";
                model.FolderID = document.FolderID;
                model.IsQueuedForWFStateUpdate = true;
                model.UsersInterestedInStatus = "";
                var evmodelNew = _evService.Validate(model);
                ExitValidateViewModel evModel = _evService.GetExitValidateMappings(evmodelNew.QueueID);
                applicationservices.viewmodels.PBPImport.FormInstanceViewModel formInstance = new applicationservices.viewmodels.PBPImport.FormInstanceViewModel();
                formInstance.FolderVersionId = evModel.FolderVersionID;
                formInstance.FormInstanceID = evModel.FormInstanceID;
                formInstance.FormDesignVersionID = evmodelNew.FormDesignVersionID;
                formInstance.Name = evModel.Name;

                PBPExportPreProcessor preprocess = new PBPExportPreProcessor(evModel.QueueID, CurrentUserId, CurrentUserName, _formDesignServices, _folderversionservices, _formInstanceDataService, _uiElementService, _formInstanceService, null, _masterListService, formInstance.FormInstanceID);
                preprocess.ExitValidateProcessRulesAndSaveSections(formInstance);
                ExitValidateQueueInfo evQueueInfo = new ExitValidateQueueInfo { QueueId = evModel.QueueID, AssemblyName = "tmg.equinox.applicationservices", ClassName = "ExitValidateCustomQueue" };
                _evEnqueueService.CreateJobWithLowpriority(evQueueInfo);
                return Request.CreateErrorResponse(HttpStatusCode.OK, Validators.Constants.Success);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Validators.Constants.Failure);
            }
        }

        /// <summary>
        /// GEt Exit Validation errors.
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Documents/ExitValidate/{formInstanceId}")]
        public HttpResponseMessage GetExitValidateErrors(int formInstanceId)
        {
            int tenantId = 1;
            //get form instance details
            FormInstanceViewModel model = _folderversionservices.GetFormInstance(tenantId, formInstanceId);
            int pbpViewId = formInstanceId;
            List<ExitValidateResultViewModel> evModels = null;
            List<ExitValidateResultViewModel> resultModels = new List<ExitValidateResultViewModel>();

            if (model.FormDesignID == 2409)
            {
                //VBID View, get the PBP View and then get results
                List<FormInstanceViewModel> models = _folderversionservices.GetFormInstanceList(tenantId, model.FolderVersionID, model.FolderID);
                var pbpModel = from mdl in models where mdl.FormDesignID == 2367 select mdl;
                if (pbpModel != null && pbpModel.Count() > 0)
                {
                    int pbpFormInstanceId = _evService.GetPBPViewFormInstanceID(model.FolderVersionID, formInstanceId);
                    evModels = _evService.GetVBIDViewExitValidateResults(pbpFormInstanceId);
                    var currentVBIDView = models.Where(a => a.FormInstanceID == formInstanceId);
                    if (currentVBIDView != null && currentVBIDView.Count() > 0)
                    {
                        string viewNum = currentVBIDView.First().FormInstanceName.Replace("VBID View ", "");
                        int viewNumber = 0;
                        if (int.TryParse(viewNum, out viewNumber) == true)
                        {
                            string endsWithCheck = '(' + viewNumber.ToString() + ')';
                            string endsWithCheck2 = "Package " + viewNumber.ToString();
                            var filteredModels = from evModel in evModels
                                                 where evModel.Section == "Section B-19" && (evModel.Screen.EndsWith(endsWithCheck) || evModel.Screen.EndsWith(endsWithCheck2))
                                                 select evModel;
                            if (filteredModels != null && filteredModels.Count() > 0)
                            {
                                //filter 19A
                                var models19A = from modA in filteredModels where modA.Screen != null && modA.Screen.ToUpper().Contains("19A") && modA.PBPViewSection != null && modA.PBPViewSection.ToUpper().Contains("19A") select modA;
                                //filter 19B
                                var models19B = from modB in filteredModels where modB.Screen != null && modB.Screen.ToUpper().Contains("19B") && modB.PBPViewSection != null && modB.PBPViewSection.ToUpper().Contains("19B") select modB;
                                var modelsOther = from modOther in filteredModels where modOther.Screen != null && !modOther.Screen.ToUpper().Contains("19A") && !modOther.Screen.ToUpper().Contains("19B") select modOther;
                                if (models19A != null && models19A.Count() > 0)
                                {
                                    resultModels.AddRange(models19A.ToList());
                                }
                                if (models19B != null && models19B.Count() > 0)
                                {
                                    resultModels.AddRange(models19B.ToList());
                                }
                                if (modelsOther != null && modelsOther.Count() > 0)
                                {
                                    resultModels.AddRange(modelsOther.ToList());
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //map PBP View Sections
                evModels = _evService.GetLatestExitValidateResults(pbpViewId);
                if (evModels != null && evModels.Count() > 0)
                {
                    resultModels = evModels.Where(a => ((a.Section != "Section B-19") ||
                       (a.Section == "Section B-19" && string.IsNullOrEmpty(a.Screen) && string.IsNullOrEmpty(a.Question))) &&
                       (!a.Error.Contains("Are you sure you want to delete Optional Supplemental Benefit Package")) ||
                       (a.Section == "Section B-19" && !string.IsNullOrEmpty(a.Screen) && a.Screen.Equals("#19 VBID/MA Uniformity Flexibility/SSBCI"))).ToList();

                    MapPBPViewSections(evModels);
                }

            }
            return Request.CreateResponse(HttpStatusCode.OK, resultModels);
        }

        private void MapPBPViewSections(IEnumerable<ExitValidateResultViewModel> evModels)
        {
            List<ExitValidateMapModel> mapModels = _evService.GetSectionMapModels();
            foreach (var model in evModels)
            {
                var mod = from map in mapModels
                          where map.PBPSection == model.Section && (model.Screen != null && model.Screen.StartsWith(map.PBPScreenNumber))
                          select map;
                if (mod != null && mod.Count() > 0)
                {
                    model.PBPViewSection = mod.First().PBPView.Trim();
                }
            }
        }
        /// <summary>
        /// Get the part of the Document by section names.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sections"></param>
        /// <returns>Returns the data for the specified sections</returns>
        //[HttpGet]
        //[ResponseType(typeof(FormInstanceViewModel))]
        //public HttpResponseMessage GetSections(int id, string sections)
        //{
        //    List<string> missingSections = _formInstanceService.hasSections(sections, id);
        //    if (missingSections.Count > 0)
        //    {
        //        HttpError myCustomError = new HttpError(string.Format(Constants.DocumentSectionsNotExist, string.Join(",", missingSections)));
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
        //    }
        //    else
        //    {
        //        var model = _formInstanceService.GetSectionsData(id, sections);
        //        return Request.CreateResponse(HttpStatusCode.OK, model);
        //    }
        //}

        /// <summary>
        /// Get the part of the Document by repeater names.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="repeaters"></param>
        /// <returns>Returns the data for the specified repeaters.</returns>
        //[HttpGet]
        //[ResponseType(typeof(FormInstanceViewModel))]
        //public HttpResponseMessage GetRepeaters(int id, string repeaters)
        //{
        //    List<string> missingRepeaters = _formInstanceService.hasRepeaters(repeaters, id);
        //    if (missingRepeaters.Count > 0)
        //    {
        //        HttpError myCustomError = new HttpError(string.Format(Constants.DocumentRepeatersNotExist, string.Join(",", missingRepeaters)));
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
        //    }
        //    else
        //    {
        //        var model = _formInstanceService.GetSectionsData(id, repeaters);
        //        return Request.CreateResponse(HttpStatusCode.OK, model);
        //    }
        //}
    }
}
