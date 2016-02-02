<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserInfo.ascx.cs" Inherits="ShP.Helper.Testing.WebParts.UserInfo.UserInfo" %>

<div style="width: 960px; margin: 0 auto">
    <h1>UserInfo</h1>
    <br />
    <h2>About you</h2>
    <br />
    <div style="text-align: left;">
        <asp:Label ID="LabelCurrentUser" runat="server" Text=""></asp:Label>
    </div>
    <br />
    <div style="text-align: right">
        UserID:
        <input runat="server" id="UserID" />
        UserLogin:
        <input runat="server" style="width: 150px" id="UserLogin" />
        <asp:Button ID="GetUserInfoButton" runat="server" Text="GetUserInfo" OnClick="GetUserInfoButton_Click" />
    </div>

    <h2>About User</h2>
    <br />
    <div style="text-align: left;">
        <asp:Label ID="LabelUser" runat="server" Text=""></asp:Label>
    </div>

    <br />
    <div style="text-align: left;">
        <asp:Label ID="LabelError" runat="server" Text=""></asp:Label>
    </div>
</div>
