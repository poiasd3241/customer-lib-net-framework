using System.Collections.Generic;
using System.Web.Mvc;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Localization;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Controllers;
using CustomerLib.WebMvc.Models;
using CustomerLib.WebMvc.Models.Notes;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Controllers
{
	public class NotesControllerTest
	{
		[Fact]
		public void ShouldCreateNotesControllerDefaultConstructor()
		{
			var controller = new NotesController();

			Assert.NotNull(controller);
		}

		[Fact]
		public void ShouldCreateNotesController()
		{
			var mockCustomerService = new StrictMock<ICustomerService>();
			var mockNoteService = new StrictMock<INoteService>();
			var mockNoteModelsMapper = new StrictMock<INoteModelsMapper>();

			var controller = new NotesController(mockCustomerService.Object,
				mockNoteService.Object, mockNoteModelsMapper.Object);

			Assert.NotNull(controller);
		}

		#region Index - GET

		[Fact]
		public void ShouldReturnNotFoundViewOnGetIndexByCustomerNotFound()
		{
			// Given
			var customerId = 5;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, true))
				.Returns((Customer)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Index(customerId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnIndexViewOnIndexGet()
		{
			// Given
			var customerId = 5;
			var notes = new List<Note>() { MockNote() };

			var customer = MockCustomer();
			customer.CustomerId = 5;
			customer.Notes = notes;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, true))
				.Returns(customer);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Index(customerId);

			// Then	
			var model = result.Model;
			Assert.True(model is NotesModel);
			var notesModel = (NotesModel)model;

			Assert.Equal("Notes for the customer One Two (a@a.aa)", notesModel.Title);
			Assert.True(notesModel.HasNotes);
			Assert.Equal(notes, notesModel.Notes);
		}

		#endregion

		#region Create - GET

		[Fact]
		public void ShouldReturnNotFoundViewOnGetCreateByCustomerNotFound()
		{
			// Given
			var customerId = 5;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns((Customer)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Create(customerId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnCreateViewOnGetCreate()
		{
			// Given
			var customerId = 5;

			var customer = MockCustomer();
			customer.CustomerId = 5;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns(customer);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Create(customerId);

			// Then	
			var model = result.Model;
			Assert.True(model is NoteEditModel);
			var createModel = (NoteEditModel)model;

			Assert.Equal("New note for the customer One Two (a@a.aa)", createModel.Title);
			Assert.Equal("Create", createModel.SubmitButtonText);

			Assert.Equal(customerId, createModel.Note.CustomerId);
			Assert.Null(createModel.Note.Content);
		}

		#endregion

		#region Create - POST

		[Fact]
		public void ShouldReturnCreateViewOnPostCreateByInvalidModel()
		{
			// Given
			var invalidNote = MockNote();
			invalidNote.Content = "  ";

			var title = "create";
			var submitButtonText = "submit";

			var invalidModel = new NoteEditModel(invalidNote)
			{
				Title = title,
				SubmitButtonText = submitButtonText
			};

			var controller = new NotesControllerFixture().CreateController();

			// When
			var result = (ViewResult)controller.Create(invalidModel);

			// Then	
			var model = result.Model;
			Assert.True(model is NoteEditModel);

			var createModel = (NoteEditModel)model;
			Assert.Equal(invalidModel, createModel);

			var modelState = Assert.Single(result.ViewData.ModelState);
			Assert.Equal("Note.Content", modelState.Key);

			var error = Assert.Single(modelState.Value.Errors);
			Assert.Equal(ValidationRules.NOTE_EMPTY_OR_WHITESPACE, error.ErrorMessage);
		}

		[Fact]
		public void ShouldReturnFailureViewOnPostCreateBySaveFail()
		{
			// Given
			var note = MockNote();

			var createModel = new NoteEditModel(note);

			var fixture = new NotesControllerFixture();
			fixture.MockNoteModelsMapper.Setup(m => m.ToEntity(createModel)).Returns(note);
			fixture.MockNoteService.Setup(s => s.Save(note)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Create(createModel);

			// Then	
			Assert.Equal("Failure", result.ViewName);

			var model = result.Model;
			Assert.True(model is FailureModel);

			var failureModel = (FailureModel)model;
			Assert.Equal("Note_Create", failureModel.Title);
			Assert.Equal("Cannot create a note for the non-existing customer #5!",
				failureModel.Message);
			Assert.Equal("To customers list", failureModel.LinkText);
			Assert.Equal("Index", failureModel.ActionName);
			Assert.Equal("Customers", failureModel.ControllerName);
			Assert.Null(failureModel.RouteValues);
		}

		[Fact]
		public void ShouldRedirectToIndexOnPostCreateSuccess()
		{
			// Given
			var note = MockNote();

			var createModel = new NoteEditModel(note);

			var fixture = new NotesControllerFixture();
			fixture.MockNoteModelsMapper.Setup(m => m.ToEntity(createModel)).Returns(note);
			fixture.MockNoteService.Setup(s => s.Save(note)).Returns(true);

			var controller = fixture.CreateController();

			// When
			var result = (RedirectToRouteResult)controller.Create(createModel);

			// Then	
			var routeValue = Assert.Single(result.RouteValues);

			Assert.Equal("action", routeValue.Key);
			Assert.Equal("Index", routeValue.Value);
		}

		#endregion

		#region Edit - GET

		[Fact]
		public void ShouldReturnNotFoundViewOnGetEditByCustomerNotFound()
		{
			// Given
			var customerId = 5;
			var noteId = 7;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns((Customer)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerId, noteId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnNotFoundViewOnGetEditByNoteNotFound()
		{
			// Given
			var customerId = 5;
			var noteId = 7;

			var customer = MockCustomer();
			customer.CustomerId = customerId;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns(customer);
			fixture.MockNoteService.Setup(s => s.Get(noteId)).Returns((Note)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerId, noteId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnNotFoundViewOnGetEditByCustomerIdMismatch()
		{
			// Given
			var customerIdCustomer = 5;
			var customerIdNote = 8;
			var noteId = 7;

			var customer = MockCustomer();
			customer.CustomerId = customerIdCustomer;

			var note = MockNote();
			note.CustomerId = customerIdNote;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerIdCustomer, false, false))
				.Returns(customer);
			fixture.MockNoteService.Setup(s => s.Get(noteId)).Returns(note);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerIdCustomer, noteId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnEditViewOnGetEdit()
		{
			// Given
			var customerId = 5;
			var noteId = 7;

			var customer = MockCustomer();
			customer.CustomerId = customerId;

			var note = MockNote();
			note.NoteId = noteId;
			note.CustomerId = customerId;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns(customer);
			fixture.MockNoteService.Setup(s => s.Get(noteId)).Returns(note);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerId, noteId);

			// Then	
			var model = result.Model;
			Assert.True(model is NoteEditModel);
			var createModel = (NoteEditModel)model;

			Assert.Equal("Edit the note for the customer One Two (a@a.aa)", createModel.Title);
			Assert.Equal("Save", createModel.SubmitButtonText);

			Assert.Equal(noteId, createModel.Note.NoteId);
			Assert.Equal(customerId, createModel.Note.CustomerId);
			Assert.Equal("text", createModel.Note.Content);
		}

		#endregion

		#region Edit - POST

		[Fact]
		public void ShouldReturnEditViewOnPostEditByInvalidModel()
		{
			// Given
			var invalidNote = MockNote();
			invalidNote.Content = "  ";

			var title = "edit";
			var submitButtonText = "submit";

			var invalidModel = new NoteEditModel(invalidNote)
			{
				Title = title,
				SubmitButtonText = submitButtonText
			};

			var controller = new NotesControllerFixture().CreateController();

			// When
			var result = (ViewResult)controller.Edit(invalidModel);

			// Then	
			var model = result.Model;
			Assert.True(model is NoteEditModel);

			var createModel = (NoteEditModel)model;
			Assert.Equal(invalidModel, createModel);

			var modelState = Assert.Single(result.ViewData.ModelState);
			Assert.Equal("Note.Content", modelState.Key);

			var error = Assert.Single(modelState.Value.Errors);
			Assert.Equal(ValidationRules.NOTE_EMPTY_OR_WHITESPACE, error.ErrorMessage);
		}

		[Fact]
		public void ShouldReturnFailureViewOnPostEditByUpdateFail()
		{
			// Given
			var note = MockNote();

			var editModel = new NoteEditModel(note);

			var fixture = new NotesControllerFixture();
			fixture.MockNoteModelsMapper.Setup(m => m.ToEntity(editModel)).Returns(note);
			fixture.MockNoteService.Setup(s => s.Update(note)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(editModel);

			// Then	
			Assert.Equal("Failure", result.ViewName);

			var model = result.Model;
			Assert.True(model is FailureModel);

			var failureModel = (FailureModel)model;
			Assert.Equal("Note_Edit", failureModel.Title);
			Assert.Equal("Cannot edit the note #3: it doesn't exist!", failureModel.Message);
			Assert.Equal("To notes list", failureModel.LinkText);
			Assert.Equal("Index", failureModel.ActionName);
			Assert.Equal("Notes", failureModel.ControllerName);

			var routeValue = Assert.Single(failureModel.RouteValues.GetType().GetProperties());
			Assert.Equal("customerId", routeValue.Name);
			Assert.Equal(5, routeValue.GetValue(failureModel.RouteValues));
		}

		[Fact]
		public void ShouldRedirectToIndexOnPostEditSuccess()
		{
			// Given
			var note = MockNote();

			var editModel = new NoteEditModel(note);

			var fixture = new NotesControllerFixture();
			fixture.MockNoteModelsMapper.Setup(m => m.ToEntity(editModel)).Returns(note);
			fixture.MockNoteService.Setup(s => s.Update(note)).Returns(true);

			var controller = fixture.CreateController();

			// When
			var result = (RedirectToRouteResult)controller.Edit(editModel);

			// Then	
			var routeValue = Assert.Single(result.RouteValues);

			Assert.Equal("action", routeValue.Key);
			Assert.Equal("Index", routeValue.Value);
		}

		#endregion

		#region Delete - GET

		[Fact]
		public void ShouldReturnNotFoundViewOnGetDeleteByCustomerNotFound()
		{
			// Given
			var customerId = 5;
			var noteId = 7;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Delete(customerId, noteId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnFailureViewOnGetDeleteByNoteNotFound()
		{
			// Given
			var customerId = 5;
			var noteId = 7;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(true);
			fixture.MockNoteService.Setup(s => s.Delete(noteId)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Delete(customerId, noteId);

			// Then	
			Assert.Equal("Failure", result.ViewName);

			var model = result.Model;
			Assert.True(model is FailureModel);

			var failureModel = (FailureModel)model;
			Assert.Equal("Note_Delete", failureModel.Title);
			Assert.Equal("Cannot delete the note #7: it doesn't exist!", failureModel.Message);
			Assert.Equal("To notes list", failureModel.LinkText);
			Assert.Equal("Index", failureModel.ActionName);
			Assert.Equal("Notes", failureModel.ControllerName);
			Assert.Null(failureModel.RouteValues);
		}

		[Fact]
		public void ShouldRedirectToIndexViewOnGetDeleteSuccess()
		{
			// Given
			var customerId = 5;
			var noteId = 7;

			var fixture = new NotesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(true);
			fixture.MockNoteService.Setup(s => s.Delete(noteId)).Returns(true);

			var controller = fixture.CreateController();

			// When
			var result = (RedirectToRouteResult)controller.Delete(customerId, noteId);

			// Then	
			var routeValue = Assert.Single(result.RouteValues);

			Assert.Equal("action", routeValue.Key);
			Assert.Equal("Index", routeValue.Value);
		}

		#endregion

		#region Fixture, object mock helpers

		public class NotesControllerFixture
		{
			public StrictMock<ICustomerService> MockCustomerService { get; set; }
			public StrictMock<INoteService> MockNoteService { get; set; }
			public StrictMock<INoteModelsMapper> MockNoteModelsMapper { get; set; }

			public NotesControllerFixture()
			{
				MockCustomerService = new();
				MockNoteService = new();
				MockNoteModelsMapper = new();
			}

			public NotesController CreateController() =>
				new(MockCustomerService.Object, MockNoteService.Object,
					MockNoteModelsMapper.Object);
		}

		public static Customer MockCustomer() => new()
		{
			CustomerId = 5,
			FirstName = "One",
			LastName = "Two",
			PhoneNumber = "+123",
			Email = "a@a.aa",
			TotalPurchasesAmount = 666,
			Addresses = null,
			Notes = null
		};

		public static Note MockNote() => new()
		{
			NoteId = 3,
			CustomerId = 5,
			Content = "text"
		};

		#endregion
	}
}
