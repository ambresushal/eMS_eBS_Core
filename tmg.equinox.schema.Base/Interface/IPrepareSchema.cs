using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.Base.Interface
{
    public interface IPrepareSchema
    {
        void PrepareSchema();
        List<ReportingTableInfo> GetSchema();
    }
}
