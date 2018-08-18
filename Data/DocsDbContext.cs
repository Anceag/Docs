using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Docs.Models;

namespace Docs.Data
{
    public class DocsDbContext : IdentityDbContext
    {
        public DocsDbContext(DbContextOptions<DocsDbContext> options) : base(options) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentMember> DocumentMembers { get; set; }
        public DbSet<Role> MembersRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DocumentMember>().HasKey(dm => new { dm.DocumentId, dm.UserId });
        }
    }
}
