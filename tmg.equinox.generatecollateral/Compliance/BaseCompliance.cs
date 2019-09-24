using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.generatecollateral
{
    public class BaseCompliance
    {
        string _complianceFilePath = "";
        List<ValidationError> _errors;
        List<ValidationError> _warnings;
        List<ValidationError> _changesMadeAfterCompliance;
        List<ValidationError> _errorExistsAfterCompliance;
        public BaseCompliance()
        {
            _errors = new List<ValidationError>();
            _warnings = new List<ValidationError>();
            _changesMadeAfterCompliance = new List<ValidationError>();
            _errorExistsAfterCompliance = new List<ValidationError>();
        }
        public virtual void MakeCompliance()
        {

        }
        public virtual void ValidateCompliancePdf()
        {

        }
        public virtual void ValidateGemBoxPdf()
        {

        }
        public string ComplianceFilePath { get { return _complianceFilePath; } }
        public List<ValidationError> Errors { get { return _errors; } }
        public List<ValidationError> Warnings { get { return _warnings; } }
        public List<ValidationError> ChangesMadeAfterCompliance { get { return _changesMadeAfterCompliance; } }

        public List<ValidationError> ErrorExistsAfterCompliance { get { return _errorExistsAfterCompliance; } }

        protected void SetComplaincePath(string newComplaincePath)
        {
            _complianceFilePath = newComplaincePath;
        }
        protected void FillErrors(IList<string> errors, string complianceType)
        {
            int count = errors.Count;

            for (int i = 0; i < count; ++i)
            {
                _errors.Add(new ValidationError { ComplianceType = complianceType, No = i + 1, Error = errors[i] });
            }
        }

        protected void FillWarnings(IList<string> warnings, string complianceType)
        {
            int count = warnings.Count;

            for (int i = 0; i < count; ++i)
            {
                _warnings.Add(new ValidationError { ComplianceType = complianceType, No = i + 1, Error = warnings[i] });
            }
        }

        protected void FillChangeLogs(IList<string> changesMadeAfterCompliance, string complianceType)
        {
            int count = changesMadeAfterCompliance.Count;

            for (int i = 0; i < count; ++i)
            {
                _changesMadeAfterCompliance.Add(new ValidationError { ComplianceType = complianceType, No = i + 1, Error = changesMadeAfterCompliance[i] });
            }
        }
        protected void FillChangeErrorLogs(IList<string> errorExistsAfterCompliance, string complianceType)
        {
            int count = errorExistsAfterCompliance.Count;

            for (int i = 0; i < count; ++i)
            {
                _errorExistsAfterCompliance.Add(new ValidationError { ComplianceType = complianceType, No = i + 1, Error = errorExistsAfterCompliance[i] });
            }
        }

    }
}
