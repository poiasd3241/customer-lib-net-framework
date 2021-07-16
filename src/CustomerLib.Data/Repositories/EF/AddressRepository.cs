using System.Collections.Generic;
using System.Linq;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories.EF
{
	public class AddressRepository : IAddressRepository
	{
		private readonly CustomerLibDataContext _context;

		public AddressRepository(CustomerLibDataContext context)
		{
			_context = context;
		}

		public AddressRepository()
		{
			_context = new();
		}

		public bool Exists(int addressId)
		{
			var address = _context.Addresses.Find(addressId);

			return address is not null;
		}

		public int Create(Address address)
		{
			var created = _context.Addresses.Add(address);

			_context.SaveChanges();

			return created.AddressId;
		}

		public Address Read(int addressId)
		{
			var address = _context.Addresses.Find(addressId);

			return address;
		}

		public IReadOnlyCollection<Address> ReadByCustomer(int customerId)
		{
			var addresses = _context.Addresses
				.Where(address => address.CustomerId == customerId)
				.ToArray();

			return addresses;
		}

		public void Update(Address address)
		{
			var found = _context.Addresses.Find(address.AddressId);

			if (found is not null)
			{
				_context.Entry(found).CurrentValues.SetValues(address);

				_context.SaveChanges();
			}
		}

		public void Delete(int addressId)
		{
			var found = _context.Addresses.Find(addressId);

			if (found is not null)
			{
				_context.Addresses.Remove(found);

				_context.SaveChanges();
			}
		}

		public void DeleteByCustomer(int customerId)
		{
			var addresses = _context.Addresses
				.Where(address => address.CustomerId == customerId);

			foreach (var address in addresses)
			{
				_context.Addresses.Remove(address);
			}

			_context.SaveChanges();
		}

		public void DeleteAll()
		{
			var addresses = _context.Addresses.ToArray();

			foreach (var address in addresses)
			{
				_context.Addresses.Remove(address);
			}

			_context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('dbo.Addresses', RESEED, 0);");

			_context.SaveChanges();
		}
	}
}
