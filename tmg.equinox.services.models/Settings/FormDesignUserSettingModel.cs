using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class FormDesignUserSettingModel : ViewModelBase
    {

        public int FormDesignVersionID { get; set; }
        public int UserId { get; set; }
        public string LevelAt { get; set; }

        public string Key { get; set; }
        public string Data { get; set; }
    
        public int FormDesignUserSettingID { get; set; }
    }

    public class FormDesignUserSettingInputModel
    {
        public int FormDesignVersionID { get; set; }
        public int UserId { get; set; }
        public string Key { get; set; }

        public string Key1 { get; set; }

    }

}
