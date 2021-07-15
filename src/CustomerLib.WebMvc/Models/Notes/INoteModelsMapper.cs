using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Notes
{
	public interface INoteModelsMapper
	{
		Note ToEntity(NoteEditModel editModel);
	}
}
