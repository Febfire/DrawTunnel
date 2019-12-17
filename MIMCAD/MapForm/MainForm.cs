using System;
using System.Windows.Forms;


namespace MapForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
        }


        private void ShowMap(string path)
        {
            Cursor mic = mapBox1.Cursor;
            mapBox1.Cursor = Cursors.WaitCursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                mapBox1.Map = Init.InitializeMapOrig(path);
                mapBox1.Map.Size = Size;
                mapBox1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error");
            }
            Cursor = Cursors.Default;
            mapBox1.Cursor = mic;
        }
       

        private void saveButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";
            dialog.Filter = "shp文件(*.shp)|*.shp";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                ShowMap(file);
            }
            
        }
    }
}
