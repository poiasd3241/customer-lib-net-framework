namespace CustomerLib.WebMvc.Models
{
	public class FailureModel
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public string LinkText { get; set; }
		public string ActionName { get; set; }
		public string ControllerName { get; set; }
		public object RouteValues { get; set; }
	}
}
