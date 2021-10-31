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
        public int SupporterID { get; set; }

        public Participant Participant { get; set; }

        public string UserId { get; set; }
    }
}
