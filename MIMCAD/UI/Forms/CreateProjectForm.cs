using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using System.IO;

[assembly: CommandClass(typeof(UI.Forms.CreateProjectForm))]
namespace UI.Forms
{
    public partial class CreateProjectForm : Form
    {
        public CreateProjectForm()
        {
            InitializeComponent();

            textBox_name.Text = "unnamed";
        }

        [CommandMethod("createform", CommandFlags.Session)]
        public void Open()
        {
            CreateProjectForm cf = new CreateProjectForm();
            cf.CenterToScreen();
            cf.Show();
        }

        [CommandMethod("openfile", CommandFlags.Session)]
        public void OpenFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                var config = Config.Read(path);
                Global.OpenProject(Path.GetDirectoryName(path), config.ProjectName);
            }
        }

        //浏览按钮
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string foldPath = folderBrowserDialog1.SelectedPath;
                this.textBox_path.Text = foldPath;
            }
        }

        /// <summary>
        /// 确定按钮，创建出新的工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string path = this.textBox_path.Text;
            string name = this.textBox_name.Text;

            if (!Fs.DirExists(path))
            {
                Autodesk.AutoCAD.ApplicationServices.Application.
                   ShowAlertDialog("请输入正确路径。");
                return;
            }
            if (Fs.DirExists(path + name))
            {
                Autodesk.AutoCAD.ApplicationServices.Application.
                    ShowAlertDialog("该目录有同名文件夹，请另存为项目为其他名称，或者将同名文件夹删除。");
                return;
            }

           

            this.Close();

            Global.CreateNewProject(path + name, name);

            Fs.createDwg(Global.CurrentProjectPath, Global.CurrentProjectName);

            Global.OpenProject(path + name, name);
          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
