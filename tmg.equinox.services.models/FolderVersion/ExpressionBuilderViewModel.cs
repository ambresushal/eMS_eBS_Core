using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
   public class ExpressionBuilderViewModel
    {
        public string TargetPath { get; set; }
        public JToken Data { get; set; }
    }
}
