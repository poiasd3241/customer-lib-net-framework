using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Addresses;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Addresses
{
	public class AddressesModelTest
	{
		[Fact]
		public void ShouldThrowOnCreateWithNullAdresses()
		{
			var exception = Assert.Throws<ArgumentException>(() => new AddressesModel(null));

			Assert.Equal("addresses", exception.ParamName);
		}

		[Fact]
		public void ShouldCreateAddressesModelFromAddresses()
		{
			// Given
			var addresses = MockAddresses();

			// When
			var model = new AddressesModel(addresses);

			// Then
			Assert.Null(model.Title);
			Assert.Equal(addresses, model.Addresses);
			Assert.True(model.HasAddresses);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given, When
			var addresses = MockAddresses();
			var model = new AddressesModel(addresses) { Title = "t" };

			// Then
			Assert.Equal("t", model.Title);
		}

		private class HasAddressesData : TheoryData<List<Address>, bool>
		{
			public HasAddressesData()
			{
				Add(new(), false);
				Add(MockAddresses(), true);
			}
		}

		[Theory]
		[ClassData(typeof(HasAddressesData))]
		public void ShouldCheckIfHasAddresses(List<Address> addresses, bool hasAddresses)
		{
			// Given, When
			var model = new AddressesModel(addresses);

			// Then
			Assert.Equal(hasAddresses, model.HasAddresses);
		}

		private static List<Address> MockAddresses() => new()
		{
			new(),
			new()
		};
	}
}
