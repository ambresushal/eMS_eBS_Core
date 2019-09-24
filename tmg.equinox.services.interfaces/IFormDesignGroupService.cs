using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignGroup;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFormDesignGroupService
    {
        //Form Design Interfaces
        IEnumerable<FormDesignGroupRowModel> GetFormDesignGroupList(int tenantId);
        IEnumerable<FormGroupFormRowModel> GetFormDesignList(int tenantId, int formGroupId);
        ServiceResult AddFormGroup(string username, int tenantId,string groupName,bool isMasterList);
        ServiceResult UpdateFormGroup(string username, int tenantId,int formGroupId, string groupName);
        Task<ServiceResult> UpdateFormGroupMapping(string username, int tenantId, int formGroupId, IList<FormGroupFormRowModel> formDesignIds);   
    }
}
