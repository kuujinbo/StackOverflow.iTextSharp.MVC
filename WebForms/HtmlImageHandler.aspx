<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="HtmlImageHandler.aspx.cs" Inherits="kuujinbo.StackOverflow.iTextSharp.MVC.WebForms.HtmlImageHandler" 
%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

<%--
    HtmlTable used for brevity - HTML conversion for a GridView is
    **EXACTLY** the same
    https://msdn.microsoft.com/en-us/library/system.web.ui.htmlcontrols.htmltable.aspx
--%>
<table id='ConvertControlToPdf' 
  width='100%' border='1' align='center' 
  cellpadding='4' cellspacing='0'
  runat='server'
>
<tr><td>ROW 1: CELL 1</td>
<td><img src='/images/alt-gravatar.png' /></td></tr>
<tr><td>ROW 2: CELL 1</td>
<td><img src='https://en.gravatar.com/userimage/29975579/84a0f3316cb83434319696c6f33bf963.png' /></td></tr>
<tr><td>ROW 3: CELL 1</td>
<td><img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></td></tr>
</table>
</div>
<div class='tbm8'>
<asp:Button runat='server'
  oncommand='ProcessHtml'
  text='Convert Html Control to PDF'
/>


</div> 
</form>
</body>
</html>
