using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Interface;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Interface
{
    public interface IReportWIthMapExecuter<Input>: IReportExecuter<Input>
    {
        
        void ApplyRule(KeyValuePair<string, object> row, ReportMappingField mapping);

        void WriteInContainer(KeyValuePair<string, object> row, ReportMappingField mapping);

    }
}
