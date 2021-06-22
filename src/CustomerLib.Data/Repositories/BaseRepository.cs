using System.Data.SqlClient;

namespace CustomerLib.Data.Repositories
{
	public class BaseRepository
	{
		public static SqlConnection GetSqlConnection() => new(
			"Server=DESKTOP-95EH9IE;Database=CustomerLib_Ivasyuk;Trusted_Connection=True");
	}
}
