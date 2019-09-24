using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ParaSequenceComparer : IComparer<JToken>
    {
        public int Compare(JToken x, JToken y)
        {
            int result = 0;
            int xPara = 0;
            int.TryParse(x["ParagraphNo"].ToString(), out xPara);
            int yPara = 0;
            int.TryParse(y["ParagraphNo"].ToString(), out yPara);
            int xSeq = 0;
            int.TryParse(x["SequenceNo"].ToString(), out xSeq);
            int ySeq = 0;
            int.TryParse(y["SequenceNo"].ToString(), out ySeq);
            if(xPara > yPara)
            {
                result = 1;
            }
            if(xPara == yPara && xSeq > ySeq)
            {
                result = 1;
            }
            if (yPara > xPara)
            {
                result = -1;
            }
            if (yPara == xPara && ySeq > xSeq)
            {
                result = -1;
            }
            return result;
        }
    }
}
