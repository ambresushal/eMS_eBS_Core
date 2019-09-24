using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Base.Interface
{
    public interface IMapper<T> where T : class
    {
        T GetMapping(ReportSetting reportConfig);
     
    }
}
