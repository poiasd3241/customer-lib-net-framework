using System;
using System.Data;
using System.Data.SqlClient;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Extensions;

namespace CustomerLib.Data.Repositories.Implementations
{
	public class CustomerRepository : BaseRepository, ICustomerRepository
	{
		#region Public Methods

		public void Create(Customer customer)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"INSERT INTO [dbo].[Customers] " +
				"([FirstName], [LastName], [PhoneNumber], [Email], [TotalPurchasesAmount]) " +
				"VALUES " +
				"(@FirstName, @LastName, @PhoneNumber, @Email, @TotalPurchasesAmount)", connection);

			command.Parameters.Add(GetFirstNameParam(customer.FirstName));
			command.Parameters.Add(GetLastNameParam(customer.LastName));
			command.Parameters.Add(GetPhoneNumberParameter(customer.PhoneNumber));
			command.Parameters.Add(GetEmailParameter(customer.Email));
			command.Parameters.Add(GetTotalPurchasesAmountParameter(customer.TotalPurchasesAmount));

			command.ExecuteNonQuery();
		}

		public Customer Read(int customerId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT * FROM [dbo].[Customers] WHERE [CustomerId] = @CustomerId", connection);

			command.Parameters.Add(GetCustomerIdParam(customerId));

			using var reader = command.ExecuteReader();

			if (reader.Read())
			{
				return new Customer()
				{
					CustomerId = customerId,
					FirstName = reader.GetValueOrDefault<string>("FirstName"),
					LastName = reader["LastName"].ToString(),
					PhoneNumber = reader.GetValueOrDefault<string>("PhoneNumber"),
					Email = reader.GetValueOrDefault<string>("Email"),
					TotalPurchasesAmount = reader.GetValueOrDefault<decimal?>(
						"TotalPurchasesAmount")
				};
			}

			return null;
		}

		public void Update(Customer customer)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"UPDATE [dbo].[Customers] " +
				"SET [FirstName] = @FirstName, [LastName] = @LastName, " +
				"[PhoneNumber] = @PhoneNumber, [Email] = @Email, " +
				"[TotalPurchasesAmount] = @TotalPurchasesAmount " +
				"WHERE [CustomerId] = @CustomerId", connection);

			command.Parameters.Add(GetFirstNameParam(customer.FirstName));
			command.Parameters.Add(GetLastNameParam(customer.LastName));
			command.Parameters.Add(GetPhoneNumberParameter(customer.PhoneNumber));
			command.Parameters.Add(GetEmailParameter(customer.Email));
			command.Parameters.Add(GetTotalPurchasesAmountParameter(customer.TotalPurchasesAmount));

			command.Parameters.Add(GetCustomerIdParam(customer.CustomerId));

			command.ExecuteNonQuery();
		}

		public void Delete(int customerId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"DELETE FROM [dbo].[Customers] WHERE [CustomerId] = @CustomerId", connection);

			command.Parameters.Add(GetCustomerIdParam(customerId));

			command.ExecuteNonQuery();
		}

		public bool IsEmailTaken(string email)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT CASE WHEN EXISTS (SELECT * FROM [dbo].[Customers] " +
				"WHERE[Email] = @Email) " +
				"THEN CAST(1 AS BIT) " +
				"ELSE CAST(0 AS BIT) " +
				"END", connection);

			command.Parameters.Add(GetEmailParameter(email));

			var result = command.ExecuteScalar();
			var isEmailTaken = (bool)result;

			return isEmailTaken;
		}

		public (bool, int) IsEmailTakenWithCustomerId(string email)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT [CustomerId] FROM [dbo].[Customers] " +
				"WHERE[Email] = @Email ", connection);

			command.Parameters.Add(GetEmailParameter(email));

			var result = command.ExecuteScalar();

			var isTaken = result is not null;
			var takenById = isTaken ? (int)result : 0;

			return (isTaken, takenById);
		}

		public static void DeleteAll()
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var deleteAddressesCommand = new SqlCommand(
					"DELETE FROM [dbo].[Addresses]", connection);
			deleteAddressesCommand.ExecuteNonQuery();

			var deleteCustomersCommand = new SqlCommand(
				"DELETE FROM [dbo].[Customers]", connection);
			deleteCustomersCommand.ExecuteNonQuery();

			var reseedAffectedTablesCommand = new SqlCommand(
				"DBCC CHECKIDENT ('dbo.Addresses', RESEED, 0) " +
				"DBCC CHECKIDENT ('dbo.Customers', RESEED, 0)", connection);
			reseedAffectedTablesCommand.ExecuteNonQuery();
		}

		#endregion

		#region Private Methods

		private static SqlParameter GetCustomerIdParam(int customerId) =>
			new("@CustomerId", SqlDbType.Int)
			{
				Value = customerId
			};
		private static SqlParameter GetFirstNameParam(string firstName) =>
			 new("@FirstName", SqlDbType.NVarChar, 50)
			 {
				 Value = firstName ?? (object)DBNull.Value
			 };
		private static SqlParameter GetLastNameParam(string lastName) =>
			 new("@LastName", SqlDbType.NVarChar, 50)
			 {
				 Value = lastName
			 };
		private static SqlParameter GetPhoneNumberParameter(string phoneNumber) =>
			 new("@PhoneNumber", SqlDbType.VarChar, 15)
			 {
				 Value = phoneNumber ?? (object)DBNull.Value
			 };
		private static SqlParameter GetEmailParameter(string email) =>
			 new("@Email", SqlDbType.VarChar, 320)
			 {
				 Value = email ?? (object)DBNull.Value
			 };
		private static SqlParameter GetTotalPurchasesAmountParameter(
			decimal? totalPurchasesAmount) =>
			new("@TotalPurchasesAmount", SqlDbType.Decimal)
			{
				Value = totalPurchasesAmount ?? (object)DBNull.Value,
				Precision = 15,
				Scale = 2
			};

		#endregion
	}
}
