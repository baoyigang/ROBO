﻿using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;
using System.Timers;
namespace App.Dispatching.Process
{
    public class MiniLoadProcess : AbstractProcess
    {
        string AreaCode = "003";
        string serviceName = "MiniLoad02";
        private class rCrnStatus
        {
            public string TaskNo { get; set; }
            public int Status { get; set; }
            public int Action { get; set; }
            public int ErrCode { get; set; }
            public int TaskStatus { get; set; }
            public int io_flag { get; set; }

            public rCrnStatus()
            {
                TaskNo = "";
                Status = 0;
                Action = 0;
                ErrCode = 0;
                TaskStatus = 0;
                io_flag = 0;
            }
        }

        // 记录堆垛机当前状态及任务相关信息
        BLL.BLLBase bll = new BLL.BLLBase();
        private Dictionary<int, rCrnStatus> dCrnStatus = new Dictionary<int, rCrnStatus>();
        private Timer tmWorkTimer = new Timer();
        private bool blRun = false;
        private DataTable dtDeviceAlarm;


        public override void Initialize(Context context)
        {
            try
            {
                dtDeviceAlarm = bll.FillDataTable("WCS.SelectDeviceAlarm", new DataParameter[] { new DataParameter("{0}", "Flag=1") });

                //获取堆垛机信息
                DataTable dt = bll.FillDataTable("CMD.SelectCrane", new DataParameter[] { new DataParameter("{0}", "CraneNo='02'") });
                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    if (!dCrnStatus.ContainsKey(i))
                    {
                        rCrnStatus crnsta = new rCrnStatus();
                        dCrnStatus.Add(i, crnsta);

                        dCrnStatus[i].TaskNo = "";
                        dCrnStatus[i].Status = int.Parse(dt.Rows[i - 1]["State"].ToString());
                        dCrnStatus[i].TaskStatus = 0;
                        dCrnStatus[i].ErrCode = 0;
                        dCrnStatus[i].Action = 0;
                    }
                }

                tmWorkTimer.Interval = 1000;
                tmWorkTimer.Elapsed += new ElapsedEventHandler(tmWorker);


                base.Initialize(context);
            }
            catch (Exception ex)
            {
                Logger.Error("MiniLoadProcess堆垛机初始化出错，原因：" + ex.Message);
            }
        }
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            switch (stateItem.ItemName)
            {
                case "TaskFinished1":
                case "TaskFinished2":

                    object obj = ObjectUtil.GetObject(stateItem.State);
                    if (obj == null)
                        return;
                    string TaskFinish = obj.ToString();
                    if (TaskFinish.Equals("True") || TaskFinish.Equals("1"))
                    {
                        int taskIndex = int.Parse(stateItem.ItemName.Substring(stateItem.ItemName.Length - 1, 1));
                        string TaskNo = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(stateItem.Name, "CraneTaskNo" + taskIndex)));

                        if (TaskNo.Length == 0)
                            return;
                        Logger.Info(stateItem.ItemName + "完成标志,任务号:" + TaskNo);
                        //更新任务状态
                        DataParameter[] param = new DataParameter[] { new DataParameter("@TaskNo", TaskNo) };
                        bll.ExecNonQueryTran("WCS.Sp_TaskProcess", param);

                        DataParameter[] paras = new DataParameter[] { new DataParameter("{0}", string.Format("WCS_Task.TaskNo='{0}'", TaskNo)) };
                        DataTable dt = bll.FillDataTable("WCS.SelectTask", paras);

                        string PalletCode = "";
                        string strState = "";

                        if (dt.Rows.Count > 0)
                        {
                            PalletCode = dt.Rows[0]["PalletCode"].ToString();
                            strState = dt.Rows[0]["State"].ToString();

                        }
                        if (strState == "5")
                        {
                            string OutStateName = "OutTaskNo3";
                            string OutFinishName = "OutFinish3";
                            if (taskIndex == 2)
                            {
                                OutStateName = "OutTaskNo1";
                                OutFinishName = "OutFinish1";
                            }
                            //输送线出库
                            sbyte[] OutTaskNo = new sbyte[20];
                            Util.ConvertStringChar.stringToBytes(TaskNo + PalletCode, 20).CopyTo(OutTaskNo, 0);
                            WriteToService("TranLine", OutStateName, OutTaskNo);
                            if (WriteToService("TranLine", OutFinishName, 1))
                            {
                                bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 6), new DataParameter("@TaskNo", TaskNo) });
                            }
                        }
                        sbyte[] ClearTaskNo = new sbyte[20];
                        Util.ConvertStringChar.stringToBytes("", 20).CopyTo(ClearTaskNo, 0);
                        WriteToService(stateItem.Name, "TaskNo" + taskIndex, ClearTaskNo);
                    }
                    break;
                case "PLCCheck":
                    WriteToService(stateItem.Name, "WriteFinished", 0);
                    break;
                case "AlarmCode":
                    object obj1 = ObjectUtil.GetObject(stateItem.State);
                    if (obj1 == null)
                        return;
                    if (obj1.ToString() != "0")
                    {
                        string strError = "";
                        DataRow[] drs = dtDeviceAlarm.Select(string.Format("Flag=1 And AlarmCode={0}", obj1.ToString()));
                        if (drs.Length > 0)
                            strError = drs[0]["AlarmDesc"].ToString();
                        else
                            strError = "堆垛机未知错误！";
                        Logger.Error(strError);
                    }
                    break;
                case "Run":
                    blRun = (int)stateItem.State == 1;
                    if (blRun)
                    {
                        tmWorkTimer.Start();
                        Logger.Info("堆垛机联机");
                    }
                    else
                    {
                        tmWorkTimer.Stop();
                        Logger.Info("堆垛机脱机");
                    }
                    break;
                default:
                    break;
            }


            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmWorker(object sender, ElapsedEventArgs e)
        {
            try
            {
                tmWorkTimer.Stop();

                DataTable dt = bll.FillDataTable("CMD.SelectCrane", new DataParameter[] { new DataParameter("{0}", "CraneNo='02'") });
                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    if (!dCrnStatus.ContainsKey(i))
                    {
                        dCrnStatus[i].Status = int.Parse(dt.Rows[i - 1]["State"].ToString());
                    }
                }

                for (int i = 1; i <= 1; i++)
                {
                    if (dCrnStatus[i].Status != 1)
                        continue;
                    if (dCrnStatus[i].io_flag == 0)
                    {
                        CraneOut(2);
                    }
                    else
                    {
                        CraneIn(2);
                    }
                }

            }
            finally
            {
                tmWorkTimer.Start();
            }
        }
        /// <summary>
        /// 检查堆垛机入库状态
        /// </summary>
        /// <param name="piCrnNo"></param>
        /// <returns></returns>
        private bool Check_Crane_Status_IsOk(int craneNo)
        {
            try
            {
                string serviceName = "MiniLoad0" + craneNo;

                string plcTaskNo1 = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "CraneTaskNo1")));
                string craneMode = ObjectUtil.GetObject(Context.ProcessDispatcher.WriteToService(serviceName, "CraneMode")).ToString();
                string CraneState1 = ObjectUtil.GetObject(Context.ProcessDispatcher.WriteToService(serviceName, "CraneState1")).ToString();
                string CraneAlarmCode = ObjectUtil.GetObject(Context.ProcessDispatcher.WriteToService(serviceName, "AlarmCode")).ToString();

                string plcTaskNo2 = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "CraneTaskNo2")));
                string craneState2 = ObjectUtil.GetObject(Context.ProcessDispatcher.WriteToService(serviceName, "CraneState2")).ToString();

                string CraneLoad1 = ObjectUtil.GetObject(Context.ProcessDispatcher.WriteToService(serviceName, "CraneLoad1")).ToString();
                string CraneLoad2 = ObjectUtil.GetObject(Context.ProcessDispatcher.WriteToService(serviceName, "CraneLoad1")).ToString();


                if (plcTaskNo1 == "" && craneMode == "1" && CraneAlarmCode == "0" && CraneState1 == "1" && plcTaskNo2 == "" && craneState2 == "1" && CraneLoad1 == "0" && CraneLoad2 == "0")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="craneNo"></param>
        private void CraneOut(int craneNo)
        {
            // 判断堆垛机的状态 自动  空闲
            //Logger.Debug("判断堆垛机" + piCrnNo.ToString() + "能否出库");
            try
            {

                //判断堆垛机
                if (!Check_Crane_Status_IsOk(craneNo))
                {
                    //Logger.Info("堆垛机状态不符合出库");
                    return;
                }
                //切换入库优先
                dCrnStatus[craneNo-1].io_flag = 1;
            }
            catch (Exception e)
            {
                Logger.Debug("Crane out 状态检查错误:" + e.Message.ToString());
                return;
            }

            try
            {
                string CraneNo = "0" + craneNo.ToString();
                //获取任务，排序优先等级、任务时间
                DataParameter[] param = new DataParameter[] { new DataParameter("{0}", string.Format("WCS_Task.TaskType in ('12','13','14','15') and WCS_Task.State='0' and WCS_Task.CraneNo='{0}'", CraneNo)) };
                DataTable dt = bll.FillDataTable("WCS.sp_GetOutStockTask", new DataParameter[] { new DataParameter("@AreaCode", AreaCode) });

                //先找12，15在同一列的双任务 100
                //找移库任务，执行 101
                //找单一出库任务，拼双任务，  深度为1为102；深度为2 为 103
                //盘点任务，以同一列的货位下任务，不管是单个还是两个，都一次性下发执行，为了回原库位考虑 104
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Flag"].ToString() == "100") 
                    {
                        DataRow[] drs = new DataRow[2];
                        drs[0] = dt.Rows[0];
                        drs[1] = dt.Rows[1];
                        Send2PLC(drs);
                    }
                    else if (dt.Rows[0]["Flag"].ToString() == "101")
                    {
                        DataRow[] drs = new DataRow[1];
                        drs[0] = dt.Rows[0];
                        Send2PLC(drs);
                    }
                    else if (dt.Rows[0]["Flag"].ToString() == "102")
                    {
                        string filter = ("Flag='103'");
                        DataRow[] drs2 = dt.Select(filter, "TaskLevel");
                        if (drs2.Length > 0)
                        {
                            DataRow[] drs = new DataRow[2];
                            drs[0] = drs2[0];
                            drs[1] = dt.Rows[0];
                            Send2PLC(drs);
                        }
                        else
                        {
                            string TaskNo = dt.Rows[0]["TaskNo"].ToString();
                            filter = string.Format("Flag='102' and TaskNo<>'{0}'", TaskNo);
                            DataRow[] drs3 = dt.Select(filter, "TaskLevel");
                            if (drs3.Length > 0)
                            {
                                DataRow[] drs = new DataRow[2];
                                drs[0] = drs3[0];
                                drs[1] = dt.Rows[0];
                                Send2PLC(drs);
                            }
                            else
                            {
                                DataRow[] drs = new DataRow[1];
                                drs[0] = dt.Rows[0];
                                Send2PLC(drs);
                            }
                        }
                    }
                    else if (dt.Rows[0]["Flag"].ToString() == "103")
                    {
                        string TaskNo = dt.Rows[0]["TaskNo"].ToString();
                        string filter = string.Format("Flag='103' and TaskNo<>'{0}'", TaskNo);
                        DataRow[] drs2 = dt.Select(filter, "TaskLevel");

                        if (drs2.Length > 0)
                        {
                            DataRow[] drs = new DataRow[2];
                            drs[0] = drs2[0];
                            drs[1] = dt.Rows[0];
                            Send2PLC(drs);
                        }
                        else
                        {
                            DataRow[] drs = new DataRow[1];
                            drs[0] = dt.Rows[0];
                            Send2PLC(drs);
                        }
                    }
                    else if (dt.Rows[0]["Flag"].ToString() == "104")
                    {
                        string CellCode = dt.Rows[0]["CellCode"].ToString().Substring(0,9);
                        string TaskNo = dt.Rows[0]["TaskNo"].ToString();
                        string filter = string.Format("Flag='104' and substring(CellCode,1,9)='{0}' and TaskNo<>'{1}'",CellCode,TaskNo);
                        DataRow[] drs2 = dt.Select(filter, "TaskLevel");
                        if (drs2.Length > 0)
                        {
                            DataRow[] drs = new DataRow[2];
                            drs[0] = dt.Rows[0];
                            drs[1] = drs2[0];
                            Send2PLC(drs);
                        }
                        else
                        {
                            DataRow[] drs = new DataRow[1];
                            drs[0] = dt.Rows[0];
                            Send2PLC(drs);
                        }
                    }
                }
            }
            catch (Exception ex1)
            {
                Logger.Debug("Crane out下发出库任务错误:" + ex1.Message);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="craneNo"></param>

        private void CraneIn(int craneNo)
        {
            // 判断堆垛机的状态 自动  空闲
            try
            {
                //判断堆垛机
                if (!Check_Crane_Status_IsOk(craneNo))
                    return;

                //切换入库优先
                dCrnStatus[1].io_flag = 0;
            }
            catch (Exception e)
            {
                //Logger.Debug("Crane out 状态检查错误:" + e.Message.ToString());
                return;
            }

            try
            {
                //获取任务，排序优先等级、任务时间
                DataParameter[] parameter = new DataParameter[] { new DataParameter("{0}", string.Format("WCS_Task.TaskType in('11','14','16') and WCS_Task.State in('1','2') and WCS_Task.AreaCode='{0}' ", AreaCode)) };
                DataTable dt = bll.FillDataTable("WCS.SelectTask", parameter);

                bool TaskOK = false;
                if (dt.Rows.Count > 0)
                {
                    string filter = "TaskType in('11','16') and State='2'";
                    DataRow[] drs = dt.Select(filter, "RequestDate desc");
                    if (drs.Length <= 0)
                        TaskOK = false;
                    else if (drs.Length == 1)
                    {
                        string StationNo = drs[0]["StationNo"].ToString();
                        if (StationNo == "03")
                        {
                            filter = "TaskType in('11','16') and State='1'";
                            DataRow[] drsOnTheWay = dt.Select(filter, "TaskNo");
                            //如果有在途的入库任务，等待
                            if (drsOnTheWay.Length > 0)
                                TaskOK = false;
                            else
                                TaskOK = true;
                        }
                        else if (StationNo == "01")
                        {
                            TaskOK = true;
                        }
                    }
                    else if(drs.Length==2)
                        TaskOK = true;

                    if (TaskOK)
                    {
                        //分配货位开始执行，用存储过程
                        DataParameter[] param = new DataParameter[] { new DataParameter("@AreaCode", AreaCode) };
                        bll.ExecNonQueryTran("WCS.Sp_UpdateMiniLoadTaskCell", param);
                        //开始下发任务给PLC,因为有更新货位所以要重新获取
                        
                        parameter = new DataParameter[] { new DataParameter("{0}", string.Format("WCS_Task.TaskType in('11','14','16') and WCS_Task.State in('2') and WCS_Task.AreaCode='{0}' ", AreaCode)) };
                        dt = bll.FillDataTable("WCS.SelectTask", parameter);
                        filter = "TaskType in('11','16') and State='2'";
                        drs = dt.Select(filter, "RequestDate desc");
                        Send2PLC(drs);
                    }
                    else
                    {
                        filter = "TaskType in('14') and State='2'";
                        drs = dt.Select(filter, "RequestDate desc");
                        if (drs.Length == 1)
                        {
                            //判断是否还有一个任务没到
                            string BillID = drs[0]["BillID"].ToString();
                            string CellCode = drs[0]["CellCode"].ToString().Substring(0,9);

                            DataParameter[] paras = new DataParameter[] { new DataParameter("{0}", string.Format("WCS_Task.BillID='{0}' and WCS_Task.TaskType='14' and WCS_Task.State in('6','8') and substring(WCS_Task.CellCode,1,9)='{1}'", BillID,CellCode)) };
                            DataTable dtTask = bll.FillDataTable("WCS.SelectTask", paras);
                            if(dtTask.Rows.Count<=0)
                                Send2PLC(drs);
                        }
                        else if (drs.Length == 2)
                        {
                            Send2PLC(drs);
                        }
                    }
                }
            }
            catch (Exception ex1)
            {
                Logger.Debug("MiniLoadProcess中MiniLoad IN下发入库任务错误:" + ex1.Message);
            }
        }
        private void Send2PLC(DataRow[] drs)
        {
            int[] TaskNo = new int[2];
            string NextState = "3";
            string[] fStation = new string[2];
            string[] tStation = new string[2];
            for (int i = 0; i < drs.Length; i++)
            {
                DataRow dr = drs[i];

                int TaskIndex = 1;
                int taskType = int.Parse(dr["TaskType"].ToString());
                string fromStation = dr["FromStation"].ToString();
                string toStation = dr["ToStation"].ToString();
                int fromDepth = int.Parse(fromStation.Substring(9, 1));
                int toDepth = int.Parse(toStation.Substring(9, 1));
                string state = dr["State"].ToString();

                if (state == "0")
                {
                    NextState = "4";
                }
                if (drs.Length == 1)
                {

                    TaskIndex = 1;
                    TaskNo[0] = int.Parse(dr["TaskNo"].ToString());
                    fStation[0] = dr["FromStation"].ToString();
                    tStation[0] = dr["ToStation"].ToString();
                    bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "B"), new DataParameter("@MergeTaskNo", TaskNo[0]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                }
                else
                {

                    fStation[i] = dr["FromStation"].ToString();
                    tStation[i] = dr["ToStation"].ToString();
                    int fromDepth1 = int.Parse(drs[0]["FromStation"].ToString().Substring(9, 1));
                    int fromDepth2 = int.Parse(drs[1]["FromStation"].ToString().Substring(9, 1));
                    if (fromDepth1 == 1 && fromDepth2 == 1)
                    {
                        if (i == 0)
                        {
                            TaskIndex = 2;
                            TaskNo[1] = int.Parse(dr["TaskNo"].ToString());
                            fStation[1] = dr["FromStation"].ToString();
                            tStation[1] = toStation;
                            bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "A"), new DataParameter("@MergeTaskNo", TaskNo[0]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                        }
                        else
                        {
                            TaskIndex = 1;
                            toStation = "0050000012"; //修改出库口位置
                            TaskNo[0] = int.Parse(dr["TaskNo"].ToString());
                            fStation[0] = dr["FromStation"].ToString();
                            tStation[0] = dr["ToStation"].ToString();
                            bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "B"), new DataParameter("@MergeTaskNo", TaskNo[1]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                        }
                    }

                    else if (fromDepth1 == 2 && fromDepth2 == 2)
                    {
                        if (i == 0)
                        {

                            TaskIndex = 2;
                            toStation = "0050000011"; //修改出库口位置
                            
                            TaskNo[1] = int.Parse(dr["TaskNo"].ToString());
                            fStation[1] = dr["FromStation"].ToString();
                            tStation[1] = toStation;
                            bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "A"), new DataParameter("@MergeTaskNo", TaskNo[0]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                        }
                        else
                        {
                            TaskIndex = 1;
                            TaskNo[0] = int.Parse(dr["TaskNo"].ToString());
                            fStation[0] = dr["FromStation"].ToString();
                            tStation[0] = dr["ToStation"].ToString();
                            bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "B"), new DataParameter("@MergeTaskNo", TaskNo[1]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                        }
                    }
                    else
                    {

                        if (taskType == 11 || (taskType == 14 && state == "2") || taskType == 16)
                        {
                            if (fromDepth == 2)
                            {
                                TaskIndex = 2;
                                TaskNo[1] = int.Parse(dr["TaskNo"].ToString());
                                fStation[1] = dr["FromStation"].ToString();
                                tStation[1] = dr["ToStation"].ToString();
                                bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "A"), new DataParameter("@MergeTaskNo", TaskNo[0]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                            }
                            else
                            {
                                TaskIndex = 1;
                                TaskNo[0] = int.Parse(dr["TaskNo"].ToString());
                                fStation[0] = dr["FromStation"].ToString();
                                tStation[0] = dr["ToStation"].ToString();
                                bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "B"), new DataParameter("@MergeTaskNo", TaskNo[1]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                            }
                        }
                        else
                        {
                            if (fromDepth == 2)
                            {
                                TaskIndex = 1;
                                TaskNo[0] = int.Parse(dr["TaskNo"].ToString());
                                fStation[0] = dr["FromStation"].ToString();
                                tStation[0] = dr["ToStation"].ToString();
                                bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "B"), new DataParameter("@MergeTaskNo", TaskNo[1]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                            }
                            else
                            {
                                TaskIndex = 2;
                                TaskNo[1] = int.Parse(dr["TaskNo"].ToString());
                                fStation[1] = dr["FromStation"].ToString();
                                tStation[1] = dr["ToStation"].ToString();
                                bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "A"), new DataParameter("@MergeTaskNo", TaskNo[0]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });
                            }
                        }
                    }
                    //if (i == 0)
                    //{
                    //    TaskIndex = 2;
                    //    TaskNo[0] = int.Parse(dr["TaskNo"].ToString());
                    //    bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "B"), new DataParameter("@MergeTaskNo", TaskNo[0]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });

                    //}
                    //else
                    //{
                    //    TaskIndex = 1;
                    //    TaskNo[1] = int.Parse(dr["TaskNo"].ToString());
                    //    bll.ExecNonQuery("WCS.UpdateTaskAB", new DataParameter[] { new DataParameter("@TaskAB", "A"), new DataParameter("@MergeTaskNo", TaskNo[0]), new DataParameter("@TaskNo", dr["TaskNo"].ToString()) });

                    //}
                }

                int[] cellAddr = new int[6];
                cellAddr[0] = int.Parse(fromStation.Substring(3, 3));
                cellAddr[1] = int.Parse(fromStation.Substring(6, 3));
                cellAddr[2] = GetPLCShelf(fromStation);
                cellAddr[3] = int.Parse(toStation.Substring(3, 3));
                cellAddr[4] = int.Parse(toStation.Substring(6, 3));
                cellAddr[5] = GetPLCShelf(toStation);
                //寫入任務號完成之後,讀取任務號是否寫入一致!
                sbyte[] sTaskNo = new sbyte[20];
                Util.ConvertStringChar.stringToBytes(dr["TaskNo"].ToString(), 20).CopyTo(sTaskNo, 0);

                WriteToService(serviceName, "TaskNo" + TaskIndex, sTaskNo);
                WriteToService(serviceName, "TaskAddress" + TaskIndex, cellAddr);

                WriteToService(serviceName, "TaskType" + TaskIndex, 1);

            }
           
            if (WriteToService(serviceName, "WriteFinished", 1))
            {
                for (int i = 0; i < drs.Length; i++)
                {                   
                    
                    string taskNo = drs[i]["TaskNo"].ToString();
                    string BillID = drs[i]["BillID"].ToString();
                    bll.ExecNonQuery("WCS.UpdateTaskTimeByTaskNo", new DataParameter[] { new DataParameter("@State", NextState), new DataParameter("@CarNo", ""), new DataParameter("@TaskNo", taskNo) });
                    bll.ExecNonQuery("WCS.UpdateBillStateByBillID", new DataParameter[] { new DataParameter("@State", NextState), new DataParameter("@BillID", BillID) });

                    //Logger.Info("任务:" + taskNo + "已下发给MiniLoad;起始地址:" + fStation[i] + ",目标地址:" + tStation[i]);
                }
                
                Logger.Info("A任务:" + TaskNo[0] + ",起始地址：" + fStation[0] + ",目标地址：" +  tStation[0] + ";B任务:" + TaskNo[1] + ",起始地址：" + fStation[1] + ",目标地址：" +  tStation[1] + "已下发");
            }
            
        }

        private int GetPLCShelf(string CellCode)
        {
            int reshelf = 5;
            int shelf = int.Parse(CellCode.Substring(0, 3));
            int Depth = int.Parse(CellCode.Substring(9, 1));
            if (shelf == 5)
            {
                if (Depth == 2)
                    reshelf = shelf;
                else
                    reshelf = shelf + 1;
            }
            else
            {
                reshelf = shelf + Depth;
            }
            return reshelf;
        }
    }
}