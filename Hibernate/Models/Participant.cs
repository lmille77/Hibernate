using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public int[] OrderId { get; set; } = new int[30];

        [NotMapped]
        public double Total { get; set; }

        [NotMapped]
        public int BedSheets { get; set; }

        [NotMapped]
        public int PillowCases { get; set; }

        [NotMapped]
        public string Name { get; set; }
    }
}
