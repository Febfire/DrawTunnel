using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.DefinedEnitity;

[assembly: CommandClass(typeof(UI.Create.StandardLineJig))]
namespace UI.Create
{
    public class StandardLineJig : EntityJig
    {
        Point3d mStartPt, mEndPt, mInflectionPt;
        DynamicDimensionDataCollection m_dims;
        int mPromptCounter;

        public StandardLineJig(Point3d spt,Point3d ipt,Point3d ept) : base(new StandardLineWrapper())
        {
            mStartPt = spt;
            mInflectionPt = ipt;
            mEndPt = ept;
            mPromptCounter = 0;

            m_dims = new DynamicDimensionDataCollection();
            Dimension dim1 = new AlignedDimension();
            dim1.SetDatabaseDefaults();
            m_dims.Add(new DynamicDimensionData(dim1, true, true));
            dim1.DynamicDimension = true;
            Dimension dim2 = new AlignedDimension();
            dim2.SetDatabaseDefaults();
            m_dims.Add(new DynamicDimensionData(dim2, true, true));
            dim2.DynamicDimension = true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jigOpts = new JigPromptPointOptions();
            jigOpts.UserInputControls = (UserInputControls.Accept3dCoordinates
                                       | UserInputControls.NoZeroResponseAccepted
                                       | UserInputControls.NoNegativeResponseAccepted
                                       | UserInputControls.NullResponseAccepted
            );

            if (mPromptCounter == 0)
            {
                jigOpts.Message = "\nEnter StandardLine Inflection Point";

                PromptPointResult dres = prompts.AcquirePoint(jigOpts);

                Point3d inflectionPointTemp = dres.Value;

                if (inflectionPointTemp != mInflectionPt)
                {
                    mInflectionPt = inflectionPointTemp;
                }
                else
                {
                    return SamplerStatus.NoChange;
                }
                if (dres.Status == PromptStatus.Cancel)
                {
                    return SamplerStatus.Cancel;
                }
                else
                {
                    return SamplerStatus.OK;
                }
            }
            else if (mPromptCounter == 1)
            {
                jigOpts.Message = "\nEnter StandardLine End Point";

                PromptPointResult dres = prompts.AcquirePoint(jigOpts);

                Point3d endPointTemp = dres.Value;

                if (endPointTemp != mEndPt)
                {
                    mEndPt = endPointTemp;
                }
                else
                {
                    return SamplerStatus.NoChange;
                }
                if (dres.Status == PromptStatus.Cancel)
                {
                    return SamplerStatus.Cancel;
                }
                else
                {
                    return SamplerStatus.OK;
                }
            }
            else
            {
                return SamplerStatus.NoChange;
            }
        }
        public void setPromptCounter(int count)
        {
            mPromptCounter = count;
        }
        protected override bool Update()
        {
           // ((StandardLineWrapper)Entity).StartPoint = mStartPt;
           // ((StandardLineWrapper)Entity).InflectionPoint = mInflectionPt;
           // ((StandardLineWrapper)Entity).EndPoint = mEndPt;

            UpdataDimension();

            return true;
        }
        protected override DynamicDimensionDataCollection GetDynamicDimensionData(double dimScale)
        {
            return m_dims;
        }
        protected override void OnDimensionValueChanged(Autodesk.AutoCAD.DatabaseServices.DynamicDimensionChangedEventArgs e)
        {

        }
        private void UpdataDimension()
        {
            if (mPromptCounter == 0)
            {
                StandardLineWrapper myStandardLine = (StandardLineWrapper)Entity;
                AlignedDimension dim = (AlignedDimension)m_dims[0].Dimension;
              //  dim.XLine1Point = myStandardLine.StartPoint;
                dim.XLine2Point = mInflectionPt;
              //  dim.DimLinePoint = myStandardLine.StartPoint;
            }
            else if (mPromptCounter == 1)
            {
                StandardLineWrapper myStandardLine = (StandardLineWrapper)Entity;
                AlignedDimension dim = (AlignedDimension)m_dims[1].Dimension;
                dim.XLine1Point = myStandardLine.InflectionPoint;
                dim.XLine2Point = mEndPt;
                dim.DimLinePoint = myStandardLine.InflectionPoint;
            }


        }
        public Entity GetEntity()
        {
            return Entity;
        }

        [CommandMethod("StandardLine")]
        static public void D()
        {
            StandardLineWrapper sl = new StandardLineWrapper();

            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            using (Transaction myT = tm.StartTransaction())
            {
                BlockTable bt = (BlockTable)tm.GetObject(db.BlockTableId, OpenMode.ForRead, false);
                BlockTableRecord btr = (BlockTableRecord)tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false);
                btr.AppendEntity(sl);
                tm.AddNewlyCreatedDBObject(sl, true);
                myT.Commit();
            }

        }

        [CommandMethod("StandardLinejig")]
        static public void DoIt()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //输入起点坐标
            PromptPointOptions opts = new PromptPointOptions("\nEnter Roadway Start Point:");
            PromptPointResult res = ed.GetPoint(opts);

            if (res.Status == PromptStatus.Cancel)
                return;

            Point3d tmpPoint = res.Value;

            StandardLineJig jig = new StandardLineJig(tmpPoint, tmpPoint, tmpPoint);

            jig.setPromptCounter(0);
            PromptResult drag = ed.Drag(jig);

            jig.setPromptCounter(1);
            drag = ed.Drag(jig);

            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            using (Transaction myT = tm.StartTransaction())
            {
                BlockTable bt = (BlockTable)tm.GetObject(db.BlockTableId, OpenMode.ForRead, false);
                BlockTableRecord btr = (BlockTableRecord)tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false);
                btr.AppendEntity(jig.GetEntity());
                tm.AddNewlyCreatedDBObject(jig.GetEntity(), true);
                myT.Commit();
            }

        }
    }
}
