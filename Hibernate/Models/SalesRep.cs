using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models
{
    public class SalesRep
    {
        [Key]
        public int SalesRepId { get; set; }

        public string UserId { get; set; }


        //public ICollection<Group> Groups { get; set; }

    }
}
