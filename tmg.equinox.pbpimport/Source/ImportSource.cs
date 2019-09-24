using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using Newtonsoft.Json.Linq;
//using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices;
using System.Configuration;
//using tmg.equinox.infrastructure.exceptionhandling;
//using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.pbpimport.Interfaces;

namespace tmg.equinox.pbpimport
{
    public class ImportSource
    {
        IAccessDbContext _accessDbContext;
        IPBPImportHelperServices _PBPImportHelperServices;
        ISQLImportOperations _SQLImportOperations;
        IPBPImportActivityLogServices _PBPImportActivityLogServices;
        IPBPMappingServices _PBPMappingServices;

        string _applicationPBPPlanConnectingString = string.Empty;
        string _applicationPBPPlanAreaConnectingString = string.Empty;
        IPBPImportService _pBPImportService = null;
        IUnitOfWorkAsync _unitOfWorkAsync = null;
        IFolderVersionServices _folderVersionService = null;
        IFormDesignService _formDesignService = null;
        ILoggingService _loggingService = null;
        IDomainModelService _domainModelService = null;
        string IMPORTFILEPATH = ConfigurationManager.AppSettings["PBPImportFiles"].ToString();


        public ImportSource(IUnitOfWorkAsync unitOfWorkAsync, IFolderVersionServices folderVersionService, ILoggingService loggingService, IDomainModelService domainModelService, IPBPImportService pBPImportService, IFormDesignService formDesignService,
                            IAccessDbContext accessDbContext, IPBPImportHelperServices PBPImportHelperServices, ISQLImportOperations SQLImportOperations, IPBPImportActivityLogServices PBPImportActivityLogServices, IPBPMappingServices PBPMappingServices)
        {
            this._folderVersionService = folderVersionService;
            this._loggingService = loggingService;
            this._domainModelService = domainModelService;
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._pBPImportService = pBPImportService;
            this._formDesignService = formDesignService;
            this._accessDbContext = accessDbContext;
            this._PBPImportHelperServices = PBPImportHelperServices;
            this._SQLImportOperations = SQLImportOperations;
            this._PBPImportActivityLogServices = PBPImportActivityLogServices;
            this._PBPMappingServices = PBPMappingServices;
        }

        public void StartPBPImportOperation(int pBPImportQueueID)
        {
            PBPImportQueueViewModel QueuedPBPImport = _pBPImportService.GetQueuedPBPImportList()
                                                                .Where(s => s.PBPImportQueueID.Equals(pBPImportQueueID))
                                                                .FirstOrDefault();
            if (QueuedPBPImport != null)
            {
                Common CommonServiceObj = new Common(this._unitOfWorkAsync);
                IEnumerable<PBPImportTablesViewModel> PBPImportTableList = CommonServiceObj.GetPBPImportTablesList();

                //_accessDbContext.InitializeVariables(Path.Combine(IMPORTFILEPATH, QueuedPBPImport.PBPFileName));
                //this._applicationPBPPlanConnectingString = _accessDbContext.GetConnectingString();
                //this._PBPPlanAccessDbContext = new AccessDbContext(Path.Combine(IMPORTFILEPATH, QueuedPBPImport.PBPFileName));
                //this._applicationPBPPlanConnectingString = this._PBPPlanAccessDbContext._connectingString;

                //_accessDbContext.InitializeVariables(Path.Combine(IMPORTFILEPATH, QueuedPBPImport.PBPPlanAreaFileName));
                //this._applicationPBPPlanAreaConnectingString = _accessDbContext.GetConnectingString();
                //this._PBPPlanAreaAccessDbContext = new AccessDbContext(Path.Combine(IMPORTFILEPATH, QueuedPBPImport.PBPPlanAreaFileName));
                //this._applicationPBPPlanAreaConnectingString = this._PBPPlanAreaAccessDbContext._connectingString;

                _PBPImportHelperServices.InitializeVariables(this._unitOfWorkAsync);
                //PBPImportHelperServices Helper = new PBPImportHelperServices(this._unitOfWorkAsync);
                //_PBPImportHelperServices.UpdateImportQueueStatus(QueuedPBPImport.PBPImportQueueID, ProcessStatusMasterCode.InProgress);
                PerformImportOperationWithSequence(QueuedPBPImport.PBPImportQueueID, PBPImportTableList, null, QueuedPBPImport.IsFullMigration, QueuedPBPImport);
            }
        }

        public void PerformImportOperationWithSequence(int pBPImportQueueID, IEnumerable<PBPImportTablesViewModel> collPBPImportTablesList, string username, bool IsFullMigration, PBPImportQueueViewModel QueuedPBPImport)
        {
            //try
            {
                _accessDbContext.InitializeVariables(Path.Combine(IMPORTFILEPATH, QueuedPBPImport.PBPFileName));
                this._applicationPBPPlanConnectingString = _accessDbContext.GetConnectingString();
                CreatePBPDataMap(this._applicationPBPPlanConnectingString, pBPImportQueueID, collPBPImportTablesList, username);

                _accessDbContext.InitializeVariables(Path.Combine(IMPORTFILEPATH, QueuedPBPImport.PBPPlanAreaFileName));
                this._applicationPBPPlanAreaConnectingString = _accessDbContext.GetConnectingString();
                CreatePBPDataMap(this._applicationPBPPlanAreaConnectingString, pBPImportQueueID, collPBPImportTablesList, username);

                PerformUserAction PerformUserActionServiceObj = new PerformUserAction(this._pBPImportService, this._unitOfWorkAsync, this._folderVersionService, this._formDesignService, _PBPImportHelperServices, _PBPImportActivityLogServices, _PBPMappingServices, QueuedPBPImport.Year);
                PerformUserActionServiceObj.PerformUserActionOnPBPPlan(pBPImportQueueID, IsFullMigration);
            }
        }

        private void CreatePBPDataMap(string connectionString, int pBPImportQueueID, IEnumerable<PBPImportTablesViewModel> collPBPImportTablesList, string username)
        {
            string message = string.Empty, strSourceTableName = string.Empty, strTargetTableName = string.Empty;
            _SQLImportOperations.InitializeVariables(connectionString);
            //SqlImportOperations objSqlImportOperations = new SqlImportOperations(connectionString);
            DataTable sourceUsedTables = _accessDbContext.GetUsedTables();
            int totalRows = 0;
            List<ExceptionalTable> ExceptinalTableList = ExceptionTableList();
            foreach (PBPImportTablesViewModel objPBPImportTablesViewModel in collPBPImportTablesList)
            {
                //try
                {
                    bool isTablePresent = false;
                    for (int i = 0; i < sourceUsedTables.Rows.Count; i++)
                    {
                        string strTalbePresent = sourceUsedTables.Rows[i][2].ToString();//.ToUpper();
                        if (objPBPImportTablesViewModel.PBPTableName.Contains(strTalbePresent))
                        {
                            isTablePresent = true;
                            break;
                        }
                    }
                    if (isTablePresent)
                    {
                        strSourceTableName = objPBPImportTablesViewModel.PBPTableName;
                        string strQuery = "SELECT * FROM " + strSourceTableName;
                        //      try
                        {
                            DataTable sourceTable = new DataTable();
                            DataTable targerTable = new DataTable();
                            try
                            {
                                sourceTable = _accessDbContext.ExecuteSelectQuery(strQuery, null);
                                Mapping MappingServiceObj = new Mapping(this._folderVersionService, null, this._unitOfWorkAsync, null, null, null,null);
                                targerTable = MappingServiceObj.CreateTableWithColumn();

                                if (ExceptinalTableList.Where(s => s.TableName.Equals(strSourceTableName)).Any())
                                {
                                    targerTable = ExceptionalTableHandler(sourceTable, strSourceTableName, pBPImportQueueID);
                                    MappingServiceObj.SavePBPDataMapList(targerTable);
                                    sourceTable = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
                                //PBPImportActivityLogServices PBPImportActivityLogServicesObj = new PBPImportActivityLogServices(this._unitOfWorkAsync);
                                _PBPImportActivityLogServices.AddPBPImportActivityLog(pBPImportQueueID, "CreatePBPDataMap()", null, strSourceTableName, null, ex);
                                sourceTable = null;
                                targerTable = null;
                            }
                            if (sourceTable != null)
                            {
                                foreach (DataRow dRow in sourceTable.Rows)
                                {
                                    try
                                    {
                                        Mapping MappingServ = new Mapping(this._folderVersionService, null, this._unitOfWorkAsync, null, null, null,null);
                                        DataColumn column = new DataColumn("QID");
                                        DataRow row = targerTable.NewRow();
                                        row["JsonData"] = MappingServ.CreateJsonForRow(dRow, sourceTable.Columns);
                                        row["QID"] = dRow["QID"].ToString();
                                        row["TableName"] = strSourceTableName;
                                        row["FieldName"] = string.Empty;
                                        row["PBPImportQueueID"] = pBPImportQueueID;
                                        targerTable.Rows.Add(row);
                                        //targerTable.AcceptChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
                                        //PBPImportActivityLogServices PBPImportActivityLogServicesObj = new PBPImportActivityLogServices(this._unitOfWorkAsync);
                                        _PBPImportActivityLogServices.AddPBPImportActivityLog(pBPImportQueueID, "CreatePBPDataMap()", "Access Table in to Json Convert.", strSourceTableName, null, ex);
                                    }
                                }

                                totalRows = sourceTable.Rows.Count;
                                try
                                {
                                    Mapping MappingObj = new Mapping(this._folderVersionService, null, this._unitOfWorkAsync, null, null, null,null);
                                    MappingObj.SavePBPDataMapList(targerTable);
                                }
                                catch (Exception ex)
                                {
                                    _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
                                    //PBPImportActivityLogServices PBPImportActivityLogServicesObj = new PBPImportActivityLogServices(this._unitOfWorkAsync);
                                    _PBPImportActivityLogServices.AddPBPImportActivityLog(pBPImportQueueID, "CreatePBPDataMap()", "PBPDataMapList Create or Save.", strSourceTableName, null, ex);
                                }
                            }
                        }
                    }
                }
            }
        }

        private DataTable ExceptionalTableHandler(DataTable sourceTable, string tableName, int pBPImportQueueID)
        {
            DataTable Target = new DataTable();
            switch (tableName)
            {
                case "PLAN_AREAS":
                    Target = PlanAreaTableHandler(sourceTable, tableName, pBPImportQueueID);
                    break;
                case "pbpregions":
                    Target = PBPRegionsTableHandler(sourceTable, tableName, pBPImportQueueID);
                    break;
                    //case "1":
                    //    break;
                    //case "1":
                    //    break;
                    //case "1":
                    //    break;
            }
            return Target;
        }

        private DataTable PlanAreaTableHandler(DataTable sourceTable, string tabelName, int pBPImportQueueID)
        {
            var DistinctQIDList = sourceTable.AsEnumerable().Select(s => s.Field<string>("PBP_A_SQUISH_ID")).Distinct();
            DataTable Target, CountiesTable;

            try
            {
                CountiesTable = new DataTable();
                Target = new DataTable();
                CountiesTable.Columns.Add("QID");
                CountiesTable.Columns.Add("PBP_A_COUNTY_NAME");
                CountiesTable.Columns.Add("PBP_A_STATE_CODE");
                CountiesTable.Columns.Add("PLAN_AREAS");
                CountiesTable.Columns.Add("PBP_A_SERVICE_AREA");

                Mapping MappingServiceObj = new Mapping(this._folderVersionService, null, this._unitOfWorkAsync, null, null, null,null);
                Target = MappingServiceObj.CreateTableWithColumn();
                foreach (var item in DistinctQIDList)
                {
                    var Counties = sourceTable.AsEnumerable()
                                   .Where(s => s.Field<string>("PBP_A_SQUISH_ID")
                                   .Equals(item))
                                   .Select(s => s.Field<string>("PBP_A_COUNTY_NAME")).Distinct();


                    var States = sourceTable.AsEnumerable()
                                   .Where(s => s.Field<string>("PBP_A_SQUISH_ID")
                                   .Equals(item))
                                   .Select(s => s.Field<string>("PBP_A_STATE_CODE")).Distinct();


                    DataRow row = CountiesTable.NewRow();
                    row["QID"] = item.ToString();
                    string COUNTY_NAME = string.Empty;
                    foreach (var county in Counties)
                    {
                        if (!string.IsNullOrEmpty(county))
                        {
                            COUNTY_NAME += county.ToString() + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(COUNTY_NAME))
                    {
                        COUNTY_NAME = COUNTY_NAME.Remove(COUNTY_NAME.Length - 1);
                    }

                    string STATE_NAME = string.Empty;
                    foreach (var state in States)
                    {
                        if (!string.IsNullOrEmpty(state))
                        {
                            STATE_NAME += state.ToString() + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(STATE_NAME))
                    {
                        STATE_NAME = STATE_NAME.Remove(STATE_NAME.Length - 1);
                    }

                    string formatter = string.Empty;
                    foreach (var st in States)
                    {
                        if (!string.IsNullOrEmpty(st))
                        {
                            var county = sourceTable.AsEnumerable()
                                       .Where(s => s.Field<string>("PBP_A_SQUISH_ID")
                                       .Equals(item)
                                       && s.Field<string>("PBP_A_STATE_CODE")
                                       .Equals(st))
                                       .Select(s => s.Field<string>("PBP_A_COUNTY_NAME")).Distinct();

                            formatter += st + ":";
                            try
                            {
                                foreach (var c in county)
                                {
                                    if (!string.IsNullOrEmpty(c))
                                    {
                                        formatter += c + ",";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            if (formatter.Length > 0)
                            {
                                formatter = formatter.Remove(formatter.Length - 1);

                                string[] arr = formatter.Split(',');
                                if (arr.Length > 1)
                                {
                                    arr[arr.Length-1] = " and "+ arr[arr.Length-1];
                                    formatter = string.Join(",",arr);
                                    formatter = formatter.Replace(", and", " and");
                                }
                                formatter += "|";
                            }
                        }
                    }
                    if (formatter.Length > 0)
                    {
                        formatter = formatter.Remove(formatter.Length - 1);
                    }
                    row["PBP_A_COUNTY_NAME"] = COUNTY_NAME;
                    row["PBP_A_STATE_CODE"] = STATE_NAME;
                    row["PLAN_AREAS"] = formatter;
                    row["PBP_A_SERVICE_AREA"] = PBP_A_SERVICE_AREAHandler(sourceTable, item);
                    CountiesTable.Rows.Add(row);
                }
                foreach (DataRow dRow in CountiesTable.Rows)
                {

                    DataColumn column = new DataColumn("QID");
                    DataRow row = Target.NewRow();
                    row["JsonData"] = MappingServiceObj.CreateJsonForRow(dRow, CountiesTable.Columns);
                    row["QID"] = dRow["QID"].ToString();
                    row["TableName"] = tabelName;
                    row["FieldName"] = string.Empty;
                    row["PBPImportQueueID"] = pBPImportQueueID;
                    Target.Rows.Add(row);
                    //targerTable.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Target = null;
            }
            return Target;
        }

        private DataTable PBPRegionsTableHandler(DataTable sourceTable, string tabelName, int pBPImportQueueID)
        {
            var DistinctQIDList = sourceTable.AsEnumerable().Select(s => s.Field<string>("PBP_A_SQUISH_ID")).Distinct();
            DataTable Target, RegionsTable;

            try
            {
                RegionsTable = new DataTable();
                Target = new DataTable();
                RegionsTable.Columns.Add("QID");
                RegionsTable.Columns.Add("PBP_A_REGION_NAME");
                RegionsTable.Columns.Add("PBP_A_REGION_CODE");
                RegionsTable.Columns.Add("PBP_A_SERVICE_AREA");
                Mapping MappingServiceObj = new Mapping(this._folderVersionService, null, this._unitOfWorkAsync, null, null, null, null);
                Target = MappingServiceObj.CreateTableWithColumn();
                foreach (var item in DistinctQIDList)
                {
                    var Regions = sourceTable.AsEnumerable()
                                   .Where(s => s.Field<string>("PBP_A_SQUISH_ID")
                                   .Equals(item))
                                   .Select(s => s.Field<string>("PBP_A_REGION_NAME")).Distinct();


                    var RegionsCode = sourceTable.AsEnumerable()
                                   .Where(s => s.Field<string>("PBP_A_SQUISH_ID")
                                   .Equals(item))
                                   .Select(s => s.Field<string>("PBP_A_REGION_CODE")).Distinct();


                    DataRow row = RegionsTable.NewRow();
                    row["QID"] = item.ToString();
                    string REGION_NAME = string.Empty;
                    foreach (var region in Regions)
                    {
                        if (!string.IsNullOrEmpty(region))
                        {
                            REGION_NAME += region.ToString() + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(REGION_NAME))
                    {
                        REGION_NAME = REGION_NAME.Remove(REGION_NAME.Length - 1);
                    }

                    string REGION_CODE = string.Empty;
                    foreach (var code in RegionsCode)
                    {
                        if (!string.IsNullOrEmpty(code))
                        {
                            REGION_CODE += code.ToString() + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(REGION_CODE))
                    {
                        REGION_CODE = REGION_CODE.Remove(REGION_CODE.Length - 1);
                    }

                    row["PBP_A_REGION_NAME"] = REGION_NAME;
                    row["PBP_A_REGION_CODE"] = REGION_CODE;
                    row["PBP_A_SERVICE_AREA"] = PBP_A_SERVICE_AREAHandler(sourceTable, item);
                    RegionsTable.Rows.Add(row);
                }
                foreach (DataRow dRow in RegionsTable.Rows)
                {

                    DataColumn column = new DataColumn("QID");
                    DataRow row = Target.NewRow();
                    row["JsonData"] = MappingServiceObj.CreateJsonForRow(dRow, RegionsTable.Columns);
                    row["QID"] = dRow["QID"].ToString();
                    row["TableName"] = tabelName;
                    row["FieldName"] = string.Empty;
                    row["PBPImportQueueID"] = pBPImportQueueID;
                    Target.Rows.Add(row);
                    //targerTable.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Target = null;
            }
            return Target;
        }

        private List<ExceptionalTable> ExceptionTableList()
        {
            List<ExceptionalTable> ExceptionalTableList = new List<ExceptionalTable>();
            ExceptionalTableList.Add(new ExceptionalTable
            {
                TableName = "PLAN_AREAS"
            });
            ExceptionalTableList.Add(new ExceptionalTable
            {
                TableName = "pbpregions"
            });
            return ExceptionalTableList;
        }

        private string PBP_A_SERVICE_AREAHandler(DataTable sourceTable, string qID)
        {
            string serviceCode = string.Empty;
            //PBP_A_SERVICE_AREA
           
                var serviceAreaList = sourceTable.AsEnumerable()
                               .Where(s => s.Field<string>("PBP_A_SQUISH_ID")
                               .Equals(qID))
                               .Select(s => s.Field<string>("PBP_A_SERVICE_AREA"));
                foreach (var srvcode in serviceAreaList)
                {
                    if (!string.IsNullOrEmpty(srvcode))
                    {
                        serviceCode += srvcode + " | ";
                    }
                }
                if (!string.IsNullOrEmpty(serviceCode))
                {
                    serviceCode = serviceCode.TrimEnd(' ');
                    serviceCode = serviceCode.TrimEnd('|');
                    serviceCode = serviceCode.TrimEnd(' ');
                //  row["PBP_A_SERVICE_AREA"] = serviceCode;
            }
            return serviceCode;
        }

        #region Private class
        private class ExceptionalTable
        {
            public string TableName { get; set; }
        }
        #endregion
    }
}
