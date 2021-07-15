using System.Collections.Generic;
using System.Transactions;
using CustomerLib.Business.ArgumentCheckHelpers;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.Business.Validators;
using CustomerLib.Data.Repositories;
using CustomerLib.Data.Repositories.Implementations;

namespace CustomerLib.ServiceLayer.Services.Implementations
{
	public class NoteService : INoteService
	{
		#region Private Members

		private readonly ICustomerRepository _customerRepository;
		private readonly INoteRepository _noteRepository;

		#endregion

		#region Constructors

		public NoteService()
		{
			_customerRepository = new CustomerRepository();
			_noteRepository = new NoteRepository();
		}

		public NoteService(ICustomerRepository customerRepository, INoteRepository noteRepository)
		{
			_customerRepository = customerRepository;
			_noteRepository = noteRepository;
		}

		#endregion

		#region Public Methods

		public bool Exists(int noteId)
		{
			CheckNumber.NotLessThan(1, noteId, nameof(noteId));

			var result = _noteRepository.Exists(noteId);
			return result;
		}

		public bool Save(Note note)
		{
			var validationResult = new NoteValidator().Validate(note);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			using TransactionScope scope = new();

			if (_customerRepository.Exists(note.CustomerId) == false)
			{
				return false;
			}

			_noteRepository.Create(note);

			scope.Complete();

			return true;
		}

		public Note Get(int noteId)
		{
			CheckNumber.NotLessThan(1, noteId, nameof(noteId));

			var note = _noteRepository.Read(noteId);
			return note;
		}

		public IReadOnlyCollection<Note> FindByCustomer(int customerId)
		{
			CheckNumber.NotLessThan(1, customerId, nameof(customerId));

			var notes = _noteRepository.ReadByCustomer(customerId);
			return notes;
		}

		/// <summary>
		/// Updates the note.
		/// </summary>
		/// <param name="note">The note to update.</param>
		/// <returns><see langword="true"/> if the update completed successfully; 
		/// <see langword="false"/> if the provided note is not in the database.</returns>

		public bool Update(Note note)
		{
			var validationResult = new NoteValidator().Validate(note);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			using TransactionScope scope = new();

			if (_noteRepository.Exists(note.NoteId) == false)
			{
				return false;
			}

			_noteRepository.Update(note);

			scope.Complete();

			return true;
		}

		/// <summary>
		/// Deletes the note.
		/// </summary>
		/// <param name="noteId">The ID of the note to delete.</param>
		/// <returns><see langword="true"/> if the deletion completed successfully; 
		/// <see langword="false"/> if the provided note (by ID) is not in the database.</returns>

		public bool Delete(int noteId)
		{
			CheckNumber.NotLessThan(1, noteId, nameof(noteId));

			using TransactionScope scope = new();

			if (_noteRepository.Exists(noteId) == false)
			{
				return false;
			}

			_noteRepository.Delete(noteId);

			scope.Complete();

			return true;
		}

		#endregion
	}
}
