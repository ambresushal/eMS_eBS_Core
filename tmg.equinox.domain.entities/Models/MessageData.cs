using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
   public class MessageData : Entity
    {
        
        public int MessageID { get; set; }
        public string MessageKey { get; set; }
        public string MessageText { get; set; }
        public string MessageType { get; set; }
    }
}
