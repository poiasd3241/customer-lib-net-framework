using System.Data;
using System.Data.SqlClient;
using CustomerLib.Business.Enums;
using CustomerLib.Data.Repositories;

namespace CustomerLib.Data.IntegrationTests.Repositories.TestHelpers
{
	public class AddressTypeHelperRepository : BaseRepository
	{
		/// <summary>
		/// Repopulates the AddressTypes table with the <see cref="AddressType"/> values.
		/// <br/>
		/// Doesn't clear FK-dependent table (Addresses), therefore must be used
		/// after the Addresses table is cleared.
		/// </summary>
		public static void UnsafeRepopulateAddressTypes()
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var deleteAddresseTypesCommand = new SqlCommand(
				"DELETE FROM [dbo].[AddressTypes]", connection);
			deleteAddresseTypesCommand.ExecuteNonQuery();

			var populateAddresseTypesCommand = new SqlCommand(
				"INSERT INTO [dbo].[AddressTypes] " +
				"([AddressTypeId], [Type]) " +
				"VALUES " +
				"(@ShippingId, @ShippingType), (@BillingId, @BillingType)", connection);

			var shippingIdParam = new SqlParameter("@ShippingId", SqlDbType.Int)
			{
				Value = (int)AddressType.Shipping
			};
			var shippingTypeParam = new SqlParameter("@ShippingType", SqlDbType.VarChar, 8)
			{
				Value = AddressType.Shipping.ToString()
			};
			var billingIdParam = new SqlParameter("@BillingId", SqlDbType.Int)
			{
				Value = (int)AddressType.Billing
			};
			var billingTypeParam = new SqlParameter("@BillingType", SqlDbType.VarChar, 8)
			{
				Value = AddressType.Billing.ToString()
			};

			populateAddresseTypesCommand.Parameters.Add(shippingIdParam);
			populateAddresseTypesCommand.Parameters.Add(shippingTypeParam);
			populateAddresseTypesCommand.Parameters.Add(billingIdParam);
			populateAddresseTypesCommand.Parameters.Add(billingTypeParam);

			populateAddresseTypesCommand.ExecuteNonQuery();
		}
	}
}
