using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using tmg.equinox.core.masterlistcascade.filter;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.viewmodels;
using tmg.equinox.applicationservices.viewmodels;
//using tmg.equinox.documentcomparer;

namespace tmg.equinox.core.masterlistcascade.filter
{
    public class DefaultFilterExpressionProcessor : IFilterExpressionProcessor
    {

        public List<DocumentFilterResult> ProcessExpression(IMasterListCascadeService mlcService, IFolderVersionServices fvService, JObject queryData, FilterExpression filter, MasterListCascadeViewModel cascadeModel, MasterListVersions versions,DateTime effectiveDate,int folderVersionID)
        {
            List<DocumentFilterResult> results = new List<DocumentFilterResult>();
            //invoke SP and get the list of documents to process
            results = GetDocumentList(mlcService, cascadeModel.TargetDesignID,cascadeModel.TargetDesignVersionID, effectiveDate,folderVersionID,cascadeModel.MasterListCascadeID);
            return results;
        }

        private List<DocumentFilterResult> GetDocumentList(IMasterListCascadeService mlcService,int formDesignID,int formDesignVersionID,DateTime effectiveDate,int folderVersionID,int masterListCascadeID)
        {
            List<DocumentFilterResult> results = new List<DocumentFilterResult>();
            results = mlcService.FilterDocuments(effectiveDate, "", formDesignID,formDesignVersionID, null,folderVersionID,masterListCascadeID);
            return results;
        }

    }
}
