<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Data;
using System.Data.OleDb;
using Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
                DataTable dt = GetExcelDatatable(Path, "Product");        
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    paras = new DataParameter[] { 
                                            new DataParameter("@ProductCode",dt.Rows[i]["ProductCode"].ToString()),
                                            new DataParameter("@ProductName", dt.Rows[i]["ProductName"].ToString()),
                                            new DataParameter("@CategoryCode",""),
                                            new DataParameter("@ProductEName", ""),
                                            new DataParameter("@FactoryID", ""),
                                            new DataParameter("@ModelNo",""),
                                            new DataParameter("@Spec", ""),
                                            new DataParameter("@Barcode", ""),
                                            new DataParameter("@Propertity",""),
                                            new DataParameter("@Unit", ""),
                                            new DataParameter("@Length",""),
                                            new DataParameter("@Width", ""),
                                            new DataParameter("@Height",""),
                                            new DataParameter("@Material", ""),
                                            new DataParameter("@Weight", ""),
                                            new DataParameter("@Color", ""),
                                            new DataParameter("@StandardNo",""),
                                            new DataParameter("@PartNo",""),
                                            new DataParameter("@ValidPeriod", ""),
                                            new DataParameter("@Description", ""),
                                            new DataParameter("@AreaCode", ""),
                                            new DataParameter("@Memo",""),
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
    public DataTable GetExcelDatatable(string fileUrl, string table)
    {
        //office2007之前 仅支持.xls
        //const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";
        //支持.xls和.xlsx，即包括office2010等版本的   HDR=Yes代表第一行是标题，不是数据；
        const string cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
        DataTable dt = null;
        //建立连接
        OleDbConnection conn = new OleDbConnection(string.Format(cmdText, fileUrl));
        try
        {
            //打开连接
            if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
          DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //获取Excel的第一个Sheet名称
            string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();
            //查询sheet中的数据
            string strSql = "select * from [" + sheetName + "]";
            OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, table);
            dt = ds.Tables[0];
            return dt;
        }
        catch (Exception exc)
        {
            throw exc;
        }
        finally
        {
            conn.Close();
            conn.Dispose();
        }
    }
}