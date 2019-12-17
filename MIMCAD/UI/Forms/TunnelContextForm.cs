using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using EntityStore.Controller;
using EntityStore.Models;
using LiteDB;
using MIM;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UI.Forms
{
    public partial class TunnelContextForm : Form
    {

        private List<DBTunnel> tunnelList = new List<DBTunnel>();
        private DBTunnel shownTunnel;
        public TunnelContextForm()
        {
            InitializeComponent();

            Global.ContextShown = false;

            Global.SelectedTunnelChanged += Global_SelectedTunnelChanged;
        }

        private void Global_SelectedTunnelChanged(object sender, List<DBTunnel> tunnels)
        {
            tunnelList = tunnels;
        }

        //窗口打开时
        private void TunnelContextForm_Load(object sender, EventArgs e)
        {
            display();
        }

        //窗口关闭时
        private void TunnelContextForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            clearData();
        }

        private void comboBox_ti_SelectedValueChanged(object sender, EventArgs e)
        {
            var comboxBox = sender as ComboBox;
            textBox_t.Text = shownTunnel.Temperatures[comboxBox.SelectedIndex].ToString();
        }

        //显示节点坐标
        private void comboBox_pi_SelectedValueChanged(object sender, EventArgs e)
        {
            var comboxBox = sender as ComboBox;
            textBox_px.Text = shownTunnel.BasePoints[comboxBox.SelectedIndex].X.ToString();
            textBox_py.Text = shownTunnel.BasePoints[comboxBox.SelectedIndex].Y.ToString();
            textBox_pz.Text = shownTunnel.BasePoints[comboxBox.SelectedIndex].Z.ToString();
        }

        //点击温度设置确定按钮
        private void SaveButton_Click(object sender, EventArgs e)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

            dbControl.Update(shownTunnel, this);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_px_TextChanged(object sender, EventArgs e)
        {
            position_changed(sender, "x");
        }

        private void textBox_py_TextChanged(object sender, EventArgs e)
        {
            position_changed(sender, "y");
        }

        private void textBox_pz_TextChanged(object sender, EventArgs e)
        {
            position_changed(sender, "z");
        }

        private void clearData()
        {
            tunnelList.Clear();
            Global.ContextShown = false;
        }

        //控件界面显示数据绑定
        private void display()
        {
            tunnelList = Global.SelectedTunnels;
            if (tunnelList.Count < 1) return;
            shownTunnel = tunnelList[0];
            List<short> temperature = shownTunnel.Temperatures;
            List<int> indexes1 = new List<int>();
            for (int i = 0; i < temperature.Count; i++)
            {
                indexes1.Add(i);
            }
            comboBox_ti.DataSource = indexes1;
            comboBox_ti.SelectedIndex = 0;

            List<DBVertice> points = shownTunnel.BasePoints;
            List<int> indexes2 = new List<int>();
            for (int i = 0; i < points.Count; i++)
            {
                indexes2.Add(i);
            }
            comboBox_pi.DataSource = indexes2;
            comboBox_pi.SelectedIndex = 0;
        }

        //切换节点时，切换坐标显示
        void position_changed(object sender, string v)
        {
            int index = comboBox_pi.SelectedIndex;
            TextBox textbox = sender as TextBox;
            try
            {
                int t = Convert.ToInt32(textbox.Text);

                if (t < 0 || t > ColorBar.ColorForm.maxTemperature) return;
                if (v == "x")
                    shownTunnel.BasePoints[index].X = t;
                else if (v == "y")
                    shownTunnel.BasePoints[index].Y = t;
                else if (v == "z")
                    shownTunnel.BasePoints[index].Z = t;
            }
            catch (System.FormatException)
            { }
        }

        private void textBox_t_Click(object sender, EventArgs e)
        {

        }

        //设置温度
        private void textBox_t_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox4.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox4.Visible = false;
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {

        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {
            textBox_t.EditValue = Convert.ToInt16(textBox_t.EditValue) + 1;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            textBox_t.EditValue = Convert.ToInt16(textBox_t.EditValue) - 1;
        }

        private void label5_MouseHover(object sender, EventArgs e)
        {
            label5.BackColor = System.Drawing.Color.Azure;
        }

        private void label4_MouseHover(object sender, EventArgs e)
        {
            label4.BackColor = System.Drawing.Color.Azure;
        }

        private void textBox_t_Validated(object sender, EventArgs e)
        {
            int index = comboBox_ti.SelectedIndex;
            DevExpress.XtraEditors.TextEdit textbox = sender as DevExpress.XtraEditors.TextEdit;
            try
            {
                short t = Convert.ToInt16(textbox.EditValue);

                if (t <= 0 || t >= ColorBar.ColorForm.maxTemperature) return;
                shownTunnel.Temperatures[index] = t;
                shownTunnel.Colors[index] = Utils.temperature2uint(t);
            }
            catch (System.FormatException)
            { }
        }

        private void textBox_t_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void label8_MouseHover(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {
            textBox1.Visible = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)//按下回车键
            {

            }
        }

        //设置巷道分段温度
        public void setTemperature(int segment)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityOptions options = new PromptEntityOptions("选择物体");
            PromptEntityResult res = ed.GetEntity(options);

            if (res.Status == PromptStatus.Cancel)
                return;

            Autodesk.AutoCAD.DatabaseServices.ObjectId id1 = res.ObjectId;
            Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            Utils.TransactionControl(() =>
            {
                Entity ent = (Entity)tm.GetObject(id1, OpenMode.ForWrite, false);

                if (ent is BaseTunnel)
                {
                    BaseTunnel tunnel = ent as BaseTunnel;
                }

            });
        }
    }
}
