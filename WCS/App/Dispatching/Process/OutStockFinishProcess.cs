using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;

namespace App.Dispatching.Process
{
    public class OutStockFinishProcess : AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            object obj = ObjectUtil.GetObject(stateItem.State);
            if (obj == null)
                return;

            BLL.BLLBase bll = new BLL.BLLBase();
            string Request = obj.ToString();
            if (Request.Equals("True") || Request.Equals("1"))
            {
                try
                {
                    string taskNo = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(WriteToService(stateItem.Name, "Barcode")));
                    if (taskNo.Trim().Length > 0)
                    {
                        string Barcode = taskNo.PadRight(20, ' ').Substring(10, 10).Trim();

                        DataTable dt = bll.FillDataTable("WCS.SelectReadTaskByPallet", new DataParameter[] { new DataParameter("@PalletCode", Barcode) });
                        if (dt.Rows.Count > 0)
                        {
                            string TaskType = dt.Rows[0]["TaskType"].ToString();
                            string TaskNo = dt.Rows[0]["TaskNo"].ToString();
                            if (TaskType == "12" || TaskType == "15" || TaskType == "14") //出库,托盘出库,盘点
                            {
                                DataParameter[] param = new DataParameter[] { new DataParameter("@TaskNo", TaskNo) };
                                bll.ExecNonQueryTran("WCS.Sp_TaskProcess", param);
                                Logger.Info("出库任务完成,任务号:" + TaskNo + " 条码号:" + Barcode);


                                string strValue = "";
                                string[] str = new string[3];
                                if (TaskType == "12" || TaskType == "14")//显示拣货信息.
                                {
                                    str[0] = "1";
                                    if (TaskType == "14")
                                        str[0] = "2";

                                    while ((strValue = FormDialog.ShowDialog(str, dt)) != "")
                                    {
                                        break;
                                    }
                                }
                            }
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("到达出库口,错误讯息:" + ex.Message);
                }
            }
        }
    }
}
