using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.Validators
{
    public class DocumentValidator : AbstractValidator<Document>
    {
        public DocumentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Constants.DocumentNameEmpty)
                .Length(0, 200).WithMessage(Constants.DocumentNameLength);

            RuleFor(x => x.DesignTemplate)
                .NotEmpty().WithMessage(Constants.DesignNameEmpty)
                .Length(0, 200).WithMessage(Constants.DesignNameLength);

            RuleFor(x => x.Data)
                .NotEmpty().WithMessage(Constants.DocumentInvalidData);

        }
    }
}