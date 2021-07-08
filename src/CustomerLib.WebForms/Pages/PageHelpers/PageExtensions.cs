using System.Web.UI;

namespace CustomerLib.WebForms.Pages.PageHelpers
{
	public static class PageExtensions
	{
		public static void Alert(
			this Page page, string key, string alertMessage, bool addScriptTags = true) =>
			ScriptManager.RegisterClientScriptBlock(
				page, page.GetType(), key, $"alert(`{alertMessage}`);", addScriptTags);

		public static void AlertRedirect(
			this Page page, string key, string alertMessage, string hrefRedirect,
			bool addScriptTags = true) =>
			ScriptManager.RegisterClientScriptBlock(
				page, page.GetType(), key,
				$"alert(`{alertMessage}`);window.location.href = '{hrefRedirect}';",
				addScriptTags);
	}
}
