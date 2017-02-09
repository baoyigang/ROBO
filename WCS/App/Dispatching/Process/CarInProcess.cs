using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;

namespace App.Dispatching.Process
{
    public class CarInProcess : AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            object obj = ObjectUtil.GetObject(stateItem.State);
            if (obj == null)
                return;
            string InRequest = obj.ToString();
            if (InRequest.Equals("True") || InRequest.Equals("1"))
            {
                
                string AreaCode = "002";
                
                BLL.BLLBase bll = new BLL.BLLBase();
                DataParameter[] param;

                param = new DataParameter[] 
                { 
                    new DataParameter("@AreaCode", AreaCode) 
                };
                string TaskNo = "";
                
                DataTable dt = bll.FillDataTable("WCS.Sp_CreateCarInTask", param);
                if (dt.Rows.Count > 0)
                    TaskNo = dt.Rows[0][0].ToString();
                
                //bll.ExecNonQuery("WCS.UpdateTaskStateByTaskNo", new DataParameter[] { new DataParameter("@State", 2), new DataParameter("@TaskNo", TaskNo) });
                Logger.Info("任务号:" + TaskNo + " 已产生");
            }
        }
    }
}
