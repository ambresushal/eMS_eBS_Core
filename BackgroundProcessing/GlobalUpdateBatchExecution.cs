using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.dependencyresolution;

namespace EquinoxProcessor
{
    public class GlobalUpdateBatchExecution
    {
        public Thread BatchProcessingThread;
        bool batchExistForProcess;
        //GlobalUpdateBatchService batchExecutionService;
        private IGlobalUpdateBatchService batchExecutionService;
        ScheduleEquinoxService scheduleService;
        bool batchExecutionresponse;

        public Thread IASProcessingThread;
        bool iasExistForProcess;
        bool iasExecutionresponse;

        public Thread ErrorLogProcessingThread;
        bool errorLogExistForProcess;
        bool errorLogExecutionresponse;

        public GlobalUpdateBatchExecution()
        {
            //batchExecutionService = new GlobalUpdateBatchService();
            batchExecutionService = UnityConfig.Resolve<IGlobalUpdateBatchService>();
        }

        public void ExecuteSchedulesBatches()
        {
            scheduleService = new ScheduleEquinoxService();
            try
            {
                batchExecutionresponse = batchExecutionService.ExecuteGlobalUpdateBatch();
                StopBatchExecution();
            }
            catch (Exception ex)
            {
                scheduleService.WriteServiceLog(scheduleService.batchServiceLogPath, EquinoxServiceConstant.BatchService + "Error {0} " + ex.Message + ex.StackTrace);
            }

        }
        public void StartBatchExecution()
        {
            batchExistForProcess = batchExecutionService.CheckIfBatchExitsForProcess();
            if (batchExistForProcess)
            {
                BatchProcessingThread = new Thread(new ThreadStart(ExecuteSchedulesBatches));
                BatchProcessingThread.Start();

            }
        }
        public void StopBatchExecution()
        {
            BatchProcessingThread.Join();
        }


        public void StartIASGeneration()
        {
            iasExistForProcess = batchExecutionService.CheckIfIASExistsForProcess();
            if (iasExistForProcess && IASProcessingThread == null)
            {
                if (ErrorLogProcessingThread == null || ErrorLogProcessingThread.IsAlive == false)
                {
                    IASProcessingThread = new Thread(new ThreadStart(ExecuteScheduledIAS));
                    IASProcessingThread.Start();
                }
            }
        }
        public void ExecuteScheduledIAS()
        {
            scheduleService = new ScheduleEquinoxService();
            try
            {
                string iasfolderPath = ConfigurationManager.AppSettings["IASfolderPath"];
                iasExecutionresponse = batchExecutionService.ExecuteIASGeneration(iasfolderPath);
                StopIASExecution();
            }
            catch (Exception ex)
            {
                scheduleService.WriteServiceLog(scheduleService.iasServiceLogPath, EquinoxServiceConstant.IASService + "Error {0} " + ex.Message + ex.StackTrace);
            }

        }
        public void StopIASExecution()
        {
            IASProcessingThread.Join();
        }


        public void StartErrorLogGeneration()
        {
            errorLogExistForProcess = batchExecutionService.CheckIfErrorLogExistsForProcess();
            if (errorLogExistForProcess && ErrorLogProcessingThread == null)
            {
                if (IASProcessingThread == null || IASProcessingThread.IsAlive == false)
                {
                    ErrorLogProcessingThread = new Thread(new ThreadStart(ExecuteScheduledErrorLog));
                    ErrorLogProcessingThread.Start();
                }
            }
        }
        public void ExecuteScheduledErrorLog()
        {
            scheduleService = new ScheduleEquinoxService();
            try
            {
                string errorLogfolderPath = ConfigurationManager.AppSettings["ErrorLogfolderPath"];
                string importIASPath = ConfigurationManager.AppSettings["IASfolderPath"];
                errorLogExecutionresponse = batchExecutionService.ExecuteErrorLogGeneration(errorLogfolderPath, importIASPath);
                StopErrorLogExecution();
            }
            catch (Exception ex)
            {
                scheduleService.WriteServiceLog(scheduleService.validationServiceLogPath, EquinoxServiceConstant.ValidationService + "Error {0} " + ex.Message + ex.StackTrace);
            }
        }
        public void StopErrorLogExecution()
        {
            ErrorLogProcessingThread.Join();
        }

    }
}
