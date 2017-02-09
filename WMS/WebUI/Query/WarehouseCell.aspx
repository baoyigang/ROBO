<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WarehouseCell.aspx.cs" Inherits="WebUI_Query_WarehouseCell" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>货位信息显示</title>
     <link href="~/Css/Main.css" type="text/css" rel="stylesheet" /> 
        <link href="~/Css/op.css" type="text/css" rel="stylesheet" /> 
        <script type="text/javascript" src="../../JQuery/jquery-1.8.3.min.js"></script>
         <script type="text/javascript" src="../../JScript/DataProcess.js"></script>

        <script type="text/javascript" language="javascript">
            var oldcell;
            $(document).ready(function () {
                $(window).resize(function () {
                    resize();
                });
            });
            function resize() {
                var h = document.documentElement.clientHeight;
                $("#pnlCell").css("height", h - 20);
            }

            function selectedcell(cellobj) {
                var obj = document.getElementById(cellobj);
                if (oldcell != null) {
                    $("#" + oldcell.id).removeClass("cellfalg");

                }
                //选中货位
                $("#" + obj.id).addClass("cellfalg");
                oldcell = obj;
            }



            function ShowCellInfo(obj) {
                closeinfo();
                selectedcell(obj);
                var product = document.getElementById("productinfo");
                var row = new Object();
                row.CellCode = obj;
                var json = eval(Ajax("GetCellInfo", row));

                if (json) {
                    document.getElementById("AreaName").innerText = json[0].AreaName;
                    document.getElementById("ShelfName").innerText = json[0].ShelfName;
                    document.getElementById("CellColumn").innerText = json[0].CellColumn;
                    document.getElementById("CellRow").innerText = json[0].CellRow;
                    document.getElementById("CellCode").innerText = json[0].CellCode;

                    if (json[0].IsLock == "0")
                        document.getElementById("State").innerText = "正常";
                    else
                        document.getElementById("State").innerText = "锁定";
                    if (json[0].ErrorFlag == "1")
                        document.getElementById("State").innerText = "异常";

                    if (json[0].PalletCode != "") {

                        document.getElementById("tdProduct").innerHTML = "<b>产品信息--" + json[0].PalletCode + "</b>";
                        $.each(json, function (idx, obj) {
                            var table = document.getElementById("tbProduct");
                            var newRow = table.insertRow(); //创建新行
                            //var CellCatergory = newRow.insertCell(); //创建新单元格
                            var CellProductCode = newRow.insertCell(); //创建新单元格
                            var CellProductName = newRow.insertCell(); //创建新单元格
                            var CellBatchNo = newRow.insertCell(); //创建新单元格
                            var CellQty = newRow.insertCell(); //创建新单元格
                            var CellUnit = newRow.insertCell(); //创建新单元格
                            var CellInDate = newRow.insertCell(); //创建新单元格

                           // CellCatergory.innerText = obj.CategoryName;
                            CellProductCode.innerText = obj.ProductCode;
                            CellProductName.innerText = obj.ProductName;
                            CellBatchNo.innerText = obj.BatchNo;
                            CellQty.innerText = obj.Quantity;
                            CellUnit.innerText = obj.Unit;
                            CellInDate.innerText = obj.Indate;
                        });
                    }

                    showinfo(obj);
                }
                else {
                    closeinfo();
                }
            }


 


            function showinfo(cellobj) {
                var obj = document.getElementById(cellobj);
                var product = document.getElementById("productinfo");
                var objtop = obj.offsetTop;
                var objheight = obj.clientHeight;
                var objleft = obj.offsetLeft;
                // while (obj = obj.offsetParent) { objtop += obj.offsetTop; objleft += obj.offsetLeft; }

                product.style.top = parseFloat(objtop + objheight) + "px";
                if ((objleft + parseFloat(product.style.width)) > document.body.clientWidth) {
                    product.style.left = parseFloat(objleft) - parseFloat(product.style.width) + "px";
                }
                else
                    product.style.left = parseFloat(objleft) + "px";

                product.style.display = "block";
            }



            function closeinfo() {

                var tb = document.getElementById('tbProduct');
                var rowNum = tb.rows.length;
                for (i = rowNum - 1; i > 0; i--) {
                    tb.deleteRow(i);
                }

                var product = document.getElementById("productinfo");
                product.style.display = "none";
            }
    </script>
    <style type="text/css">
    .cellfalg
    {
         background-image:url(../../images/flag.png);
         background-repeat:no-repeat;   
    }
    .cellinfo
    {
   width:65px;
   font-size:12px;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel ID="pnlCell" runat="server"  Width="100%" Height="450px"  style="overflow:auto;" >
    </asp:Panel>
    <div id="productinfo"  style=" width:420px; height:230px; position:absolute; background-color:#dbe7fd; display:none; border:1px solid #000;">
        <div id="btclose" style=" width:100%; height:20px;">
          <span id="cellcode" style="float:left"><b>货架信息</b></span>
          <span onclick="closeinfo()"  style=" float:right; width:15px; height:20px;  cursor:pointer">X</span>
        </div>
        <div>
             <table class="maintable" style="width:100%;">
                    <tr>
                      <td align="right" class=" musttitle" style="width:12%;" >
                             &nbsp;库区:
                      </td>
                      <td id="AreaName">
                          
                      </td>
                       <td  align="right" class="musttitle"  style="width:12%;">
                             &nbsp;货架:
                      </td>
                      <td id="ShelfName"> 
                          
                      </td>
                       <td align="right"  class="musttitle"  style="width:12%;">
                             &nbsp;货位:
                      </td>
                      <td id="CellCode">
                          
                      </td>
                   </tr>
                     
                    <tr>
                      <td  align="right" class="musttitle"  style="width:12%;">
                             &nbsp;状态:
                      </td>
                      <td id="State">
                          
                      </td>
                        <td  align="right" class="musttitle" style="width:12%;">
                                &nbsp;列:
                        </td>
                        <td id="CellColumn">
                          
                        </td>
                        <td align="right" class="musttitle"  style="width:12%;">
                                &nbsp;层:
                        </td>
                        <td id="CellRow">
                          
                        </td>
                    </tr>
                    <tr>
                      <td id='tdProduct'  colspan="6">
                        <b>产品信息</b>
                      </td>
                   </tr>
               </table>
             <div style="overflow: auto; WIDTH: 100%; HEIGHT: 150px">
                <table  class="maintable" style="width:100%" id="tbProduct">
                    <tr>
                         
                        <td class="musttitle"  style="width:10%;">
                            产品编码
                        </td>
                        <td class="musttitle"  style="width:15%;">
                            品名
                        </td>
                        <td class="musttitle"  style="width:12%;">
                            型号
                        </td>
                        <td class="musttitle"  style="width:7%;">
                            数量
                        </td>
                        <td class="musttitle"  style="width:7%;">
                            单位
                        </td>
                        <td class="musttitle"  style="width:15%;">
                            入库日期
                        </td>
                    </tr>
                </table>
             </div>
        </div>
    </div>




    </form>
</body>
</html>
