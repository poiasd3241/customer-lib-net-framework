<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerCreate.aspx.cs" Inherits="CustomerLib.WebForms.Pages.Customers.CustomerCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<h2>New customer</h2>

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

	<%-- Address --%>
	<h4>Address</h4>

	<div class="form-group">
		<asp:Label runat="server" Text="Address line" />
		<asp:TextBox ID="inputAddressLine" AutoPostBack="true" runat="server"
			OnTextChanged="OnAddressInputTextChanged" CssClass="form-control" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorAddressLine" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputAddressLine" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="Address line2" />
		<asp:TextBox ID="inputAddressLine2" AutoPostBack="true" runat="server"
			OnTextChanged="OnAddressInputTextChanged" CssClass="form-control" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorAddressLine2" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputAddressLine2" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="Type" />
		<asp:DropDownList ID="dropDownInputType" runat="server" CssClass="form-control" />
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="City" />
		<asp:TextBox ID="inputCity" AutoPostBack="true" runat="server"
			OnTextChanged="OnAddressInputTextChanged" CssClass="form-control" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorCity" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputCity" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="Postal code" />
		<asp:TextBox ID="inputPostalCode" AutoPostBack="true" runat="server"
			OnTextChanged="OnAddressInputTextChanged" CssClass="form-control" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorPostalCode" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputPostalCode" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="State" />
		<asp:TextBox ID="inputState" AutoPostBack="true" runat="server"
			OnTextChanged="OnAddressInputTextChanged" CssClass="form-control" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Label ID="validationErrorState" runat="server" CssClass="text-danger" />
			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="inputState" />
			</Triggers>
		</asp:UpdatePanel>
	</div>

	<div class="form-group">
		<asp:Label runat="server" Text="Country" />
		<asp:DropDownList ID="dropDownInputCountry" runat="server"
			CssClass="form-control" />
	</div>

	<hr />

	<%-- Note--%>
	<h4>Note</h4>

	<asp:TextBox ID="inputNoteContent" AutoPostBack="true" TextMode="MultiLine"
		OnTextChanged="OnNoteContentChanged" CssClass="form-control" runat="server" />
	<asp:UpdatePanel runat="server">
		<ContentTemplate>
			<asp:Label ID="validationErrorNoteContent" runat="server" CssClass="text-danger" />
		</ContentTemplate>
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="inputNoteContent" />
		</Triggers>
	</asp:UpdatePanel>

	<hr />

	<p>You can add more addresses and notes once the customer is created.</p>

	<asp:Button runat="server" Text="Create" OnCommand="OnCreateCommand"
		CssClass="btn btn-success" />

</asp:Content>
