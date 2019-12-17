using Autodesk.AutoCAD.ApplicationServices;

namespace UI.Initialize
{
    class Controls
    {
        public static void Init()
        {
            initPropertyControl();
            initTreeListControl();
            initGridList();
        }

        private static void initPropertyControl()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            var propControl = Project.Instance.GetActivePropCtl(doc);

            if (Project.Instance.ProCtlIsShowing == true)
            {
                Utils.ShowPaletteSet(propControl.PaletteSet, Autodesk.AutoCAD.Windows.DockSides.Left);
            }
        }

        private static void initTreeListControl()
        {
            var project = Project.Instance;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            var treeList = project.GetActiveTreeLst(doc);

            project.TreeManager.ChangeTreelist1(doc);
        }

        private static void initGridList()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            var gridControl = Project.Instance.GetActiveGridList(doc);

            if (Project.Instance.GridListIsShowing == true)
            {
                Utils.ShowPaletteSet(gridControl.PaletteSet, Autodesk.AutoCAD.Windows.DockSides.Bottom);
            }

        }
    }
}
