using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.DocumentCollateral;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IDocumentCollateralService
    {
        IEnumerable <DocumentCollateralViewModel> GetDocumentList(int tenantID);
    }
}
