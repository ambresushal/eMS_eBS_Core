using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;

//using NotifSystem.Web.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
//using tmg.equinox.dependencyresolution;


namespace tmg.equinox.notification
{
    public class NotificationHub : Hub
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        private static readonly ConcurrentDictionary<string, UserHubViewModel> Users =
            new ConcurrentDictionary<string, UserHubViewModel>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly ILog _logger = LogProvider.For<NotificationHub>();

        private IDashboardService _dashboardService;
        public NotificationHub(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //private NotifEntities context = new NotifEntities();

        //Logged Use Call
        public void GetNotification()
        {
            try
            {
                string loggedUser = CurrentUserName;

                //Get TotalNotification
                NotificationData totalNotif = LoadNotifData(loggedUser);

                //Send To
                UserHubViewModel receiver;
                if (Users.TryGetValue(loggedUser, out receiver))
                {
                    var cid = receiver.ConnectionIds.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.Client(cid).broadcaastNotif(totalNotif);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("GetNotification Error", ex);
            }
        }

        protected string CurrentUserName
        {
            get
            {
                string userName = string.Empty;
                List<Claim> nameIdentifierClaims = new List<Claim>();

                if (CurrentUserClaimsIdentity != null)
                {
                    userName = CurrentUserClaimsIdentity.GetUserName();
                    userName = userName.Substring(userName.IndexOf(@"\") + 1);

                    if (string.IsNullOrEmpty(userName))

                        nameIdentifierClaims = CurrentUserClaimsIdentity.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).ToList<Claim>();
                    if (nameIdentifierClaims != null && nameIdentifierClaims.Count > 0)
                    {
                        userName = nameIdentifierClaims.LastOrDefault().Value;
                    }
                }
                return userName;
            }
        }

        protected ClaimsIdentity CurrentUserClaimsIdentity
        {
            get
            {
                //for windows we need to get from other identities
                ClaimsIdentity claimIdentity = GetClaimIdentity();
                if (claimIdentity == null)
                    return Context.User.Identity as ClaimsIdentity;
                else
                    return claimIdentity;
            }
        }


        private ClaimsIdentity GetClaimIdentity()
        {
            var claimPrinciple = (ClaimsPrincipal)Context.User;
            foreach (var claimIdentity in claimPrinciple.Identities)
            {
                if (!(claimIdentity is System.Security.Principal.WindowsIdentity))
                {
                    if (claimIdentity is ClaimsIdentity)
                    {
                        return claimIdentity;
                    }
                }
            }
            return null;
        }

        
        //Specific User Call
        public static void SendNotification(NotificationInfo notificationInfo)
        {
            try
            {
                //Get TotalNotification
              //  string totalNotif = LoadNotifData(notificationInfo.SentTo);

                //Send To
                UserHubViewModel receiver;
                if (Users.TryGetValue(notificationInfo.SentTo.ToString(), out receiver))
                {
                    var cid = receiver.ConnectionIds.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.Client(cid).broadcaastNotif(notificationInfo.TotalNotificationCount);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("SendNotification Error", ex);
            }
        }

        public void SendDocumentOverriddenBySuperUserMessage(List<NotificationUnlockData >notificationUnlockData)
        {
            try
            {
                foreach (var unlockData in notificationUnlockData)
                {
                    UserHubViewModel receiver;
                    if (Users.TryGetValue(unlockData.toUser, out receiver))
                    {
                        var cid = receiver.ConnectionIds.FirstOrDefault();
                        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                        context.Clients.Client(cid).broadcastDocumentOverriddenBySuperUser(unlockData.viewName, unlockData.sectionName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("SendNotification Error", ex);
            }
        }

        private NotificationData LoadNotifData(string userId)
        {
            int userid = GetUserID(userId);
            if (userid != 0)
            {

                return GetNotificationCount(userid);
              //  return cnt.ToString();
            }

            return new NotificationData { total = 0 };
        }

        public override Task OnConnected()
        {
            string userName = CurrentUserName;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new UserHubViewModel
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userName);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                string userName = CurrentUserName;
                string connectionId = Context.ConnectionId;

                UserHubViewModel user;
                Users.TryGetValue(userName, out user);

                if (user != null)
                {
                    lock (user.ConnectionIds)
                    {
                        user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                        if (!user.ConnectionIds.Any())
                        {
                            UserHubViewModel removedUser;
                            Users.TryRemove(userName, out removedUser);
                            Clients.Others.userDisconnected(userName);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.ErrorException("OnDisconnect Error", ex);

            }

            return base.OnDisconnected(stopCalled);
        }


        public  NotificationData GetNotificationCount(int userid)
        {
            List<Notificationstatus> notificationDataList = _unitOfWork.RepositoryAsync<Notificationstatus>().Query()
                                        .Filter(x => x.Userid == userid && x.IsRead == false).Get().ToList().OrderBy(m=>m.Id).ToList();

            var data = new NotificationData
            {
                total = notificationDataList.Count()
            };

            if (data.total>0)
            {
                data.message = notificationDataList.LastOrDefault().Message;
            }
            return data;
        }
        public int GetUserID(string username)
        {
            User user = _unitOfWork.RepositoryAsync<User>().Query().Filter(x => x.UserName == username).Get().FirstOrDefault();
            if (user == null)
                return 0;

            return user.UserID;
        }

    }

    public class NotificationUnlockData
    {
        public string toUser { get; set; }
        public string viewName { get; set; }
        public string sectionName { get; set; }
    }

}
