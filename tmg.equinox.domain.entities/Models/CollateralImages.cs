using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class CollateralImages : Entity
    {
        public  int  ID { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public DateTime creationDate { get; set; }
    }
}
