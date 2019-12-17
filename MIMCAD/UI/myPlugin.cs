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
// This line is not mandatory, but improves loading performances
[assembly: ExtensionApplication(typeof(UI.MyPlugin))]

namespace UI
{

    // This class is instantiated by AutoCAD once and kept alive for the 
    // duration of the session. If you don't do any one time initialization 
    // then you should remove this class.
    public class MyPlugin : IExtensionApplication
    {

        void IExtensionApplication.Initialize()
        {

            // Initialize your plug-in application here

            Initialize.InitControls InitControl = new Initialize.InitControls();
            InitControl.InitPropertyControl();
            InitControl.InitTreeList();
            InitControl.InitGridControl();

            if (!Directory.Exists(PROJECT.sPath))
            {
                Directory.CreateDirectory(PROJECT.sPath);
            }         

            Events.DocumentEvents.AddDocColEvent();           
        }

        void IExtensionApplication.Terminate()
        {
            // Do plug-in application clean up here
        }

    }

}
