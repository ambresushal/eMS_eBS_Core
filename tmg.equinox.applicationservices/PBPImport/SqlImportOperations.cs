using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.PBPImport
{
    public class SqlImportOperations : ISQLImportOperations
    {
        #region Private Members
        private string _connectionString;
        IUnitOfWorkAsync _unitOfWorkAsync = null;

        #endregion

        #region Constructor
        public SqlImportOperations(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #endregion

        #region Public Method
        public bool CreateTableStructure(string strCreateTableScript)
        {
            bool isSuccessful = false;
            if (!string.IsNullOrEmpty(strCreateTableScript))
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    try
                    {
                        con.Open();

                        using (SqlCommand command = new SqlCommand(strCreateTableScript, con))
                            command.ExecuteNonQuery();
                        isSuccessful = true;
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                }
            }
            return isSuccessful;
        }

        public bool ImportDataToSqlServer(DataTable sourceDataTable)
        {
            bool isSuccessfull = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name
                    sqlBulkCopy.DestinationTableName = "PBP.PBPDATAMAP";
                    List<string> colNameTemp = new List<string>();
                    foreach (DataColumn dcol in sourceDataTable.Columns)
                        sqlBulkCopy.ColumnMappings.Add(dcol.ColumnName, dcol.ColumnName);

                    con.Open();
                    sqlBulkCopy.WriteToServer(sourceDataTable);
                    con.Close();
                }
            }
            return isSuccessfull;
        }
        public bool ImportDataToSqlServer(DataTable sourceDataTable, string destinationTableName, List<PBPImportTableColumnsViewModel> PBPImportTableColumnsViewModel)
        {
            bool isSuccessfull = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name
                    sqlBulkCopy.DestinationTableName = destinationTableName;
                    List<string> colNameTemp = new List<string>();
                    foreach (DataColumn dcol in sourceDataTable.Columns)
                    {
                        string destinationColName = dcol.ColumnName;
                        if (PBPImportTableColumnsViewModel != null)
                        {
                            PBPImportTableColumnsViewModel destiObject = PBPImportTableColumnsViewModel.FirstOrDefault(x => x.PBPImportTableColumnName.Equals(dcol.ColumnName) || x.PBPImportTableColumnName.Contains(dcol.ColumnName));
                            if (destiObject != null && destiObject.PBPImportTableColumnName != null)
                            {
                                colNameTemp.Add(destiObject.PBPImportTableColumnName);
                                destinationColName = destiObject.PBPImportTableColumnName;
                            }
                            else
                            {
                                destinationColName = destinationColName;
                            }

                        }
                        sqlBulkCopy.ColumnMappings.Add(dcol.ColumnName, dcol.ColumnName);
                    }
                    con.Open();
                    sqlBulkCopy.WriteToServer(sourceDataTable);
                    con.Close();
                }
            }
            return isSuccessfull;
        }

        public void InitializeVariables(string connectingString)
        {
            _connectionString = connectingString;
        }

        #endregion

        #region Private Method
        #endregion
    }
}
