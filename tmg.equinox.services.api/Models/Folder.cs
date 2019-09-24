using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Validators;

namespace tmg.equinox.services.api.Models
{
    [Validator(typeof(FolderValidator))]
    public class Folder
    {
        public int? AccountID { get; set; }
        public string FolderName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsPortFolio { get; set; }
        //public string MarketSegment { get; set; }
        public string Category { get; set; }
        public string CategoryId { get; set; }
        public bool IsFoundation { get; set; }
        public string GetPorfolioMessage()
        {
            if (IsPortFolio)
                return Constants.ForlderPortfolioInvalidCategory;
            else
                return Constants.ForlderNonPortfolioInvalidCategory;
        }
    }
}