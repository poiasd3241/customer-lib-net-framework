using System.Collections.Generic;
using System.Transactions;
using CustomerLib.Business.ArgumentCheckHelpers;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.Business.Validators;
using CustomerLib.Data.Repositories;
using CustomerLib.Data.Repositories.Implementations;

namespace CustomerLib.ServiceLayer.Services.Implementations
{
	public class AddressService : IAddressService
	{
		#region Private Members

		private readonly ICustomerRepository _customerRepository;
		private readonly IAddressRepository _addressRepository;

		#endregion

		#region Constructors

		public AddressService()
		{
			_customerRepository = new CustomerRepository();
			_addressRepository = new AddressRepository();
		}

		public AddressService(ICustomerRepository customerRepository,
			IAddressRepository addressRepository)
		{
			_customerRepository = customerRepository;
			_addressRepository = addressRepository;
		}

		#endregion

		#region Public Methods

		public bool Exists(int addressId)
		{
			CheckNumber.NotLessThan(1, addressId, nameof(addressId));

			var result = _addressRepository.Exists(addressId);
			return result;
		}

		public bool Save(Address address)
		{
			var validationResult = new AddressValidator().Validate(address);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			using TransactionScope scope = new();

			if (_customerRepository.Exists(address.CustomerId) == false)
			{
				return false;
			}

			_addressRepository.Create(address);

			scope.Complete();

			return true;
		}

		public Address Get(int addressId)
		{
			CheckNumber.NotLessThan(1, addressId, nameof(addressId));

			var address = _addressRepository.Read(addressId);
			return address;
		}

		public IReadOnlyCollection<Address> FindByCustomer(int customerId)
		{
			CheckNumber.NotLessThan(1, customerId, nameof(customerId));

			var addresses = _addressRepository.ReadByCustomer(customerId);
			return addresses;
		}

		/// <summary>
		/// Updates the address.
		/// </summary>
		/// <param name="address">The address to update.</param>
		/// <returns><see langword="true"/> if the update completed successfully; 
		/// <see langword="false"/> if the provided address is not in the database.</returns>
		public bool Update(Address address)
		{
			var validationResult = new AddressValidator().Validate(address);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			using TransactionScope scope = new();

			if (_addressRepository.Exists(address.AddressId) == false)
			{
				return false;
			}

			_addressRepository.Update(address);

			scope.Complete();

			return true;
		}

		/// <summary>
		/// Deletes the address.
		/// </summary>
		/// <param name="addressId">The ID of the address to delete.</param>
		/// <returns><see langword="true"/> if the deletion completed successfully; 
		/// <see langword="false"/> if the provided address (by ID) is not in the database.
		/// </returns>
		public bool Delete(int addressId)
		{
			CheckNumber.NotLessThan(1, addressId, nameof(addressId));

			using TransactionScope scope = new();

			if (_addressRepository.Exists(addressId) == false)
			{
				return false;
			}

			_addressRepository.Delete(addressId);

			scope.Complete();

			return true;
		}

		public void DeleteByCustomer(int customerId)
		{
			CheckNumber.NotLessThan(1, customerId, nameof(customerId));

			_addressRepository.DeleteByCustomer(customerId);
		}

		#endregion
	}
}
