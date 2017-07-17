<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Data;
using System.Data.OleDb;
using Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

public class Handler : IHttpHandler 
{
    
    public void ProcessRequest(HttpContext context)
    {
        JsonResult jr = new JsonResult();
        try
        {
            context.Response.ContentType = "text/html";           
            BLL.BLLBase bll=new BLL.BLLBase();
            HttpFileCollection files = HttpContext.Current.Request.Files;
            HttpPostedFile postedFile = files[0];
            if (postedFile.FileName=="")
            {
                 jr.status = 2;
                 jr.msg = "请选择Excel文件！";
                 Response(context, jr);  
            }
            bool flag = false;
            string IsXls = System.IO.Path.GetExtension(postedFile.FileName).ToString().ToLower();
            string[] allowExtensions = { ".xls", ".xlsx" };
            for (int i = 0; i < allowExtensions.Length; i++)
            {
                if (IsXls==allowExtensions[i])
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                string Path = System.Web.HttpContext.Current.Request.MapPath("~/upfiles/") + postedFile.FileName;
                postedFile.SaveAs(Path);
                DataParameter[] paras;
                //DataTable dt = GetExcelDatatable(Path, "Product"); 
                DataTable dt = ExcelToDataTable(Path, true);       
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    paras = new DataParameter[] { 
                                            new DataParameter("@ProductCode",dt.Rows[i]["ProductCode"].ToString()),
                                            new DataParameter("@ProductName", dt.Rows[i]["ProductName"].ToString()),
                                            new DataParameter("@CategoryCode",dt.Rows[i]["CategoryCode"].ToString()),
                                            new DataParameter("@ProductEName",dt.Rows[i]["ProductEName"].ToString()),
                                            new DataParameter("@FactoryID",dt.Rows[i]["FactoryID"].ToString()),
                                            new DataParameter("@ModelNo",dt.Rows[i]["ModelNo"].ToString()),
                                            new DataParameter("@Spec", dt.Rows[i]["Spec"].ToString()),
                                            new DataParameter("@Barcode", dt.Rows[i]["Barcode"].ToString()),
                                            new DataParameter("@Propertity",dt.Rows[i]["Propertity"].ToString()),
                                            new DataParameter("@Unit", dt.Rows[i]["Unit"].ToString()),
                                            new DataParameter("@Length",dt.Rows[i]["Length"].ToString()),
                                            new DataParameter("@Width", dt.Rows[i]["Width"].ToString()),
                                            new DataParameter("@Height",dt.Rows[i]["Height"].ToString()),
                                            new DataParameter("@Material", dt.Rows[i]["Material"].ToString()),
                                            new DataParameter("@Weight", dt.Rows[i]["Weight"].ToString()),
                                            new DataParameter("@Color", dt.Rows[i]["Color"].ToString()),
                                            new DataParameter("@StandardNo",dt.Rows[i]["StandardNo"].ToString()),
                                            new DataParameter("@PartNo",dt.Rows[i]["PartNo"].ToString()),
                                            new DataParameter("@ValidPeriod",dt.Rows[i]["ValidPeriod"].ToString()),
                                            new DataParameter("@Description", dt.Rows[i]["Description"].ToString()),
                                            new DataParameter("@AreaCode", dt.Rows[i]["AreaCode"].ToString()),
                                            new DataParameter("@Memo",dt.Rows[i]["Memo"].ToString()),
                                            new DataParameter("@Creator", "admin"),
                                            new DataParameter("@Updater", "admin"),                        
                                };
                    bll.ExecNonQuery("Cmd.DeleteProduct", new DataParameter[] { new DataParameter("{0}", "'"+dt.Rows[i]["ProductCode"].ToString()+"'") });
                    bll.ExecNonQuery("Cmd.InsertProduct", paras);               
                } 
                    jr.status = 1;
                    jr.msg = "导入成功！";
            }
            else
            {
                jr.status = 3;
                jr.msg = "你选择的不是Excel文件！";
                Response(context, jr); 
            }
        }
        catch (Exception ex)
        {
            jr.status = 0;
            jr.msg = ex.Message;
        }
        Response(context, jr);  
    }
    public void Response(HttpContext context,JsonResult json)
    {
         string strJson = JsonConvert.SerializeObject(json);
        context.Response.Clear();
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        context.Response.Write(strJson);
        context.Response.End();
    }
    public bool IsReusable {
        get {
            return false;
        }
    }
    //public DataTable GetExcelDatatable(string fileUrl, string table)
    //{
    //    //office2007之前 仅支持.xls
    //    //const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";
    //    //支持.xls和.xlsx，即包括office2010等版本的   HDR=Yes代表第一行是标题，不是数据；
    //    const string cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
    //    DataTable dt = null;
    //    //建立连接
    //    OleDbConnection conn = new OleDbConnection(string.Format(cmdText, fileUrl));
    //    try
    //    {
    //        //打开连接
    //        if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
    //        {
    //            conn.Open();
    //        }
    //      DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
    //        //获取Excel的第一个Sheet名称
    //        string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();
    //        //查询sheet中的数据
    //        string strSql = "select * from [" + sheetName + "]";
    //        OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
    //        DataSet ds = new DataSet();
    //        da.Fill(ds, table);
    //        dt = ds.Tables[0];
    //        return dt;
    //    }
    //    catch (Exception exc)
    //    {
    //        throw exc;
    //    }
    //    finally
    //    {
    //        conn.Close();
    //        conn.Dispose();
    //    }
    //}
    /// <summary>
    /// 将excel中的数据导入到DataTable中
    /// </summary>
    /// <param name="sheetName">excel工作薄sheet的名称</param>
    /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
    /// <returns>返回的DataTable</returns>

    public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
    { 
        DataTable dataTable = null;
        FileStream fs = null;
        DataColumn column = null;
        DataRow dataRow = null;
        IWorkbook workbook = null;
        ISheet sheet = null;
        IRow row = null;
        ICell cell = null;
        int startRow = 0;
        try
        {
                using (fs = File.OpenRead(filePath))
               {
                    // 2007版本
                    if (filePath.IndexOf(".xlsx") > 0)
                       workbook = new XSSFWorkbook(fs);
                    // 2003版本
                     else if (filePath.IndexOf(".xls") > 0)
                       workbook = new HSSFWorkbook(fs);
 
                    if (workbook != null)
                     {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                       dataTable = new DataTable();
                        if (sheet != null)
                         {
                            int rowCount = sheet.LastRowNum;//总行数
                           if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行
                                 int cellCount = firstRow.LastCellNum;//列数
 
                                //构建datatable的列
                                 if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取
                                     for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                     {
                                        cell = firstRow.GetCell(i);
                                         if (cell != null)
                                        {
                                             if (cell.StringCellValue != null)
                                             {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                             }
                                         }
                                     }
                                 }
                                 else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                   {
                                         column = new DataColumn("column" + (i + 1));
                                         dataTable.Columns.Add(column);
                                     }
                                 }
 
                                 //填充行
                                 for (int i = startRow; i <= rowCount; ++i)
                                {
                                      row = sheet.GetRow(i);
                                    if (row == null) continue;
 
                                    dataRow = dataTable.NewRow();
                                     for (int j = row.FirstCellNum; j < cellCount; ++j)
                                     {
                                        cell = row.GetCell(j);                                        
                                         if (cell == null)
                                         {
                                             dataRow[j] = "";
                                        }
                                         else
                                         {
                                           //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                             switch (cell.CellType)
                                            {
                                              case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                     //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                       dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                     break;
                                                case CellType.String:
                                                     dataRow[j] = cell.StringCellValue;
                                                     break;
                                            }
                                         }
                                     }
                                    dataTable.Rows.Add(dataRow);
                                 }
                             }
                        }
                    }
                 }
                 return dataTable;
            }

        catch (Exception)
        {
            if (fs != null)
            {
                fs.Close();
            }
            return null;
        }
    }

    
    
}