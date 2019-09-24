using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.pbpimport.Interfaces
{
    public interface IPBPImportActivityLogServices
    {
        void InitializeVariables(IUnitOfWorkAsync unitOfWorkAsync);
        void AddPBPImportActivityLog(int PBPImportQueueID, string methodName, string Message, string TableName, string QID, Exception Ex);

        void SaveUserErrorMessage(int PBPImportQueueID,string qID, string Message);
    }
}
