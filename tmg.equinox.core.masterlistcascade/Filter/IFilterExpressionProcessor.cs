using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.core.masterlistcascade.filter;
using tmg.equinox.domain.viewmodels;

namespace tmg.equinox.core.masterlistcascade.filter
{
    public interface IFilterExpressionProcessor
    {
        List<DocumentFilterResult> ProcessExpression(IMasterListCascadeService mlcService, IFolderVersionServices fvService, JObject queryData, FilterExpression filter, MasterListCascadeViewModel cascadeModel, MasterListVersions versions, DateTime effectiveDate,int folderVersionID);
    }
}
