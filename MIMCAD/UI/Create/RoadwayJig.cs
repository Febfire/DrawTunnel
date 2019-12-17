using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.DefinedEnitity;
using System;
using System.Collections.Generic;
using EntityStore.Models;
using Autodesk.AutoCAD.GraphicsInterface;

[assembly: CommandClass(typeof(UI.Create.RoadwayJig))]
namespace UI.Create
{

    public class RoadwayJig : EntityJig
    {
       
        Point3d mStartPt, mEndPt;
        DynamicDimensionDataCollection m_dims;
        int mPromptCounter;
        public RoadwayJig(Point3d spt, Point3d ept) : base(new RoadwayWrapper())
        {
            mStartPt = spt;
            mEndPt = ept;
            mPromptCounter = 0;

            m_dims = new DynamicDimensionDataCollection();
            Dimension dim1 = new AlignedDimension();
            dim1.SetDatabaseDefaults();
            m_dims.Add(new DynamicDimensionData(dim1, true, true));
            dim1.DynamicDimension = true;

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

                jigOpts.Message = "\nEnter Roadway End Point";
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

        protected override bool Update()
        {
            
            ((RoadwayWrapper)Entity).StartPoint = mStartPt;
            ((RoadwayWrapper)Entity).EndPoint = mEndPt;

            UpdataDimension();

            return true;
        }

        public void setPromptCounter(int count)
        {
            mPromptCounter = count;
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
            RoadwayWrapper myRoadway = (RoadwayWrapper)Entity;
            AlignedDimension dim = (AlignedDimension)m_dims[0].Dimension;
            dim.XLine1Point = myRoadway.StartPoint;
            dim.XLine2Point = mEndPt;
            dim.DimLinePoint = myRoadway.StartPoint;

        }
       
        public Entity GetEntity()
        {
            return Entity;
        }

        public Point3d GetEndPoint()
        {
            return mEndPt;
        }

        public void SetName(string name)
        {
            ((RoadwayWrapper)Entity).Name = name;
        }


        //开始画航道的主函数
        [CommandMethod("Roadwayjig")]
        static public void DoIt()
        {

            List<RoadwayWrapper> roadwayarray = new List<RoadwayWrapper>();

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //输入名字
            string name = "";
            PromptStringOptions opts2 = new PromptStringOptions("\nEnter Roadway name");
            PromptResult res2 = ed.GetString(opts2);
            if (res2.Status == PromptStatus.Cancel)
                return;
            else
            {
                name = res2.StringResult;
            }


            //输入起点坐标
            PromptPointOptions opts = new PromptPointOptions("\nEnter Roadway Start Point:");
            PromptPointResult res = ed.GetPoint(opts);

            if (res.Status == PromptStatus.Cancel)
                return;

            Point3d tmpPoint = res.Value;

            while (true)
            {             
                //create roadway
                RoadwayJig jig = new RoadwayJig(tmpPoint, tmpPoint);
                RoadwayWrapper CurrentRW = (RoadwayWrapper)jig.GetEntity();
                jig.SetName(name);

                jig.setPromptCounter(0);
                PromptResult drag = ed.Drag(jig);
                if (drag.Status == PromptStatus.Cancel || drag.Status == PromptStatus.None)
                    return;
                else if (drag.Status == PromptStatus.OK)
                {                   
                    Database db = Application.DocumentManager.MdiActiveDocument.Database;

                    Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

                    PROJECT pro = new PROJECT();
                    using (Transaction myT = tm.StartTransaction())
                    {
                        pro.AppendEntity(CurrentRW);
                        
                        RoadwayNodeWrapper sttNd;
                        if (roadwayarray.Count > 0)
                        {
                            RoadwayWrapper PreviousRW =
                            (RoadwayWrapper)myT.GetObject(roadwayarray[roadwayarray.Count - 1].ObjectId, OpenMode.ForRead, false);
                            //Point3dCollection pc = new Point3dCollection();
                            // CurrentRW.IntersectWith(PreviousRW, Intersect.OnBothOperands, pc, IntPtr.Zero, IntPtr.Zero);
                            // CurrentRW.StartNodeHandle = PreviousRW.EndNodeHandle;
                            ObjectId tmpid = db.GetObjectId(false, PreviousRW.EndNodeHandle, 0);
                            sttNd = (RoadwayNodeWrapper)myT.GetObject(tmpid, OpenMode.ForWrite, false);
                            sttNd.appendRoadway(CurrentRW.Handle, 1);

                            CurrentRW.StartNodeHandle = sttNd.Handle;
                        }
                        else
                        {
                            sttNd = new RoadwayNodeWrapper();
                            pro.AppendEntity(sttNd);
                            sttNd.CreateSphere(15);
                            sttNd.init(CurrentRW.Handle, 1);
                            sttNd.Position = CurrentRW.StartPoint;

                            CurrentRW.StartNodeHandle = sttNd.Handle;
                        }

                        RoadwayNodeWrapper endNd = new RoadwayNodeWrapper();
                        pro.AppendEntity(endNd);
                        endNd.CreateSphere(15);
                        endNd.init(CurrentRW.Handle, -1);
                        endNd.Position = CurrentRW.EndPoint;
                        CurrentRW.EndNodeHandle = endNd.Handle;

                        roadwayarray.Add(CurrentRW);
                        myT.Commit();                     
                    }

                    tmpPoint = jig.GetEndPoint();
                }
               
            }

        }

    }

}
