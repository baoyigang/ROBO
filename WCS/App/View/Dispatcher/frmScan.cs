using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Util;

namespace App.View.Dispatcher
{
    public partial class frmScan : Form
    {
       
        public string strValue;
        private DataTable dtTask;
        BLL.BLLBase bll = new BLL.BLLBase();
        private DataTable dtPallet;
        private bool blnOk = false;
        private string PalletCode;
        
        public frmScan()
        {
            InitializeComponent();
        }
        public frmScan(int flag, DataTable dtInfo)
        {
            InitializeComponent();
            dtTask = dtInfo;
        }
        private void frmScan_Load(object sender, EventArgs e)
        {
            DataRow dr = dtTask.Rows[0];

            this.txtBillID.Text = dr["BillID"].ToString();
            PalletCode = dr["PalletCode"].ToString();
            this.txtPalletCode.Text = dr["PalletCode"].ToString();
            this.txtTaskNo.Text = dr["TaskNo"].ToString();
            this.txtCellCode.Text = dr["CellCode"].ToString();

            dtPallet = bll.FillDataTable("WCS.SelectCheckPalletDetail", new DataParameter[] { new DataParameter("@PalletCode", this.txtPalletCode.Text) });
            this.bsMain.DataSource = dtPallet;
            blnOk = false;

        }

        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            Common.frmSelect frm = new Common.frmSelect(true, "CMD_Product", "IsFixed='0' ");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < frm.dtSelect.Rows.Count; i++)
                {
                    DataRow dr=frm.dtSelect.Rows[i];
                    DataRow drNew = dtPallet.NewRow();
                    drNew.BeginEdit();
                    drNew["chk"] = false;
                    drNew["RowID"] = dtPallet.Rows.Count + 1;
                    drNew["PalletCode"] = this.txtPalletCode.Text;
                    drNew["ProductCode"] = dr["ProductCode"];
                    drNew["ProductName"] = dr["ProductName"];
                    drNew["ModelNo"] = dr["ModelNo"];
                    drNew["Quantity"] = 0;
                    drNew["RealQty"] = 0;
                    drNew["Indate"] = DateTime.Now;
                    drNew.EndEdit();
                    dtPallet.Rows.Add(drNew);
                }


            }
        }

        private void btndelDetail_Click(object sender, EventArgs e)
        {
            for (int i = this.dgView.Rows.Count - 1; i >= 0; i--)
            {


                DataGridViewCheckBoxCell CheckCell = (DataGridViewCheckBoxCell)this.dgView.Rows[i].Cells[0];
                if ((bool)CheckCell.EditedFormattedValue)
                {

                    DataRow[] drs = dtPallet.Select(string.Format("RowID ={0} and Quantity=0 ", this.dgView.Rows[i].Cells["colRowID"].Value.ToString()));
                    for (int j = 0; j < drs.Length; j++)
                        dtPallet.Rows.Remove(drs[j]);
                }
            }
            DataRow[] drsExist = dtPallet.Select("", "RowID");
            for (int i = 0; i < drsExist.Length; i++)
            {
                drsExist[i]["RowID"] = i + 1;
            }


        }

        private void btnGetBack_Click(object sender, EventArgs e)
        {

            try
            {

                List<string> list = new List<string>();
                List<DataParameter[]> paras = new List<DataParameter[]>();
                DataParameter[] para;

                //删除托盘明细
                para = new DataParameter[] { new DataParameter("@PalletCode", this.txtPalletCode.Text) };
                list.Add("WCS.DeletePalletDetail");
                paras.Add(para);

                //增加托盘明细

                DataRow[] drs = dtPallet.Select("RealQty<>0");
                for (int i = 0; i < drs.Length; i++)
                {
                    para = new DataParameter[] {   
                                                 new DataParameter("@PalletCode", this.txtPalletCode.Text),
                                                 new DataParameter("@RowID",i+1),
                                                 new DataParameter("@ProductCode",drs[i]["ProductCode"].ToString()),
                                                 new DataParameter("@Quantity",drs[i]["RealQty"]),
                                                 new DataParameter("@InDate",drs[i]["InDate"]),
                                                 new DataParameter("@CellCode",this.txtCellCode.Text)
                    };
                    list.Add("WCS.InsertPallet");
                    paras.Add(para);
                }
                para = new DataParameter[] {new DataParameter("@BillID",this.txtBillID.Text),  new DataParameter("@PalletCode", this.txtPalletCode.Text) };
                list.Add("WCS.DeleteCheckDetail");
                paras.Add(para);
                
                //增加盘点明细
                drs = dtPallet.Select("", "RowID");
                for (int i = 0; i < drs.Length; i++)
                {
                    para = new DataParameter[] {   
                                                 new DataParameter("@BillID", this.txtBillID.Text),                         
                                                 new DataParameter("@PalletCode", this.txtPalletCode.Text),
                                                 new DataParameter("@RowID",i+1),
                                                 new DataParameter("@CellCode", this.txtCellCode.Text),
                                                 new DataParameter("@TaskNo", this.txtTaskNo.Text),

                                                 new DataParameter("@ProductCode",drs[i]["ProductCode"].ToString()),
                                                 new DataParameter("@Quantity",drs[i]["Quantity"]),
                                                 new DataParameter("@RealQty",drs[i]["RealQty"]),
                                                 new DataParameter("@DiffQty", (int)drs[i]["RealQty"]-(int)drs[i]["Quantity"])
                    };
                    list.Add("WCS.InsertCheckDetail");
                    paras.Add(para);
                }

                //现有托盘与实际托盘不符
                if (PalletCode.ToLower() != this.txtPalletCode.Text.Trim().ToLower())
                {
                    //更新任务，托盘号与实际托盘不符
                    para = new DataParameter[] {   
                                                  new DataParameter("@BarCode", "现有托盘号："+this.txtPalletCode.Text.Trim()), 
                                                  new DataParameter("@TaskNo", this.txtTaskNo.Text)
                                                 };
                    list.Add("WCS.UpdateCheckWCSTaskError");
                    paras.Add(para);


                    para = new DataParameter[] {   
                                                  new DataParameter("@PalletBarCode", this.txtPalletCode.Text.Trim()), 
                                                  new DataParameter("@CellCode", this.txtCellCode.Text)
                                                 };
                    list.Add("WCS.UpdateCheckCellError");
                    paras.Add(para);

                    //更新货位托盘号
                }

                bll.ExecTran(list.ToArray(), paras);

            }
            catch (Exception ex)
            {
                MCP.Logger.Error(ex.Message);
                return;
            }

            blnOk = true;
            strValue = "1";

            this.DialogResult = DialogResult.OK;
        }

        private void frmScan_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!blnOk)
            {
                MCP.Logger.Info("请点击确定按钮，关闭窗口!");
                e.Cancel = true;
            }
        }

        private void txtPalletCode_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.KeyCode == Keys.Enter)
            {
                if (this.txtPalletCode.Text.Trim().Length > 0)
                {
                    dtPallet = bll.FillDataTable("WCS.SelectCheckPalletDetail", new DataParameter[] { new DataParameter("@PalletCode", this.txtPalletCode.Text) });
                    this.bsMain.DataSource = dtPallet;
                }
            }
        }


         
    }
}
