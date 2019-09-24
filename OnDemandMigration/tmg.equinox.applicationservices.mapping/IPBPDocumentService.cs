using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public interface IPBPDocumentService
    {
        void UpdateDocumentJSON(int formInstanceId, string documentJSON);
        JObject GetJSONDocument(int formInstanceId);
    }
}
