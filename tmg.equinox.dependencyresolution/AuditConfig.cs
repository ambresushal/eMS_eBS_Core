using System;
using System.Collections.Specialized;
using System.Configuration;

namespace tmg.equinox.dependencyresolution
{
    public static class AuditConfig
    {
        private static readonly NameValueCollection auditLogConfigurationSection;
        private static readonly bool enableMethodDurationAudit;
        private static readonly bool enableMethodParameterAudit;
        private static readonly bool enableAuditThroughInterception;
        private static readonly bool enableQueryExecutionLog;
        private static readonly string auditQueryLogPath;

        static AuditConfig()
        {

            auditLogConfigurationSection = (NameValueCollection)ConfigurationManager.GetSection("audit");
            enableMethodDurationAudit = Convert.ToBoolean(auditLogConfigurationSection["EnableMethodDurationAudit"]);
            enableMethodParameterAudit = Convert.ToBoolean(auditLogConfigurationSection["EnableMethodParametersAudit"]);
            enableAuditThroughInterception = Convert.ToBoolean(auditLogConfigurationSection["EnableAuditThroughInterception"]);
            enableQueryExecutionLog = Convert.ToBoolean(auditLogConfigurationSection["EnableEntityFrameworkQueryLog"]);
            auditQueryLogPath = Convert.ToString(auditLogConfigurationSection["AuditQueryLogPath"]);
        }


        public static bool EnableMethodDurationAudit
        {
            get
            {
                return enableMethodDurationAudit;
            }
        }

        public static bool EnableMethodParametersAudit
        {
            get
            {
                return enableMethodParameterAudit;
            }
        }

        public static bool EnableAuditThroughInterception
        {
            get
            {
                return enableAuditThroughInterception;
            }
        }

        public static bool EnableEntityFrameworkQueryLog
        {
            get
            {
                return enableQueryExecutionLog;
            }
        }

        public static string AuditQueryLogPath
        {
            get
            {
                return auditQueryLogPath;
            }
        }
    }
}
