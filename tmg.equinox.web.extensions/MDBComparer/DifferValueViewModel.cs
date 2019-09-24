using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.MDBComparer
{
   public class DifferValueViewModel
    {
        public string QID { get; set; }
        public string FieldName { get; set; }
        public string TableName { get; set; }
        public string DocumentPath { get; set; }
        public string BaselineValue { get; set; }
        public string inPorgressValue { get; set; }
        public int BaseLineFormInstanceId { get; set; }
        public int inProgressFormInstanceId { get; set; }
    }
}
