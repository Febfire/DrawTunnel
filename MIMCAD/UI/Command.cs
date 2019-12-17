using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using MIM;

[assembly: CommandClass(typeof(UI.Command))]
namespace UI
{
    class Command
    {
       
        //导出图上所有的巷道的几何信息到json文件
        [CommandMethod("OutGeoData")]
        public static void OutputGeo()
        {
            OutJson.OutputGeo();
        }

        //导出图上所有的巷道关系数据到json
        [CommandMethod("OutRLVData")]
        public static void OutputRlv()
        {
            OutJson.OutputRlv();
        }

        //打开树列表
        [CommandMethod("openTreeControl")]
        public static void OpenTreelist()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            
            var treeList = Project.Instance.TreeManager;
            if (treeList.PaletteSet == null)
            {
                Forms.BindPaletteSet.BindTreeList(out treeList);
            }           

            Project.Instance.TreeListIsShowing = true;            
            Utils.ShowPaletteSet(treeList.PaletteSet, Autodesk.AutoCAD.Windows.DockSides.Right);
        }

        //打开属性面板
        [CommandMethod("openPropertyControl")]
        public static void OpenPropertyControl()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            var propControl = Project.Instance.GetActivePropCtl(doc);
            Project.Instance.ProCtlIsShowing = true;

            Utils.ShowPaletteSet(propControl.PaletteSet, Autodesk.AutoCAD.Windows.DockSides.Left);
        }

        //打开生成巷道的表格
        [CommandMethod("openGridControl")]
        public void OpenGrid()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            var gridControl = Project.Instance.GetActiveGridList(doc);
            Project.Instance.GridListIsShowing = true;
            gridControl.PaletteSet.Visible = true;

            Utils.ShowPaletteSet(gridControl.PaletteSet, Autodesk.AutoCAD.Windows.DockSides.Bottom);
        }


        //打开巷道显示模式窗口
        [CommandMethod("DisplayMode")]
        public void displayMode()
        {
            UI.Forms.DisplayModeForm form = new Forms.DisplayModeForm();
            Application.ShowModelessDialog(form);
        }

        //动画开关
        [CommandMethod("animate")]
        public void animate()
        {
            UI.Forms.AnimateForm form = new Forms.AnimateForm();
            //Application.ShowModalDialog(form);
            Application.ShowModelessDialog(form);
        }

        //打开shp文件测试
        [CommandMethod("OpenMap")]
        public void OpenMap()
        {
            MapForm.MainForm form = new MapForm.MainForm();
            Application.ShowModelessDialog(form);
        }

        //显示颜色条
        [CommandMethod("TemperatureBar")]
        public void temperatureBar()
        {
            ColorBar.ColorForm colorForm = new ColorBar.ColorForm();
            colorForm.ShowInTaskbar = false;
            Application.ShowModelessDialog(colorForm);

        }

        [CommandMethod("CloseTunnel")]
        public void setTunnelClosed()
        {
            Utils.SelectOne("选择巷道",(entity)=> {
                if (entity is BaseTunnel)
                {
                    BaseTunnel tunnel = entity as BaseTunnel;

                    if (tunnel.Closed == false)
                        tunnel.SetClose(true);
                    else
                        tunnel.SetClose(false);
                }
            });      
        }
    }
}
