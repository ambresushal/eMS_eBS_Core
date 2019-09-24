using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.qhp.entities.Entities.Models;
using tmg.equinox.qhp.entities.Context;
using tmg.equinox.qhp.entities.QhpServices.Contracts;

namespace tmg.equinox.qhp.entities.QhpServices
{
    public class QhpDataMapServices : IQhpDataMapServices
    {
        #region Public/ Protected / Private Member Variables       
        #endregion Member Variables

        #region Constructor/Dispose
        public QhpDataMapServices()
        {
           
        }
        #endregion Constructor/Dispose

        #region Public Methods
        public List<QhpDataMapModel> GetQhpDataMapDetails()
        {
            List<QhpDataMapModel> QhpDetails = null;
            using (var context = new QhpDataGenerationContext())
            {
                QhpDetails = (from qhpMapping in context.QhpMappingSet
                              select new QhpDataMapModel
                              {
                                  JsonAttribute = qhpMapping.JsonAttribute,
                                  QhpAttribute = qhpMapping.QhpAttribute,
                                  JsonXPath = qhpMapping.JsonXPath,
                                  FieldType = qhpMapping.FieldType,
                                  RelationType = qhpMapping.RelationType,
                                  IsParent = qhpMapping.IsParent,
                                  IsChild = qhpMapping.IsChild,
                                  Comments = qhpMapping.Comments
                              }).ToList();
            }
            return QhpDetails;
        }

        public bool SaveQhpGenerationActivityLog(QhpActivityLogModel qhpActivityLog)
        {
            bool retVal = false;
            try
            {
                QhpGenerationActivityLog activityLog = new QhpGenerationActivityLog();
                activityLog.Category = qhpActivityLog.Category;
                activityLog.Event = qhpActivityLog.Event;
                activityLog.TimeUtc = qhpActivityLog.TimeUtc;
                activityLog.UserName = qhpActivityLog.UserName;
                activityLog.Host = qhpActivityLog.Host;
                activityLog.Message = qhpActivityLog.Message;
                activityLog.URI = qhpActivityLog.URI;

                using(var context = new QhpDataGenerationContext())
                {
                    context.QhpGenerationActivityLogSet.Add(activityLog);
                    context.SaveChanges();
                    retVal = true;
                }                
            }
            catch (Exception ex)
            {
                throw;
            }
            return retVal;
        }
        #endregion Public Methods

        #region Private Methods
     
        #endregion Private Methods

        #region Helper Methods

        #endregion Helper Methods


    }
}
