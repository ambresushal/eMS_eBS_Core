using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class Notificationstatus :Entity
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public string MessageKey { get; set; }
        public int Userid { get; set; }
        public Boolean IsRead { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Boolean IsActive { get; set; }
    }
}
