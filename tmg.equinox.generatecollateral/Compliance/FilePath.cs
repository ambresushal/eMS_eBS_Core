using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.generatecollateral
{
    public class FilePath
    {
        private string _tempPath = "";
        private string _gemBoxGeneratedDocFileName = "";
        private string _currentBasePath = "";
        private string _newDocFileNamePath = "";
        private string _newComplianceFileNamePath = "";
        private string _newAccessibilityPdfFileNamePath = "";
        private string _title = "";
        private string _originalNewFileNamePDf;
        public FilePath(string gemBoxGeneratedDocFileName, string currentBasePath, string title, string originalNewFileNamePDf)
        {
            _gemBoxGeneratedDocFileName = gemBoxGeneratedDocFileName;
            _currentBasePath = currentBasePath;
            _tempPath = string.Format(@"{0}\{1}", _currentBasePath, DateTime.Now.ToString("yy-MM-dd-THH-mm-ss"));
            _originalNewFileNamePDf = originalNewFileNamePDf;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(gemBoxGeneratedDocFileName);
            _newDocFileNamePath = string.Format(@"{0}\{1}.docx", TempPath, fileName);
            _newAccessibilityPdfFileNamePath = string.Format(@"{0}\Accessibility{1}.pdf", TempPath, fileName);
            _newComplianceFileNamePath = string.Format(@"{0}\{1}.pdf", TempPath, fileName);
            _title = title;
        }
        public string Title { get { return _title; } }
        public string TempPath
        {
            get
            {
                if (!Directory.Exists(_tempPath))
                {
                    Directory.CreateDirectory(_tempPath);
                }
                return _tempPath;
            }
        }
        public string GetOriginalNewFileNamePDf { get { return _originalNewFileNamePDf; } }
        public string GemBoxGeneratedPDFFileName { get { return _gemBoxGeneratedDocFileName; } }
        public string CurrentBasePath { get { return _currentBasePath; } }
        public string NewDocFileNameWithPath { get { return _newDocFileNamePath; } }
        public string NewAccessibilityPdfFileNamePath { get { return _newAccessibilityPdfFileNamePath; } }
        public string CurrentPathWithDocFileName { get { return string.Format(@"{0}\{1}", _currentBasePath, _gemBoxGeneratedDocFileName); } }

        public string NewComplianceFileNamePath { get { return _newComplianceFileNamePath; } }

        public string GetNewComplianceFileNamePath(int ctr)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(_gemBoxGeneratedDocFileName);

            if (ctr == 1)
            {
                //  _newComplianceFileNamePath = string.Format(@"{0}\{1}}.pdf", TempPath, fileName);
                PreviouNewComplianceFileNamePath = _newAccessibilityPdfFileNamePath;
                return _newComplianceFileNamePath;
            }
            else
            {
                PreviouNewComplianceFileNamePath = _newComplianceFileNamePath;
                _newComplianceFileNamePath = string.Format(@"{0}\{1}_{2}.pdf", TempPath, fileName, ctr.ToString());
                return _newComplianceFileNamePath;
            }
        }

        public string PreviouNewComplianceFileNamePath { get; set; }

    }
}
