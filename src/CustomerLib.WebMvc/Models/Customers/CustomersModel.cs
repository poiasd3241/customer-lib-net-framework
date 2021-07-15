using System;
using System.Collections.Generic;
using System.Linq;
using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Customers
{
	public class CustomersModel
	{
		public IEnumerable<Customer> Customers { get; }
		public int CustomersCount => Customers.Count();
		public bool HasCustomers => CustomersCount > 0;
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 1;
		public int TotalPages => (int)Math.Ceiling((double)CustomersCount / PageSize);

		public CustomersModel(IEnumerable<Customer> customers)
		{
			if (customers is null)
			{
				throw new ArgumentException("The customers cannot be null", nameof(customers));
			}

			Customers = customers;
		}
	}
}
