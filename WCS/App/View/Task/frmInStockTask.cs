using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Util;

namespace App.View.Task
{
    public partial class frmInStockTask : Form
    {
        BLL.BLLBase bll = new BLL.BLLBase();
        string CraneNo = "01";
        string AreaCode = "001";
        string TaskType = "";
        public frmInStockTask()
        {
            InitializeComponent();
        }

        private void frmInStockTask_Load(object sender, EventArgs e)
        {

            DataTable dt = bll.FillDataTable("CMD.SelectArea");
            this.cmbAreaCode.DataSource = dt.DefaultView;
            this.cmbAreaCode.ValueMember = "AreaCode";
            this.cmbAreaCode.DisplayMember = "AreaName";
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                this.cbRow.Enabled = false;
                this.cbColumn.Enabled = false;
                this.cbHeight.Enabled = false;
                this.cmbDepth.Enabled = false;
            }
            else
            {
                this.cbRow.Enabled = true;
                this.cbColumn.Enabled = true;
                this.cbHeight.Enabled = true;
                this.cmbDepth.Enabled = true;
            }
        }

        private void cmbAreaCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataParameter[] param = new DataParameter[] 
            { 
                new DataParameter("{0}", string.Format("AreaCode='{0}'", this.cmbAreaCode.SelectedValue))
            };
            DataTable dt = bll.FillDataTable("CMD.SelectCellShelf", param);
            this.cbRow.DataSource = dt.DefaultView;
            this.cbRow.ValueMember = "shelfcode";
            this.cbRow.DisplayMember = "shelfcode";

            DataTable dtStation = new DataTable("dtStation");
            dtStation.Columns.Add("dtText");
            dtStation.Columns.Add("dtValue");
            DataRow dr = dtStation.NewRow();

            dr["dtText"] = "01";
            dr["dtValue"] = "01";
            dtStation.Rows.Add(dr);

            if (this.cmbAreaCode.SelectedValue.ToString() == "003")
            {
                dr = dtStation.NewRow();

                dr["dtText"] = "03";
                dr["dtValue"] = "03";
                dtStation.Rows.Add(dr);
            }
            this.cbStationNo.DataSource = dtStation;
            this.cbStationNo.DisplayMember = "dtText";
            this.cbStationNo.ValueMember = "dtValue";
        }

        private void cbRow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbRow.Text == "System.Data.DataRowView")
                return;

            DataParameter[] param = new DataParameter[] 
            { 
                new DataParameter("{0}", string.Format("ShelfCode='{0}'",this.cbRow.Text))
            };
            DataTable dt = bll.FillDataTable("CMD.SelectColumn", param);

            this.cbColumn.DataSource = dt.DefaultView;
            this.cbColumn.ValueMember = "CellColumn";
            this.cbColumn.DisplayMember = "CellColumn";

            DataTable dtDepth = bll.FillDataTable("CMD.SelectDepth", param);

            this.cmbDepth.DataSource = dtDepth.DefaultView;
            this.cmbDepth.ValueMember = "Depth";
            this.cmbDepth.DisplayMember = "Depth";



        }

        private void cbColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbRow.Text == "System.Data.DataRowView")
                return;
            if (this.cbColumn.Text == "System.Data.DataRowView")
                return;

            DataParameter[] param = new DataParameter[] 
            { 
                new DataParameter("{0}", string.Format("ShelfCode='{0}' and CellColumn={1}",this.cbRow.Text,this.cbColumn.Text))
            };
            DataTable dt = bll.FillDataTable("CMD.SelectCell", param);
            DataView dv = dt.DefaultView;
            dv.Sort = "CellRow";
            this.cbHeight.DataSource = dv;
            this.cbHeight.ValueMember = "CellRow";
            this.cbHeight.DisplayMember = "CellRow";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            if (this.txtBarcode.Text.Trim().Length <= 0)
            {
                MessageBox.Show("托盘条码/箱条码不能为空,请扫码或输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtBarcode.Focus();
                return;
            }
            if (this.txtTaskNo.Text.Length <= 0)
            {
                MessageBox.Show("此托盘条码/箱条码找不到对应的等待状态的入库任务,请确认！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtBarcode.SelectAll();
                this.txtBarcode.Focus();
                return;
            }

            if (TaskType == "11")
            {
                DataTable dt;
                DataParameter[] param;

                param = new DataParameter[] 
                {
                    new DataParameter("@AreaCode", this.cmbAreaCode.SelectedValue.ToString())                
                };
                if (this.radioButton1.Checked)
                {
                    dt = bll.FillDataTable("WCS.sp_GetCell", param);
                    if (dt.Rows.Count > 0)
                        this.txtCellCode.Text = dt.Rows[0][0].ToString();
                    else
                        this.txtCellCode.Text = "";
                }
                else
                {
                    this.txtCellCode.Text = this.cbRow.Text.Substring(3, 3) + (1000 + int.Parse(this.cbColumn.Text)).ToString().Substring(1, 3) + (1000 + int.Parse(this.cbHeight.Text)).ToString().Substring(1, 3) + this.cmbDepth.Text;
                }
                //判断货位是否为空
                param = new DataParameter[] 
                { 
                    new DataParameter("{0}", string.Format("CellCode='{0}' and PalletBarCode='' and IsActive='1' and IsLock='0' and AreaCode='{1}'", this.txtCellCode.Text,this.cmbAreaCode.SelectedValue.ToString()))
                };
                dt = bll.FillDataTable("CMD.SelectCell", param);
                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("自动获取的货位或指定的货位非空货位,请确认！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //锁定货位
                param = new DataParameter[] 
                {             
                    new DataParameter("@CellCode", this.txtCellCode.Text),                    
                    new DataParameter("@TaskNo", this.txtTaskNo.Text),
                    new DataParameter("@AreaCode", this.cmbAreaCode.SelectedValue.ToString())
                };
                bll.ExecNonQueryTran("WCS.Sp_ExecuteInStockTask", param);
            }
            else
            {
                bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 2), new DataParameter("@TaskNo", this.txtTaskNo.Text) });
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            //根据熔次卷号获取任务
            
        }

        private void frmInStockTask_Activated(object sender, EventArgs e)
        {
            this.txtBarcode.Focus();
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                DataTable dt = bll.FillDataTable("WCS.GetTaskByPallet", new DataParameter[] { new DataParameter("@PalletCode", this.txtBarcode.Text.Trim()) });
                if (dt.Rows.Count > 0)
                {
                    this.txtTaskNo.Text = dt.Rows[0]["TaskNo"].ToString();
                    TaskType = dt.Rows[0]["TaskType"].ToString();
                }
                else
                {
                    this.txtTaskNo.Text = "";
                }

            }

        }        
    }
}
