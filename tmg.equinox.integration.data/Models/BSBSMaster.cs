﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class BSBSMaster : Entity
    {
        public string PDBC_PFX { get; set; }
        public string BSBS_TYPE { get; set; }
        public string BSBS_DESC { get; set; }
    }
}
