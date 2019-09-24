using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class PBPImportEmailNotification : Entity
    {
        public PBPImportEmailNotification()
        {
        }

        public int PBPImportEmailNotification1Up { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
