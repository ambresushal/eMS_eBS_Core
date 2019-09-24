using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbp.logging
{
    public static class LogConfiguration
    {
        public static void ConfigureLog()
        {
            var logConfig = new LoggingConfiguration();

            // add targets
            var dbTarget = new DatabaseTarget();
            dbTarget.ConnectionStringName = "NLog";
            dbTarget.CommandText = @"
                INSERT INTO [dbo].[NLog]
                ([BatchId]
                ,[FormInstanceId]
                ,[FormDesignVersionId]
                ,[QID]
                ,[Level]
                ,[Message]
                ,[Logger]
                ,[Exception]
                ,[TimeStamp])
                    VALUES (
                @batchid,
                @forminstanceid,
                @formdesignversionid,
                @qid,
                @level,
                @message,
                @logger,
                @exception,
                GETDATE()
                )
            ";
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@batchid", "${mdc:item=batchid}"));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@forminstanceid", "${mdc:item=forminstanceid}"));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@formdesignversionid", "${mdc:item=formdesignversionid}"));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@qid", "${mdc:item=qid}"));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@level", "${level}"));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@message", "${message}"));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@logger", "${logger}"));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@exception", "${exception:tostring"));
            var rule = new LoggingRule("*", LogLevel.Debug, dbTarget);
            logConfig.LoggingRules.Add(rule);

            LogManager.Configuration = logConfig;
        }
    }
}
