using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//using tmg.equinox.pbpimportservices;

namespace CollateralService
{
    public class ServiceBatchExecution
    {
        public Thread CollateralProcessingThread;
        bool CollateralForProcess;
        bool CollateralExecutionresponse;
        public Thread PBPImportProcessingThread;
        bool ispbpImportProcessCompleted;
        bool pbpImportExecutionresponse;
        ScheduleEquinoxService scheduleService;
        public ServiceBatchExecution()
        {
            ispbpImportProcessCompleted = false;
        }
        public bool IspbpImportProcessCompleted
        {
            get { return ispbpImportProcessCompleted; }
            set { ispbpImportProcessCompleted = value; }
        }
        public void StartCollateralGeneration()
        {
            //pbpImportForProcess = batchExecutionService.CheckIfIASExistsForProcess();
            if (CollateralForProcess && CollateralProcessingThread == null)
            {
                //if (ErrorLogProcessingThread == null || ErrorLogProcessingThread.IsAlive == false)
                {
                    CollateralProcessingThread = new Thread(new ThreadStart(ExecuteCollateralBatches));
                    CollateralProcessingThread.Start();
                }
            }
        }
        public void ExecuteCollateralBatches()
        {
            scheduleService = new ScheduleEquinoxService();

            try
            {
                //To Do Start The Class Library Function
                //pbpImportExecutionresponse = batchExecutionService.ExecuteGlobalUpdateBatch();
                StopCollateralExecution();
            }
            catch (Exception ex)
            {
                scheduleService.WriteServiceLog(scheduleService.collateralServiceLogPath, EquinoxServiceConstant.CollateralService + "Error {0} " + ex.Message + ex.StackTrace);
            }


        }
        public void StopCollateralExecution()
        {
            CollateralProcessingThread.Join();
        }
        public void StartPBPImportGeneration()
        {
            if (!ispbpImportProcessCompleted &&(PBPImportProcessingThread == null || PBPImportProcessingThread.IsAlive == false))
            {
                PBPImportProcessingThread = new Thread(new ThreadStart(ExecutePBPImportBatches));
                PBPImportProcessingThread.Start();
            }
        }
      
        public void ExecutePBPImportBatches()
        {
           
            scheduleService = new ScheduleEquinoxService();
            try
            {
                //To Do Start The Class Library Function  UIFrameworkContext
                string ConnectionString = System.Configuration.ConfigurationManager.AppSettings["UIFrameworkContext"];
                scheduleService.WriteServiceLog(scheduleService.pbpImportServiceLogPath, EquinoxServiceConstant.PBPImportService + " {0}  Connection string " + ConnectionString);
                //PBPImportOperationService objtmgPBPImportOperationService = new PBPImportOperationService(ConnectionString);
                //objtmgPBPImportOperationService.ExecutePBPImportProcess();
                ispbpImportProcessCompleted = true;
                StopPBPImportExecution();
            }
            catch (Exception ex)
            {
                scheduleService.WriteServiceLog(scheduleService.pbpImportServiceLogPath, EquinoxServiceConstant.PBPImportService + "Error {0} " + ex.Message + ex.StackTrace);
            }
        }
        public void StopPBPImportExecution()
        {
            PBPImportProcessingThread.Join();
        }
    }
}
