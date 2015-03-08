<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ASamplePage.aspx.cs" Inherits="WebForms.Pages.ASamplePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <asp:Label ID="Label1" runat="server" Text="Label">A Property 1</asp:Label>
    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox><br />
    <asp:Label ID="Label2" runat="server" Text="Label">A Property 2</asp:Label>
    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    <br /><br />
    <asp:Button ID="ButtonSave" runat="server" Text="Save" OnClick="ButtonSave_Click" /><asp:Button ID="ButtonUpdate" runat="server" Text="Update" OnClick="ButtonUpdate_Click" />
    <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
</asp:Content>
