using Autodesk.AutoCAD.ApplicationServices;
using EntityStore;
using EntityStore.Models;
using System.IO;

namespace UI.Initialize
{
    class InitDB
    {
        public void DoIt(Document doc)
        {
            string dbFilePath = (new PROJECT()).GetDatabasePath(doc);

            string dbTmpFilePath = (new PROJECT()).GetTmpDatabasePath(doc);
            if (File.Exists(dbFilePath))
            {               
                File.Copy(dbFilePath, dbTmpFilePath, true);
            }
            else
            {
                DbControl<DBRoadway> tmpdb = new DbControl<DBRoadway>(dbTmpFilePath, "CustomEntities");
                tmpdb.Insert(new DBRoadway());
                tmpdb.Delete(LiteDB.Query.And(LiteDB.Query.EQ("StartPoint", null), LiteDB.Query.EQ("EndPoint", null)));
            }
        }
    }
}
