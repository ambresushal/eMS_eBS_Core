using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class UIElementExtension
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool CanBeSavedInDomainModel(this UIElement uiElement)
        {
            bool returnVal = false;

            if (uiElement.IsContainer())
            {
                returnVal = false;
            }
            else if (uiElement is TextBoxUIElement)
            {
                TextBoxUIElement textBoxUIElement = uiElement as TextBoxUIElement;

                //Assuming that Code for Label will be "LBL" & code for blank will be "BLK"
                //if (textBoxUIElement.UIElementType.UIElementTypeCode == "LBL" || textBoxUIElement.UIElementType.UIElementTypeCode == "BLK")
                if (textBoxUIElement.UIElementTypeID == 10 || textBoxUIElement.UIElementTypeID == 11)
                {
                    returnVal = false;
                }
                else
                    returnVal = true;
            }
            else
                returnVal = true;

            return returnVal;
        }

        public static bool IsContainer(this UIElement uiElement)
        {
            bool returnVal = false;

            if (uiElement is TabUIElement || uiElement is SectionUIElement || uiElement is RepeaterUIElement)
            {
                returnVal = true;
            }

            return returnVal;
        }

        public static bool IsRepeater(this UIElement uielement)
        {
            return uielement is RepeaterUIElement ? true : false;
        }

        public static bool IsBlank(this UIElement uielement)
        {
            bool isBlank = false;

            if (uielement is TextBoxUIElement)
            {
                if (((TextBoxUIElement)uielement).UIElementTypeID == Convert.ToInt32(tmg.equinox.domain.entities.UIElementType.BLANK))
                    isBlank = true;
            }

            return isBlank;
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
