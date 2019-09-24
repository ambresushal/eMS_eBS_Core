using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class FormDesignUserSetting : Entity
    {
        public int FormDesignUserSettingID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int UserId { get; set; }
        public string LevelAt { get; set; }

        public string Key { get; set; }
        public string Data { get; set; }

        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
