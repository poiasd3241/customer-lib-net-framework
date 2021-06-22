using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories
{
	public interface ICustomerRepository
	{
		void Create(Customer customer);
		Customer Read(int customerId);
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
