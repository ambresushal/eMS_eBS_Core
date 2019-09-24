using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IAuditRequestService
    {
        //Audit Request Interfaces       
        ServiceResult AddAuditData(string userName, string ipAddress, string areaAccessed, DateTime timeAccesed);        
    }
}
