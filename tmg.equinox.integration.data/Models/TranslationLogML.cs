using System;
using System.Collections.Generic;
using tmg.equinox.integration.data;
using tmg.equinox.integration.data.Models;
using tmg.equinox.integration.translator.dao.Models;

namespace tmg.equinox.integration.facet.data.Models
{
    public class TranslationLogML : Entity
    {
        public List<ShowActivitylog> LstShowActivitylog = new List<ShowActivitylog>();
        public List<FacetsMasterlistTranslator> LstFacetTranslatorQueue = new List<FacetsMasterlistTranslator>();
        public List<ProcessGovernance> LstProcessGovernance = new List<ProcessGovernance>();
        public List<ACDEData> LstACDEData = new List<ACDEData>();
        public List<SERLData> LstSERLDData = new List<SERLData>();
        public List<SESRData> LstSESRData = new List<SESRData>();
        public List<BSDEData> LstBSDEData = new List<BSDEData>();
        public List<ServiceGroupDetailListData> LstServiceGroupDetailListData = new List<ServiceGroupDetailListData>();
        public List<SESEData> LstSESEData = new List<SESEData>();
    }
}
