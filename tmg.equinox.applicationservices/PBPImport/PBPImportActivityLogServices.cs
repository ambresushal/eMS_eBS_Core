using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.PBPImport
{

    public class PBPImportActivityLogServices : IPBPImportActivityLogServices
    {
        #region Private Members
        IUnitOfWorkAsync _unitOfWorkAsync = null;
        #endregion Private Members

        #region Constructor
        public PBPImportActivityLogServices(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #endregion

        public void InitializeVariables(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #region Public Methods
        public void AddPBPImportActivityLog(int PBPImportQueueID, string methodName, string Message, string TableName, string QID, Exception Ex)
        {
            try
            {
                string Exceptiostr = string.Empty;
                string ErrorMessage = string.Empty;
                PBPImportActivityLogViewModel viewModel = new PBPImportActivityLogViewModel();
                viewModel.PBPImportQueueID = PBPImportQueueID;

                if (!String.IsNullOrEmpty(methodName))
                {
                    ErrorMessage+= "Error in Method " + methodName +  "\n";
                    //string.Concat(ErrorMessage, "Error in Methood ", methodName, "\n");
                }

                if (!String.IsNullOrEmpty(Message))
                {
                    ErrorMessage += "Message : -" + Message + "\n";
                    //string.Concat(ErrorMessage, string.Concat("Message : -", Message, "\n"));
                }

                if (!String.IsNullOrEmpty(TableName))
                {
                    ErrorMessage += " Table Name: -" + TableName + "\n";
                    //string.Concat(ErrorMessage, string.Concat(" Table Name: -", TableName, "\n"));
                }

                if (!String.IsNullOrEmpty(QID))
                {
                    ErrorMessage +=
                     string.Concat(ErrorMessage, string.Concat("Exception for QID :-", QID, "\n"));
                }

                viewModel.CreatedBy = "TMG Super User";
                try
                {
                    if (Ex != null)
                    {
                        Exceptiostr = GetExceptionMessages(Ex, "");
                    }
                }
                catch { }

                PBPImportActivityLog activitylog = new PBPImportActivityLog();
                activitylog.PBPImportQueueID = viewModel.PBPImportQueueID;
                activitylog.FileName = viewModel.FileName;
                activitylog.TableName = viewModel.TableName;
                activitylog.Message = String.Concat(ErrorMessage, Exceptiostr);
                activitylog.CreatedBy = viewModel.CreatedBy;
                activitylog.CreatedDate = DateTime.Now;
                activitylog.UserErrorMessage = string.Empty;
                this._unitOfWorkAsync.RepositoryAsync<PBPImportActivityLog>().Insert(activitylog);
                this._unitOfWorkAsync.Save();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        private string GetExceptionMessages(Exception e, string msgs = "")
        {
            if (e == null)
            {
                return string.Empty;
            }
            if (msgs == "")
            {
                if (!e.Message.Equals("Exception of type 'System.Exception' was thrown."))
                {
                    msgs = e.Message;
                }
            }
            if (e.InnerException != null)
                msgs += "\r\nInnerException: " + GetExceptionMessages(e.InnerException);
            return msgs;
        }

        public void SaveUserErrorMessage(int PBPImportQueueID,string QId, string Message)
        {
            try {
                PBPImportActivityLog activitylog = new PBPImportActivityLog();
                activitylog.PBPImportQueueID = PBPImportQueueID;
                activitylog.CreatedBy = "PBP Super User";
                activitylog.CreatedDate = DateTime.Now;
                activitylog.UserErrorMessage = QId +" "+ Message;
                this._unitOfWorkAsync.RepositoryAsync<PBPImportActivityLog>().Insert(activitylog);
                this._unitOfWorkAsync.Save();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

        }

        #endregion Public Methods
    }
}
