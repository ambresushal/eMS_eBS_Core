using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.notification
{
    public class BaseMesssageBuilder : IMessageBuilder
    {
                                 
        private IUnitOfWorkAsync _unitOfWork;
        private string _messagekey;
        protected Message _message;
        private IDashboardService _dashboardService;
        public List<Message> _messageCollection = new List<Message>();
        public BaseMesssageBuilder(string messagekey,IUnitOfWorkAsync unitOfWork)
        {            
            _messagekey = messagekey;
            _unitOfWork = unitOfWork;
            getMessageFromDB();
            _message = new Message();
            var message = _messageCollection.Where(m => m.MessageKey == messagekey).FirstOrDefault();
            _message = (Message)message.Clone();
        }
        
       private IList<Message> getMessageFromDB()
        {
            var ckey = "Messages";
            _messageCollection = GlavCacheWrapper.Get<List<Message>>(ckey);
            
            if (_messageCollection == null)
            {
                _messageCollection = (from i in _unitOfWork.RepositoryAsync<MessageData>().Get()
                                      select new Message
                                      {
                                          MessageID = i.MessageID,
                                          MessageKey = i.MessageKey,
                                          MessageText = i.MessageText,
                                          MessageType = i.MessageType
                                      }).ToList();

                GlavCacheWrapper.Add<List<Message>>(ckey,_messageCollection); 
            }
            //_messageCollection.Add(msg);
            //var msg2 = new Message { MessageType = "Task", MessageText = "New  {taskName} Completed By  {user}", MessageKey = MessageKey.TASK_COMPLETED };
            //_messageCollection.Add(msg2);
            //var msg3 = new Message { MessageType = "Lock", MessageText = "Locked by {user}", MessageKey = MessageKey.LOCK };
            //_messageCollection.Add(msg3);


            // get from datbase , put in cache and _messageCollection
            //_messageCollection 
            return _messageCollection;
        }
        public Message GetMessage()
        {
            FormatMessage();
            return _message;
        }

        public virtual Message FormatMessage()
        {
            return _message;
        }
        protected bool TryCast<T>(object obj, out T result)
        {
            if (obj is T)
            {
                result = (T)obj;
                return true;
            }

            result = default(T);
            return false;
        }
    }
}
