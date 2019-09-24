using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.DashBoard
{
   public class MessageDataViewModel
    {
        public int MessageID { get; set; }
        public string MessageKey { get; set; }
        public string MessageText { get; set; }
        public string MessageType { get; set; }
    }
}
