using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignGroup
{
    public class FormDesignGroupRowModel : ViewModelBase
    {
        public int FormGroupId { get; set; }
        public string FormDesignGroupName { get; set; }                        
        public int TenantID { get; set; }
    }
}