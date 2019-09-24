using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using tmg.equinox.infrastructure.exceptionhandling;


namespace tmg.equinox.web.FormInstance
{
    public class FormInstanceParser
    {
        public object GetValue(string formInstanceData, string fullPath)
        {
            object getValue = null;

            try
            {
                var converter = new ExpandoObjectConverter();
                dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(formInstanceData, converter);

                string[] elements = fullPath.Split('.');
                string keyname = elements.Last();
                int count = elements.Length - 1;
                int i = -1;

                IDictionary<string, object> values = jsonObject as IDictionary<string, object>;

                foreach (var element in elements)
                {
                    i++;
                    if (values is ExpandoObject)
                    {
                        if (i == count)
                        {
                            if (values.ContainsKey(keyname))
                            {
                                //removed .ToString() method to return the collection 
                                //collection needs to be used in SBC Report.
                                getValue = values.FirstOrDefault(x => x.Key == keyname).Value;
                                return getValue;
                            }
                        }
                        else
                        {
                            values = values[element] as IDictionary<string, object>;
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
            return getValue;
        }

        
    }
}






