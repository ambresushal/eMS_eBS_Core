using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using tmg.equinox.infrastructure.exceptionhandling;


namespace EquinoxProcessor
{
    public class ScheduleEquinoxService
    {
        public Timer BatchSchedular;
        public Timer IASSchedular;
        public Timer ErrorLogSchedular;
        private DateTime batchSceduleTime = DateTime.MinValue;
        private GlobalUpdateBatchExecution batchExecutionProcessor;
        private bool scheduledResult;

        private string batchServiceMode;
        private string iasServiceMode;
        private string validationServiceMode;

        private int batchScheduleTimeInterval;
        private int iasScheduleTimeInterval;
        private int validationTimeInterval;
        public string batchServiceLogPath;
        public string iasServiceLogPath;
        public string validationServiceLogPath;

        public ScheduleEquinoxService()
        {
            InitializeConfigSetUp();
        }

        public void ScheduleBatchService()
        {
            batchExecutionProcessor = new GlobalUpdateBatchExecution();
            try
            {
                BatchSchedular = new Timer(new TimerCallback(SchedularCallback));
                this.WriteServiceLog(batchServiceLogPath, EquinoxServiceConstant.BatchService + EquinoxServiceConstant.DailyMode + " {0}");
                batchExecutionProcessor.StartBatchExecution();
                scheduledResult = batchServiceMode.ToUpper() == EquinoxServiceConstant.DailyMode ? ScheduleDailyService(BatchSchedular, EquinoxServiceConstant.BatchService, batchSceduleTime, batchServiceLogPath) : ScheduleServiceInterval(BatchSchedular, batchScheduleTimeInterval, EquinoxServiceConstant.BatchService, batchServiceLogPath);
            }
            catch (Exception ex)
            {
                this.WriteServiceLog(batchServiceLogPath, EquinoxServiceConstant.BatchService + " Error {0} " + ex.Message + ex.StackTrace);
                scheduledResult = batchServiceMode.ToUpper() == EquinoxServiceConstant.DailyMode ? ScheduleDailyService(BatchSchedular, EquinoxServiceConstant.BatchService, batchSceduleTime, batchServiceLogPath) : ScheduleServiceInterval(BatchSchedular, batchScheduleTimeInterval, EquinoxServiceConstant.BatchService, batchServiceLogPath);
            }
        }
        private void SchedularCallback(object e)
        {
            this.WriteServiceLog(batchServiceLogPath, EquinoxServiceConstant.BatchService + " {0}");
            this.ScheduleBatchService();
        }

        public void ScheduleIASService()
        {
            batchExecutionProcessor = new GlobalUpdateBatchExecution();
            try
            {
                IASSchedular = new Timer(new TimerCallback(IASSchedularCallback));
                this.WriteServiceLog(iasServiceLogPath, EquinoxServiceConstant.IASService +EquinoxServiceConstant.IntervalMode + " {0}");
                batchExecutionProcessor.StartIASGeneration();
                ScheduleServiceInterval(IASSchedular, iasScheduleTimeInterval, EquinoxServiceConstant.IASService, iasServiceLogPath);
            }
            catch (Exception ex)
            {
                this.WriteServiceLog(iasServiceLogPath, EquinoxServiceConstant.IASService + " Error {0} " + ex.Message + ex.StackTrace);
                ScheduleServiceInterval(IASSchedular, iasScheduleTimeInterval, EquinoxServiceConstant.IASService, iasServiceLogPath);
            }
        }
        private void IASSchedularCallback(object e)
        {
            this.WriteServiceLog(iasServiceLogPath, EquinoxServiceConstant.IASService + " {0}");
            this.ScheduleIASService();
        }

        public void ScheduleErrorLogService()
        {
            batchExecutionProcessor = new GlobalUpdateBatchExecution();
            try
            {
                ErrorLogSchedular = new Timer(new TimerCallback(ErrorLogSchedularCallback));
                this.WriteServiceLog(validationServiceLogPath, EquinoxServiceConstant.ValidationService + EquinoxServiceConstant.IntervalMode + " {0}");
                batchExecutionProcessor.StartErrorLogGeneration();
                ScheduleServiceInterval(ErrorLogSchedular, validationTimeInterval, EquinoxServiceConstant.ValidationService, validationServiceLogPath);

            }
            catch (Exception ex)
            {
                this.WriteServiceLog(validationServiceLogPath, EquinoxServiceConstant.ValidationService + " Error {0} " + ex.Message + ex.StackTrace);
                ScheduleServiceInterval(ErrorLogSchedular, validationTimeInterval, EquinoxServiceConstant.ValidationService,validationServiceLogPath);
            }
        }
        private void ErrorLogSchedularCallback(object e)
        {
            this.WriteServiceLog(validationServiceLogPath, EquinoxServiceConstant.ValidationService + " {0}");
            this.ScheduleErrorLogService();
        }

        private bool ScheduleServiceInterval(Timer scheduler, int intervalMinutes, string ServiceName, string logFilePath)
        {
            try
            {
                DateTime scheduledTime = DateTime.MinValue;
                //Set the Scheduled Time by adding the Interval to Current Time.
                scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                if (DateTime.Now > scheduledTime)
                {
                    //If Scheduled Time is passed set Schedule for the next Interval.
                    scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                }

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                this.WriteServiceLog(logFilePath, ServiceName + "to run after: " + schedule + " {0}");
                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                scheduler.Change(dueTime, Timeout.Infinite);
                scheduledResult = true;

            }
            catch (Exception ex)
            {

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                this.WriteServiceLog(logFilePath, ServiceName + " Error {0} " + ex.Message + ex.StackTrace);
                scheduledResult = false;
            }

            return scheduledResult;
        }
        private bool ScheduleDailyService(Timer scheduler, string ServiceName, DateTime scheduledTime, string logFilePath)
        {
            try
            {
                //DateTime scheduledTime = DateTime.MinValue;
                scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
                if (DateTime.Now > scheduledTime)
                {
                    //If Scheduled Time is passed set Schedule for the next day.
                    scheduledTime = scheduledTime.AddDays(1);
                }
                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                this.WriteServiceLog(logFilePath, ServiceName + "scheduled to run after: " + schedule + " {0}");
                //this.WriteToIASFile("Simple Service scheduled to run after: " + schedule + " {0}");
                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);
                scheduler.Change(dueTime, Timeout.Infinite);
                scheduledResult = true;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                this.WriteServiceLog(logFilePath, ServiceName + "Error {0} " + ex.Message + ex.StackTrace);
                scheduledResult = false;
            }

            return scheduledResult;
        }
        public void WriteServiceLog(string filePath, string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                        writer.Close();
                        writer.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
        }
        private void InitializeConfigSetUp()
        {
            //Initizalize Service Mode
            batchServiceMode = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Mode"]) ? "Daily" : System.Configuration.ConfigurationManager.AppSettings["Mode"];
            iasServiceMode = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["IASMode"]) ? "Interval" : System.Configuration.ConfigurationManager.AppSettings["IASMode"];
            validationServiceMode = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ErrorLogMode"]) ? "Interval" : System.Configuration.ConfigurationManager.AppSettings["ErrorLogMode"];
            //Initialize ScheduleTime
            batchSceduleTime = (batchServiceMode.ToUpper() == "DAILY") ? DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]) : batchSceduleTime;
            if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["IntervalMinutes"], out batchScheduleTimeInterval))
            {
                batchScheduleTimeInterval = 2;
            }

            if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["IASIntervalMinutes"], out iasScheduleTimeInterval))
            {
                iasScheduleTimeInterval = 2;
            }

            if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["ErrorLogIntervalMinutes"], out validationTimeInterval))
            {
                validationTimeInterval = 2;
            }

            //Initialize Log file Path
            batchServiceLogPath = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ServiceLogFilePath"]) ? string.Empty : System.Configuration.ConfigurationManager.AppSettings["ServiceLogFilePath"];
            iasServiceLogPath = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["IASServiceLogFilePath"]) ? string.Empty : System.Configuration.ConfigurationManager.AppSettings["IASServiceLogFilePath"];
            validationServiceLogPath = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ErrorLogServiceLogFilePath"]) ? string.Empty : System.Configuration.ConfigurationManager.AppSettings["ErrorLogServiceLogFilePath"];
        }
    }

    internal class EquinoxServiceConstant
    {
        public const string BatchService = "Batch Service :";
        public const string IASService = "IAS Service :";
        public const string ValidationService = "Validation Service :";
        public const string DailyMode = "DAILY";
        public const string IntervalMode = "INTERVAL";
    }
}
