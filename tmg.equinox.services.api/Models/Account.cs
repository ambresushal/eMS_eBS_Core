using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Validators;

namespace tmg.equinox.services.api.Models
{
    [Validator(typeof(AccountValidator))]
    public class Account
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public bool IsActive { get; set; }
    }
}