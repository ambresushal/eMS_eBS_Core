using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.UnitTesting.Model;

namespace tmg.equinox.document.rulebuilder.UnitTesting
{
  public static class GetBooleanSourceData
    {
      public static string GetBooleanElementData()
      {
          string filePath = @"D:\DesignSources\ProductDesign.json";
          string elementPath = "ProductDefinition.GeneralInformation.ProductPeriodIndicator";

          string formDesignData =System.IO.File.ReadAllText(filePath);
          JObject jObject = JObject.Parse(formDesignData);
          
          FormDesignVersionDetail detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesignData);

          return formDesignData;
      }
    }
}
