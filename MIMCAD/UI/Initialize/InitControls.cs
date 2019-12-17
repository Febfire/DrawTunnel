using Autodesk.AutoCAD.Windows;
using UI.Forms;

namespace UI.Initialize
{
    class InitControls
    {
        public void InitPropertyControl()
        {           
            PaletteSet PS = new PaletteSet("属性");

            PS.Dock = DockSides.Left;
            PS.MinimumSize = new System.Drawing.Size(200, 600);
            PS.Size = new System.Drawing.Size(200, 600);
            PS.Visible = false;

            PROJECT.propControl = new EntityPropertyControl();
            PROJECT.propControl.BindPaletteSet(PS);
        }

        public void InitTreeList()
        {
            PaletteSet PS = new PaletteSet("场景管理");

            PS.Dock = DockSides.Left;
            PS.Location.Offset(1000, 1000);
            PS.MinimumSize = new System.Drawing.Size(200, 600);
            PS.Size = new System.Drawing.Size(200, 600);
            PS.Visible = false;

            PROJECT.treeList = new TreeList();
            PROJECT.treeList.BindPaletteSet(PS);
        }

        public void InitGridControl()
        {
            var PS = new PaletteSet("表单");
            PS.Dock = Autodesk.AutoCAD.Windows.DockSides.Bottom;
            PS.Location = new System.Drawing.Point(100, 400);
            PS.MinimumSize = new System.Drawing.Size(600, 200);
            PS.Size = new System.Drawing.Size(600, 200);
            PS.Visible = false;

            PROJECT.gridControl = new GridControl();
            PROJECT.gridControl.BindPaletteSet(PS);
        }

    }
}
