using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PLCCommunication.PLC;
using PLCCommunication.Config;

namespace PLCCommunication
{
    public partial class Form1 : DevComponents.DotNetBar.Metro.MetroAppForm
    {
        private PanasonicPLC panasonicPLC;

        public Form1()
        {
            InitializeComponent();
            SystemCofigXml.LoadFile();
            textBoxX4.Text = SystemCofigXml.Type.ToString();
            textBoxX5.Text = SystemCofigXml.StartAddress.ToString();
            textBoxX6.Text = SystemCofigXml.Length.ToString();
            textBoxX2.Text = SystemCofigXml.Value.ToString();
        }

        private void metroShell1_SettingsButtonClick(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SystemCofigXml.LoadFile();
                panasonicPLC = new PanasonicPLC(SystemCofigXml.IP, SystemCofigXml.Port, PLC_OnResponseData);
                panasonicPLC.Start();
            }
            catch (SystemException error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void PLC_OnResponseData(ushort ID, byte unit, byte function, byte[] values)
        {
            //Ð´PLC·´À¡
            if (ID == 8)
            {
                return;
            }
            //¶ÁPLC·´À¡
            else if (ID == 3)
            {
                Getbyte(ref values);
                short Value = BitConverter.ToInt16(values, 0);
                textBoxX1.Text += Value.ToString() + " ";
            }
        }

        private void Getbyte(ref byte[] bytebuffer)
        {
            for (int i = 0, j = bytebuffer.Length - 1; i < j; i++, j--)
            {
                byte tem = bytebuffer[i];
                bytebuffer[i] = bytebuffer[j];
                bytebuffer[j] = tem;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (panasonicPLC != null)
                {
                    panasonicPLC.Stop();
                }
            }
            catch
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> updatitem = new Dictionary<string, string>();
            updatitem.Add("Type", textBoxX4.Text);
            updatitem.Add("StartAddress", textBoxX5.Text);
            updatitem.Add("Length", textBoxX6.Text);
            updatitem.Add("Value", textBoxX2.Text);
            SystemCofigXml.Update(ref updatitem);
            PLC_CMD plc_cmd = new PLC_CMD(Convert.ToInt32(textBoxX4.Text), Convert.ToInt32(textBoxX5.Text), Convert.ToInt32(textBoxX6.Text));
            panasonicPLC.ReadHoldingRegister(3, plc_cmd.unit, plc_cmd.StartAddress, plc_cmd.Length);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> updatitem = new Dictionary<string, string>();
            updatitem.Add("Type", textBoxX4.Text);
            updatitem.Add("StartAddress", textBoxX5.Text);
            updatitem.Add("Length", textBoxX6.Text);
            updatitem.Add("Value", textBoxX2.Text);
            SystemCofigXml.Update(ref updatitem);
            PLC_CMD plc_cmd = new PLC_CMD(Convert.ToInt32(textBoxX4.Text), Convert.ToInt32(textBoxX5.Text), Convert.ToInt32(textBoxX6.Text), Convert.ToInt32(textBoxX2.Text));
            panasonicPLC.WriteMultipleRegister(8, plc_cmd.unit, plc_cmd.StartAddress, plc_cmd.Data);
        }

        private void metroShell1_HelpButtonClick(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "http://www.blue-skyauto.com");
        }   
    }
}