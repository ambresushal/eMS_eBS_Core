using System;
using System.Collections.Generic;
using tmg.equinox.integration.data;
using tmg.equinox.integration.data.Models;
using tmg.equinox.integration.translator.dao.Models;

namespace tmg.equinox.integration.facet.data.Models
{
    public class TransmissionLog  : Entity
    {
        public List<ShowActivitylog> LstShowActivitylog = new List<ShowActivitylog>();
        public List<FacetTransmitterQueue> LstFacetTransmitterQueue = new List<FacetTransmitterQueue>();
        public List<ProcessGovernance> LstProcessGovernance = new List<ProcessGovernance>();
        public List<FacetProductTransmissionSet> LstFacetProductTransmissionSet = new List<FacetProductTransmissionSet>();
        public List<FacetMLTransmissionSet> LstFacetMLTransmissionSet = new List<FacetMLTransmissionSet>();
        public List<TransmitterQuery> LstTransmitterQuery = new List<TransmitterQuery>();
    }    
}
