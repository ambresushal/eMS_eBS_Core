using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class EmailTemplatePlaceHolder : Entity
    {
        public int ID { get; set; }
        public string PlaceHolder { get; set; }
        public ICollection<EmailTemplatePlaceHolderMapping> PlaceHolders { get; set; }
    }
}
