using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Forms
{
    public partial class NewDwgForm : Form
    {
        public delegate void StringHandler(object sender, string str);
        public event StringHandler OK;
        public NewDwgForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = this.textBox1.Text;
            if (name == "")
            {
                Autodesk.AutoCAD.ApplicationServices.Application.
                    ShowAlertDialog("名称不能为空");
            }
            else
            {
                OK?.Invoke(this,name);
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
