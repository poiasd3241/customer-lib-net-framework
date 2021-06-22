namespace CustomerLib.Business.Entities
{
	public class Note
	{
		public int NoteId { get; set; }
		public int CustomerId { get; set; }
		public string Content { get; set; }
	}
}
