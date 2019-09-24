using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.schema.Base.Interface
{
    public interface IJSchema
    {
        JToken Get();
    }
}
