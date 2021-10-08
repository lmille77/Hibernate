using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models.ViewModels
{
    public class ApplicationUser : IdentityUser
    {
        //columns added to the database
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomUsername { get; set; }
        public bool isApproved { get; set; }

        public string LastPass1 { get; set; }

        public string LastPass2 { get; set; }

        public DateTime PasswordDate { get; set; }


        //just used to display in the view, not stored in database
        [NotMapped]
        public string RoleId { get; set; }

        //just used to display in the view, not stored in database
        [NotMapped]
        public string Role { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> RoleList { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<SalesRep> SalesReps { get; set; }
        public string groupName { get; set; }
    }
}
