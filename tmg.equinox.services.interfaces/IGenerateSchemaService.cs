using System.Collections.Generic;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IGenerateSchemaService
    { 
        bool Run(List<JsonDesign> jsonDesign);
    }
}
