using System;
using System.Web.UI;

namespace CustomerLib.WebForms.Pages
{
	public partial class Error : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var error = Global.SavedError;

			if (error is null)
			{
				labelException.Text = "Null error in Application_Error.";
			}
			else
			{
				labelException.Text = error.ToString();
			}
		}
	}
}