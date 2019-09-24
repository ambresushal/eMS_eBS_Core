using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class FormDesignVersionRowModel : ViewModelBase
    {
        public int? FormDesignId { get; set; }
        public int FormDesignVersionId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int StatusId { get; set; }
        public string StatusText { get; set; }
        public string Version { get; set; }
        public string FormDesignVersionData { get; set; }
        public int? FormDesignTypeID { get; set; }
        public string FormDesignName { get; set; }
        public string RuleExecutionTreeJSON { get; set; }
        public string RuleEventMapJSON { get; set; }
    }
}