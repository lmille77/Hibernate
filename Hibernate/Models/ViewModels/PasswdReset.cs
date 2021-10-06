using Hibernate.Models.ValidationClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models.ViewModels
{
    public class PasswdReset
    {
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        [PasswordFormatValidate]
        [FirstCharCapitalValidate]
        [FirstCharValidate]
        [PasswordLengthValidate]
        [NumberValidate]
        [LetterValidate]
        [SpecialCharValidate]
        public string NewPass { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPass", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPass { get; set; }

        public string Code { get; set; }
    }
}
