using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.emailnotification.Model
{
    public class EmailTemplateInfo
    {
        public string TemplateName { get; set; }
        public List<EmailTemplatePlaceHolderInfo> PlaceHolder { get; set; }
        public void SetValue(string placeHolder, string value)
        {
            //search 
            var t = PlaceHolder.Where(m => m.PlaceHolderKey == placeHolder).FirstOrDefault();
            t.PlaceHolderValue = value;
        }
    }
}
