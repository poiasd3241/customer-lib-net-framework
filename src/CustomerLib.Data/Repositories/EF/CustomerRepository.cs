using System.Collections.Generic;
using System.Linq;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories.EF
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly CustomerLibDataContext _context;

		public CustomerRepository(CustomerLibDataContext context)
		{
			_context = context;
		}

		public CustomerRepository()
		{
			_context = new();
		}

		public bool Exists(int customerId) => _context.Customers.Find(customerId) is not null;

		public int Create(Customer customer)
		{
			var created = _context.Customers.Add(customer);

			_context.SaveChanges();

			return created.CustomerId;
		}

		public Customer Read(int customerId) => _context.Customers.Find(customerId);

		public IReadOnlyCollection<Customer> ReadAll()
		{
			var customers = _context.Customers.ToArray();

			return customers;
		}

		public int GetCount() => _context.Customers.Count();

		public IReadOnlyCollection<Customer> ReadPage(int page, int pageSize)
		{
			var customers = _context.Customers
				.OrderBy(customer => customer.CustomerId)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToArray();

			return customers;
		}

		public void Update(Customer customer)
		{
			var found = _context.Customers.Find(customer.CustomerId);

			if (found is not null)
			{
				_context.Entry(found).CurrentValues.SetValues(customer);

				_context.SaveChanges();
			}
		}

		public void Delete(int customerId)
		{
			var found = _context.Customers.Find(customerId);

			if (found is not null)
			{
				_context.Customers.Remove(found);

				_context.SaveChanges();
			}
		}

		public bool IsEmailTaken(string email) =>
			_context.Customers.Any(customer => customer.Email == email);

		public (bool, int) IsEmailTakenWithCustomerId(string email)
		{
			var customerWithEmail = _context.Customers.FirstOrDefault(
				customer => customer.Email == email);

			var isTaken = customerWithEmail is not null;
			var takenById = isTaken ? customerWithEmail.CustomerId : 0;

			return (isTaken, takenById);
		}

		public void DeleteAll()
		{
			var customers = _context.Customers
				.Include("Addresses")
				.Include("Notes");

			foreach (var customer in customers)
			{
				_context.Customers.Remove(customer);
			}

			_context.Database.ExecuteSqlCommand(
				"DBCC CHECKIDENT ('dbo.Addresses', RESEED, 0);" +
				"DBCC CHECKIDENT ('dbo.Notes', RESEED, 0);" +
				"DBCC CHECKIDENT ('dbo.Customers', RESEED, 0);");

			_context.SaveChanges();
		}
	}
}
