using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Customers
{
	public interface ICustomerModelsMapper
	{
		Customer ToEntity(CustomerBasicDetailsModel basicDetailsModel);
		Customer ToEntity(CustomerCreateModel createModel);
	}
}