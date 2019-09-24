using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.FindnReplace
{
    public interface IReplaceTextProcessor
    {
        List<ChangeLogModel> Process();
    }
}
