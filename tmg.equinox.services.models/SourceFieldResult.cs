using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class SourceElement
    {
        public int ID { get; set; }
        public int ElementID { get; set; }
        public string ElementName { get; set; }
        public string ElementLabel { get; set; }
        public string ElementPath { get; set; }
        public string ElementPathName { get; set; }
        public string Keys { get; set; }
        public string Description { get; set; }
        public List<ImpactedElement> ImpactedElements { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class ImpactedElement
    {
        public int ID { get; set; }
        public int FormInstanceID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string SectionName { get; set; }
        public string SectionGeneratedName { get; set; }
        public int ElementID { get; set; }
        public string ElementName { get; set; }
        public string ElementGeneratedName { get; set; }
        public string ElementLabel { get; set; }
        public string ElementPath { get; set; }
        public string ElementPathLabel { get; set; }
        public string Description { get; set; }
        public string UpdateType { get; set; }
        public string Action { get; set; }
    }
}
