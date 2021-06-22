using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Extensions;

namespace CustomerLib.Data.Repositories.Implementations
{
	public class AddressRepository : BaseRepository, IAddressRepository
	{
		#region Public Methods

		public void Create(Address address)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"INSERT INTO [dbo].[Addresses] " +
				"([CustomerId], [AddressLine], [AddressLine2], [AddressTypeId], [City], " +
				"[PostalCode], [State], [Country]) " +
				"VALUES " +
				"(@CustomerId, @AddressLine, @AddressLine2, @AddressTypeId, @City, " +
				"@PostalCode, @State, @Country)", connection);

			command.Parameters.Add(GetCustomerIdParam(address.CustomerId));
			command.Parameters.Add(GetAddressLineParam(address.AddressLine));
			command.Parameters.Add(GetAddressLine2Param(address.AddressLine2));
			command.Parameters.Add(GetAddressTypeIdParam((int)address.Type));
			command.Parameters.Add(GetCityParam(address.City));
			command.Parameters.Add(GetPostalCodeParam(address.PostalCode));
			command.Parameters.Add(GetStateParam(address.State));
			command.Parameters.Add(GetCountryParam(address.Country));

			command.ExecuteNonQuery();
		}

		public Address Read(int addressId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT * FROM [dbo].[Addresses] WHERE [AddressId] = @AddressId", connection);

			command.Parameters.Add(GetAddressIdParam(addressId));

			using var reader = command.ExecuteReader();

			if (reader.Read())
			{
				return ReadAddress(reader);
			}

			return null;
		}

		public List<Address> ReadAllByCustomer(int customerId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT * FROM [dbo].[Addresses] WHERE [CustomerId] = @CustomerId", connection);

			command.Parameters.Add(GetCustomerIdParam(customerId));

			using var reader = command.ExecuteReader();

			if (reader.Read() == false)
			{
				return null;
			}

			var addresss = new List<Address>();

			do
			{
				addresss.Add(ReadAddress(reader));
			} while (reader.Read());

			return addresss;
		}

		public void Update(Address address)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"UPDATE [dbo].[Addresses] " +
				"SET [CustomerId] = @CustomerId, [AddressLine] = @AddressLine, " +
				"[AddressLine2] = AddressLine2, [AddressTypeId] = @AddressTypeId, " +
				"[City] = @City, [PostalCode] = @PostalCode, [State] = @State, " +
				"[Country] = @Country " +
				"WHERE [AddressId] = @AddressId", connection);

			command.Parameters.Add(GetCustomerIdParam(address.CustomerId));
			command.Parameters.Add(GetAddressLineParam(address.AddressLine));
			command.Parameters.Add(GetAddressLine2Param(address.AddressLine2));
			command.Parameters.Add(GetAddressTypeIdParam((int)address.Type));
			command.Parameters.Add(GetCityParam(address.City));
			command.Parameters.Add(GetPostalCodeParam(address.PostalCode));
			command.Parameters.Add(GetStateParam(address.State));
			command.Parameters.Add(GetCountryParam(address.Country));

			command.Parameters.Add(GetAddressIdParam(address.AddressId));

			command.ExecuteNonQuery();
		}

		public void Delete(int addressId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"DELETE FROM [dbo].[Addresses] WHERE [AddressId] = @AddressId", connection);

			command.Parameters.Add(GetAddressIdParam(addressId));

			command.ExecuteReader();
		}

		public void DeleteByCustomer(int customerId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"DELETE FROM [dbo].[Addresses] WHERE [CustomerId] = @CustomerId", connection);

			command.Parameters.Add(GetCustomerIdParam(customerId));

			command.ExecuteReader();
		}

		public static void DeleteAll()
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var deleteAddressesCommand = new SqlCommand(
				"DELETE FROM [dbo].[Addresses]", connection);
			deleteAddressesCommand.ExecuteNonQuery();

			var reseedAffectedTableCommand = new SqlCommand(
				"DBCC CHECKIDENT ('dbo.Addresses', RESEED, 0)", connection);
			reseedAffectedTableCommand.ExecuteNonQuery();
		}

		#endregion

		#region Private Methods

		private static Address ReadAddress(IDataRecord dataRecord) => new()
		{
			AddressId = (int)dataRecord["CustomerId"],
			CustomerId = (int)dataRecord["CustomerId"],
			AddressLine = dataRecord["AddressLine"].ToString(),
			AddressLine2 = dataRecord.GetValueOrDefault<string>("AddressLine2"),
			Type = (AddressType)dataRecord["AddressTypeId"],
			City = dataRecord["City"].ToString(),
			PostalCode = dataRecord["PostalCode"].ToString(),
			State = dataRecord["State"].ToString(),
			Country = dataRecord["Country"].ToString()
		};

		private static SqlParameter GetAddressIdParam(int addressId) =>
			new("@AddressId", SqlDbType.Int)
			{
				Value = addressId
			};
		private static SqlParameter GetCustomerIdParam(int customerId) =>
			new("@CustomerId", SqlDbType.Int)
			{
				Value = customerId
			};
		private static SqlParameter GetAddressLineParam(string addressLine) =>
			 new("@AddressLine", SqlDbType.NVarChar, 100)
			 {
				 Value = addressLine
			 };
		private static SqlParameter GetAddressLine2Param(string addressLine2) =>
			 new("@AddressLine2", SqlDbType.NVarChar, 100)
			 {
				 Value = addressLine2 ?? (object)DBNull.Value
			 };
		private static SqlParameter GetAddressTypeIdParam(int addressTypeId) =>
			new("@AddressTypeId", SqlDbType.Int)
			{
				Value = addressTypeId
			};
		private static SqlParameter GetCityParam(string city) =>
			 new("@City", SqlDbType.NVarChar, 50)
			 {
				 Value = city
			 };
		private static SqlParameter GetPostalCodeParam(string postalCode) =>
			 new("@PostalCode", SqlDbType.VarChar, 6)
			 {
				 Value = postalCode
			 };
		private static SqlParameter GetStateParam(string state) =>
			 new("@State", SqlDbType.NVarChar, 20)
			 {
				 Value = state
			 };
		private static SqlParameter GetCountryParam(string country) =>
			 new("@Country", SqlDbType.NVarChar, 13)
			 {
				 Value = country
			 };

		#endregion
	}
}
