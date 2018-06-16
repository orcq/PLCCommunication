using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace PLCCommunication.PLC
{
    public  class PanasonicPLC :  ModbusTCP.Master
    {
        /// <summary>
        /// PLC  ID号
        /// </summary>
        public int ID = 0;
        /// <summary>
        /// 需读取PLC地址总个数
        /// </summary>
        private int ReadAddressCount = 0;
        /// <summary>
        /// 读取PLC地址计数
        /// </summary>
        private int ReadAdress = 0;
        /// <summary>
        /// PLC写队列命令
        /// </summary>
        public Queue<PLC_CMD> QueueWrite=new Queue<PLC_CMD>();
        /// <summary>
        /// PLC读队列命令
        /// </summary
        public List<PLC_CMD> ListRead = new List<PLC_CMD>();
        /// <summary>
        /// IP地址和端口号
        /// </summary>
        private string Ip = null;
        private ushort Port = 0;

        /// <summary>
        /// 工作线程
        /// </summary>
        private bool IsExit = false;
        private Thread workThread = null;


        ModbusTCP.Master.ResponseData PLC_OnResponseData = null;

        public PanasonicPLC( string IP, ushort port,ResponseData responseData)
        {
            Ip = IP;
            Port = port;
            PLC_OnResponseData = responseData;

        }
       

        public void Start()
        {
            //连接PLC
            try
            {
                if (Ip != null && PLC_OnResponseData != null)
                {
                    connect(Ip, Port);
                    this.OnResponseData += new ModbusTCP.Master.ResponseData(PLC_OnResponseData);
                    this.OnException += new ModbusTCP.Master.ExceptionData(PLC_OnException);
                }
                else
                {
                    MessageBox.Show("PLC初始化参数有错误");
                    return;
                }
                ReadAddressCount = ListRead.Count();
            }
            catch (SystemException error)
            {
                MessageBox.Show(error.Message);
                return;
            }
            //开始工作线程
            try
            {
                workThread = new Thread(Work);
                workThread.IsBackground = true;
                workThread.Start();
            }
            catch (SystemException error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        public void Stop()
        {
            IsExit = true;
        }

        private void Work()
        {
            while (!IsExit)
            {
                try
                {
                   
                    ReadOrWrite();
                     
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Thread.Sleep(50);

            }

        }

        private void ReadOrWrite()
        {
            lock (QueueWrite)
            {
                //写队列命令为0则读取PLC地址里面的值否则往相应的地址里面写值
                if (QueueWrite.Count == 0)
                {
                    if (ReadAddressCount != 0)
                    {

                        this.ReadHoldingRegister(3, ListRead[ReadAdress].unit, ListRead[ReadAdress].StartAddress, ListRead[ReadAdress].Length);
                        ReadAdress++;
                        ReadAdress %= ReadAddressCount;

                    }
                }
                else
                {
                    while (QueueWrite.Count != 0)
                    {
                        PLC_CMD temp = QueueWrite.Dequeue();
                        this.WriteMultipleRegister(8, temp.unit, temp.StartAddress, temp.Data);
                        Thread.Sleep(30);
                    }
                }
            }           
            
        }

        private ushort ReadStartAdr(string Adress)
        {
            if (Adress.IndexOf("0x", 0, Adress.Length) == 0)
            {
                string str = Adress.Replace("0x", "");
                ushort hex = Convert.ToUInt16(str, 16);
                return hex;
            }
            else
            {
                return Convert.ToUInt16(Adress);
            }
        }

        // ------------------------------------------------------------------------
        // Modbus TCP slave exception
        // ------------------------------------------------------------------------
        private void PLC_OnException(ushort id, byte unit, byte function, byte exception)
        {
            string exc = "Modbus says error: ";
            switch (exception)
            {
                case ModbusTCP.Master.excIllegalFunction: exc += "Illegal function!"; break;
                case ModbusTCP.Master.excIllegalDataAdr: exc += "Illegal data adress!"; break;
                case ModbusTCP.Master.excIllegalDataVal: exc += "Illegal data value!"; break;
                case ModbusTCP.Master.excSlaveDeviceFailure: exc += "Slave device failure!"; break;
                case ModbusTCP.Master.excAck: exc += "Acknoledge!"; break;
                case ModbusTCP.Master.excGatePathUnavailable: exc += "Gateway path unavailbale!"; break;
                case ModbusTCP.Master.excExceptionTimeout: exc += "Slave timed out!"; break;
                case ModbusTCP.Master.excExceptionConnectionLost: exc += "Connection is lost!"; break;
                case ModbusTCP.Master.excExceptionNotConnected: exc += "Not connected!"; break;
            }

            MessageBox.Show(exc, "Modbus slave exception");
        }

    }
} 
