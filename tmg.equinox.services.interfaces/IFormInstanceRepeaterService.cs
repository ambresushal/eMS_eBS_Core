using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFormInstanceRepeaterService
    {
        ServiceResult SaveFormInstanceRepeaterData(int formInstanceId, string[] fullNameList, string repeaterData, string user, int formDesignVersionId);

        GridPagingResponse<object> GetFormInstanceRepeaterData(int formInstanceId, string fullName, GridPagingRequest gridPagingRequest);
        bool CheckDuplication(int formInstanceId, string fullname, string rowData,List<string> repaterRowId, List<string> duplicationObject);
        ServiceResult SaveRepeaterFormInstanceDataOnFinalize(int tenantId, int formDesignVersionId, string enteredBy);
        ServiceResult AddFormInstanceRepeaterDataMapOnFinalize(int formInstanceID, string repeaterData, int tenantId, string enteredBy, int repeaterUIElementID, string fullName, int formDesignVersionId);
        string GetMasterListFormInstanceData(int formInstanceId);
        IDictionary<string, object> GetDuplicatedElementsList(string repeaterFullName, int formInstanceId, int formDesignVersionId);
    }
}
