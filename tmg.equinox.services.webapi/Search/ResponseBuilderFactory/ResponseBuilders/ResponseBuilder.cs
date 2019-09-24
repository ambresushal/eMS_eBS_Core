using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;
using tmg.equinox.services.webapi.Framework;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices;
using Newtonsoft.Json;

namespace tmg.equinox.services.webapi.Framework
{
    public class ResponseBuilder
    {
        #region Private Members
        private int _formInstanceID { get; set; }
        private string _formData { get; set; }
        private IDictionary<int, string> _formDataList { get; set; }
        private ServiceDesignVersionDetail _detail { get; set; }
        StringBuilder jsonBuilder = new StringBuilder();
        JsonParser parser;
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructor
        public ResponseBuilder(string formData, ServiceDesignVersionDetail detail, int formInstanceID)
        {
            this._formData = formData;
            this._detail = detail;
            this._formInstanceID = formInstanceID;
            this._formDataList = new Dictionary<int, string>();
        }

        public ResponseBuilder(IDictionary<int, string> formDataList, ServiceDesignVersionDetail detail)
        {
            this._formDataList = formDataList;
            this._detail = detail;
        }
        #endregion Constructor

        #region Public Methods
        public StringBuilder BuildResponse()
        {
            try
            {
                parser = new JsonParser();

                if (!string.IsNullOrEmpty(_formData) && _formDataList.Count() == 0)
                {
                    BuildSingleObjectResponse();
                }
                else if (_formDataList.Count() > 0)
                {
                    BuildListResponse();
                }
                //else
                //{
                //    jsonBuilder.Append("{ Message : " + " No records found." + "}"); 
                //}
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return jsonBuilder;
        }

        #endregion Public Methods

        #region Private Methods
        private void BuildSingleObjectResponse()
        {
            jsonBuilder.Append('{');

            jsonBuilder.Append("\"" + "formInstanceID" + "\": ");
            jsonBuilder.Append(" \"" + _formInstanceID + "\", ");

            for (int index = 0; index < _detail.Sections.Count; index++)
            {
                this.BuildSectionData(_detail.Sections[index]);
                if (index <= (_detail.Sections.Count - 1))
                {
                    jsonBuilder.Append(',');
                }
            }

            for (int index = 0; index < _detail.Repeaters.Count; index++)
            {
                jsonBuilder.Append("\"" + _detail.Repeaters[index].GeneratedName + "\": ");
                this.BuildRepeaterData(_detail.Repeaters[index]);
                if (index <= (_detail.Repeaters.Count - 1))
                {
                    jsonBuilder.Append(',');
                }
            }
            jsonBuilder.Append('}');
        }

        private void BuildListResponse()
        {
            jsonBuilder.Append("[");
            foreach (var item in _formDataList)
            {
                _formData = item.Value;
                this.BuildSingleObjectResponse();
            }
            jsonBuilder.Append(",]");
        }
        private void BuildSectionData(SectionDesign section)
        {
            try
            {
                jsonBuilder.Append("\"" + section.GeneratedName + "\": { ");
                for (int index = 0; index < section.Elements.Count; index++)
                {
                    if (!String.IsNullOrEmpty(section.Elements[index].GeneratedName))
                    {
                        this.BuildElementData(section.Elements[index]);
                        if (index <= (section.Elements.Count - 1))
                        {
                            jsonBuilder.Append(",");
                        }
                    }
                }
                jsonBuilder.Append('}');
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void BuildRepeaterData(RepeaterDesign repeater)
        {
            try
            {
                if ((repeater.PrimaryDataSource != null)
                    || (repeater.InlineDataSources != null && repeater.InlineDataSources.Count() > 0)
                    || (repeater.ChildDataSources != null && repeater.ChildDataSources.Count() > 0))
                {
                    object value = parser.GetValue(_formData, repeater.UIElementFullName);
                    string val = value == null ? string.Empty : JArray.FromObject(value).ToString(Formatting.None);

                    jsonBuilder.Append(val);
                }
                else
                {
                    jsonBuilder.Append("[");

                    object value = parser.GetValue(_formData, repeater.UIElementFullName);
                    StringBuilder valueBuilder = new StringBuilder();
                    JArray array = JArray.FromObject(value);
                    if (array.Count() > 0 && repeater.Elements.Count() > 0)
                    {
                        for (int i = 0; i < array.Count(); i++)
                        {
                            valueBuilder.Append("{");
                            for (int index = 0; index < repeater.Elements.Count(); index++)
                            {
                                valueBuilder.Append("\"" + repeater.Elements[index].GeneratedName + "\": ");
                                valueBuilder.Append(" \"" + array[i][repeater.Elements[index].GeneratedName] + "\" ");

                                if (index < repeater.Elements.Count() - 1)
                                {
                                    valueBuilder.Append(",");
                                }
                            }
                            if (array[i]["IQMedicalPlanNetwork"] != null)
                            {
                                valueBuilder.Append(",");
                                valueBuilder.Append("\"" + "IQMedicalPlanNetwork" + "\": ");
                                valueBuilder.Append(array[i]["IQMedicalPlanNetwork"].ToString());
                            }
                            valueBuilder.Append("}");
                            if (i < array.Count() - 1)
                            {
                                valueBuilder.Append(",");
                            }
                        }
                        jsonBuilder.Append(valueBuilder);
                    }
                    jsonBuilder.Append("]");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void BuildElementData(ElementDesign element)
        {
            try
            {
                if (element.Section != null)
                {
                    this.BuildSectionData(element.Section);
                }
                else
                {
                    jsonBuilder.Append("\"" + element.GeneratedName + "\": ");

                    if (element.Repeater != null)
                    {
                        this.BuildRepeaterData(element.Repeater);
                    }
                    else
                    {
                        object value = parser.GetValue(_formData, element.UIElementFullName);
                        string val = value == null ? string.Empty : value.ToString();
                        jsonBuilder.Append(" \"" + val + "\" ");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion Private Methods
    }
}