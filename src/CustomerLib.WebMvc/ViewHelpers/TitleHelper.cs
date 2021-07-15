using System.Text;
using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.ViewHelpers
{
	public class TitleHelper
	{
		public static string GetTitleNoteCreate(Customer customer) =>
			$"New note for the customer {GetCustomerNameAndEmailText(customer)}";

		public static string GetTitleNoteEdit(Customer customer) =>
			$"Edit the note for the customer {GetCustomerNameAndEmailText(customer)}";

		public static string GetTitleNotes(Customer customer) =>
			$"Notes for the customer {GetCustomerNameAndEmailText(customer)}";

		public static string GetTitleAddressCreate(Customer customer) =>
			$"New address for the customer {GetCustomerNameAndEmailText(customer)}";

		public static string GetTitleAddressEdit(Customer customer) =>
			$"Edit the address for the customer {GetCustomerNameAndEmailText(customer)}";

		public static string GetTitleAddresses(Customer customer) =>
			$"Addresses for the customer {GetCustomerNameAndEmailText(customer)}";

		public static string GetCustomerNameAndEmailText(Customer customer)
		{
			var sb = new StringBuilder();

			if (customer.FirstName is not null)
			{
				sb.Append(customer.FirstName);
				sb.Append(' ');
			}

			sb.Append(customer.LastName);

			if (customer.Email is not null)
			{
				sb.Append(' ');
				sb.Append('(');
				sb.Append(customer.Email);
				sb.Append(')');
			}

			return sb.ToString();
		}
	}
}
