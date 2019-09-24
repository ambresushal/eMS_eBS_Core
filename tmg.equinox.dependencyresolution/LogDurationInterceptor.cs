using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;
using tmg.equinox.applicationservices.interfaces;
using Microsoft.Practices.Unity;
using System.Collections.Specialized;
using System.Linq;
using System.Configuration;
using System.Text;

namespace tmg.equinox.dependencyresolution
{
    public class LoggingCallHandler : ICallHandler
    {

        private DateTime startTimestamp;
        private DateTime endTimestamp;
        private ILoggingService loggingService = UnityConfig.container.Resolve<ILoggingService>();

       
        //[DebuggerStepThrough]
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            bool isMethodDurationLogEnabled = false, isParamLogEnabled = false;
            //AuditConfig auditConfig = new AuditConfig();
            isMethodDurationLogEnabled = AuditConfig.EnableMethodDurationAudit;
            isParamLogEnabled = AuditConfig.EnableMethodParametersAudit;
            // Before invoking the method get the timestamp
            startTimestamp = DateTime.Now;

            // Invoke the next handler in the chain
            var result = getNext().Invoke(input, getNext);

            // After invoking the method on the original target
            endTimestamp = DateTime.Now;

            TimeSpan span = (endTimestamp - startTimestamp);

            String format = "";
            //TO DO  : Actual call to log method should be called here
            if (isParamLogEnabled == false)
            {
                format = String.Format("Method {0} duration {1} ", input.MethodBase, span.Seconds + " seconds");
            }
            else
            {
                var count = input.Arguments.Count;
                if (input.Arguments.Count == 0)
                {
                    format = String.Format("Method {0} duration {1} ", input.MethodBase, span.Seconds + " seconds");
                }
                else
                {

                    var stringBuilder = new StringBuilder("{" + input.Arguments.Count + "}");

                    for (int i = input.Arguments.Count - 1; i > 0; i--)
                    {
                        stringBuilder.Insert(0, "{" + input.Arguments[i] + "}, ");
                    }
                    format = String.Format("Method name: {0} duration {1} Method parameters: {2} ", input.MethodBase, span.Seconds + " seconds", stringBuilder);
                }

            }
            
            loggingService.Log(format);
            return result;
        }

        public int Order
        {
            get;
            set;
        }
    }
}
