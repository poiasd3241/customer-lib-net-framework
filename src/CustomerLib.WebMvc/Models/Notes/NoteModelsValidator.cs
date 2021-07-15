using System.Collections.Generic;
using CustomerLib.Business.Validators;

namespace CustomerLib.WebMvc.Models.Notes
{
	public class NoteModelsValidator
	{
		private readonly NoteValidator _noteValidator = new();

		public Dictionary<string, string> ValidateEditModel(NoteEditModel model)
		{
			var result = new Dictionary<string, string>();

			var errors = _noteValidator.Validate(model.Note).Errors;

			foreach (var error in errors)
			{
				result.Add($"{nameof(NoteEditModel.Note)}.{error.PropertyName}",
									error.ErrorMessage);
			}

			return result;
		}
	}
}
