using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models
{
    public class Supporter
    {
        [Key]
        public int SupporterId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public Participant Participant { get; set; }
        public int ParticipantId { get; set; }


        [NotMapped]
        public int OrderId { get; set; }

        [NotMapped]
        public double Total { get; set; }

        [NotMapped]
        public int BedSheets { get; set; }

        [NotMapped]
        public int PillowCases { get; set; }


        //[NotMapped]
        //public List<Supporter> myList { get; set; }
        //public string UserId { get; set; }
    }
}
