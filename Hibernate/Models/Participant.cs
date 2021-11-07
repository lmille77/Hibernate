using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }

        public Group Group { get; set; }

        public int GroupId { get; set; }

        public string UserId { get; set; }
    }
}
