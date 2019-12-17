using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using UI.Jig;
using MIM;

[assembly: CommandClass(typeof(UI.Draw.DrawTag))]
namespace UI.Draw
{
    class DrawTag
    {
        static public void DoIt()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //输入起点坐标
            PromptPointOptions opts = new PromptPointOptions("\nEnter Tunnel Start Point:");
            PromptPointResult res = ed.GetPoint(opts);

            if (res.Status == PromptStatus.Cancel)
                return;

            Point3d tmpPoint = res.Value;

            TagJig jig = new TagJig(tmpPoint, tmpPoint, tmpPoint);

            jig.setPromptCounter(0);
            PromptResult drag = ed.Drag(jig);

            jig.setPromptCounter(1);
            drag = ed.Drag(jig);

            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            var stdLn = (Tag)jig.GetEntity();
            stdLn.Text = "标注测试文字";

            Utils.AppendEntity(stdLn);
            stdLn.Dispose();
        }
    }
}
