<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InStockPallet.aspx.cs" Inherits="WebUI_InStock_InStockPallet" %>
<%@ Register src="../../UserControl/Calendar.ascx" tagname="Calendar" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
        <link href="~/Css/Main.css" type="text/css" rel="stylesheet" /> 
        <link href="~/Css/op.css" type="text/css" rel="stylesheet" /> 
        <script type="text/javascript" src="../../JQuery/jquery-1.8.3.min.js"></script>
        <script type="text/javascript" src= "../../JScript/Common.js"></script>
        <script type="text/javascript" src="../../JScript/DataProcess.js"></script>
        <script type="text/javascript">
            $(document).ready(function () {
                $(window).resize(function () {
                    resize();
                });
                BindEvent();
            });
            function resize() {
                var h = document.documentElement.clientHeight - 100;
                $("#Sub-container").css("height", h);
            }
            function SelectProduct() {
                if (trim($("#txtSearch").val()) == "") {
                    alert("请先输入托盘编号!");
                    $("#txtSearch").focus();
                    return false;
                }

                var tableName = 'CMD_Product';
                var where = " IsFixed='0' ";

                return GetProductMulSelectValue(tableName, 'hdnMulSelect', where);
            }

            function SelectReceiptProduct() {
                if (trim($("#txtSearch").val()) == "") {
                    alert("请先输入托盘编号!");
                    $("#txtSearch").focus();
                    return false;
                }
                $("#hdnMulSelect").val("");
                var Where = '';
                var returnvalue = window.showModalDialog('ReceiptQuery.aspx?', window, 'DialogHeight:700px;DialogWidth:900px;help:no;scroll:no;location:no;Resizable:yes;');
                //alert(returnvalue);
                if (returnvalue != "undefined" && returnvalue != null && returnvalue != "[]") {
                    if (returnvalue != "") {
                        var t1 = returnvalue.replace(/&nbsp;/g, "");
                        var t2 = t1.replace(/&quot;/g, '"');
                        var t3 = t2.replace(/&lt;/g, '<');
                        var t4 = t3.replace(/&gt;/g, '>');
                        var t5 = t4.replace("@^&", "'");
                        var t6 = t5.replace(/&amp;/g, '&');
                        $("#hdnMulSelect").val(t6.split("@,#"));
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;

            }


            function BindEvent() {
                $("[ID$='ProductCode']").bind("change", function () {
                    var txtID = this.id;
                    var where = " ProductCode='" + $('#' + txtID).val() + "' and IsFixed='0'";

                    getWhereBaseData('CMD_Product', txtID + "," + txtID.replace("ProductCode", "ProductName"), 'ProductCode,ProductName', where);
                });
                $("[ID$='ProductCode']").bind("dblclick", function () {
                    var txtID = this.id;
                    $('#' + txtID.replace("ProductCode", "btnProduct"))[0].click();

                });
            }
            function Save() {
                if (trim($("#txtSearch").val()) == "") {
                    alert("托盘编号不能为空!");
                    $("#txtSearch").focus();
                    return false;
                }
                if (!ChkDelMustValue("dgViewSub1", "ProductCode", "产品编号"))
                    return false;
                if (!ChkDelMustNumericValue("dgViewSub1", "Quantity", "数量"))
                    return false;
                return true;
            }
           
        </script>
    
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager2" runat="server" />  
        <asp:UpdateProgress ID="updateprogress1" runat="server" AssociatedUpdatePanelID="updatePanel1">
            <ProgressTemplate>            
                <div id="progressBackgroundFilter" style="display:none"></div>
                <div id="processMessage"> Loading...<br /><br />
                        <img alt="Loading" src="../../images/loading.gif" />
                </div>            
                </ProgressTemplate>
        </asp:UpdateProgress>  
        <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">                
            <ContentTemplate>
                <table id="Table1" class="maintable"  width="100%" align="center" cellspacing="0" cellpadding="0" bordercolor="#ffffff" border="1"
				         runat="server">			
                    <tr>
                        <td align="center" class="smalltitle" style="width:8%;"  >
                              托盘编号  
                        </td>
                        <td  width="20%">
                             &nbsp;<asp:TextBox ID="txtSearch" runat="server"  CssClass="TextBox" Width="72%" 
                                MaxLength="20" ></asp:TextBox><asp:button id="btnSearch" tabIndex="2" runat="server" Width="58px" 
                                    CssClass="ButtonQuery" Text="查询" OnClientClick="return Search()" 
                                    onclick="btnSearch_Click"></asp:button>
                        </td>
                        <td align="center" class="smalltitle" style="width:8%;"  >
                             入库货位   
                        </td>
                        <td width="17%">
                            &nbsp;<asp:TextBox ID="txtCellCode" runat="server"  CssClass="TextRead" Width="90%" 
                                MaxLength="20" ></asp:TextBox>  
                                
                        </td>
                        <td align="right"     style="width:40%;">
                              <asp:Button ID="btnSave" runat="server" Text="保存" OnClientClick="return Save()" 
                                CssClass="ButtonSave" onclick="btnSave_Click" Height="16px" />
                            <asp:Button ID="btnExit" runat="server" Text="离开" OnClientClick="return Exit();" 
                                CssClass="ButtonExit" />
                        </td>
                        
                        
                    </tr>
                </table>
                <table style="width:100%; height:25px">
                    <tr>
                        <td class="table_titlebgcolor" height="25px">
                            <asp:Button  id="btnAddDetail" CssClass=" ButtonCreate" runat="server" 
                                Text="新增明细" onclick="btnAddDetail_Click" OnClientClick="return SelectProduct();"  Width="75px" Height="16px"  />  
                                &nbsp;&nbsp;
                                <asp:Button  id="btnReceiptDetail" CssClass=" ButtonCreate" runat="server" 
                                Text="收货单导入" onclick="btnReceiptDetail_Click" 
                                OnClientClick="return SelectReceiptProduct();"  Width="100px" Height="16px"  />
                                 &nbsp;&nbsp;
                                <asp:Button  id="btnDelDetail" CssClass=" ButtonDel" 
                                runat="server" Text="删除明细" onclick="btnDelDetail_Click" 
                                Width="75px" Height="16px" />
                                
                               
                    </td>
                    </tr>
                </table> 
                <div id="Sub-container" style="overflow: auto; width: 100%; height: 360px" >
                    <asp:GridView ID="dgViewSub1" runat="server" AutoGenerateColumns="False" SkinID="GridViewSkin"
                        AllowPaging="True" Width="100%" PageSize="10" onrowdatabound="dgViewSub1_RowDataBound" >
                        <Columns>
                            <asp:TemplateField  >
                                <HeaderTemplate>
                                <input type="checkbox" onclick="selectAll('dgViewSub1',this.checked);" />                    
                                </HeaderTemplate>
                                <ItemTemplate>
                                <asp:CheckBox id="cbSelect" runat="server" ></asp:CheckBox>                   
                                </ItemTemplate>
                                <HeaderStyle Width="3%"></HeaderStyle>
                                <ItemStyle Width="3%" HorizontalAlign="Center"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="序号">
                                <ItemTemplate>
                                    <asp:Label ID="RowID" runat="server" Text=""></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle Width="4%"  />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="产品编号">
                                    <ItemTemplate>
                                    <asp:TextBox ID="ProductCode" runat="server"  Width="80%" CssClass="TextBox"></asp:TextBox><asp:Button
                                        ID="btnProduct"  CssClass="ButtonOption" Width="20px" runat="server"  Text="..." OnClientClick="return SelectProduct();" OnClick="btnProduct_Click" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle Width="15%"  />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="品名">
                                <ItemTemplate>
                                    <asp:TextBox ID="ProductName" runat="server" Width="98%"  CssClass="TextRead" ></asp:TextBox> 
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle  Width="20%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="型号">
                                <ItemTemplate>
                                    <asp:TextBox ID="ModelNo" runat="server" Width="98%"  CssClass="TextRead" ></asp:TextBox> 
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle  Width="15%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="数量">
                                <ItemTemplate>
                                    <asp:TextBox ID="Quantity" runat="server" Width="100%" CssClass="TextBox" style="text-align:right;" 
                                    onkeypress="return regInput(this,/^\d+$/,String.fromCharCode(event.keyCode))" 	onpaste="return regInput(this,/^\d+$/,window.clipboardData.getData('Text'))" 
                                    ondrop="return regInput(this,/^\d+$/,event.dataTransfer.getData('Text'))" onfocus="TextFocus(this);"></asp:TextBox> 
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle Width="10%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="单位">
                                <ItemTemplate>
                                    <asp:TextBox ID="Unit" runat="server" Width="98%"  CssClass="TextRead" ></asp:TextBox> 
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle  Width="8%" />
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="重量">
                                <ItemTemplate>
                                    <asp:TextBox ID="Weight" runat="server" Width="100%" CssClass="TextBox" style="text-align:right;" 
                                    onkeypress="return regInput(this,/^\d*\.?\d{0,2}$/,String.fromCharCode(event.keyCode))" 	onpaste="return regInput(this,/^\d*\.?\d{0,2}$/,window.clipboardData.getData('Text'))" 
                                    ondrop="return regInput(this,/^\d*\.?\d{0,2}$/,event.dataTransfer.getData('Text'))" onfocus="TextFocus(this);"></asp:TextBox>  
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle  Width="10%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="入库时间">
                                <ItemTemplate>
                                    <asp:TextBox ID="Indate" runat="server" Width="98%"  CssClass="TextRead" ></asp:TextBox> 
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle  Width="15%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="备注">
                                <ItemTemplate>
                                    <asp:TextBox ID="SubMemo" runat="server" Width="98%"  CssClass="TextBox" ></asp:TextBox> 
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle  Width="38%" />
                            </asp:TemplateField>
                        </Columns>
                        <PagerSettings Visible="false" />
                    </asp:GridView>
                </div>
                <table  class=" maintable" style="width:100%; height:25px" > 
                    <tr>
                        <td align="center"  style="width:8%;" class="smalltitle">
                            数量合计
                        </td>
                        <td style="width:17%">
                            &nbsp;<asp:TextBox ID="txtTotalQty" runat="server" CssClass="TextRead" ReadOnly="True" Width="90%" style="text-align:right"></asp:TextBox>
                        </td>
                          
                        <td align="right">
                            
                            <asp:LinkButton ID="btnFirstSub1" runat="server"  
                                        Text="首页" onclick="btnFirstSub1_Click"></asp:LinkButton> 
                                            &nbsp;<asp:LinkButton ID="btnPreSub1" runat="server"  
                                    Text="上一页" onclick="btnPreSub1_Click"></asp:LinkButton> 
                                    &nbsp;<asp:LinkButton ID="btnNextSub1" runat="server" 
                                    Text="下一页" onclick="btnNextSub1_Click"></asp:LinkButton> 
                            &nbsp;<asp:LinkButton ID="btnLastSub1" runat="server"  
                                        Text="尾页" onclick="btnLastSub1_Click"></asp:LinkButton> 
                    &nbsp;<asp:textbox id="txtPageNoSub1" onkeypress="return regInput(this,/^\d+$/,String.fromCharCode(event.keyCode))"
					                onpaste="return regInput(this,/^\d+$/,window.clipboardData.getData('Text'))" ondrop="return regInput(this,/^\d+$/,event.dataTransfer.getData('Text'))"
					                runat="server" Width="56px" CssClass="TextBox" >
                                    </asp:textbox>&nbsp;<asp:linkbutton id="btnToPageSub1" runat="server" Text="跳转" onclick="btnToPageSub1_Click"></asp:linkbutton>
                                &nbsp;
                                <asp:Label ID="lblCurrentPageSub1" runat="server" ></asp:Label>&nbsp;&nbsp; 
                            </td>
                        <td width="1%">
                        </td>
                    </tr>
                </table>
                 

            <input type="hidden" runat="server" id="hdnMulSelect" /> 
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
