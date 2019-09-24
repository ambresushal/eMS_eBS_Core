using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace CollateralService
{
    public class ScheduleEquinoxService
    {
        #region Private Memebers
        public Timer CollateralSchedular;
        public Timer PBPImportSchedular;
        private string collateralServiceMode;
        private int collateralScheduleTimeInterval;
        public string collateralServiceLogPath;
        private string pbpImportServiceMode;
        private int pbpImportScheduleTimeInterval;
        public string pbpImportServiceLogPath;
        private DateTime batchSceduleTime = DateTime.MinValue;
        private ServiceBatchExecution batchExecutionProcessor;
        private ServiceBatchExecution batchPBPExecutionProcessor;
        bool isPBPProcessCompleted = false;
        #endregion

        #region Constructors
        public ScheduleEquinoxService()
        {
            InitializeConfigSetUp();
            isPBPProcessCompleted = true; 
        }
        #endregion

        #region Public Methods
        public void ScheduleCollateralService()
        {
            batchExecutionProcessor = new ServiceBatchExecution();
          
            try
            {
                CollateralSchedular = new Timer(new TimerCallback(CollateralCallBack));
                this.WriteServiceLog(collateralServiceLogPath, EquinoxServiceConstant.CollateralService + EquinoxServiceConstant.IntervalMode + " {0}");
                batchExecutionProcessor.StartCollateralGeneration();
                ScheduleServiceInterval(CollateralSchedular, collateralScheduleTimeInterval, EquinoxServiceConstant.CollateralService, collateralServiceLogPath);
            }
            catch (Exception ex)
            {
                this.WriteServiceLog(collateralServiceLogPath, EquinoxServiceConstant.CollateralService + " Error {0} " + ex.Message + ex.StackTrace);
                ScheduleServiceInterval(CollateralSchedular, collateralScheduleTimeInterval, EquinoxServiceConstant.CollateralService, collateralServiceLogPath);
            }
        }
        public void SchedulePBPImportService()
        {
            bool allowForProcess = false;
            if (batchPBPExecutionProcessor == null)
            {
                batchPBPExecutionProcessor = new ServiceBatchExecution();
                allowForProcess = true;
            }
            else
            {
                if (batchPBPExecutionProcessor.IspbpImportProcessCompleted)
                {
                    batchPBPExecutionProcessor = new ServiceBatchExecution();
                    allowForProcess = true;
                }
            }
            try
            {
                PBPImportSchedular = new Timer(new TimerCallback(PBPImportCallBack));
                this.WriteServiceLog(pbpImportServiceLogPath, EquinoxServiceConstant.PBPImportService + EquinoxServiceConstant.IntervalMode + " {0}");
                if (allowForProcess)
                {
                    batchPBPExecutionProcessor.StartPBPImportGeneration();
                }
                ScheduleServiceInterval(PBPImportSchedular, pbpImportScheduleTimeInterval, EquinoxServiceConstant.PBPImportService, pbpImportServiceLogPath);
            }
            catch (Exception ex)
            {
                this.WriteServiceLog(pbpImportServiceLogPath, EquinoxServiceConstant.PBPImportService + " Error {0} " + ex.Message + ex.StackTrace);
                ScheduleServiceInterval(PBPImportSchedular, pbpImportScheduleTimeInterval, EquinoxServiceConstant.PBPImportService, pbpImportServiceLogPath);
            }
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
                //bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
        }
        #endregion

        #region Private Methods
        private void CollateralCallBack(object e)
        {
            this.WriteServiceLog(collateralServiceLogPath, EquinoxServiceConstant.CollateralService + " {0}");
            this.ScheduleCollateralService();
        }
        private void PBPImportCallBack(object e)
        {
            this.WriteServiceLog(pbpImportServiceLogPath, EquinoxServiceConstant.PBPImportService + " {0}");
            this.SchedulePBPImportService();
        }
        private bool ScheduleServiceInterval(Timer scheduler, int intervalMinutes, string ServiceName, string logFilePath)
        {
            bool scheduledResult;
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

                //bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                this.WriteServiceLog(logFilePath, ServiceName + " Error {0} " + ex.Message + ex.StackTrace);
                scheduledResult = false;
            }

            return scheduledResult;
        }
        private void InitializeConfigSetUp()
        {
            //Initizalize Service Mode
            collateralServiceMode = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CollateralMode"]) ? "Daily" : System.Configuration.ConfigurationManager.AppSettings["CollateralMode"];

            if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]))
            {
                batchSceduleTime = DateTime.Parse(EquinoxServiceConstant.ScheduledTime);
            }
            else
            {
                batchSceduleTime = (collateralServiceMode.ToUpper() == "DAILY") ? DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]) : batchSceduleTime;
            }
            if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CollateralIntervalMinutes"]))
            {
                collateralScheduleTimeInterval = EquinoxServiceConstant.IntervalMinutes;
            }
            else
            {
                if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["CollateralIntervalMinutes"], out collateralScheduleTimeInterval))
                {
                    collateralScheduleTimeInterval = EquinoxServiceConstant.IntervalMinutes;
                }
            }
            collateralServiceLogPath = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CollateralServiceLogFilePath"]) ? AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt" : System.Configuration.ConfigurationManager.AppSettings["CollateralServiceLogFilePath"];

            pbpImportServiceMode = "Interval";//string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["PBPImportMode"]) ? "Daily" : System.Configuration.ConfigurationManager.AppSettings["PBPImportMode"];
            if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["PBPImportIntervalMinutes"]))
            {
                pbpImportScheduleTimeInterval = EquinoxServiceConstant.IntervalMinutes;
            }
            else
            {
                if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["PBPImportIntervalMinutes"], out pbpImportScheduleTimeInterval))
                {
                    pbpImportScheduleTimeInterval = EquinoxServiceConstant.IntervalMinutes;
                }
            }
            pbpImportServiceLogPath = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["PBPImportServiceLogFilePath"]) ? AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt" : System.Configuration.ConfigurationManager.AppSettings["PBPImportServiceLogFilePath"];
        }
        #endregion
    }
    internal class EquinoxServiceConstant
    {
        public const string DailyMode = "DAILY";
        public const string IntervalMode = "INTERVAL";
        public const string PBPImportService = "PBP Import Service :";
        public const string CollateralService = "Collateral Service :";
        public const string Daily = "Daily";
        public const string Interval = "Interval";
        public const string ScheduledTime = "23:00";
        public const int IntervalMinutes = 2;
    }
}
