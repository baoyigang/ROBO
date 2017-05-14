using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;
using System.Timers;
namespace App.Dispatching.Process
{
   public class CarProcess : AbstractProcess
    {
       string AreaCode = "002";
        private class CarStatus
        {
            public string CarNo { get; set; }
            public string TaskNo { get; set; }
            public int Status { get; set; }
            public int Action { get; set; }
            public int ErrCode { get; set; }
            public int TaskStatus { get; set; }
            public int io_flag { get; set; }

            public CarStatus()
            {
                CarNo = "";
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
        private Dictionary<int, CarStatus> dicCars = new Dictionary<int, CarStatus>();
        private Timer tmWorkTimer = new Timer();
        private bool blRun = false;
        private DataTable dtDeviceAlarm;


        public override void Initialize(Context context)
        {
            try
            {
                dtDeviceAlarm = bll.FillDataTable("WCS.SelectDeviceAlarm", new DataParameter[] { new DataParameter("{0}", "Flag=2") });

                //获取堆垛机信息
                DataTable dt = bll.FillDataTable("CMD.SelectElevatorCar", new DataParameter[] { new DataParameter("{0}", "1=1") });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!dicCars.ContainsKey(i))
                    {
                        CarStatus carStatus = new CarStatus();
                        dicCars.Add(i, carStatus);

                        dicCars[i].CarNo = dt.Rows[i]["CarNo"].ToString();
                        dicCars[i].TaskNo = "";
                        dicCars[i].Status = int.Parse(dt.Rows[i]["State"].ToString());
                        dicCars[i].TaskStatus = 0;
                        dicCars[i].ErrCode = 0;
                        dicCars[i].Action = 0;
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
                case "TaskFinished":
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
                            string carNo = stateItem.Name.Substring(8, 2);
                            dicCars[int.Parse(carNo) - 1].TaskNo = "";

                            //只更新状态，货位信息是到拣货站台再更新
                            //bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 5), new DataParameter("@TaskNo", TaskNo) });
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
                            Util.ConvertStringChar.stringToBytes(TaskNo + PalletCode, 20).CopyTo(OutTaskNo, 0);
                            WriteToService("TranLine", "OutTaskNo2", OutTaskNo);
                            if (WriteToService("TranLine", "OutFinish2", 1))
                            {
                                bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 6), new DataParameter("@TaskNo", TaskNo) });
                            }
                        }
                    }
                    break;
                case "CarAlarmCode":
                    object obj1 = ObjectUtil.GetObject(stateItem.State);
                    if (obj1 == null)
                        return;
                    if (obj1.ToString() != "0")
                    {
                        string strError = "";
                        DataRow[] drs = dtDeviceAlarm.Select(string.Format("Flag=2 And AlarmCode={0}", obj1.ToString()));
                        if (drs.Length > 0)
                            strError = drs[0]["AlarmDesc"].ToString();
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
                        Logger.Info("穿梭车联机");
                    }
                    else
                    {
                        tmWorkTimer.Stop();
                        Logger.Info("穿梭车脱机");
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

                DataTable dt = bll.FillDataTable("CMD.SelectElevatorCar", new DataParameter[] { new DataParameter("{0}", "1=1") });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dicCars.ContainsKey(i))
                    {
                        dicCars[i].Status = int.Parse(dt.Rows[i]["CarState"].ToString());
                        string carNo = dicCars[i].CarNo;
                        if (dicCars[i].Status != 1)
                            continue;
                        //读取小车状态
                        string serviceName = "CarPLC01" + carNo;
                        object[] obj = ObjectUtil.GetObjects(WriteToService(serviceName, "CarStatus"));
                        int Layer = int.Parse(obj[3].ToString());
                        int Column = int.Parse(obj[2].ToString());
                        //如果小车空闲、在出入库站台位置，先找入库任务
                        if (Check_Car_Status_IsOk(carNo))
                        {
                            DataTable dtTask = GetTask();
                            bool IsSent = false;
                            //Column=0表示在入库站台位置
                            if (Layer == 1 && Column <= 0)
                            {

                                //先找入库任务
                                IsSent = FindInTask(dt, dtTask, carNo, obj);
                                if (IsSent)
                                    continue;
                                //再找出库任务
                                IsSent = FindOutTask(dt, dtTask, carNo, obj);
                                if (IsSent)
                                    continue;
                            }
                            else
                            {
                                //先找出库任务
                                IsSent = FindOutTask(dt, dtTask, carNo, obj);
                                if (IsSent)
                                    continue;
                                //再找入库任务
                                IsSent = FindInTask(dt, dtTask, carNo, obj);
                                if (IsSent)
                                    continue;
                            }
                            int co = int.Parse(carNo) - 1;
                            if (!IsSent && Column <= 0)
                            {
                                //小车空闲，需要避开
                                //找出其他车所在层以及目标层
                                //如果其他车都空闲，则不需要避开
                                //if(GetOtherCarNoTask(dt,carNo))
                                //    return;

                                int ToLayer = GetNoTaskLayer(dt, carNo);
                                if (ToLayer > 0)
                                {
                                    Send2PLC2(carNo, "003001" + (1000 + ToLayer).ToString().Substring(1, 3));
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error("轮询任务时出现错误:" + ex.Message);
            }
            finally
            {
                tmWorkTimer.Start();
            }
        }
        private bool FindInTask(DataTable dt, DataTable dtTask, string carNo, object[] obj)
        {            
            string filter = string.Format("TaskType in ('11','16','14') and State='2'");
            DataRow[] drs = dtTask.Select(filter, "TaskLevel DESC,RequestDate,StartDate");
            if (drs.Length > 0)
            {
                return SendInTask(dt, carNo, drs, obj);
            }
            return false;
        }
        private bool FindOutTask(DataTable dt, DataTable dtTask, string carNo, object[] obj)
        {
            int carLayer = int.Parse(obj[3].ToString());
            bool IsSendTask = false;
            string filter = string.Format("TaskType in ('12','13','14',15) and State='0' and CellRow={0}", carLayer);
            DataRow[] drs = dtTask.Select(filter, "TaskLevel DESC,RequestDate,StartDate");
            if (drs.Length > 0)
            {
                IsSendTask = SendOutTask(dt, carNo, drs, obj);
            }
            if (!IsSendTask)
            {
                //再找不在这层的出库任务
                filter = string.Format("TaskType in ('12','13','14',15) and State='0'");
                drs = dtTask.Select(filter, "TaskLevel DESC,RequestDate,StartDate");

                IsSendTask = SendOutTask(dt, carNo, drs, obj);
                
            }
            return IsSendTask;
        }
        private bool SendOutTask(DataTable dt, string carNo, DataRow[] drs, object[] obj)
        {
            int carColumn = int.Parse(obj[2].ToString());
            bool IsSend = false;
            for(int i=0;i<drs.Length;i++)
            {
                DataRow dr = drs[i];
                string FromStation = dr["FromStation"].ToString();
                int FromLayer = int.Parse(FromStation.Substring(6, 3));
                string ToStation = dr["ToStation"].ToString();
                int ToLayer = int.Parse(ToStation.Substring(6, 3));
                //轮询其他小车
                if (CheckOtherCarStatus(dt, carNo, FromLayer, ToLayer, 12, obj))
                {
                    //给小车下达任务
                    int co = int.Parse(carNo) - 1;
                    if (dr["TaskType"].ToString() == "12" || dr["TaskType"].ToString() == "14" || dr["TaskType"].ToString() == "15")
                    {
                        string HasProduct = ObjectUtil.GetObject(WriteToService("TranLine", "Conveyor11")).ToString();
                        if (HasProduct == "1")
                            break;
                    }
                    Send2PLC(dr, carNo);
                    IsSend = true;
                    dicCars[co].TaskNo = dr["TaskNo"].ToString();
                    break;
                }
            }
            return IsSend;
        }
        private bool SendInTask(DataTable dt, string carNo, DataRow[] drs, object[] obj)
        {
            int carLayer = int.Parse(obj[3].ToString());
            int carColumn = int.Parse(obj[2].ToString());
            bool IsSend = false;
            for (int i = 0; i < drs.Length; i++)
            {
                DataRow dr = drs[0];
                string FromStation = dr["FromStation"].ToString();
                int FromLayer = int.Parse(FromStation.Substring(6, 3));
                string ToStation = dr["ToStation"].ToString();
                int ToLayer = int.Parse(ToStation.Substring(6, 3));
                //轮询其他小车
                if (CheckOtherCarStatus(dt, carNo, FromLayer, ToLayer, 11, obj))
                {
                    //给小车下达任务
                    int co = int.Parse(carNo) - 1;
                    Send2PLC(dr, carNo);
                    IsSend = true;
                    dicCars[co].TaskNo = dr["TaskNo"].ToString();
                    break;
                }
            }
            return IsSend;
        }
        private bool GetOtherCarNoTask(DataTable dt, string carNo)
        {
            bool NoTask = true;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dicCars.ContainsKey(i))
                {
                    dicCars[i].Status = int.Parse(dt.Rows[i]["CarState"].ToString());
                    if (dicCars[i].Status != 1)
                        continue;

                    if (dicCars[i].CarNo != carNo)
                    {
                        //读取小车状态
                        string serviceName = "CarPLC01" + dicCars[i].CarNo;
                        object[] obj = ObjectUtil.GetObjects(WriteToService(serviceName, "CarStatus"));
                        object[] obj1 = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished"));
                        object[] obj2 = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress"));

                        int TaskFlag = int.Parse(obj1[0].ToString());
                        int Layer = int.Parse(obj[3].ToString());
                        int ToLayer = int.Parse(obj[9].ToString());
                        int TaskType = int.Parse(obj[12].ToString());
                        if (TaskFlag == 1)
                            ToLayer = int.Parse(obj2[8].ToString());

                        if (TaskType > 0 || ToLayer > 0)
                        {
                            NoTask = false;
                            break;
                        }
                    }
                }
            }
            return NoTask;
        }
       //获取小车让车可去的空闲的层
        private int GetNoTaskLayer(DataTable dt, string carNo)
        {
            int NoTaskLayer = 0;
            for(int k=1;k<11;k++)
            {
                NoTaskLayer = k;
                bool isExist = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dicCars.ContainsKey(i))
                    {
                        if (dicCars[i].CarNo != carNo)
                        {
                            //读取小车状态
                            string serviceName = "CarPLC01" + dicCars[i].CarNo;
                            object[] obj = ObjectUtil.GetObjects(WriteToService(serviceName, "CarStatus"));
                            object[] obj1 = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished"));
                            object[] obj2 = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress"));

                            int TaskFlag = int.Parse(obj1[0].ToString());
                            int Layer = int.Parse(obj[3].ToString());
                            int FromLayer = int.Parse(obj[6].ToString());
                            int FromColumn = int.Parse(obj[5].ToString());
                            int ToLayer = int.Parse(obj[9].ToString());
                            int Column = int.Parse(obj[2].ToString());
                            int ToColumn = int.Parse(obj[8].ToString());
                            int TaskType = int.Parse(obj[12].ToString());
                            if (TaskFlag == 1)
                            {
                                FromLayer = int.Parse(obj[5].ToString());
                                FromColumn = int.Parse(obj[4].ToString());
                                ToLayer = int.Parse(obj2[8].ToString());
                            }
                            if (k == 1)
                            {
                                if (FromColumn> 0 && FromLayer == k)
                                    isExist = true;
                                if (ToLayer == k && ToColumn > 0)
                                    isExist = true;
                                if (Layer == k)
                                    isExist = true;
                            }
                            else
                            {
                                if (FromLayer == k || ToLayer == k || Layer == k)
                                    isExist = true;                                
                            }
                            if (isExist)
                                break;
                        }
                    } 
                    
                }
                if (isExist)
                    continue;
                else
                    break;
            }
            return NoTaskLayer;
        }

       /// <summary>
       /// 判断能否给小车下任务
       /// </summary>
       /// <param name="dt"></param>
       /// <param name="carNo"></param>
       /// <param name="carToLayer"></param>
       /// <returns></returns>
        private bool CheckOtherCarStatus(DataTable dt, string carNo, int carFromLayer, int carToLayer, int carTaskType, object[] carobj)
        {
            int carLayer = int.Parse(carobj[3].ToString());
            int carColumn = int.Parse(carobj[2].ToString());
            bool carOK = true;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dicCars.ContainsKey(i))
                {
                    dicCars[i].Status = int.Parse(dt.Rows[i]["CarState"].ToString());
                    if (dicCars[i].Status != 1)
                        continue;
                    
                    if (dicCars[i].CarNo != carNo)
                    {
                        //读取小车状态
                        string serviceName = "CarPLC01" + dicCars[i].CarNo;

                        object[] obj3 = ObjectUtil.GetObjects(WriteToService("CarPLC0101", "CarStatus"));
                        object[] obj4 = ObjectUtil.GetObjects(WriteToService("CarPLC0102", "CarStatus"));
                        object[] obj = ObjectUtil.GetObjects(WriteToService(serviceName, "CarStatus"));
                        object[] obj1 = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished"));
                        object[] obj2 = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress"));
                        int TaskFlag = int.Parse(obj1[0].ToString());
                        int OpMode = int.Parse(obj[0].ToString());

                        int Column = int.Parse(obj[2].ToString());
                        int Layer = int.Parse(obj[3].ToString());
                        int FromColumn = int.Parse(obj[5].ToString());
                        int FromLayer = int.Parse(obj[6].ToString());
                        int ToLayer = int.Parse(obj[9].ToString());
                        int ToColumn = int.Parse(obj[8].ToString());
                        int TaskType = int.Parse(obj[12].ToString());

                        //如果出库站台有任务，不下发出库任务
                        if (TaskType == 11)
                        {
                            //string TaskNo = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(WriteToService("TranLine", "ConveyorInfo11")));
                            //if (TaskNo.Length > 0)
                            //    return false;
                        }

                        //防止读到PLC目标层不准，读到标识还没移走，那么读上位机下发的目标层
                        if (TaskFlag == 1)
                            ToLayer = int.Parse(obj2[8].ToString());
                        //Logger.Debug("小车" + dicCars[i].CarNo + "目标层" + ToLayer + "当前层:" + Layer + ",任务类型:" + TaskType);

                        //入库类型
                        if (carTaskType == 11)
                        {
                            //1入库站台不能有车
                            if (Layer == 1 && Column == 0)
                            {
                                carOK = false;
                                break;
                            }                            
                            //2其他车任务起始层也在这层，但车不在这层
                            if (carFromLayer == FromLayer && Layer>1 && TaskType > 0)
                            {
                                carOK = false;
                                break;
                            }
                            //3其他车任务目标层在起始层 出库
                            if (carFromLayer == ToLayer && carColumn>0 && TaskType==11)
                            {
                                carOK = false;
                                break;
                            }
                            //4其他车任务目标层在起始层
                            if (carFromLayer == ToLayer && carColumn > 0 && Layer > 1 && TaskType == 9)
                            {
                                carOK = false;
                                break;
                            }

                            //4目标层有车
                            if (carToLayer == Layer)
                            {
                                
                                carOK = false;
                                break;
                            }
                            //6目标层有任务起始层
                            if (carToLayer == FromLayer)
                            {
                                carOK = false;
                                break;
                            }                            
                        }
                        if (carTaskType == 12)
                        {
                            //1站台不能有车
                            if (Layer == 1 && Column == 0)
                            {
                                carOK = false;
                                break;
                            }  
                            //起始层有车
                            if (carFromLayer == Layer)
                            {
                                carOK = false;
                                break;
                            }
                            //其他车任务起始层也在这层 
                            if (carFromLayer == FromLayer && carFromLayer!=carLayer && TaskType > 0)
                            {
                                carOK = false;
                                break;
                            }
                            //其他车任务目标层在起始层 
                            if (carFromLayer == ToLayer && ToColumn>0 && TaskType > 0)
                            {
                                carOK = false;
                                break;
                            }

                            //目标层有车
                            if (carToLayer == Layer && Column==0)
                            {
                                carOK = false;
                                break;
                            }
                            
                            //目标层有任务起始层
                            //if (FromLayer == 1 && FromColumn == 0)
                            //{
                            //    carOK = false;
                            //    break;
                            //}
                        }
                        if (carTaskType == 13)
                        {
                            //1站台不能有车
                            if (Layer == 1 && Column == 0)
                            {
                                carOK = false;
                                break;
                            }
                            //起始层有车
                            if (carFromLayer == Layer)
                            {
                                carOK = false;
                                break;
                            }
                            //其他车任务起始层也在这层 
                            if (carFromLayer == FromLayer && TaskType > 0)
                            {
                                carOK = false;
                                break;
                            }
                            //其他车任务目标层在起始层 
                            if (carFromLayer == ToLayer && TaskType > 0)
                            {
                                carOK = false;
                                break;
                            }

                            //目标层有车
                            if (carToLayer == Layer)
                            {
                                carOK = false;
                                break;
                            }

                            //目标层有任务起始层
                            if (carToLayer == FromLayer && TaskType > 0)
                            {
                                carOK = false;
                                break;
                            }
                            //目标层有任务目标层
                            if (carToLayer == ToLayer && TaskType > 0)
                            {
                                carOK = false;
                                break;
                            }
                        }
                    }

                }
            }
            return carOK;
        }
        private DataTable GetTask()
        {
            DataParameter[] param = new DataParameter[] { new DataParameter("{0}", string.Format("(WCS_Task.State='2' or WCS_Task.State='0') and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02'", AreaCode)) };
            DataTable dt = bll.FillDataTable("WCS.SelectTask", param);            
            return dt;
        }
        private DataTable GetInTask()
        {
            //获取任务，排序优先等级、任务时间
            //DataParameter[] parameter = new DataParameter[] { new DataParameter("{0}", string.Format(" WCS_Task.AreaCode='{0}' ", AreaCode)) };
            //DataTable dt = bll.FillDataTable("WCS.SelectTask", parameter);
            //parameter = new DataParameter[] { new DataParameter("{0}", string.Format("((WCS_Task.TaskType in ('12','13','14','15') and WCS_Task.State='0') or (WCS_Task.TaskType in ('11','14','16') and WCS_Task.State='2')) and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' ", AreaCode)) };

            DataParameter[] param = new DataParameter[] { new DataParameter("{0}", string.Format("((WCS_Task.TaskType in ('11','14','16') and WCS_Task.State='2')) and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' ", AreaCode)) };
            
            //param = new DataParameter[] { new DataParameter("{0}", string.Format("(WCS_Task.TaskType in ('12','13','14','15') and WCS_Task.State='0') and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' and CellRow={1} ", AreaCode,FromRow)) };
            //入库
            //param = new DataParameter[] { new DataParameter("{0}", string.Format("((WCS_Task.TaskType in ('11','14','16') and WCS_Task.State='2')) and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' ", AreaCode)) };
            DataTable dt = bll.FillDataTable("WCS.SelectTask", param);
            return dt;
        }
        private DataTable GetOutTask(int FromRow)
        {
            //获取任务，排序优先等级、任务时间
            //DataParameter[] parameter = new DataParameter[] { new DataParameter("{0}", string.Format(" WCS_Task.AreaCode='{0}' ", AreaCode)) };
            //DataTable dt = bll.FillDataTable("WCS.SelectTask", parameter);
            //parameter = new DataParameter[] { new DataParameter("{0}", string.Format("((WCS_Task.TaskType in ('12','13','14','15') and WCS_Task.State='0') or (WCS_Task.TaskType in ('11','14','16') and WCS_Task.State='2')) and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' ", AreaCode)) };

            DataParameter[] param = new DataParameter[] { new DataParameter("{0}", string.Format("(WCS_Task.TaskType in ('12','13','14','15') and WCS_Task.State='0') and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' and CellRow={1} ", AreaCode, FromRow)) };
            //入库
            //param = new DataParameter[] { new DataParameter("{0}", string.Format("((WCS_Task.TaskType in ('11','14','16') and WCS_Task.State='2')) and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02' ", AreaCode)) };
            DataTable dt = bll.FillDataTable("WCS.SelectTask", param);
            if (dt.Rows.Count <= 0)
            {
                param = new DataParameter[] { new DataParameter("{0}", string.Format("(WCS_Task.TaskType in ('12','13','14','15') and WCS_Task.State='0') and WCS_Task.AreaCode='{0}' and WCS_Task.AisleNo='02'", AreaCode)) };
                dt = bll.FillDataTable("WCS.SelectTask", param);
            }
            return dt;
        }
        /// <summary>
        /// 检查小车入库状态
        /// </summary>
        /// <param name="piCrnNo"></param>
        /// <returns></returns>
        private bool Check_Car_Status_IsOk(string carNo)
        {
            try
            {
                string serviceName = "CarPLC01" + carNo;
                object[] obj = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "CarStatus"));
                object[] obj1 = ObjectUtil.GetObjects(Context.ProcessDispatcher.WriteToService(serviceName, "WriteFinished"));
                int TaskFlag = int.Parse(obj1[0].ToString());
                int CarMode = int.Parse(obj[0].ToString());
                int TaskType = int.Parse(obj[12].ToString());
                //int CraneAlarmCode = int.Parse(obj[0].ToString());

                if (CarMode == 1 && TaskType == 0 && TaskFlag == 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.Error("检查小车" + carNo + "状态时出现错误:" + ex.Message);
                return false;
            }
        }
        private void Send2PLC(DataRow dr, string carNo)
        {
            string serviceName = "CarPLC01" + carNo;
            string TaskNo = dr["TaskNo"].ToString();
            string BillID = dr["BillID"].ToString();
            string TaskType = dr["TaskType"].ToString();
            string state = dr["State"].ToString();
            int taskType = 10;
            string NextState = "3";
            if (state == "0")
            {
                if (TaskType == "13")
                {
                    taskType = 9;
                    NextState = "4";
                }
                else
                {
                    taskType = 11;
                    NextState = "4";
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

            Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
            Context.ProcessDispatcher.WriteToService(serviceName, "TaskNo", taskNo);
            if (WriteToService(serviceName, "WriteFinished", 1))
            {
                bll.ExecNonQuery("WCS.UpdateTaskTimeByTaskNo", new DataParameter[] { new DataParameter("@State", NextState), new DataParameter("@CarNo", carNo), new DataParameter("@TaskNo", TaskNo) });
                bll.ExecNonQuery("WCS.UpdateBillStateByBillID", new DataParameter[] { new DataParameter("@State", 3), new DataParameter("@BillID", BillID) });
            }
            Logger.Info("任务:" + dr["TaskNo"].ToString() + "已下发给" + carNo + "穿梭车;起始地址:" + fromStation + ",目标地址:" + toStation);
        }
        private void Send2PLC2(string carNo, string toStation)
        {
            string serviceName = "CarPLC01" + carNo;
            string TaskNo = "0";
            
            int taskType = 1;

            int[] cellAddr = new int[10];

            cellAddr[0] = 0;
            cellAddr[1] = 0;
            cellAddr[2] = 0;

            cellAddr[3] = 0;
            cellAddr[4] = 0;
            cellAddr[5] = 0;
            cellAddr[6] = byte.Parse(toStation.Substring(0, 3));
            cellAddr[7] = byte.Parse(toStation.Substring(3, 3));
            cellAddr[8] = byte.Parse(toStation.Substring(6, 3));
            
            cellAddr[9] = taskType;

            int taskNo = int.Parse(TaskNo);

            Context.ProcessDispatcher.WriteToService(serviceName, "TaskAddress", cellAddr);
            Context.ProcessDispatcher.WriteToService(serviceName, "TaskNo", taskNo);
            WriteToService(serviceName, "WriteFinished", 1);
            
            Logger.Info("任务:" + TaskNo + "已下发给" + carNo + "穿梭车,目标地址:" + toStation);
        }
    }
}