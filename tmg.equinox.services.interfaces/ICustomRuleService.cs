using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tmg.equinox.applicationservices.interfaces
{
    public interface ICustomRuleService
    {
        /// <summary>
        /// Get filtered addtionalServiceListData
        /// </summary>       
        /// <param name="masterListStandardServiceDetailsList"></param>
        /// <param name="masterListServiceData"></param>
        /// <returns></returns>     
        IList<object> FilterAdditionalServiceListData(IList<object> masterListServiceData, IList<object> masterListStandardServiceDetailsList);

        /// <summary>
        /// Get addtionalServiceListData list
        /// </summary>
        /// <param name="standardServiceList"></param>
        /// <param name="masterListServiceData"></param>
        /// <param name="masterListStandardServiceDetailsList"></param>
        /// <returns></returns>
        IList<object> UpdateAdditionalServiceListData(string[] standardServiceList, IList<object> masterListServiceData, IList<object> masterListStandardServiceDetailsList);
    }
}
