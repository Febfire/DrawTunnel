using Autodesk.AutoCAD.ApplicationServices;
using EntityStore.Controller;
using EntityStore.Models;
using System.IO;

namespace UI.Initialize
{
    /// <summary>
    /// 数据库初始化
    /// </summary>
    class Database
    {
        static public void Init(Document doc)
        {           
            Project.Instance.MapProjectID(doc);

            string mainDbName = initMainCol(doc);

            DBEntityControl tmpdb = Project.Instance.GetTmpEntCol(doc);

            if (mainDbName != null)
            {
                if (File.Exists(tmpdb.Name))
                {
                    try
                    {
                        File.Delete(tmpdb.Name);
                    }
                    catch (System.IO.IOException) { }
                }

                try
                {
                    File.Copy(mainDbName, tmpdb.Name);
                }
                catch (System.IO.IOException) { }
            }
                 
        }

        static string initMainCol(Document doc)
        {
            if (!Fs.DataDirExists())
                return null;

            DBEntityControl maindb = Project.Instance.GetMainEntCol(doc,false);
            if (!File.Exists(maindb.Name))
            {
                return null;
               
            }
            else
            {
                return maindb.Name;
            }          
        }

    }
}
