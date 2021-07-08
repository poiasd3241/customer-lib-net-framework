using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebForms.Pages.PageHelpers;

namespace CustomerLib.WebForms.Pages.Notes
{
	public partial class NoteList : Page
	{
		#region Private Members

		private readonly ICustomerService _customerService;
		private readonly INoteService _noteService;

		#endregion

		#region Constructors

		public NoteList(ICustomerService customerService, INoteService noteService)
		{
			_customerService = customerService;
			_noteService = noteService;
		}

		public NoteList()
		{
			_customerService = new CustomerService();
			_noteService = new NoteService();
		}

		#endregion

		#region Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				return;
			}

			if (int.TryParse(Request.QueryString["customerId"], out int customerId) == false ||
				Request.QueryString.Count != 1)
			{
				throw new HttpException(400, "Bad Request");
			}

			InitUI(customerId);
		}

		public void InitUI(int customerId)
		{
			var customer = LoadCustomerWithNotes(customerId);

			if (customer is null)
			{
				labelCustomerDoesNotExist.Visible = true;
				tableHeaderNotes.Visible = false;
				labelTitle.Text = "Notes for the customer";
				return;
			}

			labelTitle.Text = TitleHelper.GetTitleNotes(customer);

			var notes = customer.Notes;

			if (notes is null || notes.Count == 0)
			{
				tableHeaderNotes.Visible = false;
				labelNotesAbscent.Visible = true;
			}
			else
			{
				repeaterNotes.DataSource = notes;
				repeaterNotes.DataBind();
			}

			linkButtonAddNote.Attributes["href"] =
				$"Notes/Create?customerId={customer.CustomerId}";
		}

		public Customer LoadCustomerWithNotes(int customerId) =>
			_customerService.Get(customerId, false, true);

		protected void OnDeleteNoteCommand(object sender, CommandEventArgs e)
		{
			var noteId = int.Parse(e.CommandArgument.ToString());

			var alertMessage = DeleteNote(noteId)
				? $"Note #{noteId} deleted successfully!"
				: $"Cannot delete the note #{noteId}: it doesn't exist!";

			// Refresh the page.
			this.AlertRedirect("alertDeleteResult", alertMessage, $"{Request.Url}");

			//this.RegisterClientScript("alertDeleteResult",
			//	alert + $"window.location.href = '{Request.Url}';");
		}

		public bool DeleteNote(int noteId) => _noteService.Delete(noteId);

		#endregion
	}
}