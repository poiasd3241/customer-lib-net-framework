using System.Web.Routing;

namespace CustomerLib.WebForms
{
	public static class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.MapPageRoute("Error", "Error", "~/Pages/Error.aspx");

			routes.MapPageRoute("Customers", "Customers", "~/Pages/Customers/CustomerList.aspx");
			routes.MapPageRoute("CustomerCreate", "Customers/Create",
				"~/Pages/Customers/CustomerCreate.aspx");
			routes.MapPageRoute("CustomerEdit", "Customers/Edit",
				"~/Pages/Customers/CustomerEdit.aspx");

			routes.MapPageRoute("Addresses", "Addresses", "~/Pages/Addresses/AddressList.aspx");
			// Create & Edit in one page.
			routes.MapPageRoute("AddressEdit", "Addresses/{mode}",
				"~/Pages/Addresses/AddressEdit.aspx");

			routes.MapPageRoute("Notes", "Notes", "~/Pages/Notes/NoteList.aspx");
			// Create & Edit in one page.
			routes.MapPageRoute("NoteEdit", "Notes/{mode}", "~/Pages/Notes/NoteEdit.aspx");
		}
	}
}
