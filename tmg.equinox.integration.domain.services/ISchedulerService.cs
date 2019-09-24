using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.integration.domain.services
{
    public interface ISchedulerService
    {
        /// <summary>
        /// <para>Gets All Scheduled Jobs</para>
        /// </summary>     
        /// <returns><para>List of Scheduled Jobs</para></returns>    
        IList<Job> GetScheduledJobs();

        /// <summary>
        /// Get the task details
        /// </summary>
        /// <returns></returns>
        Job GetTaskDetails(string id);

        /// <summary>
        /// Get the Plugin Versions 
        /// </summary>
        /// <returns></returns>
        IList<VersionRowModel> GetPluginVersions();

        /// <summary>
        /// Create new job 
        /// </summary>
        /// <returns></returns>
        string AddTask(Job schedulerJob);


        /// <summary>
        /// Edit Existing job 
        /// </summary>
        /// <returns></returns>
        string EditTask(Job schedulerJob);


        /// <summary>
        /// Delete Existing job 
        /// </summary>
        /// <returns></returns>
        string DeleteTask(string Id);

        /// <summary>
        /// Run job 
        /// </summary>
        /// <returns></returns>
        void Run(string Id);
    }
}
