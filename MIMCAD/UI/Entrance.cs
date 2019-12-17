// (C) Copyright 2017 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.IO;
using EntityStore.Models;
using EntityStore;
using UI.Forms;
using UI.Events;
using Autodesk.Windows;

// This line is not mandatory, but improves loading performances
[assembly: ExtensionApplication(typeof(UI.Entrance))]

namespace UI
{

    // This class is instantiated by AutoCAD once and kept alive for the 
    // duration of the session. If you don't do any one time initialization 
    // then you should remove this class.
    public class Entrance : IExtensionApplication
    {                
        void IExtensionApplication.Initialize()
        {
            // Initialize your plug-in application here          
            DocumentEvents.AddDocColEvent();
            Autodesk.Windows.ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;

        }

        void IExtensionApplication.Terminate()
        {
            // Do plug-in application clean up here
        }

        void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            if (Autodesk.Windows.ComponentManager.Ribbon != null)
            {
                //Initialize.Ribbon.MyRibbon();
                Autodesk.Windows.ComponentManager.ItemInitialized -= ComponentManager_ItemInitialized;
            }
        }
    }

}
