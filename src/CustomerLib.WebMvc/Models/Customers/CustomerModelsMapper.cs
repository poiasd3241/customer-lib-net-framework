using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Addresses;

namespace CustomerLib.WebMvc.Models.Customers
{
	public class CustomerModelsMapper : ICustomerModelsMapper
	{
		private readonly IAddressModelsMapper _addressModelsMapper;

		public CustomerModelsMapper(IAddressModelsMapper addressModelsMapper)
		{
			_addressModelsMapper = addressModelsMapper;
		}

		public Customer ToEntity(CustomerBasicDetailsModel basicDetailsModel) => new()
		{
			CustomerId = basicDetailsModel.CustomerId,
			FirstName = basicDetailsModel.FirstName,
			LastName = basicDetailsModel.LastName,
			PhoneNumber = basicDetailsModel.PhoneNumber,
			Email = basicDetailsModel.Email,
			TotalPurchasesAmount = string.IsNullOrEmpty(basicDetailsModel.TotalPurchasesAmount)
				? null
				: decimal.Parse(basicDetailsModel.TotalPurchasesAmount),
			Addresses = null,
			Notes = null
		};

		public Customer ToEntity(CustomerCreateModel createModel)
		{
			var customer = ToEntity(createModel.BasicDetails);

			customer.Addresses = new()
			{
				_addressModelsMapper.ToEntity(createModel.AddressDetails)
			};

			customer.Notes = new() { createModel.Note };

			return customer;
		}
	}
}