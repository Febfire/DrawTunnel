using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;


namespace EntityStore.Controller
{
    public class CheckDBFlag
    {
       static public string GetDbFlag(Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            string uuid = null;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            Utils.TransactionControl(() =>
            {
                TypedValue[] value = { new TypedValue((int)DxfCode.Start, "DWGMARK") };
                SelectionFilter sf = new SelectionFilter(value);
                PromptSelectionResult res = ed.SelectAll(sf);
                if (res.Status == PromptStatus.OK)
                {
                    SelectionSet SS = res.Value;
                    var idArray = SS.GetObjectIds();

                    foreach (var id in idArray)
                    {
                        Entity entity = (Entity)tm.GetObject(id, OpenMode.ForRead, true);
                        if (entity is MIM.Mark)
                        {
                            var mark = entity as MIM.Mark;
                            uuid = mark.Uuid;
                            return;
                        }
                    }
                }

                Guid guid = Guid.NewGuid();
                MIM.Mark newMark = new MIM.Mark();
                uuid = guid.ToString();
                newMark.Uuid = uuid;
                BlockTable bt = (BlockTable)tm.GetObject(db.BlockTableId, OpenMode.ForRead);
                using (doc.LockDocument())
                {
                    BlockTableRecord btr = (BlockTableRecord)tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false);
                    btr.AppendEntity(newMark);
                    tm.AddNewlyCreatedDBObject(newMark, true);
                }                
            });
            return uuid;
        }
    }
}

