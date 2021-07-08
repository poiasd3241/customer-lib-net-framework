using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CustomerLib.Business.ArgumentCheckHelpers;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.Business.Validators;
using CustomerLib.Data.Repositories;
using CustomerLib.Data.Repositories.Implementations;

namespace CustomerLib.ServiceLayer.Services.Implementations
{
	public class CustomerService : ICustomerService
	{
		// TODO: extract Addresses and Notes population.

		#region Private Members

		private readonly ICustomerRepository _customerRepository;
		private readonly IAddressService _addressService;
		private readonly INoteService _noteService;

		#endregion

		#region Constructors

		public CustomerService()
		{
			_customerRepository = new CustomerRepository();
			_addressService = new AddressService();
			_noteService = new NoteService();
		}

		public CustomerService(ICustomerRepository customerRepository,
			IAddressService addressService, INoteService noteService)
		{
			_customerRepository = customerRepository;
			_addressService = addressService;
			_noteService = noteService;
		}

		#endregion

		#region Public Methods

		public bool Exists(int customerId)
		{
			CheckNumber.NotLessThan(1, customerId, nameof(customerId));

			var result = _customerRepository.Exists(customerId);
			return result;
		}

		public void Save(Customer customer)
		{
			var validationResult = new CustomerValidator().Validate(customer);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			using TransactionScope scope = new();

			if (_customerRepository.IsEmailTaken(customer.Email))
			{
				throw new EmailTakenException(customer.Email);
			}

			var customerId = _customerRepository.Create(customer);

			foreach (var address in customer.Addresses)
			{
				address.CustomerId = customerId;
				_addressService.Save(address);
			}

			foreach (var note in customer.Notes)
			{
				note.CustomerId = customerId;
				_noteService.Save(note);
			}

			scope.Complete();
		}

		public Customer Get(int customerId, bool includeAddresses, bool includeNotes)
		{
			CheckNumber.NotLessThan(1, customerId, nameof(customerId));

			using TransactionScope scope = new();

			var customer = _customerRepository.Read(customerId);

			if (customer is null)
			{
				return customer;
			}

			if (includeAddresses)
			{
				customer.Addresses =
					_addressService.FindByCustomer(customer.CustomerId)?.ToList();
			}

			if (includeNotes)
			{
				customer.Notes =
					_noteService.FindByCustomer(customer.CustomerId)?.ToList();
			}

			return customer;
		}

		public IReadOnlyCollection<Customer> GetAll(bool includeAddresses, bool includeNotes)
		{
			using TransactionScope scope = new();

			var customers = _customerRepository.ReadAll();

			if (customers is null)
			{
				return customers;
			}

			if (includeAddresses)
			{
				foreach (var customer in customers)
				{
					customer.Addresses =
						_addressService.FindByCustomer(customer.CustomerId)?.ToList();
				}
			}

			if (includeNotes)
			{
				foreach (var customer in customers)
				{
					customer.Notes =
						_noteService.FindByCustomer(customer.CustomerId)?.ToList();
				}
			}

			return customers;
		}

		public int GetCount() => _customerRepository.GetCount();

		public IReadOnlyCollection<Customer> GetPage(int page, int pageSize,
			bool includeAddresses, bool includeNotes,
			bool checkTotalSame = false, int expectedTotal = 0)
		{
			CheckNumber.NotLessThan(1, page, nameof(page));
			CheckNumber.NotLessThan(1, pageSize, nameof(pageSize));
			CheckNumber.NotLessThan(0, expectedTotal, nameof(expectedTotal));

			using TransactionScope scope = new();

			if (checkTotalSame)
			{
				var totalCustomers = GetCount();

				if (expectedTotal != totalCustomers)
				{
					throw new DataChangedWhileProcessingException();
				}
			}

			var customers = _customerRepository.ReadPage(page, pageSize);

			if (customers is null)
			{
				return customers;
			}

			if (includeAddresses)
			{
				foreach (var customer in customers)
				{
					customer.Addresses =
						_addressService.FindByCustomer(customer.CustomerId)?.ToList();
				}
			}

			if (includeNotes)
			{
				foreach (var customer in customers)
				{
					customer.Notes =
						_noteService.FindByCustomer(customer.CustomerId)?.ToList();
				}
			}

			return customers;
		}

		/// <summary>
		/// Updates the customer (Addresses and Notes are ignored).
		/// </summary>
		/// <param name="customer">The customer to update.</param>
		/// <returns><see langword="true"/> if the update completed successfully; 
		/// <see langword="false"/> if the provided customer is not in the database.</returns>
		public bool Update(Customer customer)
		{
			var validationResult = new CustomerValidator()
				.ValidateWithoutAddressesAndNotes(customer);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			using TransactionScope scope = new();

			if (_customerRepository.Exists(customer.CustomerId) == false)
			{
				return false;
			}

			var (isTaken, takenById) = _customerRepository.IsEmailTakenWithCustomerId(
				customer.Email);

			if (isTaken && takenById != customer.CustomerId)
			{
				throw new EmailTakenException(customer.Email);
			}

			_customerRepository.Update(customer);

			scope.Complete();

			return true;
		}

		/// <summary>
		/// Deletes the customer, including its Addresses and Notes.
		/// </summary>
		/// <param name="customerId">The ID of the customer to delete.</param>
		/// <returns><see langword="true"/> if the deletion completed successfully; 
		/// <see langword="false"/> if the provided customer (by ID) is not in the database.
		/// </returns>
		public bool Delete(int customerId)
		{
			CheckNumber.NotLessThan(1, customerId, nameof(customerId));

			using TransactionScope scope = new();

			if (_customerRepository.Exists(customerId) == false)
			{
				return false;
			}

			_addressService.DeleteByCustomer(customerId);

			_customerRepository.Delete(customerId);

			scope.Complete();

			return true;
		}

		#endregion
	}
}
