using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(UI.Initialize.Ribbon))]
namespace UI.Initialize
{
    class Ribbon
    {
        static bool isOpen = false;
        [CommandMethod("openribbon")]
        //添加面板选项卡
        public static void MyRibbon()
        {
            if (UI.Global.isOpen == false)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("请先打开一个工程！");
                return;
            }
            Autodesk.Windows.RibbonControl ribbonControl = Autodesk.Windows.ComponentManager.Ribbon;
            RibbonTab Tab = new RibbonTab();
            //for (int i = 0; i < ribbonControl.Tabs.Count; i++)
            //{
            //    if(ribbonControl.Tabs[i].Id == "RibbonSample_TAB_ID")
            //    {
            //        isOpen = true;
            //    }
            //}
            if (isOpen == false)
            {
                ribbonControl.Tabs.Add(Tab);
                Tab.Title = "  煤矿工程  ";
                Tab.Id = "RibbonSample_TAB_ID";


                Autodesk.Windows.RibbonPanelSource panel1Panel = new RibbonPanelSource();
                panel1Panel.Title = "手工绘制";
                RibbonPanel Panel1 = new RibbonPanel();
                Panel1.Source = panel1Panel;
                Tab.Panels.Add(Panel1);

                RibbonButton pan1button1 = new RibbonButton();
                pan1button1.Text = "绘制方形";
                pan1button1.ShowText = true;
                //pan1button1.ShowImage = true;
                //pan1button1.Image = Images.getBitmap(Forms.Resources.Small);
                //pan1button1.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan1button1.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan1button1.Size = RibbonItemSize.Standard;
                pan1button1.CommandHandler = new RibbonCommandHandler();
                pan1button1.CommandParameter = "SqTunnel ";
                panel1Panel.Items.Add(pan1button1);

                RibbonButton pan1button2 = new RibbonButton();
                pan1button2.Text = "绘制圆形";
                pan1button2.ShowText = true;
                //pan1button2.ShowImage = true;
                //pan1button2.Image = Images.getBitmap(Forms.Resources.Small);
                //pan1button2.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan1button2.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan1button2.Size = RibbonItemSize.Standard;
                pan1button2.CommandHandler = new RibbonCommandHandler();
                pan1button2.CommandParameter = "CylTunnel ";
                panel1Panel.Items.Add(pan1button2);

                RibbonButton pan1button3 = new RibbonButton();
                pan1button3.Text = "绘制梯形";
                pan1button3.ShowText = true;
                //pan1button3.ShowImage = true;
                //pan1button3.Image = Images.getBitmap(Forms.Resources.Small);
                //pan1button3.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan1button3.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan1button3.Size = RibbonItemSize.Standard;
                pan1button3.CommandHandler = new RibbonCommandHandler();
                pan1button3.CommandParameter = "TrTunnel ";
                panel1Panel.Items.Add(pan1button3);

                RibbonPanelSource panel2Panel = new RibbonPanelSource();
                panel2Panel.Title = "视图窗口";
                RibbonPanel panel2 = new RibbonPanel();
                panel2.Source = panel2Panel;
                Tab.Panels.Add(panel2);

                RibbonButton pan2button1 = new RibbonButton();
                pan2button1.Text = "菜单列表";
                pan2button1.ShowText = true;
                //pan2button1.ShowImage = true;
                //pan2button1.Image = Images.getBitmap(Forms.Resources.Small);
                //pan2button1.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan2button1.Size = RibbonItemSize.Standard;
                pan2button1.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan2button1.CommandHandler = new RibbonCommandHandler();
                pan2button1.CommandParameter = "openTreeControl ";

                panel2Panel.Items.Add(pan2button1);


                RibbonButton pan3button1 = new RibbonButton();
                pan3button1.Text = "坐标列表";
                pan3button1.ShowText = true;
                //pan3button1.ShowImage = true;
                //pan3button1.Image = Images.getBitmap(Forms.Resources.Small);
                //pan3button1.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan3button1.Size = RibbonItemSize.Standard;
                pan3button1.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan3button1.CommandHandler = new RibbonCommandHandler();
                pan3button1.CommandParameter = "openGridControl ";

                panel2Panel.Items.Add(pan3button1);


                RibbonPanelSource panel4Panel = new RibbonPanelSource();
                panel4Panel.Title = "显示模式";
                RibbonPanel panel4 = new RibbonPanel();
                panel4.Source = panel4Panel;
                Tab.Panels.Add(panel4);

                RibbonButton pan4button1 = new RibbonButton();
                pan4button1.Text = "切换模式";
                pan4button1.ShowText = true;
                //pan4button1.ShowImage = true;
                //pan4button1.Image = Images.getBitmap(Forms.Resources.Small);
                //pan4button1.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan4button1.Size = RibbonItemSize.Standard;
                pan4button1.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan4button1.CommandHandler = new RibbonCommandHandler();
                pan4button1.CommandParameter = "DisplayMode ";
                panel4Panel.Items.Add(pan4button1);

                RibbonButton pan5button1 = new RibbonButton();
                pan5button1.Text = "演示动画";
                pan5button1.ShowText = true;
                //pan5button1.ShowImage = true;
                //pan5button1.Image = Images.getBitmap(Forms.Resources.Small);
                //pan5button1.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan5button1.Size = RibbonItemSize.Standard;
                pan5button1.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan5button1.CommandHandler = new RibbonCommandHandler();
                pan5button1.CommandParameter = "animate ";
                panel4Panel.Items.Add(pan5button1);


                RibbonButton pan6button1 = new RibbonButton();
                pan6button1.Text = "特性面板";
                pan6button1.ShowText = true;
                //pan6button1.ShowImage = true;
                //pan6button1.Image = Images.getBitmap(Forms.Resources.Small);
                //pan6button1.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan6button1.Size = RibbonItemSize.Standard;
                pan6button1.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan6button1.CommandHandler = new RibbonCommandHandler();
                pan6button1.CommandParameter = "openPropertyControl ";
                panel2Panel.Items.Add(pan6button1);

                RibbonPanelSource panel7Panel = new RibbonPanelSource();
                panel7Panel.Title = "数据传输";
                RibbonPanel panel7 = new RibbonPanel();
                panel7.Source = panel7Panel;
                Tab.Panels.Add(panel7);

                RibbonButton pan7button1 = new RibbonButton();
                pan7button1.Text = "导出信息";
                pan7button1.ShowText = true;
                //pan7button1.ShowImage = true;
                //pan7button1.Image = Images.getBitmap(Forms.Resources.Small);
                //pan7button1.LargeImage = Images.getBitmap(Forms.Resources.large);
                pan7button1.Size = RibbonItemSize.Standard;
                pan7button1.Orientation = System.Windows.Controls.Orientation.Vertical;
                pan7button1.CommandHandler = new RibbonCommandHandler();
                pan7button1.CommandParameter = "OutGeoData ";
                panel7Panel.Items.Add(pan7button1);

                Tab.IsActive = true;
            }
         
        }

        public class RibbonCommandHandler : System.Windows.Input.ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                Document doc = acadApp.DocumentManager.MdiActiveDocument;
                RibbonButton ribBtn = parameter as RibbonButton;

                doc.Editor.WriteMessage("\n命令: " + ribBtn.Text + "\n");

                if (ribBtn != null)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application
                        .DocumentManager.MdiActiveDocument
                        .SendStringToExecute(
                            (string)ribBtn.CommandParameter, true, false, true);
                }
            }
        }

        public class Images
        {
            public static BitmapImage getBitmap(Bitmap image)
            {
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Png);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = stream;
                bmp.EndInit();

                return bmp;
            }
        }
    }
}
