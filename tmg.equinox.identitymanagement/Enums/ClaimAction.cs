using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace tmg.equinox.identitymanagement.Enums
{
    public enum ClaimAction
    {
        [Description("Edit")]
        Edit,
        [Description("View")]
        View,
        [Description("NA")]
        NA
    }
}
