using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace CustomerLib.WebForms
{
	public class Global : HttpApplication
	{
		void Application_Start(object sender, EventArgs e)
		{
			// Code that runs on application startup
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		/// <summary>
		/// Store last server error here to counter null error caused by UpdatePanels.
		/// </summary>
		public static Exception SavedError { get; private set; }

		void Application_Error(object sender, EventArgs e)
		{
			var error = Server.GetLastError();
			Server.ClearError();

			if (error is not null)
			{
				SavedError = error;
			}

			if (SavedError is not null)
			{
				Response?.Redirect("/Error");
			}
		}
	}
}