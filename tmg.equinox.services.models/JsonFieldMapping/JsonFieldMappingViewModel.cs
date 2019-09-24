using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class JsonFieldMappingViewModel
    {
        public string Label { get; set; }
        public string JSONPath { get; set; }
        public string Value { get; set; }
    }

    public class JsonFieldMappingViewModelExtended : JsonFieldMappingViewModel
    {
        public string DesignType { get; set; }
        public string FieldName { get; set; }
    }

    public class ResultJsonViewModel
    {
        public string ContractNumber { get; set; }
        public JObject Data { get; set; }
        //public List<JsonFieldMappingViewModel> Data { get; set; }
    }

    public class ProductData
    {
        public string ContractNumber { get; set; }
        public string EffectiveYear { get; set; }
    }
}
