using System.Web.UI.WebControls;

namespace CustomerLib.WebForms.Validation
{
	public class CustomerInputValidator
	{
		public bool ValidateTotalPurchasesAmount(string input, out decimal? validValue)
		{
			if (decimal.TryParse(input, out decimal value))
			{
				validValue = value;
				return true;
			}

			validValue = null;

			return string.IsNullOrEmpty(input);
		}

		public void AdjustTotalPurchasesAmountErrorLabel(bool isValid, Label validationErrorLabel)
		{
			if (isValid)
			{
				validationErrorLabel.Visible = false;
				return;
			}

			validationErrorLabel.Text = "Total purchases amount must be a decimal number." +
				"<br />" +
				"*optional - clear the field if you don't have any value to enter.";
			validationErrorLabel.Visible = true;
		}
	}
}
