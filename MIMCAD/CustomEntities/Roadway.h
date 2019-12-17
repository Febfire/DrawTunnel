#pragma once
#ifndef ROADWAY_H
#define ROADWAY_H

#define VERSION_RW 1

#include "detail.h"

namespace MIM
{
	class RoadwayReactor :public AcDbObjectReactor
	{
	public:

		ACRX_DECLARE_MEMBERS(RoadwayReactor);

		RoadwayReactor() {}
		virtual ~RoadwayReactor() {};

		virtual void erased(const AcDbObject* dbObj, Adesk::Boolean bErasing) override;

		virtual void modified(const AcDbObject* dbObj) override;
	};


	class __declspec(dllexport) Roadway final:public AcDbEntity
	{
	private:
		typedef std::shared_ptr<RoadwayReactor> pRoadwayReactor;

		struct TmpPara
		{
			AcGePoint3d ArrawPosition;  //标注点(箭头)位置

			std::shared_ptr<Adesk::Int32> FaceIndexPtrArray; //巷道面列表的集合

			std::shared_ptr<AcGePoint3d> PointPtrArray; //巷道上所有点的集合

			std::shared_ptr<AcGiVertexData> VertexData;  //巷道shell顶点属性

			std::shared_ptr<AcCmEntityColor> pTmpVerticesColorList;//顶点颜色属性

			AcGeVector3d CenterVector; //中心线向量	
		};
		enum ERoadwayPreBuf
		{
			eStardNodeHandle = 1,
			eEndNodeHandle = 2,
			eExtensionData = 3
		};
		struct BufPair
		{
			ERoadwayPreBuf type;
			Any buf;
		};
		friend class RoadwayReactor;
	public:
		
		ACRX_DECLARE_MEMBERS(Roadway);
		Roadway();
		virtual ~Roadway();

	private:


		Adesk::UInt32 m_segmentCount;

		AcGePoint3d m_startPoint;
		AcGePoint3d m_endPoint;

		wchar_t*	 m_name;

		bool m_animationSwitch;

		AcGePoint3d m_arrowPoint;

		mutable AcDbHandle m_startNodeHandle;
		mutable AcDbHandle m_endNodeHandle;

		std::shared_ptr<Adesk::UInt16> m_colorIndex;

		TmpPara* m_tmp;

		wchar_t* m_extensionData;

		pRoadwayReactor m_reactor;

		mutable bool moved[2];

		std::vector<BufPair> preBuf;

		spin_mutex mutex;
		/*************************************************************
		override AcDbObject
		*************************************************************/
	public:

		virtual Acad::ErrorStatus dwgOutFields(AcDbDwgFiler *pFiler) const override;

		virtual Acad::ErrorStatus dwgInFields(AcDbDwgFiler *pFiler) override;

		Acad::ErrorStatus subClose() override;
		//virtual Acad::ErrorStatus subHighlight(const AcDbFullSubentPath&
		//	= kNullSubent, const Adesk::Boolean highlightAll = Adesk::kFalse) const override;

		//virtual Acad::ErrorStatus subUnhighlight(const AcDbFullSubentPath&
		//	= kNullSubent, const Adesk::Boolean highlightAll = Adesk::kFalse) const override;

		//virtual Acad::ErrorStatus subErase(Adesk::Boolean erasing) override;



		/*************************************************************
		override AcDbEntity
		*************************************************************/
	public:

		virtual Adesk::Boolean subWorldDraw(AcGiWorldDraw *mode) override;

		virtual Acad::ErrorStatus subGetGripPoints(AcGePoint3dArray &gripPoints,
			AcDbIntArray &osnapModes,
			AcDbIntArray &geomIds) const override;

		virtual Acad::ErrorStatus subMoveGripPointsAt(const AcDbIntArray &indices, const AcGeVector3d &offset) override;

		virtual Acad::ErrorStatus subGetOsnapPoints(
			AcDb::OsnapMode     osnapMode,
			Adesk::GsMarker     gsSelectionMark,
			const AcGePoint3d&  pickPoint,
			const AcGePoint3d&  lastPoint,
			const AcGeMatrix3d& viewXform,
			AcGePoint3dArray&   snapPoints,
			AcDbIntArray &   geomIds) const override;


		virtual Acad::ErrorStatus   subIntersectWith(
			const AcDbEntity*   ent,
			AcDb::Intersect     intType,
			const AcGePlane&    projPlane,
			AcGePoint3dArray&   points,
			Adesk::GsMarker     thisGsMarker = 0,
			Adesk::GsMarker     otherGsMarker = 0)
			const override;

		//通过这个函数可以判断是否跟别的物体相交了，还可以求出交点
		virtual Acad::ErrorStatus   subIntersectWith(
			const AcDbEntity*   ent,
			AcDb::Intersect     intType,
			AcGePoint3dArray&   points,
			Adesk::GsMarker     thisGsMarker = 0,
			Adesk::GsMarker     otherGsMarker = 0)
			const override;

		virtual Acad::ErrorStatus  subTransformBy(const AcGeMatrix3d& xform) override;


		virtual Acad::ErrorStatus	subExplode(AcDbVoidPtrArray& entitySet) const;

		virtual void                subList() const;

	public:
		/*************************************************************
		自定义设置属性函数
		*************************************************************/
		//设置全局变量isNodifying
		static bool getIsNodifying();
		static void startNodifying();
		static void endNodifying();

		//get set
		inline Acad::ErrorStatus setStartPoint(const AcGePoint3d& sp);
		inline Acad::ErrorStatus setEndPoint(const AcGePoint3d& ep);
		inline Acad::ErrorStatus setName(const TCHAR* name);
		inline Acad::ErrorStatus setStartNodeHandle(const AcDbHandle&) ;
		inline Acad::ErrorStatus setEndNodeHandle(const AcDbHandle&);
		inline Acad::ErrorStatus setAnimateSwich(bool swich);
		inline Acad::ErrorStatus setExtensionData(const TCHAR* jsonStr);

		inline const AcGePoint3d& getStartPoint() const;
		inline const AcGePoint3d& getEndPoint() const;
		inline const TCHAR* getName() const;
		inline const AcDbHandle& getStartNodeHandle() const;
		inline const AcDbHandle& getEndNodeHandle() const;
		inline bool getAnimateSwich() const;
		inline const TCHAR* getExtensionData() const;

	private:
		void setPreBuf();

		void drawArrow(
			AcGiWorldDraw *mode,
			const AcGeVector3d& cv,
			const AcGeVector3d& hv,
			const AcGeVector3d& vv,
			double h,
			double w);

		void getFaceList();

		void getVerticesColorList();
	};
}
#endif // !ROADWAY_H


