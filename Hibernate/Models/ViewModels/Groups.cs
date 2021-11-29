using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models.ViewModels
{
    public class Groups
    {

        [NotMapped]
        public double GTotal { get; set; }

        [NotMapped]
        public List<Group> Gs{ get; set; }

        [NotMapped]
        public int SId { get; set; }
    }
}
