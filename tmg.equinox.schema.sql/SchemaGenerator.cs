using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Transactions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.config;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.schema.Base;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.sql
{
    public class SchemaGenerator
    {
        private static readonly ILog _logger = LogProvider.For<SchemaGenerator>();
        ISchemaRepository _schemaRepostory;
        Server _server;
        Database _database;
        string _serverInstanceName;
        string _userName;
        string _password;
        private IMDMSyncDataService _mDMSyncDataService;

        public SchemaGenerator(ISchemaRepository schemaRepostory, IMDMSyncDataService mDMSyncDataService)
        {
            this._schemaRepostory = schemaRepostory;
            _mDMSyncDataService = mDMSyncDataService;
            if (!string.IsNullOrEmpty(Config.GetServerInstanceName()))
                _serverInstanceName = Config.GetServerInstanceName();
            else
                throw new Exception("Server Instance Name is Required! Please provide the Server Instance Name.");

            if (!string.IsNullOrEmpty(Config.GetUserName()))
                _userName = Config.GetUserName();
            else
                throw new Exception("User Name is Required! Please provide the User Name.");

            if (!string.IsNullOrEmpty(Config.GetPassword()))
                _password = Config.GetPassword();
            else
                throw new Exception("Password is Required! Please provide the Password.");
        }
        public bool Execute(List<JsonDesign> jsonDesigns)
        {
            int formDesignId = 0, formDesignVersionId = 0;
            try
            {
                //
                _server = new Server(SqlServerConnect());
                GetDatabaseInstance(Config.GetReportingDatabaseName());

                //Jamir :Used _server transaction to maintain the newly created database and tables. (ReportingCenter Database)
                //  _server.ConnectionContext.BeginTransaction();
                // _server.ConnectionContext.ConnectTimeout = 999999999;

                _logger.Debug("ReportingCenter database transaction started.");

                //Jamir :Used TransactionScope object to maintain the transacttion for metadata of newly created database and tables. (WellCare Database)
                // using (TransactionScope trans = new TransactionScope(TransactionScopeOption.Required, option))
                //   {
                _logger.Debug("WellCare database transaction started.");

                foreach (JsonDesign jsonDesign in jsonDesigns)
                {
                    string schemaName = _schemaRepostory.isSchemaExistForDesignID(jsonDesign);

                    if (string.IsNullOrEmpty(schemaName) && _database.Schemas.Contains(jsonDesign.TableSchemaName))
                    {
                        jsonDesign.TableSchemaName = jsonDesign.TableSchemaName + jsonDesign.JsonDesignId;
                    }
                    else if (!string.IsNullOrEmpty(schemaName))
                    {
                        jsonDesign.TableSchemaName = schemaName;
                    }

                    formDesignId = jsonDesign.JsonDesignId;
                    formDesignVersionId = jsonDesign.JsonDesignVersionId;
                    if (jsonDesign.JsonDesignData != "")
                    {
                        IJSchema jsonSchema = new JSonSchema(jsonDesign);
                        IPrepareSchema prepareSQLSchema = new PrepareSQLSchema(jsonSchema, jsonDesign);

                        IGenerateSchema generateSchema = new GenerateSchema(_schemaRepostory, jsonDesign, _server, _database);

                        var schemeManager = new SchemeManager(prepareSQLSchema, generateSchema);
                        /*  var option = new TransactionOptions();
                          option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                          option.Timeout = TimeSpan.FromMinutes(60);
                          using (TransactionScope trans = new TransactionScope(TransactionScopeOption.Required, option))
                          {*/

                        try
                        {
                            schemeManager.Execute();
                        }
                        catch (Exception ex)
                        {

                            //_mDMSyncDataService.AddLogForMDMProcess(new MDMLog() { ForminstanceID = 0, FormDesignID = formDesignId, FormDesignVersionID = formDesignVersionId, AddedDate = DateTime.Now, Error = ex.Message, ErrorDescription = ex.StackTrace.ToString() });
                            if (jsonDesigns.Count == 1)
                            {
                                throw ex;
                            }
                        }
                        //   trans.Complete();
                        //}
                    }
                }
                //     trans.Complete();
                _logger.Debug("WellCare database transaction committed.");
                //  }
                //_server.ConnectionContext.CommitTransaction();
                _logger.Debug("ReportingCenter database transaction committed.");
            }
            catch (Exception ex)
            {
                _mDMSyncDataService.AddLogForMDMProcess(new MDMLog() { ForminstanceID = 0, FormDesignID = formDesignId, FormDesignVersionID = formDesignVersionId, AddedDate = DateTime.Now, Error = ex.Message, ErrorDescription = ex.StackTrace.ToString() });
                _logger.ErrorException("Exceprion Occures while generating ReportingCenter database.", ex);
                throw ex;
            }
            finally
            {
                //    _server.ConnectionContext.RollBackTransaction();
                _logger.Debug("ReportingCenter database transaction roll backed.");
                if (_server.ConnectionContext.IsOpen) _server.ConnectionContext.Disconnect();
                _logger.Debug("ReportingCenter database connection closed.");
            }
            return true;
        }
        private ServerConnection SqlServerConnect()
        {
            ServerConnection _serverConnection;
            try
            {
                _serverConnection = new ServerConnection();
                _serverConnection.ServerInstance = _serverInstanceName;
                _serverConnection.LoginSecure = false;
                _serverConnection.Login = _userName;
                _serverConnection.Password = _password;
                _serverConnection.ConnectTimeout = 999999999;
                _serverConnection.Connect();
                _logger.Debug("Connected to [" + _serverInstanceName + "] SQL server.");
                return _serverConnection;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Could not able to connect to [" + _serverInstanceName + "] server.", ex);
                throw ex;
            }
        }
        private void GetDatabaseInstance(string databaseName)
        {
            try
            {
                if (_server.Databases.Contains(databaseName))
                {
                    _database = _server.Databases[databaseName];
                    _logger.Debug("Get SQL database [" + databaseName + "] instance.");
                }
                else
                {
                    _database = new Database(_server, databaseName);
                    _database.Create();
                    _logger.Debug("Created SQL database [" + databaseName + "].");
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Could not able to create dataabase [" + databaseName + "].", ex);
                throw ex;
            }
        }

    }
}
