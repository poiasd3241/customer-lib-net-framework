using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Notes
{
	public class NoteEditModel
	{
		public string Title { get; set; }
		public string SubmitButtonText { get; set; }
		public Note Note { get; set; }

		public NoteEditModel(Note note)
		{
			Note = note;
		}

		public NoteEditModel() { }
	}
}
