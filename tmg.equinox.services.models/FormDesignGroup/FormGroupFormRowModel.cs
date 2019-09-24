using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class FormGroupFormRowModel : ViewModelBase
    {     
        public int FormDesignId { get; set; }     
        public bool IsIncluded { get; set; }        
        public bool AllowMultipleInstance { get; set; }        
        public string FormDesignName { get; set; }        
        public string Abbreviation { get; set; }        
        public int TenantID { get; set; }
        public int Sequence { get; set; } 
    }
}