using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ApplicationDataTypeRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods

        public static int GetUIElementDataTypeID(this IRepositoryAsync<ApplicationDataType> applicationDataTypeRepository, string uiElementType)
        {
            var uiElementDataType = 0;
            var result="";
            try
            {
                switch (uiElementType.ToUpper())
                {
                    case "SECTION":
                        result = "NA";
                        break;
                    case "REPEATER":
                        result = "NA";
                        break;
                    case "TEXTBOX":
                    case "[BLANK]":
                    case "LABEL":
                    case "MULTILINE TEXTBOX":
                    case "RICH TEXTBOX": 
                        result = "string";
                        break;
                    case "CHECKBOX":
                        result = "bool";
                        break;
                    case "DROPDOWN LIST":
                    case "DROPDOWN TEXTBOX":
                        result = "string";
                        break;
                    case "RADIO BUTTON":
                        result = "bool";
                        break;
                    case "CALENDAR":
                        result = "date";
                        break;
                }

                uiElementDataType = applicationDataTypeRepository
                                .Query()
                                .Filter(c => c.ApplicationDataTypeName == result)
                                .Get()
                                .Select(c => c.ApplicationDataTypeID)
                                .FirstOrDefault();
            }
            catch (Exception es)
            {
                throw;
            }
            return uiElementDataType;
        }
        #endregion Public Methods

        #region Private Methods



        #endregion Private Methods
    }
}
