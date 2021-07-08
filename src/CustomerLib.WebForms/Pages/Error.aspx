<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="CustomerLib.WebForms.Pages.Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<h2 class="text-danger">Error</h2>
	<hr />

	<asp:Label ID="labelException" runat="server" CssClass="text-monospace small" />

</asp:Content>
