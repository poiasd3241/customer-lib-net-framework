<%@ Page Title="Customers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
	CodeBehind="CustomerList.aspx.cs"
	Inherits="CustomerLib.WebForms.Pages.Customers.CustomerList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<h2 class="my-4">All customers</h2>

	<asp:Label ID="labelCustomersAbscent" runat="server" Visible="false"
		Text="There are currently no customers in the database." />

	<table class="table">
		<tr runat="server" id="tableHeaderCustomers">
			<th>First name</th>
			<th>Last name</th>
			<th>Phone number</th>
			<th>Email</th>
			<th>Total purchases amount</th>
			<th>Notes</th>
			<th>Addresses</th>
			<th></th>
		</tr>
		<asp:Repeater ID="repeaterCustomers" runat="server"
			ItemType="CustomerLib.Business.Entities.Customer">
			<ItemTemplate>
				<tr>
					<td class="text-break"><%# Eval("FirstName")%></td>
					<td class="text-break"><%# Eval("LastName") %></td>
					<td><%# Eval("PhoneNumber") %></td>
					<td class="text-break"><%# Eval("Email") %></td>
					<td><%# Eval("TotalPurchasesAmount") %></td>
					<td>
						<a href="Notes?customerId=<%# Eval("CustomerId") %>"
							class="btn btn-link">Notes</a>
					</td>
					<td>
						<a href="Addresses?customerId=<%# Eval("CustomerId") %>"
							class="btn btn-link">Addresses</a>
					</td>
					<td>
						<a href="Customers/Edit?customerId=<%# Eval("CustomerId") %>"
							class="btn btn-secondary">Edit</a>
						<asp:Button runat="server" Text="Delete" CssClass="btn btn-danger"
							OnCommand="OnDeleteCustomerCommand"
							CommandArgument='<%# Eval("CustomerId")%>' />
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>

	<%-- Pagination navigation --%>
	<div class="d-flex flex-row" runat="server" id="customersPageNav">
		<asp:LinkButton ID="linkBtnPreviousPage" runat="server" CssClass="btn btn-link"
			Text="Previous" />
		<asp:Label ID="labelCurrentPage" runat="server" CssClass="align-self-center px-2" />
		<asp:LinkButton ID="linkBtnNextPage" runat="server" CssClass="btn btn-link" Text="Next" />
	</div>

	<hr />

	<asp:LinkButton runat="server" href="Customers/Create" CssClass="btn btn-primary"
		Text="Create a new customer" />

</asp:Content>
