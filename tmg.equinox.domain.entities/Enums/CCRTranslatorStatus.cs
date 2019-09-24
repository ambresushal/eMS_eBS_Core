using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Enums
{
    public enum CCRTranslatorStatus
    {
        Queued = 1,
        InProgress = 2,
        Errored = 3,
        Completed = 4,
        Queued_PROD = 5,
        Completed_PROD = 6
    }
}
