using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Forms
{
    static class BindPaletteSet
    {
        static public void BindPropertyControl(out PropertyControl propControl)
        {
            PaletteSet PS = new PaletteSet("属性面板");
            PS.DockEnabled = DockSides.Left;
            PS.MinimumSize = new System.Drawing.Size(250, 600);
            PS.Size = new System.Drawing.Size(250, 600);
            PS.Visible = false;

            propControl = new PropertyControl();
            propControl.BindPaletteSet(PS);
        }


        static public void BindTreeList(out TreeManager treeManager)
        {
            PaletteSet PS = new PaletteSet("项目资源管理器");
            PS.DockEnabled = DockSides.Right;
            PS.Location.Offset(1000, 1000);
            PS.MinimumSize = new System.Drawing.Size(200, 600);
            PS.Size = new System.Drawing.Size(200, 600);
            PS.Visible = false;

            treeManager = Project.Instance.TreeManager;
            treeManager.BindPaletteSet(PS);

        }

        static public void BindGridList(out GridList gridControl)
        {
            var PS = new PaletteSet("表单");
            PS.Location = new System.Drawing.Point(10, 400);
            PS.MinimumSize = new System.Drawing.Size(2, 1);
            PS.Size = new System.Drawing.Size(2, 1);
            PS.Visible = false;
            PS.Dock = Autodesk.AutoCAD.Windows.DockSides.Bottom;

            gridControl = new GridList();
            gridControl.BindPaletteSet(PS);

        }
    }
}
