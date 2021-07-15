using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using Xunit;

namespace CustomerLib.Business.Tests.Entities
{
	public class AddressTest
	{
		[Fact]
		public void ShouldCreateAddress()
		{
			Address address = new();

			Assert.Equal(0, address.AddressId);
			Assert.Equal(0, address.CustomerId);
			Assert.Null(address.AddressLine);
			Assert.Null(address.AddressLine2);
			Assert.Equal(0, (int)address.Type);
			Assert.Null(address.City);
			Assert.Null(address.PostalCode);
			Assert.Null(address.State);
			Assert.Null(address.Country);
		}

		[Fact]
		public void ShouldSetAddressProperties()
		{
			var text = "a";
			var type = AddressType.Billing;

			Address address = new();

			address.AddressId = 1;
			address.CustomerId = 1;
			address.AddressLine = text;
			address.AddressLine2 = text;
			address.Type = type;
			address.City = text;
			address.PostalCode = text;
			address.State = text;
			address.Country = text;

			Assert.Equal(1, address.AddressId);
			Assert.Equal(1, address.CustomerId);
			Assert.Equal(text, address.AddressLine);
			Assert.Equal(text, address.AddressLine2);
			Assert.Equal(type, address.Type);
			Assert.Equal(text, address.City);
			Assert.Equal(text, address.PostalCode);
			Assert.Equal(text, address.State);
			Assert.Equal(text, address.Country);
		}

		#region Equals by value

		[Fact]
		public void ShouldThrowOnEqualsByValueByBadObjectType()
		{
			// Given
			var address1 = new Address();
			var whatever = "whatever";

			// When
			var exception = Assert.Throws<ArgumentException>(() =>
				address1.EqualsByValue(whatever));

			// Then
			Assert.Equal("Must use the same entity type for comparison", exception.Message);
		}

		[Fact]
		public void ShouldConfirmEqualsByValue()
		{
			// Given
			var address1 = MockAddress();
			var address2 = MockAddress();

			// When
			var equalsByValue = address1.EqualsByValue(address2);

			// Then
			Assert.True(equalsByValue);
		}

		[Fact]
		public void ShouldRefuteEqualsByValueByNull()
		{
			// Given
			var address1 = MockAddress();
			Address address2 = null;

			// When
			var equalsByValue = address1.EqualsByValue(address2);

			// Then
			Assert.False(equalsByValue);
		}

		[Fact]
		public void ShouldRefuteEqualsByValueByAddressId()
		{
			// Given
			var addressId1 = 5;
			var addressId2 = 7;

			var address1 = MockAddress();
			var address2 = MockAddress();

			address1.AddressId = addressId1;
			address2.AddressId = addressId2;

			// When
			var equalsByValue = address1.EqualsByValue(address2);

			// Then
			Assert.False(equalsByValue);
		}

		#endregion

		#region Lists equal by value

		private class NullAndNotNullListsData : TheoryData<List<Address>, List<Address>>
		{
			public NullAndNotNullListsData()
			{
				Add(null, new());
				Add(new(), null);
			}
		}

		[Theory]
		[ClassData(typeof(NullAndNotNullListsData))]
		public void ShouldRefuteListsEqualByValueByOneListNull(
			List<Address> list1, List<Address> list2)
		{
			// When
			var equalByValue = Address.ListsEqualByValues(list1, list2);

			// Then
			Assert.False(equalByValue);
		}

		[Fact]
		public void ShouldRefuteListsEqualByValueByCountMismatch()
		{
			// Given
			var list1 = new List<Address>();
			var list2 = new List<Address>() { new() };

			// When
			var equalByValue = Address.ListsEqualByValues(list1, list2);

			// Then
			Assert.False(equalByValue);
		}

		[Fact]
		public void ShouldConfirmListsEqualByValueByBothNull()
		{
			// Given
			List<Address> list1 = null;
			List<Address> list2 = null;

			// When
			var equalByValue = Address.ListsEqualByValues(list1, list2);

			// Then
			Assert.True(equalByValue);
		}

		private class NotNullEqualListsData : TheoryData<List<Address>, List<Address>>
		{
			public NotNullEqualListsData()
			{
				Add(new(), new());

				Add(new() { null }, new() { null });
				Add(new() { MockAddress() }, new() { MockAddress() });
			}
		}

		[Theory]
		[ClassData(typeof(NotNullEqualListsData))]
		public void ShouldConfirmListsEqualNotNull(List<Address> list1, List<Address> list2)
		{
			// When
			var equalByValue = Address.ListsEqualByValues(list1, list2);

			// Then
			Assert.True(equalByValue);
		}

		private class NotNullNotEqualListsData : TheoryData<List<Address>, List<Address>>
		{
			public NotNullNotEqualListsData()
			{
				Add(new() { null }, new() { MockAddress() });
				Add(new() { MockAddress() }, new() { null });

				var addressId1 = 5;
				var addressId2 = 7;

				var address1 = MockAddress();
				var address2 = MockAddress();

				address1.AddressId = addressId1;
				address2.AddressId = addressId2;

				Add(new() { address1 }, new() { address2 });
			}
		}

		[Theory]
		[ClassData(typeof(NotNullNotEqualListsData))]
		public void ShouldRefuteListsEqualNotNull(List<Address> list1, List<Address> list2)
		{
			// When
			var equalByValue = Address.ListsEqualByValues(list1, list2);

			// Then
			Assert.False(equalByValue);
		}

		#endregion

		private static Address MockAddress() => new()
		{
			AddressId = 5,
			CustomerId = 8,
			AddressLine = "one",
			AddressLine2 = "two",
			Type = AddressType.Shipping,
			City = "Seattle",
			PostalCode = "123456",
			State = "WA",
			Country = "United States"
		};
	}
}
