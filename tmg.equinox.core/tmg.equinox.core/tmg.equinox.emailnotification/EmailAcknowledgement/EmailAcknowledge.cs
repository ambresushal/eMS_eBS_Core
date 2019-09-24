using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.emailnotification.Model;

namespace tmg.equinox.emailnotification.EmailAcknowledgement
{
   public class EmailAcknowledge
    {
       EmailResponseData acknowledgement = new EmailResponseData();

       public EmailResponseData GetAcknowledgement(Task response)
       {
           acknowledgement.IsCompleted = response.IsCompleted;
           acknowledgement.Exceptions = response.Exception;
           acknowledgement.Status = response.Status;
           return acknowledgement;
       }
    }
}
