using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.web.ExpresionBuilder.Handlers
{
    public class SectionProductRules
    {
        private FormDesignVersionDetail _detail;
        private IFolderVersionServices _folderVersionServices;
        private int _formInstanceId;
        private static object lockObject = new object();
        public SectionProductRules(int formInstanceId, FormDesignVersionDetail detail, IFolderVersionServices folderVersionServices)
        {
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._folderVersionServices = folderVersionServices;
        }

        public void Run()
        {
            try
            {
                JObject objFormData = JObject.Parse(_detail.JSONData);
                string isFoundation = "false";
                if (_folderVersionServices.IsFoundationFolder(_formInstanceId))
                    isFoundation = "true";

                objFormData.SelectToken(ExpressionBuilderConstants.IsFoundation).Replace(new JValue(isFoundation));
                JObject objFormDataUpdated = UpdatePlanInformation(objFormData);
                this._detail.JSONData = JsonConvert.SerializeObject(objFormData);
            }
            catch (Exception ex)
            {
                ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
        }

        public JObject UpdatePlanInformation(JObject objFormData)
        {
            FormInstanceViewModel formDetails = _folderVersionServices.GetFormInstance(1, _formInstanceId);
            string updatedFoundationTemplate = (string)objFormData.SelectToken(ExpressionBuilderConstants.IsFoundation);
            if (updatedFoundationTemplate == "true")
                objFormData.SelectToken(ExpressionBuilderConstants.BasePlanTemplate).Replace(formDetails.FormDesignName);
            else
                objFormData.SelectToken(ExpressionBuilderConstants.PlanName).Replace(formDetails.FormDesignName);

            DateTime PlanEffectiveDate = ((DateTime)formDetails.EffectiveDate);
            var effectiveDate = PlanEffectiveDate.ToShortDateString();
            objFormData.SelectToken(ExpressionBuilderConstants.PlanEffectiveDate).Replace(effectiveDate.ToString());

            string planRenewalDefaultData = (string)objFormData.SelectToken(ExpressionBuilderConstants.PlanRenewalDate);
            if (string.IsNullOrEmpty(planRenewalDefaultData))
            {
                DateTime planRenewalDate = PlanEffectiveDate.AddYears(1).AddDays(-1);
                var renewalDate = planRenewalDate.ToShortDateString();
                planRenewalDefaultData = renewalDate.ToString();
            }
            objFormData.SelectToken(ExpressionBuilderConstants.PlanRenewalDate).Replace(planRenewalDefaultData);
            return objFormData;
        } 
    }
}
