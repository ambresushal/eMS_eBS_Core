using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormInstanceDetail
{
    public class FormInstanceRepeaterDataBuilder
    {
        IUnitOfWorkAsync _unitOfWork;

        public FormInstanceRepeaterDataBuilder(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// This method is used to get the repeater full name using the uielement Id.
        /// </summary>
        /// <param name="uiElmentId"></param>
        /// <returns></returns>
        public string GetRepeaterFullName(int uiElmentId)
        {
            string fullName = string.Empty;
            UIElement uielement = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                    .Get()
                                    .Where(c => c.UIElementID == uiElmentId)
                                   select u).FirstOrDefault();
            if (uielement != null)
            {
                int parentUIElementID = uielement.ParentUIElementID.HasValue ? uielement.ParentUIElementID.Value : 0;

                fullName = uielement.GeneratedName;
                while (parentUIElementID > 0)
                {
                    uielement = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                    .Get()
                                    .Where(c => c.UIElementID == parentUIElementID)
                                 select u).FirstOrDefault();
                    parentUIElementID = uielement.ParentUIElementID.HasValue ? uielement.ParentUIElementID.Value : 0;
                    if (parentUIElementID > 0)
                    {
                        fullName = uielement.GeneratedName + "." + fullName;
                    }
                }
            }
            return fullName;
        }

        /// <summary>
        /// This method is used to retrieve spliced repeater data and updated formInstance data.
        /// </summary>
        /// <param name="formInstanceData"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetSplicedRepeaterAndUpdatedFormInstanceData(string formInstanceData, string fullPath)
        {
            string repeaterDataJsonString = string.Empty;
            string formInstanceDataJsonString = string.Empty;
            string[] elements = fullPath.Split('.');
            string keyname = elements.Last();

            IDictionary<string, string> updatedValues = new Dictionary<string, string>();
            IDictionary<string, object> repeaterValues = new Dictionary<string, object>();

            try
            {
                if (formInstanceData != string.Empty)
                {
                    dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(formInstanceData, new ExpandoObjectConverter());

                    IDictionary<string, object> values = jsonObject as IDictionary<string, object>;

                    for (int index = 0; index <= elements.Length - 1; index++)
                    {
                        if (values.ContainsKey(keyname))
                        {
                            if (!repeaterValues.ContainsKey(keyname))
                            {
                                repeaterValues.Add("" + keyname + "", values[keyname]);
                            }
                            repeaterDataJsonString = JsonConvert.SerializeObject(repeaterValues);
                            values[keyname] = new Dictionary<string, string>();
                            formInstanceDataJsonString = JsonConvert.SerializeObject(jsonObject);
                            if (!updatedValues.ContainsKey(keyname) && !updatedValues.ContainsKey(elements.First()))
                            {
                                updatedValues.Add("" + keyname + "", repeaterDataJsonString);
                                updatedValues.Add("" + elements.First() + "", formInstanceDataJsonString);
                            }

                        }
                        else if (values.ContainsKey(elements[index]))
                        {
                            values = values[elements[index]] as IDictionary<string, object>;
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }


            return updatedValues;
        }

        public string GetUpdatedMasterListJsonDataWithRepeaterData(int formInstsanceId, IDictionary<string, string> masterListJsonData)
        {
            string updatedMasterList = string.Empty;
            try
            {

                IList<FormInstanceRepeaterDataMap> formInstanceRepeaterData =
                             this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Query()
                                                       .Filter(e => e.FormInstanceID == formInstsanceId)
                                                       .Get().ToList();
                if (formInstanceRepeaterData.Count() > 0)
                {
                    FormInstanceRepeaterParcer formInstanceRepeaterParcer = new FormInstanceRepeaterParcer(this._unitOfWork);
                    updatedMasterList = formInstanceRepeaterParcer.GetMasterListFormInstanceRepeaterData(masterListJsonData, formInstanceRepeaterData);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return updatedMasterList;
        }

        

    }
}
