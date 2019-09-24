using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.CCRIntegration;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.config;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models.CCRTranslator;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class CCRTranslatorService : ICCRTranslatorService
    {
        private IRptUnitOfWorkAsync _unitOfWork { get; set; }

        public CCRTranslatorService(Func<string, IRptUnitOfWorkAsync> unitOfWork)
        {
            _unitOfWork = unitOfWork("UI");
        }

        public ServiceResult AddProducttoTranslate(FormInstanceViewModel formInstance, string currentuserName)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                string currentDataHash = string.Empty;

                ProcessGovernance newProcessGovernance = new ProcessGovernance();
                newProcessGovernance.AddedBy = currentuserName;
                newProcessGovernance.AddedDate = DateTime.Now;
                newProcessGovernance.Processor1Up = 1;
                newProcessGovernance.ProcessStatus1Up = (int)CCRTranslatorStatus.Queued;
                newProcessGovernance.ProcessType = 1;
                newProcessGovernance.RunDate = DateTime.Now;
                newProcessGovernance.ErrorDescription = null;
                newProcessGovernance.HasError = false;
                newProcessGovernance.IsActive = true;
                this._unitOfWork.RepositoryAsync<ProcessGovernance>().Insert(newProcessGovernance);
                this._unitOfWork.Save();

                //Queue for translation
                CCRTranslatorQueue translationQueue = new CCRTranslatorQueue();
                translationQueue.ProcessGovernance1Up = newProcessGovernance.ProcessGovernance1up;
                translationQueue.FormInstanceID = formInstance.FormInstanceID;
                translationQueue.FormInstanceName = formInstance.FormInstanceName;
                translationQueue.FolderID = formInstance.FolderID;
                translationQueue.FolderName = formInstance.FolderName;
                translationQueue.FolderVersionID = formInstance.FolderVersionID;
                translationQueue.FolderVersionNumber = formInstance.FolderVersionNumber;
                translationQueue.EffectiveDate = formInstance.EffectiveDate;
                translationQueue.AccountID = formInstance.AccountID;
                translationQueue.AccountName = formInstance.AccountName;
                translationQueue.ConsortiumName = formInstance.ConsortiumName;
                translationQueue.CreatedBy = currentuserName;
                translationQueue.CreatedDate = DateTime.Now;
                translationQueue.ProcessStatus1Up = (int)CCRTranslatorStatus.Queued;
                translationQueue.HasError = false;
                translationQueue.IsActive = true;
                translationQueue.IsRetro = false;
                translationQueue.IsShell = false;
                translationQueue.ProductName = formInstance.ProductName;
                translationQueue.ProductID = formInstance.ProductID;
                translationQueue.ProductType = formInstance.ProductType;
                this._unitOfWork.RepositoryAsync<CCRTranslatorQueue>().Insert(translationQueue);
                this._unitOfWork.Save();


                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return result;
        }

        public string GetTableDetails(string id)
        {
            string data = "";
            using (SqlConnection connection = new SqlConnection(Config.GetReportingCenterConnectionString()))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("GetTableDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (id == "defualt")
                    {
                        id = "0";
                    }

                    SqlParameter ProductId = new SqlParameter("@ProductId", id);
                    SqlParameter parmOUT = new SqlParameter("@TableDetails", SqlDbType.NVarChar, Int32.MaxValue);
                    parmOUT.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(ProductId);
                    cmd.Parameters.Add(parmOUT);
                    try
                    {
                        int i = cmd.ExecuteNonQuery();
                        string returnVALUE = (string)cmd.Parameters["@TableDetails"].Value;
                        ICompressionBase compressionObj = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                        data = compressionObj.Decompress(returnVALUE).ToString();
                        data = this.XmlToJSON(data, true);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            return data;
        }

        public string GetReportDetails(string id)
        {
            string data = "";
            using (SqlConnection connection = new SqlConnection(Config.GetReportingCenterConnectionString()))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("GetReportDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (id == "defualt")
                    {
                        id = "0";
                    }
                    SqlParameter ProductId = new SqlParameter("@ProductId", id);
                    SqlParameter parmOUT = new SqlParameter("@ReportDetails", SqlDbType.NVarChar, Int32.MaxValue);
                    parmOUT.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(ProductId);
                    cmd.Parameters.Add(parmOUT);
                    try
                    {
                        int i = cmd.ExecuteNonQuery();
                        string returnVALUE = (string)cmd.Parameters["@ReportDetails"].Value;
                        ICompressionBase compressionObj = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
                        data = compressionObj.Decompress(returnVALUE).ToString();
                        data = XmlToJSON(data, false);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            return data;
        }

        public List<TranslationQueueViewModel> GetTranslatorQueue()
        {
            List<TranslationQueueViewModel> QueueList = new List<TranslationQueueViewModel>();

            QueueList = (from pg in this._unitOfWork.Repository<CCRTranslatorQueue>().Get()
                         select new TranslationQueueViewModel
                         {
                             ProcessGovernance1Up = pg.ProcessGovernance1Up,
                             ProductID = pg.ProductID,
                             ProductType = pg.ProductType,
                             ProductName = pg.ProductName,
                             AccountName = pg.AccountName,
                             FolderName = pg.FolderName,
                             EffectiveDate = pg.EffectiveDate,
                             ProcessStatus1Up = pg.ProcessStatus1Up,
                             StartTime = pg.StartTime.HasValue == false ? null : pg.StartTime,
                             EndTime = pg.EndTime.HasValue == false ? null : pg.EndTime,
                             ProcessTime = ""
                         }).OrderByDescending(s => s.ProcessGovernance1Up).ToList();

            // QueueList.Where(r => r.Status == "Errored" || r.Status == "Complete").ToList().ForEach(row => { row.ProcessTime = this.GetProcessTime(row.StartDate, row.EndDate); });

            QueueList.ForEach(row => { row.ProcessTime = this.GetProcessTime(row.StartTime, row.EndTime); });

            return QueueList;
        }

        public List<CCRTranslatorQueue> GetTranslatorProduct()
        {
            List<CCRTranslatorQueue> QueueList = new List<CCRTranslatorQueue>();


            QueueList = this._unitOfWork.RepositoryAsync<CCRTranslatorQueue>()
                                               .Query()
                                               .Get()
                                               .Where(a => a.ProcessStatus1Up == 4)
                                               .OrderByDescending(s => s.EndTime).ToList();

            return QueueList;
        }
        
        public DataSet GetTranslationLog(int processGovernance1up, int roleID)
        {
            using (SqlConnection con = new SqlConnection(Config.GetReportingCenterConnectionString()))
            {
                SqlCommand com = new SqlCommand();
                SqlDataAdapter dt = new SqlDataAdapter();
                DataSet ds = new DataSet();
                com = new SqlCommand("[Log].[ShowActivityLog]", con);
                com.Parameters.AddWithValue("@TenantID", 1);
                com.Parameters.AddWithValue("@ProcessGovernance1up", processGovernance1up);
                com.CommandType = CommandType.StoredProcedure;
                dt = new SqlDataAdapter(com);
                dt.Fill(ds);

                //SqlDataAdapter dt1 = new SqlDataAdapter();
                //com = new SqlCommand("[Log].[uspGetTranslationLog]", con);                
                //com.Parameters.AddWithValue("@ProcessGovernance1up", processGovernance1up);
                //com.CommandType = CommandType.StoredProcedure;
                //dt1 = new SqlDataAdapter(com);
                //dt1.Fill(ds);
                con.Close();
                return ds;
            }        
        }

        private string XmlToJSON(string data, bool columnRequired)
        {
            List<JToken> JList = new List<JToken>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("row");

            foreach (XmlElement node in nodes)
            {
                JObject obj = JObject.Parse("{'TableName':'','ColumnJSON':'','TableDataJSON':''}");
                foreach (XmlElement cnode in node.ChildNodes)
                {
                    if (cnode.Name == "TableDataJSON")
                    {
                        if (cnode.InnerText == "[]")
                        {
                            obj[cnode.Name] = "[]";
                        }
                        else
                        {
                            ChildXmlNodeToJSON(cnode, obj);
                        }
                    }
                    else if (columnRequired == true && cnode.Name == "ColumnJSON")
                    {
                        if (cnode.InnerText == "[]")
                        {
                            obj[cnode.Name] = "[]";
                        }
                        else
                        {
                            ChildXmlNodeToJSON(cnode, obj);
                        }
                    }
                    else
                    {
                        obj[cnode.Name] = cnode.FirstChild.Value;
                    }
                }
                JList.Add(obj);

            }
            return data = JsonConvert.SerializeObject(JList);
        }

        private void ChildXmlNodeToJSON(XmlNode cnode, JObject obj)
        {
            try
            {
                XmlDocument childDoc = new XmlDocument();
                childDoc.LoadXml(cnode.InnerText);
                XmlElement childRoot = childDoc.DocumentElement;
                XmlNodeList childNodeList = childRoot.SelectNodes("row");
                List<JToken> childList = new List<JToken>();
                foreach (XmlElement childNode in childNodeList)
                {
                    JObject cObj = new JObject();
                    foreach (XmlElement tnode in childNode.ChildNodes)
                    {
                        if (tnode.FirstChild != null && tnode.FirstChild.Value != null)
                        {
                            cObj[tnode.Name] = tnode.FirstChild.Value;
                        }
                        else
                        {
                            cObj[tnode.Name] = "";
                        }
                    }
                    childList.Add(cObj);
                }
                obj[cnode.Name] = JsonConvert.SerializeObject(childList);
            }
            catch (Exception e)
            {

            }
        }

        private string GetProcessTime(DateTime? startDate, DateTime? endDate)
        {
            string result = "00:00:00 hours";
            if (startDate != null && endDate != null)
            {
                TimeSpan diff = Convert.ToDateTime(endDate) - Convert.ToDateTime(startDate);
                result = String.Format("{0}:{1}:{2} hours", diff.Hours, diff.Minutes, diff.Seconds);
            }
            else if (startDate != null && endDate == null)
            {
                TimeSpan diff = Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(startDate);
                result = String.Format("{0}:{1}:{2} hours", diff.Hours, diff.Minutes, diff.Seconds);
            }

            return result;
        }
    }
}
