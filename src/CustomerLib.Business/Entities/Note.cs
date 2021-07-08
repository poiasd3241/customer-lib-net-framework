using System;

namespace CustomerLib.Business.Entities
{
	[Serializable]
	public class Note : Entity
	{
		public int NoteId { get; set; }
		public int CustomerId { get; set; }
		public string Content { get; set; }
	}
}
