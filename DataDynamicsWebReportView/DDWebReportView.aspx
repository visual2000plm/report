<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DDWebReportView.aspx.cs" Inherits="DynamicWebReportView.DDWebReportView" %>
<%@ Register assembly="DataDynamics.Reports.Web" namespace="DataDynamics.Reports.Web" tagprefix="dd" %>
<?xml version="1.0" encoding="utf-8" ?>
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head id="Head1" runat="server">
    <title> Visual 2000 Report View</title>
 
</head>
<body>
    <form id="form1" runat="server">
    <table id="Table1" runat="server" border="0" cellpadding="0" cellspacing="2"
        height="100%" width="100%">
    
       
        <tr>
            
                        <td  valign="top" style="height: 100%; width: 100%;">
                            <dd:WebReportViewer ID="WebReportViewer2" runat="server"   Height="900"   />
                           
                        </td>
           
        </tr>
        <%--<tr>
            <td align="left" valign="top" style="height: 100%; width: 100%;">
                <asp:Button ID="Button1" runat="server"  OnClick="GeneratePDF" Text="Button" />
            </td>
        </tr>--%>
    </table>
    </form>
</body>
</html>




