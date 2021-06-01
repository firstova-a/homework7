using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace hwork7
{
	class AtistContext : DbContext
	{
		public DbSet<Customer> Customers { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=hw7db;Trusted_Connection=True;");
		}
	}
}
