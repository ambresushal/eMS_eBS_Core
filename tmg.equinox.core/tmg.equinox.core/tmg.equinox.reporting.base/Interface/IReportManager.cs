using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting.Interface;

namespace tmg.equinox.reporting.Base.Interface
{
    public interface IReportManager<Input>
    {
        IResult Execute(Input queueId, IReportExecuter<Input> reportExecuter);
        //IReportExecuter<Input> ReportExecuter { get; set; }
    }
}
