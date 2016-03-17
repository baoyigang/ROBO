<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StockPrint.aspx.cs" Inherits="WebUI_Query_StockPrint" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title> 
        <link href="~/Css/Main.css" type="text/css" rel="stylesheet" /> 
        <link href="~/Css/op.css" type="text/css" rel="stylesheet" /> 
   
        <script type="text/javascript" src='<%=ResolveClientUrl("~/JQuery/jquery-1.8.3.min.js") %>'></script>
        <script type="text/javascript" src='<%=ResolveClientUrl("~/JScript/Common.js") %>'></script>
       <script type="text/javascript">
           $(document).ready(function () {
               var src1 = "../../RptFiles/PDF/" +'<%= PdfName %>';
               $('#divPrint1').empty();
               $('#divPrint1').append("<embed width='100%' height='100%' src='" + src1 + "'></embed>");

               $(window).resize(function () {
                   resize();
               });
              
           });
           function resize() {
               var h = document.documentElement.clientHeight - 10;
               $("#rptview").css("height", h);
           }
           function ViewBack() {
               location.href = "../" + '<%=PagePath %>' + "/" + '<%=FormID %>' + "View.aspx?SubModuleCode=" + '<%=SubModuleCode %>' + "&FormID=" + '<%=FormID %>' + "&SqlCmd=" + '<%=SqlCmd %>' + "&ID=" + '<%=BillID %>';
               return false;
           }
        </script>
   
    </head>
<body  style="overflow:hidden;">
  <form id="form1" runat="server"> 
       <table  style="width: 100%; height: 25px;" class="OperationBar">
            <tr>
                <td align="left" style="width:40%">
                    打印
                </td>
                <td align="right">
                    <asp:Button ID="btnBack" runat="server" Text="返回" OnClientClick="return ViewBack();" CssClass="ButtonBack" />
                    <asp:Button ID="btnExit" runat="server" Text="离开" OnClientClick="return Exit();" CssClass="ButtonExit"  />
                        
                </td>
            </tr>
        </table>
       <table align="center" id="PDFShow"  style="width:100%; height:500px; padding:0px 0px position:absolute; background-color:#dbe7fd; border:1px solid #000;">
            <tr>
                <td  align="center">
                <div  id="divPrint1"  style=" width:98%; height:95%" >
                
                </div>

                </td>
            </tr>
            
        </table>
         
       <input id="HdnProduct" type="hidden" runat="server" />
      
       <input id="HdnWH" type="hidden" runat="server" value="" />
         
   </form>
</body>
</html>