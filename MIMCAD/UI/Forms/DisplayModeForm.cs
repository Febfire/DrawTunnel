using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Windows.Forms;
using System.Collections.Generic;
using MIM;

namespace UI.Forms
{

    public partial class DisplayModeForm : Form
    {
        private ObjectId[] idarray;

        List<Handle> handleArray = new List<Handle>();
        public DisplayModeForm()
        {
            InitializeComponent();
            ShowInTaskbar = false;

        }
        private void LModeButton_Click(object sender, EventArgs e)
        {
            BaseTunnel.setDisplayMode(1);
            selectAll();
            Utils.ReflushViewport(idarray);
        }

        private void DLModeButton_Click(object sender, EventArgs e)
        {
            BaseTunnel.setDisplayMode(2);
            selectAll();
            Utils.ReflushViewport(idarray);
        }

        private void RModeButton_Click(object sender, EventArgs e)
        {
            BaseTunnel.setDisplayMode(3);
            selectAll();
            Utils.ReflushViewport(idarray);
        }


        private void selectAll()
        {
            Utils.SelectAll("TUNNEL_SQUARE,TUNNEL_CYLINDER,TUNNELNODE", (idArray) => {
                idarray = idArray;
            });
        }
    }
}
