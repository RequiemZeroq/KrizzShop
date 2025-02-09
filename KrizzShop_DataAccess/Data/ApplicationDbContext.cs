﻿using KrizzShop_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KrizzShop_DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    { 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<InquiryHeader> InquiryHeaders { get; set; }
        public DbSet<InquiryDetail> InquiryDetails { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; } 
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
