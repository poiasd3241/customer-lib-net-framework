using Xunit;

namespace CustomerLib.WebMvc.Models
{
	public class FailureModelTest
	{
		[Fact]
		public void ShouldCreateFailureModel()
		{
			var model = new FailureModel();

			Assert.Null(model.Title);
			Assert.Null(model.Message);
			Assert.Null(model.LinkText);
			Assert.Null(model.ActionName);
			Assert.Null(model.ControllerName);
			Assert.Null(model.RouteValues);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given
			var title = "t";
			var message = "message";
			var linkText = "link";
			var actionName = "action";
			var controllerName = "controller";
			var routeValues = new { whatever = "whatever" };

			var model = new FailureModel();

			Assert.NotEqual(title, model.Title);
			Assert.NotEqual(message, model.Message);
			Assert.NotEqual(linkText, model.LinkText);
			Assert.NotEqual(actionName, model.ActionName);
			Assert.NotEqual(controllerName, model.ControllerName);
			Assert.NotEqual(routeValues, model.RouteValues);

			// When
			model.Title = title;
			model.Message = message;
			model.LinkText = linkText;
			model.ActionName = actionName;
			model.ControllerName = controllerName;
			model.RouteValues = routeValues;

			// Then
			Assert.Equal(title, model.Title);
			Assert.Equal(message, model.Message);
			Assert.Equal(linkText, model.LinkText);
			Assert.Equal(actionName, model.ActionName);
			Assert.Equal(controllerName, model.ControllerName);
			Assert.Equal(routeValues, model.RouteValues);
		}
	}
}
