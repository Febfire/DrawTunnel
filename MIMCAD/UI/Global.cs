using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using EntityStore.Models;
using MIM;
using System;
using System.Collections.Generic;

namespace UI
{
    //作用于整个程序的全局变量
    static class Global
    {
        //自定义环境变量"ACAD_RES",该环境变量指向的路径应该为程序部署安装的路径
        static public readonly string sPath = Environment.GetEnvironmentVariable("ACAD_RES");
        //程序图片文件资源储存路径
        static public readonly string imgPath = sPath + @"\imgs\";

        //当前打开的项目的名字
        static public string CurrentProjectName { get; private set; }
        //当前打开的项目的根目录
        static public string CurrentProjectPath { get; private set; }

        static public string CurrentProjectDataPath { get; private set; }

        static public bool isOpen = false;

        static public void CreateNewProject(string path, string name)
        {
            if (!Project.CheckValiad())
            {
                Global.setProjectName(name);
                Global.setProjectPath(path);

                Fs.CreateProjectDir();

                Config config = new Config();
                config.ProjectName = Global.CurrentProjectName;
                config.Write(Global.CurrentProjectPath);
            }
        }

        static public void OpenProject(string path, string name)
        {
            if (!Project.CheckValiad())
            {
                Application.DocumentManager.CloseAll();

                if (Fs.DirExists(path))
                {
                    isOpen = true;//判断已打开工程
                    Application.DocumentManager.CloseAll();
                    Global.setProjectName(name);
                    Global.setProjectPath(path);

                    Project.CreateInstance();

                    var doc = Fs.openDwg(Project.Instance.RootPath, Global.CurrentProjectName);
                    Project.Instance.OpenedDocs.Add(doc.Name);

                    //Initialize.Ribbon.MyRibbon();//加载命令面板
                }
                else
                {
                    throw new Exception("找不到项目");
                }
            }
        }

        static public void CloseProject()
        {
            isOpen = false;
            CurrentProjectName = null;
            CurrentProjectPath = null;
            CurrentProjectDataPath = null;

            Project.CleanInstance();
        }

        /// <summary>
        /// 设置打开项目的名字
        /// </summary>
        /// <param name="name"></param>
        static private void setProjectName(string name)
        {
            CurrentProjectName = name;
        }

        /// <summary>
        /// 设置打开项目的根路径
        /// </summary>
        /// <param name="path"></param>
        static private void setProjectPath(string path)
        {
            CurrentProjectPath = path;
            CurrentProjectDataPath = path + @"\.data";
        }


        public delegate void SelectedTunnelChangedHandler(object sender, List<DBTunnel> tunnels);
        public delegate void SelectedNodeChangedHandler(object sender, List<DBNode> nodes);

        //右键上下文菜单是否打开
        static public bool ContextShown = false;

        //动画模式标志
        static public bool AnimateMode = false;

        //当前选中的对象
        static public List<DBTunnel> SelectedTunnels { get; private set; }
        static public List<DBNode> SelectedNodes { get; private set; }

        static public event SelectedTunnelChangedHandler SelectedTunnelChanged;
        static public event SelectedNodeChangedHandler SelectedNodeChanged;

        static public void EraseSelectedTunnel(object sender, long handleValue)
        {
            if (SelectedTunnels == null || SelectedTunnels.Count == 0) return;

            var et = SelectedTunnels.Find((tunnel) =>
         {
             if (tunnel.HandleValue == handleValue)
                 return true;
             else return false;
         });

            if (et != null)
            {
                SelectedTunnels.Remove(et);
            }

            SelectedTunnelChanged?.Invoke(sender, SelectedTunnels);
        }

        static public void ChangeSelection(object sender, SelectionSet set)
        {
            SelectedTunnels = new List<DBTunnel>();
            SelectedNodes = new List<DBNode>();

            if (set.Count != 0)
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor ed = doc.Editor;

                var ids = set.GetObjectIds();
                Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

                Utils.TransactionControl(() =>
                {
                    Entity entity = null;
                    foreach (var id in ids)
                    {
                        entity = (Entity)tm.GetObject(id, OpenMode.ForRead, false);
                        if (entity.IsErased == true) return;
                         long handleValue = entity.Handle.Value;
                        if (entity is BaseTunnel)
                        {
                            DBTunnel dbTunnel = Utils.GetEntityFromDB(handleValue) as DBTunnel;

                            if (dbTunnel == null) return;
                            else
                            {
                                SelectedTunnels.Add(dbTunnel);
                            }
                        }
                        else if (entity is Node)
                        {
                            DBNode dbNode = Utils.GetEntityFromDB(handleValue) as DBNode;
                            if (dbNode == null) return;
                            else
                            {
                                SelectedNodes.Add(dbNode);
                            }
                        }
                    }
                });
            }
            SelectedTunnelChanged?.Invoke(sender, SelectedTunnels);
            SelectedNodeChanged?.Invoke(sender, SelectedNodes);
        }
    }
}
