using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.domain.viewmodels;

namespace tmg.equinox.core.masterlistcascade.filter
{
    public class DocumentFilter
    {
        private IMasterListCascadeService _mlcService;
        private IFolderVersionServices _fvService;
        private FilterExpression _filter;
        private MasterListCascadeViewModel _cascadeModel;
        private DateTime _effectiveDate;
        private int _folderVersionID;
        private MasterListVersions _versions;
        public DocumentFilter(IMasterListCascadeService mlcService, IFolderVersionServices fvService, FilterExpression filter, MasterListCascadeViewModel cascadeModel, MasterListVersions versions, DateTime effectiveDate,int folderVersionID)
        {
            _mlcService = mlcService;
            _fvService = fvService;
            _filter = filter;
            _effectiveDate = effectiveDate;
            _cascadeModel = cascadeModel;
            _folderVersionID = folderVersionID;
            _versions = versions;
        }

        public List<DocumentFilterResult> ProcessFilterExpression(JObject queryData)
        {
            List<DocumentFilterResult> documentList = new List<DocumentFilterResult>();
            //get the list of fields to process using the filter expression
            IFilterExpressionProcessor processor = FilterExpressionProcessorFactory.GetProcessor(_filter);
            documentList = processor.ProcessExpression(_mlcService, _fvService, queryData, _filter,_cascadeModel,_versions, _effectiveDate,_folderVersionID);
            return documentList;
        }

        public void ProcessFilter(MasterListCascadeViewModel mlCascade, MasterListVersions mlVers, int folderVersionID, ref List<DocumentFilterResult> results)
        {
            //get current Master List data
            JObject mlData = _mlcService.GetMasterListSectionData(mlCascade, mlVers.CurrentFolderVersionID, mlVers.CurrentFormInstanceID);
            //---- filter the Products to be processed
            if (_filter == null)
            {
                _filter = GetFilterExpression(mlCascade);
            }
            //get list of Products with Folder information
            results.AddRange(this.ProcessFilterExpression(mlData));

        }
        public FilterExpression GetFilterExpression(MasterListCascadeViewModel model)
        {
            FilterExpression exp = new FilterExpression();
            string rule = model.FilterExpressionRule;
            MLFilter filter = JsonConvert.DeserializeObject<MLFilter>(rule);
            if (filter != null && filter.Filters.Count > 0)
            {
                exp = filter.Filters[0];
            }
            return exp;
        }
    }
}
