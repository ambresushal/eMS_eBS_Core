using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.Validation
{
    public class DocumentValidator2016BenefitPackageTemplate
    {
         List<PlanBenefitPackage> _packages;
        private string issuerID;
        private string issuerState;
        private string marketCoverage;
        private string dentalOnlyIndicator;
        private string tinNumber;

        public DocumentValidator2016BenefitPackageTemplate(List<PlanBenefitPackage> packages)
        {
            _packages = packages;
        }

        public List<QHPValidationError> ValidateQHPDocument()
        {
            List<QHPValidationError> errors = new List<QHPValidationError>();
            int index = 0;
            foreach (PlanBenefitPackage package in _packages)
            {
                QHPBenefitPackage2016Validator packageValidator = new QHPBenefitPackage2016Validator(package);
                if (index == 0)
                {
                    SetHeaderSyncInformation(package);
                }
                else
                {
                    ValidatePackageHeaderSync(package, ref errors);
                }
                errors.AddRange(packageValidator.ValidateQHPPackage(package));
                index++;
            }
            return errors;
        }

        private void SetHeaderSyncInformation(PlanBenefitPackage package)
        {
            issuerID = package.HIOSIssuerID;
            issuerState = package.IssuerState;
            marketCoverage = package.MarketCoverage;
            dentalOnlyIndicator = package.DentalOnlyPlan;
            tinNumber = package.TIN;
        }
        private void ValidatePackageHeaderSync(PlanBenefitPackage package, ref List<QHPValidationError> errors)
        {
            //HIOS Issuer ID
            if (package.HIOSIssuerID != issuerID)
            {
                errors.AddError(package.HIOSIssuerID, QHPBenefitPackage2016ErrorStrings.IssuerIDNotSync);
            }
            //Issuer State
            if (package.IssuerState != issuerState)
            {
                errors.AddError(package.IssuerState, QHPBenefitPackage2016ErrorStrings.IssuerStateNotSync);
            }
            //Market Coverage
            if (package.MarketCoverage != marketCoverage)
            {
                errors.AddError(package.MarketCoverage, QHPBenefitPackage2016ErrorStrings.MarketCoverageNotSync);
            }
            //Dental Only Indicator
            if (package.DentalOnlyPlan != dentalOnlyIndicator)
            {
                errors.AddError(package.DentalOnlyPlan, QHPBenefitPackage2016ErrorStrings.DentalOnlyNotSync);
            }
            //TIN
            if (package.TIN != tinNumber)
            {
                errors.AddError(package.TIN, QHPBenefitPackage2016ErrorStrings.TINNotSync);
            }
        }
    }
}
