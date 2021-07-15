using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Addresses
{
	public interface IAddressModelsMapper
	{
		Address ToEntity(AddressDetailsModel detailsModel);
		Address ToEntity(AddressEditModel editModel);
	}
}
