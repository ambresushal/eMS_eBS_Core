﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentDesignType : Entity
    {
        public int DocumentDesignTypeID { get; set; }
        public string DocumentDesignName { get; set; }
    }
}
