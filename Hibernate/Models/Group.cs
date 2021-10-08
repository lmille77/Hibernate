using Hibernate.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Display(Name ="Group Name")]
        [Required]
        public string Name { get; set; }


        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }


        public SalesRep SalesRep { get; set; }
        
        public int SalesRepId { get; set; }
        [NotMapped]
        public string AssignId { get; set; }

    }
}
