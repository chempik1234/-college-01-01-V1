using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTkani.DataModel
{
    public class DanisContext : DbContext
    {
        public DbSet<User> User { get; set; }
		public DbSet<Product> Product { get; set; }

		public DanisContext()
        {
            Database.EnsureCreated();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
            optionsBuilder.UseMySql(
				"server=localhost;user=root;password=admin;database=trade;",
                new MySqlServerVersion("9.5.0"));
		}
	}
}
