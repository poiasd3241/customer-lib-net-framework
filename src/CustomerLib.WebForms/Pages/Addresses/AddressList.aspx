<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
	CodeBehind="AddressList.aspx.cs" Inherits="CustomerLib.WebForms.Pages.Addresses.AddressList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<asp:Label runat="server" CssClass="my-4 h2 d-block" ID="labelTitle" />

	<asp:Label ID="labelCustomerDoesNotExist" runat="server" CssClass="text-danger"
		Visible="false" Text="The customer with the specified ID doesn't exist." />

	<asp:Label ID="labelAddressesAbscent" runat="server" CssClass="text-warning" Visible="false"
		Text="This customer doesn't have any addresses. You should add at least one address." />

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
						<a href="Addresses/Edit?addressId=<%# Eval("AddressId") %>"
							class="btn btn-secondary ">Edit</a>
						<asp:Button runat="server" Text="Delete" CssClass="btn btn-danger"
							OnCommand="OnDeleteAddressCommand"
							CommandArgument='<%# Eval("AddressId")%>' />
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>

	<hr />

	<asp:LinkButton runat="server" CssClass="btn btn-primary" Text="Add an address"
		ID="linkButtonAddAnAddress" />

</asp:Content>
