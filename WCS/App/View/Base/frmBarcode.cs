using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Util;
using DataGridViewAutoFilter;

namespace App.View.Base
{
    public partial class frmBarcode : BaseForm
    {
        BLL.BLLBase bll = new BLL.BLLBase();

        public frmBarcode()
        {
            InitializeComponent();
        }

        private void toolStripButton_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void BindData()
        {
            DataTable dt = bll.FillDataTable("CMD.SelectPallet", new DataParameter[] { new DataParameter("{0}", "AreaCode='002'") });
            bsMain.DataSource = dt;
        }

        private void frmBarcode_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dgvMain.Columns.Count; i++)
                ((DataGridViewAutoFilterTextBoxColumn)this.dgvMain.Columns[i]).FilteringEnabled = true;
        }

        private void dgvMain_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 5)
                {
                    //若行已是选中状态就不再进行设置
                    if (dgvMain.Rows[e.RowIndex].Selected == false)
                    {
                        dgvMain.ClearSelection();
                        dgvMain.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    if (dgvMain.SelectedRows.Count == 1)
                    {
                        dgvMain.CurrentCell = dgvMain.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }
                    //弹出操作菜单
                }
            }
            //if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            //{
            //    var dataSelect = dgvMain.SelectedRows;
            //    foreach (DataGridViewRow row in dataSelect)
            //    {
            //        if (row.Cells[5].Value == "1")
            //        {
            //            row.Cells[5].Value = "0";
            //        }
            //        else
            //        {
            //            row.Cells[5].Value = "1";
            //        }
            //    }
            //}
        }
        

        private void frmInStock_Activated(object sender, EventArgs e)
        {
            this.BindData();
        }

        private void toolStripButton_Refresh_Click(object sender, EventArgs e)
        {
            this.BindData();
        }

        private void toolStripButton_Print_Click(object sender, EventArgs e)
        {
            try
            {

                // printDocument1 为 打印控件  
                //设置打印用的纸张 当设置为Custom的时候，可以自定义纸张的大小，还可以选择A4,A5等常用纸型 
                PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = new StandardPrintController();

                DataTable dt = new DataTable();

                // 列强制转换  
                for (int count = 0; count < dgvMain.Columns.Count; count++)
                {
                    DataColumn dc = new DataColumn(dgvMain.Columns[count].Name.ToString());
                    dt.Columns.Add(dc);
                }

                // 循环行  
                for (int count = 0; count < dgvMain.Rows.Count; count++)
                {
                    DataRow dr = dt.NewRow();
                    for (int countsub = 0; countsub < dgvMain.Columns.Count; countsub++)
                    {
                        dr[countsub] = Convert.ToString(dgvMain.Rows[count].Cells[countsub].Value);
                    }
                    dt.Rows.Add(dr);
                }
                drs = dt.Select("Select='1'");
                PageCount = drs.Length;
                foreach (PaperSize ps in pd.PrinterSettings.PaperSizes)//查找当前设置纸张
                {
                    if ("ROBO" == ps.PaperName)
                    {
                        pd.DefaultPageSettings.PaperSize = ps;
                        break;
                    }
                }
                //pd.DefaultPageSettings.PaperSize = new PaperSize("Custum", 80, 40);
                pd.PrintPage += new PrintPageEventHandler(MyPrintDocument_PrintPage);
                //将写好的格式给打印预览控件以便预览  
                printPreviewDialog.Document = pd;
                printPreviewDialog.ShowDialog();
                //显示打印预览  
                //DialogResult result = printPreviewDialog1.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }
        DataRow[] drs;
        int PageCount = 2;
        int currentPage = 0;
        /// <summary>  
  /// 打印的格式  
  /// </summary>  
  /// <param name="sender"></param>  
  /// <param name="e"></param>  
        private void MyPrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            Margins margins = new Margins(5, 5, 5, 5);
            e.PageSettings.Margins = margins;

            Graphics g = e.Graphics;


            string Barcode = drs[currentPage]["Column4"].ToString();
            //e.Graphics.DrawString(Barcode, new Font(new FontFamily("黑体"), 8), System.Drawing.Brushes.Black, 9, 35);

            barcodeControl1.Data = Barcode;
            barcodeControl1.ShowCode39StartStop = false;
            //barcodeControl1.Draw(g, barcodeControl1.ClientRectangle, GraphicsUnit.Inch, 0.01f, 0, null);
            barcodeControl1.Draw(g, new Rectangle(10, 0, 300, 150), GraphicsUnit.Inch, 0.01f, 0, null);

            currentPage++;
            if (currentPage < PageCount)
                e.HasMorePages = true;
            else
            {
                e.HasMorePages = false;
                currentPage = 0;
            }

            g.Dispose();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked==true)
            {
                foreach (DataGridViewRow row in dgvMain.Rows)
                {
                    ((DataGridViewCheckBoxCell)row.Cells[5]).Value = "1";
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvMain.Rows)
                {
                    ((DataGridViewCheckBoxCell)row.Cells[5]).Value = "0";
                }
            }
        }


        private void dgvMain_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                var dataSelect = dgvMain.SelectedRows;
                foreach (DataGridViewRow row in dataSelect)
                {
                    if (row.Cells[5].Value == "1")
                    {
                        row.Cells[5].Value = "0";
                    }
                    else
                    {
                        row.Cells[5].Value = "1";
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string SBarcode = textBox1.Text;

                dgvMain.FirstDisplayedScrollingRowIndex = int.Parse(SBarcode.Substring(1)) - 1;
            }
            catch (Exception)
            {

            }
        }




    }
}
