using Domin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.Context
{
    public partial class BusinessCardDBContext : DbContext
    {
        public static string userName = string.Empty;
        public static string userInfo = string.Empty;

        public BusinessCardDBContext()
        {
        }

        public BusinessCardDBContext(DbContextOptions<BusinessCardDBContext> options)
       : base(options)
        {
        }

        public virtual DbSet<BusinessCard> BusinessCards { get; set; }
        public virtual DbSet<Attachment> Attachment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BusinessCard>()
                .HasMany(b => b.Attachments)
                .WithOne(a => a.BusinessCard)
                .HasForeignKey(a => a.BusinessCardId);
        }
    }

}
