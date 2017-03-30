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
    public partial class Main : App.View.BaseForm
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
                FormDialog.OnDialog += new DialogEventHandler(FormDialog_OnDialog);
                context = new Context();

                ContextInitialize initialize = new ContextInitialize();
                initialize.InitializeContext(context);

                View.frmMonitor f = new View.frmMonitor();
                ShowForm(f);
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
        string FormDialog_OnDialog(DialogEventArgs args)
        {

            string strValue = "";
            if (InvokeRequired)
            {
                return (string)this.Invoke(new DialogEventHandler(FormDialog_OnDialog), args);
            }
            else
            {
                if (args.Message[0] == "1")//出库
                {
                    View.Dispatcher.frmOutView frm = new View.Dispatcher.frmOutView(int.Parse(args.Message[0]), args.dtInfo, context);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        strValue = frm.strValue;
                        //bool tt = context.ProcessDispatcher.WriteToService("CarPLC2", "PickFinished", 1);
                    }
                }
                if (args.Message[0] == "2")//盘点
                {
                    View.Dispatcher.frmScan frm = new View.Dispatcher.frmScan(int.Parse(args.Message[0]), args.dtInfo);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        strValue = frm.strValue;

                    }
                }
            }
            return strValue;
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
            if (BLL.Server.GetAreaCode() == "001")
            {
                this.toolStripButton1.Visible = true;
                this.toolStripButton2.Visible = false;
                this.toolStripButton3.Visible = false;
                toolStripButton_Scan.Visible = true;
            }
            else
            {
                this.toolStripButton1.Visible = false;
                this.toolStripButton2.Visible = false;
                this.toolStripButton3.Visible = false;
                toolStripButton_Scan.Visible = false;
            }

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

            if (this.toolStripButton_StartCrane.Text == "联机自动")
            {
                if (BLL.Server.GetAreaCode() == "001")
                    context.ProcessDispatcher.WriteToProcess("CraneProcess", "Run", 1);
                else
                {
                    context.ProcessDispatcher.WriteToProcess("CarProcess", "Run", 1);
                    context.ProcessDispatcher.WriteToProcess("MiniLoadProcess", "Run", 1);
                }
                this.toolStripButton_StartCrane.Image = App.Properties.Resources.process_accept;
                this.toolStripButton_StartCrane.Text = "脱机";
            }
            else
            {
                if (BLL.Server.GetAreaCode() == "001")
                    context.ProcessDispatcher.WriteToProcess("CraneProcess", "Run", 0);
                else
                {
                    context.ProcessDispatcher.WriteToProcess("CarProcess", "Run", 0);
                    context.ProcessDispatcher.WriteToProcess("MiniLoadProcess", "Run", 0);
                }
                this.toolStripButton_StartCrane.Image = App.Properties.Resources.process_remove;
                this.toolStripButton_StartCrane.Text = "联机自动";
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
            string AreaCode = BLL.Server.GetAreaCode();
            string PalletCodeSub = "";
            if (AreaCode!="001")
            {
                 PalletCodeSub = "!=";
            }
            else
            {
                PalletCodeSub = " = ";
            }
            DataTable dt = bll.FillDataTable("WCS.SelectTask", new DataParameter[] { new DataParameter("{0}", string.Format("(WCS_TASK.State not in('7','9')) and SUBSTRING(palletCode,1,1){0} 'a'",PalletCodeSub)) });
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
                    if (TaskType == "11" || TaskType == "16")
                    {
                        this.ToolStripMenuItem11.Visible = true;
                        this.ToolStripMenuItem12.Visible = true;
                        this.ToolStripMenuItem13.Visible = true;
                        this.ToolStripMenuItem14.Visible = false;
                        this.ToolStripMenuItem15.Visible = false;
                        this.ToolStripMenuItem16.Visible = false;
                        this.ToolStripMenuItem17.Visible = true;
                        this.ToolStripMenuItem18.Visible = false;
                        this.ToolStripMenuItem19.Visible = true;
                    }
                    else if (TaskType == "12" || TaskType == "15")
                    {
                        this.ToolStripMenuItem11.Visible = false;
                        this.ToolStripMenuItem12.Visible = false;
                        this.ToolStripMenuItem13.Visible = false;
                        this.ToolStripMenuItem14.Visible = true;
                        this.ToolStripMenuItem15.Visible = true;
                        this.ToolStripMenuItem16.Visible = true;
                        this.ToolStripMenuItem17.Visible = true;
                        this.ToolStripMenuItem18.Visible = false;
                        this.ToolStripMenuItem19.Visible = true;
                    }
                    else if (TaskType == "13")
                    {
                        this.ToolStripMenuItem11.Visible = false;
                        this.ToolStripMenuItem12.Visible = false;
                        this.ToolStripMenuItem13.Visible = false;
                        this.ToolStripMenuItem14.Visible = true;
                        this.ToolStripMenuItem15.Visible = false;
                        this.ToolStripMenuItem16.Visible = false;
                        this.ToolStripMenuItem17.Visible = true;
                        this.ToolStripMenuItem18.Visible = false;
                        this.ToolStripMenuItem19.Visible = true;
                    }
                    else if (TaskType == "14")
                    {
                        this.ToolStripMenuItem10.Visible = true;
                        this.ToolStripMenuItem11.Visible = true;
                        this.ToolStripMenuItem12.Visible = true;
                        this.ToolStripMenuItem13.Visible = true;
                        this.ToolStripMenuItem14.Visible = true;
                        this.ToolStripMenuItem15.Visible = true;
                        this.ToolStripMenuItem16.Visible = true;
                        this.ToolStripMenuItem17.Visible = true;
                        this.ToolStripMenuItem18.Visible = true;
                        this.ToolStripMenuItem19.Visible = true;
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

                if (TaskType == "11" || TaskType == "16")
                {
                    DataGridViewSelectedRowCollection rowColl = dgvMain.SelectedRows;
                    if (rowColl == null)
                        return;
                    DataRow dr = (rowColl[0].DataBoundItem as DataRowView).Row;
                    App.View.frmReassignEmptyCell f = new App.View.frmReassignEmptyCell(dr);
                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        this.BindData();
                }
                else if (TaskType == "12" || TaskType == "14" || TaskType == "15")
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
                DataRowView drv = dgvMain.SelectedRows[0].DataBoundItem as DataRowView;
                DataRow dr = drv.Row;
                string State = dr["State"].ToString(); ;
                string TaskNo = dr["TaskNo"].ToString();

                string fromStation = dr["FromStation"].ToString();
                string toStation = dr["ToStation"].ToString();

                if (fromStation.Trim() == "" || toStation.Trim() == "")
                {
                    Logger.Info(TaskNo + "目标位置或者起始位置错误,无法重新下达任务！");
                    return;
                }
                if (State == "3" || State == "4")
                    Send2PLC(dr);
                else
                {
                    Logger.Info("非正在上下架的任务无法重新下发");
                    return;
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
            string CraneNo = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colCraneNo"].Value.ToString();
            string CarNo = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colCarNo"].Value.ToString();

            string serviceName = "";
            int[] cellAddr = new int[9];
            if (CraneNo == "01")
            {
                serviceName = "CranePLC1";
            }
            else if (CraneNo == "02")
            {
                serviceName = "MiniLoad02";
                cellAddr = new int[19];
            }
            else if (CraneNo.Trim().Length <= 0)
            {
                cellAddr = new int[10];
                if (CarNo.Length > 0)
                    serviceName = "CarPLC01" + CarNo;
            }

            cellAddr[0] = 0;
            cellAddr[1] = 1;

            context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
            context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished", 1);

            MCP.Logger.Info("已下发取消任务指令");
        }


        #endregion

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            View.Task.frmCarTask f = new View.Task.frmCarTask();
            ShowForm(f);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            View.Task.frmMiniLoadTask f = new View.Task.frmMiniLoadTask();
            ShowForm(f);
        }

        private void toolStripButton_Barcode_Click(object sender, EventArgs e)
        {
            View.Base.frmBarcode f = new View.Base.frmBarcode();
            ShowForm(f);
        }
        private int getTaskType(string TaskType, string State)
        {
            int taskType = 10;
            if (TaskType == "11" || TaskType == "16")
                taskType = 10;
            else if (TaskType == "12" || TaskType == "15")
                taskType = 11;
            else if (TaskType == "13")
                taskType = 9;
            else if (TaskType == "14" && State == "4")
                taskType = 11;
            else if (TaskType == "14" && State == "3")
                taskType = 10;
            return taskType;
        }
        private void Send2PLC(DataRow dr)
        {
            string AreaCode = dr["AreaCode"].ToString();
            if (AreaCode == "001")
                Send2PLC1(dr);
            else if (AreaCode == "002")
                Send2PLC2(dr);
            else if (AreaCode == "003")
                Send2PLC3(dr);
        }
        private void Send2PLC1(DataRow dr)
        {
            string serviceName = "CranePLC1";
            string TaskNo = dr["TaskNo"].ToString();
           
            string fromStation = dr["FromStation"].ToString();
            string toStation = dr["ToStation"].ToString();
           
            int[] cellAddr = new int[9];

            cellAddr[0] = 0;
            cellAddr[1] = 0;
            cellAddr[2] = 0;

            cellAddr[3] = byte.Parse(fromStation.Substring(0, 3));
            cellAddr[4] = byte.Parse(fromStation.Substring(3, 3));
            cellAddr[5] = byte.Parse(fromStation.Substring(6, 3));
            cellAddr[6] = byte.Parse(toStation.Substring(0, 3));
            cellAddr[7] = byte.Parse(toStation.Substring(3, 3));
            cellAddr[8] = byte.Parse(toStation.Substring(6, 3));
            sbyte[] taskNo = new sbyte[10];
            Util.ConvertStringChar.stringToBytes(dr["TaskNo"].ToString(), 10).CopyTo(taskNo, 0);
            context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);

            context.ProcessDispatcher.WriteToService(serviceName, "TaskNo", taskNo);
            context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished", 1);
          
            //Logger.Info("任务:" + dr["TaskNo"].ToString() + "已下发给" + carNo + "穿梭车;起始地址:" + fromStation + ",目标地址:" + toStation);
        }
        private void Send2PLC2(DataRow dr)
        {
            string CarNo = dr["CarNo"].ToString();
            string serviceName = "CarPLC01" + CarNo;
            string TaskNo = dr["TaskNo"].ToString();
            int taskType = getTaskType(dr["TaskType"].ToString(), dr["State"].ToString());

            string fromStation = dr["FromStation"].ToString();
            string toStation = dr["ToStation"].ToString();

            int[] cellAddr = new int[10];

            cellAddr[0] = 0;
            cellAddr[1] = 0;
            cellAddr[2] = 0;

            cellAddr[3] = byte.Parse(fromStation.Substring(0, 3));
            cellAddr[4] = byte.Parse(fromStation.Substring(3, 3));
            cellAddr[5] = byte.Parse(fromStation.Substring(6, 3));
            cellAddr[6] = byte.Parse(toStation.Substring(0, 3));
            cellAddr[7] = byte.Parse(toStation.Substring(3, 3));
            cellAddr[8] = byte.Parse(toStation.Substring(6, 3));
            cellAddr[9] = taskType;

            int taskNo = int.Parse(TaskNo);

            context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
            context.ProcessDispatcher.WriteToService(serviceName, "TaskNo", taskNo);
            context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished", 2);

            Logger.Info("任务:" + dr["TaskNo"].ToString() + "已下发给" + CarNo + "穿梭车;起始地址:" + fromStation + ",目标地址:" + toStation);
        }
        private void Send2PLC3(DataRow dr)
        {
            string serviceName = "MiniLoad02";
            string TaskNo = dr["TaskNo"].ToString();
            string TaskType = dr["TaskType"].ToString();
            string state = dr["State"].ToString();
            int taskType = 10;

            if (state == "0")
            {
                if (TaskType == "13")
                {
                    taskType = 9;
                }
                else
                {
                    taskType = 11;
                }
            }

            string fromStation = dr["FromStation"].ToString();
            string toStation = dr["ToStation"].ToString();

            int[] cellAddr = new int[10];

            cellAddr[0] = 0;
            cellAddr[1] = 0;
            cellAddr[2] = 0;

            cellAddr[3] = byte.Parse(fromStation.Substring(0, 3));
            cellAddr[4] = byte.Parse(fromStation.Substring(3, 3));
            cellAddr[5] = byte.Parse(fromStation.Substring(6, 3));
            cellAddr[6] = byte.Parse(toStation.Substring(0, 3));
            cellAddr[7] = byte.Parse(toStation.Substring(3, 3));
            cellAddr[8] = byte.Parse(toStation.Substring(6, 3));
            cellAddr[9] = taskType;

            int taskNo = int.Parse(TaskNo);

            context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
            context.ProcessDispatcher.WriteToService(serviceName, "TaskNo", taskNo);
            context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished", 2);

            //Logger.Info("任务:" + dr["TaskNo"].ToString() + "已下发给" + carNo + "穿梭车;起始地址:" + fromStation + ",目标地址:" + toStation);
        }


    }
}
