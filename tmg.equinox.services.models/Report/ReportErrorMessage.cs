using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
    public class ReportErrorMessage:Exception
    {
        public ReportErrorMessage(string ErrorMessage)
            : base(ErrorMessage)
        {
        }

        public ReportErrorMessage(string ErrorMessage, Exception InnerException)
            :base(ErrorMessage,InnerException)
        {
        }


    }
}
