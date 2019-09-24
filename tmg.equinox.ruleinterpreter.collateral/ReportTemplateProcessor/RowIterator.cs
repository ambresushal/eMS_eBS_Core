using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class RowIterator
    {
        public RowIterationType IterationType;
        public List<IGrouping<JToken, JToken>> GroupStringResult;
        public List<IGrouping<JToken,JToken>> GroupByObjectResult;
        public List<JToken> RowResult;
    }
}
