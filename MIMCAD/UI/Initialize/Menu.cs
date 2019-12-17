using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Customization;
using System.Collections.Specialized;
using Autodesk.AutoCAD.ApplicationServices;
using System;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Windows;
using System.IO;
using System.Runtime.InteropServices;

[assembly: CommandClass(typeof(UI.Initialize.Menu))]
namespace UI.Initialize
{
    class Menu
    {
        public static void Init()
        {
            addMenu();
        }
        [CommandMethod("openmenu")]
        public static void addMenu()
        {
            string myGroupName = "myGroup";
            string myCuiName = "myMenu.cui";
            CustomizationSection myCsection = new CustomizationSection();
            myCsection.MenuGroupName = myGroupName;

            MacroGroup mg = new MacroGroup("myMethod", myCsection.MenuGroup);
            MenuMacro mm1 = new MenuMacro(mg, "新建文件", "createform ", "");
            MenuMacro mm2 = new MenuMacro(mg, "打开文件", "openfile ", "");
            MenuMacro mm3 = new MenuMacro(mg, "打开文件", "openribbon ", "");
            //声明菜单别名
            StringCollection scMyMenuAlias = new StringCollection();
            scMyMenuAlias.Add("MyPop1");
            scMyMenuAlias.Add("MyTestPop");

            //菜单项（将显示在项部菜单栏中）
            PopMenu pmParent = new PopMenu("矿山平台", scMyMenuAlias, "我的菜单", myCsection.MenuGroup);


            //子项的菜单（多级）
            PopMenu pm1 = new PopMenu("文件", new StringCollection(), "", myCsection.MenuGroup);
            PopMenuRef pmr1 = new PopMenuRef(pm1, pmParent, -1);
            PopMenuItem pmi1 = new PopMenuItem(mm1, "新建", pm1, -1);
            PopMenuItem pmi2 = new PopMenuItem(mm2, "打开", pm1, -1);

            ////子项的菜单（单级）
            PopMenuItem pmi3 = new PopMenuItem(mm3, "命令", pmParent, -1);

            bool k = myCsection.SaveAs(Global.sPath + @"\" + myCuiName);

            //AcadApplication app = (AcadApplication)Marshal.GetActiveObject("AutoCAD.Application.22");
            //AcadMenuBar menuBar = app.MenuBar;
            //AcadMenuGroup menuGroup = app.MenuGroups.Item(0);
            //AcadPopupMenus menus = menuGroup.Menus;
            //AcadPopupMenu mymenu = menus.Add("MyMenu");

            //mymenu.AddMenuItem(0, "Hello", "hello");
            //mymenu.AddSeparator(1);
            //mymenu.AddMenuItem(2, "Hello2", "hello");
            //AcadPopupMenu ext = mymenu.AddSubMenu(3, "Ext");
            //ext.AddMenuItem(0, "Hello", "hello");
            //ext.AddSeparator(1);
            //ext.AddMenuItem(2, "Hello2", "hello");
            //mymenu.InsertInMenuBar(menuBar.Count - 2);
        }
     
    }
   
}

