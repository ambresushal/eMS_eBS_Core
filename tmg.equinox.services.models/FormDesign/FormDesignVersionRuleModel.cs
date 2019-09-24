using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class FormDesignVersionRuleModel : ViewModelBase
    {
        public int FormDesignVersionId { get; set; }

        public List<RuleDesign> Rules { get; set; }

        public List<ValidationDesign> Validations { get; set; }

    }
}