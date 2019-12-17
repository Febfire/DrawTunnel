using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;

[assembly: CommandClass(typeof(UI.Events.DocumentEvents))]
namespace UI.Events
{
    class DocumentEvents
    {
        static public void AddDocColEvent()
        {
            Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;

            Application.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;

            Application.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;

        }

        private static void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (!Project.CheckValiad())
                return;

            if (e.Document == null)
            {
                //隐藏所有的控件
                foreach (var ctl in Project.Instance.PropControls)
                {
                    ctl.Value.PaletteSet.Visible = false;
                }

                foreach (var tl in Project.Instance.ProjectTreeLists)
                {
                    tl.Value.Visible = false;
                }

                foreach (var gl in Project.Instance.GridLists)
                {
                    gl.Value.PaletteSet.Visible = false;
                }

                if (Project.Instance.TreeManager != null)
                {
                    Project.Instance.TreeManager.ChangeTreelist1(null);
                    Project.Instance.TreeManager.ShowNullDwgMessage();
                }
                    
            }
            else
            {
                //将所有非当前文档的控件隐藏
                //foreach (var ctl in Project.Instance.PropControls)
                //{
                //    if (ctl.Key != Project.Instance.GetDwgId(e.Document))
                //        ctl.Value.PaletteSet.Visible = false;

                //}

                //foreach (var gl in Project.Instance.GridLists)
                //{
                //    if (gl.Key != Project.Instance.GetDwgId(e.Document))
                //        gl.Value.PaletteSet.Visible = false;
                //}

                hideOldCol(e.Document);
                //显示当前文档的控件
                Initialize.Controls.Init();                
            }           
        }
  

        /// <summary>
        /// 文档打开时的事件
        /// </summary>
        /// <param name="senderObj"></param>
        /// <param name="e"></param>
        static private void DocumentManager_DocumentCreated(object senderObj, DocumentCollectionEventArgs e)
        {
            if (!Project.CheckValiad())
                return;

            if (e.Document != null)
            {
                hideOldCol(e.Document);
                //初始化数据库
                Initialize.Database.Init(e.Document);
                //添加自定义实体的右键菜单
                Initialize.ContextMenu.Init();

                Initialize.ProjectTree.Init();

                Initialize.Controls.Init();
                //添加自定义cad数据库事件
                Events.DatabaseEvents DE = new DatabaseEvents();
                DE.AddEvent();
                //添加自定义cad交互事件
                Events.EditorEvents EE = new EditorEvents();
                EE.AddEvent();
            }
        }

        //将所有非当前文档的控件隐藏
        static void hideOldCol(Document doc)
        {
            //将所有非当前文档的控件隐藏
            foreach (var ctl in Project.Instance.PropControls)
            {
                if (ctl.Key != Project.Instance.GetDwgId(doc))
                    ctl.Value.PaletteSet.Visible = false;

            }

            foreach (var gl in Project.Instance.GridLists)
            {
                if (gl.Key != Project.Instance.GetDwgId(doc))
                    gl.Value.PaletteSet.Visible = false;
            }
        }

        /// <summary>
        /// 文档关闭时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static private void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            if (!Project.CheckValiad())
                return;

            Project.Instance.DisposeSampleDoc(e.FileName);
            //var propertyCtl = Project.Instance.GetActivePropCtl(e.FileName);
            //if (propertyCtl != null)
            //    propertyCtl.GetPaletteSet().Visible = false;

            //var treelistCtl = Project.Instance.GetActiveTreeLst(e.FileName);
            //if (treelistCtl != null)
            //    treelistCtl.Visible = false;
            
            //var gridlistCtl = Project.Instance.GetActiveGridList(e.FileName);
            //if (gridlistCtl != null)
            //    gridlistCtl.GetPaletteSet().Visible = false;
        }

    }
}
