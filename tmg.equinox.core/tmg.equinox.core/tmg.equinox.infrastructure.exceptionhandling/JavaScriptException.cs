using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.infrastructure.exceptionhandling
{
    public class JavaScriptException : Exception
    {
        #region Private Members
        string message;
        int statusCode;
        string url;
        #endregion Private Members

        #region Properties
        public override string Message
        {
            get
            {
                if (message.Contains(": at document path "))
                {
                    return message.Substring(0, message.IndexOf(": at document path "));
                }
                return message;
            }
        }
        #endregion Properties

        #region Constructor
        public JavaScriptException(string message)
            : base(message)
        {
            this.message = message;
        }
      

        public JavaScriptException(string message, string source, string statusCode)
            : base(message)
        {
          
            this.Source = source;
            this.message = message;
            this.statusCode = 0;
        }
        #endregion Constructor

        #region  Public Methods
        public override string ToString()
        {
            return message;
        }
        public  string Status()
        {
            return message;
        }
        #endregion Public Methods
    }
}
