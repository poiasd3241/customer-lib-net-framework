using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories
{
	public interface ICustomerRepository
	{
		bool Exists(int customerId);

		/// <returns>The Id of the created item.</returns>
		int Create(Customer customer);
		Customer Read(int customerId);
		IReadOnlyCollection<Customer> ReadAll();
		int GetCount();
		IReadOnlyCollection<Customer> ReadPage(int page, int pageSize);
		void Update(Customer customer);
		void Delete(int customerId);

		/// <summary>
		/// Checks if the specified email is taken by any user.
		/// </summary>
		/// <param name="email">The email to check.</param>
		/// <returns>true if the email is present in a repository; otherwise, false.</returns>
		bool IsEmailTaken(string email);

		/// <summary>
		/// Checks if the specified email is taken by any user.
		/// </summary>
		/// <param name="email">The email to check.</param>
		/// <returns>(isTaken, takenById)
		/// <br/>
		/// - isTaken: true if the email is present in a repository; otherwise, false;
		/// <br/> 
		/// - takenById: the Id of the customer the email belongs to. Should be ignored when 
		/// the email is not found ("isTaken" is false).</returns>
		(bool, int) IsEmailTakenWithCustomerId(string email);
	}
}
