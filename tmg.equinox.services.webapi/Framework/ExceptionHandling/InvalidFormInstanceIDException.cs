using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.webapi.Framework
{
    public class InvalidFormInstanceIDException : Exception
    {
        public InvalidFormInstanceIDException(string message)
            : base(message)
        {
        }
    }
}