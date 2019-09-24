using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class EmailTemplate : Entity
    {
        public int ID { get; set; }  
        public string TemplateName { get; set; }
        public string TemplateType { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
        public bool IsHTML { get; set; } = false;

        public ICollection<EmailTemplatePlaceHolderMapping> PlaceHolders { get; set; }
    }
}
