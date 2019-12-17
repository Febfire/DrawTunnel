using EntityStore.Models;
using System;
using System.Windows.Forms;

namespace UI.Forms
{
    public partial class DrawForm : Form
    {
        static private string _squareTunnel = "方形巷道";
        static private string _cylinderTunnel = "圆形巷道";
        public DrawForm()
        {
            InitializeComponent();
        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            string type = (string)this.TypeEdit.SelectedItem;

            DBTunnel tunnel = new DBTunnel();

            if (type == _squareTunnel)
            {

            }
            else if (type == _cylinderTunnel)
            {

            }

        }
    }
}
