using System.Data.Entity;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories.EF
{
	public class CustomerLibDataContext : DbContext
	{
		public CustomerLibDataContext() : base("CustomerLibDb") { }

		public DbSet<Customer> Customers { set; get; }
		public DbSet<Address> Addresses { set; get; }
		public DbSet<Note> Notes { set; get; }
	}
}
