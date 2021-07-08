using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Validators;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebForms.Pages.PageHelpers;
using CustomerLib.WebForms.Validation;
using FluentValidation.Results;

namespace CustomerLib.WebForms.Pages.Notes
{
	public partial class NoteEdit : Page
	{
		#region Private Members

		private readonly ICustomerService _customerService;
		private readonly INoteService _noteService;
		private readonly NoteValidator _noteValidator = new();

		private readonly EntityInputValidator _entityInputValidator = new();

		#endregion

		#region Public Properties

		public Note Note { get; set; }
		public int CustomerId { get; set; }
		public bool IsCreate { get; set; }

		#endregion

		#region Constructors

		public NoteEdit()
		{
			_customerService = new CustomerService();
			_noteService = new NoteService();
		}

		public NoteEdit(ICustomerService customerService, INoteService noteService)
		{
			_customerService = customerService;
			_noteService = noteService;
		}

		#endregion

		#region Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack == false)
			{
				// Create mode.
				var isCustomerIdSpecified = int.TryParse(Request.QueryString["customerId"],
					out int customerId);

				// Edit mode
				var isAddressIdSpecified = int.TryParse(Request.QueryString["noteId"],
					out int noteId);

				if (Request.QueryString.Count != 1 ||
					isAddressIdSpecified == false && isCustomerIdSpecified == false)
				{
					throw new HttpException(400, "Bad Request");
				}

				var type = (string)RouteData.Values["mode"];

				if (type != "Create" && type != "Edit" ||
					type == "Create" && isAddressIdSpecified ||
					type == "Edit" && isCustomerIdSpecified)
				{
					throw new HttpException(400, "Bad Request");
				}

				if (type == "Create")
				{
					if (CheckCustomerExists(customerId) == false)
					{
						return;
					}

					IsCreate = true;
					CustomerId = customerId;
					Note = new() { CustomerId = CustomerId };
				}
				else
				{
					if (LoadNote(noteId) == false)
					{
						return;
					}

					IsCreate = false;
					inputNoteContent.Text = Note.Content;
				}

				InitCustomerDependentUI();
			}
		}

		public bool CheckCustomerExists(int customerId)
		{
			if (_customerService.Exists(customerId))
			{
				return true;
			}

			this.AlertRedirect("alertCustomerDoesNotExist",
				$"The customer #{customerId} doesn't exist!",
				"/Customers");
			return false;
		}

		public bool LoadNote(int addressId)
		{
			Note = _noteService.Get(addressId);

			if (Note is null)
			{
				this.AlertRedirect("alertNoteDoesNotExist",
					$"The note #{addressId} doesn't exist!",
					"/Customers");
				return false;
			}

			return true;
		}

		public Customer LoadCustomer(int customerId) =>
			_customerService.Get(customerId, false, false);

		public void InitCustomerDependentUI()
		{
			var customer = LoadCustomer(Note.CustomerId);

			labelTitle.Text = IsCreate
				? TitleHelper.GetTitleNoteCreate(customer)
				: TitleHelper.GetTitleNoteEdit(customer);

			btnSave.Text = IsCreate ? "Create" : "Save";
		}

		protected override object SaveViewState()
		{
			ViewState["Note"] = Note;
			ViewState["IsCreate"] = IsCreate;

			if (IsCreate)
			{
				ViewState["CustomerId"] = CustomerId;
			}

			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			Note = (Note)ViewState["Note"];
			IsCreate = (bool)ViewState["IsCreate"];

			if (IsCreate)
			{
				CustomerId = (int)ViewState["CustomerId"];
			}
		}

		public void OnSaveCommand(object sender, CommandEventArgs e)
		{
			if (ValidateAll() == false)
			{
				this.Alert("alertInputErrors", "Please correct the input errors.");
				return;
			}

			if (IsCreate)
			{
				CreateNote();
			}
			else
			{
				SaveExistingNote();
			}
		}

		public void CreateNote()
		{
			_noteService.Save(Note);

			// Redirect to the customer's notes.
			this.AlertRedirect("alertCreateSuccess", "Note created successfully!",
				$"/Notes?customerId={CustomerId}");
		}

		public void SaveExistingNote()
		{
			if (_noteService.Update(Note))
			{
				this.Alert("alertUpdateResult", $"Note #{Note.NoteId} updated successfully!");
			}
			else
			{
				this.AlertRedirect("alertUpdateResult",
					$"Cannot update the note #{Note.NoteId}: it doesn't exist!",
					"/Customers");
			}
		}

		#endregion

		#region Validation

		/// <summary>
		/// Validates the note according to <see cref="AddressValidator"/>.
		/// </summary>
		/// <returns>True if the note is valid, otherwise false.</returns>
		public bool ValidateAll()
		{
			var allValid = ValidateNoteAndAdjustErrorLabel(inputNoteContent.Text);

			var noteResult = _noteValidator.Validate(Note);

			if (allValid && noteResult.IsValid == false)
			{
				throw new Exception("Mismatch in Note validation.");
			}

			return allValid;
		}

		protected void OnNoteContentChanged(object sender, EventArgs e) =>
			ValidateNoteAndAdjustErrorLabel(inputNoteContent.Text);

		public ValidationResult ValidateNote(string content)
		{
			if (content == "")
			{
				content = null;
			}

			Note.Content = content;

			return _entityInputValidator.ValidatePropertyResult(Note, "Content");
		}

		public bool ValidateNoteAndAdjustErrorLabel(string content)
		{
			var result = ValidateNote(content);

			_entityInputValidator.AdjustErrorLabel(result, validationErrorNoteContent);

			return result.IsValid;
		}

		#endregion
	}
}