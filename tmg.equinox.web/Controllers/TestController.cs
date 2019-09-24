
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.web.DataSource;
using tmg.equinox.web.Framework;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;

namespace tmg.equinox.web.Controllers
{
    public class TestController : AuthenticatedController
    {
        private IFormDesignService _formDesignService { get; set; }
        private IUIElementService _uIEelementService { get; set; }
        private IDomainModelService _domainModelService { get; set; }
        private IFolderVersionServices _folderService { get; set; }
        private IDataValueService _dataValueService { get; set; }
        private IServiceDesignService _serviceDesignService { get; set; }



        public TestController(IFormDesignService formDesignService, IUIElementService uIEelementService,
                                IDomainModelService domainModelService, IFolderVersionServices folderService, IDataValueService dataValueService, IServiceDesignService serviceDesignService)
        {
            _formDesignService = formDesignService;
            _uIEelementService = uIEelementService;
            _domainModelService = domainModelService;
            _folderService = folderService;
            _dataValueService = dataValueService;
            _serviceDesignService = serviceDesignService;
        }

        //
        // GET: /Test/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetFormDesignList(int tenantID)
        {
            var formDesignList = this._formDesignService.GetFormDesignList(tenantID);
            return View(formDesignList);
        }

        //public ActionResult AddFormDesign()
        //{
        //    ServiceResult result = _formDesignService.AddFormDesign(User.Identity.Name, 1, "Form 1", "Form 1", "fmr1", false, 1, 0);

        //    return View(result);
        //}

        public ActionResult GetFormDesignVersionList(int tenantID, int formDesignID)
        {
            var formDesignVersionList = this._formDesignService.GetFormDesignVersionList(tenantID, formDesignID);
            return View(formDesignVersionList);
        }

        public ActionResult InsertFormDesignVersion()
        {
            ServiceResult result = _formDesignService.AddFormDesignVersion(User.Identity.Name, 1, 1, DateTime.Now, "Version1", "Data");

            return View(result);
        }

        public ActionResult UpdateFormDesignVersion()
        {
            ServiceResult result = _formDesignService.UpdateFormDesignVersion(User.Identity.Name, 1, 1, DateTime.Now.AddDays(1), "Version2");
            return View(result);
        }

        //public async Task<ActionResult> AddSectionElement()
        //{
        //    await this._uIEelementService.AddSectionDesign(User.Identity.Name, 1, 1, 1, "admin", "Section");
        //    return null;
        //}

        public ActionResult UpdateSectionElement()
        {
            this._uIEelementService.UpdateSectionDesign(User.Identity.Name, 1, 1, 1, 1457, false, "admin Help me!!!!", false, false, "admin Help Me!!!!!!!!!!!!", 3, false, true, "TestAvaniush", "Description", null, false, false, "", null,3,true,null);
            return null;
        }

        public ActionResult GetSectionElement()
        {
            this._uIEelementService.GetSectionDesignDetail(1, 1, 1457);
            return null;
        }

        public ActionResult GetValidator()
        {
            var validatorList = this._uIEelementService.GetValidator(1, 1, 3);
            return null;
        }

        public ActionResult AddValidator()
        {
            this._uIEelementService.AddValidator(User.Identity.Name, 1, 1, 3, 1, true, true, "test", "test");
            return null;
        }

        public ActionResult UpdateValidator()
        {
            this._uIEelementService.UpdateValidator(User.Identity.Name, 1, 1, 3, 1036, 1, false, true, "test", "test");
            return null;
        }

        public ActionResult GetRules()
        {
            var ruleList = this._uIEelementService.GetRulesForUIElement(1, 1, 284);
            return null;
        }

        public ActionResult AddExpression()
        {
            IList<ExpressionRowModel> lst = new List<ExpressionRowModel>();
            lst.Add(new ExpressionRowModel
            {
                RuleId = 1,
                TenantId = 1,
                RightOperand = "Right",
                OperatorTypeId = 1,
                LogicalOperatorTypeId = 1
            });

            lst.Add(new ExpressionRowModel
            {
                RuleId = 2,
                TenantId = 1,
                RightOperand = "Right2",
                OperatorTypeId = 1,
                LogicalOperatorTypeId = 1
            });


            this._uIEelementService.AddExpressions(User.Identity.Name, lst);
            return null;
        }

        public ActionResult UpdateExpression()
        {
            IList<ExpressionRowModel> lst = new List<ExpressionRowModel>();
            lst.Add(new ExpressionRowModel
            {
                ExpressionId = 13203,
                RuleId = 1,
                TenantId = 1,
                RightOperand = "RightUpdate",
                OperatorTypeId = 1,
                LogicalOperatorTypeId = 1
            });

            lst.Add(new ExpressionRowModel
            {
                ExpressionId = 13204,
                RuleId = 2,
                TenantId = 1,
                RightOperand = "Right2Update",
                OperatorTypeId = 1,
                LogicalOperatorTypeId = 1
            });
            this._uIEelementService.UpdateExpressions(User.Identity.Name, lst);
            return null;
        }

        public ActionResult DeleteExpression()
        {
            IList<int> lst = new List<int>();
            lst.Add(13202);
            lst.Add(13201);

            this._uIEelementService.DeleteExpressions(User.Identity.Name, 1, 1, 1, 1, lst);
            return null;
        }

        public ActionResult GetUIElementListForFormDesignVersion(int tenantId, int formDesignVersionId)
        {
            var uiElementList = this._uIEelementService.GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId);
            return View(uiElementList);
        }

        public ActionResult GetUIElementList(int tenantId, int formDesignVersionId)
        {
            var uiElementList = this._uIEelementService.GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId);
            return Json(uiElementList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTextBox()
        {
            var textBox = this._uIEelementService.GetTextBox(1, 1, 3);
            return null;
        }

        public ActionResult AddTextBox()
        {
            //var result = this._uIEelementService.AddTextBox("Avanish", 1, 1, 1459, 2, false, false, false, "TestTextBox", "Test text box by Avanish", 1);
            return null;
        }

        public ActionResult UpdateTextBox()
        {
            //var result = this._uIEelementService.UpdateTextBox("admin", 1, 1, 1460, false, false, false, "Visibility falsified by admin", false, false, false, 1, "admin", "admin", 2222, 1, false, false, "", 0, null, false, false, "");
            return null;
        }

        public ActionResult CopyFormDesignVersion()
        {
            ServiceResult result = _formDesignService.CopyFormDesignVersion("admin", 1, 10, DateTime.Now, "Version1", "Data");
            return null;
        }

        public ActionResult GetRadioButton()
        {
            var radioButton = this._uIEelementService.GetRadioButton(1, 1, 28);
            return null;
        }

        public ActionResult UpdateRadioButton()
        {
            var radioButton = this._uIEelementService.UpdateRadioButton(User.Identity.Name, 1, 1, 1, 28, true, true, true, "Great Radio Button", true, "Great Radio", "Option1", "Option2", false, 1, true, null, false, false, string.Empty, true, 1, true,null);
            return null;
        }

        public ActionResult AddRadioButton()
        {
            //var result = this._uIEelementService.AddRadioButton(User.Identity.Name, 1, 1, 2, 4, "Added Radio Button", "Hep Radio button", 1);
            return null;
        }

        public ActionResult GetDropDown()
        {
            var dropDown = this._uIEelementService.GetDropDown(1, 1, 6);
            return null;
        }

        public ActionResult AddDropDown()
        {
            //var result = this._uIEelementService.AddDropDown("admin", 1, 1, 1459, 5, "TestDropDown", "Test Drop Down By admin", 1);
            return null;
        }

        public ActionResult UpdateDropDown()
        {
            //var result = this._uIEelementService.UpdateDropDown("admin", 1, 1,1, 1464, false, false, false, "Visibility falsified by admin", false, "Selected", "VisibleFalseDDL", 1, null, null, false, false, string.Empty, 1, false);
            var result = this._uIEelementService.UpdateDropDown("admin", 1, 1, 1, 1464, false, false, false, "Visibility falsified by admin", false, "Selected", "VisibleFalseDDL", 1, null, null, false, false, string.Empty, 1, false, false, null, null, true, true,1,false, true,null);
            return null;
        }

        public ActionResult GetCalendar()
        {
            var calendar = this._uIEelementService.GetCalendar(1, 1, 1465);
            return null;
        }

        public ActionResult AddCalendar()
        {
            //var result = this._uIEelementService.AddCalendar("admin", 1, 1, 1459, 5, "TestCalendar", "Test Drop Down By admin", 1);
            return null;
        }

        public ActionResult UpdateCalendar()
        {
            var result = this._uIEelementService.UpdateCalendar("admin", 1, 1, 1, 1465, false, false, false, "Visibility falsified by admin", false, "VisibleFalseCalendar", 1, DateTime.Now, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), null, false, false, string.Empty, true,1, true,null);
            return null;
        }

        public ActionResult GetCheckBox()
        {
            var checkBox = this._uIEelementService.GetCheckBox(1, 1, 1466);
            return null;
        }

        public ActionResult AddCheckBox()
        {
            //var result = this._uIEelementService.AddCheckBox("admin", 1, 1, 1459, 4, "TestCheckBox", "Test text box by admin", 1);
            return null;
        }

        public ActionResult UpdateCheckBox()
        {
            var result = this._uIEelementService.UpdateCheckBox("admin", 1, 1, 1, 1466, false, false, false, "Visibility falsified by admin", false, "VisibleFalseCheckbox", "optionalLabel", 1, false, null, false, false, string.Empty, true,1, true,null);
            return null;
        }

        public ActionResult GetRepeater()
        {
            var repeater = this._uIEelementService.GetRepeater(1, 1, 1467);
            return null;
        }

        public ActionResult AddRepeater()
        {
            //this._uIEelementService.AddRepeater("admin", 1, 1, 1, 2, "Repeater 1", "Repeater 1", 1);
            return null;
        }

        public ActionResult UpdateRepeater()
        {

            this._uIEelementService.UpdateRepeater("admin", 1, 1, 1, 1467, false, false, false, "Visibility Disabled by admin", false, "VisibileFalseRepeater", 1, 1, 1, true, "RepeaterDS", "Description", null, false, false, string.Empty, false, false, false, new RepeaterElementModel(), new RepeaterUIElementPropertyModel(), true,null);
            return null;
        }

        public ActionResult DeleteFormDesignVersion()
        {
            this._formDesignService.DeleteFormDesignVersion("admin", 1, 6, 1);
            return null;
        }

        public ActionResult DeleteFormDesign(int id)
        {
            this._formDesignService.DeleteFormDesign("admin", 1, id);
            return null;
        }

        public ActionResult DeleteTextBox()
        {
            this._uIEelementService.DeleteTextBox(1, 9, 1);
            return null;
        }

        public ActionResult DeleteDropDown()
        {
            this._uIEelementService.DeleteDropDown(1, 9, 1);
            return null;
        }

        public ActionResult DeleteCalendar()
        {
            this._uIEelementService.DeleteCalendar(1, 9, 1);
            return null;
        }

        public ActionResult DeleteCheckBox()
        {
            this._uIEelementService.DeleteCheckBox(1, 9, 1);
            return null;
        }

        public ActionResult DeleteRadioButton()
        {
            this._uIEelementService.DeleteRadioButton(1, 9, 1);
            return null;
        }

        public ActionResult DeleteRepeater()
        {
            this._uIEelementService.DeleteRepeater(1, 9, 1);
            return null;
        }

        public ActionResult DeleteSection()
        {
            this._uIEelementService.DeleteSection(1, 9, 1);
            return null;
        }

        public ActionResult Error()
        {
            throw new NotImplementedException("boom");
        }

        public ActionResult GetFormDesignVersionData()
        {
            string data = this._formDesignService.GetFormDesignVersionData(1, 29);
            return null;
        }

        public ActionResult GetFolderVersionData()
        {
            var data = this._folderService.GetFolderVersion(1, "mvaid", 1, 2, 1);
            return null;
        }

        public ActionResult GetFolderVersionWorkflowData()
        {
            //var data = this._folderService.GetFolderVersionWorkFlowList(1, 2);
            return null;
        }

        //public ActionResult GetFormInstanceData()
        //{
        //    var data = this._folderService.GetFormInstanceList(1, 2, 1);
        //    return null;
        //}

        public ActionResult CreateDomainModel()
        {
            _domainModelService.Create(1, 32);
            return null;
        }

        public ActionResult SaveFormDataValues()
        {
            string data =
                "{\'PersonalDetails\': { \'EducationalDetails\':  [{ \'SchoolCollegeName\':  \'Indo\'  ,\'Place\':  \'Hyd\'  ,\'Standard\':  \'10\'  ,\'Grade\':  \'75\'  }] , \'PersonalInfo\': {\'FirstName\':  \'\'  ,\'LastName\':  \'\'  ,\'Sex\':  \'false\' }},\'Addresses\': { \'Address\':  [ { \'Area\':  \'Area1\'  ,\'CityTown\':  \'City1\'  ,\'State\':  \'State1\'  , \'PinCode\':  \'PinCode1\'   }, { \'Area\':  \'Area2\'  ,\'CityTown\':  \'City2\'  ,\'State\':  \'State2\'  , \'PinCode\':  \'PinCode2\'   }, { \'Area\':  \'Area3\'  ,\'CityTown\':  \'City3\'  ,\'State\':  \'State3\'  , \'PinCode\':  \'PinCode3\'   }]}}";
            var converter = new ExpandoObjectConverter();
            dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(data, converter);
            _dataValueService.GetDeserializedJsonObject(jsonObject, 31, 1);
            return null;
        }
        public ActionResult SaveBaseLineFolderdata()
        {
            //this._folderService.BaseLineFolder(1,1,1,1,"admin","admin");

            return null;
        }

        public ActionResult PrepareJsonObjectFromDomainModel()
        {
            //var detail2 = this._formDesignService.GetFormDesignVersionDetail(1, 32);
            //string jsonData = this._folderService.GetFormInstanceData(1, 5);
            //detail2.JSONData = detail2.GetDefaultJSONDataObject();

            //if (detail2.DataSources != null && detail2.DataSources.Count > 0)
            //{
            //    var dm = new DataSourceMapper(1, 5, 12, 31, _folderService, detail2.JSONData, detail2);
            //    dm.AddDataSourceRange(detail2.DataSources);
            //    detail2.JSONData = dm.MapDataSources();
            //}

            var detail2 = this._formDesignService.GetFormDesignVersionDetailFromDataModel(1, 32);
            detail2.JSONData = detail2.GetDefaultJSONDataObject();

            return null;
        }

        public ActionResult DeleteFormInstance(int folderId, int folderVersionId, int formInstanceId)
        {
            ServiceResult result = this._folderService.DeleteFormInstance(folderId, 1, folderVersionId, formInstanceId, CurrentUserName);

            if (result.Result == ServiceResultStatus.Success)
                return Content("Deleted successfully.");
            else
                return Content("Unable to delete ");
        }

        public ActionResult DeleteFolderVersion(int folderVersionId)
        {
            ServiceResult result = this._folderService.DeleteFolderVersion(1, 1, folderVersionId, null, CurrentUserName);

            if (result.Result == ServiceResultStatus.Success)
                return Content("Deleted successfully.");
            else
                return Content("Unable to delete ");
        }

        public ActionResult DeleteFolder(int folderId)
        {
            ServiceResult result = this._folderService.DeleteFolder(1, folderId);

            if (result.Result == ServiceResultStatus.Success)
                return Content("Deleted successfully.");
            else
                return Content("Unable to delete ");
        }

        public ActionResult IsValidFolderVersionNumber(string versionNumber, int tenantId)
        {
            bool isValid = false;

            //Validate FolderVersionNumber if and only if this has format of 'XXXX_X.XX'
            var _regex = new Regex(@"^[0-9]{4}[_][0-9][.][0-9]{1,2}$");
            Match match = _regex.Match(versionNumber);
            if (match.Success)
            {
                isValid = true;
            }
            return Content("" + isValid);
        }

        public ActionResult GetNextMinorVersionNumber(string versionNumber, DateTime effectiveDate)
        {
            string nextVersionNumber = null;
            int result;
            var b = int.TryParse((versionNumber.Split('_')[1]).Split('.')[1], out result);

            if (b)
            {
                if (result > 0)
                {
                    int result2;
                    var tryParse = int.TryParse((versionNumber.Split('_')[1]).Split('.')[0], out result2);

                    if (tryParse)
                    {
                        result2 += 1;
                        nextVersionNumber = versionNumber.Split('_')[0] + "_" +
                                             result2 + ".1";
                    }
                }
                else
                {
                    int result1;
                    var tryParse = int.TryParse((versionNumber.Split('_')[1]).Split('.')[0], out result1);
                    if (tryParse)
                    {
                        nextVersionNumber = versionNumber.Split('_')[0] + "_" +
                                        result1 + ".1";
                    }
                }
            }
            else
            {
                nextVersionNumber = "Not Valid";
            }

            return Content("" + nextVersionNumber);
        }


        public ActionResult ServiceDesignData(int serviceDesignVersionID)
        {
            string data = _serviceDesignService.GetServiceDesignVersionDetail(1, serviceDesignVersionID).GetJsonDataObject();
            return Content(data);
        }
    }
}