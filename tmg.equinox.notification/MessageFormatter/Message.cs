using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.notification
{
    public class Message: ICloneable
    {
        public int MessageID { get; set; }
        public string MessageKey { get; set; }  
        public string MessageText { get; set; }
        public string MessageType { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class Paramters
    {
        public string key { get; set; }
        public string Value { get; set; }
        public string temp { get; set; }
    }

}
