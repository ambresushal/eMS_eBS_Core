using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
    public class AuditChecklistReportViewModel : ViewModelBase
    {
        public int FolderVersionID { get; set; }
        public int FolderID { get; set; }
        public string FolderVersionNumber { get; set; }
        public int TenantID { get; set; }
        public string DocumentName { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public int FormDesignID { get; set; }
        public int FormInstanceID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string FormData { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class AuditChecklistReportingModel
    {
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public string ReleaseDate { get; set; }
        public string DocumentName { get; set; }
        public string QID { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string AssignedTo { get; set; }
        public string AuditType { get; set; }
        public string SpecifyOther { get; set; }
        public string Product { get; set; }
        public string ProductQCError { get; set; }
        public string ProductPoints { get; set; }
        public string RX { get; set; }
        public string RXQCError { get; set; }
        public string RXPoints { get; set; }
        public string Dental { get; set; }
        public string DentalQCError { get; set; }
        public string DentalPoints { get; set; }
        public string Vision { get; set; }
        public string VisionQCError { get; set; }
        public string VisionPoints { get; set; }
        public string Stoploss { get; set; }
        public string StoplossQCError { get; set; }
        public string DEDE { get; set; }
        public string DEDEQCError { get; set; }
        public string DEDEPoints { get; set; }
        public string LTLT { get; set; }
        public string LTLTQCError { get; set; }
        public string LTLTPoints { get; set; }
        public string EBCL { get; set; }
        public string EBCLQCError { get; set; }
        public string EBCLPoints { get; set; }
        public string SEPY1 { get; set; }
        public string SEPY1QCError { get; set; }
        public string SEPY1Points { get; set; }
        public string SEPY2 { get; set; }
        public string SEPY2QCError { get; set; }
        public string SEPY2Points { get; set; }
        public string SEPY3 { get; set; }
        public string SEPY3QCError { get; set; }
        public string SEPY3Points { get; set; }
        public string SEPY4 { get; set; }
        public string SEPY4QCError { get; set; }
        public string SEPY4Points { get; set; }
        public string SEPY5 { get; set; }
        public string SEPY5QCError { get; set; }
        public string SEPY5Points { get; set; }
        public string SEPY6 { get; set; }
        public string SEPY6QCError { get; set; }
        public string SEPY6Points { get; set; }
        public string BSBSBSDL { get; set; }
        public string BSBSBSDLQCError { get; set; }
        public string BSBSBSDLPoints { get; set; }
        public string HRAAdminInfo { get; set; }
        public string HRAAdminInfoQCError { get; set; }
        public string HRAAdminInfoPoints { get; set; }
        public string HRAAllocationRules { get; set; }
        public string HRAAllocationRulesQCError { get; set; }
        public string HRAAllocationRulesPoints { get; set; }
        public string MARIS { get; set; }
        public string MARISQCError { get; set; }
        public string MARISPoints { get; set; }
        public string TotalPoint { get; set; }
        public string AuditScore { get; set; }
        public string GeneralComments { get; set; }
        public string CompletedBy { get; set; }
        public string DateCompleted { get; set; }
    }
}
