using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.VendorFeedReport
{
   public class VendorFeedViewModel : ViewModelBase
    {                   
          
            public string VendorType { get; set; }
            public string VendorName { get; set; }
            public string OutBoundEligiblityFeed { get; set; }
            public string OutBoundClaimsFeed { get; set; }
            public string CustomerServicePhoneNumber { get; set; }
            public string ClaimsAddress1 { get; set; }
            public string ClaimsAddress2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string VendorWebsite { get; set; }
        
    }
   public class VendorOutboundEligibilityViewModel
   {
       public string VendorType { get; set; }
       public string VendorName { get; set; }      
       public string DeliveryMethod { get; set; }
       public string Frequency { get; set; }
       public string UseExistingFeed { get; set; }
       public string HSBorHAXResponsibility { get; set; }
       public string AdditionalInformation { get; set; }
       public string ContactName { get; set; }
       public string Email { get; set; }
       public string OfficePhone { get; set; }       
   }
   public class VendorInboundFileContractInfoViewModel
   {
       public string InboundFileType { get; set; }
       public string FileFormat { get; set; }
       public string DeliveryMethod { get; set; }
       public string VendorSendingFile { get; set; }       
       public string ContactName { get; set; }
       public string Email { get; set; }
       public string OfficePhone { get; set; }
   }
   public class VendorOutboundClaimsViewModel
   {
       
       public string VendorType { get; set; }
       public string VendorName { get; set; }
       public string DeliveryMethod { get; set; }
       public string Frequency { get; set; }
       public string UseExistingFeed { get; set; }
       public string IsFeedIntegratedwithMedicalPlan { get; set; }
       public string IfIntegratedHowToApplyDeductiblesAndCoinsurance { get; set; }
       public string AdditionalInformation { get; set; }
       public string ContactName { get; set; }
       public string Email { get; set; }
       public string OfficePhone { get; set; }
   }
}
