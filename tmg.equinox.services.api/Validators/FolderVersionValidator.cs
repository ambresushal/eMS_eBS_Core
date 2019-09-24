using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.Validators
{
    public class FolderVersionValidator : AbstractValidator<FolderVersion>
    {
        public FolderVersionValidator()
        {
            RuleFor(x => x.folderId)
                    .NotEmpty().WithMessage(Constants.FolderIDEmpty);

            RuleFor(x => x.EffectiveDate)
                   .NotEmpty().WithMessage(Constants.FolderVersionEffectiveDateEmpty)
                   .Must(BeAValidDate).WithMessage(Constants.FolderVersionInvalidEffectiveDate); 

            RuleFor(x => x.CategoryId)
               .NotEmpty().WithMessage(Constants.FolderCategoryIdEmpty)
               .Length(0, 50).WithMessage(Constants.ForlderNameLength);

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage(Constants.ForlderCategoryEmpty)
                .Must(IsValidNonPortfolioCategory).WithMessage(Constants.ForlderNonPortfolioInvalidCategory);

        }

        private bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default(DateTime));
        }
        private bool IsValidNonPortfolioCategory(string category)
        {
            List<string> categoryListNonPortfolio = new List<string>() { "New Account", "Renewal", "Revision", "Termination" };

            return categoryListNonPortfolio.Contains(category);


        }
    }
}