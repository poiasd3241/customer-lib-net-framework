using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Addresses
{
	public class AddressModelsMapper : IAddressModelsMapper
	{
		public Address ToEntity(AddressDetailsModel detailsModel) => new()
		{
			AddressId = detailsModel.Address.AddressId,
			CustomerId = detailsModel.Address.CustomerId,
			AddressLine = detailsModel.Address.AddressLine,
			AddressLine2 = detailsModel.Address.AddressLine2,
			Type = detailsModel.Address.Type,
			City = detailsModel.Address.City,
			PostalCode = detailsModel.Address.PostalCode,
			State = detailsModel.Address.State,
			Country = detailsModel.Address.Country
		};

		public Address ToEntity(AddressEditModel editModel) => ToEntity(editModel.AddressDetails);
	}
}
