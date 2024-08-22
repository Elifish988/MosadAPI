using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using MosadApi.Models;
using MosadApi.Models;


namespace MosadApi.DAL
{
    public class MosadDBContext : DbContext
    {
        public DbSet<Agent> agents { get; set; }
        public DbSet<Target> targets { get; set; }
        public DbSet<Missoion> missoions { get; set; }
        public DbSet<Location> locations { get; set; }


        public MosadDBContext(DbContextOptions<MosadDBContext> options) : base(options)
        {
            Database.EnsureCreated();

        }
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new
                DbContextOptionsBuilder(), connectionString).Options;
        }

    }

}
