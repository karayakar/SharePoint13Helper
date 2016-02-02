<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LibraryItemFields.ascx.cs" Inherits="ShP.Helper.Testing.WebParts.LibraryItemFields.LibraryItemFields" %>


<script type="text/javascript" src="/_layouts/15/ShP.Helper.Testing/js/main.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        ParseListItemData();
    });
</script>
<div>
    <h1>List Fields Data</h1>
    <div id="ListItemFieldsData">
        <asp:Label ID="ListFieldsDataLabel" runat="server" Text=""></asp:Label>
    </div>
</div>
<br />
<br />
<br />
<br />
<div>
    <h1>List Item Data</h1>
    <br />
        <asp:Label ID="ListItemDataLabelInfo" runat="server" Text=""></asp:Label>
<br />
    <div id="ListItemData">
        <asp:Label ID="ListItemDataLabel" runat="server" Text=""></asp:Label>
    </div>
</div>
<br />
<br />
<br />
<br />
<div>
    <asp:Label ID="InfoLabel" runat="server" Text=""></asp:Label>
    <br />
    <asp:Label ID="ErrorLabel" runat="server" Text=""></asp:Label>
</div>

