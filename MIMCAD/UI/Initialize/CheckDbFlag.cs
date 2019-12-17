using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace UI.Initialize
{
    class CheckDbFlag
    {
        public string GetDbFlag(Document doc)
        {
            //有层
            if (checkLayer(doc))
            {
                //有点
                if (checkFlag(doc)!=null)
                {
                    return checkFlag(doc);
                }
                else  //没有点
                {
                   return createFlag(doc);
                }
            }
            else   //没有层
            {
                createLayer(doc);
                return createFlag(doc);
            }

        }

        private bool checkLayer(Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction tm = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)tm.GetObject(db.LayerTableId, OpenMode.ForWrite, false);

                if (lt.Has("-735"))
                {
                    LayerTableRecord ltr = (LayerTableRecord)tm.GetObject(lt["-735"], OpenMode.ForWrite, false);
                    ltr.IsFrozen = false;

                    return true;
                }

                else
                {
                    return false;
                }

            }
        }

        private string checkFlag(Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            TypedValue[] value = {
                        new TypedValue((int)DxfCode.LayerName,"-735")
                        
                    };
            SelectionFilter sf = new SelectionFilter(value);
            PromptSelectionResult res = ed.SelectAll(sf);
            SelectionSet ss = res.Value;
            ObjectId[] idarray = ss.GetObjectIds();

            if (ss.Count == 0) return null;

            using (Transaction tm = db.TransactionManager.StartTransaction())
            {
                foreach (var id in idarray)
                {
                    Entity entity = (Entity)tm.GetObject(id, OpenMode.ForWrite, true);
                    if (entity.ExtensionDictionary.IsNull) continue;
                    
                    DBDictionary extensionDic = (DBDictionary)tm.GetObject(entity.ExtensionDictionary,OpenMode.ForRead,false);
                    DataTable dt = (DataTable)tm.GetObject(extensionDic.GetAt("database flag"),OpenMode.ForRead,false);
                    if ((bool)dt.GetCellAt(0, 0).Value)
                    {
                        string databaseName = (string)dt.GetCellAt(0, 1).Value;
                        //锁定，冻结图层
                        LayerTable lt = (LayerTable)tm.GetObject(db.LayerTableId, OpenMode.ForRead, false);
                        LayerTableRecord ltr = (LayerTableRecord)tm.GetObject(lt["-735"], OpenMode.ForWrite,false);
                        ltr.IsFrozen = true;

                        return databaseName;
                    }
                    return null;
                }
            }
            return null;
        }

        private void createLayer(Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction tm = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)tm.GetObject(db.LayerTableId, OpenMode.ForWrite, false);
                LayerTableRecord ltr = new LayerTableRecord();
                ltr.Name = "-735";
                lt.Add(ltr);
                tm.AddNewlyCreatedDBObject(ltr, true);
                tm.Commit();
            }
        }

        private string createFlag(Document doc)
        {
            string dbnamestr = null;
            Database db = doc.Database;
            DBPoint flag = null;
            //创建点
            using (Transaction tm = db.TransactionManager.StartTransaction())
            {
                flag = new DBPoint(new Point3d(0, 0, 0));
                flag.Layer = "-735";
                flag.Visible = false;

                BlockTable bt = (BlockTable)tm.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false);
                btr.AppendEntity(flag);

                tm.AddNewlyCreatedDBObject(flag, true);
                tm.Commit();
            }

            ////扩展数据
            Guid guid = Guid.NewGuid();
            DataTable dt = new DataTable();
            dt.TableName = "database flag";
            dt.AppendColumn(CellType.Bool, "hasDatabase");
            dt.AppendColumn(CellType.CharPtr, "databaseName");
            DataCellCollection row = new DataCellCollection();
            DataCell hasDatabase = new DataCell();
            DataCell databaseName = new DataCell();
            hasDatabase.SetBool(true);
            dbnamestr = guid.ToString();
            databaseName.SetString(dbnamestr);
            row.Add(hasDatabase);
            row.Add(databaseName);
            dt.AppendRow(row, true);

            using (Transaction tm = db.TransactionManager.StartTransaction())
            {
                Entity entity = (Entity)tm.GetObject(flag.ObjectId, OpenMode.ForWrite, false);

                if (entity.ExtensionDictionary == new ObjectId())
                    entity.CreateExtensionDictionary();

                DBDictionary extensionDic = (DBDictionary)tm.GetObject(flag.ExtensionDictionary, OpenMode.ForWrite, false);
                extensionDic.SetAt("database flag", dt);
                tm.AddNewlyCreatedDBObject(dt,true);

                ////锁定，冻结图层
                LayerTable lt = (LayerTable)tm.GetObject(db.LayerTableId, OpenMode.ForRead, false);
                LayerTableRecord ltr = (LayerTableRecord)tm.GetObject(lt["-735"], OpenMode.ForWrite, false);
                ltr.IsFrozen = true;

                tm.Commit();

            }
            return dbnamestr;
        }
    }
}
