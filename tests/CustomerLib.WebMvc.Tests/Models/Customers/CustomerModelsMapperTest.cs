using CustomerLib.Business.Entities;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Models.Addresses;
using CustomerLib.WebMvc.Models.Customers;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Customers
{
	public class CustomerModelsMapperTest
	{
		private class TotalPurchasesAmountData : TheoryData<string, decimal?>
		{
			public TotalPurchasesAmountData()
			{
				Add(null, null);
				Add("", null);
				Add("666", 666);
			}
		}

		[Theory]
		[ClassData(typeof(TotalPurchasesAmountData))]
		public void ShouldMapBasicDetailsModelToEntity(string totalPurchasesAmountText,
			decimal? totalPurchasesAmountNumber)
		{
			// Given
			var customer = MockCustomer();
			customer.TotalPurchasesAmount = totalPurchasesAmountNumber;

			var model = new CustomerBasicDetailsModel(customer)
			{
				TotalPurchasesAmount = totalPurchasesAmountText
			};

			var mockAddressModelsMapper = MockAddressModelsMapper();

			var mapper = new CustomerModelsMapper(mockAddressModelsMapper.Object);

			// When
			var entity = mapper.ToEntity(model);

			// Then
			Assert.True(customer.EqualsByValue(entity));
		}

		[Fact]
		public void ShouldMapCreateModelToEntity()
		{
			// Given
			var note = MockNote();
			var address = MockAddress();
			var customer = MockCustomer();

			customer.Addresses = new() { address };
			customer.Notes = new() { note };

			var model = new CustomerCreateModel(customer, address)
			{
				Note = note
			};

			var mockAddressModelsMapper = MockAddressModelsMapper();
			mockAddressModelsMapper.Setup(m => m.ToEntity(model.AddressDetails)).Returns(address);

			var mapper = new CustomerModelsMapper(mockAddressModelsMapper.Object);

			// When
			var entity = mapper.ToEntity(model);

			// Then
			Assert.True(customer.EqualsByValue(entity));
		}

		private Customer MockCustomer() => new()
		{
			CustomerId = 5,
			FirstName = "one",
			LastName = "two",
			PhoneNumber = "+123",
			Email = "a@a.aa",
			TotalPurchasesAmount = null,
			Addresses = null,
			Notes = null
		};

		private Address MockAddress() => new()
		{
			AddressId = 5,
			CustomerId = 8,
			AddressLine = "1",
			AddressLine2 = "2",
			Type = Business.Enums.AddressType.Billing,
			City = "city x",
			PostalCode = "111",
			State = "state x",
			Country = "Canada"
		};

		private Note MockNote() => new()
		{
			NoteId = 5,
			CustomerId = 8,
			Content = "text"
		};

		private StrictMock<IAddressModelsMapper> MockAddressModelsMapper() => new();
	}
}
