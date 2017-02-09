using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;
using System.Timers;
namespace App.Dispatching.Process
{
    public class ElevatorProcess : AbstractProcess
    {
        string AreaCode = "002";
        string serviceName = "Elevator";
        private class CarStatus
        {
            public string CarNo { get; set; }
            public string TaskNo { get; set; }
            public string TaskType { get; set; }
            public int Status { get; set; }
            public int Action { get; set; }
            public int ErrCode { get; set; }
            public int TaskStatus { get; set; }
            public int io_flag { get; set; }            
        }
        private class ElevatorStatus
        {
            public string ElevatorNo { get; set; }
            public string TaskNo { get; set; }
            public string TaskType { get; set; }
            public int Status { get; set; }
            public int Action { get; set; }
            public int ErrCode { get; set; }
            public int TaskStatus { get; set; }
            public int io_flag { get; set; }
            public Dictionary<int, CarStatus> dicCar { get; set; }
        }
        // 记录堆垛机当前状态及任务相关信息
        BLL.BLLBase bll = new BLL.BLLBase();
        private Dictionary<int, ElevatorStatus> dicElevator = new Dictionary<int, ElevatorStatus>();
        
        //private Dictionary<int, CarStatus> dicCarStatus = new Dictionary<int, CarStatus>();
        private Timer tmWorkTimer = new Timer();
        private bool blRun = false;
        private DataTable dtDeviceAlarm;

        public override void Initialize(Context context)
        {
            try
            {
                dtDeviceAlarm = bll.FillDataTable("WCS.SelectDeviceAlarm", new DataParameter[] { new DataParameter("{0}", "Flag=3") });

                //获取堆垛机信息SelectElevatorCar
                DataTable dt = bll.FillDataTable("CMD.SelectDistinctElevator", new DataParameter[] { new DataParameter("{0}", "CMD_Elevator.State='1'") });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!dicElevator.ContainsKey(i))
                    {
                        ElevatorStatus elevatorStatus = new ElevatorStatus();
                        elevatorStatus.dicCar = new Dictionary<int, CarStatus>();

                        elevatorStatus.ElevatorNo = dt.Rows[i]["ElevatorNo"].ToString();
                        elevatorStatus.TaskNo = "";
                        elevatorStatus.Status = int.Parse(dt.Rows[i]["State"].ToString());
                        elevatorStatus.TaskStatus = 0;
                        elevatorStatus.ErrCode = 0;
                        elevatorStatus.Action = 0;
                        elevatorStatus.io_flag = 0;
                        DataTable dtCar = bll.FillDataTable("CMD.SelectElevatorCar", new DataParameter[] { new DataParameter("{0}", "CMD_Elevator.CarState='1'") });
                        for (int j = 0; j < dtCar.Rows.Count; j++)
                        {
                            CarStatus carStatus = new CarStatus();
                            carStatus.CarNo = dtCar.Rows[j]["CarNo"].ToString();
                            carStatus.TaskNo = "";
                            carStatus.Status = int.Parse(dtCar.Rows[j]["CarState"].ToString());
                            carStatus.TaskStatus = 0;
                            carStatus.ErrCode = 0;
                            carStatus.Action = 0;

                            elevatorStatus.dicCar.Add(j, carStatus);
                        }
                        dicElevator.Add(i, elevatorStatus);
                    }
                }

                tmWorkTimer.Interval = 1000;
                tmWorkTimer.Elapsed += new ElapsedEventHandler(tmWorker);

                base.Initialize(context);
            }
            catch (Exception ex)
            {
                Logger.Error("CarProcess堆垛机初始化出错，原因：" + ex.Message);
            }
        }
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            switch (stateItem.ItemName)
            {
                case "TaskFinished01":
                case "TaskFinished02":
                    object obj = ObjectUtil.GetObject(stateItem.State);
                    if (obj == null)
                        return;
                    int TaskNo = int.Parse(obj.ToString());
                    {                      
                        
                        //存储过程处理
                        if (TaskNo > 0)
                        {
                            WriteToService(stateItem.Name, stateItem.ItemName, 0);
                            Logger.Info(stateItem.ItemName + "完成标志,任务号:" + TaskNo);
                            //更新任务状态
                            bll.ExecNonQuery("WCS.UpdateTaskCarNoByTaskNo", new DataParameter[] { new DataParameter("@CarNo", stateItem.ItemName.Substring(12,2)), new DataParameter("@TaskNo", TaskNo) });
                            DataParameter[] param = new DataParameter[] { new DataParameter("@TaskNo", TaskNo) };
                            bll.ExecNonQueryTran("WCS.Sp_TaskProcess", param);
                        }
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
                            //输送线出库
                            sbyte[] OutTaskNo = new sbyte[20];
                            Util.ConvertStringChar.stringToBytes(TaskNo + PalletCode, 10).CopyTo(OutTaskNo, 0);
                            WriteToService("TranLine", "OutTaskNo2", OutTaskNo);
                            if (WriteToService("TranLine", "OutFinish2", 1))
                            {
                                bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 6), new DataParameter("@TaskNo", TaskNo) });
                            }
                        }
                    }
                    break;
                case "CraneAlarmCode":
                    object obj1 = ObjectUtil.GetObject(stateItem.State);
                    if (obj1 == null)
                        return;
                    if (obj1.ToString() != "0")
                    {
                        string strError = "";
                        DataRow[] drs = dtDeviceAlarm.Select(string.Format("AlarmCode={0}", obj1.ToString()));
                        if (drs.Length > 0)
                            strError = drs[0]["AlarmCode"].ToString();
                        else
                            strError = "穿梭车未知错误！";
                        Logger.Error(strError);
                    }
                    break;
                case "Run":
                    blRun = (int)stateItem.State == 1;
                    if (blRun)
                    {
                        tmWorkTimer.Start();
                        Logger.Info("提升机联机");
                    }
                    else
                    {
                        tmWorkTimer.Stop();
                        Logger.Info("提升机脱机");
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
                if (!blRun)
                {
                    tmWorkTimer.Stop();
                    return;
                }
                tmWorkTimer.Stop();

                DataTable dt = bll.FillDataTable("CMD.SelectDistinctElevator", new DataParameter[] { new DataParameter("{0}", "CMD_Elevator.CarState='1'") });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!dicElevator.ContainsKey(i))
                    {
                        dicElevator[i].Status = int.Parse(dt.Rows[i]["State"].ToString());
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dicElevator[i].Status != 1)
                        continue;
                    if (dicElevator[i].io_flag == 0)
                    {
                        ElevatorOut(dicElevator[i].ElevatorNo);
                    }
                    else
                    {
                        ElevatorIn(dicElevator[i].ElevatorNo);
                    }
                }

            }
            finally
            {
                tmWorkTimer.Start();
            }
        }
        /// <summary>
        /// 检查小车入库状态
        /// </summary>
        /// <param name="piCrnNo"></param>
        /// <returns></returns>
        private bool Check_Elevator_Status_IsOk(string elevatorNo)
        {
            try
            {
                object[] obj = ObjectUtil.GetObjects(WriteToService(serviceName, "ElevatorStatus"));
                 int ElevatorMode = int.Parse(obj[0].ToString());
                //int CraneAlarmCode = int.Parse(obj[0].ToString());
                 int IsSendTask = int.Parse(obj[2].ToString());
                 if (ElevatorMode == 1 && IsSendTask==1)
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
        /// <param name="carNo"></param>
        private void ElevatorOut(string elevatorNo)
        {
            // 判断提升机的状态 自动  空闲
            try
            {
                //判断堆垛机
                if (!Check_Elevator_Status_IsOk(elevatorNo))
                    return;

                //切换入库优先
                int i = int.Parse(elevatorNo) - 1;
                dicElevator[i].io_flag = 1;
            }
            catch (Exception ex)
            {
                //Logger.Debug("Elevator Out 状态检查错误:" + ex.Message.ToString());
                return;
            }

            try
            {
                object[] obj1 = ObjectUtil.GetObjects(WriteToService(serviceName, "Car01"));
                //string carStatus = Util.ConvertStringChar.BytesToString(obj);
                object[] obj2 = ObjectUtil.GetObjects(WriteToService(serviceName, "Car02"));

                int TaskType1 = int.Parse(obj1[5].ToString());
                int TaskNo1 = int.Parse(obj1[6].ToString());
                
                int TaskType2 = int.Parse(obj2[5].ToString());
                int TaskNo2 = int.Parse(obj2[6].ToString());

                if (TaskNo1 > 0 && TaskNo2 > 0)
                {
                    if (TaskType1 == 10 && TaskType2 == 10)
                    {
                        //查找出库任务
                    }
                    else if(TaskType1 == 10 && TaskType2 == 11)
                    {
                        //查找出库任务
                    }
                    else if (TaskType1 == 11 && TaskType2 == 10)
                    {
                        //查找出库任务
                    }
                    else if (TaskType1 == 11 && TaskType2 == 11)
                    {
                        //查找出库任务
                    }
                }
                else if (TaskNo1 > 0 && TaskNo2 <= 0)
                {

                }
                else if (TaskNo2 > 0 && TaskNo1 <= 0)
                {

                }
                else if (TaskNo1 <= 0 && TaskNo2 <= 0)
                {

                }
                //获取任务，排序优先等级、任务时间
                //DataParameter[] parameter = new DataParameter[] { new DataParameter("{0}", string.Format(" WCS_Task.AreaCode='{0}' ", AreaCode)) };
                //DataTable dt = bll.FillDataTable("WCS.SelectTask", parameter);


                //入库
                //DataParameter[] parameter = new DataParameter[] { new DataParameter("{0}", string.Format("((WCS_Task.TaskType in ('12','13','14','15') and WCS_Task.State='0') or (WCS_Task.TaskType in ('11','14','16') and WCS_Task.State='2')) and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' ", AreaCode)) };
                string filter = "";
                DataParameter[] parameter = new DataParameter[] { new DataParameter("{0}", string.Format("(WCS_Task.State='0' or WCS_Task.State='2') and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' ", AreaCode)) };
                DataTable dt = bll.FillDataTable("WCS.SelectTask", parameter);
                int outCount = Convert.ToInt32(dt.Compute("count(1)", "TaskType in ('12','13','14','15')"));

                filter = string.Format("CellRow={0} and TaskType in ('12','13','14','15')", obj1[0]);
                DataRow[] drOut = dt.Select(filter, "TaskLevel");
                

                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    string TaskNo = dr["TaskNo"].ToString();
                    string BillID = dr["BillID"].ToString();
                    string TaskType = dr["TaskType"].ToString();
                    string state = dr["State"].ToString();
                    int taskType = 10;
                    string NextState = "3";
                    if (state == "0")
                    {
                        taskType = 11;
                        NextState = "4";
                    }

                    string fromStation = dt.Rows[0]["FromStation"].ToString();
                    string toStation = dt.Rows[0]["ToStation"].ToString();

                    int[] cellAddr = new int[11];

                    cellAddr[0] = 0;
                    cellAddr[1] = 0;
                    cellAddr[2] = 0;

                    cellAddr[3] = byte.Parse(fromStation.Substring(3, 3));
                    cellAddr[4] = byte.Parse(fromStation.Substring(6, 3));
                    cellAddr[5] = byte.Parse(fromStation.Substring(0, 3));
                    cellAddr[6] = byte.Parse(toStation.Substring(3, 3));
                    cellAddr[7] = byte.Parse(toStation.Substring(6, 3));
                    cellAddr[8] = byte.Parse(toStation.Substring(0, 3));
                    cellAddr[9] = taskType;
                    cellAddr[10] = 0;

                    int taskNo = int.Parse(TaskNo);

                    Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
                    Context.ProcessDispatcher.WriteToService(serviceName, "TaskNo", taskNo);
                    if (WriteToService(serviceName, "WriteFinished", 1))
                    {
                        bll.ExecNonQuery("WCS.UpdateTaskTimeByTaskNo", new DataParameter[] { new DataParameter("@State", NextState), new DataParameter("@TaskNo", TaskNo) });
                        bll.ExecNonQuery("WCS.UpdateBillStateByBillID", new DataParameter[] { new DataParameter("@State", 3), new DataParameter("@BillID", BillID) });
                    }
                    Logger.Info("任务:" + dr["TaskNo"].ToString() + "已下发给" + elevatorNo + "提升机;起始地址:" + fromStation + ",目标地址:" + toStation);
                }

            }
            catch (Exception ex1)
            {
                Logger.Debug("ElevatorProcess中下发入库任务错误:" + ex1.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="carNo"></param>
        private void ElevatorIn(string elevatorNo)
        {
            // 判断堆垛机的状态 自动  空闲
            try
            {
                //判断提升机
                if (!Check_Elevator_Status_IsOk(elevatorNo))
                    return;

                //切换入库优先
                int i = int.Parse(elevatorNo) - 1;
                dicElevator[i].io_flag = 0;
            }
            catch (Exception e)
            {
                //Logger.Debug("Crane out 状态检查错误:" + e.Message.ToString());
                return;
            }

            try
            {

                object[] obj = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "Status"));
                string carStatus = Util.ConvertStringChar.BytesToString(obj);

                //获取任务，排序优先等级、任务时间
                DataParameter[] parameter = new DataParameter[] { new DataParameter("{0}", string.Format(" WCS_Task.State='2' and  WCS_Task.AreaCode='{0}' ", AreaCode)) };
                DataTable dt = bll.FillDataTable("WCS.SelectTask", parameter);
                if (dt.Rows.Count > 0)
                {

                    string TaskNo = dt.Rows[0]["TaskNo"].ToString();

                    //分配货位
                    string CellCode = dt.Rows[0]["CellCode"].ToString();
                    if (CellCode == "")
                    {
                        parameter = new DataParameter[] { new DataParameter("@AreaCode", AreaCode) };

                        dt = bll.FillDataTable("WCS.sp_GetCell2", parameter);
                        if (dt.Rows.Count > 0)
                            CellCode = dt.Rows[0][0].ToString();
                        parameter = new DataParameter[] { new DataParameter("{0}", string.Format("CellCode='{0}' and PalletBarCode='' and IsActive='1' and IsLock='0' and AreaCode='{1}'", CellCode, AreaCode)) };
                        dt = bll.FillDataTable("CMD.SelectCell", parameter);
                        if (dt.Rows.Count <= 0)
                        {
                            Logger.Error("CarProcess 中CraneIn自动获取的货位或指定的货位非空货位,请确认！");
                            return;
                        }

                        parameter = new DataParameter[] { new DataParameter("@CellCode", CellCode), new DataParameter("@TaskNo", TaskNo), new DataParameter("@AreaCode", AreaCode) };
                        bll.ExecNonQueryTran("WCS.Sp_ExecuteInStockTask2", parameter);

                    }
                    //入库
                    parameter = new DataParameter[] { new DataParameter("{0}", string.Format("WCS_Task.State='2' and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='0{1}' and WCS_Task.TaskNo='{2}' ", AreaCode, elevatorNo, TaskNo)) };
                    dt = bll.FillDataTable("WCS.SelectTask", parameter);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];



                        string BillID = dr["BillID"].ToString();
                        int taskType = 10;
                        string fromStation = dt.Rows[0]["FromStation"].ToString();
                        string toStation = dt.Rows[0]["ToStation"].ToString();

                        int[] cellAddr = new int[6];

                        cellAddr[0] = byte.Parse(fromStation.Substring(3, 3));
                        cellAddr[1] = byte.Parse(fromStation.Substring(6, 3));
                        cellAddr[2] = byte.Parse(fromStation.Substring(0, 3));
                        cellAddr[3] = byte.Parse(toStation.Substring(3, 3));
                        cellAddr[4] = byte.Parse(toStation.Substring(6, 3));
                        cellAddr[5] = byte.Parse(toStation.Substring(0, 3));

                        int taskNo = int.Parse(TaskNo);

                        Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
                        Context.ProcessDispatcher.WriteToService(serviceName, "TaskNo", taskNo);
                        Context.ProcessDispatcher.WriteToService(serviceName, "TaskType", taskType);
                        if (WriteToService(serviceName, "WriteFinished", 1))
                        {
                            string State = "3";

                            bll.ExecNonQuery("WCS.UpdateTaskTimeByTaskNo", new DataParameter[] { new DataParameter("@State", State), new DataParameter("@TaskNo", TaskNo) });
                            bll.ExecNonQuery("WCS.UpdateBillStateByBillID", new DataParameter[] { new DataParameter("@State", 3), new DataParameter("@BillID", BillID) });
                        }
                        Logger.Info("任务:" + dr["TaskNo"].ToString() + "已下发给" + elevatorNo + "堆垛机;起始地址:" + fromStation + ",目标地址:" + toStation);
                    }
                }

            }
            catch (Exception ex1)
            {
                Logger.Debug("CarProcess中Car IN下发入库任务错误:" + ex1.Message);
            }
        }
    }
}