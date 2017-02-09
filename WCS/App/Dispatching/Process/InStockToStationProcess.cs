using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;

namespace App.Dispatching.Process
{
    public class InStockToStationProcess : AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            object obj = ObjectUtil.GetObject(stateItem.State);
            if (obj == null)
                return;
            string TaskFinish = obj.ToString();
            if (TaskFinish.Equals("True") || TaskFinish.Equals("1"))
            {
                string ReadName = "";
                int SlideNum = 0;
                string AreaCode = "";
                string StationNo = "";
                switch (stateItem.ItemName)
                {
                    case "InFinish1":
                        ReadName = "InTaskNo1";
                        SlideNum = 1;
                        AreaCode = "003";
                        StationNo = "01";
                        break;
                    case "InFinish2":
                        ReadName = "InTaskNo2";
                        SlideNum = 2;
                        AreaCode = "002";
                        StationNo = "02";
                        break;
                    case "InFinish3":
                        ReadName = "InTaskNo3";
                        SlideNum = 3;
                        AreaCode = "003";
                        StationNo = "03";
                        break;
                }

                try
                {
                    string TaskInfo = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(WriteToService(stateItem.Name, ReadName)));
                    if (TaskInfo.Length <= 0)
                        return;
                    string TaskNo = TaskInfo.Substring(0, 10).Trim();
                    string Barcode = TaskInfo.PadRight(20, ' ').Substring(10, 10).Trim();

                    BLL.BLLBase bll = new BLL.BLLBase();

                    DataParameter[] param = new DataParameter[] { new DataParameter("@AreaCode", AreaCode), new DataParameter("@TaskNo", TaskNo), new DataParameter("@StationNo", @StationNo) };
                    bll.ExecNonQueryTran("WCS.Sp_UpdateTaskCell", param);

                    Logger.Info("任务号:" + TaskNo + " 托盘/箱号:" + Barcode + "到达入库站台:" + StationNo);
                }
                catch (Exception ex)
                {
                    Logger.Error("InStockToStationProcess出错，原因：" + ex.Message);
                }
            }
        }
    }
}
