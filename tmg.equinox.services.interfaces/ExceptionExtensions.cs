using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.interfaces
{  
  public static class ExceptionExtensions
  {
      public static ServiceResult ExceptionMessages(this Exception exception)
     {
         ServiceResult result = new ServiceResult();
         IList<ServiceResultItem> resulItems = new List<ServiceResultItem>();
         List<string> messages = new List<string>();
         ServiceResultItem resultitem = new ServiceResultItem();
         Exception ex = exception;
         do
         {
                messages.Add(ex.Message);                
                ex = ex.InnerException;
         }
         while (ex != null);

         resultitem.Messages = messages.ToArray();
         resulItems.Add(resultitem);
         result.Items = resulItems;
         result.Result = ServiceResultStatus.Failure;
         return result;
        }     
    }
}
