<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GridView.aspx.cs" Inherits="WebForms.Pages.GridView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" DataKeyNames="MovieID" DataSourceID="SqlDataSource1" ForeColor="Black" GridLines="Vertical" Width="564px">
        <AlternatingRowStyle BackColor="#CCCCCC" />
        <Columns>
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
            <asp:TemplateField HeaderText="Movie ID" SortExpression="MovieID">
                <EditItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("MovieID") %>'></asp:Label>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:Button ID="Button1" runat="server" Text="Insert" />
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("MovieID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Title" SortExpression="MovieName">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("MovieName") %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="NewTitle" runat="server"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("MovieName") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Director" SortExpression="Director">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Director") %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="NewDirector" runat="server"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Director") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Year Released" SortExpression="YearReleased">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("YearReleased") %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="NewYear" runat="server"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("YearReleased") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Rating" SortExpression="Rating">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("Rating") %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="NewRating" runat="server"></asp:TextBox>
                </FooterTemplate>
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
    <asp:FormView ID="FormView1" runat="server" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" DataKeyNames="MovieID" DataSourceID="SqlDataSource2" DefaultMode="Insert" ForeColor="Black" GridLines="Vertical" OnItemInserted="FormView1_ItemInserted" OnLoad="FormView1_Load" Width="205px">
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
        <EditRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <FooterStyle BackColor="#CCCC99" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <InsertItemTemplate>
            MovieID:
            <asp:TextBox ID="MovieIDTextBox" runat="server" Text='<%# Bind("MovieID") %>' />
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
            <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" />
            &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
        </InsertItemTemplate>
        <ItemTemplate>
            MovieID:
            <asp:Label ID="MovieIDLabel" runat="server" Text='<%# Eval("MovieID") %>' />
            <br />
            MovieName:
            <asp:Label ID="MovieNameLabel" runat="server" Text='<%# Bind("MovieName") %>' />
            <br />
            Director:
            <asp:Label ID="DirectorLabel" runat="server" Text='<%# Bind("Director") %>' />
            <br />
            YearReleased:
            <asp:Label ID="YearReleasedLabel" runat="server" Text='<%# Bind("YearReleased") %>' />
            <br />
            Rating:
            <asp:Label ID="RatingLabel" runat="server" Text='<%# Bind("Rating") %>' />
            <br />
            <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" />
            &nbsp;<asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete" />
            &nbsp;<asp:LinkButton ID="NewButton" runat="server" CausesValidation="False" CommandName="New" Text="New" />
        </ItemTemplate>
        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
        <RowStyle BackColor="#F7F7DE" />
    </asp:FormView>
    <br />
</asp:Content>
