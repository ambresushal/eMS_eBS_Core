using System.Collections.Generic;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
//using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.web.Framework;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using System.IO;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities.Enums;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;
using tmg.equinox.web.Framework.Caching;
using System.Linq;
using System.Threading.Tasks;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager;
using tmg.equinox.dependencyresolution;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.web.Controllers
{
    public class ODMController : AuthenticatedController
    {
        private IOdmService _OdmService;
        private IPBPImportService _pBPImportService;
        private IFormDesignService _fdService;
        private IUnitOfWorkAsync _unitOfWork;

        public ODMController(IOdmService OdmService, IPBPImportService pBPImportService, IFormDesignService fdService, IUnitOfWorkAsync unitOfWork)
        {
            this._OdmService = OdmService;
            this._pBPImportService = pBPImportService;
            this._fdService = fdService;
            this._unitOfWork = unitOfWork;
        }

        public ActionResult ODM()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View("~/Views/PBPImport/ODM.cshtml");
        }

        public JsonResult FormDesignParentSectionList(int tenantId, string formName, string year)
        {
            IEnumerable<ODMParentSectionDeatilsViewModel> sectionList = new List<ODMParentSectionDeatilsViewModel>();
            int formDesignVersionID = 0;
            if (year.Trim() != "" && year != null)
            {
                var lastDayOfTheYear = new DateTime(Convert.ToInt32(year), 12, 31);
                formDesignVersionID = _fdService.GetLatestFormDesignVersionID(formName, lastDayOfTheYear);
                sectionList = _OdmService.GetParentSectionsFromFolderVersion(tenantId, formDesignVersionID);//_OdmService.GetSectionList(formDesignVersionID);
            }
            return Json(sectionList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadPBPFiles()
        {
            string file = string.Empty;
            string ODMFILEPATH = ConfigurationManager.AppSettings["ODMAccessFilePath"];
            if (Request.Files.Count > 0)
            {
                try
                {
                    List<string> FileNameList = _OdmService.SaveFiles(Request, ODMFILEPATH);

                    if (FileNameList.Count > 0)
                    {
                        file = Path.GetFileName(FileNameList[0].ToString());
                    }


                    ODMConfigrationViewModel PlanConfigrations = _OdmService.GetPlanDetails(Request, file, ODMFILEPATH);

                    PlanConfigrations.FileName = FileNameList[0].ToString();
                    PlanConfigrations.OriginalFileName = FileNameList[1].ToString();

                    return Json(PlanConfigrations);

                }
                catch (Exception ex)
                {
                    return Json(0);
                }
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult GetMigrationQueue(GridPagingRequest gridPagingRequest)
        {
            List<ODMMigrationQueueViewModel> odmMigarationQueue = new List<ODMMigrationQueueViewModel>();
            odmMigarationQueue = _OdmService.GetMigrationQueue(gridPagingRequest);
            return Json(odmMigarationQueue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QueueForMigration(string planData, string pbpSections, string sotSections, string description, string year, string fileName, string originalFileName)
        {
            ServiceResult result = new ServiceResult();
            ODMCacheHandler handler = new ODMCacheHandler();

            var lastDayOfTheYear = new DateTime(Convert.ToInt32(year), 12, 31);
            int formDesignVersionIDSOT = _fdService.GetLatestFormDesignVersionID("Medicare", lastDayOfTheYear);
            int formDesignVersionIDPBP = _fdService.GetLatestFormDesignVersionID("PBPView", lastDayOfTheYear);
            result = _OdmService.QueueForMigration(planData, pbpSections, sotSections, description, year, fileName, originalFileName, formDesignVersionIDSOT, formDesignVersionIDPBP, base.CurrentUserName);

            var planTestData = JsonConvert.DeserializeObject(planData);
            List<JToken> planDataList = ((JArray)planTestData).ToList();

            for (int i = 0; i < planDataList.Count; i++)
            {
                string data = handler.IsExists(Convert.ToInt32(planDataList[i]["FolderVersionId"]));
                if (data == null)
                {
                    handler.Add(Convert.ToInt32(planDataList[i]["FolderVersionId"]));
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BaselineAndCreateNewMinorVersion(int batchId, bool isBeforeBaseline, bool isAfterBaseline, bool runManualUpdateOnly)
        {
            ServiceResult result = new ServiceResult();
            ODMCacheHandler handler = new ODMCacheHandler();
            List<ODMPlanConfigViewModel> planList = _OdmService.planList(batchId);
            Task.Delay(300000).Wait();
            try
            {
                if (isBeforeBaseline)
                {
                    _OdmService.BaselineAndCreateNewMinorVersion(planList, batchId, Convert.ToInt32(base.CurrentUserId), base.CurrentUserName, false);
                }
                IOnDemandMigrationExecutionManager ODMExecutionManager = new OnDemandMigrationExecutionManager(UnityConfig.Resolve<IUnitOfWorkAsync>());
                //ODMExecutionManager.ExecuteAsync(batchId);


                //This code is added temporary for testing ODM functionality
                ODMExecutionManager.Execute(batchId, runManualUpdateOnly);
                List<ODMPlanConfigViewModel> planListNew = _OdmService.planList(batchId);
                if (isAfterBaseline)
                {
                    _OdmService.BaselineAndCreateNewMinorVersion(planListNew, batchId, Convert.ToInt32(base.CurrentUserId), base.CurrentUserName, true);
                }
                //End testing block

                foreach (ODMPlanConfigViewModel plan in planList)
                {
                    handler.Remove(plan.FolderVersionId);
                }
            }
            catch (Exception ex)
            {
                foreach (ODMPlanConfigViewModel plan in planList)
                {
                    handler.Remove(plan.FolderVersionId);
                    handler.RemoveOpenFolderVersion(plan.FolderVersionId);
                }

                MigrationBatchs migPlan = this._unitOfWork.RepositoryAsync<MigrationBatchs>()
                                                                       .Query()
                                                                       .Filter(c => c.BatchId == batchId)
                                                                       .Get()
                                                                       .FirstOrDefault();
                if (migPlan != null)
                {
                    migPlan.Status = "Fail";
                    migPlan.ExecutedDate = DateTime.Now;
                    migPlan.IsActive = false;
                    this._unitOfWork.RepositoryAsync<MigrationBatchs>().Update(migPlan);
                    this._unitOfWork.Save();
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
