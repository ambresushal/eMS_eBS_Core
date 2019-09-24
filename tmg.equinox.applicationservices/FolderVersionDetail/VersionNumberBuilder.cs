using System;

namespace tmg.equinox.applicationservices.FolderVersionDetail
{
    public class VersionNumberBuilder
    {
        //GetNextMajorVersionNumber Method
        public string GetNextMajorVersionNumber(string folderVersionNumber, DateTime effectiveDate)
        {
            var details = new VersionNumberDetails(effectiveDate.Year, folderVersionNumber, isMinor: false, isRetro: false);

            return details.ToString();
        }

        //GetNextMinorVersionNumber Method
        public string GetNextMinorVersionNumber(string folderVersionNumber, DateTime effectiveDate)
        {
            var details = new VersionNumberDetails(effectiveDate.Year, folderVersionNumber, isMinor: true, isRetro: false);

            return details.ToString();
        }

        //GetNextMinorVersionNumberForRetroChanges Method
        public string GetNextMinorVersionNumberForRetroChanges(string folderVersionNumber, DateTime effectiveDate)
        {
            var details = new VersionNumberDetails(effectiveDate.Year, folderVersionNumber, isMinor: true, isRetro: true);

            return details.ToString();
        }

        //GetNextMajorVersionNumberForRetroChanges Method
        public string GetNextMajorVersionNumberForRetroChanges(string folderVersionNumber, DateTime effectiveDate)
        {
            var details = new VersionNumberDetails(effectiveDate.Year, folderVersionNumber, isMinor: false, isRetro: false);

            return details.ToString();
        }

        public int GetIntegerPartFromVersionNumber(string folderVersionNumber)
        {
            int number;

            var isParsing = int.TryParse((folderVersionNumber.Split('_')[1]).Split('.')[0], out number);

            if (isParsing)
            {
                return number;
            }
            else
            {
                throw new NotSupportedException("The specified VersionNumber is not in Integer Format");
            }
        }

        public int GetYearFromVersionNumber(string folderVersionNumber)
        {
            int year;

            var isParsing = int.TryParse(folderVersionNumber.Split('_')[0], out year);

            if (isParsing)
            {
                return year;
            }
            else
            {
                throw new NotSupportedException("The specified VersionNumber is not in Integer Format");
            }
        }

        private class VersionNumberDetails
        {
            #region Private Variables

            private int _year;
            private string _versionNumber;
            private bool _isMinor;
            private bool _isRetro;


            #region CONSTANTS

            private const string UNDERSCORE = "_";
            private const string NULLSTRING = "";
            private const string FIRSTMINORVERSION = "0.01";
            private const string MINORVERSION = ".01";
            private const string MAJORVERSION = ".0";
            #endregion

            #endregion

            #region Constructor

            public VersionNumberDetails(int year, string versionNumber,bool isMinor,bool isRetro)
            {
                _year = year;
                _versionNumber = versionNumber;
                _isMinor = isMinor;
                _isRetro = isRetro;
            }

            #endregion

            #region Public Methods

            //logic regarding version number is given here
            public override string ToString()
            {
                string nextVersionNumber = null;
                double versionNumeric = 0;
                if (!string.IsNullOrEmpty(_versionNumber))
                {
                    if (_isRetro)
                    {
                        nextVersionNumber = NextVersionNumberForRetro();
                    }
                    else
                    {
                        if (_isMinor)
                        {
                            if (_versionNumber.Split('_')[0] != _year.ToString())
                            {
                                nextVersionNumber = NULLSTRING + _year + UNDERSCORE +FIRSTMINORVERSION;
                            }
                            else
                            {
                                versionNumeric = Convert.ToDouble(_versionNumber.Split('_')[1]) + 0.01;

                                nextVersionNumber = _year.ToString() + UNDERSCORE + versionNumeric;
                            }
                        }
                        else
                        {
                            string[] versionData = _versionNumber.Split('_');

                            versionNumeric = Convert.ToDouble(versionData[1].Split('.')[0]) + 1;

                            nextVersionNumber = _year.ToString() + UNDERSCORE + versionNumeric + MAJORVERSION;
                        }
                    }
                    
                    
                }
                else
                {
                    if (_year > 0)
                    {
                        nextVersionNumber = NULLSTRING + _year + UNDERSCORE + FIRSTMINORVERSION;
                    }
                    else
                    {
                        nextVersionNumber = NULLSTRING + DateTime.Now.Year + UNDERSCORE + FIRSTMINORVERSION;
                    }
                }

                return nextVersionNumber;
            }

            #endregion

            #region Private Methods

            private string NextVersionNumberForRetro()
            {
                string nextVersionNumber = null;
                int result;
                var isValidResult = int.TryParse((_versionNumber.Split('_')[1]).Split('.')[1], out result);

                if (isValidResult)
                {
                    if (result > 0)
                    {
                        int result2;
                        var tryParse = int.TryParse((_versionNumber.Split('_')[1]).Split('.')[0], out result2);

                        if (tryParse)
                        {
                            result2 += 1;
                            nextVersionNumber = _versionNumber.Split('_')[0] + UNDERSCORE +
                                                result2 + MINORVERSION;
                        }
                        else
                        {
                            throw new NotSupportedException("The specified VersionNumber is not in Integer Format");
                        }
                    }
                    else
                    {
                        int result1;
                        var tryParse = int.TryParse((_versionNumber.Split('_')[1]).Split('.')[0], out result1);
                        if (tryParse)
                        {
                            nextVersionNumber = _versionNumber.Split('_')[0] + UNDERSCORE + result1 + MINORVERSION;
                        }
                        else
                        {
                            throw new NotSupportedException("The specified VersionNumber is not in Integer Format");
                        }
                    }
                }
                else
                {
                    nextVersionNumber = "Not Valid";
                }
                return nextVersionNumber;
            }

            #endregion
        }
    }
}
