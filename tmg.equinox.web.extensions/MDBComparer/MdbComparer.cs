using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.pbp.dataaccess;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.Models.Mapping;
using tmg.equinox.web.ExpresionBuilder;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.MDBComparer;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{
    public class MdbComparer
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }

        #region PBP Table List        
        private string _PBPMRX_PTable = "PBPMRX_P";
        private string _PBPMRX_TTable = "PBPMRX_T";

        private string _PBPC_OONTable = "PBPC_OON";
        private string _PBPC_POSTable = "PBPC_POS";

        private string _PBPD_OONTable = "PBPD_OON";
        private string _PBPD_OPTTable = "PBPD_OPT";

        private string _STEP10BTable = "STEP10B";
        private string _STEP16ATable = "STEP16A";
        private string _STEP16BTable = "STEP16B";
        private string _STEP17ATable = "STEP17A";
        private string _STEP17BTable = "STEP17B";
        private string _STEP18ATable = "STEP18A";
        private string _STEP18BTable = "STEP18B";
        private string _STEP7BTable = "STEP7B";
        private string _STEP7FTable = "STEP7F";
        #endregion

        #region VBID table List
        private string _PBPB1_2_B19A_VBID = "PBPB1_2_B19A_VBID";
        private string _PBPB1_2_B19B_VBID = "PBPB1_2_B19B_VBID";
        private string _PBPB1_B19A_VBID = "PBPB1_B19A_VBID";
        private string _PBPB1_B19B_VBID = "PBPB1_B19B_VBID";
        private string _PBPB10_VBID = "PBPB10_VBID";
        private string _PBPB13_VBID = "PBPB13_VBID";
        private string _PBPB14_VBID = "PBPB14_VBID";
        private string _PBPB16_VBID = "PBPB16_VBID";
        private string _PBPB17_VBID = "PBPB17_VBID";
        private string _PBPB18_VBID = "PBPB18_VBID";
        private string _PBPB19_2_VBID = "PBPB19_2_VBID";
        private string _PBPB19_3_VBID = "PBPB19_3_VBID";
        private string _PBPB19_4_VBID = "PBPB19_4_VBID";
        private string _PBPB19_VBID = "PBPB19_VBID";
        private string _PBPB2_B19A_VBID = "PBPB2_B19A_VBID";
        private string _PBPB2_B19B_VBID = "PBPB2_B19B_VBID";
        private string _PBPB3_VBID = "PBPB3_VBID";
        private string _PBPB4_VBID = "PBPB4_VBID";
        private string _PBPB7_VBID = "PBPB7_VBID";
        private string _PBPB9_VBID = "PBPB9_VBID";
        private string _PBPMRX_P_VBID = "PBPMRX_P_VBID";
        private string _PBPMRX_T_VBID = "PBPMRX_T_VBID";
        private string _PBPMRX_VBID = "PBPMRX_VBID";
        #endregion VBID table

        private bool _tableDifference = false;
        int _diffCount = 0;

        private IList<MigrationFieldItem> _migrationMapList;

        private string[] ignoreTableList = { "HISTORY", "PATCH_HISTORY", "USER", "VERSION", "DELETED_PLANS", "Paste Errors" };

        #region Public Method        
        public MdbComparer(IUnitOfWorkAsync unitOfWork, IFolderVersionServices folderVersionService)
        {
            _unitOfWork = unitOfWork;
            _folderVersionService = folderVersionService;
        }
        public ServiceResult ValidateMDBFiles(string importfile, string exportfile, string mDBCompareFilePath)
        {
            ServiceResult result = new ServiceResult();
            AccessDBTableService importServiceObj = new AccessDBTableService(mDBCompareFilePath + "\\" + importfile);
            DataTable importTableList = importServiceObj.GetTableList();

            AccessDBTableService exportServiceObj = new AccessDBTableService(mDBCompareFilePath + "\\" + exportfile);
            DataTable exportTableList = exportServiceObj.GetTableList();

            var findPBPTableInImportedMDB = from DataRow myRow in importTableList.Rows where (string)myRow["TABLE_NAME"] == "PBP" select myRow;
            var findPBPTableInExportedMDB = from DataRow myRow in exportTableList.Rows where (string)myRow["TABLE_NAME"] == "PBP" select myRow;

            if (findPBPTableInImportedMDB.Count() > 0 && findPBPTableInExportedMDB.Count() > 0)
            {
                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                var findVBIDTableInImportedMDB = from DataRow myRow in importTableList.Rows where (string)myRow["TABLE_NAME"] == "PBPB1_2_B19A_VBID" select myRow;
                var findVBIDTableInExportedMDB = from DataRow myRow in exportTableList.Rows where (string)myRow["TABLE_NAME"] == "PBPB1_2_B19A_VBID" select myRow;
                if (findVBIDTableInImportedMDB.Count() > 0 && findVBIDTableInExportedMDB.Count() > 0)
                {
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }

            return result;
        }

        public MemoryStream Comparer(HttpRequestBase Request, string importfile, string exportfile, string mDBCompareFilePath, string importFileName, string importedFilePath, string exportFileName, string exportedFilePath)
        {
            MemoryStream fileStream = null;

            AccessDBTableService importServiceObj = new AccessDBTableService(mDBCompareFilePath + "\\" + importfile);
            DataTable importTableList = importServiceObj.GetTableList();

            AccessDBTableService exportServiceObj = new AccessDBTableService(mDBCompareFilePath + "\\" + exportfile);
            DataTable exportTableList = exportServiceObj.GetTableList();

            var findPBPTableInImportedMDB = from DataRow myRow in importTableList.Rows where (string)myRow["TABLE_NAME"] == "PBP" select myRow;

            if (findPBPTableInImportedMDB.Count() > 0)
            {
                fileStream = PBPComparer(Request, importfile, exportfile, mDBCompareFilePath, importFileName, importedFilePath, exportFileName, exportedFilePath, importTableList, exportTableList, importServiceObj, exportServiceObj);
            }
            else
            {
                var findVBIDTableInImportedMDB = from DataRow myRow in importTableList.Rows where (string)myRow["TABLE_NAME"] == "PBPB1_2_B19A_VBID" select myRow;
                if (findVBIDTableInImportedMDB.Count() > 0)
                {
                    fileStream = VBIDComparer(Request, importfile, exportfile, mDBCompareFilePath, importFileName, importedFilePath, exportFileName, exportedFilePath, importTableList, exportTableList, importServiceObj, exportServiceObj);
                }
            }

            return fileStream;
        }

        public MemoryStream PBPComparer(HttpRequestBase Request, string importfile, string exportfile, string mDBCompareFilePath, string importFileName, string importedFilePath, string exportFileName, string exportedFilePath,
            DataTable importTableList, DataTable exportTableList, AccessDBTableService importServiceObj, AccessDBTableService exportServiceObj)
        {
            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = null;
            ExcelPackage excelPkg = new ExcelPackage();
            List<MigrationFieldItem> migrationViewMapList = new List<MigrationFieldItem>();

            _migrationMapList = this.GetBenefitMapping();

            //DataTable resultSummaryDt = new DataTable();
            //resultSummaryDt.Columns.Add("Table Name", typeof(string));
            //resultSummaryDt.Columns.Add("Difference Count", typeof(string));
            MDBComparerHelper Helper = new MDBComparerHelper(mDBCompareFilePath + "\\" + importfile, _folderVersionService, this._unitOfWork);
            List<DifferValueViewModel> ValuePair = Helper.ProcessHelper();
            ExcelWorksheet worksheet2 = excelPkg.Workbook.Worksheets.Add("Summary Report");

            //Difference of column with unique count
            DataTable diffSummryOfTable = new DataTable();
            diffSummryOfTable.Columns.Add("Table Name", typeof(string));
            diffSummryOfTable.Columns.Add("Column Name", typeof(string));
            diffSummryOfTable.Columns.Add("Difference Count", typeof(Int32));
            diffSummryOfTable.Columns.Add("Unique Count", typeof(Int32));
            diffSummryOfTable.Columns.Add("Column Range", typeof(string));

            //Missing count of import and export
            DataTable importExportMissingDT = new DataTable();
            importExportMissingDT.Columns.Add("Table Name", typeof(string));
            importExportMissingDT.Columns.Add("QID", typeof(string));
            importExportMissingDT.Columns.Add("Status", typeof(string));
            importExportMissingDT.Columns.Add("Difference Count", typeof(Int32));
            importExportMissingDT.Columns.Add("Column Range", typeof(string));

            foreach (DataRow importTable in importTableList.Rows)
            {
                string tableName = importTable.ItemArray[2].ToString();

                if (ignoreTableList.Contains(tableName)) { continue; }

                DataTable importDataTable = importServiceObj.ReadTable(tableName);

                DataTable exportDataTable = exportServiceObj.ReadTable(tableName);

                //import and export data into one datatable
                DataTable combineResultdt = new DataTable();
                combineResultdt = importDataTable.Copy();
                combineResultdt.Columns.Add("Type", typeof(string)).SetOrdinal(1);
                combineResultdt.Clear();

                migrationViewMapList = _migrationMapList.Where(a => a.TableName == tableName).ToList();

                CombineImportAndExportData(tableName, importDataTable, exportDataTable, combineResultdt);

                SetColumnHeaderToJSONField(tableName, combineResultdt, migrationViewMapList);

                ExcelWorksheet worksheet1 = excelPkg.Workbook.Worksheets.Add(tableName);
                fileStream = excelBuilder.DownLoadMDBCompareToExcel(worksheet1, combineResultdt, "", migrationViewMapList, ref _tableDifference, ref _diffCount
                    , importFileName, importedFilePath, exportFileName, exportedFilePath, ref diffSummryOfTable, ref importExportMissingDT, ValuePair);

                //if (_tableDifference)
                //{
                //    DataRow toInsert = resultSummaryDt.NewRow();
                //    toInsert[0] = tableName;
                //    toInsert[1] = _diffCount;
                //    resultSummaryDt.Rows.InsertAt(toInsert, resultSummaryDt.Rows.Count + 1);
                //}

                excelPkg.SaveAs(fileStream);
                fileStream.Position = 0;
            }

            //if (resultSummaryDt.Rows.Count == 0)
            //{
            //    DataRow toInsert = resultSummaryDt.NewRow();
            //    resultSummaryDt.Rows.InsertAt(toInsert, 1);
            //}

            fileStream = excelBuilder.DownLoadMDBCompareToExcel(worksheet2, diffSummryOfTable, "Summary", migrationViewMapList, ref _tableDifference, ref _diffCount
               , importFileName, importedFilePath, exportFileName, exportedFilePath, ref diffSummryOfTable, ref importExportMissingDT, ValuePair);

            excelPkg.SaveAs(fileStream);
            fileStream.Position = 0;

            return fileStream;
        }

        public MemoryStream VBIDComparer(HttpRequestBase Request, string importfile, string exportfile, string mDBCompareFilePath, string importFileName, string importedFilePath, string exportFileName, string exportedFilePath,
            DataTable importTableList, DataTable exportTableList, AccessDBTableService importServiceObj, AccessDBTableService exportServiceObj)
        {
            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = null;
            ExcelPackage excelPkg = new ExcelPackage();
            List<MigrationFieldItem> migrationViewMapList = new List<MigrationFieldItem>();

            _migrationMapList = this.GetBenefitMapping();

            MDBComparerHelper Helper = new MDBComparerHelper(mDBCompareFilePath + "\\" + importfile, _folderVersionService, this._unitOfWork);
            List<DifferValueViewModel> ValuePair = Helper.ProcessHelper();
            ExcelWorksheet worksheet2 = excelPkg.Workbook.Worksheets.Add("Summary Report");

            //Difference of column with unique count
            DataTable diffSummryOfTable = new DataTable();
            diffSummryOfTable.Columns.Add("Table Name", typeof(string));
            diffSummryOfTable.Columns.Add("Column Name", typeof(string));
            diffSummryOfTable.Columns.Add("Difference Count", typeof(Int32));
            diffSummryOfTable.Columns.Add("Unique Count", typeof(Int32));
            diffSummryOfTable.Columns.Add("Column Range", typeof(string));

            //Missing count of import and export
            DataTable importExportMissingDT = new DataTable();
            importExportMissingDT.Columns.Add("Table Name", typeof(string));
            importExportMissingDT.Columns.Add("QID", typeof(string));
            importExportMissingDT.Columns.Add("Status", typeof(string));
            importExportMissingDT.Columns.Add("Difference Count", typeof(Int32));
            importExportMissingDT.Columns.Add("Column Range", typeof(string));

            foreach (DataRow importTable in importTableList.Rows)
            {
                string tableName = importTable.ItemArray[2].ToString();

                if (ignoreTableList.Contains(tableName)) { continue; }

                DataTable importDataTable = importServiceObj.ReadTable(tableName);

                DataTable exportDataTable = exportServiceObj.ReadTable(tableName);

                //import and export data into one datatable
                DataTable combineResultdt = new DataTable();
                combineResultdt = importDataTable.Copy();
                combineResultdt.Columns.Add("Type", typeof(string)).SetOrdinal(1);
                combineResultdt.Clear();

                migrationViewMapList = _migrationMapList.Where(a => a.TableName == tableName).ToList();

                CombineImportAndExportVBIDData(tableName, importDataTable, exportDataTable, combineResultdt);

                SetColumnHeaderToJSONField(tableName, combineResultdt, migrationViewMapList);

                ExcelWorksheet worksheet1 = excelPkg.Workbook.Worksheets.Add(tableName);
                fileStream = excelBuilder.DownLoadMDBCompareToExcel(worksheet1, combineResultdt, "", migrationViewMapList, ref _tableDifference, ref _diffCount
                    , importFileName, importedFilePath, exportFileName, exportedFilePath, ref diffSummryOfTable, ref importExportMissingDT, ValuePair);

                excelPkg.SaveAs(fileStream);
                fileStream.Position = 0;
            }

            fileStream = excelBuilder.DownLoadMDBCompareToExcel(worksheet2, diffSummryOfTable, "Summary", migrationViewMapList, ref _tableDifference, ref _diffCount
               , importFileName, importedFilePath, exportFileName, exportedFilePath, ref diffSummryOfTable, ref importExportMissingDT, ValuePair);

            excelPkg.SaveAs(fileStream);
            fileStream.Position = 0;

            return fileStream;
        }
        #endregion

        #region Private Method        
        private IList<MigrationFieldItem> GetBenefitMapping()
        {
            IList<MigrationFieldItem> migrationMapList = (from map in this._unitOfWork.RepositoryAsync<BenefitMapping>()
                       .Get()
                       .Where(a => a.ViewType == "PBP")
                                                          select new MigrationFieldItem
                                                          {
                                                              DocumentPath = map.DocumentPath,
                                                              Title = map.Title,
                                                              FieldTitle = map.FieldTitle,
                                                              ColumnName = map.ColumnName,
                                                              TableName = map.TableName
                                                          }).ToList();

            return migrationMapList;
        }

        private void SetColumnHeaderToJSONField(string tableName, DataTable combineResultdt, List<MigrationFieldItem> migrationViewMapList)
        {
            foreach (MigrationFieldItem pbp in migrationViewMapList)
            {
                if (combineResultdt.Columns[pbp.ColumnName] != null && pbp.FieldTitle != null && !combineResultdt.Columns.Contains(pbp.FieldTitle))
                {
                    combineResultdt.Columns[pbp.ColumnName].ColumnName = pbp.FieldTitle;
                }
                else if (combineResultdt.Columns.Contains(pbp.FieldTitle))
                {
                }
            }
        }

        private void CombineImportAndExportData(string tableName, DataTable importDataTable, DataTable exportDataTable, DataTable combineResultdt)
        {
            if (tableName == this._PBPMRX_PTable)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_POST_BENEFIT_TYPE") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("MRX_TIER_POST_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("MRX_TIER_POST_TYPE_ID") == drRow.ItemArray[3].ToString());

                    InsetIntoCombineDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_POST_BENEFIT_TYPE = r.Field<string>("MRX_TIER_POST_BENEFIT_TYPE"), MRX_TIER_POST_ID = r.Field<string>("MRX_TIER_POST_ID"), MRX_TIER_POST_TYPE_ID = r.Field<string>("MRX_TIER_POST_TYPE_ID") })
                 .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_POST_BENEFIT_TYPE = r.Field<string>("MRX_TIER_POST_BENEFIT_TYPE"), MRX_TIER_POST_ID = r.Field<string>("MRX_TIER_POST_ID"), MRX_TIER_POST_TYPE_ID = r.Field<string>("MRX_TIER_POST_TYPE_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), MRX_TIER_POST_BENEFIT_TYPE = r.Field<string>("MRX_TIER_POST_BENEFIT_TYPE"), MRX_TIER_POST_ID = r.Field<string>("MRX_TIER_POST_ID"), MRX_TIER_POST_TYPE_ID = r.Field<string>("MRX_TIER_POST_TYPE_ID") }
                                   equals new { id.QID, id.MRX_TIER_POST_BENEFIT_TYPE, id.MRX_TIER_POST_ID, id.MRX_TIER_POST_TYPE_ID }
                                   select r).CopyToDataTable();

                }

                NotinImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);

            }
            else if (tableName == this._PBPMRX_TTable)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_BENEFIT_TYPE") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("MRX_TIER_TYPE_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("MRX_TIER_ID") == drRow.ItemArray[3].ToString());

                    InsetIntoCombineDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_BENEFIT_TYPE = r.Field<string>("MRX_TIER_BENEFIT_TYPE"), MRX_TIER_TYPE_ID = r.Field<string>("MRX_TIER_TYPE_ID"), MRX_TIER_ID = r.Field<string>("MRX_TIER_ID") })
                 .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_BENEFIT_TYPE = r.Field<string>("MRX_TIER_BENEFIT_TYPE"), MRX_TIER_TYPE_ID = r.Field<string>("MRX_TIER_TYPE_ID"), MRX_TIER_ID = r.Field<string>("MRX_TIER_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), MRX_TIER_BENEFIT_TYPE = r.Field<string>("MRX_TIER_BENEFIT_TYPE"), MRX_TIER_TYPE_ID = r.Field<string>("MRX_TIER_TYPE_ID"), MRX_TIER_ID = r.Field<string>("MRX_TIER_ID") }
                                   equals new { id.QID, id.MRX_TIER_BENEFIT_TYPE, id.MRX_TIER_TYPE_ID, id.MRX_TIER_ID }
                                   select r).CopyToDataTable();

                }

                NotinImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);

            }
            else if (tableName == this._PBPC_OONTable)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_C_OON_OUTPT_GROUP_NUM_ID") == drRow.ItemArray[1].ToString());

                    InsetIntoCombineDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_C_OON_OUTPT_GROUP_NUM_ID = r.Field<string>("PBP_C_OON_OUTPT_GROUP_NUM_ID") })
                   .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_C_OON_OUTPT_GROUP_NUM_ID = r.Field<string>("PBP_C_OON_OUTPT_GROUP_NUM_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), PBP_C_OON_OUTPT_GROUP_NUM_ID = r.Field<string>("PBP_C_OON_OUTPT_GROUP_NUM_ID") }
                                   equals new { id.QID, id.PBP_C_OON_OUTPT_GROUP_NUM_ID }
                                   select r).CopyToDataTable();

                }

                NotinImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);
            }
            else if (tableName == this._PBPC_POSTable)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_C_POS_OUTPT_GROUP_NUM_ID") == drRow.ItemArray[1].ToString());

                    InsetIntoCombineDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_C_POS_OUTPT_GROUP_NUM_ID = r.Field<string>("PBP_C_POS_OUTPT_GROUP_NUM_ID") })
                    .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_C_POS_OUTPT_GROUP_NUM_ID = r.Field<string>("PBP_C_POS_OUTPT_GROUP_NUM_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), PBP_C_POS_OUTPT_GROUP_NUM_ID = r.Field<string>("PBP_C_POS_OUTPT_GROUP_NUM_ID") }
                                   equals new { id.QID, id.PBP_C_POS_OUTPT_GROUP_NUM_ID }
                                   select r).CopyToDataTable();

                }

                NotinImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);
            }
            else if (tableName == this._PBPD_OONTable)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_D_OPT_OON_IDENTIFIER") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("PBP_D_OPT_OON_TYPE_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("PBP_D_OPT_OON_CAT_ID") == drRow.ItemArray[3].ToString());

                    InsetIntoCombineDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_D_OPT_OON_IDENTIFIER = r.Field<string>("PBP_D_OPT_OON_IDENTIFIER"), PBP_D_OPT_OON_TYPE_ID = r.Field<string>("PBP_D_OPT_OON_TYPE_ID"), PBP_D_OPT_OON_CAT_ID = r.Field<string>("PBP_D_OPT_OON_CAT_ID") })
                .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_D_OPT_OON_IDENTIFIER = r.Field<string>("PBP_D_OPT_OON_IDENTIFIER"), PBP_D_OPT_OON_TYPE_ID = r.Field<string>("PBP_D_OPT_OON_TYPE_ID"), PBP_D_OPT_OON_CAT_ID = r.Field<string>("PBP_D_OPT_OON_CAT_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), PBP_D_OPT_OON_IDENTIFIER = r.Field<string>("PBP_D_OPT_OON_IDENTIFIER"), PBP_D_OPT_OON_TYPE_ID = r.Field<string>("PBP_D_OPT_OON_TYPE_ID"), PBP_D_OPT_OON_CAT_ID = r.Field<string>("PBP_D_OPT_OON_CAT_ID") }
                                   equals new { id.QID, id.PBP_D_OPT_OON_IDENTIFIER, id.PBP_D_OPT_OON_TYPE_ID, id.PBP_D_OPT_OON_CAT_ID }
                                   select r).CopyToDataTable();

                }

                NotinImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);
            }
            else if (tableName == this._PBPD_OPTTable)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_D_OPT_IDENTIFIER") == drRow.ItemArray[1].ToString());

                    InsetIntoCombineDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_D_OPT_IDENTIFIER = r.Field<string>("PBP_D_OPT_IDENTIFIER") })
                .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_D_OPT_IDENTIFIER = r.Field<string>("PBP_D_OPT_IDENTIFIER") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), PBP_D_OPT_IDENTIFIER = r.Field<string>("PBP_D_OPT_IDENTIFIER") }
                                   equals new { id.QID, id.PBP_D_OPT_IDENTIFIER }
                                   select r).CopyToDataTable();

                }

                NotinImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);
            }
            else if (tableName == this._STEP10BTable || tableName == this._STEP16ATable || tableName == this._STEP16BTable
                || tableName == this._STEP17ATable || tableName == this._STEP17BTable || tableName == this._STEP18ATable || tableName == this._STEP7BTable || tableName == this._STEP18BTable
                || tableName == this._STEP7FTable)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_D_OPT_IDENTIFIER") == drRow.ItemArray[1].ToString());

                    InsetIntoCombineDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_D_OPT_IDENTIFIER = r.Field<string>("PBP_D_OPT_IDENTIFIER") })
                .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_D_OPT_IDENTIFIER = r.Field<string>("PBP_D_OPT_IDENTIFIER") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), PBP_D_OPT_IDENTIFIER = r.Field<string>("PBP_D_OPT_IDENTIFIER") } equals new { id.QID, id.PBP_D_OPT_IDENTIFIER }
                                   select r).CopyToDataTable();

                }

                NotinImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);
            }
            else
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable().SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString());
                    InsetIntoCombineDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID") })
                .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID") } equals new { id.QID }
                                   select r).CopyToDataTable();

                }

                NotinImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);
            }

            if (importDataTable.Rows.Count == 0 && exportDataTable.Rows.Count == 0)
            {
                DataRow toInsert = combineResultdt.NewRow();
                combineResultdt.Rows.InsertAt(toInsert, 1);
            }
        }

        private void InsetIntoCombineDataTable(DataTable combineResultdt, DataRow importRow, DataRow exportRow)
        {
            combineResultdt.ImportRow(importRow);
            DataRow latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
            latestRow.SetField("Type", "Import");

            if (exportRow != null)
            {
                combineResultdt.ImportRow(exportRow);
                latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
                latestRow.SetField("Type", "Export");
            }
            else
            {
                DataRow toInsert = combineResultdt.NewRow();
                toInsert[0] = importRow.ItemArray[0].ToString();

                combineResultdt.Rows.InsertAt(toInsert, combineResultdt.Rows.Count + 1);
                latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
                latestRow.SetField("Type", "Export Missing");
            }
        }

        private void NotinImport(string tableName, DataTable importDataTable, DataTable exportDataTable, DataTable combineResultdt, DataTable notInImport)
        {
            foreach (DataRow drRow in notInImport.Rows)
            {
                DataRow toInsert = combineResultdt.NewRow();
                toInsert[0] = drRow.ItemArray[0].ToString();

                combineResultdt.Rows.InsertAt(toInsert, combineResultdt.Rows.Count + 1);
                DataRow latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
                latestRow.SetField("Type", "Import Missing");

                DataRow exportRow;
                if (tableName == this._PBPMRX_PTable)
                {
                    exportRow = exportDataTable.AsEnumerable()
                       .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_POST_BENEFIT_TYPE") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("MRX_TIER_POST_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("MRX_TIER_POST_TYPE_ID") == drRow.ItemArray[3].ToString());
                }
                else if (tableName == this._PBPMRX_TTable)
                {
                    exportRow = exportDataTable.AsEnumerable()
                    .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_BENEFIT_TYPE") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("MRX_TIER_TYPE_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("MRX_TIER_ID") == drRow.ItemArray[3].ToString());
                }
                else if (tableName == this._PBPC_OONTable)
                {
                    exportRow = exportDataTable.AsEnumerable()
                       .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                       && r.Field<string>("PBP_C_OON_OUTPT_GROUP_NUM_ID") == drRow.ItemArray[1].ToString());
                }
                else if (tableName == this._PBPC_POSTable)
                {
                    exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_C_POS_OUTPT_GROUP_NUM_ID") == drRow.ItemArray[1].ToString());
                }
                else if (tableName == this._PBPD_OONTable)
                {
                    exportRow = exportDataTable.AsEnumerable()
                         .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_D_OPT_OON_IDENTIFIER") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("PBP_D_OPT_OON_TYPE_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("PBP_D_OPT_OON_CAT_ID") == drRow.ItemArray[3].ToString());
                }
                else if (tableName == this._PBPD_OPTTable)
                {
                    exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_D_OPT_IDENTIFIER") == drRow.ItemArray[1].ToString());
                }
                else if (tableName == this._STEP10BTable || tableName == this._STEP16ATable || tableName == this._STEP16BTable
                 || tableName == this._STEP17ATable || tableName == this._STEP17BTable || tableName == this._STEP18ATable || tableName == this._STEP7BTable || tableName == this._STEP18BTable
                 || tableName == this._STEP7FTable)
                {
                    exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_D_OPT_IDENTIFIER") == drRow.ItemArray[1].ToString());
                }
                else
                {
                    exportRow = exportDataTable.AsEnumerable().SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString());
                }

                combineResultdt.ImportRow(exportRow);
                latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
                latestRow.SetField("Type", "Export");
            }
        }

        private void CombineImportAndExportVBIDData(string tableName, DataTable importDataTable, DataTable exportDataTable, DataTable combineResultdt)
        {
            if (tableName == this._PBPMRX_P_VBID)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_GROUP_ID") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("MRX_TIER_POST_TYPE_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("MRX_TIER_POST_ID") == drRow.ItemArray[3].ToString());

                    InsetIntoCombineVBIDDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID"), MRX_TIER_POST_TYPE_ID = r.Field<string>("MRX_TIER_POST_TYPE_ID"), MRX_TIER_POST_ID = r.Field<string>("MRX_TIER_POST_ID") })
                 .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID"), MRX_TIER_POST_TYPE_ID = r.Field<string>("MRX_TIER_POST_TYPE_ID"), MRX_TIER_POST_ID = r.Field<string>("MRX_TIER_POST_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID"), MRX_TIER_POST_TYPE_ID = r.Field<string>("MRX_TIER_POST_TYPE_ID"), MRX_TIER_POST_ID = r.Field<string>("MRX_TIER_POST_ID") }
                                   equals new { id.QID, id.MRX_TIER_GROUP_ID, id.MRX_TIER_POST_TYPE_ID, id.MRX_TIER_POST_ID }
                                   select r).CopyToDataTable();

                }

                NotinVBIDImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);

            }
            else if (tableName == this._PBPMRX_T_VBID)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_GROUP_ID") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("MRX_TIER_TYPE_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("MRX_TIER_ID") == drRow.ItemArray[3].ToString());

                    InsetIntoCombineVBIDDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID"), MRX_TIER_TYPE_ID = r.Field<string>("MRX_TIER_TYPE_ID"), MRX_TIER_ID = r.Field<string>("MRX_TIER_ID") })
                   .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID"), MRX_TIER_TYPE_ID = r.Field<string>("MRX_TIER_TYPE_ID"), MRX_TIER_ID = r.Field<string>("MRX_TIER_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID"), MRX_TIER_TYPE_ID = r.Field<string>("MRX_TIER_TYPE_ID"), MRX_TIER_ID = r.Field<string>("MRX_TIER_ID") }
                                   equals new { id.QID, id.MRX_TIER_GROUP_ID, id.MRX_TIER_TYPE_ID, id.MRX_TIER_ID }
                                   select r).CopyToDataTable();

                }

                NotinVBIDImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);
            }
            else if (tableName == this._PBPMRX_VBID)
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_GROUP_ID") == drRow.ItemArray[1].ToString());

                    InsetIntoCombineVBIDDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID") })
                    .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), MRX_TIER_GROUP_ID = r.Field<string>("MRX_TIER_GROUP_ID") }
                                   equals new { id.QID, id.MRX_TIER_GROUP_ID }
                                   select r).CopyToDataTable();

                }

                NotinVBIDImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);
            }
            else
            {
                foreach (DataRow drRow in importDataTable.Rows)
                {
                    DataRow exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_VBID_GROUP_ID") == drRow.ItemArray[1].ToString());

                    InsetIntoCombineVBIDDataTable(combineResultdt, drRow, exportRow);
                }

                var notInTarget = exportDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_VBID_GROUP_ID = r.Field<string>("PBP_VBID_GROUP_ID") })
                 .Except(importDataTable.AsEnumerable().Select(r => new { QID = r.Field<string>("QID"), PBP_VBID_GROUP_ID = r.Field<string>("PBP_VBID_GROUP_ID") }));

                DataTable notInImport = new DataTable();

                if (notInTarget.Any())
                {
                    notInImport = (from r in exportDataTable.AsEnumerable()
                                   join id in notInTarget
                                   on new { QID = r.Field<string>("QID"), PBP_VBID_GROUP_ID = r.Field<string>("PBP_VBID_GROUP_ID") }
                                   equals new { id.QID, id.PBP_VBID_GROUP_ID }
                                   select r).CopyToDataTable();

                }

                NotinVBIDImport(tableName, importDataTable, exportDataTable, combineResultdt, notInImport);

            }

            if (importDataTable.Rows.Count == 0 && exportDataTable.Rows.Count == 0)
            {
                DataRow toInsert = combineResultdt.NewRow();
                combineResultdt.Rows.InsertAt(toInsert, 1);
            }
        }

        private void InsetIntoCombineVBIDDataTable(DataTable combineResultdt, DataRow importRow, DataRow exportRow)
        {
            combineResultdt.ImportRow(importRow);
            DataRow latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
            latestRow.SetField("Type", "Import");

            if (exportRow != null)
            {
                combineResultdt.ImportRow(exportRow);
                latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
                latestRow.SetField("Type", "Export");
            }
            else
            {
                DataRow toInsert = combineResultdt.NewRow();
                toInsert[0] = importRow.ItemArray[0].ToString();

                combineResultdt.Rows.InsertAt(toInsert, combineResultdt.Rows.Count + 1);
                latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
                latestRow.SetField("Type", "Export Missing");
            }
        }

        private void NotinVBIDImport(string tableName, DataTable importDataTable, DataTable exportDataTable, DataTable combineResultdt, DataTable notInImport)
        {
            foreach (DataRow drRow in notInImport.Rows)
            {
                DataRow toInsert = combineResultdt.NewRow();
                toInsert[0] = drRow.ItemArray[0].ToString();

                combineResultdt.Rows.InsertAt(toInsert, combineResultdt.Rows.Count + 1);
                DataRow latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
                latestRow.SetField("Type", "Import Missing");

                DataRow exportRow;
                if (tableName == this._PBPMRX_P_VBID)
                {
                    exportRow = exportDataTable.AsEnumerable()
                    .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_GROUP_ID") == drRow.ItemArray[1].ToString()
                        && r.Field<string>("MRX_TIER_POST_TYPE_ID") == drRow.ItemArray[2].ToString()
                        && r.Field<string>("MRX_TIER_POST_ID") == drRow.ItemArray[3].ToString());
                }
                else if (tableName == this._PBPMRX_T_VBID)
                {
                    exportRow = exportDataTable.AsEnumerable()
                       .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                       && r.Field<string>("MRX_TIER_GROUP_ID") == drRow.ItemArray[1].ToString()
                       && r.Field<string>("MRX_TIER_TYPE_ID") == drRow.ItemArray[2].ToString()
                       && r.Field<string>("MRX_TIER_ID") == drRow.ItemArray[3].ToString());
                }
                else if (tableName == this._PBPMRX_VBID)
                {
                    exportRow = exportDataTable.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("MRX_TIER_GROUP_ID") == drRow.ItemArray[1].ToString());
                }
                else
                {
                    exportRow = exportDataTable.AsEnumerable()
                       .SingleOrDefault(r => r.Field<string>("QID") == drRow.ItemArray[0].ToString()
                        && r.Field<string>("PBP_VBID_GROUP_ID") == drRow.ItemArray[1].ToString());
                }

                combineResultdt.ImportRow(exportRow);
                latestRow = combineResultdt.Rows[combineResultdt.Rows.Count - 1];
                latestRow.SetField("Type", "Export");
            }
        }
        #endregion
    }
}
