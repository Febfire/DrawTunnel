using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using MIM;
using System;
using UI.Forms;

namespace UI.Initialize
{
    class ContextMenu
    {
        static bool isSet = false;
        public static void Init()
        {
            if (isSet)
                return;
            isSet = true;
            NodeContextMenu();
            TunnelContextMenu();
        }
        static void NodeContextMenu()
        {
            ContextMenuExtension contextMenu =
                                        new ContextMenuExtension();

            MenuItem item0 = new MenuItem("节点详细属性");

            contextMenu.MenuItems.Add(item0);

            item0.Click += new EventHandler(tunnelClickHandler);


            //Application.AddObjectContextMenuExtension(

            //            RXObject.GetClass(typeof(Node)), contextMenu);

        }
        static void TunnelContextMenu()
        {
            ContextMenuExtension contextMenu =
                                        new ContextMenuExtension();

            MenuItem item0 = new MenuItem("巷道详细属性");

            contextMenu.MenuItems.Add(item0);

            item0.Click += new EventHandler(tunnelClickHandler);


            Application.AddObjectContextMenuExtension(

                        RXObject.GetClass(typeof(BaseTunnel)), contextMenu);
        }

        

        static void tunnelClickHandler(object sender, EventArgs e)
        {
            if (Global.ContextShown == true)
                return;
           
            TunnelContextForm contextForm = new TunnelContextForm();
            Application.ShowModelessDialog(contextForm);
        }

        static void nodeClickHandler(object sender, EventArgs e)
        {

        }
    }
}
