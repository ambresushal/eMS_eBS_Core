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
    public static class UIElementTypeRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods

        public static int GetUIElementTypeID(this IRepositoryAsync<UIElementType> uiElementTypeRepository, string uiElementType)
        {
            var uiElementDataType = 0;
            var result="";
            try
            {
                switch (uiElementType.ToUpper())
                {
                    case "SECTION":
                        result = "SEC";
                        break;
                    case "REPEATER":
                        result = "RPT";
                        break;
                    case "TEXTBOX":
                        result = "TXT";
                        break;
                    case "[BLANK]":
                        result = "BLK";
                        break;
                    case "LABEL":
                        result = "LBL";
                        break;
                    case "MULTILINE TEXTBOX":
                        result = "MXT";
                        break;
                    case "CHECKBOX":
                        result = "CHK";
                        break;
                    case "DROPDOWN LIST":
                        result = "DDL";
                        break;
                    case "RADIO BUTTON":
                        result = "RAD";
                        break;
                    case "CALENDAR":
                        result = "CAL";
                        break;
                    case "DROPDOWN TEXTBOX":
                        result = "DDT";
                        break;
                    case "RICH TEXTBOX":
                        result = "RXT";
                        break;
                }

                uiElementDataType = uiElementTypeRepository
                                .Query()
                                .Filter(c => c.UIElementTypeCode == result)
                                .Get()
                                .Select(c => c.UIElementTypeID)
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
