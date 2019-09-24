using System.Collections.Generic;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;

namespace tmg.equinox.schema.Base.Interface
{
    public interface IGenerateSchema
    {
        void CreateSchema(List<ReportingTableInfo> tables);
    }
}
