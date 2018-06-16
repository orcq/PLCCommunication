using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PLCCommunication.Config;

namespace PLCCommunication
{
    public partial class Form2 : DevComponents.DotNetBar.Metro.MetroAppForm
    {
        public Form2()
        {
            InitializeComponent();
            SystemCofigXml.LoadFile();
            textBoxX1.Text = SystemCofigXml.IP;
            textBoxX2.Text = SystemCofigXml.Port.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> updatitem = new Dictionary<string, string>();
            updatitem.Add("IP", textBoxX1.Text);
            updatitem.Add("Port", textBoxX2.Text);
            SystemCofigXml.Update(ref updatitem);
        }
    }
}