﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Report : Entity
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
    }
}
