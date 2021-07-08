using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebForms.Pages.PageHelpers;

namespace CustomerLib.WebForms.Pages.Customers
{
	public partial class CustomerList : Page
	{
		#region Private Members

		private readonly ICustomerService _customerService;

		private static readonly string _defaultPageUrl = "Customers?page=1";
		private static readonly string _btnLinkDisabledCssClass = "btn btn-link disabled";

		#endregion

		#region Public Properties

		public IEnumerable<Customer> Customers { get; set; }
		public int CustomersPerPage { get; set; } = 10;

		#endregion

		#region Constructors

		public CustomerList()
		{
			_customerService = new CustomerService();
		}

		public CustomerList(ICustomerService customerService)
		{
			_customerService = customerService;
		}

		#endregion

		#region Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				return;
			}

			int.TryParse(Request.QueryString["page"], out int page);

			if (page < 1)
			{
				Response?.Redirect(_defaultPageUrl);
			}

			var customersCount = _customerService.GetCount();

			if (customersCount == 0)
			{
				InitCustomersAbscentUI();
				return;
			}

			var totalPages = (int)Math.Ceiling((double)customersCount / CustomersPerPage);

			if (page > totalPages)
			{
				// Redirect to the last page
				Response?.Redirect(GetPageUrl(Math.Max(1, totalPages)));
			}

			InitPageNavUI(totalPages, page);
			InitCustomersUI(page, customersCount);
		}

		public void InitCustomersAbscentUI()
		{
			labelCustomersAbscent.Visible = true;
			tableHeaderCustomers.Visible = false;
			customersPageNav.Visible = false;
		}

		public void InitPageNavUI(int totalPages, int currentPage)
		{
			if (currentPage == 1)
			{
				linkBtnPreviousPage.Enabled = false;
				linkBtnPreviousPage.CssClass = _btnLinkDisabledCssClass;
			}
			else
			{
				linkBtnPreviousPage.Attributes["href"] = GetPageUrl(currentPage - 1);
			}

			if (currentPage == totalPages)
			{
				linkBtnNextPage.Enabled = false;
				linkBtnNextPage.CssClass = _btnLinkDisabledCssClass;
			}
			else
			{
				linkBtnNextPage.Attributes["href"] = GetPageUrl(currentPage + 1);
			}

			labelCurrentPage.Text = $"Page {currentPage} of {totalPages}";
		}

		/// <summary>
		/// Populates the customers repeater with the database data.
		/// <br/>
		/// If the expected number of customers doesn't match the database data,
		/// the page will be redirected to try and retrieve the first page of customers data.
		/// </summary>
		/// <param name="page">The number of the page to display.</param>
		/// <param name="expectedCustomersCount">The expected total customers count. 
		/// Should be 1 or greater.</param>
		public void InitCustomersUI(int page, int expectedCustomersCount)
		{
			try
			{
				LoadCustomers(page, expectedCustomersCount);

				if (Customers is null || Customers.Count() == 0)
				{
					Response?.Redirect(_defaultPageUrl);
				}

				repeaterCustomers.DataSource = Customers;
				repeaterCustomers.DataBind();
			}
			catch (DataChangedWhileProcessingException)
			{
				Response?.Redirect(_defaultPageUrl);
			}
		}

		public void LoadCustomers(int page, int expectedCustomersCount)
		{
			Customers = _customerService.GetPage(page, CustomersPerPage, false, false,
				checkTotalSame: true, expectedTotal: expectedCustomersCount);
		}

		protected void OnDeleteCustomerCommand(object sender, CommandEventArgs e)
		{
			var customerId = int.Parse(e.CommandArgument.ToString());

			var alertMessage = DeleteCustomer(customerId)
				? $"Customer #{customerId} deleted successfully!"
				: $"Cannot delete the customer #{customerId}: it doesn't exist!";

			// Refresh the page.
			this.AlertRedirect("alertDeleteResult", alertMessage, $"{Request.Url}");

			//this.RegisterClientScript("alertDeleteResult",
			//	alert + $"window.location.href = '{Request.Url}';");
		}

		public bool DeleteCustomer(int customerId) => _customerService.Delete(customerId);

		public static string GetPageUrl(int page) => $"Customers?page={page}";

		#endregion
	}
}
