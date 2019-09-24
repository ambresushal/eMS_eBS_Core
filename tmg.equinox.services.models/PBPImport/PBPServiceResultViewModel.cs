using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class PBPServiceResultViewModel:ViewModelBase
    {
        public String ErrorMsg { get; set; }
        public int PBPErrorCode { get; set; }
    }

    public enum PBPErrorCode
    {
        SchemeIsNotValid=1,
        MisMatchQID=2,

    }
}
