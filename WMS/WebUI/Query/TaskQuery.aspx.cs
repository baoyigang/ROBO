 


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


public partial class WebUI_Query_TaskQuery : BasePage
{
    private string strWhere;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            rptview.Visible = false;
            BindOther();
            this.txtEndDate.DateValue = DateTime.Now;
            this.txtStartDate.DateValue = DateTime.Now.AddMonths(-1);
            writeJsvar("", "", "");


        }
        else
        {
            string hdnwh = HdnWH.Value;
            int W = int.Parse(hdnwh.Split('#')[0]);
            int H = int.Parse(hdnwh.Split('#')[1]);
            if (W != 0)
            {
                WebReport1.Width = W - 30;
                WebReport1.Height = H - 65;
            }
            if (this.HdnProduct.Value.Length > 0)
                this.btnProduct.Text = "取消指定";
            else
                this.btnProduct.Text = "指定";



            ScriptManager.RegisterStartupScript(this, this.GetType(), "", "BindEvent();", true);
        }
        SetTextReadOnly(this.txtProductName);

    }
    private void BindOther()
    {
        BLL.BLLBase bll = new BLL.BLLBase();
        DataTable ProductType = bll.FillDataTable("Cmd.SelectProductCategory", new DataParameter[] { new DataParameter("{0}", "cmd.AreaCode='001' AND IsFixed !='1'") });
        DataRow dr = ProductType.NewRow();
        dr["CategoryCode"] = "";
        dr["CategoryName"] = "请选择";
        ProductType.Rows.InsertAt(dr, 0);
        ProductType.AcceptChanges();

        this.ddlProductType.DataValueField = "CategoryCode";
        this.ddlProductType.DataTextField = "CategoryName";
        this.ddlProductType.DataSource = ProductType;
        this.ddlProductType.DataBind();


        DataTable dtArea = bll.FillDataTable("Cmd.SelectArea");
        dr = dtArea.NewRow();
        dr["AreaCode"] = "";
        dr["AreaName"] = "请选择";
        dtArea.Rows.InsertAt(dr, 0);
        dtArea.AcceptChanges();

        this.ddlArea.DataValueField = "AreaCode";
        this.ddlArea.DataTextField = "AreaName";
        this.ddlArea.DataSource = dtArea;
        this.ddlArea.DataBind();

        DataTable dtBillType = bll.FillDataTable("Cmd.SelectBillType", new DataParameter[] { new DataParameter("{0}", "BillTypeCode not in ('040','050')") });
        dr = dtBillType.NewRow();
        dr["BillTypeCode"] = "";
        dr["BillTypeName"] = "请选择";
        dtBillType.Rows.InsertAt(dr, 0);
        dtBillType.AcceptChanges();

        this.ddlBillType.DataValueField = "BillTypeCode";
        this.ddlBillType.DataTextField = "BillTypeName";
        this.ddlBillType.DataSource = dtBillType;
        this.ddlBillType.DataBind();

    }

    protected void ddlAreaCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        string AreaCode = ddlArea.SelectedValue;
        BLL.BLLBase bll = new BLL.BLLBase();
        DataTable ProductType = bll.FillDataTable("Cmd.SelectProductCategory", new DataParameter[] { new DataParameter("{0}", "cmd.AreaCode='" + AreaCode + "' AND IsFixed !='1'") });

        DataRow dr = ProductType.NewRow();
        dr["CategoryCode"] = "";
        dr["CategoryName"] = "请选择";
        ProductType.Rows.InsertAt(dr, 0);
        ProductType.AcceptChanges();

        this.ddlProductType.DataValueField = "CategoryCode";
        this.ddlProductType.DataTextField = "CategoryName";
        this.ddlProductType.DataSource = ProductType;
        this.ddlProductType.DataBind();

    }

    protected void WebReport1_StartReport(object sender, EventArgs e)
    {
        if (!rptview.Visible) return;
        LoadRpt();
    }
    protected void btnPreview_Click(object sender, EventArgs e)
    {
        rptview.Visible = true;
        WebReport1.Refresh();
    }
    private void GetStrWhere()
    {
        strWhere = "1=1";
        if (this.txtStartDate.tDate.Text != "")
        {
            strWhere += string.Format(" and CONVERT(nvarchar(10),TaskDate,111)>='{0}'", this.txtStartDate.tDate.Text);
        }
        if (this.txtEndDate.tDate.Text != "")
        {
            strWhere += string.Format(" and CONVERT(nvarchar(10),TaskDate,111)<='{0}'", this.txtEndDate.tDate.Text);
        }
        //任务类型
        if (this.ddlBillType.SelectedValue != "")
        {
            strWhere += string.Format(" and Task.BillTypeCode='{0}'", this.ddlBillType.SelectedValue);
        }
        //任务状态
        if (this.ddlState.SelectedValue != "0")
        {
            if (this.ddlState.SelectedValue == "1")
            {
                strWhere += " and Task.State<7";
            }
            else if (this.ddlState.SelectedValue == "2")
            {
                strWhere += " and Task.State=7";
            }
            else
            {
                strWhere += " and Task.State=9";
            }

        }
        //库区
        if (this.ddlArea.SelectedValue != "")
        {
            strWhere += string.Format(" and Task.AreaCode='{0}'", this.ddlArea.SelectedValue);
        }


        if (this.ddlProductType.SelectedValue != "")
        {
            strWhere += string.Format(" and Product.CategoryCode='{0}'", this.ddlProductType.SelectedValue);
        }


        if (this.HdnProduct.Value.Length == 0)
        {
            if (this.txtProductCode.Text.Trim().Length > 0)
                strWhere += string.Format(" and Task.ProductCode='{0}'", this.txtProductCode.Text);
        }
        else
        {
            strWhere += "and Task.ProductCode in (" + this.HdnProduct.Value + ") ";
        }

    }
    private bool LoadRpt()
    {
        try
        {
            GetStrWhere();
            string frx = "TaskQuery.frx";
            string Comds = "WMS.SelectTaskQuery";


            WebReport1.Report = new Report();
            WebReport1.Report.Load(System.AppDomain.CurrentDomain.BaseDirectory + @"RptFiles\" + frx);

            BLL.BLLBase bll = new BLL.BLLBase();

            DataTable dt = bll.FillDataTable(Comds, new DataParameter[] { new DataParameter("{0}", strWhere) });



            if (dt.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('您所选择的条件没有资料!');", true);
            }

            WebReport1.Report.RegisterData(dt, "TaskQuery");
        }
        catch (Exception ex)
        {
        }
        return true;
    }

}
 