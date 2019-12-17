using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using EntityStore.Models;
using UI.Model;
using EntityStore.Controller;
using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using MIM;
using Autodesk.AutoCAD.DatabaseServices;
using DevExpress.Utils.Menu;
using System.Collections;
using LiteDB;

[assembly: CommandClass(typeof(UI.Forms.PropertyControl))]
namespace UI.Forms
{
    public partial class PropertyControl : UserControl
    {
        public Autodesk.AutoCAD.Windows.PaletteSet PaletteSet { get; set; }
        ArrayList tunnelName = new ArrayList();//用于存放选中巷道的Name
        List<DBEntity> ListDbentity = new List<DBEntity>();
        

        public List<DBTunnel> dbt = new List<DBTunnel>();//临时存放选中巷道数据
        public PropertyControl()
        {
            InitializeComponent();
            DPTunnel dptunnel = new DPTunnel();
            Display("Tunnel", dptunnel);
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

            dbControl.EntityUpdated += dataUpdated; //添加litedb数据库事件，为了litedb数据更新时同步到这个控件
            dbControl.EntityDeleted += dataDelete;
            Global.SelectedTunnelChanged += proSelectTunnel;
            comboBoxEdit1.Text = "无选择" + "(" + dbt.Count.ToString() + ")";
        }

        //选中巷道触发事件
        private void proSelectTunnel(object sender, List<DBTunnel> dbtunnels)
        {
            comboBoxEdit1.Properties.Items.Clear();
            tunnelName.Clear();
            dbt.Clear();
            comboBoxEdit1.Text = "无选择" + "(" + dbt.Count.ToString() + ")";
            DPTunnel dptunnel = new DPTunnel();
            if (dbtunnels.Count == 0)
            {
                Display("Tunnel", dptunnel);
            }
            else
            {
                if (dbtunnels.Count == 1)
                {
                    dptunnel.SetFromDBObject(dbtunnels[0]);
                    Display("Tunnel", dptunnel);
                }
                for (int i=0;i<dbtunnels.Count;i++)
                {
                    tunnelName.Add(dbtunnels[i].Name);
                    comboBoxEdit1.Properties.Items.Add((i+1).ToString() + "." + tunnelName[i]);
                    dbt.Add(dbtunnels[i]);
                }
                comboBoxEdit1.Text = "选择巷道数" + "(" + dbt.Count.ToString() + ")";
            }
            try
            {
                propertyGridControl1.Refresh();
            }
            catch (System.NullReferenceException) { }
        }
        private void proSelectNode(object sender,List<DBNode> nodes)
        {

        }

        //删除巷道触发事件
        private void dataDelete(object sender, System.Collections.Generic.List<DBEntity> entities)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            if (dbControl.Senders.Contains(Project.Instance.GetActivePropCtl(doc)))
            {
                return;
            }

            if (propertyGridControl1.SelectedObject == null) return;
            DPTunnel dptunnel = new DPTunnel();
            foreach (var entity in entities)
            {
                if (dbt.Count > 0)
                {
                    for (int i = 0; i < dbt.Count; i++)
                    {
                        if (dbt[i].HandleValue == entity.HandleValue)
                        {
                            if (comboBoxEdit1.SelectedIndex == i)
                            {
                                Display("Tunnel", dptunnel);
                            }
                            dbt.RemoveAt(i);
                            comboBoxEdit1.Properties.Items.RemoveAt(i);                    
                        }                 
                    }
                    comboBoxEdit1.Text = "选择巷道数" + "(" + dbt.Count.ToString() + ")";
                }
                else
                {
                    comboBoxEdit1.Text = "无选择" + "(" + dbt.Count.ToString() + ")";
                    Display("Tunnel", dptunnel);
                }
                Refresh();
            }

        }

        public void BindPaletteSet(Autodesk.AutoCAD.Windows.PaletteSet ps)
        {
            ps.Add("属性控件", this);

            this.PaletteSet = ps;

        }

        public Autodesk.AutoCAD.Windows.PaletteSet GetPaletteSet()
        {
            return this.PaletteSet;
        }

        //判断选择类型为巷道还是节点
        public void Display(string type, object entity)
        {
            switch (type)
            {
                case "Tunnel":
                    DPTunnel tunnel = entity as DPTunnel;
                    propertyGridControl1.SelectedObject = entity;
                    break;
                case "Node":
                    DPNode node = entity as DPNode;
                    propertyGridControl1.SelectedObject = entity;
                    break;
                default:
                    propertyGridControl1.SelectedObject = null;
                    break;
            }


        }

        //数据库dblite修改后事件
        private void dataUpdated(object sender, DBEntity entity)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            if (dbControl.Senders.Contains(Project.Instance.GetActivePropCtl(doc)))
            {
                return;
            }

            var selEntity = this.propertyGridControl1.SelectedObject;
            if (selEntity == null)
                return;

            if (selEntity is DPNode && entity is DBNode)
            {
                if (entity.HandleValue != ((DPNode)selEntity).HandleValue) return;
                DPNode displayNode = new DPNode();
                displayNode.SetFromDBEntity(entity as DBNode);
                this.propertyGridControl1.SelectedObject = displayNode;
                this.propertyGridControl1.Refresh();
            }
            else if (selEntity is DPTunnel && entity is DBTunnel)
            {
                if (entity.HandleValue != ((DPTunnel)selEntity).HandleValue) return;
                DPTunnel displayTunnel = new DPTunnel();
                //if (displayTunnel.HandleValue != entity.HandleValue) return;
                displayTunnel.SetFromDBObject(entity as DBTunnel);
                this.propertyGridControl1.SelectedObject = displayTunnel;
                this.propertyGridControl1.Refresh();
            }
        }

        private void propertyGridControl1_RowChanged(object sender, DevExpress.XtraVerticalGrid.Events.RowChangedEventArgs e)
        {

            if (e.Row.Properties.Caption == "坐标集合")
            {

                propertyGridControl1.ExpandAllRows();
                e.Row.Properties.RowEditName = "编辑坐标";
            }
            if (e.Row.Properties.Caption == "坐标:1")
            {

            }
                  
        }

        //combox下拉选项变化触发事件
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dbt.Count > 0)
            {
                for (int i = 0; i < tunnelName.Count; i++)
                {
       
                    if (comboBoxEdit1.SelectedIndex == i)
                    {
                        Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                        DBEntityControl dbcontrol = Project.Instance.GetTmpEntCol(doc);
                        DBEntity entity = dbcontrol.FindOne(Query.EQ("HandleValue", dbt[i].HandleValue));
                        DBTunnel dbtunnel = entity as DBTunnel;
                        DPTunnel dptunnel = new DPTunnel();
                        dptunnel.SetFromDBObject(dbtunnel);
                        if (Utils.GetEntityFromDB(dbt[i].HandleValue) != null)
                        {
                            Display("Tunnel", dptunnel);
                            propertyGridControl1.Refresh();
                        }
                    }
                }
            }
            comboBoxEdit1.Refresh();
        }

    }
}
