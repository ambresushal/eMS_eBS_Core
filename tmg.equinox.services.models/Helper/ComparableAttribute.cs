using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facetsresearch.Helper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ComparableAttribute : System.Attribute
    {
        public bool isComparable = false;
        public ComparableAttribute(bool isComparable)
        {
            this.isComparable = isComparable;
        }

    }
}
