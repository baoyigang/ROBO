 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FastReport;
using FastReport.Data;
using FastReport.Utils;
using System.Data;
using System.IO;
using Util;


public partial class WebUI_Query_StockPrint : BasePage
{
    protected string BillID;
    protected string PdfName;
    protected string PagePath;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            BillID = Request.QueryString["BillID"] + "";
            FormID = Request.QueryString["FormID"] + "";
            PdfName = Request.QueryString["PdfName"] + "";
            SubModuleCode = Request.QueryString["SubModuleCode"] + "";
            SqlCmd = Request.QueryString["SqlCmd"] + "";
            PagePath = Request.QueryString["PagePath"] + "";
        }
 

    }
    
    protected void WebReport1_StartReport(object sender, EventArgs e)
    {
       
    }
    protected void btnPreview_Click(object sender, EventArgs e)
    {
        
        //WebReport1.Refresh();
    }
    
    private bool LoadRpt()
    {
        try
        {
            
             


 
            //WebReport1.Report = new Report();
            //WebReport1.Report.Load(System.AppDomain.CurrentDomain.BaseDirectory + @"RptFiles\" + frx);

            //BLL.BLLBase bll = new BLL.BLLBase();

            //DataTable dt = bll.FillDataTable(Comds);//, new DataParameter[] { new DataParameter("{0}", BillID) }
            //WebReport1.Report.RegisterData(dt, "ProductQuery");
        }
        catch (Exception ex)
        {
        }
        return true;
    }

}
 