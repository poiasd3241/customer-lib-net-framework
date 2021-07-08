<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NoteEdit.aspx.cs" Inherits="CustomerLib.WebForms.Pages.Notes.NoteEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<asp:Label runat="server" CssClass="my-4 h2 d-block" ID="labelTitle" />

	<hr />

	<div class="form-group">
		<asp:Label runat="server" Text="Content" />

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
	</div>

	<asp:Button runat="server" OnCommand="OnSaveCommand" ID="btnSave"
		CssClass="btn btn-success" />

</asp:Content>
