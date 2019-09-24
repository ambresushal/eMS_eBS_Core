using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.expressionbuilder
{
    public class SectionARules
    {
        private FormDesignVersionDetail _detail;
        private IFormInstanceService _formInstanceService;
        private int _formInstanceId;
        private static object lockObject = new object();
        public SectionARules(FormDesignVersionDetail detail, IFormInstanceService formInstanceService, int formInstanceId)
        {
            this._detail = detail;
            this._formInstanceService = formInstanceService;
            this._formInstanceId = formInstanceId;
        }

        public void Run()
        {
            try
            {
                JObject objFormData = JObject.Parse(_detail.JSONData);
                var contractNumber = objFormData.SelectToken(ExpressionBuilderConstants.ContractNumber);
                if (string.IsNullOrEmpty(Convert.ToString(contractNumber)))
                {
                    string proxyNumber = String.Empty;
                    lock (lockObject)
                    {
                        proxyNumber = _formInstanceService.GetProxyNumber(_formInstanceId);
                    }
                    objFormData.SelectToken(ExpressionBuilderConstants.ContractNumber).Replace(new JValue(proxyNumber));
                }

                this._detail.JSONData = JsonConvert.SerializeObject(objFormData);
            }
            catch (Exception ex)
            {
                ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
        }
    }
}
