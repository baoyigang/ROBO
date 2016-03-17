<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReceiptQuery.aspx.cs" Inherits="WebUI_InStock_ReceiptQuery"  culture="auto" uiculture="auto" MaintainScrollPositionOnPostback="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="~/Css/Main.css" type="text/css" rel="stylesheet" /> 
    <link href="~/Css/op.css" type="text/css" rel="stylesheet" /> 
    <script type="text/javascript" src="../../JQuery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src= "../../JScript/Common.js"></script>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $(window).resize(function () {
                resize();
            });
        });
        function resize() {
            var h = document.documentElement.clientHeight - 270;
            $("#divSub").css("height", h);
        }
        function AddValues(oChk, strReturn) {
            var chkSelect = document.getElementById(oChk);
            if (chkSelect.checked) {
                (chkSelect.parentElement).parentElement.className = "bottomtable";
                if (SelectPage.HdnSelectedValues.value == '')
                    SelectPage.HdnSelectedValues.value += strReturn;
                else
                    SelectPage.HdnSelectedValues.value += ',' + strReturn;
            }
            else {
                (chkSelect.parentElement).parentElement.className = "";
                SelectPage.HdnSelectedValues.value = SelectPage.HdnSelectedValues.value.replace(',' + strReturn, '');
                SelectPage.HdnSelectedValues.value = SelectPage.HdnSelectedValues.value.replace(strReturn + ',', '');
                SelectPage.HdnSelectedValues.value = SelectPage.HdnSelectedValues.value.replace(strReturn, '');
            }
        }
        function SelectAll(tempControl) {
            var theBox = tempControl;
            xState = theBox.checked;

            elem = theBox.form.elements;
            for (i = 0; i < elem.length; i++)
            //if(elem[i].type=="checkbox" && elem[i].id!=theBox.id && elem[i].id.substring(elem[i].id.length-2,elem[i].id.length)==theBox.id.substring(theBox.id.length-2,theBox.id.length))
                if (elem[i].id.indexOf("chkSelect") >= 0) {
                    if (elem[i].checked != xState) {
                        elem[i].click();
                    }
                }
        }
        function SelectedCell() {
            elem = SelectPage.elements;
            for (i = 0; i < elem.length; i++)
                if (elem[i].type == 'checkbox' && elem[i].checked == true) {
                    (elem[i].parentElement).parentElement.className = "bottomtable";
                }
        }
        function AllClear(bln) {
            elem = SelectPage.elements;
            for (i = 0; i < elem.length; i++)
                if (elem[i].type == "checkbox" && elem[i].id.substring(elem[i].id.length - 6, elem[i].id.length) == "Select") {
                    if (bln == 0)
                        elem[i].checked = true;
                    else
                        elem[i].checked = false;
                    elem[i].click();
                }
        }
        function Close() {
            SelectPage.HdnSelectedValues.value = "";
            window.returnValue = document.getElementById('HdnSelectedValues').value;
            window.close();
        }


        function Select() {
            window.returnValue = "[" + document.getElementById('HdnSelectedValues').value + "]";
            window.close();
        }
    </script>
</head>
<body>
    <form id="SelectPage" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />  
        <asp:UpdateProgress ID="updateprogress" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
            <ProgressTemplate>            
                <div id="progressBackgroundFilter" style="display:none"></div>
                <div id="processMessage"> Loading...<br /><br />
                        <img alt="Loading" src="../../images/loading.gif" />
                </div>      
            </ProgressTemplate> 
        </asp:UpdateProgress>  
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">                
            <ContentTemplate>
                <div>
                    <table  style="width: 100%; height: 20px;">
                    <tr>
						    <td class="smalltitle" align="center" width="7%" >
                                <asp:Literal ID="Literal1" Text="查询栏位" runat="server"  ></asp:Literal>
                             </td>
						    <td  width="15%" height="20">&nbsp;<asp:dropdownlist id="ddlField" runat="server" Width="85%" >
                                    <asp:ListItem Selected="True" Value="BillTypeName">入库类型</asp:ListItem>
                                    <asp:ListItem Value="FactoryName">工厂</asp:ListItem>
                                    <asp:ListItem Value="BillID">入库单号</asp:ListItem>
                                    <asp:ListItem Value="Memo">备注</asp:ListItem>
                                 </asp:dropdownlist>
                            </td>
						    <td class="smalltitle" align="center" width="7%">
                                <asp:Literal ID="Literal2" Text="查询内容" runat="server"></asp:Literal>
                            </td>
						    <td  width="26%" height="20" valign="middle">&nbsp;<asp:textbox id="txtSearch" 
                                    tabIndex="1" runat="server" Width="90%" CssClass="TextBox"  
                                    heigth="16px" ></asp:textbox>
                               
                          </td>
                          <td width="15%" align="left">
                           &nbsp;<asp:button id="btnSearch" tabIndex="2" runat="server" Width="58px" 
                                    CssClass="ButtonQuery" Text="查询" OnClientClick="return Search()" 
                                    onclick="btnSearch_Click"></asp:button>&nbsp;&nbsp;
                              <asp:Button ID="btnRefresh" runat="server" CssClass="ButtonRefresh" 
                                  onclick="btnSearch_Click" OnClientClick="return Refresh()" tabIndex="2" 
                                  Text="刷新" Width="58px" />
                          
                          </td>
                          <td align="right"  style="width:30%" valign="middle">
                             <%-- <asp:Button ID="btnPrint" runat="server" Text="导出" CssClass="ButtonPrint" OnClientClick="return print();"/>--%>
                           <asp:Button ID="btnSelect" runat="server" Text="确定" Width="60px" CssClass="ButtonOk" OnClientClick="return Select();" />
                           <asp:Button ID="btnClose" runat="server" Text="关闭" Width="60px" CssClass="ButtonExit" OnClientClick="return Close();" /></td>
                    </tr>
                        <caption>
                            &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;
                            </td>
                            </tr>
                        </caption>
                </table>
                    
                </div>
                <div id="divMain" style="overflow: auto; WIDTH: 100%; HEIGHT: 150px" onscroll="javascript:RecordPostion(this);">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" SkinID="GridViewSkin" Width="100%" OnRowDataBound="GridView1_RowDataBound">
                        <Columns>
                           <asp:BoundField DataField="BillID" HeaderText="收货单号" SortExpression="BillID">
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField> 
                            <asp:TemplateField HeaderText="入库日期" SortExpression="BillDate">
                                <ItemTemplate>
                                    <%# ToYMD(DataBinder.Eval(Container.DataItem, "BillDate"))%>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Wrap="False" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="StateDesc" HeaderText="单据状态" SortExpression="StateDesc">
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="BillTypeName" HeaderText="入库类型" SortExpression="BillTypeName">
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="SourceBillNo" HeaderText="来源单号" SortExpression="SourceBillNo">
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="FactoryName" HeaderText="工厂" SortExpression="FactoryName">
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="Memo" HeaderText="备注" 
                                SortExpression="Memo" >
                                <ItemStyle HorizontalAlign="Left" Width="15%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                             <asp:BoundField DataField="Checker" HeaderText="审核人员" 
                                SortExpression="Checker"  >
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="审核日期" SortExpression="CheckDate">
                                <ItemTemplate>
                                    <%# ToYMD(DataBinder.Eval(Container.DataItem, "CheckDate"))%>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Wrap="False" />
                            </asp:TemplateField>

                             <asp:BoundField DataField="Creator" HeaderText="建单人员" 
                                SortExpression="Creator"  >
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="建单日期" SortExpression="CreateDate">
                                <ItemTemplate>
                                    <%# ToYMD(DataBinder.Eval(Container.DataItem, "CreateDate"))%>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Wrap="False" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="Updater" HeaderText="修改人员" 
                                SortExpression="Updater"  >
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="修改日期" SortExpression="UpdateDate">
                                <ItemTemplate>
                                    <%# ToYMD(DataBinder.Eval(Container.DataItem, "UpdateDate"))%>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Wrap="False" />
                            </asp:TemplateField>
               
                        </Columns>
                        <PagerSettings Visible="False" />
                    </asp:GridView>
                </div>
                <div>
                    &nbsp;&nbsp;<asp:LinkButton ID="btnFirst" runat="server" OnClick="btnFirst_Click" Text="首页"></asp:LinkButton> 
                    &nbsp;<asp:LinkButton ID="btnPre" runat="server" OnClick="btnPre_Click" Text="上一页"></asp:LinkButton> 
                    &nbsp;<asp:LinkButton ID="btnNext" runat="server" OnClick="btnNext_Click" Text="下一页"></asp:LinkButton> 
                    &nbsp;<asp:LinkButton ID="btnLast" runat="server" OnClick="btnLast_Click" Text="尾页"></asp:LinkButton> 
                    &nbsp;<asp:textbox id="txtPageNo" onkeypress="return regInput(this,/^\d+$/,String.fromCharCode(event.keyCode))"
					        onpaste="return regInput(this,/^\d+$/,window.clipboardData.getData('Text'))" ondrop="return regInput(this,/^\d+$/,event.dataTransfer.getData('Text'))"
					        runat="server" Width="56px" CssClass="TextBox" ></asp:textbox>
                    &nbsp;<asp:linkbutton id="btnToPage" runat="server" onclick="btnToPage_Click" Text="跳转"></asp:linkbutton>
                    &nbsp;<asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="True"  Visible="false"></asp:DropDownList>
                    &nbsp;<asp:Label ID="lblCurrentPage" runat="server" ></asp:Label>
                </div>
                 <table class="maintable" cellpadding="0" cellspacing="0" style="width: 100%; height:24px">
                    <tr>
                        <td valign="middle" align="left" height="22px">
                            <b>入库单明细</b> 
                        </td>
                    </tr>
                </table>
                 <div id="divSub" style="overflow: auto; WIDTH: 100%; HEIGHT: 155px">
                    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
                         AllowPaging="True" PageSize="10" SkinID="GridViewSkin" Width="100%" 
                         onrowdatabound="GridView2_RowDataBound">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>                        
				                    <input type="checkbox" runat="server" id="chkHeadSelect" onclick="SelectAll(this);"/>                    
                                </HeaderTemplate>
			                    <ItemTemplate>
				                    <input type="checkbox" runat="server" id="chkSelect"/>
			                    </ItemTemplate>
                                <HeaderStyle Width="20px" HorizontalAlign="Center" />
                                <ItemStyle Width="20px" HorizontalAlign="Center" />
		                    </asp:TemplateField>
                            <asp:BoundField DataField="RowID" HeaderText="序号" SortExpression="RowID">
                                <ItemStyle HorizontalAlign="Left" Width="7%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ProductCode" HeaderText="产品编号" SortExpression="ProductCode">
                                <ItemStyle HorizontalAlign="Left" Width="15%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ProductName" HeaderText="品名" SortExpression="ProductName">
                                <ItemStyle HorizontalAlign="Left" Width="20%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                           <asp:BoundField DataField="ModelNo" HeaderText="型号" SortExpression="ModelNo">
                                <ItemStyle HorizontalAlign="Left" Width="20%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Quantity" HeaderText="数量" SortExpression="Quantity">
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Weight" HeaderText="重量" SortExpression="Weight">
                                <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Memo" HeaderText="备注" 
                                SortExpression="Memo" >
                                <ItemStyle HorizontalAlign="Left" Width="38%" Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                        </Columns>
                        <PagerSettings Visible="False" />
                    </asp:GridView>
                </div>
                <div style="height:23px;">
                    &nbsp;&nbsp;<asp:LinkButton ID="btnFirstSub1" runat="server" 
                        OnClick="btnFirstSub1_Click" Text="首页"></asp:LinkButton> 
                    &nbsp;<asp:LinkButton ID="btnPreSub1" runat="server" OnClick="btnPreSub1_Click" 
                        Text="上一页"></asp:LinkButton> 
                    &nbsp;<asp:LinkButton ID="btnNextSub1" runat="server" 
                        OnClick="btnNextSub1_Click" Text="下一页"></asp:LinkButton> 
                    &nbsp;<asp:LinkButton ID="btnLastSub1" runat="server" 
                        OnClick="btnLastSub1_Click" Text="尾页"></asp:LinkButton> 
                    &nbsp;<asp:textbox id="txtPageNoSub1" onkeypress="return regInput(this,/^\d+$/,String.fromCharCode(event.keyCode))"
					        onpaste="return regInput(this,/^\d+$/,window.clipboardData.getData('Text'))" ondrop="return regInput(this,/^\d+$/,event.dataTransfer.getData('Text'))"
					        runat="server" Width="56px" CssClass="TextBox" ></asp:textbox>
                    &nbsp;<asp:linkbutton id="btnToPageSub1" runat="server" 
                        onclick="btnToPageSub1_Click" Text="跳转"></asp:linkbutton>
                    &nbsp;<asp:DropDownList ID="ddlPageSizeSub" runat="server" AutoPostBack="True" Visible="false"></asp:DropDownList>
                    &nbsp;<asp:Label ID="lblCurrentPageSub1" runat="server" ></asp:Label>
                </div>
                <div>
                    <asp:Button ID="btnReload" runat="server" Text="" OnClick="btnReload_Click"  CssClass="HiddenControl" />
                    <input id="HdnSelectedValues" type="hidden" name="HdnSelectedValues" runat="server" />
                    <asp:HiddenField ID="hdnRowIndex" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnRowValue" runat="server"  />
                    <input type="hidden" id="dvscrollX" runat="server" />
                    <input type="hidden" id="dvscrollY" runat="server" /> 
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
