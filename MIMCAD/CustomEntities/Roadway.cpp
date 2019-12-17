#include "StdAfx.h"
#include "RoadwayNode.h"
#include "Roadway.h"


namespace MIM
{

	//*************************************************************************
	// Roadway Reactor 
	//*************************************************************************

	//将即将删除的节点绑定的巷道转移到另一个节点

	static void handleRoadwayNode(int i, RoadwayNode* otherRwnd, const Roadway* rdw);
	static void handleRoadway(int i, Roadway* otherRdw, const Roadway* rdw);

	ACRX_NO_CONS_DEFINE_MEMBERS(RoadwayReactor, AcDbObjectReactor);

	void RoadwayReactor::erased(const AcDbObject* dbObj, Adesk::Boolean bErasing)
	{
		if (bErasing == true)
		{
			Acad::ErrorStatus es = Acad::eOk;

			RoadwayNode* startNode = nullptr;
			RoadwayNode* endNode = nullptr;

			const Roadway *roadway = static_cast<const Roadway*>(dbObj);

			//自己的句柄
			AcDbHandle handle;
			roadway->getAcDbHandle(handle);

			OpenObject<RoadwayNode> open1(startNode, roadway->getStartNodeHandle(), AcDb::kForWrite),
				open2(endNode, roadway->getEndNodeHandle(), AcDb::kForWrite);

			es = open1.open();

			if (es == Acad::eOk)
			{
				//从节点中移除对该巷道的引用计数
				startNode->removeRoadway(handle);

				//如果引用为0，删除
				if (bErasing == true)
				{
					bool isErased = startNode->tryErase();
				}
			}
			else
			{
				int a = 0;
				int b = a++;
			}
			es = open2.open();

			if (es == Acad::eOk)
			{
				//从节点中移除对该巷道的引用计数
				endNode->removeRoadway(handle);

				//如果引用为0，删除
				if (bErasing == true)
				{
					bool isErased = endNode->tryErase();
				}
			}
			else
			{
				int a = 0;
				int b = a++;
			}
		}
	}


	//主要是移动后将重合的节点合并
	void RoadwayReactor::modified(const AcDbObject* dbObj)
	{

		if (dbObj->isEraseStatusToggled() || dbObj->isUndoing() || dbObj->isErased())
			return;

		Acad::ErrorStatus es = Acad::eOk;

		const Roadway *rdw = static_cast<const Roadway*>(dbObj);
		const AcGePoint3d& sp = rdw->getStartPoint();
		const AcGePoint3d& ep = rdw->getEndPoint();

		if (sp == ep)
			return;
		
		//移动结束后判断相交

		auto &moved = rdw->moved;

		for (int i = 0; i < 2; i++)
		{
			AcGePoint3d movedPoint;
			if (moved[i] == false)
				continue;
			else if (i == 0)  //移动的是起点
			{
				movedPoint = rdw->getStartPoint();
				moved[i] = false;
			}
			else   //移动的是终点
			{
				movedPoint = rdw->getEndPoint();
				moved[i] = false;
			}

			ads_name ss, name;
			AcDbObjectId id;
			Adesk::Int32 sslen;
			resbuf eb;
			ACHAR sbuf1[] = TEXT("RoadwayNode,Roadway");
			eb.restype = 0;
			eb.resval.rstring = sbuf1;
			eb.rbnext = nullptr;
			Roadway::startNodifying();
			int result = acedSSGet(NULL, asDblArray(movedPoint), NULL, &eb, ss);

			if (result != RTNORM)
			{
				acedSSFree(ss);
				continue;
			}
			acedSSLength(ss, &sslen);

			MIMDEBUGASSERT(sslen <= 1);

			for (int i = 0; i < sslen; i++)
			{
				acedSSName(ss, i, name);	
			}

			acedSSFree(ss);

			if ((es = acdbGetObjectId(id, name)) != Acad::eOk)
				continue;

			AcDbEntity* otherEntity = nullptr;
			OpenObject<AcDbEntity> open1(otherEntity, id, AcDb::kForWrite);
			if ((es = open1.open()) != Acad::eOk)
				continue;

			//如果相交在RoadwayNode
			if (otherEntity->isKindOf(RoadwayNode::desc()))
			{
				RoadwayNode* otherRwnd = static_cast<RoadwayNode*>(otherEntity);
				MIMDEBUGASSERT(otherRwnd != nullptr);

				handleRoadwayNode(i, otherRwnd, rdw);
			}
			//如果相交在Roadway
			else if (otherEntity->isKindOf(Roadway::desc()))
			{

				Roadway* otherRdw = static_cast<Roadway*>(otherEntity);
				MIMDEBUGASSERT(otherRdw != nullptr);

				handleRoadway(i,otherRdw,rdw);
			}
		}
	}

	//*************************************************************************
	// Statics functions used in this file. 
	//*************************************************************************
	static Acad::ErrorStatus intLine(const Roadway*         poly,
		const AcGeLine3d        line,
		AcGePoint3dArray& points);

	static Acad::ErrorStatus intLine(const Roadway*         poly,
		const AcDbLine*         line,
		AcDb::Intersect   intType,
		const AcGePlane*        projPlane,
		AcGePoint3dArray& points);

	static Acad::ErrorStatus intLine(const Roadway*         poly,
		const AcGeLineSeg3d     line,
		AcDb::Intersect   intType,
		const AcGePlane*        projPlane,
		AcGePoint3dArray& points);

	//*************************************************************************
	// Code for the Class Body. 
	//*************************************************************************


	

	ACRX_DXF_DEFINE_MEMBERS(Roadway, AcDbEntity, AcDb::kDHL_CURRENT,
		AcDb::kMReleaseCurrent, 0, ROADWAY, /*MSG0*/"KLND");

	Roadway::Roadway() :m_segmentCount(1),
		m_startPoint(0, 0, 0), m_endPoint(1000, 0, 0),
		m_name(NULL), m_animationSwitch(false),
		m_arrowPoint(m_startPoint),
		m_startNodeHandle(nullptr), m_endNodeHandle(nullptr),
		m_colorIndex(std::shared_ptr<Adesk::UInt16>(new Adesk::UInt16[2], std::default_delete<Adesk::UInt16[]>())),
		m_tmp(new TmpPara()), m_extensionData(NULL),
		m_reactor(std::shared_ptr<RoadwayReactor>(new RoadwayReactor()))
	{
		this->addReactor(m_reactor.get());

		for (int i = 0; i < 2; i++)
		{
			m_colorIndex.get()[i] = i + 1;
		}

		//巷道面列表的集合
		m_tmp->FaceIndexPtrArray =
			std::shared_ptr<Adesk::Int32>
			(new Adesk::Int32[m_segmentCount * 20], std::default_delete<Adesk::Int32[]>());

		//巷道上所有点的集合
		m_tmp->PointPtrArray = std::shared_ptr<AcGePoint3d>
			(new AcGePoint3d[(m_segmentCount + 1) * 4], std::default_delete<AcGePoint3d[]>());

		//顶点颜色属性
		m_tmp->pTmpVerticesColorList = std::shared_ptr<AcCmEntityColor>
			(new AcCmEntityColor[(m_segmentCount + 1) * 4], std::default_delete<AcCmEntityColor[]>());

		//shell顶点属性
		m_tmp->VertexData = std::shared_ptr<AcGiVertexData>(new AcGiVertexData());

	}

	Roadway::~Roadway()
	{
		acutDelString(m_name);
		acutDelString(m_extensionData);
		delete m_tmp;
	}

	//----------------------------------------------------------------------

	Acad::ErrorStatus
		Roadway::dwgOutFields(AcDbDwgFiler *pFiler) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = AcDbEntity::dwgOutFields(pFiler);
		if (es != Acad::eOk)
			return (es);

		Adesk::Int16 version = VERSION_RW;
		pFiler->writeItem(version);

		pFiler->writeUInt32(m_segmentCount);

		pFiler->writePoint3d(m_startPoint);
		pFiler->writePoint3d(m_endPoint);

		for (uint i = 0; i < m_segmentCount * 2; i++)
		{
			pFiler->writeUInt16(m_colorIndex.get()[i]);
		}

		pFiler->writeAcDbHandle(m_startNodeHandle);
		pFiler->writeAcDbHandle(m_endNodeHandle);

		if (m_extensionData)
			pFiler->writeString(m_extensionData);
		else
			pFiler->writeString(TEXT(""));

		if (m_name)
			pFiler->writeString(m_name);
		else
			pFiler->writeString(TEXT(""));


		return pFiler->filerStatus();
	}

	//----------------------------------------------------------------------------------------------------------------//

	Acad::ErrorStatus
		Roadway::dwgInFields(AcDbDwgFiler *pFiler)
	{
		assertWriteEnabled();
		Acad::ErrorStatus es = AcDbEntity::dwgInFields(pFiler);
		if (es != Acad::eOk)
			return (es);

		Adesk::Int16 version;

		pFiler->readItem(&version);
		if (version > VERSION_RW)
			return Acad::eMakeMeProxy;

		pFiler->readUInt32(&m_segmentCount);

		pFiler->readPoint3d(&m_startPoint);
		pFiler->readPoint3d(&m_endPoint);

		for (uint i = 0; i < 2; i++)
		{
			pFiler->readUInt16(&m_colorIndex.get()[i]);
		}

		pFiler->readAcDbHandle(&m_startNodeHandle);
		pFiler->readAcDbHandle(&m_endNodeHandle);
		
		acutDelString(m_extensionData);
		pFiler->readString(&m_extensionData);

		acutDelString(m_name);
		pFiler->readString(&m_name);

		return pFiler->filerStatus();
	}

	Acad::ErrorStatus 
		Roadway::subClose()
	{
		Acad::ErrorStatus es = AcDbEntity::subClose();
		if (es != Acad::eOk)
			return (es);

		if (!(this->isWriteEnabled()))
		{
			return Acad::eOk;
		}
		else
		{
			if (preBuf.size() > 0)
			{
				setPreBuf();
			}
			
			return Acad::eOk;
		}
	}

	//----------------------------------------------------------------------------------------------------------------//

	//----------------------------------------------------------------------------------------------------------------//
	//
	//Acad::ErrorStatus
	//Roadway::subHighlight(const AcDbFullSubentPath& path, const Adesk::Boolean highlightAll) const
	//{
	//	if (m_animationSwitch == true)
	//		return Acad::eWrongCellType;
	//
	//	AcDbObjectId id = objectId();
	//#ifdef _DEBUG
	//	acutPrintf(_T("\nHighlight %d"), id.asOldId());
	//#endif
	//	//acdbQueueForRegen(&id, 1);
	//	return AcDbEntity::subHighlight(path, highlightAll);
	//}
	//
	//Acad::ErrorStatus
	//Roadway::subUnhighlight(const AcDbFullSubentPath& path, const Adesk::Boolean highlightAll) const
	//{
	//	if (m_animationSwitch == true)
	//		return Acad::eWrongCellType;
	//
	//
	//	AcDbObjectId id = objectId();
	//#ifdef _DEBUG
	//	acutPrintf(_T("\nUnhighlight %d"), id.asOldId());
	//#endif
	//	//acdbQueueForRegen(&id, 1);
	//	return AcDbEntity::subUnhighlight(path, highlightAll);
	//}

	//----------------------------------------------------------------------------------------------------------------//
	//----------------------------------------------------------------------------------------------------------------//

	Adesk::Boolean Roadway::subWorldDraw(AcGiWorldDraw *mode)
	{
		assertReadEnabled();

		if (mode->regenAbort()) {
			//clear the drag flags once we are drawn
			return Adesk::kTrue;
		}
		//巷道中心线向量
		AcGeVector3d  centerVector = (m_endPoint - m_startPoint);

		if (m_animationSwitch == true)
		{
			if (mode->regenType() == AcGiRegenType::kAcGiStandardDisplay)
			{
				AcGePoint3d ptArray[5];
				ptArray[0] = m_tmp->PointPtrArray.get()[0];
				ptArray[1] = m_tmp->PointPtrArray.get()[m_segmentCount * 4];
				ptArray[2] = m_tmp->PointPtrArray.get()[m_segmentCount * 4 + 3];
				ptArray[3] = m_tmp->PointPtrArray.get()[3];
				ptArray[4] = m_tmp->PointPtrArray.get()[0];

				mode->geometry().polyline(5, ptArray);
			}
			else
			{
				getVerticesColorList();
				m_tmp->VertexData->setTrueColors(m_tmp->pTmpVerticesColorList.get());

				//shell:
				mode->geometry().shell((
					m_segmentCount + 1) * 4,
					m_tmp->PointPtrArray.get(),
					20 * m_segmentCount,
					m_tmp->FaceIndexPtrArray.get(),
					NULL,
					NULL,
					m_tmp->VertexData.get());

				//箭头
				double width = abs((m_tmp->PointPtrArray.get()[0] - m_tmp->PointPtrArray.get()[3]).length());
				double height = abs((m_tmp->PointPtrArray.get()[0] - m_tmp->PointPtrArray.get()[1]).length());
				AcGeVector3d hv = (m_tmp->PointPtrArray.get()[0] - m_tmp->PointPtrArray.get()[3]).normal();
				AcGeVector3d vv = (m_tmp->PointPtrArray.get()[0] - m_tmp->PointPtrArray.get()[1]).normal();
				drawArrow(mode, centerVector, hv, vv, height, width);

			}

			return (AcDbEntity::subWorldDraw(mode));
		}


		//断面形状，最初的截面
		AcGePoint3dArray pts;

		double width = 10;
		double height = 10;

		pts.append(AcGePoint3d(width*0.5, 0, 0));
		pts.append(AcGePoint3d(width*0.5, height, 0));
		pts.append(AcGePoint3d(-width*0.5, height, 0));
		pts.append(AcGePoint3d(-width*0.5, 0, 0));

		//断面定点数量
		const Adesk::UInt32 numOfFaceVerticesCount = pts.length();


		//巷道方向的“宽”方向向量
		AcGeVector3d horizontalVector = centerVector.perpVector();

		//巷道方向到Z方向角度 90度
		double angleToZ = AcGeVector3d::kZAxis.angleTo(centerVector);
		for (int i = 0; i < pts.length(); i++)
		{
			pts.at(i).rotateBy(angleToZ, horizontalVector);
		}

		double angleToCenter = (-AcGeVector3d::kYAxis).angleTo(centerVector, AcGeVector3d::kZAxis);
		//点的旋转
		for (int i = 0; i < pts.length(); i++)
		{
			pts.at(i).rotateBy(angleToCenter, centerVector);
		}
		
		//点的平移
		for (int i = 0; i < pts.length(); i++)
		{
			pts.at(i) += (m_startPoint - AcGePoint3d(0, 0, 0));
			pts.at(i) += AcGeVector3d(0, 0, -height*0.5);
			
		}

		//AcGeLineSeg3d geLine3d(m_startPoint, m_endPoint);
		//AcGePoint3dArray segmentPoints;
		//geLine3d.getSamplePoints(m_segmentCount + 1, segmentPoints);

		for (uint i = 0; i < m_segmentCount + 1; i++)
		{
			m_tmp->PointPtrArray.get()[4 * i] = pts.at(0) + centerVector*i;
			m_tmp->PointPtrArray.get()[4 * i + 1] = pts.at(1) + centerVector*i;
			m_tmp->PointPtrArray.get()[4 * i + 2] = pts.at(2) + centerVector*i;
			m_tmp->PointPtrArray.get()[4 * i + 3] = pts.at(3) + centerVector*i;
		}


		//面列表
		getFaceList();

		//顶点颜色数据列表
		getVerticesColorList();

		m_tmp->VertexData->setTrueColors(m_tmp->pTmpVerticesColorList.get());

		/*
		//设计标注
		//计算每个整体面上其中一个对角线，一共4个
		AcGeLineSeg3d diagonal[4];
		//找到标注点,每个面的中心点
		AcGePoint3d standardPoints[4];
		for (size_t i = 0; i < 4; i++)
		{
			AcGeVector3d v;
			if (i == 3)
			{
				diagonal[i].set(vertices.get()[i], vertices.get()[4 * m_segmentCount]);
				v = vertices.get()[i] - vertices.get()[0];
			}
			else
			{
				diagonal[i].set(vertices.get()[i], vertices.get()[4 * m_segmentCount + i + 1]);
				v = vertices.get()[i] - vertices.get()[i+1];
			}

			standardPoints[i] = diagonal[i].midPoint();

			v.rotateBy(PI / 2, AcGeVector3d::kXAxis);
			AcGePoint3d p = standardPoints[i] + v;
			AcGePoint3d ps[2];
			ps[0] = standardPoints[i];
			ps[1] = p;
			mode->geometry().polyline(2, ps);
		}

		*/

		if (mode->regenType() == AcGiRegenType::kAcGiStandardDisplay)
		{
			AcGePoint3d ptArray[5];
			ptArray[0] = m_tmp->PointPtrArray.get()[0];
			ptArray[1] = m_tmp->PointPtrArray.get()[m_segmentCount * 4];
			ptArray[2] = m_tmp->PointPtrArray.get()[m_segmentCount * 4 + 3];
			ptArray[3] = m_tmp->PointPtrArray.get()[3];
			ptArray[4] = m_tmp->PointPtrArray.get()[0];

			mode->geometry().polyline(5, ptArray);
		}
		else
		{
			//shell:
			mode->geometry().shell(
				numOfFaceVerticesCount*(m_segmentCount + 1),
				m_tmp->PointPtrArray.get(),
				20 * m_segmentCount,
				m_tmp->FaceIndexPtrArray.get(),
				NULL,
				NULL,
				m_tmp->VertexData.get());

			//箭头
			double width = abs((m_tmp->PointPtrArray.get()[0] - m_tmp->PointPtrArray.get()[3]).length());
			double height = abs((m_tmp->PointPtrArray.get()[0] - m_tmp->PointPtrArray.get()[1]).length());
			AcGeVector3d hv = (m_tmp->PointPtrArray.get()[0] - m_tmp->PointPtrArray.get()[3]).normal();
			AcGeVector3d vv = (m_tmp->PointPtrArray.get()[0] - m_tmp->PointPtrArray.get()[1]).normal();
			drawArrow(mode, centerVector, hv, vv, height, width);
		}

		return (AcDbEntity::subWorldDraw(mode));
	}




	//----------------------------------------------------------------------------------------------------------------//

	Acad::ErrorStatus Roadway::subGetOsnapPoints(
		AcDb::OsnapMode osnapMode,
		Adesk::GsMarker gsSelectionMark,
		const AcGePoint3d &pickPoint,
		const AcGePoint3d &lastPoint,
		const AcGeMatrix3d &viewXform,
		AcGePoint3dArray &snapPoints,
		AcDbIntArray &geomIds) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = Acad::eOk;

		switch (osnapMode)
		{
		case AcDb::kOsModeEnd:
		{
			snapPoints.append(m_startPoint);
			snapPoints.append(m_endPoint);
			break;
		}	
		case AcDb::kOsModeMid:
		{
			snapPoints.append(AcGePoint3d((m_startPoint.x + m_endPoint.x) / 2, (m_startPoint.y + m_endPoint.y) / 2, (m_startPoint.z + m_endPoint.z) / 2));
			break;
		}		
		case AcDb::kOsModeCen:
			break;
		case AcDb::kOsModeNode:
			break;
		case AcDb::kOsModeQuad:
			break;
		case AcDb::kOsModeIns:
			break;
		case AcDb::kOsModePerp:
			break;
		case AcDb::kOsModeTan:
			break;
		case AcDb::kOsModeNear:
			break;
		case AcDb::kOsModeCentroid:
		{
			AcGeLineSeg3d tmpSeg(m_startPoint, m_endPoint);
			AcGePoint3dArray pointArray;

			tmpSeg.getSamplePoints(100, pointArray);
			for (auto &point : pointArray)
			{
				snapPoints.append(point);
			}
			break;
		}	
		default:
			break;
		}
		return es;
	}

	//----------------------------------------------------------------------------------------------------------------//

	Acad::ErrorStatus Roadway::subGetGripPoints(
		AcGePoint3dArray &gripPoints, AcDbIntArray &osnapModes, AcDbIntArray &geomIds
	) const {
		assertReadEnabled();

		Acad::ErrorStatus es = Acad::eOk;
		gripPoints.append(m_startPoint);
		gripPoints.append(m_endPoint);

		return es;
	}

	//----------------------------------------------------------------------------------------------------------------//

	Acad::ErrorStatus Roadway::subMoveGripPointsAt(const AcDbIntArray &indices, const AcGeVector3d &offset)
	{
		if (indices.length() == 0 || offset.isZeroLength())
			return Acad::eOk; //that's easy :-)

		else
		{
			// Only if we're not dragging do we want to make an undo
			// recording and check if the object's open for write.
			//
			assertWriteEnabled();
			for (int i = 0; i < indices.length(); i++)
			{
				int idx = indices.at(i);

				acutIsPrint(offset.x);

				if (idx == 0) 
					m_startPoint += offset;
				if (idx == 1) 
					m_endPoint += offset;
			}
			return Acad::eOk;
		}
	}

	//----------------------------------------------------------------------------------------------------------------//

	Acad::ErrorStatus
		Roadway::subIntersectWith(
			const AcDbEntity* ent,
			AcDb::Intersect intType,
			const AcGePlane& projPlane,
			AcGePoint3dArray& points,
			Adesk::GsMarker /*thisGsMarker*/,
			Adesk::GsMarker /*otherGsMarker*/) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = Acad::eOk;
		if (ent == NULL)
			return Acad::eNullEntityPointer;

		if (ent->isKindOf(AcDbLine::desc())) {
			if ((es = intLine(this, AcDbLine::cast(ent),
				intType, &projPlane, points)) != Acad::eOk)
			{
				return es;
			}
		}
		return es;
	}

	//----------------------------------------------------------------------------------------------------------------//

	Acad::ErrorStatus   Roadway::subIntersectWith(
		const AcDbEntity*   ent,
		AcDb::Intersect     intType,
		AcGePoint3dArray&   points,
		Adesk::GsMarker     thisGsMarker,
		Adesk::GsMarker     otherGsMarker)
		const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = Acad::eOk;
		if (ent == NULL)
			return Acad::eNullEntityPointer;
		if (ent->isKindOf(AcDbLine::desc())) {
			if ((es = intLine(this, AcDbLine::cast(ent),
				intType, NULL, points)) != Acad::eOk)
			{
				return es;
			}
		}
		else if (ent->isKindOf(Roadway::desc()))
		{
			AcGePoint3dArray vertexArray;
			vertexArray.append(getStartPoint());
			vertexArray.append(getEndPoint());

			if (intType == AcDb::kExtendArg
				|| intType == AcDb::kExtendBoth)
			{
				intType = AcDb::kExtendThis;
			}
			AcDbLine *pAcadLine;
			for (int i = 0; i < vertexArray.length() - 1; i++) {
				pAcadLine = new AcDbLine();
				pAcadLine->setStartPoint(vertexArray[i]);
				pAcadLine->setEndPoint(vertexArray[i + 1]);
				pAcadLine->setNormal(AcGeVector3d(0, 0, 1));

				if ((es = ent->intersectWith(pAcadLine, intType,
					points)) != Acad::eOk)
				{
					delete pAcadLine;
					return es;
				}
				delete pAcadLine;
			}

			const Roadway *rdw = static_cast<const Roadway*>(ent);
			const AcGePoint3d& startPoint = rdw->getStartPoint();
			const AcGePoint3d& endPoint = rdw->getEndPoint();
			AcGePoint3d insPoint;

			if (points.length() > 0)
			{
				insPoint = points.at(0);
			}


			if (endPoint == insPoint)
			{
				AcDbHandle handle;
				handle.setNull();
				this->getAcDbHandle(handle);
				bool ttt = handle.isNull();
				bool mmm = this->isModified();
				int a = 10;
			}
		}
		return es;
	}



	//----------------------------------------------------------------------------------------------------------------//

	Acad::ErrorStatus  Roadway::subTransformBy(const AcGeMatrix3d& xform)
	{

		assertWriteEnabled();
		m_startPoint.transformBy(xform);
		m_endPoint.transformBy(xform);

		return Acad::eOk;
	}

	Acad::ErrorStatus Roadway::subExplode(AcDbVoidPtrArray& entitySet) const
	{
		assertReadEnabled();

		Acad::ErrorStatus es = Acad::eOk;

		uint32_t pointsCount = (m_segmentCount + 1) * 4;
		uint32_t facesCount = m_segmentCount * 4 * 5;
		AcGePoint3dArray vertexArray;
		vertexArray.setPhysicalLength(pointsCount);

		AcArray<AcCmEntityColor> clrArray;
		for (uint32_t i = 0; i < pointsCount; i++)
		{
			vertexArray.append(m_tmp->PointPtrArray.get()[i]);
			clrArray.append(m_tmp->pTmpVerticesColorList.get()[i]);
		}

		AcArray<Adesk::Int32> faceArray;
		faceArray.setPhysicalLength(facesCount);

		for (uint32_t i = 0; i < facesCount; i++)
		{
			faceArray.append(m_tmp->FaceIndexPtrArray.get()[i]);
		}

		AcDbSubDMesh *pSubDMesh = new AcDbSubDMesh();

		es = pSubDMesh->setSubDMesh(vertexArray, faceArray, 0);

		AcDbBlockTable *pBlockTable;

		AcDbBlockTableRecord *pSpaceRecord;

		es = acdbHostApplicationServices()->workingDatabase()
			->getSymbolTable(pBlockTable, AcDb::kForRead);

		es = pBlockTable
			->getAt(ACDB_MODEL_SPACE, pSpaceRecord, AcDb::kForWrite);

		es = pBlockTable->close();

		AcDbObjectId meshId = AcDbObjectId::kNull;

		es = pSpaceRecord->appendAcDbEntity(meshId, pSubDMesh);

		es = pSubDMesh->setVertexColorArray(clrArray);

		es = pSpaceRecord->close();

		entitySet.append(pSubDMesh);

		es = pSubDMesh->close();


		return es;

	}

	void  Roadway::subList() const
	{
		assertReadEnabled();

		AcDbEntity::subList();


		acutPrintf(TEXT("%22s X = %-9.16q0, Y = %-9.16q0, Z = %-9.16q0\n "), TEXT("起点: "),
			m_startPoint.x, m_startPoint.y, m_startPoint.z);

		acutPrintf(TEXT("%21s X = %-9.16q0, Y = %-9.16q0, Z = %-9.16q0\n "), TEXT("终点: "),
			m_endPoint.x, m_endPoint.y, m_endPoint.z);

		double length = (m_endPoint - m_startPoint).length();
		acutPrintf(TEXT("%20s %-9.16q0 \n"), TEXT("长度: "), length);

	}
	//----------------------------------------------------------------------------------------------------------------//
	//----------------------------------------------------------------------------------------------------------------//
	//----------------------------------------------------------------------------------------------------------------//


	bool Roadway::getIsNodifying()
	{
		return isNodifying;
	}

	void Roadway::startNodifying()
	{
		isNodifying = true;
	}

	void Roadway::endNodifying()
	{
		isNodifying = false;
	}

	inline Acad::ErrorStatus Roadway::setStartPoint(const AcGePoint3d& sp)
	{
		assertWriteEnabled();
		if (m_startPoint != sp)
		{
			m_startPoint = sp;
		}
		moved[0] = true;
		return Acad::eOk;
	}


	inline Acad::ErrorStatus Roadway::setEndPoint(const AcGePoint3d& ep)
	{
		assertWriteEnabled();
		m_endPoint = ep;

		moved[1] = true;

		return Acad::eOk;
	}


	Acad::ErrorStatus Roadway::setStartNodeHandle(const AcDbHandle& handle)
	{
		if (this->isWriteEnabled())
		{
			assertWriteEnabled();
			m_startNodeHandle = handle;
		}
		else
		{
			m_startNodeHandle = handle;
			BufPair buf;
			buf.type = ERoadwayPreBuf::eStardNodeHandle;
			buf.buf = handle;
			preBuf.push_back(std::move(buf));
		}
		
		return Acad::eOk;
	}

	Acad::ErrorStatus Roadway::setEndNodeHandle(const AcDbHandle& handle)
	{
		if (this->isWriteEnabled())
		{
			assertWriteEnabled();
			m_endNodeHandle = handle;
		}
		else
		{
			m_endNodeHandle = handle;
			BufPair buf;
			buf.type = ERoadwayPreBuf::eEndNodeHandle;
			buf.buf = handle;
			preBuf.push_back(std::move(buf));
		}

		
		return Acad::eOk;
	}

	Acad::ErrorStatus Roadway::setName(const TCHAR* name)
	{
		assertWriteEnabled();
		acutDelString(m_name);
		m_name = NULL;
		if (name != NULL)
		{
			acutUpdString(name, m_name);
		}
		return Acad::eOk;
	}

	Acad::ErrorStatus Roadway::setExtensionData(const TCHAR* jsonStr)
	{

		assertWriteEnabled();
		acutDelString(m_extensionData);
		m_extensionData = NULL;
		if (jsonStr != NULL)
		{
			acutUpdString(jsonStr, m_extensionData);
		}
		return Acad::eOk;

		
	}

	Acad::ErrorStatus
		Roadway::setAnimateSwich(bool swich)
	{
		assertWriteEnabled();
		m_animationSwitch = swich;
		return Acad::eOk;
	}

	//----------------------------------------------------------------------------------------------------------------//

	const AcGePoint3d& Roadway::getStartPoint() const
	{
		assertReadEnabled();
		return  m_startPoint;
	}


	const AcGePoint3d& Roadway::getEndPoint() const
	{
		assertReadEnabled();
		return  m_endPoint;
	}

	const TCHAR* Roadway::getName() const
	{
		assertReadEnabled();
		return m_name;
	}

	const AcDbHandle& Roadway::getStartNodeHandle() const
	{
		assertReadEnabled();
		return m_startNodeHandle;
	}

	const AcDbHandle& Roadway::getEndNodeHandle() const
	{
		assertReadEnabled();
		return m_endNodeHandle;
	}

	const TCHAR* Roadway::getExtensionData() const
	{
		assertReadEnabled();
		return m_extensionData;
	}


	bool Roadway::getAnimateSwich() const
	{
		assertReadEnabled();
		return m_animationSwitch;
	}

	//----------------------------------------------------------------------------------------------------------------//
	//----------------------------------------------------------------------------------------------------------------//
	//----------------------------------------------------------------------------------------------------------------//

	static Acad::ErrorStatus intLine(const Roadway*         poly,
		const AcGeLine3d        line,
		AcGePoint3dArray& points)
	{
		Acad::ErrorStatus es = Acad::eOk;

		AcGePoint3dArray vertexArray;

		vertexArray.append(poly->getStartPoint());
		vertexArray.append(poly->getEndPoint());

		AcGeLineSeg3d tlnsg;
		AcGePoint3d   pt;

		for (int i = 0; i < vertexArray.length() - 1; i++) {

			tlnsg.set(vertexArray[i], vertexArray[i + 1]);

			if (!tlnsg.intersectWith(line, pt)) {
				continue;
			}
			else {
				points.append(pt);
			}
		}

		return es;
	}

	static Acad::ErrorStatus intLine(const Roadway*         poly,
		const AcGeLineSeg3d     lnsg,
		AcDb::Intersect   intType,
		const AcGePlane*        projPlane,
		AcGePoint3dArray& points)
	{
		Acad::ErrorStatus es = Acad::eOk;

		AcGePoint3dArray vertexArray;
		//if ((es = poly->getVertices3d(vertexArray)) != Acad::eOk) {
		//	return es;
		//   }

		vertexArray.append(poly->getStartPoint());
		vertexArray.append(poly->getEndPoint());

		AcGeLine3d aline(lnsg.startPoint(), lnsg.endPoint());
		AcGeLineSeg3d tlnsg;
		AcGePoint3d   pt;
		AcGePoint3d   dummy;

		for (int i = 0; i < vertexArray.length() - 1; i++) {

			tlnsg.set(vertexArray[i], vertexArray[i + 1]);

			if (intType == AcDb::kExtendArg || intType == AcDb::kExtendBoth) {
				if (projPlane == NULL) {
					if (!tlnsg.intersectWith(aline, pt)) {
						continue;
					}
					else {
						points.append(pt);
					}
				}
				else {
					if (!tlnsg.projIntersectWith(aline, projPlane->normal(),
						pt, dummy))
					{
						continue;
					}
					else {
						points.append(pt);
					}
				}
			}
			else {
				if (projPlane == NULL) {
					if (!tlnsg.intersectWith(lnsg, pt)) {
						continue;
					}
					else {
						points.append(pt);
					}
				}
				else {
					if (!tlnsg.projIntersectWith(lnsg, projPlane->normal(),
						pt, dummy))
					{
						continue;
					}
					else {
						points.append(pt);
					}
				}
			}
		}
		if (points.length() != 0)
			AcGePoint3d tp = points.at(0);
		return es;
	}

	static Acad::ErrorStatus intLine(const Roadway*         poly,
		const AcDbLine*         line,
		AcDb::Intersect   intType,
		const AcGePlane*        projPlane,
		AcGePoint3dArray& points)
	{
		Acad::ErrorStatus es = Acad::eOk;

		AcGeLineSeg3d lnsg(line->startPoint(), line->endPoint());
		es = intLine(poly, lnsg, intType, projPlane, points);

		//判断两个物体的首尾关系
		AcGeLineSeg3d tmplines[2];
		tmplines[0].set(line->startPoint(), line->endPoint());
		tmplines[1].set(poly->getStartPoint(), poly->getEndPoint());

		AcGePoint3d intersectPoint;
		tmplines[0].intersectWith(tmplines[1], intersectPoint);
		AcGeLineSeg3d frontLine, backLine;
		if (intersectPoint == tmplines[0].startPoint() && intersectPoint == tmplines[1].endPoint())
		{
			frontLine = tmplines[1];
			backLine = tmplines[0];
		}
		else if (intersectPoint == tmplines[1].startPoint() && intersectPoint == tmplines[0].endPoint())
		{
			frontLine = tmplines[0];
			backLine = tmplines[1];
		}
		else
		{
			return Acad::eOk;
		}

		AcGePoint3d p1, p2, p3, p4;
		p1 = frontLine.startPoint(); p2 = frontLine.endPoint();
		p3 = backLine.startPoint(); p4 = backLine.endPoint();

		AcGeVector3d v1 = frontLine.startPoint() - frontLine.endPoint();
		AcGeVector3d v2 = backLine.endPoint() - backLine.startPoint();
		double angelfronttoback = v1.angleTo(v2);          //两个向量的夹角
		double dushu = angelfronttoback * 180 / 3.14159265358979323846;
		double halfangel = angelfronttoback / 2;

		AcGeVector3d transfVector1(v1), transfVector2(v2);
		AcGeMatrix3d mat1, mat2;
		mat1.setToRotation(halfangel, AcGeVector3d::kZAxis, frontLine.endPoint());
		transfVector1.transformBy(mat1);
		transfVector1.rotateBy(halfangel, AcGeVector3d::kZAxis);
		transfVector2.rotateBy(halfangel, AcGeVector3d::kZAxis);

		//AcGeVector3d v1 = poly->getStartPoint() - poly->getEndPoint();
		//AcGeVector3d v2 = line->startPoint() - line->endPoint();
		//double angel = 3.14159265358979323846 - v1.angleTo(v2);
		//double dushu = angel * 180 / 3.14159265358979323846;
		//double halfangel = angel / 2;
		//double hdushu = halfangel * 180 / 3.14159265358979323846;
		//
		//AcGeVector3d transfVector1(v1), transfVector2(v2);
		//transfVector1.rotateBy(halfangel,v1).normalize();
		//transfVector2.rotateBy(halfangel,v2).normalize();

		AcGeLine3d ln(line->startPoint(), line->endPoint());
		//es = intLine(poly,ln, points);

		return es;
	}

	//计算面列表
	void Roadway::getFaceList()
	{
		Adesk::Int32* faceList = m_tmp->FaceIndexPtrArray.get();
		for (uint32_t i = 0; i < m_segmentCount; i++)
		{
			//下面
			faceList[20 * i] = 4;
			faceList[20 * i + 1] = i * 4;
			faceList[20 * i + 2] = (i + 1) * 4;
			faceList[20 * i + 3] = (i + 1) * 4 + 1;
			faceList[20 * i + 4] = i * 4 + 1;
			//背面
			faceList[20 * i + 5] = 4;
			faceList[20 * i + 6] = i * 4 + 1;
			faceList[20 * i + 7] = (i + 1) * 4 + 1;
			faceList[20 * i + 8] = (i + 1) * 4 + 2;
			faceList[20 * i + 9] = i * 4 + 2;
			//上面
			faceList[20 * i + 10] = 4;
			faceList[20 * i + 11] = i * 4 + 2;
			faceList[20 * i + 12] = i * 4 + 3;
			faceList[20 * i + 13] = (i + 1) * 4 + 3;
			faceList[20 * i + 14] = (i + 1) * 4 + 2;
			//前面
			faceList[20 * i + 15] = 4;
			faceList[20 * i + 16] = i * 4 + 3;
			faceList[20 * i + 17] = i * 4;
			faceList[20 * i + 18] = (i + 1) * 4;
			faceList[20 * i + 19] = (i + 1) * 4 + 3;

		}
	}

	//计算顶点颜色列表
	void Roadway::getVerticesColorList()
	{
		for (uint i = 0; i < m_segmentCount + 1; i++)
		{
			m_tmp->pTmpVerticesColorList.get()[4 * i].setColorIndex(m_colorIndex.get()[i]);
			m_tmp->pTmpVerticesColorList.get()[4 * i + 1].setColorIndex(m_colorIndex.get()[i]);
			m_tmp->pTmpVerticesColorList.get()[4 * i + 2].setColorIndex(m_colorIndex.get()[i]);
			m_tmp->pTmpVerticesColorList.get()[4 * i + 3].setColorIndex(m_colorIndex.get()[i]);
		}
	}


	void  Roadway::drawArrow(
		AcGiWorldDraw *mode,
		const AcGeVector3d& cv,
		const AcGeVector3d& nhv,	 //横向偏移
		const AcGeVector3d& nvv,	 //竖直方向偏移
		double h,                   //截面高度
		double w                    //截面宽度
	)
	{
		bool changed = false;
		int numofpoints = 4;
		AcGePoint3d p0, p1, p2, p3;
		AcGeVector3d normalCenterVector;


		if (!(m_tmp->CenterVector == cv))   //巷道位置变化了
		{
			m_tmp->CenterVector = cv;
			normalCenterVector = cv.normal();  //底面中心线单位向量
			changed = true;
			m_tmp->ArrawPosition = AcGePoint3d
			(
				0.5*(m_startPoint.x + m_endPoint.x),
				0.5*(m_startPoint.y + m_endPoint.y),
				0.5*(m_startPoint.z + m_endPoint.z)
			);
		} //巷道没有变化
		else
		{
			changed = false;
		}

		if (m_animationSwitch == false)  //静态模式时
		{
			m_arrowPoint = m_tmp->ArrawPosition;
		}
		else   //动画模式时
		{
			AcGeVector3d v1, v2;
			if (changed == true)
			{
				m_arrowPoint = m_tmp->ArrawPosition;
				changed = false;
			}
			v1 = m_arrowPoint - m_startPoint;
			v2 = m_endPoint - m_startPoint;

			//走到endPoint的时候重置位置
			if (v1.length() > v2.length())
				m_arrowPoint = m_startPoint;
			else
			{
				m_arrowPoint += normalCenterVector * 1;
			}
		}

		//前后两个点
		p0 = m_arrowPoint + normalCenterVector*w - nvv*h;
		p2 = m_arrowPoint + normalCenterVector*w*0.3 - nvv*h;
		//左右两个点
		p1 = m_arrowPoint + nhv*w*0.5 - nvv*h;
		p3 = m_arrowPoint - nhv*w*0.5 - nvv*h;

		AcGePoint3d pts[] = { p0,p1,p2,p3 };
		Adesk::Int32 faceList[] = { 3, 0, 1, 2, 3, 0, 2, 3 };

		//颜色
		/*AcGiVertexData vertexData;
		AcCmEntityColor colorList[4];

		for (auto &v : colorList)
		{
			v.setColorIndex(7);
		}

		vertexData.setTrueColors(colorList);*/

		mode->geometry().shell(numofpoints, pts, 8, faceList, nullptr, nullptr, nullptr);

		for (int j = 0; j < 3; j++)
		{
			for (int i = 0; i < 4; i++)
			{
				pts[i].rotateBy(PI / 2, normalCenterVector, m_arrowPoint);
			}
			mode->geometry().shell(numofpoints, pts, 8, faceList, nullptr, nullptr, nullptr);
		}

	}


	static void moveRoadwayBind(const Roadway* rdw, int i, RoadwayNode* retain, RoadwayNode * erase, bool retainIsthis)
	{
		Acad::ErrorStatus es = Acad::eOk;
		//判断两个节点是否是同一个，是的话不做处理
		AcDbHandle eraseNodeHandle1, retainNodehandle;
		erase->getAcDbHandle(eraseNodeHandle1);
		retain->getAcDbHandle(retainNodehandle);

		if (eraseNodeHandle1 == retainNodehandle)
		{
			return;
		}

#ifdef _DEBUG
		size_t count = retain->getCountsOfBinded();
		retain->copyRoadway(*erase);
		count = retain->getCountsOfBinded();
#else
		retain->copyRoadway(*erase);
#endif // _DEBUG

		const std::map<AcDbHandle, WHICHSIDE>& hdlsd = retain->getHdlsd();
		
		erase->clearRoadway();
		erase->tryErase();

		if (retainIsthis == true)
		{

			for (auto &v : hdlsd)
			{

				Roadway* otherRdw = nullptr;
				OpenObject<Roadway> open1(otherRdw, v.first, OpenMode::kForWrite);

				es = open1.open();
				if (es == Acad::eOk)
				{

					if (v.second == STARTSIDE)
					{
						otherRdw->setStartNodeHandle(retainNodehandle);

					}
					else if (v.second == ENDSIDE)
					{
						otherRdw->setEndNodeHandle(retainNodehandle);
					}
				}
			}
		}
		else
		{
			if (i == 0)
			{
				const_cast<Roadway*>(rdw)->setStartNodeHandle(retainNodehandle);
			}
			else
			{
				const_cast<Roadway*>(rdw)->setEndNodeHandle(retainNodehandle);
			}
		}
	}

	static Acad::ErrorStatus getRwnd(int i, const Roadway* rdw, RoadwayNode*& Rwnd, OpenMode mode = OpenMode::kForRead)
	{
		Acad::ErrorStatus es = Acad::eOk;

		AcDbHandle thisNodeHanle = nullptr;

		if (i == 0)
		{
			thisNodeHanle = rdw->getStartNodeHandle();
		}
		else
		{
			thisNodeHanle = rdw->getEndNodeHandle();
		}

		OpenObject<RoadwayNode> open1(Rwnd, thisNodeHanle, mode);
		es = open1.open();

		if (es != Acad::eOk)
		{
			if (es == Acad::eWasOpenForWrite)
			{
				return es;
			}
			else if (es == Acad::eWasNotifying)
			{
				return es;
			}
			else
			{
				MIMDEBUGASSERT(false);
				return es;
			}
		}
		return es;
	}

	void Roadway::setPreBuf()
	{
		for (auto &v : preBuf)
		{
			switch (v.type)
			{
			case eStardNodeHandle:
			{
				setStartNodeHandle(v.buf.AnyCast<AcDbHandle>());
				break;
			}
			case eEndNodeHandle:
			{
				setEndNodeHandle(v.buf.AnyCast<AcDbHandle>());
				break;
			}
			case eExtensionData:
			{
				break;
			}		
			default:
				break;
			}
		}
		preBuf.clear();

	}


	/*************************************************************************************/
	/*************************************************************************************/
	/*************************************************************************************/

	void handleRoadwayNode(int i, RoadwayNode* otherRwnd, const Roadway* rdw)
	{
		Acad::ErrorStatus es = Acad::eOk;

		RoadwayNode *thisRwnd = nullptr;

		es = getRwnd(i, rdw, thisRwnd);
		if (es == Acad::eWasOpenForWrite || es == Acad::eWasNotifying)
		{
			return;
		}


		MIMDEBUGASSERT(thisRwnd != nullptr);

		//判断有没有巷道长度为0
		const std::map<AcDbHandle, WHICHSIDE>& hdlsd = thisRwnd->getHdlsd();

		for (auto const &v : hdlsd)
		{

			Roadway *_rdw = nullptr;
			OpenObject<Roadway> open3(_rdw, v.first, OpenMode::kForRead);
			es = open3.open();

			if (es != Acad::eOk)continue;

			const AcGePoint3d &sp1 = _rdw->getStartPoint();
			const AcGePoint3d &ep1 = _rdw->getEndPoint();

			if (sp1 == ep1)
			{
				return;
			}
		}

		if (thisRwnd->isWriteEnabled())    //第一次产生节点应该走这条分支
		{
			thisRwnd->upgradeOpen();

			moveRoadwayBind(rdw, i, otherRwnd, thisRwnd, false);
		}
		else if (thisRwnd->isReadEnabled())//后期移动节点应该走这条分支
		{
			moveRoadwayBind(rdw, i, thisRwnd, otherRwnd, true);
		}
		else
		{
			acutPrintf(TEXT("handleRoadwayNode error"));
			//MIMDEBUGASSERT(false);
			return;
		}
	}

	void handleRoadway(int i, Roadway* otherRdw, const Roadway* rdw)
	{

		Acad::ErrorStatus es = Acad::eOk;

		AcDbHandle otherHandle;
		otherRdw->getAcDbHandle(otherHandle);

		//判断如果移动点在别的节点的话就返回
		if (i == 0)
		{
			if (
				otherRdw->getEndPoint() == rdw->getStartPoint()
				|| otherRdw->getStartPoint() == rdw->getStartPoint()
				)
				return;
		}
		else
		{
			if (
				otherRdw->getEndPoint() == rdw->getEndPoint()
				|| otherRdw->getStartPoint() == rdw->getEndPoint()
				)
				return;
		}

		AcDbBlockTable *pBlockTable = NULL;
		AcDbBlockTableRecord *pBlockTableRcd = NULL;
		es = acdbHostApplicationServices()->workingDatabase()->getBlockTable(pBlockTable, AcDb::kForRead);
		es = pBlockTable->getAt(ACDB_MODEL_SPACE, pBlockTableRcd, AcDb::kForWrite);

		RoadwayNode *thisRwnd = nullptr, *otherRwnd1 = nullptr, *otherRwnd2 = nullptr;

		es = getRwnd(i, rdw, thisRwnd);
		es = getRwnd(0, otherRdw, otherRwnd1, OpenMode::kForWrite);
		es = getRwnd(1, otherRdw, otherRwnd2, OpenMode::kForWrite);

		if (
			(Acad::eOk != getRwnd(i, rdw, thisRwnd)) ||
			(Acad::eOk != getRwnd(0, otherRdw, otherRwnd1, OpenMode::kForWrite)) ||
			(Acad::eOk != getRwnd(1, otherRdw, otherRwnd2, OpenMode::kForWrite))
			)
			return;
		

		Roadway* newRdw1 = new Roadway;
		Roadway* newRdw2 = new Roadway;

		
		otherRwnd1->removeRoadway(otherHandle);
		otherRwnd2->removeRoadway(otherHandle);

		const AcGePoint3d& movedPoint = thisRwnd->getPosition();

		newRdw1->setStartPoint(otherRdw->getStartPoint());
		newRdw1->setEndPoint(movedPoint);
		newRdw2->setStartPoint(movedPoint);
		newRdw2->setEndPoint(otherRdw->getEndPoint());

		es = pBlockTableRcd->appendAcDbEntity(newRdw1);
		es = pBlockTableRcd->appendAcDbEntity(newRdw2);

		AcDbHandle newRdwHandle, newRwndHandle;

		newRdw1->getAcDbHandle(newRdwHandle);
		otherRwnd1->getAcDbHandle(newRwndHandle);
		otherRwnd1->appendRoadway(newRdwHandle, STARTSIDE);
		newRdw1->setStartNodeHandle(newRwndHandle);


		thisRwnd->appendRoadway(newRdwHandle, ENDSIDE);
		newRdw2->getAcDbHandle(newRdwHandle);
		thisRwnd->getAcDbHandle(newRwndHandle);
		thisRwnd->appendRoadway(newRdwHandle, STARTSIDE);
		newRdw1->setEndNodeHandle(newRwndHandle);
		newRdw2->setStartNodeHandle(newRwndHandle);

		otherRwnd2->getAcDbHandle(newRwndHandle);
		otherRwnd2->appendRoadway(newRdwHandle, ENDSIDE);
		newRdw2->setEndNodeHandle(newRwndHandle);	

		pBlockTable->close();
		pBlockTableRcd->close();
		newRdw1->close();
		newRdw2->close();

		otherRdw->erase();
	}

}  //end namespace MIM