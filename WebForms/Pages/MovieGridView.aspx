<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MovieGridView.aspx.cs" Inherits="WebForms.Pages.MovieGridView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <strong>Below is our Movie GridView Database! After inserting a new movie, please click &quot;Refresh Movies&quot; to view the update!</strong>
    <asp:GridView ID="GridView1" runat="server" ShowFooter="true" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" DataKeyNames="MovieID" DataSourceID="SqlDataSource1" ForeColor="Black" GridLines="Vertical" Width="948px" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
        <AlternatingRowStyle BackColor="#CCCCCC" />
        <Columns>
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
            <asp:TemplateField HeaderText="Movie ID" SortExpression="MovieID">
                <EditItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("MovieID") %>'></asp:Label>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:Button ID="Button2" runat="server" Text="Refresh Movies" OnClick="Button2_Click" />
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("MovieID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Title" SortExpression="MovieName">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("MovieName") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("MovieName") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Director" SortExpression="Director">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Director") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Director") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Year Released" SortExpression="YearReleased">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("YearReleased") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("YearReleased") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Rating" SortExpression="Rating">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("Rating") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("Rating") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <FooterStyle BackColor="#CCCCCC" />
        <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" />
        <SortedAscendingHeaderStyle BackColor="Gray" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#383838" />
    </asp:GridView>
    <asp:FormView ID="FormView1" runat="server" BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" CellPadding="2" DataKeyNames="MovieID" DataSourceID="SqlDataSource2" DefaultMode="Insert" OnLoad="Page_Load" Width="226px" OnPageIndexChanging="FormView1_PageIndexChanging" ForeColor="Black">
        <EditItemTemplate>
            MovieID:
            <asp:Label ID="MovieIDLabel1" runat="server" Text='<%# Eval("MovieID") %>' />
            <br />
            MovieName:
            <asp:TextBox ID="MovieNameTextBox" runat="server" Text='<%# Bind("MovieName") %>' />
            <br />
            Director:
            <asp:TextBox ID="DirectorTextBox" runat="server" Text='<%# Bind("Director") %>' />
            <br />
            YearReleased:
            <asp:TextBox ID="YearReleasedTextBox" runat="server" Text='<%# Bind("YearReleased") %>' />
            <br />
            Rating:
            <asp:TextBox ID="RatingTextBox" runat="server" Text='<%# Bind("Rating") %>' />
            <br />
            <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update" Text="Update" />
            &nbsp;<asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
        </EditItemTemplate>
        <EditRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <InsertItemTemplate>
            Movie ID:
            <asp:TextBox ID="MovieIDTextBox" runat="server" Text='<%# Bind("MovieID") %>' ValidateRequestMode="Disabled" />
            <br />
            Title:<br />
            <asp:TextBox ID="MovieNameTextBox" runat="server" Text='<%# Bind("MovieName") %>' />
            <br />
            Director:<br />
            <asp:TextBox ID="DirectorTextBox" runat="server" Text='<%# Bind("Director") %>' />
            <br />
            Year Released:<br />
            <asp:TextBox ID="YearReleasedTextBox" runat="server" Text='<%# Bind("YearReleased") %>' />
            <br />
            Rating:<asp:TextBox ID="RatingTextBox" runat="server" Text='<%# Bind("Rating") %>' />
            <br />
            <asp:Button ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" />
            &nbsp;<asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
        </InsertItemTemplate>
        <ItemTemplate>
            Movie ID:
            <asp:Label ID="MovieIDLabel" runat="server" Text='<%# Eval("MovieID") %>' />
            <br />
            Title:
            <asp:Label ID="MovieNameLabel" runat="server" Text='<%# Bind("MovieName") %>' />
            <br />
            Director:
            <asp:Label ID="DirectorLabel" runat="server" Text='<%# Bind("Director") %>' />
            <br />
            Year Released:
            <asp:Label ID="YearReleasedLabel" runat="server" Text='<%# Bind("YearReleased") %>' />
            <br />
            Rating:
            <asp:Label ID="RatingLabel" runat="server" Text='<%# Bind("Rating") %>' />
            <br />
            <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" />
            &nbsp;<asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete" />
            &nbsp;<asp:LinkButton ID="NewButton" runat="server" CausesValidation="False" CommandName="New" Text="New" />
        </ItemTemplate>
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
    </asp:FormView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ObjectDatabase %>" DeleteCommand="DELETE FROM [Movies] WHERE [MovieID] = @MovieID" InsertCommand="INSERT INTO [Movies] ([MovieID], [MovieName], [Director], [YearReleased], [Rating]) VALUES (@MovieID, @MovieName, @Director, @YearReleased, @Rating)" SelectCommand="SELECT * FROM [Movies]" UpdateCommand="UPDATE [Movies] SET [MovieName] = @MovieName, [Director] = @Director, [YearReleased] = @YearReleased, [Rating] = @Rating WHERE [MovieID] = @MovieID">
        <DeleteParameters>
            <asp:Parameter Name="MovieID" Type="String" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="MovieID" Type="String" />
            <asp:Parameter Name="MovieName" Type="String" />
            <asp:Parameter Name="Director" Type="String" />
            <asp:Parameter Name="YearReleased" Type="String" />
            <asp:Parameter Name="Rating" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="MovieName" Type="String" />
            <asp:Parameter Name="Director" Type="String" />
            <asp:Parameter Name="YearReleased" Type="String" />
            <asp:Parameter Name="Rating" Type="String" />
            <asp:Parameter Name="MovieID" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ObjectDatabase %>" DeleteCommand="DELETE FROM [Movies] WHERE [MovieID] = @MovieID" InsertCommand="INSERT INTO [Movies] ([MovieID], [MovieName], [Director], [YearReleased], [Rating]) VALUES (@MovieID, @MovieName, @Director, @YearReleased, @Rating)" SelectCommand="SELECT * FROM [Movies]" UpdateCommand="UPDATE [Movies] SET [MovieName] = @MovieName, [Director] = @Director, [YearReleased] = @YearReleased, [Rating] = @Rating WHERE [MovieID] = @MovieID">
        <DeleteParameters>
            <asp:Parameter Name="MovieID" Type="String" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="MovieID" Type="String" />
            <asp:Parameter Name="MovieName" Type="String" />
            <asp:Parameter Name="Director" Type="String" />
            <asp:Parameter Name="YearReleased" Type="String" />
            <asp:Parameter Name="Rating" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="MovieName" Type="String" />
            <asp:Parameter Name="Director" Type="String" />
            <asp:Parameter Name="YearReleased" Type="String" />
            <asp:Parameter Name="Rating" Type="String" />
            <asp:Parameter Name="MovieID" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <br />
</asp:Content>
