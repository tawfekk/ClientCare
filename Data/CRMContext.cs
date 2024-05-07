using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ClientCare.Models.CRM;

namespace ClientCare.Data
{
    public partial class CRMContext : DbContext
    {
        public CRMContext()
        {
        }

        public CRMContext(DbContextOptions<CRMContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ClientCare.Models.CRM.Netværk>()
              .HasOne(i => i.Medlem)
              .WithMany(i => i.Netværk)
              .HasForeignKey(i => i.MedlemId)
              .HasPrincipalKey(i => i.Id);

        }

        public DbSet<ClientCare.Models.CRM.Medlem> Medlemmer { get; set; }

        public DbSet<ClientCare.Models.CRM.Netværk> Netværk { get; set; }

        public DbSet<ClientCare.Models.CRM.Branche> Brancher { get; set; }

        public DbSet<ClientCare.Models.CRM.RelationsAnsvarlig> RelationsAnsvarlige { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}
