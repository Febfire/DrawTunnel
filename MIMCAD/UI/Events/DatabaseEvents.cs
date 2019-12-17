using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using EntityStore.Controller;
using EntityStore.Models;
using LiteDB;
using MIM;
using System.Collections.Generic;

[assembly: CommandClass(typeof(UI.Events.DatabaseEvents))]
namespace UI.Events
{
    /// <summary>
    /// cad的数据库事件
    /// </summary>
    class DatabaseEvents
    {
        private object modifyMutex = new object();
        public void AddEvent()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = HostApplicationServices.WorkingDatabase;

            db.SaveComplete += db_SaveComplete;

            db.ObjectErased += db_ObjectErased;
            db.ObjectAppended += db_ObjectAppend;
            db.ObjectModified += db_ObjectModified;

            db.ObjectOpenedForModify += Db_ObjectOpenedForModify;

            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            dbControl.EntityUpdated += dataUpdated;  //添加litedb数据库事件，为了litedb数据更新时同步到cad数据库
            dbControl.EntityDeleted += dataDeleted;
        }

        private void Db_ObjectOpenedForModify(object sender, ObjectEventArgs e)
        {
            if (e.DBObject is BaseTunnel)
            {
                BaseTunnel tunnel = e.DBObject as BaseTunnel;

                Document doc = Application.DocumentManager.MdiActiveDocument;
                DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

                DBEntity oldTunnel = dbControl.FindOne
                    (Query.EQ("HandleValue", e.DBObject.ObjectId.Handle.Value));
                if (oldTunnel != null && tunnel.Location != oldTunnel.Location)
                    tunnel.Location = oldTunnel.Location;
            }

        }

        /// <summary>
        /// 保存事件使数据库与图纸中的已保存数据相一致
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void db_SaveComplete(object sender, DatabaseIOEventArgs e)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            //保存树结构
            DBTreeControl treeControl = Project.Instance.GetMainTreeCol(doc);
            treeControl.StoreTree(Project.Instance.GetProjectTree(doc));

            //保存模型数据
            DBEntityControl dbControl = Project.Instance.GetMainEntCol(doc, true);
            TypedValue[] value = { new TypedValue((int)DxfCode.Start, "TUNNEL_SQUARE,TUNNEL_CYLINDER,TUNNELNODE") };
            SelectionFilter sf = new SelectionFilter(value);
            PromptSelectionResult res = ed.SelectAll(sf);
            if (res.Status != PromptStatus.OK)
            {
                dbControl.Delete(Query.All(), db);
                return;
            }
            SelectionSet SS = res.Value;
            if (SS == null)
                return;

            Autodesk.AutoCAD.DatabaseServices.ObjectId[] idArray = SS.GetObjectIds();

            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            using (Transaction myT = tm.StartTransaction())
            {
                //储存之前先把之前数据的删除干净
                int deleteCounts = dbControl.Delete(Query.All(), db);

                foreach (var id in idArray)
                {
                    Entity entity = (Entity)tm.GetObject(id, OpenMode.ForRead, true);
                    if (entity is BaseTunnel)
                    {
                        DBTunnel dbTunnel = new DBTunnel();
                        dbTunnel.SetProperty(entity);
                        dbControl.Insert(dbTunnel, db);
                    }
                    else if (entity is Node)
                    {
                        DBNode dbNode = new DBNode();
                        dbNode.SetProperty(entity);
                        dbControl.Insert(dbNode, db);
                    }
                }
                myT.Commit();
            }
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void db_ObjectErased(object sender, ObjectErasedEventArgs e)
        {
            if (!(e.DBObject is BaseTunnel) && !(e.DBObject is Node))
                return;

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            if (e.Erased == true)   //这时候是删除触发的
            {
                DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

                dbControl.Delete(Query.EQ("HandleValue", e.DBObject.ObjectId.Handle.Value), db);

                if (e.DBObject is BaseTunnel)
                    Global.EraseSelectedTunnel(sender, e.DBObject.ObjectId.Handle.Value);
            }
            else             //这时候是删除后的撤销动作触发的
            {
                Entity entity = e.DBObject as Entity;
                if (entity is BaseTunnel)
                {
                    DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

                    DBTunnel dbTunnel = new DBTunnel();
                    dbTunnel.SetProperty(entity);
                    dbControl.Insert(dbTunnel, db);

                }
                else if (entity is Node)
                {
                    DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
                    DBNode dbNode = new DBNode();
                    dbNode.SetProperty(entity);
                    dbControl.Insert(dbNode, db);
                }
            }
        }

        /// <summary>
        /// 添加事件使临时数据库与图纸中所有效的数据相一致
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void db_ObjectAppend(object sender, ObjectEventArgs e)
        {
            if (!(e.DBObject is BaseTunnel) && !(e.DBObject is Node))
                return;

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Entity entity = e.DBObject as Entity;

            if (entity == null || entity.BlockName != "*Model_Space")
                return;

            if (entity is BaseTunnel)
            {
                DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

                DBTunnel dbTunnel = new DBTunnel();
                dbTunnel.SetProperty(entity);
                dbControl.Insert(dbTunnel, db);

            }
            else if (entity is Node)
            {
                DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
                DBNode dbNode = new DBNode();
                dbNode.SetProperty(entity);
                dbControl.Insert(dbNode, db);
            }
        }

        /// <summary>
        /// 修改事件使临时数据库与图纸窗口中有效的数据相一致
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void db_ObjectModified(object sender, ObjectEventArgs e)
        {
            //  if (modifyGraph == true) return;

            if (Global.AnimateMode == true) return;

            Entity entity = e.DBObject as Entity;

            if (!(entity is BaseTunnel) && !(entity is Node))
                return;

            try
            {
                if (entity == null || entity.BlockName != "*Model_Space")
                    return;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                return;
            }

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            if (e.DBObject is BaseTunnel)
            {

                DBTunnel newTunnel = new DBTunnel();
                newTunnel.SetProperty((Entity)(e.DBObject));

                DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

                DBEntity oldTunnel = dbControl.FindOne
                    (Query.EQ("HandleValue", e.DBObject.ObjectId.Handle.Value));
                if (oldTunnel == null)
                    return;
                newTunnel._id = oldTunnel._id;

                lock (modifyMutex)
                {
                    dbControl.Update(newTunnel, db);
                }

            }
            else if (e.DBObject is Node)
            {

                DBNode newNode = new DBNode();
                newNode.SetProperty((Entity)(e.DBObject));

                DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

                DBEntity oldTunnel = dbControl.FindOne
                    (Query.EQ("HandleValue", e.DBObject.ObjectId.Handle.Value));
                if (oldTunnel == null)
                    return;
                newNode._id = oldTunnel._id;
                lock (modifyMutex)
                {
                    dbControl.Update(newNode, db);
                }

            }

        }

        private void dataUpdated(object sender, DBEntity dbEntity)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

            if (dbControl.Senders.Contains(db)) return;

            Handle handle = new Handle(dbEntity.HandleValue);

            using (DocumentLock docLock = doc.LockDocument())
            {
                Utils.TransactionControl(() =>
                {

                    Entity ent = Utils.OpenEntityByHandle(handle);

                    if (ent is Node)
                    {
                        Node node = ent as Node;
                        DBNode dbNode = dbEntity as DBNode;

                        dbNode.ToCADObject(node);

                    }
                    else if (ent is BaseTunnel)
                    {
                        BaseTunnel tunnel = ent as BaseTunnel;
                        DBTunnel dbTunnel = dbEntity as DBTunnel;

                        dbTunnel.ToCADObject(tunnel);
                    }
                });
            }
        }
        private void dataDeleted(object sender, List<DBEntity> entities)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);

            if (dbControl.Senders.Contains(db)) return;

            using (DocumentLock docLock = doc.LockDocument())
            {
                Utils.TransactionControl(() =>
                {
                    foreach (var entity in entities)
                    {
                        Handle handle = new Handle(entity.HandleValue);
                        Entity ent = Utils.OpenEntityByHandle(handle);
                        if (ent != null)
                            ent.Erase();
                    }
                });
            }
        }
    }
}
