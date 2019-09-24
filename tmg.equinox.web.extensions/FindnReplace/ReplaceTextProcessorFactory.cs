using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.web.FindnReplace
{
    public class ReplaceTextProcessorFactory
    {
        public static IReplaceTextProcessor GetHandler(DocumentInfo documentInfo, ReplaceCriteria criteria, IFormDesignService formDesignServices, IFormInstanceDataServices dataService, IFolderVersionServices folderVerionService, int? userId, string userName, FormDesignVersionDetail detail)
        {
            IReplaceTextProcessor handler = null;
            if (string.IsNullOrWhiteSpace(criteria.SpecificPath))
            {
                handler = new ReplaceTextProcessor(documentInfo, criteria, formDesignServices, dataService, folderVerionService, userId, userName, detail);
            }
            else
            {
                handler = new ReplaceTextLookInProcessor(documentInfo, criteria, formDesignServices, dataService, folderVerionService, userId, userName, detail);
            }

            return handler;
        }
    }
}
