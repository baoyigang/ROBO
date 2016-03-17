using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCP;
using Util;
using DataGridViewAutoFilter;

namespace App
{
    public partial class Main : Form
    {
        private bool IsActiveForm = false;
        public bool IsActiveTab = false;
        private Context context = null;
        private System.Timers.Timer tmWorkTimer = new System.Timers.Timer();
        BLL.BLLBase bll = new BLL.BLLBase();

        public Main()
        {
            InitializeComponent();
        }

        private void inStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.View.Task.frmInStock f = new View.Task.frmInStock();
            ShowForm(f);
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            try
            {
                lbLog.Scrollable = true;
                Logger.OnLog += new LogEventHandler(Logger_OnLog);
                context = new Context();

                ContextInitialize initialize = new ContextInitialize();
                initialize.InitializeContext(context);

                //View.frmMonitor f = new View.frmMonitor();
                //ShowForm(f);
                MainData.OnTask += new TaskEventHandler(Data_OnTask);
                this.BindData();
                for (int i = 0; i < this.dgvMain.Columns.Count - 1; i++)
                    ((DataGridViewAutoFilterTextBoxColumn)this.dgvMain.Columns[i]).FilteringEnabled = true;

                tmWorkTimer.Interval = 3000;
                tmWorkTimer.Elapsed += new System.Timers.ElapsedEventHandler(tmWorker);
                tmWorkTimer.Start();

               
            }
            catch (Exception ee)
            {
                Logger.Error("初始化处理失败请检查配置，原因：" + ee.Message);
            }
        }
        private void tmWorker(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmWorkTimer.Stop();
                DataTable dt = GetMonitorData();
                MainData.TaskInfo(dt);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            finally
            {
                tmWorkTimer.Start();
            }
        }      
        void Logger_OnLog(MCP.LogEventArgs args)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new LogEventHandler(Logger_OnLog), args);
            }
            else
            {
                lock (lbLog)
                {
                    string msg1 = string.Format("[{0}]", args.LogLevel);
                    string msg2 = string.Format("{0}", DateTime.Now);
                    string msg3 = string.Format("{0} ", args.Message);
                    this.lbLog.BeginUpdate();
                    ListViewItem item = new ListViewItem(new string[] { msg1, msg2, msg3 });
                    
                    if (msg1.Contains("[ERROR]"))
                    {
                        //item.ForeColor = Color.Red;
                        item.BackColor = Color.Red;
                    }
                    lbLog.Items.Insert(0, item);
                    this.lbLog.EndUpdate();
                    WriteLoggerFile(msg1 + msg2 + msg3);
                }
            }
        }

        private void CreateDirectory(string directoryName)
        {
            if (!System.IO.Directory.Exists(directoryName))
                System.IO.Directory.CreateDirectory(directoryName);
        }

        private void WriteLoggerFile(string text)
        {
            try
            {
                string path = "";
                CreateDirectory("日志");
                path = "日志";
                path = path + @"/" + DateTime.Now.ToString().Substring(0, 4).Trim();
                CreateDirectory(path);
                path = path + @"/" + DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 7).Trim();
                path = path.TrimEnd(new char[] { '-' });
                CreateDirectory(path);
                path = path + @"/" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                System.IO.File.AppendAllText(path, string.Format("{0} {1}", DateTime.Now, text + "\r\n"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        #region 公共方法
        /// <summary>
        /// 打开一个窗体

        /// </summary>
        /// <param name="frm"></param>
        private void ShowForm(Form frm)
        {
            if (OpenOnce(frm))
            {
                frm.MdiParent = this;
                ((View.BaseForm)frm).Context = context;
                frm.Show();
                frm.WindowState = FormWindowState.Maximized;
                AddTabPage(frm.Handle.ToString(), frm.Text);
            }
        }
        /// <summary>
        /// 判断窗体是否已打开
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        private bool OpenOnce(Form frm)
        {
            foreach (Form mdifrm in this.MdiChildren)
            {
                int index = mdifrm.Text.IndexOf(" ");
                if (index > 0)
                {
                    if (frm.Name == mdifrm.Name && frm.Text == mdifrm.Text.Substring(0, index))
                    {
                        mdifrm.Activate();
                        return false;
                    }
                }
                else
                {
                    if (frm.Name == mdifrm.Name && frm.Text == mdifrm.Text)
                    {
                        mdifrm.Activate();
                        return false;
                    }
                }
            }
            return true;

        }
        
        private void AddTabPage(string strKey, string strText)
        {
            IsActiveForm = true;
            TabPage tab = new TabPage();
            tab.Name = strKey.ToString();
            tab.Text = strText;
            tabForm.TabPages.Add(tab);
            tabForm.SelectedTab = tab;
            this.pnlTab.Visible = true;
            IsActiveForm = false;
        }
        
        public void SetActiveTab(string strKey, bool blnActive)
        {
            foreach (TabPage tab in this.tabForm.TabPages)
            {
                if (tab.Name == strKey)
                {
                    IsActiveForm = true;

                    if (blnActive)
                        tabForm.SelectedTab = tab;
                    else
                    {
                        tabForm.TabPages.Remove(tab);
                        if (this.MdiChildren.Length > 1)
                            this.pnlTab.Visible = true;
                        else
                            this.pnlTab.Visible = false;
                    }

                    IsActiveForm = false; ;
                }
            }
        }
        private void tabForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsActiveForm)
                return;
            foreach (Form mdifrm in this.MdiChildren)
            {
                if (mdifrm.Handle.ToInt32() == int.Parse(((TabControl)sender).SelectedTab.Name))
                {
                    IsActiveTab = true;
                    mdifrm.Activate();
                    IsActiveTab = false;
                }
            }
        }
        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
           
                
        }

        private void ToolStripMenuItem_Cell_Click(object sender, EventArgs e)
        {
            App.View.Dispatcher.frmCellQuery f = new App.View.Dispatcher.frmCellQuery();
            ShowForm(f);
        }

        private void toolStripButton_Close_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("您确定要退出调度系统吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Logger.Info("退出系统");
                System.Environment.Exit(0);
            }
        }

        private void toolStripButton_InStockTask_Click(object sender, EventArgs e)
        {
            App.View.Task.frmInStock f = new View.Task.frmInStock();
            ShowForm(f);
        }

        private void toolStripButton_OutStock_Click(object sender, EventArgs e)
        {
            App.View.Task.frmOutStock f = new View.Task.frmOutStock();
            ShowForm(f);
        }

        private void OutStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.View.Task.frmOutStock f = new View.Task.frmOutStock();
            ShowForm(f);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("您确定要退出调度系统吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Logger.Info("退出系统");
                System.Environment.Exit(0);
            }
            else
                e.Cancel = true;
        }

        private void MoveStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.View.Task.frmMoveStock f = new View.Task.frmMoveStock();
            ShowForm(f);
        }

        private void toolStripButton_MoveStock_Click(object sender, EventArgs e)
        {
            App.View.Task.frmMoveStock f = new View.Task.frmMoveStock();
            ShowForm(f);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            View.Task.frmCraneTask f = new View.Task.frmCraneTask();
            ShowForm(f);
        }

        private void toolStripButton_StartCrane_Click(object sender, EventArgs e)
        {
            if (this.toolStripButton_StartCrane.Text == "堆垛机联机")
            {
                context.ProcessDispatcher.WriteToProcess("CraneProcess", "Run", 1);
                this.toolStripButton_StartCrane.Image = App.Properties.Resources.process_accept;
                this.toolStripButton_StartCrane.Text = "堆垛机脱机";
            }
            else
            {
                context.ProcessDispatcher.WriteToProcess("CraneProcess", "Run", 0);
                this.toolStripButton_StartCrane.Image = App.Properties.Resources.process_remove;
                this.toolStripButton_StartCrane.Text = "堆垛机联机";
            }            
        }

        private void toolStripButton_Inventor_Click(object sender, EventArgs e)
        {
            App.View.Task.frmInventor f = new View.Task.frmInventor();
            ShowForm(f);
        }

        private void InventortoolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.View.Task.frmInventor f = new View.Task.frmInventor();
            ShowForm(f);
        }

        private void toolStripButton_CellMonitor_Click(object sender, EventArgs e)
        {
            App.View.Dispatcher.frmCellQuery f = new App.View.Dispatcher.frmCellQuery();
            ShowForm(f);
        }

        private void ToolStripMenuItem_Param_Click(object sender, EventArgs e)
        {
            App.View.Param.ParameterForm f = new App.View.Param.ParameterForm();
            ShowForm(f);
        }

        private void toolStripButton_Scan_Click(object sender, EventArgs e)
        {
            App.View.Task.frmInStockTask f = new App.View.Task.frmInStockTask();
            f.ShowDialog();
        }

        #region 正执行任务处理
        void Data_OnTask(TaskEventArgs args)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new TaskEventHandler(Data_OnTask), args);
            }
            else
            {
                lock (this.dgvMain)
                {
                    DataTable dt = args.datatTable;
                    this.bsMain.DataSource = dt;
                    for (int i = 0; i < this.dgvMain.Rows.Count; i++)
                    {
                        if (dgvMain.Rows[i].Cells["colErrCode"].Value.ToString() != "0")
                            this.dgvMain.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        else
                        {
                            if (i % 2 == 0)
                                this.dgvMain.Rows[i].DefaultCellStyle.BackColor = Color.White;
                            else
                                this.dgvMain.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(192, 255, 192);

                        }
                    }
                }
            }
        }


        private void BindData()
        {
            bsMain.DataSource = GetMonitorData();
        }
        private DataTable GetMonitorData()
        {
            DataTable dt = bll.FillDataTable("WCS.SelectTask", new DataParameter[] { new DataParameter("{0}", "(WCS_TASK.TaskType='11' and WCS_TASK.State in('1','2','3')) OR (WCS_TASK.TaskType in('12','13','15') and WCS_TASK.State in('0','2','3')) OR (WCS_TASK.TaskType in('14') and WCS_TASK.State in('0','2','3','4','5','6'))") });
            return dt;
        }

        private void dgvMain_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
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
                    string TaskType = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colTaskType"].Value.ToString();
                    if (TaskType == "11")
                    {
                        this.ToolStripMenuItem11.Visible = true;
                        this.ToolStripMenuItem12.Visible = false;
                        this.ToolStripMenuItem13.Visible = true;
                        this.ToolStripMenuItem14.Visible = false;
                        this.ToolStripMenuItem15.Visible = false;
                        this.ToolStripMenuItem16.Visible = false;
                    }
                    else if (TaskType == "12" || TaskType == "13")
                    {
                        this.ToolStripMenuItem11.Visible = false;
                        this.ToolStripMenuItem12.Visible = false;
                        this.ToolStripMenuItem13.Visible = true;
                        this.ToolStripMenuItem14.Visible = false;
                        this.ToolStripMenuItem15.Visible = false;
                        this.ToolStripMenuItem16.Visible = false;
                    }
                    else if (TaskType == "14")
                    {
                        this.ToolStripMenuItem10.Visible = true;
                        this.ToolStripMenuItem11.Visible = false;
                        this.ToolStripMenuItem12.Visible = false;
                        this.ToolStripMenuItem13.Visible = true;
                        this.ToolStripMenuItem14.Visible = false;
                        this.ToolStripMenuItem15.Visible = true;
                        this.ToolStripMenuItem16.Visible = true;
                    }
                    //弹出操作菜单
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void ToolStripMenuItemCellCode_Click(object sender, EventArgs e)
        {
            if (this.dgvMain.CurrentCell != null)
            {
                BLL.BLLBase bll = new BLL.BLLBase();
                string TaskNo = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells[0].Value.ToString();
                string TaskType = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colTaskType"].Value.ToString();
                string ErrCode = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colErrCode"].ToString();

                if (TaskType == "11")
                {
                    DataGridViewSelectedRowCollection rowColl = dgvMain.SelectedRows;
                    if (rowColl == null)
                        return;
                    DataRow dr = (rowColl[0].DataBoundItem as DataRowView).Row;
                    App.View.frmReassignEmptyCell f = new App.View.frmReassignEmptyCell(dr);
                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        this.BindData();
                }
                else if (TaskType == "12" || TaskType == "14")
                {
                    DataGridViewSelectedRowCollection rowColl = dgvMain.SelectedRows;
                    if (rowColl == null)
                        return;
                    DataRow dr = (rowColl[0].DataBoundItem as DataRowView).Row;
                    App.View.frmReassign f = new App.View.frmReassign(dr);
                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        this.BindData();
                }
                else if (TaskType == "13")
                {
                    DataGridViewSelectedRowCollection rowColl = dgvMain.SelectedRows;
                    if (rowColl == null)
                        return;
                    DataRow dr = (rowColl[0].DataBoundItem as DataRowView).Row;

                    App.View.frmReassignOption fo = new App.View.frmReassignOption();
                    if (fo.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (fo.option == 0)
                        {
                            App.View.frmReassign f = new App.View.frmReassign(dr);
                            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                this.BindData();
                        }
                        else
                        {
                            App.View.frmReassignEmptyCell fe = new App.View.frmReassignEmptyCell(dr);
                            if (fe.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                this.BindData();
                        }
                    }
                }
            }
        }

        private void ToolStripMenuItemReassign_Click(object sender, EventArgs e)
        {
            if (this.dgvMain.CurrentCell != null)
            {
                BLL.BLLBase bll = new BLL.BLLBase();
                string TaskNo = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells[0].Value.ToString();
                string TaskType = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colTaskType"].Value.ToString();

                if (TaskType == "11")
                    bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 1), new DataParameter("@TaskNo", TaskNo) });
                else if (TaskType == "12" || TaskType == "13")
                    bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 0), new DataParameter("@TaskNo", TaskNo) });
                else if (TaskType == "14")
                {
                    App.View.frmTaskOption f = new App.View.frmTaskOption();
                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (f.option == 0)
                            bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 1), new DataParameter("@TaskNo", TaskNo) });
                        else
                            bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 5), new DataParameter("@TaskNo", TaskNo) });

                    }
                }
                this.BindData();
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ItemName = ((ToolStripMenuItem)sender).Name;
            string State = ItemName.Substring(ItemName.Length - 1, 1);

            if (this.dgvMain.CurrentCell != null)
            {
                BLL.BLLBase bll = new BLL.BLLBase();
                string TaskNo = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells[0].Value.ToString();

                DataParameter[] param = new DataParameter[] { new DataParameter("@TaskNo", TaskNo), new DataParameter("@State", State) };
                bll.ExecNonQueryTran("WCS.Sp_UpdateTaskState", param);

                //bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", State), new DataParameter("@TaskNo", TaskNo) });

                ////堆垛机完成执行
                //if (State == "7")
                //{
                //    DataParameter[] param = new DataParameter[] { new DataParameter("@TaskNo", TaskNo) };
                //    bll.ExecNonQueryTran("WCS.Sp_TaskProcess", param);
                //}
                BindData();
            }
        }

        private void ToolStripMenuItemDelCraneTask_Click(object sender, EventArgs e)
        {
            string serviceName = "CranePLC1";
            int[] cellAddr = new int[9];
            cellAddr[0] = 0;
            cellAddr[1] = 1;

            context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
            context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished", 0);

            MCP.Logger.Info("已给堆垛机下发取消任务指令");
        }        


        #endregion

       




         
    }
}
