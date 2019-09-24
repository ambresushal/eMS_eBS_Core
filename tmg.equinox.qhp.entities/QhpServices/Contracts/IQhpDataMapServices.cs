using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.qhp.entities.Entities.Models;

namespace tmg.equinox.qhp.entities.QhpServices.Contracts
{
    public interface IQhpDataMapServices
    {
        List<QhpDataMapModel> GetQhpDataMapDetails();
        bool SaveQhpGenerationActivityLog(QhpActivityLogModel qhpActivityLog);
    }
}
