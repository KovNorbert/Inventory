using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
        }
        public void SetData(string toolWorkID, string toolSerialNumber, string toolName, string toolDescription, string toolCategory, string personID, string personName, string personPosition, string mapName)
        {
            label1.Text = toolWorkID;
            label2.Text = toolSerialNumber;
            label3.Text = toolName;
            label4.Text = toolDescription;
            label5.Text = toolCategory;
            label6.Text = personID;
            label7.Text = personName;
            label8.Text = personPosition;
            label9.Text = mapName;
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
