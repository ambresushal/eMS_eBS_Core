using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class EmailTemplatePlaceHolderMapping : Entity
    { 
        public int ID { get; set; }
        public int EmailTemplateId { get; set; }
        public int PlaceHolderId { get; set; }
        public string Description { get; set; }
        public string PlaceHolderValue { get; set; }
        public EmailTemplate Template { get; set; }
        public EmailTemplatePlaceHolder PlaceHolder { get; set; }
    }
}
