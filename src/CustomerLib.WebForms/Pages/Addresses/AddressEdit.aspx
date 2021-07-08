<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddressEdit.aspx.cs" Inherits="CustomerLib.WebForms.Pages.Addresses.AddressEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<asp:Label runat="server" CssClass="my-4 h2 d-block" ID="labelTitle" />

	<hr />

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

	<asp:Button runat="server" OnCommand="OnSaveCommand" ID="btnSave"
		CssClass="btn btn-success" />

</asp:Content>
