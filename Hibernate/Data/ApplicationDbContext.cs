using Hibernate.Models;
using Hibernate.Models.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Data
{
    public class ApplicationDbContext :  IdentityDbContext<ApplicationUser> 
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<SalesRep> SalesReps { get; set; }

        public DbSet<GroupLeader> GroupLeaders { get; set; }

        public DbSet<Participant> Participants { get; set; }

        public DbSet<Supporter> Supporters { get; set; }
    }
}
