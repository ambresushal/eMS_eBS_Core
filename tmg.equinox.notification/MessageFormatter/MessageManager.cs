using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.notification
{
    /// <summary>
    /// create a Message 
    /// Usage:
    ///   List<Paramters> paramater = new List<Paramters>();
    ///        paramater.Add(new Paramters { key = "user", Value = "Nitin" });
    ///       paramater.Add(new Paramters { key = "taskName", Value = "Task" });
    ///
    ///        var message1 = MessageManager<List<Paramters>>.GetMessage(MessageKey.TASK_COMPLETED, paramater);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageManager<T>
    {
        public static Message GetMessage(string messageKey, T parameter, IUnitOfWorkAsync unitOfWork)
        {
            IMessageBuilder messsageBuilder= new DataBaseMessageBuilder<T>(messageKey, parameter, unitOfWork);            
            return messsageBuilder.GetMessage();
        }
    }
}
