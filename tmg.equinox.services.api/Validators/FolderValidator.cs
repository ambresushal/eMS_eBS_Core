using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.Validators
{
    public class FolderValidator : AbstractValidator<Folder>
    {
        public FolderValidator()
        {
            RuleFor(x => x.FolderName)
                    .NotEmpty().WithMessage(Constants.ForlderNameEmpty)
                    .Length(0, 200).WithMessage(Constants.ForlderNameLength);

            RuleFor(x => x.EffectiveDate)
                .NotEmpty().WithMessage(Constants.ForlderEffectiveDateEmpty)
                .Must(BeAValidDate).WithMessage(Constants.ForlderInvalidEffectiveDate); 

            //Market Segment Not required for HN hence commenting
            //RuleFor(x => x.MarketSegment)
            //    .NotEmpty().WithMessage(Constants.ForlderMarketSegmentEmpty)
            //    .Must(IsValidMarketSegment).WithMessage(Constants.ForlderInvalidMarketSegment); 

          /*  RuleFor(x => x.Category)
                .NotEmpty().WithMessage(Constants.ForlderCategoryEmpty)
                .When(HasMaterialPublishedElseWhereText)
                .Must(IsValidNonPortfolioCategory).WithMessage(Constants.ForlderNonPortfolioInvalidCategory); 

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage(Constants.ForlderCategoryEmpty)
               .When(HasMaterialPublishedElseWhereText)
               .Must(IsValidPortfolioCategory).WithMessage(Constants.ForlderPortfolioInvalidCategory);
            */
            /*We are checking for IsPortfolio false condition only */
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage(Constants.ForlderCategoryEmpty)
                .When(HasMaterialPublishedElseWhereText)
                .Must((m, y) => IsValidCategory(m.Category, false)).WithMessage(c => c.GetPorfolioMessage());

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage(Constants.FolderCategoryIdEmpty)
                .Length(0, 50).WithMessage(Constants.ForlderNameLength);


        }

        private bool IsValidCategory(string category, bool isPortFolio)
        {
            if (isPortFolio)
                return IsValidPortfolioCategory(category);
            else
                return IsValidNonPortfolioCategory(category);
        }

        private bool HasMaterialPublishedElseWhereText(Folder model)
        {
            return model.IsPortFolio;
        }	

        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default(DateTime));
        }

        private bool IsValidMarketSegment(string segmentName)
        {
            List<string> segments = new List<string>() { "Largr ASC", "Middle ASC", "Underwritten Small", "On Exchange" };
            return segments.Contains(segmentName);
        }

        private bool IsValidNonPortfolioCategory(string category)
        {
            List<string> categoryListNonPortfolio = new List<string>() { "New Account", "Renewal", "Revision", "Termination" }; 
            
            return categoryListNonPortfolio.Contains(category);
            
                
        }
        private bool IsValidPortfolioCategory(string category)
        { 
            List<string> categoryListPortfolio = new List<string>() { "Templates" }; 
             
            return categoryListPortfolio.Contains(category);
             
        } 
       
    } 

}