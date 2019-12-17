using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using EntityStore.Models;
using EntityStore.Controller;
using LiteDB;
using UI.Model;
using MIM;
using System.Collections.Generic;

[assembly: CommandClass(typeof(UI.Events.EditorEvents))]
namespace UI.Events
{
    class EditorEvents
    {
        static List<Handle> selectionHandleSet = new List<Handle>();
        public void AddEvent()
        {
            Editor ed = null;
            if (Application.DocumentManager.MdiActiveDocument != null
                        && Application.DocumentManager.Count != 0
                        && Application.DocumentManager.MdiActiveDocument.Editor != null)
            {
                ed = Application.DocumentManager.MdiActiveDocument.Editor;
            }

            ed.PromptForSelectionEnding += new PromptForSelectionEndingEventHandler(callback_PromptForSelectionEnding);

        }
        /// <summary>
        /// 点击后的事件，目前有将点击物体显示到表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void callback_PromptForSelectionEnding(object sender, PromptForSelectionEndingEventArgs e)
        {
            bool eq = true;

            if ( e.Selection.Count != selectionHandleSet.Count)
                eq = false;
            else
            {
                for (int i = 0; i < e.Selection.Count; i++)
                {
                    eq = e.Selection[i].ObjectId.Handle.Equals(selectionHandleSet[i]);
                    if (!eq) break;
                }
            }

            if (eq) return;
            selectionHandleSet.Clear();
            foreach (var objectid in e.Selection.GetObjectIds())
            {
                selectionHandleSet.Add(objectid.Handle);
            }

            if (!BaseTunnel.getIsNodifying() && !BaseTunnel.getIsAnimateMode())
            {
                Global.ChangeSelection(sender, e.Selection);
            }
            else
            {
                BaseTunnel.endNodifying();
            }
        }
    }
}
