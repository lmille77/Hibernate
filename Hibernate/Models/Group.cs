using Hibernate.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public List<SelectListItem> GL_List { get; set; }
        [NotMapped]
        public string GroupLeader { get; set; }
        [NotMapped]
        public string GroupLeaderId { get; set; }
        [NotMapped]
        public GroupLeader GroupLeaderObj { get; set; }

        [NotMapped]
        public string SalesRepName { get; set; }

        [NotMapped]
        public string AssignId { get; set; }

        [NotMapped]
        public double Total { get; set; }

        [NotMapped]
        public int BedSheets { get; set; }

        [NotMapped]
        public int PillowCases { get; set; }

        [NotMapped]
        public int[] OrderId { get; set; } = new int[50];
    }
}
