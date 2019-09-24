using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.notification
{
    public class NotificationInfo
    {
        public string SentTo { get; set; }
        
        public string MessageKey { get; set; }
        public int UserID { get; set; }
        public List<Paramters> ParamterValues { get; set; }
        public NotificationData TotalNotificationCount { get; set; }
        public string loggedInUserName { get; set; }
    }
    public class NotificationData
    {
        public int total { get; set; }
        public string message { get; set; }

    }
}
