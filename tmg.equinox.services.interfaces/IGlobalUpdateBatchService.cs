using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.interfaces
{
   public interface IGlobalUpdateBatchService
    {
       //used start up call for batch execution 
       bool ExecuteGlobalUpdateBatch();
       //check if batch available for processing . baased on this thread will be initizlied in windows service.
       bool CheckIfBatchExitsForProcess();

       bool CheckIfIASExistsForProcess();
       bool ExecuteIASGeneration(string iasfolderPath);

       bool CheckIfErrorLogExistsForProcess();
       bool ExecuteErrorLogGeneration(string errorLogfolderPath, string importIASPath);
    }
}
