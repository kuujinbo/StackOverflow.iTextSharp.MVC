<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="HtmlImageHandler.aspx.cs" Inherits="kuujinbo.StackOverflow.iTextSharp.MVC.WebForms.HtmlImageHandler" 
%>

<asp:content contentplaceholderid='webFormContent' runat='server'>
<h1>HTMLWorker Image Handler</h1>
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

<asp:Button runat='server'
    class='btn btn-primary margin-tb10'
    oncommand='ProcessHtml'
    text='Convert Html Control to PDF'
/>

</asp:content>