using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting.Base.SQLDataAccess;

namespace tmg.equinox.reporting.Base.Interface
{
    public interface IDbRepostory
    {
        ICollection<DataHolder> Get(string sqlStatment);
        DataSet GetData(string sqlStatment);
    }
}
