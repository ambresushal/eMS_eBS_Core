using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using tmg.equinox.applicationservices.interfaces;
using Microsoft.Practices.Unity;

namespace tmg.equinox.dependencyresolution
{
    public class CommandInterceptor : DbCommandInterceptor
    {
        private ILoggingService loggingService = UnityConfig.container.Resolve<ILoggingService>();
        private delegate void ExecutingMethod<T>(System.Data.Common.DbCommand command, DbCommandInterceptionContext<T> interceptionContext);
        public override void NonQueryExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            CommandExecuting<int>(base.NonQueryExecuting, command, interceptionContext);
        }

        public override void ReaderExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            CommandExecuting<System.Data.Common.DbDataReader>(base.ReaderExecuting, command, interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            // TODO: Finding better solution instead of stopwatch.
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                base.ScalarExecuting(command, interceptionContext);
                sw.Stop();
                WriteToLog(command, interceptionContext, sw);
            }
            catch (Exception e)
            {
                    
            }
        }

        private void CommandExecuting<T>(ExecutingMethod<T> executingMethod, System.Data.Common.DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            try
            {
                // TODO: Finding better solution instead of stopwatch.
                Stopwatch sw = Stopwatch.StartNew();
                executingMethod.Invoke(command, interceptionContext);
                sw.Stop();
                WriteToLog(command, interceptionContext, sw);
            }
            catch (Exception e)
            {
                    
            }
        }

        /// <summary>
        /// Write QueryExecution to Log.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        /// <param name="sw"></param>
        private void WriteToLog<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext, Stopwatch sw)
        {
            loggingService.LogQueryExecution(String.Format("Query {0} duration {1} ", command.CommandText, sw.Elapsed), interceptionContext.Exception);
                
        }
    }
}

