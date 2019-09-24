using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IServiceDesignService
    {
        //Service Design Interfaces
        IEnumerable<ServiceDesignRowModel> GetServiceDesignList(int tenantId);
        ServiceResult AddServiceDesign(string userName, int tenantId, string serviceName, string serviceMethodName, bool doesReturnAList, bool IsReturnJSON);
        ServiceResult UpdateServiceDesign(string userName, int tenantId, int serviceDesignId, string serviceName, string serviceMethodName);
        ServiceResult DeleteServiceDesign(string userName, int tenantId, int serviceDesignId);

        //Service Design Version Interfaces
        IEnumerable<ServiceDesignVersionRowModel> GetServiceDesignVersionList(int tenantId, int serviceDesignId);
        ServiceResult AddServiceDesignVersion(string userName, int tenantId, int serviceDesignId, DateTime effectiveDate, string versionNumber, int formDesignID, int formDesignVersionID);
        ServiceResult UpdateServiceDesignVersion(string userName, int tenantId, int serviceDesignVersionId, int formDesignVersionId, DateTime effectiveDate);
        ServiceResult FinalizeServiceDesignVersion(string userName, int tenantId, int serviceDesignVersionId);
        ServiceResult DeleteServiceDesignVersion(string userName, int tenantId, int serviceDesignVersionId, int serviceDesignId);
        ServiceResult CopyServiceDesignVersion(string userName, int tenantId, int serviceDesignVersionId, int serviceDesignId, DateTime effectiveDate, string versionNumber);
        ServiceDesignVersionDetail GetServiceDesignVersionDetail(int tenantID, int serviceDesignVersionID);

        IList<ServiceRouteViewModel> GetServiceDesignRouteList(int tenantID);
        ServiceRouteViewModel GetServiceDesignRouteList(int tenantID, int serviceDesignVersionID);
        ServiceDesignPreviewViewModel GetServiceDesignPreview(int tenantID, int serviceDesignID, int serviceDesignVersionID);
    }
}
