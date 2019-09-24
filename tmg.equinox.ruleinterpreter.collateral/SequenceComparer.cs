using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class SequenceComparer : IComparer<JToken>
    {
        public int Compare(JToken x, JToken y)
        {
            int result = 0;
            int xSeq = 0;
            if(x["SequenceNo"] != null && y["SequenceNo"] != null)
            {
                int.TryParse(x["SequenceNo"].ToString(), out xSeq);
                int ySeq = 0;
                int.TryParse(y["SequenceNo"].ToString(), out ySeq);
                if (xSeq > ySeq)
                {
                    result = 1;
                }
                if (ySeq > xSeq)
                {
                    result = -1;
                }
            }
            return result;
        }
    }
}
