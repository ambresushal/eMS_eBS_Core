using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.QueryManager
{
    public class QueryManagerViewModel
    {
        public QueryManagerViewModel(string _userQuery, bool _isCommit)
        {
            this.UserQuery = _userQuery;
            this.IsCommit = _isCommit;
        }
        public string UserQuery { get; set; }
        public string ResultComments { get; set; }
        public bool IsCommit { get; set; }
    }
}
