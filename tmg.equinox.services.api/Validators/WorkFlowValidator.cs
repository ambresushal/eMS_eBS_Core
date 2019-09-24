using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.Validators
{
    public class WorkFlowValidator : AbstractValidator<WorkFlow>
    {
        public WorkFlowValidator()
        {
            RuleFor(x => x.FolderVersionId)
                   .NotEmpty().WithMessage(Constants.FolderVersionId);
            RuleFor(x => x.WFStateName)
                  .NotEmpty().WithMessage(Constants.WFStateName); 
        }
    }
}