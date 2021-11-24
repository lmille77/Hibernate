using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models.ViewModels
{
    public class Parts
    {
        [NotMapped]
        public double GTotal { get; set; }

        [NotMapped]
        public List<Participant> Participants { get; set; }
    }
}
