using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016.DocumentExporter.Mappings
{
    internal static class QhpCellDataTypes
    {
        public static readonly string DECIMALTWOPLACED = "deimalTwoPlaced";
        public static readonly string INTWITHCOMMA = "intWithComma";
        public static readonly string PERCENTAGE = "percentage";
        public static readonly string PERCENTAGETWOPLACED = "percentageTowPlaced";
        public static readonly string DOLLAR = "dollar";
        public static readonly string LINK = "link";
    }

    internal class QhpCellFormattingBuilder
    {
        #region Private Memebers
        private IList<QhpToExcelMap> MappingsList { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public QhpCellFormattingBuilder()
        {
            this.MappingsList = new List<QhpToExcelMap>();
        }
        #endregion Constructor

        #region Public Methods
        public void BuildFormatMappings()
        {
            BuilderFormattingMappings();
        }

        public QhpToExcelMap GetMap(string ColumnName)
        {
            try
            {
                return this.MappingsList
                        .Where(c => c.ColumnName.Equals(ColumnName, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Public Methods

        #region Private Methods
        private void BuilderFormattingMappings()
        {


            QhpToExcelMap InDefaultCoins = new QhpToExcelMap();
            InDefaultCoins.ColumnName = "InNetworkDefaultCoinsurance";
            InDefaultCoins.CellFormat.DataType = QhpCellDataTypes.PERCENTAGETWOPLACED;
            this.MappingsList.Add(InDefaultCoins);

            QhpToExcelMap InTier2DefaultCoins = new QhpToExcelMap();
            InTier2DefaultCoins.ColumnName = "InNetworkTier2DefaultCoinsurance";
            InTier2DefaultCoins.CellFormat.DataType = QhpCellDataTypes.PERCENTAGETWOPLACED;
            this.MappingsList.Add(InTier2DefaultCoins);

            QhpToExcelMap limitedCost = new QhpToExcelMap();
            limitedCost.ColumnName = "LimitedCostSharingPlanVariation";
            limitedCost.CellFormat.DataType = QhpCellDataTypes.DOLLAR;
            this.MappingsList.Add(limitedCost);

            QhpToExcelMap Deductible1Map = new QhpToExcelMap();
            Deductible1Map.ColumnName = "Deductible";
            Deductible1Map.CellFormat.DataType = QhpCellDataTypes.DOLLAR;
            this.MappingsList.Add(Deductible1Map);

            QhpToExcelMap Coinsurance1Map = new QhpToExcelMap();
            Coinsurance1Map.ColumnName = "Coinsurance";
            Coinsurance1Map.CellFormat.DataType = QhpCellDataTypes.DOLLAR;
            this.MappingsList.Add(Coinsurance1Map);

            QhpToExcelMap Copayment1Map = new QhpToExcelMap();
            Copayment1Map.ColumnName = "Copayment";
            Copayment1Map.CellFormat.DataType = QhpCellDataTypes.DOLLAR;
            this.MappingsList.Add(Copayment1Map);

            QhpToExcelMap Limits1Map = new QhpToExcelMap();
            Limits1Map.ColumnName = "Limit";
            Limits1Map.CellFormat.DataType = QhpCellDataTypes.DOLLAR;
            this.MappingsList.Add(Limits1Map);

            QhpToExcelMap EHBPerMap = new QhpToExcelMap();
            EHBPerMap.ColumnName = "EHBPercentofTotalPremium";
            EHBPerMap.CellFormat.DataType = QhpCellDataTypes.PERCENTAGETWOPLACED;
            this.MappingsList.Add(EHBPerMap);

            QhpToExcelMap ActuarialValue = new QhpToExcelMap();
            ActuarialValue.ColumnName = "IssuerActuarialValue";
            ActuarialValue.CellFormat.DataType = QhpCellDataTypes.PERCENTAGETWOPLACED;
            this.MappingsList.Add(ActuarialValue);


            QhpToExcelMap FirstTier = new QhpToExcelMap();
            FirstTier.ColumnName = "FirstTierUtilization";
            FirstTier.CellFormat.DataType = QhpCellDataTypes.PERCENTAGE;
            this.MappingsList.Add(FirstTier);

            QhpToExcelMap SecondTier = new QhpToExcelMap();
            SecondTier.ColumnName = "SecondTierUtilization";
            SecondTier.CellFormat.DataType = QhpCellDataTypes.PERCENTAGE;
            this.MappingsList.Add(SecondTier);

            QhpToExcelMap enrollPayment = new QhpToExcelMap();
            enrollPayment.ColumnName = "URLforEnrollmentPayment";
            enrollPayment.CellFormat.DataType = QhpCellDataTypes.LINK;
            this.MappingsList.Add(enrollPayment);

            QhpToExcelMap sobc = new QhpToExcelMap();
            sobc.ColumnName = "URLforSummaryofBenefitsCoverage";
            sobc.CellFormat.DataType = QhpCellDataTypes.LINK;
            this.MappingsList.Add(sobc);

            QhpToExcelMap planBrochure = new QhpToExcelMap();
            planBrochure.ColumnName = "PlanBrochure";
            planBrochure.CellFormat.DataType = QhpCellDataTypes.LINK;
            this.MappingsList.Add(planBrochure);



        }

        #endregion Private Methods
    }
}
