using System;
using System.Collections.Generic;
using System.Linq;

namespace tmg.equinox.web.Validator
{
    public class ValidationMessage
    {
        public static readonly string requiredMsg = "{0} is required.";
        public static readonly string regexMsg = "{0} does not have correct format.";
        public static readonly string incorrectValueMsg = "Select One is an incorrect value for {0}.";
        public static readonly string selectOne = "[Select One]";
        public static readonly string invalidIntMsg = "{0} is not a valid number.";
        public static readonly string invalidFloatMsg = "{0} is not a valid decimal number.";
        public static readonly string duplicateMsg = "Value in {0} already exists.";
        public static readonly string ruleErrorMsg = "{0} rule failed.";
        public static readonly string invalidDateMsg = "{0} does not have a valid date format.";
        public static readonly string nonZeroMsg = "Value in {0} should not be zero.";
		public static readonly string incorrectSelValueMsg = "Selected value for {0} is an incorrect. Please select valid value.";
    }
}
