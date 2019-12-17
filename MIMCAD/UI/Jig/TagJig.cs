using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using MIM;

namespace UI.Jig
{
    public class TagJig : EntityJig
    {
        Point3d mStartPt, mEndPt, mInflectionPt;
        DynamicDimensionDataCollection m_dims;
        int mPromptCounter;

        public TagJig(Point3d spt,Point3d ipt,Point3d ept) : base(new Tag())
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
                jigOpts.Message = "\nEnter Tag Inflection Point";

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
                jigOpts.Message = "\nEnter Tag End Point";

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
            ((Tag)Entity).StartPoint = mStartPt;
            ((Tag)Entity).InflectionPoint = mInflectionPt;
            ((Tag)Entity).EndPoint = mEndPt;

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
                Tag myTag = (Tag)Entity;
                AlignedDimension dim = (AlignedDimension)m_dims[0].Dimension;
                dim.XLine1Point = myTag.StartPoint;
                dim.XLine2Point = mInflectionPt;
                dim.DimLinePoint = myTag.StartPoint;
            }
            else if (mPromptCounter == 1)
            {
                Tag myTag = (Tag)Entity;
                AlignedDimension dim = (AlignedDimension)m_dims[1].Dimension;
                dim.XLine1Point = myTag.InflectionPoint;
                dim.XLine2Point = mEndPt;
                dim.DimLinePoint = myTag.InflectionPoint;
            }
        }
        public Entity GetEntity()
        {
            return Entity;
        }

    }
}
