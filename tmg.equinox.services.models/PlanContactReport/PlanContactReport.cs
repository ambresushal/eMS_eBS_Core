using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PlanContactReport
{
    public class PlanContactReportViewModel {
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string FormData { get; set; }
    }
    public class GroupContactReportViewModel
    {
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string OfficePhone { get; set; }
        public string CellPhone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string FaxNumber { get; set; }
    }
    public class BroakerContactReportViewModel
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string OfficePhone { get; set; }
        public string CellPhone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string FaxNumber { get; set; }
    }
    public class HSBContactReportViewModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string OfficePhone { get; set; }
        public string CellPhone { get; set; }
        public string Location { get; set; }
    }
}
