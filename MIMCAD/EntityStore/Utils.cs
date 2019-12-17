using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using ColorBar;
using System;
using System.Drawing;

namespace EntityStore
{
    internal static class Utils
    {
        static public void AppendEntity(Entity ent)
        {
            if (ent.ObjectId.IsNull == false)
                return;
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            TransactionControl(() =>
            {
                BlockTable bt = (BlockTable)tm.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false);
                btr.AppendEntity(ent);
                tm.AddNewlyCreatedDBObject(ent, true);
            });
        }

        static public Entity GetEntityByHandle(Handle handle)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            ObjectId id = db.GetObjectId(false, handle, 0);
            if (id == null) return null;

            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            Entity ent = null;
            TransactionControl(() =>
            {
                ent = (Entity)tm.GetObject(id, OpenMode.ForWrite, false);
            });
            return ent;
        }

        static public void TransactionControl(Action handler)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            if (tm.TopTransaction != null)
            {

                handler();
            }
            else
            {
                using (Transaction myT = tm.StartTransaction())
                {
                    handler();
                    try
                    {
                        myT.Commit();
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception exception)
                    {
                        ed.WriteMessage(exception.ToString());
                    }
                    finally
                    {
                        myT.Dispose();
                    }
                }
            }
        }

        static public void SelectAll(string type, Action<ObjectId[]> handler)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            TypedValue[] value = {
                new TypedValue((int)DxfCode.Start, type)
            };
            SelectionFilter sf = new SelectionFilter(value);
            PromptSelectionResult res = ed.SelectAll(sf);

            SelectionSet SS = res.Value;
            if (SS == null)
                return;

            ObjectId[] idArray = SS.GetObjectIds();

            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            Utils.TransactionControl(() =>
            {
                handler(idArray);
            });
        }

        static public Color uint2Color(uint n)
        {
            uint argb = n & 0xffffffff;
            uint r = (n & 0x00ff0000) >> 16;
            uint g = (n & 0x0000ff00) >> 8;
            uint b = (n & 0x000000ff);
            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        static public uint color2uint(Color color)
        {
            return ((uint)color.ToArgb()) & 0x00ffffff;
        }

        static public Color temperature2Color(short t)
        {
            int k = t * 255 / ColorBar.ColorForm.maxTemperature;
            ColorManager cm = new ColorManager();
            int[,] cMap = cm.CMap;
            return System.Drawing.Color.FromArgb(cMap[k, 0], cMap[k, 1], cMap[k, 2], cMap[k, 3]);
        }

        static public uint temperature2uint(short t)
        {
            Color c = temperature2Color(t);
            return color2uint(c);
        }
    }
}
