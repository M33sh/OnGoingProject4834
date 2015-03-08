<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditUserProfile.aspx.cs" Inherits="WebForms.Pages.EditUserProfile" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="InputForm">
    <table>
        <tr>
            <td>Full Name</td>
            <td>
                <asp:TextBox ID="tbFullName" runat="server"></asp:TextBox></td>
                
        </tr>
        <tr>
            <td>Address</td>
            <td>
                <asp:TextBox ID="tbAddress" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>City</td>
            <td>
                <asp:TextBox ID="tbCity" runat="server"></asp:TextBox></td>
        </tr>
         <tr>
            <td>State</td>
            <td>
                <asp:TextBox ID="tbState" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Phone</td>
            <td>
                <asp:TextBox ID="tbPhone" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Email</td>
            <td>
                <asp:TextBox ID="tbEmail" runat="server"></asp:TextBox></td>
        </tr>
    </table>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        <asp:Button ID="ButtonUpd" runat="server" CommandName="UpdateProfile" CommandArgument='<%#Eval("id") %>' Text="Update" OnClick="ButtonUpd_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
        <asp:Button ID="BtnReturn" runat="server" OnClick="BtnReturn_Click" Text="Return to Home" />
        </div>

</asp:Content> 


