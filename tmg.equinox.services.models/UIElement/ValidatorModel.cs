using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class ValidatorModel : ViewModelBase
    {

        public int TenantId { get; set; }

        public int UIElementId { get; set; }

        public int ValidatorId { get; set; }

        public string Label { get; set; }

        public bool IsRequired { get; set; }

        public bool IsLibraryRegex { get; set; }

        public int LibraryRegexId { get; set; }

        public string Regex { get; set; }

        public string Message { get; set; }

    }
}
