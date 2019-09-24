using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.Validators
{
    public class ConsortiumValidator : AbstractValidator<Consortium>
    {
        public ConsortiumValidator()
        {
            RuleFor(x => x.ConsortiumName)
                .NotEmpty().WithMessage(Constants.ConsortiumNameEmpty)
                .Length(0, 200).WithMessage(Constants.ConsortiumNameLength);

        }
    }
}