using CustomerLib.Business.Entities;
using CustomerLib.Business.Localization;
using FluentValidation;

namespace CustomerLib.Business.Validators
{
	/// <summary>
	/// The fluent validator of <see cref="Note"/> objects.
	/// </summary>
	public class NoteValidator : AbstractValidator<Note>
	{
		private static readonly int _note_max_length = 1000;

		public NoteValidator()
		{
			// Content
			RuleFor(note => note.Content).Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(ValidationRules.NOTE_EMPTY_OR_WHITESPACE)
				.MaximumLength(_note_max_length).WithMessage(
					string.Format(ValidationRules.NOTE_MAX_LENGTH, _note_max_length));
		}
	}
}
