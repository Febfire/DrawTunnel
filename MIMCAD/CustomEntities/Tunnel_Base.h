#pragma once
#ifndef MIM_TUNNEL_H
#define MIM_TUNNEL_H

#define TUNNEL_VERSION 1
#include "utils.h"

namespace MIM
{
	enum DisplayMode
	{
		Line = 1,
		DoubleLine = 2,
		Real = 3
	};

	class TunnelReactor;
	typedef std::shared_ptr<TunnelReactor> TunnelReactorPtr;

	class __declspec(dllexport) Tunnel_Base :public AcDbCurve
	{
		
	protected:
		friend class TunnelReactor;

	public:

		//设置全局静态函数
		static bool getIsNodifying();
		static void startNodifying();
		static void endNodifying();
		static void startAnimateMode();
		static void endAnimateMode();
		static bool getAnimateMode();
		static UINT16 getDisplayMode();
		static void setDisplayMode(UINT16 mode);

		ACRX_DECLARE_MEMBERS(Tunnel_Base);
		
		Tunnel_Base();

		virtual ~Tunnel_Base();


	protected:
		TCHAR* m_type;

		UInt16 m_geo_segments;

		UInt16 m_color_segments;

		std::vector<AcGePoint3d> m_geo_basePoints;

		std::vector<AcGePoint3d> m_color_basePoints;

		std::vector<int> m_turningPointIndexes;

		std::vector<int> m_hasColorPointIndexes;

		std::vector<AcDbHandle> m_nodesHandle;

		std::vector<Adesk::UInt32> m_colors;

		std::vector<Adesk::Int16> m_temperatures;

		bool m_closed;

		std::vector<AcGePoint3d> m_drawable_basePoints;

		std::vector<AcGePoint3d> m_arrowPoints;


	private:
		
		TCHAR*	 m_name;
		TCHAR*   m_tagData;
		TCHAR*   m_location;
		AcDbHandle m_tagHandle;
		TunnelReactorPtr m_reactor;
		
		std::vector<AcGePoint3d> m_allPoints3;

		std::vector<AcGePoint3d> m_allPoints2;

		std::vector<int> m_allFaces;

		std::vector<AcCmEntityColor> m_allColors;

		std::vector<AcGePoint3d> m_allArrawPosition;
		/*************************************************************
		override AcDbObject
		*************************************************************/
	public:

		virtual Acad::ErrorStatus dwgOutFields(AcDbDwgFiler *pFiler) const override;

		virtual Acad::ErrorStatus dwgInFields(AcDbDwgFiler *pFiler) override;
		
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

		virtual Acad::ErrorStatus  subTransformBy(const AcGeMatrix3d& xform) override;

		virtual void               subList() const;

		virtual Acad::ErrorStatus   subIntersectWith(
			const AcDbEntity*   ent,
			AcDb::Intersect     intType,
			AcGePoint3dArray&   points,
			Adesk::GsMarker     thisGsMarker = 0,
			Adesk::GsMarker     otherGsMarker = 0)
			const;

		virtual Acad::ErrorStatus   subIntersectWith(
			const AcDbEntity*   ent,
			AcDb::Intersect     intType,
			const AcGePlane&    projPlane,
			AcGePoint3dArray&   points,
			Adesk::GsMarker     thisGsMarker = 0,
			Adesk::GsMarker     otherGsMarker = 0)
			const;

		virtual Acad::ErrorStatus subExplode(
			AcDbVoidPtrArray& entitySet
		) const;

		virtual Adesk::Boolean subCloneMeForDragging() { return true; }

		//禁止ctrl+c
		virtual Acad::ErrorStatus subWblockClone(
			AcRxObject* pOwnerObject,
			AcDbObject*& pClonedObject,
			AcDbIdMapping& idMap,
			Adesk::Boolean isPrimary = true
		) const
		{
			return Acad::ErrorStatus::eCopyFailed;
		}
		virtual Acad::ErrorStatus Tunnel_Base::subDeepClone(
			AcDbObject* pOwnerObject,
			AcDbObject*& pClonedObject,
			AcDbIdMapping& idMap,
			Adesk::Boolean isPrimary = true
		) const = 0;

		/*************************************************************
		override AcDbCurve
		*************************************************************/
	public:
		virtual Adesk::Boolean    isClosed() const override {
			assertReadEnabled();
			return m_closed;
		}
		virtual Adesk::Boolean    isPeriodic() const override {
			assertReadEnabled();
			return Adesk::kFalse;
		}
		virtual Adesk::Boolean    isPlanar() const override {
			assertReadEnabled();
			return Adesk::kFalse;
		} 

		virtual Acad::ErrorStatus getStartParam(double& startParam) const override
		{
			assertReadEnabled();
			startParam = 0.0;
			return Acad::eOk;
		}

		virtual Acad::ErrorStatus getEndParam(double& endParam)    const override
		{
			assertReadEnabled();
			endParam = m_geo_basePoints.size();
			return Acad::eOk;
		}

	public:
		/*************************************************************
		自定义设置属性函数
		*************************************************************/
		void reflush(){assertWriteEnabled();}
		bool displayTag(bool display,bool set) const;
		bool displayNodes(bool display, bool set) const;
		Acad::ErrorStatus addVertexAt(unsigned int index, const AcGePoint3d& pt);
		//get set
		Acad::ErrorStatus setBasePoints(const std::vector<AcGePoint3d>& pts);
		Acad::ErrorStatus setColors(const std::vector<UInt32>&  colors);
		Acad::ErrorStatus setNodesHandle(const std::vector<AcDbHandle>& handles);
		Acad::ErrorStatus setTagHandle(const AcDbHandle& handle);
		Acad::ErrorStatus setType(const TCHAR* type);
		Acad::ErrorStatus setName(const TCHAR* name);		
		Acad::ErrorStatus setTagData(const TCHAR* tag);
		Acad::ErrorStatus setLocation(const TCHAR* location);
		Acad::ErrorStatus setColorSegments(int num);
		Acad::ErrorStatus setTemperatures(const std::vector<Int16> temperatures);

		Acad::ErrorStatus setClose(bool close);

		const Adesk::UInt16 getGeoSegments() const;
		const std::vector<AcGePoint3d>& getBasePoints() const;
		const std::vector<UInt32>& getColors() const;
		const std::vector<AcDbHandle>& getNodesHandle() const;
		const AcDbHandle& getTagHandle() const;
		const TCHAR* getType() const;
		const TCHAR* getName() const;
		const TCHAR* getTagData() const;
		const TCHAR* getLocation() const;
		const int getColorSegments() const;
		const std::vector<Int16> getTemperatures() const;
		virtual const double getHeight() const = 0;
		
		Acad::ErrorStatus  getVertices2(AcGePoint3dArray& vertexArray) const;
		Acad::ErrorStatus  getVertices3(AcGePoint3dArray& vertexArray) const;
		Acad::ErrorStatus  getFaces3(AcGeIntArray& faceArray) const;
		Acad::ErrorStatus  getVerticesColors(AcGeIntArray& colorArray) const;

		Acad::ErrorStatus changeVertice(const AcGePoint3d& vertice, int index);
		Acad::ErrorStatus changeNodeColor(UInt32 color, int index);

		

		//巷道方向中心线向量
		void getCenterVector(std::vector<AcGeVector3d>& cvs) const;
		//巷道截面的“宽”方向向量
		void getHorizontalVector(std::vector<AcGeVector3d>& hvs) const;
		//巷道截面“高”方向向量
		void getVerticalVector(std::vector<AcGeVector3d>& vvs) const;
		//巷道的总长度
		double getLengthSum() const;

		std::vector<AcGePoint3d> getCenterPoints() const;

	protected:
		virtual void setVetices(std::vector<AcGePoint3d>&, std::vector<AcGePoint3d>&) = 0;

		virtual void setFaceList(std::vector<int>&) = 0;

		virtual void setVerticesColorList(std::vector<AcCmEntityColor>&) = 0;

		void setAllColorArray(std::vector<uint32_t>& allColor);

		virtual void createDerive(Tunnel_Base*& newTunnel) const = 0;

		virtual const double getWidth() const = 0;

		UInt16 getDrawableSegments() const;

	private:		

		void setDrawableBasePoints()
		{
			m_drawable_basePoints = m_geo_basePoints;
			if (m_closed)
				m_drawable_basePoints.emplace_back(m_geo_basePoints.at(0));
		}

		void setResultBuffer();

		void drawArrow2(AcGiWorldDraw *mode);

		void drawArrow3(AcGiWorldDraw *mode);

		void calculateColorBaseVertice(const std::vector<AcGePoint3d>& points);

		double setBaseColorPoints(const std::vector<AcGePoint3d>& points,double record,int numOfGs, double gsl, double csl);

	};
}



#endif // !MIM_TUNNEL_H
