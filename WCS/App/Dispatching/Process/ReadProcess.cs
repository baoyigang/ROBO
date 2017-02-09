using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;
using System.IO.Ports;

namespace App.Dispatching.Process
{
    public class ReadProcess : AbstractProcess
    {
        SerialPort comm;
        string Barcode = "";
        string PortName = "";
        int BaudRate = 0;
        bool isRead = false;

        public override void Initialize(Context context)
        {
            MCP.Config.Configuration conf = new MCP.Config.Configuration();
            conf.Load("Config.xml");
            PortName = conf.Attributes["PortName"];
            BaudRate = int.Parse(conf.Attributes["BaudRate"]);
            base.Initialize(context);            
        }
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {

            object obj = ObjectUtil.GetObject(stateItem.State);
            isRead = false;
            string TaskFinish = obj.ToString();
            if (TaskFinish.Equals("True") || TaskFinish.Equals("1"))
            {
                
                try
                {
                    comm = new SerialPort();
                    comm.PortName = PortName;
                    comm.BaudRate = BaudRate;
                    comm.DataBits = 8;
                    comm.Open();
                    comm.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

                    isRead = true;
                    Barcode = "";
                    string TaskNo = Util.ConvertStringChar.BytesToString(ObjectUtil.GetObjects(WriteToService(stateItem.Name, "ConveyorInfo04")));
                    string A4Barcode = TaskNo.PadRight(20, ' ').Substring(0, 10).Trim(); 
                    
                    while (isRead)
                    {

                        if (Barcode.Length > 0)
                            isRead = false;
                    }
                    //读取A004的任务号, 判断当前读取的条码与
                    if (A4Barcode == "" || A4Barcode != Barcode)
                    {
                        string Msg = "";
                        if (A4Barcode == "")
                            Msg = "输送线条码为空!";
                        else
                            Msg = "输送线传输出现错误,现有条码:" + Barcode + " 原有条码为:" + A4Barcode;

                        Logger.Error(Msg);
                        BLL.BLLBase bll = new BLL.BLLBase();
                        DataTable dt = bll.FillDataTable("WCS.SelectReadTaskByPallet", new DataParameter[] { new DataParameter("@PalletCode", Barcode) });
                        if (dt.Rows.Count > 0)
                            TaskNo = dt.Rows[0]["TaskNo"].ToString() + Barcode;
                        else
                            TaskNo = "0000000000" + Barcode;
                    }
                    sbyte[] taskNo = new sbyte[20];
                    Util.ConvertStringChar.stringToBytes(TaskNo, 20).CopyTo(taskNo, 0);
                    WriteToService(stateItem.Name, "Barcode", taskNo);
                    Logger.Info("条码读取成功,当前条码:" + Barcode);
                }
                catch (Exception ex)
                {
                    Logger.Error("读条码产生错误,错误内容:" + ex.Message);
                }
            }
            else
            {
                if (comm != null)
                {
                    if (comm.IsOpen)
                        comm.Close();
                    comm.Dispose();
                }
            }
        }
        private string PstBarcode = "";

        void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                do
                {
                    do
                    {
                        if (!comm.IsOpen)
                            return;
                        if (comm.BytesToRead > 0)
                            PstBarcode = PstBarcode + comm.ReadExisting();
                    }
                    while (PstBarcode.IndexOf((char)3) < 0);
                    int num1 = PstBarcode.IndexOf((char)2);
                    int num2 = PstBarcode.IndexOf((char)3);
                    if (num1 >= num2 && num1 >= 0)
                        continue;
                    Barcode = PstBarcode.Substring(num1, num2);
                    //Logger.Debug(Barcode);
                    PstBarcode = PstBarcode.Substring(PstBarcode.IndexOf((char)3) + 1);
                    Barcode = Barcode.Substring(1, Barcode.Length - 1);
                    //Logger.Debug(Barcode);
                }
                while (true);
            }
            catch (Exception ex)
            {
                Logger.Error("条码读取错误:" + ex.Message);
            }
        }         
    }
}
