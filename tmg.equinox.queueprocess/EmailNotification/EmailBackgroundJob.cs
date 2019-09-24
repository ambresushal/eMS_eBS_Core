using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.hangfire;
using tmg.equinox.applicationservices;

namespace tmg.equinox.queueprocess.EmailNotification
{
    public class EmailBackgroundJob : BaseBackgroundJobLow<EmailQueueInfo>
    {
        IWorkFlowStateServices _workFlowStateServices;
        public EmailBackgroundJob(IWorkFlowStateServices workFlowStateServices)
        {
            _workFlowStateServices = workFlowStateServices;
        }

        public override void Execute(EmailQueueInfo info)
        {
            //EmailNotification emailNotification = new EmailNotification();

            _workFlowStateServices.SendMailForFoldersWithUnchangedWorkFlowState();
        }
    }
}
