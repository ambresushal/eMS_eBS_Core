using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class FormDesignRowModel : ViewModelBase
    {
        public int FormDesignId { get; set; }
        public string FormDesignName { get; set; }
        public string DisplayText { get; set; }
        public bool IsActive { get; set; }
        public string Abbreviation { get; set; }
        public int TenantID { get; set; }
        public bool IsIncluded { get; set; }
        public bool IsMasterList { get; set; }
        public int DocumentDesignTypeID { get; set; }
        public int DocumentLocationID { get; set; }
        public int SourceDesign { get; set; }
        public bool IsAliasDesignMasterList { get; set; }
        public bool UsesAliasDesignMasterList { get; set; }
        public bool IsSectionLock { get; set; }
    }
}