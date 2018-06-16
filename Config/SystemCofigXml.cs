using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace PLCCommunication.Config
{
   public static class SystemCofigXml
   {
       #region 相机一
       /// <summary>
       /// IP地址
       /// </summary>
        public static string IP
        {
            get;
            set;
        }
       /// <summary>
       /// 端口号
       /// </summary>
        public static ushort Port
        {
            get;
            set;
        }
        /// <summary>
        /// 命令类型(0读或1写)
        /// </summary>
        public static int Type
        {
            get;
            set;
        }
        /// <summary>
        /// 起始地址
        /// </summary>
        public static int StartAddress
        {
            get;
            set;
        }
        /// <summary>
        /// 读取长度
        /// </summary>
        public static int Length
        {
            get;
            set;
        }
        /// <summary>
        /// 待写入的值
        /// </summary>
        public static int Value
        {
            get;
            set;
        }
       #endregion

       public static bool LoadFile()
        {
            string strfilepath = Application.StartupPath + "\\SystemConfig.xml";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(strfilepath);
                //得到根节点
                XmlNode root = doc.SelectSingleNode("root");
                foreach(XmlNode item in root)
                {
                    XmlElement tempelement = (XmlElement)item;
                    string strname=tempelement.Name;
                    if (strname == "IP")
                    {
                        IP = tempelement.InnerText;
                    }
                    else if (strname == "Port")
                    {
                        Port = Convert.ToUInt16(tempelement.InnerText);
                    }
                    else if (strname == "Type")
                    {
                        Type = Convert.ToInt32(tempelement.InnerText);
                    }
                    else if (strname == "StartAddress")
                    {
                        StartAddress = Convert.ToInt32(tempelement.InnerText);
                    }
                    else if (strname == "Length")
                    {
                        Length = Convert.ToInt32(tempelement.InnerText);
                    }
                    else if (strname == "Value")
                    {
                        Value = Convert.ToInt32(tempelement.InnerText);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool Update(ref Dictionary<string,string> updateitem)
        {
            XmlDocument doc = new XmlDocument();
            string strfilepath = Application.StartupPath + "\\SystemConfig.xml";
            doc.Load(strfilepath);
            //得到根节点
            XmlNode root = doc.SelectSingleNode("root");
            try
            {
                foreach(KeyValuePair<string,string> item in updateitem)
                {
                    foreach(XmlNode childitem in root)
                    {
                        XmlElement templement = (XmlElement)childitem;
                        string strname = templement.Name;
                        if(strname==item.Key)
                        {
                            templement.InnerText = item.Value;
                        }
                    }
                }
                //保存
                doc.Save(strfilepath);

                //重新加载
                LoadFile();

                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            doc.Load(strfilepath);
            return true;
        }

    }
}
