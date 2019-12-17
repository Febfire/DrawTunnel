using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using ColorBar;
using EntityStore.Controller;
using EntityStore.Models;
using MIM;
using System;
using System.Drawing;

namespace UI
{
    static class Utils
    {
        
        //添加cad对象的方法封装
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

        //通过句柄找到cad对象方法封装
        static public Entity OpenEntityByHandle(Handle handle)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            ObjectId id;
            try
            {
                id = db.GetObjectId(false, handle, 0);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                return null;
            }
            if (id == null) return null;

            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            Entity ent = null;
            TransactionControl(() =>
            {
                try
                {
                    ent = (Entity)tm.GetObject(id, OpenMode.ForWrite, false);
                }
                catch (Autodesk.AutoCAD.Runtime.Exception)
                {
                    ent = null;
                }
                
            });
            return ent;
        }

        /// <summary>
        /// cad事务操作的方法封装
        /// </summary>
        /// <param name="handler">传入的要在事务中运行的事件回调</param>
        static public void TransactionControl(Action handler)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            doc.LockDocument();
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

        /// <summary>
        /// 鼠标点击选择一个物体的方法
        /// </summary>
        /// <param name="message">选择时的提示</param>
        /// <param name="handler">选择后的事件回调</param>
        static public void SelectOne(string message, Action<Entity> handler)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityOptions options = new PromptEntityOptions(message);
            PromptEntityResult res = ed.GetEntity(options);

            if (res.Status == PromptStatus.Cancel)
                return;

            ObjectId id = res.ObjectId;

            Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            Utils.TransactionControl(() =>
            {
                Entity ent = (Entity)tm.GetObject(id, OpenMode.ForWrite, false);

                handler(ent);

            });
        }

        /// <summary>
        /// 选择试图窗口所有物体的方法
        /// </summary>
        /// <param name="type">选择类型过滤</param>
        /// <param name="handler">选择后的事件回调</param>
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

        /// <summary>
        /// 刷新cad图形对象
        /// </summary>
        /// <param name="idArray">要刷新对象的id数组</param>
        static public void ReflushViewport(ObjectId[] idArray)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            doc.LockDocument();

            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            Utils.TransactionControl(() => {
                if (idArray == null) return;

                foreach (ObjectId id in idArray)
                {
                    Entity entity = (Entity)tm.GetObject(id, OpenMode.ForWrite, false);

                    if (entity is BaseTunnel)
                    {
                        BaseTunnel tunnel = (BaseTunnel)entity;
                        tunnel.Reflesh();
                    }
                    else if (entity is Node && Global.AnimateMode == false)
                    {
                        Node node = (Node)entity;
                        node.reflesh();
                    }
                }
            });
        }

        /// <summary>
        /// 从当前项目的litedb中找到对象
        /// </summary>
        /// <param name="handleValue">要找到对象的句柄数值</param>
        /// <returns></returns>
        public static DBEntity GetEntityFromDB(long handleValue)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            DBEntity dbEntity = dbControl.FindOne(LiteDB.Query.EQ("HandleValue", handleValue));

            return dbEntity;
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        /// <param name="PaletteSet">要显示的面板</param>
        /// <param name="side">停靠位置</param>
        static public void ShowPaletteSet(Autodesk.AutoCAD.Windows.PaletteSet PaletteSet, Autodesk.AutoCAD.Windows.DockSides side)
        {
            PaletteSet.Visible = true;
            PaletteSet.Dock = side;
            PaletteSet.KeepFocus = true;
        }

        /// <summary>
        /// 32位无符号整形转化为Color对象
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 温度转为Color
        /// </summary>
        /// <param name="t"></param>
        /// <param name="maxt"></param>
        /// <returns></returns>
        static public Color temperature2Color(short t, short maxt)
        {
            int k = t * 255 / maxt ;
            ColorManager cm = new ColorManager();
            int[,] cMap = cm.CMap;
            return System.Drawing.Color.FromArgb(cMap[k, 0], cMap[k, 1], cMap[k, 2], cMap[k, 3]);
        }

        static public uint temperature2uint(short t)
        {
            Color c = temperature2Color(t, ColorBar.ColorForm.maxTemperature);
            return color2uint(c);
        }

        static public string formatDbFileName(string dir,string id)
        {
            return string.Format("{0}\\{1}.litedb",dir,id);
        }

    }
}
