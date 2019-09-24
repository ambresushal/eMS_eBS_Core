using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Validators;

namespace tmg.equinox.services.api.Models
{
    [Validator(typeof(FolderVersionValidator))]
    public class FolderVersion
    {
        public int folderId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Category { get; set; }
        public string CategoryId { get; set; } 
       
    }
    public class FolderVersionToDelete {
        public int folderversionId { get; set; }
        public int folderId { get; set; }
        public string versionType { get; set; }
    }

    public class FolderVersionToUpdate
    {
        public int folderId { get; set; } 
        public string comments { get; set; }      
        
    }
}