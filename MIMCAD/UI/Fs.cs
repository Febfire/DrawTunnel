using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.IO;

namespace UI
{
    class Fs
    {
        /// <summary>
        /// 初始化一个新的项目文件夹
        /// </summary>
        /// <returns></returns>
        static public bool CreateProjectDir()
        {
            string rootdir = Global.CurrentProjectPath;
            string datadir = Global.CurrentProjectDataPath;

            if (!Directory.Exists(rootdir))
            {
                try
                {
                    Directory.CreateDirectory(rootdir);
                }
                catch (System.ArgumentException)
                {

                }
            }
            else
            {
                Autodesk.AutoCAD.ApplicationServices.Application.
                    ShowAlertDialog("该目录有同名文件夹，请另存为项目为其他名称，或者将同名文件夹删除。");
                return false;
            }

            if (!Directory.Exists(datadir))
            {
                Directory.CreateDirectory(datadir);
                File.SetAttributes(datadir,FileAttributes.Hidden);
            }

            return true;
        }


        static public bool FileExists(string path)
        {
            return File.Exists(path);
        }
        
        static public bool DirExists(string path)
        {
            return Directory.Exists(path);
        }
        /// <summary>
        /// 当前项目文件夹是否存在
        /// </summary>
        /// <returns></returns>
        static public bool RootDirExists()
        {
            return Directory.Exists(Project.Instance.RootPath);
        }

        static public bool DataDirExists()
        {
            return Directory.Exists(Global.CurrentProjectDataPath);
        }


        /// <summary>
        /// 创建一个新的dwg文件
        /// </summary>
        /// <param name="dir">文件夹位置</param>
        /// <param name="name">文件名</param>
        static public void createDwg(string dir, string name)
        {
            Directory.CreateDirectory(dir);

            Document tempdoc = Application.DocumentManager.Add("");
            Application.DocumentManager.MdiActiveDocument = tempdoc;
            Application.DocumentManager.CurrentDocument = tempdoc;

            Database db = tempdoc.Database;
            try
            {
                db.SaveAs(FormatDwgName(dir,name), DwgVersion.Newest);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.
                    ShowAlertDialog("文件无法创建");
                return;
            }
            
            tempdoc.LockMode(false);
            tempdoc.CloseAndDiscard();
        }

        static public string FormatDwgName(string dir,string name)
        {
            return string.Format("{0}\\{1}.dwg",dir,name);
        }

        /// <summary>
        /// 创建一个新的dwg文件
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        static public void createDwg(string fullPath)
        {
            Document tempdoc = Application.DocumentManager.Add("");
            Application.DocumentManager.MdiActiveDocument = tempdoc;
            Application.DocumentManager.CurrentDocument = tempdoc;

            Database db = tempdoc.Database;
            db.SaveAs(fullPath, DwgVersion.Newest);

            tempdoc.LockMode(false);
            tempdoc.CloseAndDiscard();
        }


        /// <summary>
        /// 打开一个dwg文件
        /// </summary>
        /// <param name="dir">文件夹位置</param>
        /// <param name="name">文件名</param>
        static public Document openDwg(string dir, string name)
        {
            string path = string.Format("{0}\\{1}.dwg",dir,name);
            if (Fs.FileExists(path))
                return Application.DocumentManager.Open(path, false);
            else
                return null;
        }

        /// <summary>
        /// 打开一个dwg文件
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        static public Document openDwg(string fullPath)
        {
            return Application.DocumentManager.Open(fullPath, false);
        }

        /// <summary>
        /// 将字符串写入文件
        /// </summary>
        /// <param name="str"></param>
        static public void WriteStr(string str, string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(str);
            sw.Close();
            fs.Close();
        }
    }
}
