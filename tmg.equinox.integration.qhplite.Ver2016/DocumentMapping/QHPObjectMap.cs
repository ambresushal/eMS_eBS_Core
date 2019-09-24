using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.DocumentMapping
{
    internal class QHPObjectMap
    {
        public string DocumentContainer { get; set; }
        public ParentPropertyType ParentType { get; set; }
        public string QHPContainer { get; set; }
        public List<QHPPropertyMap> PropertyMappings { get; set; }
    }
}
