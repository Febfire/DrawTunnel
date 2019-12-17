using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UI.Draw;
using EntityStore.Models;
using Autodesk.AutoCAD.ApplicationServices;
using System.Collections;
using EntityStore.Controller;
using Autodesk.AutoCAD.DatabaseServices;
using LiteDB;
using Autodesk.AutoCAD.EditorInput;

namespace UI.Forms
{
    public partial class GridList : DevExpress.XtraEditors.XtraUserControl
    {
        public BindingList<Tunnel> tunnels = new BindingList<Tunnel>();//GridControl绑定的数据源
        public BindingList<Tunnel> coTunnels = new BindingList<Tunnel>();//拷贝tunnels数据1,用于数据库事件
        public BindingList<Tunnel> cdTunnels = new BindingList<Tunnel>();//拷贝tunnels数据2，用于删除未生成图形的坐标
        public int clickNum;//定义添加巷道按钮的点击次数
        public ArrayList hand = new ArrayList();//用于存放巷道数目
        public ArrayList handleValue = new ArrayList();//用于存放选择的巷道的HandleValue
        public List<Entity> listEntity = new List<Entity>();

        public static bool isClick { get; set; }

        public Autodesk.AutoCAD.Windows.PaletteSet PaletteSet { get; private set; }//自定义CAD面板
        public GridList()
        {
            InitializeComponent();

            this.gridControl.DataSource = tunnels;//绑定数据源
            gridControl.RefreshDataSource();

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            dbControl.EntityInserted += dataInserted; //添加litedb数据库事件，为了litedb数据更新时同步到这个控件
            dbControl.EntityDeleted += dataDeleted;   //..删除..
            dbControl.EntityUpdated += dataUpdated;   //..修改..

            Global.SelectedTunnelChanged += tunnelSelect;//触发巷道选择事件        
        }

        //图形界面选择巷道触发事件
        private void tunnelSelect(object sender, List<DBTunnel> dtunnels)
        {
            gridView1.Appearance.Reset();
            if (dtunnels.Count > 0)
            {
                for(int i = 0; i < dtunnels.Count; i++)
                {
                    handleValue.Add(dtunnels[i].HandleValue);
                }
            }
            else
            {
                handleValue.Clear();
            }
        }

        public Autodesk.AutoCAD.Windows.PaletteSet GetPaletteSet()
        {
            return this.PaletteSet;
        }
        public void BindPaletteSet(Autodesk.AutoCAD.Windows.PaletteSet ps)
        {
            ps.Add("巷道坐标", this);
            this.PaletteSet = ps;
        }


        //点击添加巷道按钮，新建一条巷道
        private void addButtom_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridControl.DataSource = tunnels;
            clickNum = clickNum - 1;
            tunnels.Add(new Tunnel(clickNum, "巷道",0 ,0 ,0 ));
            tunnels.Add(new Tunnel(clickNum, "巷道", 100, 100, 100));
            hand.Add(clickNum);
            gridControl.RefreshDataSource();
        }

        //巷道类，作为GridControl的数据源
        public class Tunnel
        {
            public Tunnel(long handleValue, string name, double x, double y, double z)
            {
                this.HandleValue = handleValue;
                this.Name = name + handleValue.ToString();
                this.X = x;
                this.Y = y;
                this.Z = z;

            }
            public long HandleValue { get; set; }
            public string Name { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }


        }

        //添加右键菜单，在指定位置添加或删除坐标
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int rowNum = 0;
            if ((e.ClickedItem).Name == "AddItemNex")//右键插入下一点坐标
            {
                if (tunnels.Count == 0)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.
                                       ShowAlertDialog("请先添加一个巷道!");
                }
                else
                {
                    tunnels.Insert(gridView1.FocusedRowHandle + 1, new Tunnel(tunnels[gridView1.FocusedRowHandle].HandleValue, "巷道", 100, 100, 100));
                }
            }
            if ((e.ClickedItem).Name == "AddItemBef")//右键插入上一点坐标
            {
                if (tunnels.Count == 0)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.
                                        ShowAlertDialog("请先添加一个巷道!");
                }
                else
                {
                    if (gridView1.FocusedRowHandle == 0)
                    {
                        tunnels.Insert(0, new Tunnel(tunnels[gridView1.FocusedRowHandle].HandleValue, "巷道", 100, 100, 100));
                    }
                    else if (gridView1.FocusedRowHandle == 1)
                    {
                        tunnels.Insert(1, new Tunnel(tunnels[gridView1.FocusedRowHandle].HandleValue, "巷道", 100, 100, 100));
                    }
                    else
                    {
                        tunnels.Insert(gridView1.FocusedRowHandle, new Tunnel(tunnels[gridView1.FocusedRowHandle].HandleValue, "巷道", 100, 100, 100));
                    }
                }
            }
            if ((e.ClickedItem).Text == "删除坐标")//右键删除坐标
            {
                if (gridView1.RowCount > 0)
                {
                    foreach (var cot in tunnels)
                    {
                        if (tunnels[gridView1.FocusedRowHandle].HandleValue == cot.HandleValue)
                        {
                            rowNum++;
                        }
                    }
                    if (rowNum > 2)
                    {
                        deleDataRow();
                    }
                    else
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.
                    ShowAlertDialog("当前坐标不可删除！");
                        return;
                    }
                }
            }
            if ((e.ClickedItem).Text == "删除巷道")//右键删除巷道
            {
                long handle = tunnels[gridView1.FocusedRowHandle].HandleValue;
                if (handle > 0)//删除已生成图形的巷道
                {
                    Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                    DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
                    dbControl.Delete(Query.EQ("HandleValue", handle), Project.Instance.GetActiveGridList(doc));
                    doc.Editor.Regen();
                }
                else//删除未生成图形的巷道
                {
                    for(int i = 0; i < gridView1.RowCount; i++)
                    {
                        if (tunnels[i].HandleValue == tunnels[gridView1.FocusedRowHandle].HandleValue)
                        {
                            cdTunnels.Add(tunnels[i]);
                        }
                    }
                    for(int i = 0; i < cdTunnels.Count; i++)
                    {
                        tunnels.Remove(cdTunnels[i]);
                    }
                }
            }
            gridControl.RefreshDataSource();
        }


        //将HandValue隐藏
        private void gridView1_CustomColumnDisplayText_1(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == this.gridColumn1)
                {
                    string str = e.DisplayText;
                    if (e.ListSourceRowIndex >= 0)
                    {
                        e.DisplayText = str.Replace(tunnels[e.ListSourceRowIndex].HandleValue.ToString(), "");
                    }
                }
            
            }
            catch
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("操作太频繁了");
            }
        }


        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {

        }

      
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            gridView1.Appearance.Reset();        
        }

        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {

        }

        bool isInRow;//判断鼠标是否点击在行内
        //设置右键菜单点击的有效位置，点击空白处无效
        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//右键
            {
                Point pt = new Point(e.X, e.Y);
                var info = gridView1.CalcHitInfo(pt);

                if (info.RowInfo == null)
                {
                    gridControl.ContextMenuStrip = null;
                }
                else
                {
                    if(info.Column.Caption == "Name")
                    {
                        gridControl.ContextMenuStrip = contextMenuStrip1;
                        DeleteItem.Text = "删除巷道";
                        AddItemBef.Visible = false;
                        AddItemNex.Visible = false;
                    }
                    else
                    {
                        gridControl.ContextMenuStrip = contextMenuStrip1;
                        DeleteItem.Text = "删除坐标";
                        AddItemBef.Visible = true;
                        AddItemNex.Visible = true;
                    }
                }
            }
            if (e.Button == MouseButtons.Left)//左键
            {
                Point pt = new Point(e.X, e.Y);
                var info = gridView1.CalcHitInfo(pt);
                if(info.RowInfo != null)
                {          
                    isInRow = true;
                    //SelectEntity(tunnels[info.RowHandle].HandleValue);
                }
                else
                {
                    isInRow = false;
                }
            }
        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {

        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {



        }

        //选中巷道时改变背景色,选中表格时图形高亮显示
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (isInRow)//选中表格行内
                {
                    e.Appearance.Reset();
                    if (tunnels[gridView1.FocusedRowHandle].HandleValue == tunnels[e.RowHandle].HandleValue)
                    {
                        e.Appearance.BackColor = Color.SkyBlue;
                    }
                }
                else
                {
                    e.Appearance.Reset();
                }
                for (int i = 0; i < handleValue.Count; i++)
                {
                    if (tunnels[e.RowHandle].HandleValue == (long)handleValue[i])//图形界面选择巷道
                    {
                        e.Appearance.BackColor = Color.SkyBlue;
                    }
                }
            }
            catch
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("操作太频繁了");
            }
            this.gridColumn2.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumn3.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
        }

        //向CAD发送画图命令
        private void upButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.CloseEditor();
            gridControl.RefreshDataSource();
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            {
                for(int i=0;i<tunnels.Count;i++)
                {
                    if (tunnels[i].HandleValue < 0)
                    {
                        DrawStatic();
                    }
                }
            }
            //清除添加巷道点击次数
            bool isHandle = false;
            for (int i = 0; i < tunnels.Count; i++)
            {
                if (tunnels[i].HandleValue < 0)
                {
                    isHandle = true;
                }
            }
            if (isHandle)
            {
                clickNum = 0;
                hand.Clear();
            }
        }

        //数据库插入事件的回调,包含cad撤销事件
        private void dataInserted(object sender, DBEntity entity)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            if (dbControl.Senders.Contains(Project.Instance.GetActiveGridList(doc)))
            {
                return;
            }

            if (!(entity is DBTunnel)) return;

            DBTunnel dbTunnel = entity as DBTunnel;
            gridControl.RefreshDataSource();
            if (gridView1.RowCount > 0)
            {
                for (int i = 0; i < dbTunnel.BasePoints.Count; i++)
                {
                    if (dbTunnel.HandleValue != tunnels[tunnels.Count - i - 1].HandleValue)
                    {
                        tunnels.Add(new Tunnel(dbTunnel.HandleValue, dbTunnel.Name, dbTunnel.BasePoints[i].X, dbTunnel.BasePoints[i].Y, dbTunnel.BasePoints[i].Z));
                        coTunnels.Add(new Tunnel(dbTunnel.HandleValue, dbTunnel.Name, dbTunnel.BasePoints[i].X, dbTunnel.BasePoints[i].Y, dbTunnel.BasePoints[i].Z));
                    }
                }
            }
            else
            {
                for (int i = 0; i < dbTunnel.BasePoints.Count; i++)
                {
                    tunnels.Add(new Tunnel(dbTunnel.HandleValue, dbTunnel.Name, dbTunnel.BasePoints[i].X, dbTunnel.BasePoints[i].Y, dbTunnel.BasePoints[i].Z));
                    coTunnels.Add(new Tunnel(dbTunnel.HandleValue, dbTunnel.Name, dbTunnel.BasePoints[i].X, dbTunnel.BasePoints[i].Y, dbTunnel.BasePoints[i].Z));
                }
            }
            gridControl.RefreshDataSource();

        }
        //数据库删除事件的回调
        private void dataDeleted(object sender, List<DBEntity> entities)
        {
            ArrayList deleteNum = new ArrayList();
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            if (dbControl.Senders.Contains(Project.Instance.GetActiveGridList(doc)))
            {
                return;
            }
            using(DocumentLock doclock = doc.LockDocument())
            {
                foreach (var entity in entities)
                {
                    if (!(entity is DBTunnel)) continue;

                    DBTunnel dbTunnel = entity as DBTunnel;
                    gridControl.RefreshDataSource();

                    for (int i = 0; i < tunnels.Count + deleteNum.Count; i++)
                    {

                        if (tunnels[i - deleteNum.Count].HandleValue == dbTunnel.HandleValue)
                        {
                            tunnels.RemoveAt(i - deleteNum.Count);
                            deleteNum.Add(i);
                        }
                    }
                    deleteNum.Clear();
                }
                gridControl.RefreshDataSource();
            }
        }
        //数据库修改事件的回调
        private void dataUpdated(object sender, DBEntity entity)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            if (dbControl.Senders.Contains(Project.Instance.GetActiveGridList(doc)))
            {
                return;
            }
            if (!(entity is DBTunnel)) return;

            DBTunnel dbTunnel = entity as DBTunnel;
            int mm = 0;
            gridControl.RefreshDataSource();
            for (int i = 0; i < tunnels.Count; i++)
            {
                bool isBool = true;
                if (tunnels[i].HandleValue == dbTunnel.HandleValue)
                {
                    tunnels.RemoveAt(i);

                    for (int j = mm; j < dbTunnel.BasePoints.Count; j++)
                    {

                        if (isBool == true)
                        {
                            tunnels.Insert(i, new Tunnel(dbTunnel.HandleValue, dbTunnel.Name, dbTunnel.BasePoints[j].X, dbTunnel.BasePoints[j].Y, dbTunnel.BasePoints[j].Z));
                            isBool = false;
                            mm = mm + 1;
                        }
                    }

                }

            }

            gridControl.RefreshDataSource();
        }

        //坐标数据表GridList生成巷道
        public void DrawStatic()
        {
            List<DBTunnel> inList = new List<DBTunnel>();
            for (int i = 1; i <= hand.Count;i++)
            {
                bool isAddpoint = false;
                DBTunnel dbtunnel = new DBTunnel();
                dbtunnel.BasePoints = new List<DBVertice>();
                foreach (var item in tunnels)
                {
                    if (item.HandleValue == -i)
                    {
                        dbtunnel.BasePoints.Add(new DBVertice(item.X, item.Y, item.Z));
                        isAddpoint = true;
                    }
                }
                if (isAddpoint)
                {
                    inList.Add(dbtunnel);
                }
                else
                {
                    continue;
                }

            }
            DrawTunnel.StaticDrawSquareTunnel(inList);

            List<Tunnel> deleteTunnel = new List<Tunnel>();
            foreach (var tunnel in tunnels)
            {
                if (tunnel.HandleValue < 0)
                {
                    deleteTunnel.Add(tunnel);
                }
            }
            foreach (var tunnel in deleteTunnel)
            {
                tunnels.Remove(tunnel);
            }


        }

        //修改已生成巷道的表格坐标数据，图形界面同步刷新
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (tunnels[e.RowHandle].HandleValue > 0)
            {
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
                DBEntity entity = dbControl.FindOne(Query.EQ("HandleValue", tunnels[e.RowHandle].HandleValue));
                
                DBTunnel newTunnel = entity as DBTunnel;
                newTunnel.BasePoints.Clear();
                foreach (var t in tunnels)
                {
                    if (t.HandleValue == tunnels[e.RowHandle].HandleValue)
                    {
                        newTunnel.BasePoints.Add(new DBVertice(t.X, t.Y, t.Z));
                    }
                }
                dbControl.Update(newTunnel, this);
            }
        }

        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {

        }


        private void gridView1_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {

        }

        //验证输入格式
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            for(int i = 0; i < gridView1.RowCount; i++)
            {
                if (gridView1.EditingValue == null)
                {
                    e.ErrorText = "坐标不能为空！";
                    e.Valid = false;
                }
            }
            if (e.Value.ToString().Contains("."))
            {
                string str = e.Value.ToString().Substring(0, e.Value.ToString().IndexOf("."));
                if (str.Length > 8)
                {
                    e.ErrorText = "数值过大,请重新输入。";
                    e.Valid = false;
                    return;
                }
            }
            else
            {
                if (e.Value.ToString().Length > 8)
                {
                    e.ErrorText = "数值过大,请重新输入。";
                    e.Valid = false;
                    return;
                }
            }
        }

        //删除坐标，图形界面同步删除节点
        public void deleDataRow()
        {
            if (tunnels[gridView1.FocusedRowHandle].HandleValue < 0)
            {
                gridView1.DeleteRow(gridView1.FocusedRowHandle);
            }
            else
            {
                gridView1.DeleteRow(gridView1.FocusedRowHandle);
                gridControl.RefreshDataSource();
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
                DBEntity entity = dbControl.FindOne(Query.EQ("HandleValue", tunnels[gridView1.FocusedRowHandle].HandleValue));
                DBTunnel newTunnel = entity as DBTunnel;
                newTunnel.BasePoints.Clear();
                foreach (var t in tunnels)
                {
                    if (t.HandleValue == tunnels[gridView1.FocusedRowHandle].HandleValue)
                    {
                        newTunnel.BasePoints.Add(new DBVertice(t.X, t.Y, t.Z));
                    }
                }
                dbControl.Update(newTunnel, this);
            }
        }

        //输入坐标超出时隐藏
        public string getContext(string inputstring,int len)
        {
            ASCIIEncoding asci = new ASCIIEncoding();
            int temLen = 0;
            string temStr = "";
            byte[] s = asci.GetBytes(inputstring);
            for(int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    temLen += 2;
                }
                else
                {
                    temLen += 1;
                }
                try
                {
                    temStr += inputstring.Substring(i, 1);
                }
                catch
                {
                    break;
                }
                if (temLen > len)
                {
                    break;
                }
            }
            byte[] mybyte = System.Text.ASCIIEncoding.Default.GetBytes(inputstring);
            if (mybyte.Length > len)
            {
                temStr += "...";
            }
            return temStr;
        }

        public void SelectEntity(long handle)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            //Entity entity = Utils.GetEntityByHandle(new Autodesk.AutoCAD.DatabaseServices.Handle(handle));
            Editor ed = doc.Editor;
            string str = handle.ToString();
            TypedValue[] typed = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Operator,"<or"),

                new TypedValue((int)DxfCode.Operator,"<and"),

                new TypedValue((int)DxfCode.Start, "T_CylinderTunnel"),

                new TypedValue((int)DxfCode.Operator, "="),

                new TypedValue((int)DxfCode.Real, 10),

                new TypedValue((int)DxfCode.Operator,"and>"),

                new TypedValue((int)DxfCode.Operator,"or>")
            };
            SelectionFilter sf = new SelectionFilter(typed);
            PromptSelectionResult psr = ed.SelectAll(sf);
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet ss = psr.Value;

                ed.SetImpliedSelection(ss);
            }

            //Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            //Database db = doc.Database;
            //Editor ed = doc.Editor;

            //PromptSelectionResult res = ed.SelectAll();

            //SelectionSet SS = res.Value;
            //if (SS == null)
            //    return;

            //ed.SetImpliedSelection(SS);
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {

        }

        private void gridView1_RowLoaded(object sender, DevExpress.XtraGrid.Views.Base.RowEventArgs e)
        {

        }
    }
}
