using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Notes
{
	public class NoteModelsMapper
	{
		public Note ToEntity(NoteEditModel editModel) => new()
		{
			NoteId = editModel.Note.NoteId,
			CustomerId = editModel.Note.CustomerId,
			Content = editModel.Note.Content
		};
	}
}
