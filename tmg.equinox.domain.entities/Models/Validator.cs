using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Validator : Entity
    {
        public int ValidatorID { get; set; }
        public int UIElementID { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public string Regex { get; set; }
        public Nullable<bool> IsLibraryRegex { get; set; }
        public Nullable<int> LibraryRegexID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; }
        public string MaskExpression { get; set; }
        public virtual RegexLibrary RegexLibrary { get; set; }
        public virtual UIElement UIElement { get; set; }
        public bool MaskFlag { get; set; }

        public Validator Clone(string username, DateTime addedDate)
        {
            Validator item = new Validator();

            item.ValidatorID = this.ValidatorID;
            item.IsRequired = this.IsRequired;
            item.Regex = this.Regex;
            item.IsLibraryRegex = this.IsLibraryRegex;
            item.LibraryRegexID = this.LibraryRegexID;
            item.AddedBy = username;
            item.AddedDate = addedDate;
            item.IsActive = this.IsActive;
            item.Message = this.Message;
            item.MaskExpression = this.MaskExpression;
            item.MaskFlag = this.MaskFlag;
            return item;
        }
    }
}
