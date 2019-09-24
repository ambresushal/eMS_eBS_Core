using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.ExitValidate
{
    public class ExitValidateResultComparer : EqualityComparer<ExitValidateResult>
    {
        public override bool Equals(ExitValidateResult x, ExitValidateResult y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.PBPCOLUMN == y.PBPCOLUMN && x.PBPFIELD == y.PBPFIELD && x.Section == y.Section && x.Screen == y.Screen && x.Question == y.Question && x.Error == y.Error;
        }

        public override int GetHashCode(ExitValidateResult error)
        {
            if (Object.ReferenceEquals(error, null)) return 0;
            string combine = String.IsNullOrEmpty(error.PBPCOLUMN) ? error.PBPCOLUMN : "";
            string part = String.IsNullOrEmpty(error.PBPFIELD) ? error.PBPFIELD : "";
            combine = combine + part;
            part = String.IsNullOrEmpty(error.Section) ? error.Section : "";
            combine = combine + part;
            part = String.IsNullOrEmpty(error.Screen) ? error.Screen : "";
            combine = combine + part;
            part = String.IsNullOrEmpty(error.Question) ? error.Question : "";
            combine = combine + part;
            part = String.IsNullOrEmpty(error.Error) ? error.Error : "";
            combine = combine + part;
            return combine.GetHashCode();
        }
    }
}
