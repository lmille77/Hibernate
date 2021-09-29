﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Group Name")]
        [Required]
        public string Name { get; set; }


        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }
    }
}
