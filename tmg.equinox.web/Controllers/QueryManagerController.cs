using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using tmg.equinox.web.QueryManager;
using tmg.equinox.web.QueryManager.DataBaseObjects;
using tmg.equinox.caching.client;
using OfficeOpenXml;
using System.IO;
using tmg.equinox.applicationservices.viewmodels.QueryManager;
using System.Text;

namespace tmg.equinox.web.Controllers
{
    public class QueryManagerController : AuthenticatedController
    {
        #region Private Variables
        private QueryExecutor Executor = null;
        private QueryManagerViewModel model = null;
        #endregion

        #region Action Methods
        // GET: QueryEditor
        [Authorize(Roles = "TMG Super User")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "TMG Super User")]
        public ActionResult ExecuteQuery(string cnName, string queryText, bool IsCommit)
        {
            model = new QueryManagerViewModel(HttpUtility.UrlDecode(queryText), IsCommit);

            if (model.UserQuery != null && model.UserQuery.Trim().Length > 0)
            {
                Executor = new QueryExecutor();
                string cnString = WebConfigurationManager.ConnectionStrings[cnName].ConnectionString;
                Executor.Initialize(cnString);

                if (Executor.TestConnection())
                    Executor.ExecQuery(model);
                else
                    model.ResultComments = "Couldn't connect to the database. ";

                if (Executor.Error)
                {
                    model.ResultComments = model.ResultComments + GetQueryErrors();
                }
                else
                {
                    MemoryStream fileStream = ExporttoExcel();

                    byte[] byteArray = new byte[fileStream.Length];
                    fileStream.Position = 0;
                    fileStream.Read(byteArray, 0, (int)fileStream.Length);

                    var fileDownloadName = "SQLQueryResult_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(byteArray, contentType, fileDownloadName);
                }
            }

            return View("Index", model);
        }

        [HttpGet]
        [Authorize(Roles = "TMG Super User")]
        public JsonResult GetSQLObjectList(string cnName, string selectedIndex)
        {
            string cnString = WebConfigurationManager.ConnectionStrings[cnName].ConnectionString;
            SqlConnector DataProvider = new SqlConnector(cnString);
            List<ISqlObject> FilteredObjs = new List<ISqlObject>();

            switch (selectedIndex)
            {
                case "1"://Tables
                    DataProvider.LoadTables();
                    FilteredObjs = DataProvider.DbObjects.Where(X => X.Kind == ObjectType.Table).ToList();
                    break;
                case "2"://Views
                    DataProvider.LoadViews();
                    FilteredObjs = DataProvider.DbObjects.Where(X => X.Kind == ObjectType.View).ToList();
                    break;
                case "3"://Stored procedures
                    DataProvider.LoadProcedures();
                    FilteredObjs = DataProvider.DbObjects.Where(X => X.Kind == ObjectType.Procedure).ToList();
                    break;
                case "4"://Scalar functions
                    DataProvider.LoadScalarFunctions();
                    FilteredObjs = DataProvider.DbObjects.Where(X => X.Kind == ObjectType.ScalarFunction).ToList();
                    break;
            }
            return Json(FilteredObjs.Select(x => new {
                id = x.Id,
                schema = x.Schema,
                name = x.Name}), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "TMG Super User")]
        public ActionResult GetSQLObjectScript(string cnName, int id, string objectType)
        {
            string cnString = WebConfigurationManager.ConnectionStrings[cnName].ConnectionString;
            SqlConnector DataProvider = new SqlConnector(cnString);
            List<ISqlObject> FilteredObjs = new List<ISqlObject>();

            switch (objectType)
            {
                case "1"://Tables
                    DataProvider.LoadTables(true, id);
                    break;
                case "2"://Views
                    DataProvider.LoadViews(true, id);
                    break;
                case "3"://Stored procedures
                    DataProvider.LoadProcedures(true, id);
                    break;
                case "4"://Scalar functions
                    DataProvider.LoadScalarFunctions(true, id);
                    break;
            }

            if(DataProvider.DbObjects[0].Script.Length > 0)
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(DataProvider.DbObjects[0].Script);
                
                var fileDownloadName = DataProvider.DbObjects[0].Name + "_Script" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                var contentType = "application/text";

                return File(byteArray, contentType, fileDownloadName);
            }

            return View("Index");
        }

        #endregion

        #region Private Methods
        private MemoryStream ExporttoExcel()
        {
            int AffectedRecords = 0, intaux;
            List<string> ResultQueue = new List<string>();
            string resultComm = string.Empty;

            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                foreach (DataTable dt in Executor.Results.Tables)
                {
                    if (dt.TableName.StartsWith("NonQuery", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ResultQueue.Add(String.Format("Affected Records: {0}", dt.Rows[0][0].ToString()));
                        resultComm = resultComm + String.Format("Affected Records: {0}", dt.Rows[0][0].ToString()) + "\n";

                        if (int.TryParse(dt.Rows[0][0].ToString(), out intaux))
                            AffectedRecords += intaux;
                    }
                    else
                    {
                        ExcelWorksheet worksheet = excelPkg.Workbook.Worksheets.Add(dt.TableName);
                        worksheet.Cells.LoadFromDataTable(dt, true);
                        worksheet.Row(1).Style.Font.Bold = true;
                        worksheet.Cells.AutoFitColumns();
                    }
                }

                //Add result sheet
                ExcelWorksheet rworksheet = excelPkg.Workbook.Worksheets.Add("Result");
                rworksheet.Cells.LoadFromCollection(ResultQueue, false);

                var fileStream = new MemoryStream();
                excelPkg.SaveAs(fileStream);
                fileStream.Position = 0;

                model.ResultComments = resultComm;

                return fileStream;
            }
        }

        public string GetQueryErrors()
        {
            string errorList = string.Empty;

            if (Executor.SqlEx != null)     //Sql exception
            {
                int i, n = Executor.SqlEx.Errors.Count;
                for (i = 0; i < n; i++)
                    errorList = errorList + Executor.SqlEx.Errors[i].Message + "\n";
            }
            else if (Executor.NrEx != null) //regular exception                
                errorList = errorList + Executor.NrEx.Message + "\n";
            else                            //Exception from the executor??            
                errorList = errorList + "Error\n";

            return errorList;
        }
        #endregion
    }
}