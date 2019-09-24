using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.core.logging.Logging;

namespace tmg.equinox.backgroundjob
{
    public class CustomQueueMangement
    {
        private static readonly ILog _logger = LogProvider.For<CustomQueueMangement>();
        public object CreateInstanceCustomQueue(BaseJobInfo jobinfo)
        {
            Assembly customQueueAssembly = null;

            if ((string.IsNullOrEmpty(jobinfo.AssemblyName) == true && string.IsNullOrEmpty(jobinfo.ClassName) == true))
                return null;

            //customQueueAssembly
            // = AppDomain.CurrentDomain.GetAssemblies().
            //      SingleOrDefault(assembly => assembly.GetName().Name == jobinfo.AssemblyName);

            customQueueAssembly = Assembly.Load(jobinfo.AssemblyName);


            return customQueueAssembly.CreateInstance(string.Format("{0}.{1}", jobinfo.AssemblyName, jobinfo.ClassName));
        }
        public void ExecuteCustomQueueMethod(object instanceCustomQueue, BaseJobInfo jobInfo, string methodName)
        {
            try
            {
                if (instanceCustomQueue == null)
                    return;

                MethodInfo method = instanceCustomQueue.GetType().GetMethod(methodName);

                if (method != null)
                {
                    method.Invoke(instanceCustomQueue, new object[] { jobInfo });
                }
            }
            catch (TargetInvocationException ex)
            {
                _logger.ErrorException("Error in Invocation " + methodName, ex);
                throw ex.InnerException; // ex now stores the original exception
            }
            catch(Exception ex)
            {
                _logger.ErrorException("Error in Invocation " + methodName, ex);
                throw ex; 
            }
        }
    }
}
