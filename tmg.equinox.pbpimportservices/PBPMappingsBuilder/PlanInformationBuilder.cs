using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;

namespace tmg.equinox.pbpimportservices.PBPMappingsBuilder
{
    public class PlanInformationBuilder
    {
        int _forminstanceid;
        string _pbpQId = string.Empty;
        int _pbpBatchId;
        List<PBPDataMapViewModel> _pbpDataModelList;
        string _userName;
        private IPBPImportServices _pbpImportServices;
        MapPBPData _mapPBPData;

        public PlanInformationBuilder(int forminstanceId, string pbpQId, int pbpBatchId, string userName, List<PBPDataMapViewModel> pbpDataModelList, IPBPImportServices pbpServices, MapPBPData mapPBPData)
        {
            this._forminstanceid = forminstanceId;
            this._pbpBatchId = pbpBatchId;
            this._pbpQId = pbpQId;
            this._pbpDataModelList = pbpDataModelList;
            this._userName = userName;
            this._pbpImportServices = pbpServices;
            this._mapPBPData = mapPBPData;
        }

        #region PlanInformation

        private void MapPlanInformationData(JObject json)
        {
            List<PlanInformationViewModel> planinfomapping = _pbpImportServices.GetPlanInformationMapping();
            List<PlanTypeViewModel> planTypeList = _pbpImportServices.GetPlanTypeCodeDescription();

            foreach (PlanInformationViewModel viewmodel in planinfomapping)
            {
                string pbpData = string.Empty;
                if (viewmodel.IsCustomRule == false)
                    pbpData = HelperUtility.GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                else
                    pbpData = HelperUtility.ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, 0, _pbpDataModelList, _pbpQId, _pbpBatchId, planTypeList);

                json.SelectToken(viewmodel.FieldPath)[viewmodel.FieldName] = pbpData;

                if (viewmodel.FieldName == ELEMENTNAME.PlanType && pbpData == DATA.HMOPOS)
                    _mapPBPData._isOONApplicable = true;
            }
        }

        #endregion PlanInformation
    }
}
