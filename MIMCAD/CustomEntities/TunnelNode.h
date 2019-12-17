#pragma once
#pragma once
#ifndef TUNNELNODE_H
#define TUNNELNODE_H

#define VERSION_NS 1 

#define INDEX int

#include "util.h"

namespace MIM
{

	class Tunnel_Base;

	class TunnelNodeReactor :public AcDbEntityReactor
	{
	public:
		ACRX_DECLARE_MEMBERS(TunnelNodeReactor);
		
		TunnelNodeReactor() {}
		virtual ~TunnelNodeReactor() {};

		virtual void modified(const AcDbObject* dbObj) override;

		//virtual void erased(const AcDbObject* dbObj,Adesk::Boolean bErasing) override;

	};

	class __declspec(dllexport) TunnelNode final:public AcDbEntity
	{

		typedef std::shared_ptr<TunnelNodeReactor> TunnelNodeReactorPtr;
		typedef std::map<AcDbHandle, INDEX> HdlSd;
		typedef std::shared_ptr<HdlSd> HdlSdPtr;
		friend class TunnelNodeReactor;
		enum Side
		{
			EFront = 0,
			EBack = 1,
			EMiddle = 2
		};

	public:
		ACRX_DECLARE_MEMBERS(TunnelNode);

		TunnelNode();
		virtual ~TunnelNode();


	private:
		AcGePoint3d m_position;
		Adesk::Int32 m_radius;
		UINT32 m_color;
		TCHAR* m_name;
		TCHAR* m_location;
		TunnelNodeReactorPtr m_nodeReactor;
		HdlSdPtr m_hdlsd;

	public:

		virtual Acad::ErrorStatus dwgOutFields(AcDbDwgFiler *pFiler) const override;

		virtual Acad::ErrorStatus dwgInFields(AcDbDwgFiler *pFiler) override;

		//重写删除操作控制不让直接删除节点
		virtual Acad::ErrorStatus subErase(Adesk::Boolean erasing) override;

		virtual Acad::ErrorStatus subClose() override;

		//下面两个个函数重写不让节点可复制
		virtual Acad::ErrorStatus subWblockClone(
			AcRxObject* pOwnerObject,
			AcDbObject*& pClonedObject,
			AcDbIdMapping& idMap,
			Adesk::Boolean isPrimary = true
		) const
		{
			return Acad::ErrorStatus::eCopyFailed;
		}
		virtual Acad::ErrorStatus subDeepClone(
			AcDbObject* pOwnerObject,
			AcDbObject*& pClonedObject,
			AcDbIdMapping& idMap,
			Adesk::Boolean isPrimary = true
		) const
		{
			return Acad::ErrorStatus::eCopyFailed;
		}

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

		virtual Acad::ErrorStatus subGetGeomExtents(AcDbExtents& extents)const override;

	public:
		void reflush()
		{
			assertWriteEnabled();
		}

		void appendTunnel(AcDbHandle, INDEX);

		void removeTunnel(const AcDbHandle&);

		void clearTunnel() const;

		bool tryErase();

		Acad::ErrorStatus setPosition(const AcGePoint3d&);
		Acad::ErrorStatus setRadius(uint radius);
		Acad::ErrorStatus setNodeColor(UINT32 color);
		Acad::ErrorStatus setName(const TCHAR* name);
		Acad::ErrorStatus setLocation(const TCHAR* location);


		Acad::ErrorStatus changeIndex(AcDbHandle handle,INDEX index);

		const AcGePoint3d& getPosition() const;
		double getRadius() const;
		size_t getCountsOfBinded() const;
		UINT32 getNodeColor() const;
		const TCHAR* getName() const;
		const TCHAR* getLocation() const;
		const std::map<AcDbHandle, INDEX>& getHdlsd() const;

		void copyTunnel(const TunnelNode &);

	private:

		void draw2d(AcGiWorldDraw *mode) const;
		void draw3d(AcGiWorldDraw *mode) const;
	};

}
#endif //TUNNELNODE_H

