using FluentValidation.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Validators;

namespace tmg.equinox.services.api.Models
{
    [Validator(typeof(DocumentValidator))]
    public class Document
    {
        public string Name { get; set; }
        public string DesignTemplate { get; set; }
        public JObject Data { get; set; }
    }
}