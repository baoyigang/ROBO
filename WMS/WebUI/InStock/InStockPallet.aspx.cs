using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Util;


public partial class WebUI_InStock_InStockPallet : BasePage
{
    protected string strID;
    BLL.BLLBase bll = new BLL.BLLBase();
    protected void Page_Load(object sender, EventArgs e)
    {
       
        this.dgViewSub1.PageSize = pageSubSize;
        if (!IsPostBack)
        {
            BindData("");
            this.btnAddDetail.Enabled = false;
            this.btnDelDetail.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnReceiptDetail.Enabled = false;
        }

        ScriptManager.RegisterStartupScript(this.updatePanel1, this.updatePanel1.GetType(), "Resize", "resize();BindEvent();", true);
        SetTextReadOnly(this.txtTotalQty, txtCellCode);


    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindData(this.txtSearch.Text.Trim());
        DataTable dt = (DataTable)ViewState[FormID + "_Edit_dgViewSub1"];
        if (dt.Rows.Count > 0)
        {
            this.txtCellCode.Text = dt.Rows[0]["CellCode"].ToString();
            if (this.txtCellCode.Text.Length > 0)
            {
                this.btnAddDetail.Enabled = false;
                this.btnDelDetail.Enabled = false;
                this.btnSave.Enabled = false;
                this.btnReceiptDetail.Enabled = false;
                return;
            }
        }

        this.btnAddDetail.Enabled = true;
        this.btnDelDetail.Enabled = true;
        this.btnReceiptDetail.Enabled = true;
        this.btnSave.Enabled = true;
    }


    private void BindData(string PalletCode)
    {
        DataTable dt = bll.FillDataTable("WMS.SelectWmsPallet", new DataParameter[] { new DataParameter("{0}", string.Format("PalletCode='{0}'", PalletCode)) });
        ViewState[FormID + "_Edit_dgViewSub1"] = dt;
        this.dgViewSub1.DataSource = dt;
        this.dgViewSub1.DataBind();
        MovePage("Edit", this.dgViewSub1, 0, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

   
    protected void dgViewSub1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowIndex % 2 == 1)
                e.Row.CssClass = " bottomtable";
            DataRowView drv = e.Row.DataItem as DataRowView;

            DataRow dr = drv.Row;

            SetTextReadOnly((TextBox)e.Row.FindControl("ProductName"), (TextBox)e.Row.FindControl("Unit"), (TextBox)e.Row.FindControl("Indate"));
           
            if (drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("IsLock")].ToString()=="1")
            {
                SetTextReadOnly((TextBox)e.Row.FindControl("ProductCode"), (TextBox)e.Row.FindControl("Quantity"), (TextBox)e.Row.FindControl("Weight"));
                ((CheckBox)e.Row.FindControl("cbSelect")).Enabled = false;
                ((Button)e.Row.FindControl("btnProduct")).Enabled = false;
            }
            ((Label)e.Row.FindControl("RowID")).Text = drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("RowID")].ToString();
            ((TextBox)e.Row.FindControl("ProductCode")).Text = drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("ProductCode")].ToString();
            ((TextBox)e.Row.FindControl("ProductName")).Text = drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("ProductName")].ToString();
            ((TextBox)e.Row.FindControl("Quantity")).Text = drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("Quantity")].ToString();
            ((TextBox)e.Row.FindControl("Unit")).Text = drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("Unit")].ToString();
            ((TextBox)e.Row.FindControl("Weight")).Text = drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("Weight")].ToString();
            ((TextBox)e.Row.FindControl("InDate")).Text = ToYMD(drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("InDate")]);
            ((TextBox)e.Row.FindControl("SubMemo")).Text = drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("Memo")].ToString();
            ((TextBox)e.Row.FindControl("ModelNo")).Text = drv.Row.ItemArray[drv.DataView.Table.Columns.IndexOf("ModelNo")].ToString();
        }
    }

    protected void btnAddDetail_Click(object sender, EventArgs e)
    {
        UpdateTempSub(this.dgViewSub1);
        DataTable dt = (DataTable)ViewState[FormID + "_Edit_dgViewSub1"];
        DataTable dt1 = Util.JsonHelper.Json2Dtb(hdnMulSelect.Value);
        if (dt1.Rows.Count > 0)
        {
            SetTextReadOnly(this.txtSearch);
        }

        DataView dv = dt.DefaultView;
        dv.Sort = "RowID";
        dt = dv.ToTable();

        DataRow dr;
        int cur = dt.Rows.Count;
        for (int i = 0; i < dt1.Rows.Count; i++)
        {

            dr = dt.NewRow();
            dt.Rows.InsertAt(dr, cur + i);
            dr["RowID"] = cur + i + 1;
            dr["ProductCode"] = dt1.Rows[i]["ProductCode"];
            dr["ProductName"] = dt1.Rows[i]["ProductName"];
            dr["ModelNo"] = dt1.Rows[i]["ModelNo"];
            dr["Unit"] = dt1.Rows[i]["Unit"];
            dr["PalletCode"] = this.txtSearch.Text;
            dr["Quantity"] = 1;
            dr["IsLock"] = "0";
        }

        this.dgViewSub1.DataSource = dt;
        this.dgViewSub1.DataBind();
        ViewState[FormID + "_Edit_dgViewSub1"] = dt;
        MovePage("Edit", this.dgViewSub1, this.dgViewSub1.PageIndex, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
        
    }
    protected void btnReceiptDetail_Click(object sender, EventArgs e)
    {
        UpdateTempSub(this.dgViewSub1);
        DataTable dt = (DataTable)ViewState[FormID + "_Edit_dgViewSub1"];
        DataTable dt1 = Util.JsonHelper.Json2Dtb(hdnMulSelect.Value);
        if (dt1.Rows.Count > 0)
        {
            SetTextReadOnly(this.txtSearch);
        }

        DataView dv = dt.DefaultView;
        dv.Sort = "RowID";
        dt = dv.ToTable();

        DataRow dr;
        int cur = dt.Rows.Count;
        for (int i = 0; i < dt1.Rows.Count; i++)
        {

            dr = dt.NewRow();
            dt.Rows.InsertAt(dr, cur + i);
            dr["RowID"] = cur + i + 1;
            dr["ProductCode"] = dt1.Rows[i]["ProductCode"];
            dr["ProductName"] = dt1.Rows[i]["ProductName"];
            dr["ModelNo"] = dt1.Rows[i]["ModelNo"];
            dr["Unit"] = dt1.Rows[i]["Unit"];
            dr["PalletCode"] = this.txtSearch.Text;
            dr["Quantity"] = dt1.Rows[i]["Quantity"];
            dr["IsLock"] = "0";
        }

        this.dgViewSub1.DataSource = dt;
        this.dgViewSub1.DataBind();
        ViewState[FormID + "_Edit_dgViewSub1"] = dt;
        MovePage("Edit", this.dgViewSub1, this.dgViewSub1.PageIndex, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);

    }
    protected void btnDelDetail_Click(object sender, EventArgs e)
    {
        UpdateTempSub(this.dgViewSub1);
        DataTable dt = (DataTable)ViewState[FormID + "_Edit_" + dgViewSub1.ID];
        int RowID = 0;
        for (int i = 0; i < this.dgViewSub1.Rows.Count; i++)
        {
            CheckBox cb = (CheckBox)(this.dgViewSub1.Rows[i].FindControl("cbSelect"));
            if (cb != null && cb.Checked && cb.Enabled)
            {
                Label hk = (Label)(this.dgViewSub1.Rows[i].Cells[1].FindControl("RowID"));
                RowID = int.Parse(hk.Text);
                DataRow[] drs = dt.Select(string.Format("RowID ={0}", hk.Text));
                for (int j = 0; j < drs.Length; j++)
                    dt.Rows.Remove(drs[j]);

            }
        }

        DataRow[] drsExist = dt.Select("", "RowID");
        for (int i = 0; i < drsExist.Length; i++)
        {
            drsExist[i]["RowID"] = i + 1;
        }



          drsExist = dt.Select("IsLock='0'");
        if (drsExist.Length == 0)
        {
            this.txtSearch.Attributes.Remove("ReadOnly");
        }

        this.dgViewSub1.DataSource = dt;
        this.dgViewSub1.DataBind();
        ViewState[FormID + "_Edit_" + dgViewSub1.ID] = dt;
        MovePage("Edit", this.dgViewSub1, this.dgViewSub1.PageIndex, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);

    }





    protected void btnProduct_Click(object sender, EventArgs e)
    {
        UpdateTempSub(this.dgViewSub1);
        DataTable dt = (DataTable)ViewState[FormID + "_Edit_dgViewSub1"];
        DataTable dt1 = Util.JsonHelper.Json2Dtb(hdnMulSelect.Value);
        int cur = int.Parse(((Button)sender).ClientID.Split('_')[1].Replace("ctl", "")) - 2 + this.dgViewSub1.PageSize * dgViewSub1.PageIndex;
        if (dt1.Rows.Count > 0)
        {
            DataRow[] drs = dt.Select("RowID>" + (cur + 1));
            for (int j = 0; j < drs.Length; j++)
            {
                drs[j].BeginEdit();
                drs[j]["RowID"] = cur + j + 1 + dt1.Rows.Count;
                drs[j].EndEdit();
            }
        }
        DataView dv = dt.DefaultView;
        dv.Sort = "RowID";
        dt = dv.ToTable();

        DataRow dr;
        for (int i = 0; i < dt1.Rows.Count; i++)
        {
            if (i == 0)
            {
                dr = dt.Rows[cur];
            }
            else
            {
                dr = dt.NewRow();
                dt.Rows.InsertAt(dr, cur + i);

            }
            dr["RowID"] = i + cur + 1;
            dr["PalletCode"] = this.txtSearch.Text;
            dr["ProductCode"] = dt1.Rows[i]["ProductCode"];
            dr["ProductName"] = dt1.Rows[i]["ProductName"];
            dr["ModelNo"] = dt1.Rows[i]["ModelNo"];
            dr["Unit"] = dt1.Rows[i]["Unit"];
            dr["Quantity"] = 1;
            dr["IsLock"] = "0";

        }

        this.dgViewSub1.DataSource = dt;
        this.dgViewSub1.DataBind();
        ViewState[FormID + "_Edit_dgViewSub1"] = dt;
        MovePage("Edit", this.dgViewSub1, this.dgViewSub1.PageIndex, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }



    public override void UpdateTempSub(GridView dgv)
    {
        DataTable dt1 = (DataTable)ViewState[FormID + "_Edit_" + dgv.ID];
        if (dt1.Rows.Count == 0)
        {

            DataTable dt = bll.FillDataTable("WMS.SelectWmsPallet", new DataParameter[] { new DataParameter("{0}", string.Format("PalletCode='{0}'", this.txtSearch.Text)) });
            if (dt.Rows.Count > 0)
            {
                this.txtCellCode.Text = dt.Rows[0]["CellCode"].ToString();
                if (this.txtCellCode.Text.Length > 0)
                {
                    this.btnAddDetail.Enabled = false;
                    this.btnDelDetail.Enabled = false;
                    this.btnSave.Enabled = false;
                    this.btnReceiptDetail.Enabled = false;
                    ViewState[FormID + "_Edit_" + dgv.ID] = dt;
                    return;
                }
                else
                {
                    ViewState[FormID + "_Edit_" + dgv.ID] = dt;
 
                }
            }
            return;
        } 
        DataRow dr;
        for (int i = 0; i < dgv.Rows.Count; i++)
        {
            dr = dt1.Rows[i + dgv.PageSize * dgv.PageIndex];
            dr.BeginEdit();
            dr["RowID"] = ((Label)dgv.Rows[i].FindControl("RowID")).Text;
            dr["ProductCode"] = ((TextBox)dgv.Rows[i].FindControl("ProductCode")).Text;
            dr["Quantity"] = ((TextBox)dgv.Rows[i].FindControl("Quantity")).Text;
            dr["Weight"] = ((TextBox)dgv.Rows[i].FindControl("Weight")).Text.Trim() == "" ? 0 : decimal.Parse(((TextBox)dgv.Rows[i].FindControl("Weight")).Text.Trim());
            dr["Memo"] = ((TextBox)dgv.Rows[i].FindControl("SubMemo")).Text;
            dr.EndEdit();
        }
        dt1.AcceptChanges();
        

        object o = dt1.Compute("SUM(Quantity)", "");
        this.txtTotalQty.Text = o.ToString();
        ViewState[FormID + "_Edit_" + dgv.ID] = dt1;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        UpdateTempSub(this.dgViewSub1);

        DataTable dt = (DataTable)ViewState[FormID + "_Edit_dgViewSub1"];
        DataTable dtProduct = dt.DefaultView.ToTable("Product", true, new string[] { "ProductCode", "ProductName" });
        for (int i = 0; i < dtProduct.Rows.Count; i++)
        {
            object o = dt.Compute("Count(ProductCode)", string.Format("ProductCode='{0}' and IsLock='0'", dtProduct.Rows[i]["ProductCode"]));
            if (o != null)
            {
                int Qty = int.Parse(o.ToString());
                if (Qty > 1)
                {
                    JScript.ShowMessage(this.updatePanel1, dtProduct.Rows[i]["ProductName"].ToString() + " 重复，请重新修改后保存！");
                    return;
                }
            }
        }
        if (bll.GetRowCount("WMS_Pallet", string.Format("PalletCode='{0}' and CellCode<>''", txtSearch.Text)) > 0)
        {
            JScript.ShowMessage(this.updatePanel1, "托盘已经入库,无法保存！");
            return;
        }



        DataRow[] drs = dt.Select("IsLock='0'");
        string[] Comds = new string[drs.Length + 1];
        List<DataParameter[]> paras = new List<DataParameter[]>();
        DataParameter[] para;
        for (int i = 0; i < drs.Length; i++)
        {
            DataRow dr = drs[i];
            int HasCount = bll.GetRowCount("WMS_Pallet", string.Format("PalletCode='{0}' and RowID='{1}'", dr["PalletCode"].ToString(), dr["RowID"].ToString()));
            if (HasCount > 0)
            {
                Comds[i] = "WMS.UpdatePallet";
                para = new DataParameter[] {    new DataParameter("@ProductCode",dr["ProductCode"].ToString()), 
                                                new DataParameter("@Quantity",dr["Quantity"].ToString()),
                                                new DataParameter("@Weight",dr["Weight"].ToString()),
                                                new DataParameter("@Memo",dr["Memo"].ToString()),
                                                new DataParameter("@PalletCode",dr["PalletCode"].ToString()),
                                                new DataParameter("@RowID",dr["RowID"].ToString()),
                                                new DataParameter("@Creator", Session["EmployeeCode"].ToString()),
                                                new DataParameter("@Updater", Session["EmployeeCode"].ToString())
                                            };

            }
            else
            {
                Comds[i] = "WMS.InsertPallet";
                para = new DataParameter[] {   
                                                new DataParameter("@PalletCode",dr["PalletCode"].ToString()),
                                                new DataParameter("@RowID",dr["RowID"].ToString()),
                                                new DataParameter("@ProductCode",dr["ProductCode"].ToString()), 
                                                new DataParameter("@Quantity",dr["Quantity"].ToString()),
                                                new DataParameter("@Weight",dr["Weight"].ToString().Trim().Length==0?"0":dr["Weight"].ToString()),
                                                new DataParameter("@Memo",dr["Memo"].ToString()),
                                                new DataParameter("@Creator", Session["EmployeeCode"].ToString()),
                                                new DataParameter("@Updater", Session["EmployeeCode"].ToString())
                                                };

            }
            paras.Add(para);
        }
        Comds[drs.Length] = "WMS.SpCreateTaskByPallet";
        para = new DataParameter[] { new DataParameter("@PalletCode", this.txtSearch.Text), new DataParameter("@UserName", Session["EmployeeCode"].ToString()) };
        paras.Add(para);
        try
        {
            bll.ExecTran(Comds, paras);
            this.txtSearch.Text = "";
            this.txtSearch.Attributes.Remove("ReadOnly");
            BindData("");
            this.btnAddDetail.Enabled = false;
            this.btnDelDetail.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnReceiptDetail.Enabled = false;

        }
        catch (Exception ex)
        {
            JScript.ShowMessage(this.updatePanel1, ex.Message);
            BindData(this.txtSearch.Text);
            return;
        }


    }

    #region 子表绑定

    protected void btnFirstSub1_Click(object sender, EventArgs e)
    {
        MovePage("Edit", this.dgViewSub1, 0, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

    protected void btnPreSub1_Click(object sender, EventArgs e)
    {
        MovePage("Edit", this.dgViewSub1, this.dgViewSub1.PageIndex - 1, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

    protected void btnNextSub1_Click(object sender, EventArgs e)
    {
        MovePage("Edit", this.dgViewSub1, this.dgViewSub1.PageIndex + 1, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

    protected void btnLastSub1_Click(object sender, EventArgs e)
    {
        MovePage("Edit", this.dgViewSub1, this.dgViewSub1.PageCount - 1, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }

    protected void btnToPageSub1_Click(object sender, EventArgs e)
    {
        MovePage("Edit", this.dgViewSub1, int.Parse(this.txtPageNoSub1.Text) - 1, btnFirstSub1, btnPreSub1, btnNextSub1, btnLastSub1, btnToPageSub1, lblCurrentPageSub1);
    }



    #endregion
}
 