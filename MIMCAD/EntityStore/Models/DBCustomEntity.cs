using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.DefinedEnitity;
using System;
using System.ComponentModel;

namespace EntityStore.Models
{
    public abstract class DBCustomEntity
    {
        protected void modifiyValue(Action<Entity> handler)
        {

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Entity entity = null;
            Editor ed = doc.Editor;

            PromptSelectionResult res = ed.SelectAll();

            if (res.Status == PromptStatus.Error)
            {
                return;
            }
            Autodesk.AutoCAD.EditorInput.SelectionSet SS = res.Value;
            Autodesk.AutoCAD.DatabaseServices.ObjectId[] idarray = SS.GetObjectIds();
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            using (DocumentLock docLock = doc.LockDocument())
            {
                Transaction myT = null;
                if (tm.NumberOfActiveTransactions == 0)
                {
                    myT = tm.StartTransaction();
                }
                else
                {
                    myT = tm.TopTransaction;
                }

                foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId id in idarray)
                {
                    if (id.Handle.Value == HandleValue)
                    {
                        entity = (RoadwayWrapper)myT.GetObject(id, OpenMode.ForWrite, false);
                        handler(entity);
                        break;
                    }
                }
                myT.Commit();

            }
        }

        protected void SetBasicProperty(Entity ent)
        {
            HandleValue = ent.Handle.Value;
        }
        public abstract void SetProperty(Entity ent);

        [BrowsableAttribute(false),
         DefaultValueAttribute(false)]
        public int _id { get; set; }

        [BrowsableAttribute(false),
         DefaultValueAttribute(false)]
        public long HandleValue { get; set; }
        [CategoryAttribute("基本属性"), DisplayNameAttribute("类型")]
        public virtual string Type { get { return "Entity"; } }

    }
}
