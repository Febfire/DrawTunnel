using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using EntityStore.Models;


[assembly: CommandClass(typeof(UI.Forms.EntityPropertyControl))]
namespace UI.Forms
{
    public partial class EntityPropertyControl : UserControl
    {
        public Autodesk.AutoCAD.Windows.PaletteSet PaletteSet { get; set; }

        [CommandMethod("ShowPropertyControl")]
        public void ShowPaletteSet()
        {
            if (PaletteSet != null)
                PaletteSet.Visible = true;
        }

       
        public EntityPropertyControl()
        {
            InitializeComponent();
        }

        public void BindPaletteSet(Autodesk.AutoCAD.Windows.PaletteSet ps)
        {
            ps.Add("属性控件", this);
            this.PaletteSet = ps;
        }

        public void Display(string type, DBCustomEntity ent)
        {
            switch (type)
            {
                case "Roadway":
                    DBRoadway roadway = ent as DBRoadway;
                    propertyGridControl1.SelectedObject = roadway;

                    break;
                default:
                    propertyGridControl1.SelectedObject = null;
                    break;
            }
        }

        public object GetSource()
        {
            return propertyGridControl1.SelectedObject;
        }
       
        //编译框文本改变时调用
        private void propertyGridControl1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            
        }
    }
}
