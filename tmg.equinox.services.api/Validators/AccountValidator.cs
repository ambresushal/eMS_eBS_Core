using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.Validators
{
    public class AccountValidator : AbstractValidator<Account>
    {
        public AccountValidator()
        {
            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage(Constants.AccountNameEmpty)
                .Length(0, 200).WithMessage(Constants.AccountNameLength);

        }
    }
}