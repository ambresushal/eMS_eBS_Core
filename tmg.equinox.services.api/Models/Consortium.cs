using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Validators;

namespace tmg.equinox.services.api.Models
{
    //[Validator(typeof(ConsortiumValidator))]
    public class Consortium
    {
        public int ConsortiumID { get; set; }
        public string ConsortiumName { get; set; }
    }
    //[Validator(typeof(ConsortiumValidator))]
    public class ConsortiumAdd
    {
        public string ConsortiumName { get; set; }
    }
}