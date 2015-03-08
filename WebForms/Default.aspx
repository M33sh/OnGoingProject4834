<%@ Page Title=" Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebForms._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1>Web Systems II<%: Title %></h1>
            </hgroup>
            <p>
                Lots of neat stuff all around. Seriously.</p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>I suggest the following:</h3>
    <ol class="round">
        <li class="one">
            <h5>Register Your Profile!</h5>
            See the link in the upper right corner to register your new account - OR - Feel free to log into your existing account.</li>
        <li class="two">
            <h5>Check out the Movie GridView Database!</h5>
            Feel free to add your own favorites. Click here -->
            <a href="http://michelesimms.azurewebsites.net/Pages/MovieGridView.aspx">Movies</a>
        </li>        
    </ol>
</asp:Content>
