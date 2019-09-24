using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class QhpModelToExcelMappings
    {
        #region Private Memebers
        private IList<QhpToExcelMap> MappingsList { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public QhpModelToExcelMappings()
        {
            this.MappingsList = new List<QhpToExcelMap>();
            BuildMappings();
        }
        #endregion Constructor

        #region Public Methods

        #endregion Public Methods

        #region Private Methods
        private void BuildMappings()
        {

        }
        #endregion Private Methods
    }
}
