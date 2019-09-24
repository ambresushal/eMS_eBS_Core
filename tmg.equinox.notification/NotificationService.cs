
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR.Hubs;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using System.Windows.Input;

namespace tmg.equinox.notification
{
    public class NotificationService : INotificationService
    {

        private static readonly ILog _logger = LogProvider.For<NotificationService>();
        private IUnitOfWorkAsync _unitOfWork;
        string serviceMessageKey = "";
        public NotificationService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Pass the Message Key & the paramters to replace with actual values from the message
        /// </summary>
        /// <param name="notificationInfo"></param>
        /// 

        public void SendNotification(NotificationInfo notificationInfo)
        {

            try
            {

                UserViewModel user = new UserViewModel();
                user = (from x in _unitOfWork.RepositoryAsync<User>().Get()
                        where x.UserName == notificationInfo.SentTo
                        select new UserViewModel
                        {
                            UserName = string.Concat(x.FirstName, " ", x.LastName, " ", x.UserName),

                        }).FirstOrDefault();
                string checkAccFoundOrNot = string.Empty;
                bool isPortfolio = true;
                foreach (var item in notificationInfo.ParamterValues)
                {
                    if (item.key == "user")
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            var sentFromUser = (from x in _unitOfWork.RepositoryAsync<User>().Get()
                                                where x.UserName == item.Value
                                                select new UserViewModel
                                                {
                                                    UserName = string.Concat(x.FirstName, " ", x.LastName, " ", x.UserName),

                                                }).FirstOrDefault();
                            item.temp = sentFromUser.UserName;

                        }
                    }
                    if (item.key == "Accountname" && (item.Value != "NA" || item.Value != "" || item.Value != null))
                        isPortfolio = false;
                }
                var message = MessageManager<List<Paramters>>.GetMessage(notificationInfo.MessageKey, notificationInfo.ParamterValues, _unitOfWork);
                //insert the message in notification table
                if (!string.IsNullOrEmpty(notificationInfo.SentTo))
                    notificationInfo.UserID = GetUserID(notificationInfo.SentTo);

                serviceMessageKey = notificationInfo.MessageKey;

                if (isPortfolio)
                    message.MessageText = message.MessageText.Replace("Account {Accountname} and ", "");
                else
                    message.MessageText = message.MessageText.Replace("Account  and ", "");

                message.MessageText = message.MessageText.Replace("[SentTo]", notificationInfo.SentTo);
                bool value = SaveNotification(message.MessageText, notificationInfo.UserID, notificationInfo.loggedInUserName);
                ///get the userId based on UserName

                notificationInfo.TotalNotificationCount = GetNotificationCount(notificationInfo.UserID);

                message.MessageText = message.MessageText.IndexOf("{Section}") != -1 ? message.MessageText.Replace(", Section: {Section}", ""): message.MessageText;
                NotificationHub.SendNotification(notificationInfo);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("SendNotification Error", ex, notificationInfo);
            }
        }
        private int GetUserID(string username)
        {
            User user = _unitOfWork.RepositoryAsync<User>().Query().Filter(x => x.UserName == username).Get().FirstOrDefault();
            if (user == null)
                return 0;

            return user.UserID;
        }
        public bool MarkNotificationToRead(bool viewMode, int CurrentUserId, string CurrentUserName)
        {
            var unReadNotificationdata = _unitOfWork.RepositoryAsync<Notificationstatus>().Query().Filter
                (x => x.Userid == CurrentUserId && x.IsRead == viewMode).Get().ToList();

            foreach (var item in unReadNotificationdata)
            {
                item.IsRead = true;
                this._unitOfWork.RepositoryAsync<Notificationstatus>().Update(item);
                this._unitOfWork.Save();
            }
            var notificationInfo = new NotificationInfo();
            notificationInfo.UserID = CurrentUserId;
            notificationInfo.TotalNotificationCount = GetNotificationCount(notificationInfo.UserID);
            notificationInfo.SentTo = CurrentUserName;
            NotificationHub.SendNotification(notificationInfo);

            return true;
        }

        public NotificationData GetNotificationCount(int userid)
        {
            List<Notificationstatus> notificationDataList = _unitOfWork.RepositoryAsync<Notificationstatus>().Query()
                                 .Filter(x => x.Userid == userid && x.IsRead == false).Get().ToList().OrderBy(m => m.Id).ToList();

            var data = new NotificationData
            {
                total = notificationDataList.Count()
            };

            if (data.total > 0)
            {
                data.message = notificationDataList.LastOrDefault().Message;
            }
            return data;
        }
        public Boolean SaveNotification(string Message, int SendToUserId, string loggedinUserName)
        {



            Notificationstatus notificationDataObj;
            notificationDataObj = new Notificationstatus();
            notificationDataObj.Message = Message;
            notificationDataObj.MessageKey = serviceMessageKey;
            notificationDataObj.Userid = Convert.ToInt32(SendToUserId);
            notificationDataObj.IsRead = false;
            notificationDataObj.IsActive = true;
            notificationDataObj.AddedBy = loggedinUserName;// GetUserNameByUserId(Convert.ToInt32(CurrentUserId));
            notificationDataObj.AddedDate = DateTime.Now;
            notificationDataObj.UpdatedBy = loggedinUserName;// GetUserNameByUserId(Convert.ToInt32(CurrentUserId));
            notificationDataObj.UpdatedDate = DateTime.Now;
            this._unitOfWork.RepositoryAsync<Notificationstatus>().Insert(notificationDataObj);
            _unitOfWork.Save();

            return true;
        }

    }
}
