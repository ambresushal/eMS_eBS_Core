using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.emailnotification.Model
{

   public class EmailResponseData
    {
       public EmailResponseData()
       {
           this.Message = "Success";
       }
       public TaskStatus Status { get; set; }
       public bool IsCompleted { get; set; }
       public Exception Exceptions { get; set; }
       public string Message { get; set; }
    }
}
