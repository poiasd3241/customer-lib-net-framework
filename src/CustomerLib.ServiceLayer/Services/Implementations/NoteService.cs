using System;
using System.Collections.Generic;
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

		private readonly INoteRepository _noteRepository;

		#endregion

		#region Constructors

		public NoteService()
		{
			_noteRepository = new NoteRepository();
		}

		public NoteService(INoteRepository noteRepository)
		{
			_noteRepository = noteRepository;
		}

		#endregion

		#region Public Methods

		public void Save(Note note)
		{
			var validationResult = new NoteValidator().Validate(note);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			try
			{
				_noteRepository.Create(note);
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public Note Get(int noteId)
		{
			if (noteId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(noteId));
			}

			try
			{
				var note = _noteRepository.Read(noteId);
				return note;
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public List<Note> FindByCustomer(int customerId)
		{
			if (customerId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(customerId));
			}

			try
			{
				var notes = _noteRepository.ReadAllByCustomer(customerId);
				return notes;
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public void Update(Note note)
		{
			var validationResult = new NoteValidator().Validate(note);

			if (validationResult.IsValid == false)
			{
				throw new EntityValidationException(validationResult.ToString());
			}

			try
			{
				_noteRepository.Update(note);
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		public void Delete(int noteId)
		{
			if (noteId < 1)
			{
				throw new ArgumentException("Cannot be less than 1.", nameof(noteId));
			}

			try
			{
				_noteRepository.Delete(noteId);
			}
			catch
			{
				// Log / return helpful message to the user.
				throw;
			}
		}

		#endregion
	}
}
