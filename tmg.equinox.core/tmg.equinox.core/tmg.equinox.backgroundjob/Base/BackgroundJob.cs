using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.backgroundjob
{
    public abstract class BackgroundJob<TArgs> : IBackgroundJob<TArgs>
    {


        public abstract void Execute(TArgs args);
        //public abstract void ExecuteQueue(TArgs args);
    }

}
