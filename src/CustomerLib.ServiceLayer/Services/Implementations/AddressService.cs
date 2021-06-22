using System;
using System.Collections.Generic;
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

		private readonly IAddressRepository _addressRepository;

		#endregion

		#region Constructors

		public AddressService()
		{
			_addressRepository = new AddressRepository();
		}

		public AddressService(IAddressRepository addressRepository)
		{
			_addressRepository = addressRepository;
		}

		#endregion

		#region Public Methods

		public void Save(Address address)
		{
			var validationResult = new AddressValidator().Validate(address);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			try
			{
				_addressRepository.Create(address);
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public Address Get(int addressId)
		{
			if (addressId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(addressId));
			}

			try
			{
				var address = _addressRepository.Read(addressId);
				return address;
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public List<Address> FindByCustomer(int customerId)
		{
			if (customerId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(customerId));
			}

			try
			{
				var addresss = _addressRepository.ReadAllByCustomer(customerId);
				return addresss;
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public void Update(Address address)
		{
			var validationResult = new AddressValidator().Validate(address);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			try
			{
				_addressRepository.Update(address);
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public void Delete(int addressId)
		{
			if (addressId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(addressId));
			}

			try
			{
				_addressRepository.Delete(addressId);
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public void DeleteByCustomer(int customerId)
		{
			if (customerId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(customerId));
			}

			try
			{
				_addressRepository.DeleteByCustomer(customerId);
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
