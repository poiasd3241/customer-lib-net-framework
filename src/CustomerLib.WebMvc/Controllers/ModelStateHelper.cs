using System.Collections.Generic;
using System.Web.Mvc;

namespace CustomerLib.WebMvc.Controllers
{
	public class ModelStateHelper
	{
		public static void AddErrors(ModelStateDictionary modelState,
			Dictionary<string, string> errors)
		{
			if (errors.Count > 0)
			{
				foreach (var error in errors)
				{
					modelState.AddModelError(error.Key, error.Value);
				}
			}
		}
	}
}
