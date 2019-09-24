using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016.Validation
{
    public static class QHPValidationErrorListExtensions
    {
        public static void AddError(this List<QHPValidationError> list, string errorField, string errorMessage)
        {
            QHPValidationError error = new QHPValidationError();
            if (errorField == "")
            {
                errorField = " ";
            }
            error.ErrorMessage = errorField + " - " + errorMessage;
            list.Add(error);
        }

        public static void AddError(this List<QHPValidationError> list, string errorMessage)
        {
            QHPValidationError error = new QHPValidationError();
            error.ErrorMessage = errorMessage;
            list.Add(error);
        }
    }
}
