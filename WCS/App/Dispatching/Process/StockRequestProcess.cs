using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;

namespace App.Dispatching.Process
{
    public class StockRequestProcess : AbstractProcess
    {
        BLL.BLLBase bll = new BLL.BLLBase();
        int Instation = 3;

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            object obj = ObjectUtil.GetObject(stateItem.State);
            if (obj == null)
                return;
            string TaskFinish = obj.ToString();

            if (TaskFinish.Equals("True") || TaskFinish.Equals("1"))
            {
                try
                {
                    //测试下发到达指定入库站台
                    //sbyte[] test = new sbyte[20];
                    //Util.ConvertStringChar.stringToBytes("0000000000B001", 20).CopyTo(test, 0);
                    //WriteToService("TranLine", "Barcode", test);
                    //WriteToService("TranLine", "SlideNum", Instation);
                    //if (Instation == 1)
                    //    Instation = 3;
                    //else
                    //    Instation = 1;
                    //return;
                    string taskNo = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(WriteToService(stateItem.Name, "Barcode")));
                    string Barcode = taskNo.PadRight(20, ' ').Substring(10, 10).Trim();
                    
                    sbyte[] staskNo = new sbyte[20];
                    if (Barcode == "NoRead" || Barcode.Length <= 0)
                    {
                        Util.ConvertStringChar.stringToBytes("0000000000" + Barcode, 20).CopyTo(staskNo, 0);
                        WriteToService("TranLine", "Barcode", staskNo);
                        WriteToService("TranLine", "SlideNum", 99);
                        return;
                    }
                    //判断是否还有货位可以存放

                    string F = Barcode.Substring(0, 1).ToLower();
                    string AreaCode = "002";
                    int SlideNum = 2;
                    string StationNo = "02";
                    string AisleNo = "02";
                    string CraneNo = "";

                    if (F == "c")
                    {
                        AreaCode = "003";
                        SlideNum = 3;
                        StationNo = "03";
                        AisleNo = "03";
                        CraneNo = "02";

                        DataTable dtSlide = bll.FillDataTable("WCS.SelectSlideTask");
                        if (dtSlide.Rows.Count > 0)
                        {
                            if (dtSlide.Rows[0]["StationNo"].ToString() == "03")
                            {
                                SlideNum = 1;
                                StationNo = "01";
                            }
                        }                        
                    }
                    //入库请求时,判断是否存在输送线出库的任务,如果存在,则等待出库完成!
                    //int count = bll.GetRowCount("WCS_Task", string.Format("PalletCode='{0}' and WCS_TASK.TaskType in ('12','14','15') and  WCS_TASK.State='6'", Barcode));
                    //if (count > 0)
                    //    return;


                    
                    DataParameter[] param = new DataParameter[] { new DataParameter("{0}", string.Format("PalletCode='{0}' and ((WCS_TASK.TaskType in ('11','16') and  WCS_TASK.State='0') or (WCS_TASK.TaskType='14' and  WCS_TASK.State='8'))", Barcode)) };
                    DataTable dt = bll.FillDataTable("WCS.SelectTask", param);
                    if (dt.Rows.Count > 0)
                    {
                        taskNo = dt.Rows[0]["TaskNo"].ToString();
                        //如果是盘点任务,因为盘点回原库位，所以按照库位指定入库站台


                        if (dt.Rows[0]["TaskType"].ToString() == "14" && dt.Rows[0]["State"].ToString() == "8")
                        {

                            string CellCode = dt.Rows[0]["CellCode"].ToString();

                            if (CellCode.Length > 0)
                            {
                                if (dt.Rows[0]["AisleNo"].ToString() == "03")
                                {
                                    if (CellCode.Substring(9, 1) == "2")
                                    {
                                        SlideNum = 1;
                                        StationNo = "01";
                                    }
                                    else
                                    {
                                        SlideNum = 3;
                                        StationNo = "03";
                                    }
                                }
                            }
                            else
                            {
                                Logger.Error("盘点任务货位丢失，请核对");
                                return;
                            }
                        }

                        else
                        {
                            //判断此条码有没有在库位上存在或在途
                            //if (BarcodeIsExist(Barcode,staskNo))
                            //    return;
                            //判断有没有可用货位
                            dt = bll.FillDataTable("WCS.SelectHasCell", new DataParameter[] { new DataParameter("@AreaCode", AreaCode) });
                            if (int.Parse(dt.Rows[0][0].ToString()) == 0)
                            {
                                Util.ConvertStringChar.stringToBytes("", 20).CopyTo(staskNo, 0);
                                WriteToService("TranLine", "Barcode", staskNo);
                                WriteToService("TranLine", "SlideNum", 98);
                                Logger.Error("没有空余的货位可以入库!");
                                return;
                            }

                        }
                    }
                    else
                    {
                        if (BarcodeIsExist(Barcode,staskNo))
                            return;
                        //产生空周转箱入库任务
                        param = new DataParameter[] { new DataParameter("@PalletCode", Barcode) };
                        dt = bll.FillDataTable("WCS.Sp_CreatePalletInTask", param);
                        if (dt.Rows.Count > 0)
                            taskNo = dt.Rows[0][0].ToString();
                    }
                    //盘点时可能存在问题，当只有一个货位的盘点时，
                    //并且是深度为1的货位，这时紧跟的盘点任务不能补到入库站台1，
                    //不然一起入库回不到原库位，所以可能等深度为1的上架后再下发到入库站台的任务
                    Util.ConvertStringChar.stringToBytes(taskNo + Barcode, 20).CopyTo(staskNo, 0);
                    WriteToService("TranLine", "Barcode", staskNo);
                    WriteToService("TranLine", "SlideNum", SlideNum);       
                    //更新状态
                    param = new DataParameter[] { new DataParameter("@StationNo", StationNo), new DataParameter("@AisleNo", AisleNo), new DataParameter("@AreaCode", AreaCode), new DataParameter("@CraneNo", CraneNo), new DataParameter("@TaskNo", taskNo) };
                    bll.ExecNonQuery("WCS.UpdateTaskInStockRequest", param);
                    Logger.Info("任务号:" + taskNo + " 托盘:" + Barcode + " 开始入库,去往入库口:" + SlideNum);
                }
                catch (Exception ex)
                {
                    Logger.Error("入库请求出错,错误内容:" + ex.Message);
                }
            }
        }
        private bool BarcodeIsExist(string Barcode,sbyte[] staskNo)
        {
            //判断此条码有没有在库位上存在或在途
            DataTable dt = bll.FillDataTable("WCS.PalletBarcodeExist", new DataParameter[] { new DataParameter("@Barcode", Barcode) });
            if (dt.Rows.Count > 0)
            {
                Util.ConvertStringChar.stringToBytes("", 20).CopyTo(staskNo, 0);
                WriteToService("TranLine", "Barcode", staskNo);
                WriteToService("TranLine", "SlideNum", 99);
                Logger.Error("此条码已经入库，请确认条码!");
                return true;
            }
            return false;
        }
    }
}
