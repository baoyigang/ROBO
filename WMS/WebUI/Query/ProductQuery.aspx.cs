 

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


public partial class WebUI_Query_ProductQuery : BasePage
{
    private string strWhere;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            rptview.Visible = false;
            BindOther();
            writeJsvar("", "", "");

        }
        else
        {
            string hdnwh = HdnWH.Value;
            int W = int.Parse(hdnwh.Split('#')[0]);
            int H = int.Parse(hdnwh.Split('#')[1]);
            WebReport1.Width = W - 60;
            WebReport1.Height = H - 55;
            if (this.HdnProduct.Value.Length > 0)
                this.btnProduct.Text = "取消指定";
            else
                this.btnProduct.Text = "指定";
        }
        SetTextReadOnly(this.txtProductName);

    }
    private void BindOther()
    {
        BLL.BLLBase bll = new BLL.BLLBase();
        DataTable ProductType = bll.FillDataTable("Cmd.SelectProductCategory", new DataParameter[] { new DataParameter("{0}", " IsFixed<>'1'") });
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
        if (this.ddlProductType.SelectedValue != "")
        {
            strWhere += string.Format(" and CategoryCode='{0}'", this.ddlProductType.SelectedValue);
        }


        if (this.HdnProduct.Value.Length == 0)
        {
            if (this.txtProductCode.Text.Trim().Length > 0)
                strWhere += string.Format(" and pallet.ProductCode='{0}'", this.txtProductCode.Text);
        }
        else
        {
            strWhere += " and pallet.ProductCode in (" + this.HdnProduct.Value + ") ";
        }
        

    }
    private bool LoadRpt()
    {
        try
        {
            GetStrWhere();
            string frx = "ProductDetailQuery.frx";
            string Comds = "WMS.SelectProductDetailQuery";

            if (rpt2.Checked)
            {
                frx = "ProductTotalQuery.frx";
                Comds = "WMS.SelectProductTotalQuery";

            }
            WebReport1.Report = new Report();
            WebReport1.Report.Load(System.AppDomain.CurrentDomain.BaseDirectory + @"RptFiles\" + frx);

            BLL.BLLBase bll = new BLL.BLLBase();

            DataTable dt = bll.FillDataTable(Comds, new DataParameter[] { new DataParameter("{0}", strWhere) });



            if (dt.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "alert('您所选择的条件没有资料!');", true);
            }

            WebReport1.Report.RegisterData(dt, "ProductQuery");
        }
        catch (Exception ex)
        {
        }
        return true;
    }

}
 