<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ImageParser.aspx.cs" Inherits="kuujinbo.StackOverflow.iTextSharp.MVC.WebForms.XmlWorkers.ImageParser" %>

<asp:content contentplaceholderid='webFormContent' runat='server'>
<h1>Converting HTML with Images to PDF using iTextSharp & XML Worker</h1>

<table id='ConvertControlToPdf' 
    class='margin-tb10'
    width='100%' border='1' align='center' 
    cellpadding='4' cellspacing='0'
    runat='server'
>
<tr><td>ROW 1: CELL 1</td>
<td><img src='https://en.gravatar.com/userimage/29975579/99436672eb556cd8a7aa1c49c0ea7b4d.png' /></td></tr>
<tr><td>ROW 2: CELL 1</td>
<td><img src='/images/alt-gravatar.png' /></td></tr>
<tr><td>ROW 3: CELL 1</td>
<td><img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></td></tr>
<tr><td>ROW 4: CELL 1</td>
<td><a href='/UploadMultipleImages'>Upload Multiple Images</a></td></tr>
</table>

<asp:Button runat='server'
    class='btn btn-primary margin-tb10'
    oncommand='ProcessHtml'
    text='Convert Html Control to PDF'
/>

<div class='center margin-tb10'>
    <p>
        <a href='/ImageHandler/'
           class='btn btn-primary btn-lg'>Go To MVC Example</a>
    </p>
</div>

</asp:content>