using Hibernate.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models.ValidationClasses
{
    public class PasswordFormatValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            LetterValidate l = new LetterValidate();
            NumberValidate n = new NumberValidate();
            PasswordLengthValidate pwl = new PasswordLengthValidate();
            SpecialCharValidate sch = new SpecialCharValidate();
            FirstCharValidate fch = new FirstCharValidate();

            if (l.IsValid(value) != true)
            {
                return new ValidationResult("Password does not meet required criteria.");
            }
            if (n.IsValid(value) != true)
            {
                return new ValidationResult("Password does not meet required criteria.");
            }
            if (pwl.IsValid(value) != true)
            {
                return new ValidationResult("Password does not meet required criteria.");
            }
            if (sch.IsValid(value) != true)
            {
                return new ValidationResult("Password does not meet required criteria.");
            }
            if (fch.IsValid(value) != true)
            {
                return new ValidationResult("Password does not meet required criteria.");
            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }
}
