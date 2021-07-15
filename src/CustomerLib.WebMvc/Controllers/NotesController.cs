using System.Web.Mvc;
using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebMvc.Models;
using CustomerLib.WebMvc.Models.Notes;
using CustomerLib.WebMvc.ViewHelpers;

namespace CustomerLib.WebMvc.Controllers
{
	[Route("Customers/{customerId:int}/Notes/{action=Index}/{noteId:int?}")]
	public class NotesController : Controller
	{
		#region Private Members

		private readonly ICustomerService _customerService;
		private readonly INoteService _noteService;
		private readonly INoteModelsMapper _noteModelsMapper;
		private readonly NoteModelsValidator _noteModelsValidator = new();

		#endregion

		#region Constructors

		public NotesController()
		{
			_customerService = new CustomerService();
			_noteService = new NoteService();
		}

		public NotesController(ICustomerService customerService, INoteService noteService,
			INoteModelsMapper noteModelsMapper)
		{
			_customerService = customerService;
			_noteService = noteService;
			_noteModelsMapper = noteModelsMapper;
		}

		#endregion

		#region Actions

		// GET: Customers/4/Notes
		public ActionResult Index(int customerId)
		{
			var customer = _customerService.Get(customerId, false, true);

			if (IsCustomerNotFound(customer))
			{
				return View("NotFound");
			}

			var notesModel = new NotesModel(customer.Notes)
			{
				Title = TitleHelper.GetTitleNotes(customer)
			};

			return View(notesModel);
		}

		// GET: Customers/4/Notes/Create
		public ActionResult Create(int customerId)
		{
			var customer = _customerService.Get(customerId, false, false);

			if (IsCustomerNotFound(customer))
			{
				return View("NotFound");
			}

			var noteCreateModel = new NoteEditModel(new Note() { CustomerId = customerId })
			{
				Title = TitleHelper.GetTitleNoteCreate(customer),
				SubmitButtonText = "Create"
			};

			return View(noteCreateModel);
		}

		// POST: Customers/4/Notes/Create
		[HttpPost]
		public ActionResult Create(NoteEditModel model)
		{
			var errors = _noteModelsValidator.ValidateEditModel(model);

			ModelStateHelper.AddErrors(ModelState, errors);

			if (ModelState.IsValid == false)
			{
				return View(model);
			}

			var note = _noteModelsMapper.ToEntity(model);

			if (_noteService.Save(note) == false)
			{
				var failureModel = new FailureModel()
				{
					Title = "Note_Create",
					Message =
						$"Cannot create a note for the non-existing customer #{note.CustomerId}!",
					LinkText = "To customers list",
					ActionName = "Index",
					ControllerName = "Customers"
				};

				return View("Failure", failureModel);
			}

			return RedirectToAction("Index");
		}

		// GET: Customers/4/Notes/Edit/5
		public ActionResult Edit(int customerId, int noteId)
		{
			var customer = _customerService.Get(customerId, false, false);

			if (IsCustomerNotFound(customer))
			{
				return View("NotFound");
			}

			// TODO: combine the check for [note belongs to the customer]
			var note = _noteService.Get(noteId);

			if (IsNoteNotFound(note) ||
				note.CustomerId != customer.CustomerId)
			{
				return View("NotFound");
			}

			var noteEditModel = new NoteEditModel(note)
			{
				Title = TitleHelper.GetTitleNoteEdit(customer),
				SubmitButtonText = "Save"
			};

			return View(noteEditModel);
		}

		// POST: Customers/4/Notes/Edit/5
		[HttpPost]
		public ActionResult Edit(NoteEditModel model)
		{
			// TODO: ensure the note belongs to the customer

			var errors = _noteModelsValidator.ValidateEditModel(model);

			ModelStateHelper.AddErrors(ModelState, errors);

			if (ModelState.IsValid == false)
			{
				return View(model);
			}

			var note = _noteModelsMapper.ToEntity(model);

			if (_noteService.Update(note) == false)
			{
				var failureModel = new FailureModel()
				{
					Title = "Note_Edit",
					Message = $"Cannot edit the note #{note.NoteId}: it doesn't exist!",
					LinkText = "To notes list",
					ActionName = "Index",
					ControllerName = "Notes",
					RouteValues = new { customerId = note.CustomerId }
				};

				return View("Failure", failureModel);
			}

			return RedirectToAction("Index");
		}

		// GET: Customers/4/Notes/Delete/5
		public ActionResult Delete(int customerId, int noteId)
		{
			// TODO: ensure the note belongs to the customer

			if (IsCustomerNotFound(customerId))
			{
				return View("NotFound");
			}

			var foundAndDeleted = _noteService.Delete(noteId);

			if (foundAndDeleted == false)
			{
				var failureModel = new FailureModel()
				{
					Title = "Note_Delete",
					Message = $"Cannot delete the note #{noteId}: it doesn't exist!",
					LinkText = "To notes list",
					ActionName = "Index",
					ControllerName = "Notes"
				};

				return View("Failure", failureModel);
			}

			return RedirectToAction("Index");
		}

		#endregion

		#region Private Methods

		private bool IsCustomerNotFound(int customerId) =>
			_customerService.Exists(customerId) == false;
		private bool IsNoteNotFound(Note note) => note is null;
		private bool IsCustomerNotFound(Customer customer) => customer is null;

		#endregion
	}
}
