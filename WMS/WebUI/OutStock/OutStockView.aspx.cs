 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Util;
using FastReport;



public partial class WebUI_OutStock_OutStockView : BasePage
{
    private string strID;
    private string TableName = "View_WMS_BillMaster";
    private string PrimaryKey = "BillID";
    BLL.BLLBase bll = new BLL.BLLBase();
    protected void Page_Load(object sender, EventArgs e)
    {
        strID = Request.QueryString["ID"] + "";
        this.dgViewSub1.PageSize = pageSubSize;
        if (!IsPostBack)
        {
            BindDropDownList();
            DataTable dt = bll.FillDataTable("WMS.SelectBillMaster", new DataParameter[] { new DataParameter("{0}", string.Format("BillID='{0}'", strID)) });
            BindData(dt);

        }

        ScriptManager.RegisterStartupScript(this.updatePanel, this.updatePanel.GetType(), "Resize", "resize();", true);
        writeJsvar(FormID, SqlCmd, strID);
    }

    #region 绑定方法
    private void BindDropDownList()
    { 
        DataTable dtBillType = bll.FillDataTable("Cmd.SelectBillType", new DataParameter[] { new DataParameter("{0}", "Flag=2") });
        this.ddlBillTypeCode.DataValueField = "BillTypeCode";
        this.ddlBillTypeCode.DataTextField = "BillTypeName";
        this.ddlBillTypeCode.DataSource = dtBillType;
        this.ddlBillTypeCode.DataBind();

    }


    private void BindData(DataTable dt)
    {
        if (dt.Rows.Count > 0)
        {
            this.txtID.Text = dt.Rows[0]["BillID"].ToString();
            this.txtBillDate.Text = ToYMD(dt.Rows[0]["BillDate"]);
            
            this.ddlBillTypeCode.SelectedValue = dt.Rows[0]["BillTypeCode"].ToString();
            this.txtSourceBillNo.Text = dt.Rows[0]["SourceBillNo"].ToString();
            this.txtBatchNo.Text = dt.Rows[0]["BatchNo"].ToString();
             
            this.txtMemo.Text = dt.Rows[0]["Memo"].ToString();
            this.txtCreator.Text = dt.Rows[0]["Creator"].ToString();
            this.txtCreatDate.Text = ToYMD(dt.Rows[0]["CreateDate"]);
            this.txtUpdater.Text = dt.Rows[0]["Updater"].ToString();
            this.txtUpdateDate.Text = ToYMD(dt.Rows[0]["UpdateDate"]);
            this.txtChecker.Text = dt.Rows[0]["Checker"].ToString();
            this.txtCheckDate.Text = ToYMD(dt.Rows[0]["CheckDate"]);
             
            this.txtState.Text = dt.Rows[0]["StateDesc"].ToString();
            hdnState.Value = dt.Rows[0]["State"].ToString();
            if (this.txtChecker.Text.Trim() != "")
            {
                this.btnCheck.Text = "反审";
                this.btnCheck.CssClass = "ButtonAudit2";
            }
            else
            {
                this.btnCheck.Text = "审核";
                this.btnCheck.CssClass = "ButtonAudit";
            }

        }
        BindDataSub();
        SetPermission();
    }
    /// <summary>
    /// 設定權限
    /// </summary>
    private void SetPermission()
    {
        bool blnDelete = false;
        bool blnEdit = false;
        bool blnCheck = false;
        bool blnTask = false;
        bool blnUnTask = false;
        bool blnPrint = false;
        DataTable dtOP = (DataTable)(Session["DT_UserOperation"]);
        DataRow[] drs = dtOP.Select(string.Format("SubModuleCode='{0}'", Session["SubModuleCode"].ToString()));

        foreach (DataRow dr in drs)
        {
            int op = int.Parse(dr["OperatorCode"].ToString());
            switch (op)
            {
                case 1:
                    blnDelete = true;
                    break;
                case 2: //修改
                    blnEdit = true;
                    break;
                case 3:
                    blnPrint = true;
                    break;
                case 5:
                    blnCheck = true;
                    break;
                case 6:
                    blnTask = true;
                    break;
                case 7:
                    blnUnTask = true;
                    break;

            }
        }
        this.btnDelete.Enabled = blnDelete;
        this.btnEdit.Enabled = blnEdit;
        this.btnCheck.Enabled = blnCheck;
        this.btnUpERP.Enabled = false;
        this.btnTask.Enabled = false;
        this.btnUnTask.Enabled = false;
        this.btnPrint.Enabled = blnPrint;
        int State = int.Parse(hdnState.Value);
        int IsUpErp = 0;

        if (State == 1)
        {
            this.btnDelete.Enabled = false;
            this.btnEdit.Enabled = false;
            this.btnTask.Enabled = blnTask;

        }
        if (State == 2)
        {
            this.btnUnTask.Enabled = blnUnTask;
        }
        if (State > 1)
        {
            this.btnDelete.Enabled = false;
            this.btnEdit.Enabled = false;
            this.btnCheck.Enabled = false;
        }

        if (State == 4 && IsUpErp == 0)
            this.btnUpERP.Enabled = blnCheck;

    }
    private void BindDataSub()
    {
        DataTable dt = bll.FillDataTable("WMS.SelectBillDetail", new DataParameter[] { new DataParameter("{0}", string.Format("BillID='{0}'", this.txtID.Text)) });
        ViewState[FormID + "_View_dgViewSub1"] = dt;
        this.dgViewSub1.DataSource = dt;
        this.dgViewSub1.DataBind();
        object o = dt.Compute("SUM(Quantity)", "");
        this.txtTotalQty.Text = o.ToString();
        MovePage("View", this.dgViewSub1, 0, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);

    }
    private void BindDataNull()
    {
        this.txtID.Text = "";
        this.txtBillDate.Text = "";
        this.txtBatchNo.Text = "";
        this.txtSourceBillNo.Text = "";
        this.txtMemo.Text = "";
        this.txtCreator.Text = "";
        this.txtCreatDate.Text = "";
        this.txtUpdater.Text = "";
        this.txtUpdateDate.Text = "";
        this.txtChecker.Text = "";
        this.txtCheckDate.Text = "";
        hdnState.Value = "0";
    }
    #endregion

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string strID = this.txtID.Text;
        int Count = bll.GetRowCount("VUsed_WMS_BillMaster", string.Format("BillID='{0}'", this.txtID.Text.Trim()));
        if (Count > 0)
        {
            JScript.ShowMessage(this.updatePanel, "该出库单号已被其它单据使用，请调整后再删除！");
            return;
        }

        string[] comds = new string[2];
        comds[0] = "WMS.DeleteBillMaster";
        comds[1] = "WMS.DeleteBillDetail";
        List<DataParameter[]> paras = new List<DataParameter[]>();
        paras.Add(new DataParameter[] { new DataParameter("{0}", "'" + strID + "'") });
        paras.Add(new DataParameter[] { new DataParameter("{0}", string.Format("BillID='{0}'", strID)) });
        bll.ExecTran(comds, paras);

        AddOperateLog("出库单", "删除单号：" + strID);

        btnNext_Click(sender, e);
        if (this.txtID.Text == strID)
        {
            btnPre_Click(sender, e);
            if (this.txtID.Text == strID)
            {
                BindDataNull();
            }
        }

    }

    protected void dgViewSub1_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    #region 上下笔事件
    protected void btnFirst_Click(object sender, EventArgs e)
    {
        BindData(bll.GetRecord("F", TableName, "BillID like 'OS%'", PrimaryKey, this.txtID.Text));
    }
    protected void btnPre_Click(object sender, EventArgs e)
    {
        BindData(bll.GetRecord("P", TableName, "BillID like 'OS%'", PrimaryKey, this.txtID.Text));
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        BindData(bll.GetRecord("N", TableName, "BillID like 'OS%'", PrimaryKey, this.txtID.Text));
    }
    protected void btnLast_Click(object sender, EventArgs e)
    {
        BindData(bll.GetRecord("L", TableName, "BillID like 'OS%'", PrimaryKey, this.txtID.Text));
    }
    #endregion

    #region 子表绑定

    protected void btnFirstSub1_Click(object sender, EventArgs e)
    {
        MovePage("View", this.dgViewSub1, 0, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

    protected void btnPreSub1_Click(object sender, EventArgs e)
    {
        MovePage("View", this.dgViewSub1, this.dgViewSub1.PageIndex - 1, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

    protected void btnNextSub1_Click(object sender, EventArgs e)
    {
        MovePage("View", this.dgViewSub1, this.dgViewSub1.PageIndex + 1, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

    protected void btnLastSub1_Click(object sender, EventArgs e)
    {
        MovePage("View", this.dgViewSub1, this.dgViewSub1.PageCount - 1, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

    protected void btnToPageSub1_Click(object sender, EventArgs e)
    {
        MovePage("View", this.dgViewSub1, int.Parse(this.txtPageNoSub1.Text) - 1, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }



    #endregion

    protected void btnCheck_Click(object sender, EventArgs e)
    {
        DataParameter[] paras = new DataParameter[4];
        if (this.btnCheck.Text == "审核")
        {
            paras[0] = new DataParameter("@Checker", Session["EmployeeCode"].ToString());
            paras[1] = new DataParameter("{0}", "getdate()");
            paras[2] = new DataParameter("@State", 1);
        }
        else
        {
            int State = int.Parse(bll.GetFieldValue("WMS_BillMaster", "State", string.Format("BillID='{0}'", this.txtID.Text)));
            if (State > 1)
            {
                JScript.ShowMessage(this.updatePanel, this.txtID.Text + " 单号已经作业，不能进行反审。");
                return;
            }
            paras[0] = new DataParameter("@Checker", "");
            paras[1] = new DataParameter("{0}", "null");
            paras[2] = new DataParameter("@State", 0);
        }
        paras[3] = new DataParameter("@BillID", this.txtID.Text);


        bll.ExecNonQuery("WMS.UpdateCheckBillMaster", paras);
        AddOperateLog("出库单 ", btnCheck.Text + " " + txtID.Text);

        DataTable dt = bll.FillDataTable("WMS.SelectBillMaster", new DataParameter[] { new DataParameter("{0}", string.Format("BillID='{0}'", strID)) });
        BindData(dt);
    }
    protected void btnUpERP_Click(object sender, EventArgs e)
    {
        try
        {
            //ERP.ErpDataService erp = new ERP.ErpDataService();
            //string MSG = "";
            //if (!erp.transBatchOutStock(this.txtID.Text, out MSG))
            //{
            //    JScript.ShowMessage(this.updatePanel, this.txtID.Text + " Erp出错：" + MSG);
            //    return;
            //}
            //DataTable dt = bll.FillDataTable("WMS.SelectBillMaster", new DataParameter[] { new DataParameter("{0}", string.Format("BillID='{0}'", strID)) });
            //BindData(dt);
        }
        catch (Exception ex)
        {
            JScript.ShowMessage(this.updatePanel, ex.Message);
        }
    }
    protected void btnTask_Click(object sender, EventArgs e)
    {
        BLL.BLLBase bll = new BLL.BLLBase();
        int State = int.Parse(bll.GetFieldValue("WMS_BillMaster", "State", string.Format("BillID='{0}'", this.txtID.Text)));
        if (State == 0)
        {
            JScript.ShowMessage(this.updatePanel, this.txtID.Text + "单号还未审核不能作业，请审核后，再进行出库作业。");
            return;
        }
        if (State > 1)
        {
            JScript.ShowMessage(this.updatePanel, this.txtID.Text + "单号已经作业，不能再进行出库作业。");
            return;
        }
        try
        {
            bll.ExecNonQueryTran("WMS.SpOutStockTask", new DataParameter[] { new DataParameter("@BillID", this.txtID.Text), new DataParameter("@UserName", Session["EmployeeCode"].ToString()) });
            AddOperateLog("出库单", "出库作业单号：" + this.txtID.Text);
            DataTable dt = bll.FillDataTable("WMS.SelectBillMaster", new DataParameter[] { new DataParameter("{0}", string.Format("BillID='{0}'", strID)) });
            BindData(dt);
        }
        catch (Exception ex)
        {
            JScript.ShowMessage(this.updatePanel, ex.Message);
        }

    }
    protected void btnUnTask_Click(object sender, EventArgs e)
    {
        
        BLL.BLLBase bll = new BLL.BLLBase();
        int State = int.Parse(bll.GetFieldValue("WMS_BillMaster", "State", string.Format("BillID='{0}'", this.txtID.Text)));
        if (State < 2)
        {
            JScript.ShowMessage(this.updatePanel, this.txtID.Text + "单号还未作业，不能进行取消作业。");
            return;
        }
        if (State > 2)
        {
            JScript.ShowMessage(this.updatePanel, this.txtID.Text + "单号已经执行，不能再进行取消作业。");
            return;
        }
        try
        {
            bll.ExecNonQueryTran("WMS.SpCancelOutStockTask", new DataParameter[] { new DataParameter("@BillID", strID), new DataParameter("@UserName", Session["EmployeeCode"].ToString()) });
            AddOperateLog("入库单", "入库取消作业单号：" + this.txtID.Text);
            DataTable dt = bll.FillDataTable("WMS.SelectBillMaster", new DataParameter[] { new DataParameter("{0}", string.Format("BillID='{0}'", strID)) });
            BindData(dt);
        }
        catch (Exception ex)
        {
            JScript.ShowMessage(this.updatePanel, ex.Message);
        }

    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string Path = System.AppDomain.CurrentDomain.BaseDirectory + @"RptFiles\";

        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Path + @"PDF\");
        foreach (System.IO.FileInfo fi in di.GetFiles())
        {
            TimeSpan ts = DateTime.Now - fi.CreationTime;
            if (ts.Hours >= 2)
            {
                fi.Delete();
            }

        }
        DataTable dt = bll.FillDataTable("WMS.SelectPrintBillDetail", new DataParameter[] { new DataParameter("@BillID", this.txtID.Text) });

        DataTable dtNew = dt.Clone();
        int pageNum = 0;
        if (dt.Rows.Count % 14 != 0)
            pageNum = dt.Rows.Count / 14 + 1;
        else
            pageNum = dt.Rows.Count / 14;

        for (int i = 0; i < pageNum; i++)
        {
            for (int j = 1; j <= 14; j++)
            {
                DataRow[] drs = dt.Select(string.Format("RowID={0}", i * 14 + j));
                if (drs.Length > 0)
                {
                    drs[0]["RowNum"] = j;
                    dtNew.Rows.Add(drs[0].ItemArray);

                }
                else
                {
                    DataRow dr = dtNew.NewRow();
                    dr["BillID"] = this.txtID.Text;
                    dr["RowID"] = i * 14 + j;
                    dr["RowNum"] = j;
                    dtNew.Rows.Add(dr);

                }

            }

        }

        using (Report report = new Report())
        {
            report.Load(Path + @"OutStockView.frx");

            report.RegisterData(dtNew, "OutStockView");

            report.Prepare();
            //report.FinishReport += new EventHandler(report_FinishReport);
            string FileName = Path + @"PDF\OutStockView_" + this.txtID.Text + ".pdf";
            FastReport.Export.Pdf.PDFExport pdfExport = new FastReport.Export.Pdf.PDFExport();

            report.Export(pdfExport, FileName);

            FileName = "OutStockView_" + this.txtID.Text + ".pdf";
            Response.Redirect("../Query/StockPrint.aspx?FormID=" + FormID + "&BillID=" + this.txtID.Text + "&PdfName=" + FileName + "&SubModuleCode=" + SubModuleCode + "&SqlCmd=" + SqlCmd + "&PagePath=OutStock");

        }
    }
}
