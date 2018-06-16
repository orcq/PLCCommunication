using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLCCommunication.PLC
{
    public class PLC_CMD
    {
        /// <summary>
        /// 起始地址
        /// </summary>
        public ushort StartAddress = 0;
        /// <summary>
        /// 数据长度（单位：字）
        /// </summary>
        public byte Length = 0;
        /// <summary>
        ///待写的数据
        /// </summary>
        public byte[] Data = null;
        /// <summary>
        /// 标识符
        /// </summary>
        public byte unit = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">命令类型(0读或1写)</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="length">读取长度（单位：字）</param>
        /// <param name="value">待写入的值</param>
        public PLC_CMD(int type,int startAddress,int length,int value=0)
        {
            StartAddress = ReadStartAdr(startAddress.ToString());
            if (startAddress >= 255)//标识符为八比特位无符号整数
            {
                unit = 254;
            }
            else
            {
                unit = Convert.ToByte(startAddress.ToString());
            }         
            if (type==0)//读取命令只需要起始地址与长度以及标识符
            {
                Length = Convert.ToByte(length.ToString());
                return;
            } 
            byte[] temp = BitConverter.GetBytes(value);
            Data = new byte[length*2];
            for (int i = 0; i < length*2; i++ )
            {
                Data[i] = temp[i];
            }
            TransformToPLC(ref Data);
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

        //将byte数组转化为松下PLC数据格式
        private void TransformToPLC(ref byte[] arr)
        {
            for (int i = 0; i + 1 < arr.Length; i += 2)
            {
                byte temp = arr[i + 1];
                arr[i + 1] = arr[i];
                arr[i] = temp;
            }
        }
    }
}
