<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SendEMail.ascx.cs" Inherits="ShP.Helper.Testing.WebParts.SendEMail.SendEMail" %>

<div>
    <h1>Send Mails</h1>
    <br />
    Format emails: "admin1@com;admin2@com;admin3@com"
    <input runat="server" id="senDTo" style="width: 500px;" />
    <br />
    Subject:
    <input runat="server" id="Subject" style="width: 500px;" />
    <br />
    Body
    <asp:TextBox ID="BodyTextBox" Rows="6" runat="server"></asp:TextBox>
    <br />
    <br />
    <asp:Button ID="SendMailButton1" runat="server" Text="SendMail 1" OnClick="SendMailButton1_Click" />
    <asp:Button ID="SendMailButton2" runat="server" Text="SendMail 2" OnClick="SendMailButton2_Click" />
    <br />
    <br />
    <asp:FileUpload ID="FileUpload2" runat="server" />
    <asp:Button ID="SendMailButton3" runat="server" Text="SendMail 3" OnClick="SendMailButton3_Click" />
    <br />
    <br />
    <asp:FileUpload ID="FileUpload" runat="server" />
    <asp:FileUpload ID="FileUpload1" runat="server" />
    <asp:Button ID="SendMailButton4" runat="server" Text="SendMail 4" OnClick="SendMailButton4_Click" />
    <br />
    <br />
    <asp:Label ID="LabelError" runat="server" Text=""></asp:Label>
</div>
