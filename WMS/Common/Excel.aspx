<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Excel.aspx.cs" Inherits="Common_Excel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Css/op.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../JQuery/jquery-1.8.3.min.js"></script>
    <script src="../JScript/ajaxfileupload.js" type="text/javascript"></script>
  <script type="text/javascript">
      function Close() {
          window.returnValue = "1";
          window.close();
      }
      function Upload() {
          $.ajaxFileUpload({
              url: '../ExcelHandler.ashx',
              secureuri: false, //异步
              fileElementId: 'uploadfile', //上传控件ID
              dataType: 'json', 
              success: function (data, status) {
                  if (data.status == 1) {
                      alert(data.msg);
                      Close();
                  } else {
                      alert(data.msg);
                  }
              }, error: function (data, status, e) {
                  alert(data);
              }
          });
   }
  </script>
</head>
<body>   
        <form id="form1" enctype="multipart/form-data">
            <fieldset style="width: 320px;margin:50px auto">                   
                    <legend class="smalltitle">请选择Excel文件</legend>
                     <br />
                    <input type="file" id="uploadfile" name="uploadfile"  style="width: 250px;height:25px;background:white"/>&nbsp;
                    <input type="button"  value="确定" onclick="Upload()" class="ButtonOk"/>&nbsp;&nbsp;&nbsp;  
            </fieldset>
    </form>
</body>
</html>
