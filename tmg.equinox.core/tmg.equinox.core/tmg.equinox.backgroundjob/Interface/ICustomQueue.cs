using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.backgroundjob
{
    public interface ICustomQueue
    {
        void InsertQueue(BaseJobInfo baseInfo);
        void UpdateQueue(BaseJobInfo baseInfo);
        void UpdateFailQueue(BaseJobInfo baseInfo);
    }
}
