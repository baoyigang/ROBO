using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Util;
using DataGridViewAutoFilter;
using MCP;

namespace App.View
{
    public partial class frmMonitor : BaseForm
    {
        private Point InitialP1;
        private Point InitialP2;

        float colDis = 20.75f;
        float rowDis = 54.4f;
        
       // private System.Timers.Timer tmWorkTimer = new System.Timers.Timer();
        private System.Timers.Timer tmCrane1 = new System.Timers.Timer();
        BLL.BLLBase bll = new BLL.BLLBase();
        Dictionary<int, string> dicCraneFork = new Dictionary<int, string>();
        Dictionary<int, string> dicCraneState = new Dictionary<int, string>();
        Dictionary<int, string> dicCraneAction = new Dictionary<int, string>();
        
        Dictionary<int, string> dicCraneOver = new Dictionary<int, string>();
        DataTable dtCraneErr;

        public frmMonitor()
        {
            InitializeComponent();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Point P2 = picCrane.Location;
            P2.X = P2.X - 90;

            this.picCrane.Location = P2;
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
           
            Cranes.OnCrane += new CraneEventHandler(Monitor_OnCrane);
            AddDicKeyValue();

            InitialP1 = picCrane.Location;
            picCrane.Parent = pictureBox1;
            picCrane.BackColor = Color.Transparent;
            
            InitialP2 = picCar.Location;
            picCar.Parent = pictureBox1;
            picCar.BackColor = Color.Transparent;

            //this.BindData();
            //for (int i = 0; i < this.dgvMain.Columns.Count - 1; i++)
            //    ((DataGridViewAutoFilterTextBoxColumn)this.dgvMain.Columns[i]).FilteringEnabled = true;

            //tmWorkTimer.Interval = 3000;
            //tmWorkTimer.Elapsed += new System.Timers.ElapsedEventHandler(tmWorker);
            //tmWorkTimer.Start();

            tmCrane1.Interval = 3000;
            tmCrane1.Elapsed += new System.Timers.ElapsedEventHandler(tmCraneWorker1);
            tmCrane1.Start();
        }
        private void AddDicKeyValue()
        {
            dicCraneFork.Add(0, "非原点");
            dicCraneFork.Add(1, "原点");

            dicCraneState.Add(0, "空闲");
            dicCraneState.Add(1, "等待");
            dicCraneState.Add(2, "定位");
            dicCraneState.Add(3, "取货");
            dicCraneState.Add(4, "放货");
            dicCraneState.Add(98, "维修");

            dicCraneAction.Add(0, "非自动");
            dicCraneAction.Add(1, "自动");


            dtCraneErr = bll.FillDataTable("WCS.SelectCraneError");

        }
      
        void Monitor_OnCrane(CraneEventArgs args)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CraneEventHandler(Monitor_OnCrane), args);
            }
            else
            {
                Crane crane = args.crane;
                TextBox txt = GetTextBox("txtTaskNo", crane.CraneNo);
                if (txt != null)
                    txt.Text = crane.TaskNo;

                txt = GetTextBox("txtState", crane.CraneNo);
                if (txt != null && dicCraneState.ContainsKey(crane.TaskType))
                    txt.Text = dicCraneState[crane.TaskType];

                txt = GetTextBox("txtCraneAction", crane.CraneNo);
                if (txt != null && dicCraneAction.ContainsKey(crane.Action))
                    txt.Text = dicCraneAction[crane.Action];

                txt = GetTextBox("txtRow", crane.CraneNo);
                if (txt != null)
                    txt.Text = crane.Row.ToString();

                txt = GetTextBox("txtColumn", crane.CraneNo);
                if (txt != null)
                    txt.Text = crane.Column.ToString();

                //堆垛机位置
                if (crane.CraneNo == 1)
                {
                    this.picCrane.Visible = true;
                    Point P1 = InitialP1;
                    if(crane.Column<46)
                        P1.X = P1.X + (int)((crane.Column-1) * colDis);
                    else
                        P1.X = picCar.Location.X+15;

                    P1.Y = P1.Y + (int)(rowDis * (crane.Row - 1));
                    this.picCrane.Location = P1;

                    Point P2 = InitialP2;
                    P2.Y = P2.Y + (int)(rowDis * (crane.Row - 1));
                    this.picCar.Location = P2;
                }                

                txt = GetTextBox("txtHeight", crane.CraneNo);
                if (txt != null)
                    txt.Text = crane.Height.ToString();

                txt = GetTextBox("txtForkStatus", crane.CraneNo);
                if (txt != null && dicCraneFork.ContainsKey(crane.ForkStatus))
                    txt.Text = dicCraneFork[crane.ForkStatus];
                txt = GetTextBox("txtErrorNo", crane.CraneNo);
                if (txt != null)
                    txt.Text = crane.ErrCode.ToString();

                string strError = "";
                txt = GetTextBox("txtErrorDesc", crane.CraneNo);
                if (txt != null)
                {
                    if (crane.ErrCode != 0)
                    {
                        DataRow[] drs = dtCraneErr.Select(string.Format("CraneErrCode={0}", crane.ErrCode));
                        if (drs.Length > 0)
                            strError = drs[0]["CraneErrDesc"].ToString();
                        else
                            strError = "堆垛机未知错误！";
                    }
                    else
                    {
                        strError = "";
                    }
                    txt.Text = strError;
                }

                //更新错误代码、错误描述
                //更新任务状态为执行中
                //bll.ExecNonQuery("WCS.UpdateTaskError", new DataParameter[] { new DataParameter("@CraneErrCode", crane.ErrCode.ToString()), new DataParameter("@CraneErrDesc", dicCraneError[crane.ErrCode]), new DataParameter("@TaskNo", crane.TaskNo) });
                if (crane.ErrCode > 0)
                {
                    DataParameter[] param = new DataParameter[] { new DataParameter("@TaskNo", crane.TaskNo), new DataParameter("@CraneErrCode", crane.ErrCode.ToString()), new DataParameter("@CraneErrDesc", strError) };
                    bll.ExecNonQueryTran("WCS.Sp_UpdateTaskError", param);
                    Logger.Error(crane.CraneNo.ToString() + "堆垛机执行时出现错误,代码:" + crane.ErrCode.ToString() + ",描述:" + strError);
                }
            }
        }
        TextBox GetTextBox(string name, int CraneNo)
        {
            Control[] ctl = this.Controls.Find(name + CraneNo.ToString(), true);
            if (ctl.Length > 0)
                return (TextBox)ctl[0];
            else
                return null;
        }
        
      
        private void tmCraneWorker1(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmCrane1.Stop();
                string binary = Convert.ToString(255, 2).PadLeft(8, '0');

                string serviceName = "CranePLC1";
                string plcTaskNo = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "CraneTaskNo")));

                string craneMode = ObjectUtil.GetObject(Context.ProcessDispatcher.WriteToService(serviceName, "CraneMode")).ToString();
                string craneFork = ObjectUtil.GetObject(Context.ProcessDispatcher.WriteToService(serviceName, "CraneFork")).ToString();
                object[] obj = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "CraneAlarmCode"));

                Crane crane = new Crane();
                crane.CraneNo = 1;
                crane.Row = int.Parse(obj[4].ToString());
                crane.Column = int.Parse(obj[2].ToString());
                crane.Height = int.Parse(obj[3].ToString());
                crane.ForkStatus = int.Parse(craneFork);
                crane.Action = int.Parse(craneMode);
                crane.TaskType = int.Parse(obj[1].ToString());
                crane.ErrCode = int.Parse(obj[0].ToString());
                crane.PalletCode = "";
                crane.TaskNo = plcTaskNo;

                Cranes.CraneInfo(crane);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            finally
            {
                tmCrane1.Start();
            }
        }        
        
      
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (this.btnBack1.Text == "启动")
            {
                Context.ProcessDispatcher.WriteToProcess("CraneProcess", "Run", 1);
                this.btnBack1.Text = "停止";
            }
            else
            {
                Context.ProcessDispatcher.WriteToProcess("CraneProcess", "Run", 0);
                this.btnBack1.Text = "启动";
            }
        }

        private void btnBack1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要召回1号堆垛机到初始位置?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                PutCommand("1", 0);
                Logger.Info("1号堆垛机下发召回命令");
            }
        }
        private void PutCommand(string craneNo, byte TaskType)
        {
            string serviceName = "CranePLC" + craneNo;
            int[] cellAddr = new int[9];
            cellAddr[TaskType] = 1;            

            //cellAddr[3] = int.Parse(this.cbFromColumn.Text);
            //cellAddr[4] = int.Parse(this.cbFromHeight.Text);
            //cellAddr[5] = int.Parse(this.cbFromRow.Text.Substring(3, 3));
            //cellAddr[6] = int.Parse(this.cbToColumn.Text);
            //cellAddr[7] = int.Parse(this.cbToHeight.Text);
            //cellAddr[8] = int.Parse(this.cbToRow.Text.Substring(3, 3));

            Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
            Context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished", 0);
        }

        private void btnStop1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要急停1号堆垛机?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                PutCommand("1", 2);
                Logger.Info("1号堆垛机下发急停命令");
            }
        }

        //private void BindData()
        //{
        //    bsMain.DataSource = GetMonitorData();
        //}
        //private DataTable GetMonitorData()
        //{
        //    DataTable dt = bll.FillDataTable("WCS.SelectTask", new DataParameter[] { new DataParameter("{0}", "(WCS_TASK.TaskType='11' and WCS_TASK.State in('1','2','3')) OR (WCS_TASK.TaskType in('12','13') and WCS_TASK.State in('0','2','3')) OR (WCS_TASK.TaskType in('14') and WCS_TASK.State in('0','2','3','4','5','6'))") });
        //    return dt;
        //}

        //private void dgvMain_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
        //        {
        //            //若行已是选中状态就不再进行设置
        //            if (dgvMain.Rows[e.RowIndex].Selected == false)
        //            {
        //                dgvMain.ClearSelection();
        //                dgvMain.Rows[e.RowIndex].Selected = true;
        //            }
        //            //只选中一行时设置活动单元格
        //            if (dgvMain.SelectedRows.Count == 1)
        //            {
        //                dgvMain.CurrentCell = dgvMain.Rows[e.RowIndex].Cells[e.ColumnIndex];
        //            }
        //            string TaskType = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colTaskType"].Value.ToString();
        //            if (TaskType == "11")
        //            {
        //                this.ToolStripMenuItem11.Visible = true;
        //                this.ToolStripMenuItem12.Visible = false;
        //                this.ToolStripMenuItem13.Visible = true;
        //                this.ToolStripMenuItem14.Visible = false;
        //                this.ToolStripMenuItem15.Visible = false;
        //                this.ToolStripMenuItem16.Visible = false;
        //            }
        //            else if (TaskType == "12" || TaskType == "13")
        //            {
        //                this.ToolStripMenuItem11.Visible = false;
        //                this.ToolStripMenuItem12.Visible = false;
        //                this.ToolStripMenuItem13.Visible = true;
        //                this.ToolStripMenuItem14.Visible = false;
        //                this.ToolStripMenuItem15.Visible = false;
        //                this.ToolStripMenuItem16.Visible = false;
        //            }
        //            else if (TaskType == "14")
        //            {
        //                this.ToolStripMenuItem10.Visible = true;
        //                this.ToolStripMenuItem11.Visible = false;
        //                this.ToolStripMenuItem12.Visible = false;
        //                this.ToolStripMenuItem13.Visible = true;
        //                this.ToolStripMenuItem14.Visible = false;
        //                this.ToolStripMenuItem15.Visible = true;
        //                this.ToolStripMenuItem16.Visible = true;
        //            }
        //            //弹出操作菜单
        //            contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
        //        }
        //    }
        //}

        //private void ToolStripMenuItemCellCode_Click(object sender, EventArgs e)
        //{
        //    if (this.dgvMain.CurrentCell != null)
        //    {
        //        BLL.BLLBase bll = new BLL.BLLBase();
        //        string TaskNo = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells[0].Value.ToString();
        //        string TaskType = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colTaskType"].Value.ToString();
        //        string ErrCode = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colErrCode"].ToString();

        //        if (TaskType=="11")
        //        {
        //            DataGridViewSelectedRowCollection rowColl = dgvMain.SelectedRows;
        //            if (rowColl == null)
        //                return;
        //            DataRow dr = (rowColl[0].DataBoundItem as DataRowView).Row;
        //            frmReassignEmptyCell f = new frmReassignEmptyCell(dr);
        //            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //                this.BindData(); 
        //        }
        //        else if (TaskType == "12" || TaskType == "14")
        //        {
        //            DataGridViewSelectedRowCollection rowColl = dgvMain.SelectedRows;
        //            if (rowColl == null)
        //                return;
        //            DataRow dr = (rowColl[0].DataBoundItem as DataRowView).Row;
        //            frmReassign f = new frmReassign(dr);
        //            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //                this.BindData(); 
        //        }
        //        else if (TaskType == "13")
        //        {
        //            DataGridViewSelectedRowCollection rowColl = dgvMain.SelectedRows;
        //            if (rowColl == null)
        //                return;
        //            DataRow dr = (rowColl[0].DataBoundItem as DataRowView).Row;

        //            frmReassignOption fo = new frmReassignOption();
        //            if (fo.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //            {
        //                if (fo.option == 0)
        //                {
        //                    frmReassign f = new frmReassign(dr);
        //                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //                        this.BindData(); 
        //                }
        //                else
        //                {
        //                    frmReassignEmptyCell fe = new frmReassignEmptyCell(dr);
        //                    if (fe.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //                        this.BindData(); 
        //                }
        //            }                    
        //        }
        //    }
        //}

        //private void ToolStripMenuItemReassign_Click(object sender, EventArgs e)
        //{
        //    if (this.dgvMain.CurrentCell != null)
        //    {
        //        BLL.BLLBase bll = new BLL.BLLBase();
        //        string TaskNo = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells[0].Value.ToString();
        //        string TaskType = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells["colTaskType"].Value.ToString();

        //        if (TaskType == "11")
        //            bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 1), new DataParameter("@TaskNo", TaskNo) });
        //        else if (TaskType == "12" || TaskType == "13")
        //            bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 0), new DataParameter("@TaskNo", TaskNo) });
        //        else if (TaskType == "14")
        //        {
        //            frmTaskOption f = new frmTaskOption();
        //            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //            {
        //                if(f.option==0)
        //                    bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 1), new DataParameter("@TaskNo", TaskNo) });
        //                else
        //                    bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 5), new DataParameter("@TaskNo", TaskNo) });

        //            }
        //        }
        //        this.BindData();
        //    }
        //}

        //private void ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    string ItemName = ((ToolStripMenuItem)sender).Name;
        //    string State = ItemName.Substring(ItemName.Length-1, 1);

        //    if (this.dgvMain.CurrentCell != null)
        //    {
        //        BLL.BLLBase bll = new BLL.BLLBase();
        //        string TaskNo = this.dgvMain.Rows[this.dgvMain.CurrentCell.RowIndex].Cells[0].Value.ToString();

        //        DataParameter[] param = new DataParameter[] { new DataParameter("@TaskNo", TaskNo), new DataParameter("@State", State) };
        //        bll.ExecNonQueryTran("WCS.Sp_UpdateTaskState", param);
                
        //        //bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", State), new DataParameter("@TaskNo", TaskNo) });

        //        ////堆垛机完成执行
        //        //if (State == "7")
        //        //{
        //        //    DataParameter[] param = new DataParameter[] { new DataParameter("@TaskNo", TaskNo) };
        //        //    bll.ExecNonQueryTran("WCS.Sp_TaskProcess", param);
        //        //}
        //        BindData();
        //    }
        //}

        //private void ToolStripMenuItemDelCraneTask_Click(object sender, EventArgs e)
        //{
        //    string serviceName = "CranePLC1";
        //    int[] cellAddr = new int[9];
        //    cellAddr[0] = 0;
        //    cellAddr[1] = 1;

        //    Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
        //    Context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished", 0);

        //    MCP.Logger.Info("已给堆垛机下发取消任务指令");           
        //}        
    }
}
