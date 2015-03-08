<%@ Page Title="You are Logged In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DefaultLoggedIn.aspx.cs" Inherits="WebForms._DefaultLoggedIn" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Click on Edit Profile in the upper right corner to make changes to your user profile!</h2>
            </hgroup>
            <p>
                Explore.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>I suggest the following:</h3>
    <ol class="round">
        <li class="one">
            <h5>Update Your Profile!</h5>
            See the link in the upper right corner to Edit Your Profile!</li>
        <li class="two">
            <h5>Check out the Movie GridView Database!</h5>
            Feel free to add your own favorites. Click here -->
            <a href="http://michelesimms.azurewebsites.net/Pages/MovieGridView.aspx">Movies</a>
        </li>        
    </ol>
</asp:Content>
