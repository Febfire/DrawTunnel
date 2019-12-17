using Autodesk.AutoCAD.ApplicationServices;
using EntityStore.Controller;
using EntityStore.Models;
using System.Collections.Generic;


namespace UI.Initialize
{
    /// <summary>
    /// 工程树的数据结构初始化
    /// </summary>
    class ProjectTree
    {
        public static void Init()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            DBTreeControl treeControl = Project.Instance.GetMainTreeCol(doc);

            ProjectTreeNode currentWorkingSurface;
            List<ProjectTreeNode> tree = treeControl.RebuldFromDB(out currentWorkingSurface);          
            Project.Instance.CreateProjectTree(doc,tree);
            Project.Instance.setCurrentSurface(doc,currentWorkingSurface);
        }
    }
}
