using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Enums
{
    public enum WatchTaskStatus
    {
        Assigned = 1,
        InProgress = 2,
        Completed = 3,
        Late = 4,
        CompletedFail = 5,
        CompletedPass = 6
    }

    public enum WatchTaskPriority
    {
        Critical = 1,
        High = 2,
        Medium = 3,
        Low = 4
    }
}
