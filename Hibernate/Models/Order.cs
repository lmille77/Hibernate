using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

 

        [DataType(DataType.Currency)]
        public double Total { get; set; }


        public Supporter Supporter { get; set; }
        public int? SupporterId { get; set; }

        public Participant Participant { get; set; }
        public int? ParticipantId { get; set; }

        public Group Groups { get; set; }
        public int? GroupId { get; set; }
    }
      
    
}

