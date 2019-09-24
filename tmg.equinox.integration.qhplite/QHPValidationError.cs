using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite
{
    public class QHPValidationError
    {
        public string ErrorCode
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public string ErrorType
        {
            get;
            set;
        }

        public QHPValidationError()
        {
        }
    }
}