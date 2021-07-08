<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NoteList.aspx.cs" Inherits="CustomerLib.WebForms.Pages.Notes.NoteList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<asp:Label runat="server" CssClass="my-4 h2 d-block" ID="labelTitle" />

	<asp:Label ID="labelCustomerDoesNotExist" runat="server" CssClass="text-danger"
		Visible="false" Text="The customer with the specified ID doesn't exist." />

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
						<a href="Notes/Edit?noteId=<%# Eval("NoteId") %>"
							class="btn btn-secondary ">Edit</a>
						<asp:Button runat="server" Text="Delete" CssClass="btn btn-danger"
							OnCommand="OnDeleteNoteCommand"
							CommandArgument='<%# Eval("NoteId")%>' />
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>

	<hr />

	<asp:LinkButton runat="server" CssClass="btn btn-primary" Text="Add a note"
		ID="linkButtonAddNote" />

</asp:Content>
