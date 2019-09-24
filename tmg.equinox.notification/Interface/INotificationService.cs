using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.notification
{
    public interface INotificationService
    {
        /// <summary>
        /// SendTo : userName
        /// UserId: if you have userId then pass user id its same as send message to this UserId
        /// Paramaters :  var paramaters = new List<Paramters>();
        ///                 paramaters.Add(new Paramters { key = "{user}", Value = "userName" });
        ///                paramaters.Add(new Paramters { key = "{sectionName}", Value = "displaySectionName" });
        /// MessageKey = MessageKey.LOCK
        /// LoggedInUser : AddedBy
        /// </summary>
        /// <param name="notificationInfo"></param>
        void SendNotification(NotificationInfo notificationInfo);
        bool MarkNotificationToRead(bool viewMode, int CurrentUserId, string CurrentUserName);
    }
}
