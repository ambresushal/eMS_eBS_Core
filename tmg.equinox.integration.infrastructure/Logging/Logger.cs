using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
namespace tmg.equinox.integration.infrastructure.logging
{
    public class Logger : ILog
    {
        #region Enum Members
        enum listnerNames
        {
            FlatFileErrorTraceListener,
            FlatFileMethodDurationTraceListener
        };
        #endregion Enum Members

        #region Private Members
        private const string fileExtension = ".log";
        private const string logDirectory = "Logs";
        private static string configuredFileName = string.Empty;
        private static string loggerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logDirectory);
        private static LogWriter _defaultWriter = null;
        private Thread _loggingThread;
        private static int _asyncLogFileAppenderCount = 0;
        #endregion Private Members

        #region Public Members
        public object _message { get; set; }
        public string _customAttributes { get; set; }
        #endregion Public Members

        #region Constructor
        /// <summary>
        /// Intialize the LogWriter object
        /// 
        /// </summary>
        static Logger()
        {
            /* Supressing fetching the DefaultConfiguration settings */
            //LogWriterFactory logWriterFactory = new LogWriterFactory();
            //if (_defaultWriter == null)
            //    _defaultWriter = logWriterFactory.Create();
            ////_defaultWriter = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
            /* End */

            LogWriterFactory logWriterFactory = new LogWriterFactory(GetConfigSource());
            if (_defaultWriter == null)
                _defaultWriter = logWriterFactory.Create();
        }
        #endregion Constructor

        #region Public Methods
        public void Info()
        {
            WriteToLog(Convert.ToString(_message), "Audit");
        }


        public void Warn(object message)
        {
            throw new NotImplementedException();
        }

        public void Error()
        {
            //Logger.Write(Convert.ToString(_message));
            WriteToLog(Convert.ToString(_message), "Error");
        }

        public void Fatal(object message)
        {
            throw new NotImplementedException();
        }

        public void Debug(object message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Info(object message, Exception ex)
        {
            if (ex == null)
            {
                //Logger.Write(Convert.ToString(message));

                WriteToLog(Convert.ToString(message), "Audit");
            }
            else
            {
                // Logger.Write(Convert.ToString(message));

                WriteToLog(Convert.ToString(ex), "Audit");
            }

        }

        public void Warn(object message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Error(object message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Fatal(object message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void AsyncError(object message, string customAttributes = "")
        {
            _message = message;
            _customAttributes = customAttributes;
            ThreadStart threadDelegate = new ThreadStart(Error);
            Thread newThread = new Thread(threadDelegate)
            {
                IsBackground = true,
                Name = "AsyncLogThread-" + Interlocked.Increment(ref _asyncLogFileAppenderCount),
            };
            newThread.Start();
        }

        public void AsyncInfo(object message, string customAttributes = "")
        {
            _message = message;
            ThreadStart threadDelegate = new ThreadStart(Info);
            Thread newThread = new Thread(threadDelegate) { IsBackground = true, Name = "AsyncLogThread-" + Interlocked.Increment(ref _asyncLogFileAppenderCount), };
            newThread.Start();
        }


        public void Info(object message)
        {
            WriteToLog(Convert.ToString(message), "Audit");
        }

        public void Error(object message)
        {
            //Logger.Write(Convert.ToString(_message));
            WriteToLog(Convert.ToString(message), "Error");
        }

        public void Debug(object message)
        {
            throw new NotImplementedException();
        }
        #endregion Public Methods

        #region Private Methods


        /// <summary>
        ///  /**File Read , Change configuration information, return modified ConfigSource object **/
        /// </summary>
        /// <returns></returns>

        private static IConfigurationSource GetConfigSource()
        {

            IConfigurationSource source = null;
            source = new FileConfigurationSource(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            var logginConfigurationSection = (LoggingSettings)source.GetSection("loggingConfiguration");

            //Audit
            var auditTraceListernData = logginConfigurationSection.TraceListeners.Get(listnerNames.FlatFileMethodDurationTraceListener.ToString()) as RollingFlatFileTraceListenerData;
            if (auditTraceListernData != null)
            {
                auditTraceListernData.FileName = SetFilePath(auditTraceListernData.FileName);
            }
            //Error
            var errorTraceListernData = logginConfigurationSection.TraceListeners.Get(listnerNames.FlatFileErrorTraceListener.ToString()) as RollingFlatFileTraceListenerData;
            if (errorTraceListernData != null)
            {
                errorTraceListernData.FileName = Convert.ToString(ConfigurationManager.AppSettings["ErrorlogPath"]); //SetFilePath(errorTraceListernData.FileName);
            }
            return source;
        }

        /// <summary>
        /// Setting the Path of file in Application Base Directory
        /// </summary>
        /// <param name="fileNamefromConfig"></param>
        /// <returns></returns>
        private static string SetFilePath(string fileNamefromConfig)
        {
            if (validateFileExtension(Path.GetFileName(fileNamefromConfig)))
            {
                configuredFileName = Path.Combine(loggerPath, Path.GetFileName(fileNamefromConfig));
            }
            else
            {
                configuredFileName = Path.Combine(loggerPath, (Path.ChangeExtension(Path.GetFileName(fileNamefromConfig), fileExtension)));
            }
            return configuredFileName;
        }

        /** Validates for file extension **/
        private static bool validateFileExtension(string path)
        {
            var extension = Path.GetExtension(path);
            if (extension.ToLower() == fileExtension)
                return true;
            else
                return false;
        }

        private void WriteToLog(string message, string category)
        {
            if (_defaultWriter.IsLoggingEnabled())
            {
                if (_customAttributes != null)
                    _defaultWriter.Write(_customAttributes, category);
                else
                    _defaultWriter.Write(message, category);
            }
        }
        #endregion Private Methods

    }

}


