using Autodesk.AutoCAD.Runtime;
using DevExpress.XtraTreeList.Nodes;
using System.Collections.Generic;
using System.Windows.Forms;



[assembly: CommandClass(typeof(UI.Forms.TreeList))]
namespace UI.Forms
{
    public partial class TreeList : UserControl
    {
        [CommandMethod("addTreelist")]
        public static void AddTreelist()
        {
            List<SceneTreeNode> lt = new List<SceneTreeNode>();

            lt.Add(new SceneTreeNode(1, 0, 1, "煤矿1"));

            lt.Add(new SceneTreeNode(2, 1, 2, "开采水平1"));
            lt.Add(new SceneTreeNode(7, 1, 2, "开采水平2"));
            lt.Add(new SceneTreeNode(8, 1, 2, "开采水平3"));

            lt.Add(new SceneTreeNode(3, 2, 3, "采区1"));
            lt.Add(new SceneTreeNode(9, 2, 3, "采区1"));
            lt.Add(new SceneTreeNode(10, 2, 3, "采区1"));

            lt.Add(new SceneTreeNode(4, 3, 4, "工作面1"));
            lt.Add(new SceneTreeNode(11, 3, 4, "工作面2"));
            lt.Add(new SceneTreeNode(12, 3, 4, "工作面3"));

            lt.Add(new SceneTreeNode(5, 4, 5, "巷道1"));
            lt.Add(new SceneTreeNode(6, 4, 5, "巷道2"));

            PROJECT.treeList.PaletteSet.Visible = true;
            PROJECT.treeList.BindData(lt);

        }
        public Autodesk.AutoCAD.Windows.PaletteSet PaletteSet { get; set; }
        public TreeList()
        {
            InitializeComponent();

            imageList1.Images.Add(System.Drawing.Image.FromFile(PROJECT.sPath + @"\a1.ico"));

            imageList1.Images.Add(System.Drawing.Image.FromFile(PROJECT.sPath + @"\a3.ico"));

            treeList1.OptionsBehavior.Editable = false; //不可编辑

            treeList1.OptionsView.ShowIndicator = false;//不显示左边的一列 
            treeList1.OptionsView.ShowHorzLines = false;//不显示水平线
            treeList1.OptionsView.ShowVertLines = false;//垂直线条是否显示

            //聚焦的样式是否只适用于聚焦细胞或所有细胞除了聚焦对象，失去焦点后
            treeList1.LookAndFeel.UseWindowsXPTheme = true;
            treeList1.OptionsSelection.InvertSelection = true;

            treeList1.ShowFindPanel();
           
            treeList1.ExpandAll();

            treeList1.KeyFieldName = "ID";
            treeList1.ParentFieldName = "ParentID";
        }

        public void BindPaletteSet(Autodesk.AutoCAD.Windows.PaletteSet ps)
        {
            ps.Add("场景管理", this);
            this.PaletteSet = ps;
        }

        public void BindData(object o)
        {
            treeList1.DataSource = o;
        }

        private void treeList1_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            if (e.Node == null) return;
            TreeListNode node = e.Node;

            object value = node.GetValue("Level");

            if (value == null) return;

            int Level = (int)value;

            switch (Level)
            {
                case 5:
                    e.NodeImageIndex = 0;
                    break;
                default:
                    e.NodeImageIndex = 1;
                    break;
            }
              
        }
    }

    public class SceneTreeNode
    {
        public SceneTreeNode(int id, int parentId, int level, string name,string description = null)
        {
            ID = id;
            ParentID = parentId;
            Level = level;
            Name = name;
            Description = "（空）";
        }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
