using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IServiceRequestHandlerService
    {
        bool ValidateFormInstance(int formInstanceID, int serviceDesignVersionID, int formDesignVersionID);
        bool CheckFormInstanceFormDesignID(int formInstanceID, int formDesignVersionID);
        IDictionary<int, string> GetFormInstanceData(int serviceDesignVersionID, int formDesignVersionID, IList<ServiceRouteParameterViewModel> searchParametersList, IDictionary<string, object> searchValues);
        ResponseType GetServiceResponseType(int serviceDesignID);
    }
}
