using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories.Implementations
{
	public class NoteRepository : BaseRepository, INoteRepository
	{
		#region Public Methods

		public bool Exists(int noteId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT CASE WHEN EXISTS (SELECT * FROM [dbo].[Notes] " +
				"WHERE [NoteId] = @NoteId) " +
				"THEN CAST(1 AS BIT) " +
				"ELSE CAST(0 AS BIT) " +
				"END;", connection);

			command.Parameters.Add(GetNoteIdParam(noteId));

			var result = command.ExecuteScalar();
			var exists = (bool)result;

			return exists;
		}

		public int Create(Note note)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"INSERT INTO [dbo].[Notes] " +
				"([CustomerId], [Content]) " +
				"VALUES " +
				"(@CustomerId, @Content); " +
				"SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

			command.Parameters.Add(GetCustomerIdParam(note.CustomerId));
			command.Parameters.Add(GetContentParam(note.Content));

			return (int)command.ExecuteScalar();
		}

		public Note Read(int noteId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT * FROM [dbo].[Notes] WHERE [NoteId] = @NoteId", connection);

			command.Parameters.Add(GetNoteIdParam(noteId));

			using var reader = command.ExecuteReader();

			if (reader.Read() == false)
			{
				return null;
			}

			return ReadNote(reader);
		}

		public IReadOnlyCollection<Note> ReadByCustomer(int customerId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"SELECT * FROM [dbo].[Notes] WHERE [CustomerId] = @CustomerId", connection);

			command.Parameters.Add(GetCustomerIdParam(customerId));

			using var reader = command.ExecuteReader();

			var notes = new List<Note>();

			if (reader.Read() == false)
			{
				return notes;
			}

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
				"WHERE [NoteId] = @NoteId;", connection);

			command.Parameters.Add(GetContentParam(note.Content));
			command.Parameters.Add(GetNoteIdParam(note.NoteId));

			command.ExecuteNonQuery();
		}

		public void Delete(int noteId)
		{
			using var connection = GetSqlConnection();
			connection.Open();

			var command = new SqlCommand(
				"DELETE FROM [dbo].[Notes] WHERE [NoteId] = @NoteId; ", connection);

			command.Parameters.Add(GetNoteIdParam(noteId));

			command.ExecuteNonQuery();
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
