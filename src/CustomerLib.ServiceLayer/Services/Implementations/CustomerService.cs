using System;
using System.Transactions;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.Business.Validators;
using CustomerLib.Data.Repositories;
using CustomerLib.Data.Repositories.Implementations;

namespace CustomerLib.ServiceLayer.Services.Implementations
{
	public class CustomerService : ICustomerService
	{
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

		public void Save(Customer customer)
		{
			var validationResult = new CustomerValidator().Validate(customer);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			try
			{
				if (_customerRepository.IsEmailTaken(customer.Email))
				{
					throw new EmailTakenException(customer.Email);
				}

				using var scope = new TransactionScope();

				_customerRepository.Create(customer);

				foreach (var address in customer.Addresses)
				{
					_addressService.Save(address);
				}

				foreach (var note in customer.Notes)
				{
					_noteService.Save(note);
				}

				scope.Complete();
			}
			catch (Exception e) when (e is not EmailTakenException)
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public Customer Get(int customerId)
		{
			if (customerId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(customerId));
			}

			try
			{
				var customer = _customerRepository.Read(customerId);
				return customer;
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public void Update(Customer customer)
		{
			var validationResult = new CustomerValidator().Validate(customer);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			try
			{
				var (isTaken, takenById) = _customerRepository.IsEmailTakenWithCustomerId(
					customer.Email);

				if (isTaken && takenById != customer.CustomerId)
				{
					throw new EmailTakenException(customer.Email);
				}

				using TransactionScope scope = new();

				_customerRepository.Update(customer);

				foreach (var address in customer.Addresses)
				{
					_addressService.Update(address);
				}

				foreach (var note in customer.Notes)
				{
					_noteService.Update(note);
				}

				scope.Complete();
			}
			catch (Exception e) when (e is not EmailTakenException)
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public void Delete(int customerId)
		{
			if (customerId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(customerId));
			}

			try
			{
				using TransactionScope scope = new();

				_addressService.DeleteByCustomer(customerId);

				_customerRepository.Delete(customerId);

				scope.Complete();
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		#endregion
	}
}
