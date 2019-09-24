using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.webapi.Framework
{
    public static class ExceptionMessages
    {
        public const string FormInstanceIDIsNull = "FromInstanceID can not be null or 0.";
        public const string FormInstanceIDNotFound = "FromInstanceID {0} does not exist.";
        public const string InvalidFormInstanceID = "FromInstanceID {0} is not associated with the given document type.";
    }
}