using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        //public string UserId { get; set; }
    }
}
