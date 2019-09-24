using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbp.logging
{
    public static class AppLogger
    {
        private static readonly CustomLogger _logger = (CustomLogger)LogManager.GetCurrentClassLogger(typeof(CustomLogger));

        public static void SetContext(int batchId, int formInstanceId, int formDesignVersionId, string qid)
        {
            _logger.SetContext(batchId, formInstanceId, formDesignVersionId, qid);
        }
        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Error(Exception ex, string message)
        {
            _logger.Error(ex, message);
        }
    }
}
