using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PrefixCounter : Entity
    {
        public int PrefixCounterId { get; set; }
        public string EntityName { get; set; }
        public string Prefix { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
    }
}
