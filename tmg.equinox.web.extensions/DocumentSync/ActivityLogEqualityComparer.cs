using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.documentcomparer.RepeaterCompareUtils
{
    public class ActivityLogEqualityComparer : EqualityComparer<ActivityLogModel>
    {
        public override bool Equals(ActivityLogModel x, ActivityLogModel y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Field == y.Field && x.ElementPath== y.ElementPath;
        }
        public override int GetHashCode(ActivityLogModel product)
        {
            if (Object.ReferenceEquals(product, null)) return 0;
            int hashProductName = product.Field == null ? 0 : product.Field.GetHashCode();
            int hashProductCode = product.ElementPath.GetHashCode();
            return hashProductName ^ hashProductCode;
        }
    }
}
