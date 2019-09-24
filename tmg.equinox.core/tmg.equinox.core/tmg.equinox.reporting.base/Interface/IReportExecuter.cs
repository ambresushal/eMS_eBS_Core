using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.repository.models;
using tmg.equinox.reporting.Base.SQLDataAccess;

namespace tmg.equinox.reporting.Interface
{
    public interface IReportExecuter<Input>: IDisposable
    {
        string PrepareSQLStatement(ReportSetting reportSetting, Input input);

        IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dataSet);

        void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex);

        void AddtechincalLogInSheet(ICollection<DataHolder> dataSet);
        void UpdateResultInQueue(Base.Interface.IResult result, Input input);
    }
}
