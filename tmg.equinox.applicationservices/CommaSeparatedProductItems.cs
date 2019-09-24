using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.Portfolio;

namespace tmg.equinox.applicationservices.ConsumerAccountDetail
{
    public class CommaSeparatedProductItems
    {
        #region Private Member

        private string _productName;
        private string _productType;
        private const string CommaOperator = " , ";
        private List<ConsumerAccountViewModel> _accountDetailsList;
        private List<PortfolioViewModel> _portfolioDetailsList;

        #endregion

        #region Constructor

        public CommaSeparatedProductItems(List<ConsumerAccountViewModel> accountDetailsList)
        {
            _accountDetailsList = accountDetailsList;
        }

        public CommaSeparatedProductItems(List<PortfolioViewModel> portfolioDetailsList)
        {
            _portfolioDetailsList = portfolioDetailsList;
        }
        #endregion

        #region Public Methods

        public  List<ConsumerAccountViewModel> GetProductListBeforeExpansion()
        {
            var commaSeparatedListOfProducts = new List<ConsumerAccountViewModel>();
            //For creating comma seperated list of product names and product types.
            if (_accountDetailsList != null && _accountDetailsList.Any())
            {
                _accountDetailsList.ForEach(accountDetails =>
                {
                    _productName = accountDetails.ProductName;
                    _productType = accountDetails.ProductType;

                    var match =
                        commaSeparatedListOfProducts.FirstOrDefault(
                            o => o.FolderVersionID == accountDetails.FolderVersionID);
                    if (match == null)
                    {
                        commaSeparatedListOfProducts.Add(accountDetails);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(_productName) && !String.IsNullOrEmpty(_productType))
                            SetDetailsWhenProductNotNull(match);
                        else
                            SetDetailsWhenProductNull(match);

                        if (!String.IsNullOrEmpty(_productName))
                        {
                            match.HAXSGroupID = _productName;
                        }
                    }
            });
        }
            return commaSeparatedListOfProducts;
        }

        public List<ConsumerAccountViewModel> GetProductListAfterExpansion()
        {
            var commaSeparatedListOfProducts = new List<ConsumerAccountViewModel>();
            //For creating comma seperated list of product names and product types. 
            _accountDetailsList.ForEach(accountDetails =>
                {
                    _productName = accountDetails.ProductName;
                    _productType = accountDetails.ProductType;

                    if (accountDetails.FolderID != null)
                    {
                        var match =
                            commaSeparatedListOfProducts.FirstOrDefault(
                                o => o.FolderVersionID == accountDetails.FolderVersionID);
                        if (match == null)
                        {
                            commaSeparatedListOfProducts.Add(accountDetails);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(_productName) && !String.IsNullOrEmpty(_productType))
                            {
                                match.ProductName = match.ProductName + CommaOperator + _productName;
                                match.ProductType = match.ProductType + CommaOperator + _productType;
                            }
                        }
                    }
                });
            return commaSeparatedListOfProducts;
        }

        public List<PortfolioViewModel> GetCommaSeparatedListForPortfolio()
        {
            var detailsWithCommaSeparatedList = new List<PortfolioViewModel>(); 
            //For creating comma seperated list of product names and product types.
            if (_portfolioDetailsList.Any())
            {
                _portfolioDetailsList.ForEach(portfolioDetails =>
                    {
                        _productName = portfolioDetails.ProductName;
                        _productType = portfolioDetails.ProductType;
                        var getMatchRecord =
                            detailsWithCommaSeparatedList.FirstOrDefault(o => o.FolderID == portfolioDetails.FolderID);

                        if (getMatchRecord == null)
                            detailsWithCommaSeparatedList.Add(portfolioDetails);
                        else
                        {
                            if (!String.IsNullOrEmpty(_productName) && !String.IsNullOrEmpty(_productName))
                            {
                                //For removing starting comma in comma seperated list.
                                if (String.IsNullOrEmpty(getMatchRecord.ProductName) && 
                                    String.IsNullOrEmpty(getMatchRecord.ProductType))
                                {
                                    getMatchRecord.ProductName = _productName;
                                    getMatchRecord.ProductType = _productType;
                                }
                                else
                                {
                                    getMatchRecord.ProductName = getMatchRecord.ProductName + CommaOperator + _productName;
                                    getMatchRecord.ProductType = getMatchRecord.ProductType + CommaOperator + _productType;
                                }
                            }
                        }
                    });
            }
            return detailsWithCommaSeparatedList;
        }

        #endregion

        #region Private Methods

        private void SetDetailsWhenProductNotNull(ConsumerAccountViewModel match)
        {
            //To remove starting comma in comma separated list.
            if (String.IsNullOrEmpty(match.ProductName) && String.IsNullOrEmpty(match.ProductType))
            {
                match.ProductName = _productName;
                match.ProductType = _productType;
            }
            else if (String.IsNullOrEmpty(match.ProductName))
            {
                match.ProductName = _productName;
                match.ProductType = match.ProductType + CommaOperator + _productType;
            }
            else if (String.IsNullOrEmpty(match.ProductType))
            {
                match.ProductName = match.ProductName + CommaOperator + _productName;
                match.ProductType = _productType;
            }
            else
            {
                match.ProductName = match.ProductName + CommaOperator + _productName;
                match.ProductType = match.ProductType + CommaOperator + _productType;
            }
        }

        private void SetDetailsWhenProductNull(ConsumerAccountViewModel match)
        {
            if (String.IsNullOrEmpty(_productName) && String.IsNullOrEmpty(_productType))
            {
                match.ProductName = match.ProductName;
                match.ProductType = match.ProductType;
            }
            //Condition when Product name is null or empty
            else if (String.IsNullOrEmpty(_productName))
            {
                match.ProductType = String.IsNullOrEmpty(match.ProductType)
                                        ? _productType
                                        : match.ProductType + CommaOperator + _productType;
            }
            //Condition when Product type is null or empty
            else if (String.IsNullOrEmpty(_productType))
            {
                match.ProductType = String.IsNullOrEmpty(match.ProductName)
                                        ? _productName
                                        : match.ProductName + CommaOperator + _productName;
            }
        }
        #endregion

    }
}
