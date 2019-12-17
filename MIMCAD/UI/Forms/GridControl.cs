using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System.ComponentModel;
using System.Windows.Forms;

[assembly: CommandClass(typeof(UI.Forms.GridControl))]
namespace UI.Forms
{
    public partial class GridControl : UserControl
    {
        
        [CommandMethod("addGrid")]
        static public void addGrid()
        {
            PROJECT.gridControl.PaletteSet.Visible = true;

            BindingList<People> p = new BindingList<People>();
            PROJECT.gridControl.gridControl1.DataSource = typeof(People);
            People z = new People { Name = "张三", Age = 19, Sex = "男" };
            p.Add(new People { Name = "李四", Age = 29, Sex = "男" });
            PROJECT.gridControl.gridControl1.DataSource = p;

        }

        public Autodesk.AutoCAD.Windows.PaletteSet PaletteSet { get; set; }
        public GridControl()
        {
            InitializeComponent();            
        }

        public void BindPaletteSet(Autodesk.AutoCAD.Windows.PaletteSet ps)
        {
            ps.Add("表单控件", this);
            this.PaletteSet = ps;
        }

        public void BindData(object o)
        {
            gridControl1.DataSource = o;
        }

    }


    class People
    {
        public int ID { get; set; }
        public int ParentID { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
    }
    class Subject
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int yuwen { get; set; }
        public int math { get; set; }
    }
}
