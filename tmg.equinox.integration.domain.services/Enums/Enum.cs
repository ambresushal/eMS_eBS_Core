using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities
{
    enum Component
        {
            PDPD,
            ACCM,
            ACDE,
            DEDE,
            IPMC,
            LTID,
            LTIP,
            LTLT,
            LTPR,
            PDBC,
            LTSE,
            PDDS,
            PDVC,
            SEDF,
            SEPY,
            SERL,
            SESE,
            SESP,
            SESR,
            SETR

        };

        enum FileExtension
        {
            csv,
            xml
        };

        enum Status
        {
            Queued = 1,
            Completed,
            Errored
        }
}
