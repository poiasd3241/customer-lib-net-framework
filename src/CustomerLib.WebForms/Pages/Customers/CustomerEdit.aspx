<%@ Page Title="Customer edit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
	CodeBehind="CustomerEdit.aspx.cs" Inherits="CustomerLib.WebForms.Pages.Customers.CustomerEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Edit customer</h2>

	<hr />

	<%-- Basic details --%>
	<h4>Basic details</h4>

	<div class="form-group">
		<asp:Label runat="server" Text="First name" />
		<asp:TextBox ID="inputFirstName" AutoPostBack="true"
			OnTextChanged="OnInputTextChanged" CssClass="form-control" runat="server" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorFirstName" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputFirstName" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="Last name" />
		<asp:TextBox ID="inputLastName" AutoPostBack="true"
			OnTextChanged="OnInputTextChanged" CssClass="form-control" runat="server" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorLastName" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputLastName" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="Phone number" />
		<asp:TextBox ID="inputPhoneNumber" AutoPostBack="true"
			OnTextChanged="OnInputTextChanged" CssClass="form-control" runat="server" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorPhoneNumber" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputPhoneNumber" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="Email" />
		<asp:TextBox ID="inputEmail" AutoPostBack="true" OnTextChanged="OnInputTextChanged"
			CssClass="form-control" runat="server" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorEmail" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputEmail" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="Total purchases amount" />
		<asp:TextBox ID="inputTotalPurchasesAmount" AutoPostBack="true"
			OnTextChanged="OnInputTextChanged" CssClass="form-control" runat="server" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorTotalPurchasesAmount" runat="server"
					CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputTotalPurchasesAmount" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<hr />

	<asp:Button runat="server" Text="Save" OnCommand="OnSaveCommand" ID="btnSave"
		CssClass="btn btn-success" />

	<hr />

	<%-- Addresses --%>
	<h4>Addresses</h4>

	<asp:Label ID="labelAddressesAbscent" runat="server" CssClass="text-warning"
		Visible="false" Text="This customer doesn't have any addresses. You should add
				at least one address." />

	<table class="table">
		<tr runat="server" id="tableHeaderAddresses">
			<th>Address line</th>
			<th>Address line2</th>
			<th>Type</th>
			<th>City</th>
			<th>Postal Code</th>
			<th>State</th>
			<th>Country</th>
			<th></th>
		</tr>
		<asp:Repeater ID="repeaterAddresses" runat="server"
			ItemType="CustomerLib.Business.Entities.Address">
			<ItemTemplate>
				<tr>
					<td class="text-break"><%# Eval("AddressLine")%></td>
					<td class="text-break"><%# Eval("AddressLine2") %></td>
					<td><%# Eval("Type") %></td>
					<td class="text-break"><%# Eval("City") %></td>
					<td><%# Eval("PostalCode") %></td>
					<td class="text-break"><%# Eval("State") %></td>
					<td class="text-break"><%# Eval("Country") %></td>
					<td>
						<a href="/Addresses/Edit?addressId=<%# Eval("AddressId") %>"
							class="btn btn-secondary ">Edit</a>
						<asp:Button runat="server" Text="Delete" CssClass="btn btn-danger"
							OnCommand="OnDeleteAddressCommand"
							CommandArgument='<%# Eval("AddressId")%>' />
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>

	<asp:LinkButton runat="server" Text="Add an address" CssClass="btn btn-primary"
		ID="linkButtonAddAddress" />

	<hr />

	<%-- Notes--%>
	<h4>Notes</h4>

	<asp:Label ID="labelNotesAbscent" runat="server" CssClass="text-warning" Visible="false"
		Text="This customer doesn't have any notes. You should add at least one note." />

	<table class="table">
		<tr runat="server" id="tableHeaderNotes">
			<th>Content</th>
			<th></th>
		</tr>
		<asp:Repeater ID="repeaterNotes" runat="server"
			ItemType="CustomerLib.Business.Entities.Note">
			<ItemTemplate>
				<tr>
					<td class="text-break"><%# Eval("Content")%></td>
					<td>
						<a href="/Notes/Edit?noteId=<%# Eval("NoteId") %>"
							class="btn btn-secondary ">Edit</a>
						<asp:Button runat="server" Text="Delete" CssClass="btn btn-danger"
							OnCommand="OnDeleteNoteCommand"
							CommandArgument='<%# Eval("NoteId")%>' />
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>

	<asp:LinkButton runat="server" Text="Add a note" CssClass="btn btn-primary"
		ID="linkButtonAddNote" />

</asp:Content>
