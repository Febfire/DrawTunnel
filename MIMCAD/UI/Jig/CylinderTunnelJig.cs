using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using MIM;
using System.Collections.Generic;

namespace UI.Jig
{
    class CylinderTunnelJig : EntityJig
    {
        List<Point3d> mPoints;
        DynamicDimensionDataCollection m_dims;
        int mPromptCounter;
        public CylinderTunnelJig(Point3d firstPt) : base(new CylinderTunnel())
        {
            mPoints = new List<Point3d>();
            mPoints.Add(firstPt);
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


            if (mPromptCounter >= 0)
            {
                jigOpts.Message = "\nEnter next point";
                PromptPointResult dres = prompts.AcquirePoint(jigOpts);
                if (mPoints.Count <= mPromptCounter + 1)
                {
                    mPoints.Add(mPoints[mPromptCounter]);
                }

                Point3d nextPointTemp = dres.Value;

                if (nextPointTemp != mPoints[mPromptCounter + 1])
                {
                    mPoints[mPromptCounter + 1] = nextPointTemp;
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
            ((CylinderTunnel)Entity).BasePoints = mPoints;

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
            CylinderTunnel Tunnel = (CylinderTunnel)Entity;

            if (m_dims.Count <= mPromptCounter)
            {
                Dimension _dim = new AlignedDimension();
                _dim.SetDatabaseDefaults();
                m_dims.Add(new DynamicDimensionData(_dim, true, true));
                _dim.DynamicDimension = true;
            }

            AlignedDimension dim = (AlignedDimension)m_dims[mPromptCounter].Dimension;
            dim.XLine1Point = Tunnel.BasePoints[mPromptCounter];
            dim.XLine2Point = mPoints[mPromptCounter + 1];
            dim.DimLinePoint = Tunnel.BasePoints[mPromptCounter];

        }

        public Entity GetEntity()
        {
            return Entity;
        }

        public void SetName(string name)
        {
            ((CylinderTunnel)Entity).Name = name;
        }
    }
}
