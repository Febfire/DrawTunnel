using Autodesk.AutoCAD.DatabaseServices;
using MIM;
using System;
using System.Windows.Forms;

namespace UI.Forms
{
    public partial class AnimateForm : Form
    {
        private ObjectId[] idarray;
        public AnimateForm()
        {
            InitializeComponent();
            timer1.Interval = 15;
            timer1.Tick += new EventHandler(arrowMove);//添加事件
            ShowInTaskbar = false;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (Global.AnimateMode == false)
            {
                selectAll();
            }
            timer1.Enabled = true;
            Global.AnimateMode = true;
        }

        private void EndButton_Click(object sender, EventArgs e)
        {
            stop();
            this.Close();
        }

        private void AnimateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            stop();
        }
        private void selectAll()
        {
            Utils.SelectAll("TUNNEL_SQUARE,TUNNEL_CYLINDER,TUNNELNODE", (idArray) => {
                idarray = idArray;
            });
        }

        private void arrowMove(object sender, EventArgs e)
        {
            BaseTunnel.startAnimateMode();
            Utils.ReflushViewport(idarray);
        }
        private void stop()
        {
            timer1.Enabled = false;
            BaseTunnel.endAnimateMode();
            Utils.ReflushViewport(idarray);
            Global.AnimateMode = false;
        }
    }
}
