using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class DisallowedMessagesData : Entity
    {
        public string Description { get; set; }
        public string DisallowedExecutionCodeID { get; set; }
        public int ProcessGovernance1up { get; set; }

        public DisallowedMessagesData Clone()
        {
            return this.MemberwiseClone() as DisallowedMessagesData;
        }
    }
}