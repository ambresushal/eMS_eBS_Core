using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IServiceDefinitionService
    {
        IEnumerable<ServiceDefinitionRowModel> GetServiceDefinitionListForServiceDesignVersion(int tenantId, int serviceDesignVersionID);

        ServiceResult AddServiceDefinition(int tenantID, int formDesignVersionID, int serviceDesignVersionID, int uiElementID, int parentServiceDefinitionID, string username, bool isKey, bool addChildKeys, bool addChildElements);

        ServiceResult DeleteServiceDefinition(int tenantID, int serviceDesignVersionID, int serviceDefinitionID);

        ServiceResult UpdateServiceDefinition(ServiceDefinitionViewModel viewModel, string userName);

        ServiceDefinitionViewModel GetServiceDefinitionDetails(int tenantID, int serviceDefinitionID);

        IEnumerable<ServiceParameterRowModel> GetServiceParameterList(int tenantID, int serviceDesignID, int serviceDesignVersionID);

        ServiceResult AddServiceParameter(int tenantID, int serviceDesignID, int serviceDesignVersionID, int dataTypeID, string name, bool isRequired, int uielementID, string userName);

        ServiceResult AddDefaultServiceParameter(int tenantID, int serviceDesignID, int serviceDesignVersionID, int dataTypeID, string name, bool isRequired, string userName);

        ServiceResult UpdateServiceParameter(int tenantID, int serviceParameterID, int serviceDesignID, int serviceDesignVersionID, int dataTypeID, string name, bool isRequired, int uielementID, string userName);

        ServiceResult DeleteServiceParameter(int tenantID, int serviceParameterID, int serviceDesignID, int serviceDesignVersionID, string userName);

        IEnumerable<UIElementModel> GetUIElementList(int tenantID, int formDesignVersionID, int formDesignID);
    }
}
