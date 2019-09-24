using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class RegexLibrary : Entity
    {
        public RegexLibrary()
        {
            this.Validators = new List<Validator>();
        }

        public int LibraryRegexID { get; set; }
        public string LibraryRegexName { get; set; }
        public string RegexValue { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string AddedBy { get; set; }
        public string MaskExpression { get; set; }
        public string Placeholder { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual ICollection<Validator> Validators { get; set; }
        public string Message { get; set; }
        
    }
}
