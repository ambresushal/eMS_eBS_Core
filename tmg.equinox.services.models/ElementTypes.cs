using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices
{
    public class ElementTypes
    {
        public static string[] list = { "Radio Button", "Checkbox", "Textbox", "Multiline TextBox", "Dropdown List", "Calendar", "Repeater", "Tab", "Section", "Label", "[Blank]", "Dropdown TextBox", "Rich TextBox" };

        public const string RADIO = "Radio";
        public const string CHECKBOX = "CheckBox";
        public const string TEXTBOX = "TextBox";
        public const string DROPDOWN = "DropDown";
        public const string CALENDAR = "Calendar";
        public const string REPEATER = "Repeater";
        public const string TAB = "Tab";
        public const string SECTION = "Section";

        public const string LABEL = "Label";

        public const int RADIOID = 1;
        public const int CHECKBOXID = 2;
        public const int TEXTBOXID = 3;
        public const int MULTILINETEXTBOXID = 4;
        public const int DROPDOWNID = 5;
        public const int CALENDARID = 6;
        public const int REPEATERID = 7;
        public const int TABID = 8;
        public const int SECTIONID = 9;
        public const int LABELID = 10;
        public const int BLANKID = 11;
        public const int DROPDOWNTEXTBOXID = 12;
        public const int RICHTEXTBOXID = 13;
    }

    public static class ViewTypes
    {
        public static string GetViewType(int type)
        {
            string viewType = "Default";
            if (type == 2)
            {
                viewType = "SOT";
            }
            else if (type == 3)
            {
                viewType = "Both";
            }
            else if (type == 4)
            {
                viewType = "None";
            }
            return viewType;
        }
    }
}
