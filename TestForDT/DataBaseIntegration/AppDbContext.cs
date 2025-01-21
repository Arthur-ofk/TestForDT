using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestForDT.Entities;

namespace TestForDT.DataBaseIntegration
{
    public class AppDbContext : DbContext
    {
        private readonly string _connectionString;
        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString); 
        }

        public DbSet<TripRecord> TripRecords { get; set; }
    }
}
