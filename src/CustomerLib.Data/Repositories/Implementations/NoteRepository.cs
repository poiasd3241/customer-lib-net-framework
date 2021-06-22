using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories.Implementations
{
	public class NoteRepository : BaseRepository, INoteRepository
	{
		#region Public Methods

		public void Create(Note note)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"INSERT INTO [dbo].[Notes] " +
				"([CustomerId], [Content]) " +
				"VALUES " +
				"(@CustomerId, @Content)", connection);

			command.Parameters.Add(GetCustomerIdParam(note.CustomerId));
			command.Parameters.Add(GetContentParam(note.Content));

			command.ExecuteNonQuery();
		}

		public Note Read(int noteId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT * FROM [dbo].[Notes] WHERE [NoteId] = @NoteId", connection);

			command.Parameters.Add(GetNoteIdParam(noteId));

			using var reader = command.ExecuteReader();

			if (reader.Read())
			{
				return ReadNote(reader);
			}

			return null;
		}

		public List<Note> ReadAllByCustomer(int customerId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT * FROM [dbo].[Notes] WHERE [CustomerId] = @CustomerId", connection);

			command.Parameters.Add(GetCustomerIdParam(customerId));

			using var reader = command.ExecuteReader();

			if (reader.Read() == false)
			{
				return null;
			}

			var notes = new List<Note>();

			do
			{
				notes.Add(ReadNote(reader));
			} while (reader.Read());

			return notes;
		}

		public void Update(Note note)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"UPDATE [dbo].[Notes] " +
				"SET [Content] = @Content " +
				"WHERE [NoteId] = @NoteId", connection);

			command.Parameters.Add(GetContentParam(note.Content));
			command.Parameters.Add(GetNoteIdParam(note.NoteId));

			command.ExecuteNonQuery();
		}

		public void Delete(int noteId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"DELETE FROM [dbo].[Notes] WHERE [NoteId] = @NoteId", connection);

			command.Parameters.Add(GetNoteIdParam(noteId));

			command.ExecuteReader();
		}

		public static void DeleteAll()
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var deleteNotesCommand = new SqlCommand(
				"DELETE FROM [dbo].[Notes]", connection);
			deleteNotesCommand.ExecuteNonQuery();

			var reseedAffectedTableCommand = new SqlCommand(
				"DBCC CHECKIDENT ('dbo.Notes', RESEED, 0)", connection);
			reseedAffectedTableCommand.ExecuteNonQuery();
		}

		#endregion

		#region Private Methods

		private static Note ReadNote(IDataRecord dataRecord) => new()
		{
			NoteId = (int)dataRecord["NoteId"],
			CustomerId = (int)dataRecord["CustomerId"],
			Content = dataRecord["Content"].ToString()
		};

		private static SqlParameter GetNoteIdParam(int noteId) =>
			new("@NoteId", SqlDbType.Int)
			{
				Value = noteId
			};

		private static SqlParameter GetCustomerIdParam(int customerId) =>
			new("@CustomerId", SqlDbType.Int)
			{
				Value = customerId
			};

		private static SqlParameter GetContentParam(string content) =>
			 new("@Content", SqlDbType.NVarChar, 1000)
			 {
				 Value = content
			 };

		#endregion
	}
}
