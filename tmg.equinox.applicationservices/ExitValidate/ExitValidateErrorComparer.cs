using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;

namespace tmg.equinox.applicationservices.ExitValidate
{
    public class ExitValidateErrorComparer : EqualityComparer<ErrorsList>
    {
        public override bool Equals(ErrorsList x, ErrorsList y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.COLUMN == y.COLUMN && x.FIELD == y.FIELD && x.QUESTION == y.QUESTION && x.SCREEN == y.SCREEN && x.ERROR == y.ERROR;
        }
        public override int GetHashCode(ErrorsList error)
        {
            if (Object.ReferenceEquals(error, null)) return 0;
            string combine = String.IsNullOrEmpty(error.COLUMN) ? error.COLUMN : "";
            string part = String.IsNullOrEmpty(error.FIELD) ? error.FIELD: "";
            combine = combine + part;
            part = String.IsNullOrEmpty(error.QUESTION) ? error.QUESTION: "";
            combine = combine + part;
            part = String.IsNullOrEmpty(error.SCREEN) ? error.SCREEN: "";
            combine = combine + part;
            part = String.IsNullOrEmpty(error.ERROR) ? error.ERROR : "";
            combine = combine + part;
            return combine.GetHashCode();
        }
    }
}
