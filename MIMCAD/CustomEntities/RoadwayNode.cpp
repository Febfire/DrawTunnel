#include "StdAfx.h"
#include "RoadwayNode.h"
#include "Roadway.h"


namespace MIM
{
	//***************************************************************//
	//******************RoadwayNodeReactor****************************//
	//***************************************************************//

	ACRX_NO_CONS_DEFINE_MEMBERS(RoadwayNodeReactor, AcDbObjectReactor);

	void RoadwayNodeReactor::modified(const AcDbObject* dbObj)
	{
		if (dbObj->isEraseStatusToggled() || dbObj->isUndoing() || dbObj->isErased())
			return;

		Acad::ErrorStatus es = Acad::eOk;

		const RoadwayNode *rdwn = static_cast<const RoadwayNode*>(dbObj);
		const std::map<AcDbHandle, WHICHSIDE>& hdlsd = rdwn->getHdlsd();
	

		const AcGePoint3d& newPosition = rdwn->getPosition();
		
		for (auto &v : hdlsd)
		{
			Roadway *rdw = nullptr;
			OpenObject<Roadway> open1(rdw, v.first, AcDb::kForWrite, true);

			es = open1.open();
			if (es != Acad::eOk)
			{
				//MIMDEBUGASSERT(es == Acad::eWasOpenForNotify);
				continue;
			}

			if (rdw == nullptr)
			{
				MIMDEBUGASSERT(false);
				return;
			}

			int side = v.second;
			if (side == ENDSIDE)
			{
				rdw->setEndPoint(newPosition);
			}
			else if (side == STARTSIDE)
			{
				rdw->setStartPoint(newPosition);
			}

		}
	}

	//***************************************************************//
	//******************RoadwayNode**********************************//
	//***************************************************************//


	static AcGePoint3d getPoint(double radius, double u, double v, const AcGeMatrix3d& xform);


	ACRX_DXF_DEFINE_MEMBERS(RoadwayNode, AcDbEntity, AcDb::kDHL_CURRENT,
		AcDb::kMReleaseCurrent, 0, ROADWAYNODE, /*MSG0*/"KLND");

	RoadwayNode::RoadwayNode() :m_position(AcGePoint3d(0, 0, 0)), m_radius(15)
	{
		m_modified = std::make_shared<RoadwayNodeReactor>();
		this->addReactor(m_modified.get());

		m_hdlsd = std::make_shared<HdlsdMap>();

	}

	RoadwayNode::~RoadwayNode()
	{

	}

	Acad::ErrorStatus
		RoadwayNode::dwgOutFields(AcDbDwgFiler *pFiler) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = AcDbEntity::dwgOutFields(pFiler);
		if (es != Acad::eOk)
			return (es);

		pFiler->writePoint3d(m_position);

		pFiler->writeInt32(m_radius);


		HdlsdMap *hdlsd = m_hdlsd.get();

		pFiler->writeUInt32(hdlsd->size());

		for (auto v : *hdlsd)
		{
			pFiler->writeAcDbHandle(v.first);
			pFiler->writeInt32(v.second);
		}

		return pFiler->filerStatus();
	}

	Acad::ErrorStatus
		RoadwayNode::dwgInFields(AcDbDwgFiler *pFiler)
	{
		assertWriteEnabled();
		Acad::ErrorStatus es = AcDbEntity::dwgInFields(pFiler);
		if (es != Acad::eOk)
			return (es);

		pFiler->readPoint3d(&m_position);

		pFiler->readInt32(&m_radius);

		HdlsdMap *phdlsd = m_hdlsd.get();

		Adesk::UInt32 length = 0;
		pFiler->readUInt32(&length);
		AcDbHandle *tmphandle = new AcDbHandle;
		int32_t tmpside = 0;
		for (uint i = 0; i < length; i++)
		{
			pFiler->readAcDbHandle(tmphandle);
			pFiler->readInt32(&tmpside);
			phdlsd->insert(std::pair<AcDbHandle, WHICHSIDE>(*tmphandle, tmpside));
		}

		AcDbHandle hhh = phdlsd->begin()->first;
		int iii = phdlsd->begin()->second;

		/*std::map<AcDbHandle, int> *pp = new std::map<AcDbHandle, int>();
		pFiler->readBytes(pp,sizeof(std::map<AcDbHandle, int>));
		m_hdlsd.swap(*pp);
		delete pp;*/

		return pFiler->filerStatus();
	}

	Acad::ErrorStatus
		RoadwayNode::subErase(Adesk::Boolean erasing)
	{
		assertWriteEnabled();

		if (m_hdlsd.get()->size() > 0)
		{
			return Acad::ErrorStatus::eCannotBeErasedByCaller;
		}

		Acad::ErrorStatus es = AcDbEntity::subErase(erasing);
		if (es != Acad::eOk)
			return (es);

		return Acad::eOk;
	}

	Acad::ErrorStatus 
		RoadwayNode::subClose()
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

	Adesk::Boolean 
		RoadwayNode::subWorldDraw(AcGiWorldDraw *mode)
	{
		assertReadEnabled();

		mode->geometry().polypoint(1, &m_position);

		if (mode->regenType() == AcGiRegenType::kAcGiStandardDisplay)
		{
			mode->geometry().circularArc(m_position,(double)m_radius,AcGeVector3d::kZAxis,AcGeVector3d::kXAxis,2*PI);
			return (AcDbEntity::subWorldDraw(mode));
		}

		int uStepsNum = 50, vStepNum = 50;
		double ustep = 1 / (double)uStepsNum, vstep = 1 / (double)vStepNum;
		double u = 0, v = 0;
		AcGePoint3d ptArray[3];

		AcGeMatrix3d matrix;
		AcGeVector3d vec = m_position - AcGePoint3d(0, 0, 0);

		matrix.setToTranslation(vec);


		//绘制下端三角形组
		for (int i = 0; i < uStepsNum; i++)
		{
			AcGePoint3d a = getPoint(m_radius, 0, 0, matrix);
			AcGePoint3d b = getPoint(m_radius, u, vstep, matrix);
			AcGePoint3d c = getPoint(m_radius, u + ustep, vstep, matrix);

			AcGePoint3d ptArray[3] = { a,b,c };
			Adesk::Int32 faceList[] = { 3, 0, 1, 2 };
			mode->geometry().shell(3, ptArray, 4, faceList);
			//mode->geometry().polyline(3, ptArray);

			u += ustep;
		}

		//绘制中间四边形组
		u = 0, v = vstep;
		for (int i = 1; i < vStepNum - 1; i++)
		{
			for (int j = 0; j < uStepsNum; j++)
			{
				AcGePoint3d a = getPoint(m_radius, u, v, matrix);
				AcGePoint3d b = getPoint(m_radius, u + ustep, v, matrix);
				AcGePoint3d c = getPoint(m_radius, u + ustep, v + vstep, matrix);
				AcGePoint3d d = getPoint(m_radius, u, v + vstep, matrix);

				AcGePoint3d ptArray[4] = { a,b,c,d };
				Adesk::Int32 faceList[] = { 4, 0, 1, 2, 3 };
				mode->geometry().shell(4, ptArray, 5, faceList);
				//mode->geometry().polyline(4, ptArray);
				u += ustep;
			}
			v += vstep;
		}

		//绘制下端三角形组
		u = 0;
		for (int i = 0; i < uStepsNum; i++)
		{
			AcGePoint3d a = getPoint(m_radius, 0, 1, matrix);
			AcGePoint3d b = getPoint(m_radius, u, 1 - vstep, matrix);
			AcGePoint3d c = getPoint(m_radius, u + ustep, 1 - vstep, matrix);

			AcGePoint3d ptArray[3] = { a,b,c };
			Adesk::Int32 faceList[] = { 3, 0, 1, 2 };
			mode->geometry().shell(3, ptArray, 4, faceList);
			//mode->geometry().polyline(3, ptArray);

			u += ustep;
		}

		return (AcDbEntity::subWorldDraw(mode));
	}


	Acad::ErrorStatus RoadwayNode::subGetGripPoints(AcGePoint3dArray &gripPoints,
		AcDbIntArray &osnapModes,
		AcDbIntArray &geomIds) const
	{
		assertReadEnabled();

		Acad::ErrorStatus es = Acad::eOk;
		gripPoints.append(m_position);

		return es;
	}

	Acad::ErrorStatus RoadwayNode::subMoveGripPointsAt(const AcDbIntArray &indices, const AcGeVector3d &offset)
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

				//acutIsPrint(offset.x);
				if (idx == 0) m_position += offset;
				
					
			}
			return Acad::eOk;
		}
	}

	Acad::ErrorStatus RoadwayNode::subGetOsnapPoints(
		AcDb::OsnapMode     osnapMode,
		Adesk::GsMarker     gsSelectionMark,
		const AcGePoint3d&  pickPoint,
		const AcGePoint3d&  lastPoint,
		const AcGeMatrix3d& viewXform,
		AcGePoint3dArray&   snapPoints,
		AcDbIntArray &   geomIds) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = Acad::eOk;


		switch (osnapMode)
		{
		case AcDb::kOsModeEnd:
			break;
		case AcDb::kOsModeMid:
			break;
		case AcDb::kOsModeCen:
			snapPoints.append(m_position);
			break;
		case AcDb::kOsModeNode:
			snapPoints.append(m_position);
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
			break;
		default:
			break;
		}
		return es;
	}

	Acad::ErrorStatus
		RoadwayNode::subIntersectWith(
			const AcDbEntity* ent,
			AcDb::Intersect intType,
			const AcGePlane& projPlane,
			AcGePoint3dArray& points,
			Adesk::GsMarker /*thisGsMarker*/,
			Adesk::GsMarker /*otherGsMarker*/) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = Acad::eOk;

		return es;
	}

	Acad::ErrorStatus   RoadwayNode::subIntersectWith(
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

		if (ent->isKindOf(RoadwayNode::desc())) {

			points.append(this->m_position);

		}
		else if (ent->isKindOf(Roadway::desc()))
		{
			//acutPrintf(TEXT("与roadway相交"));
		}

		return es;
	}



	Acad::ErrorStatus  RoadwayNode::subTransformBy(const AcGeMatrix3d& xform)
	{
		assertWriteEnabled();
		m_position.transformBy(xform);
		return Acad::eOk;
	}

	Acad::ErrorStatus RoadwayNode::subGetGeomExtents(AcDbExtents& extents) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = Acad::eOk;
		
		AcGePoint3d minPoint, maxPoint;
		minPoint = m_position;
		maxPoint = m_position;
		extents.set(minPoint, maxPoint);
		return es;
	}

	void RoadwayNode::appendRoadway(AcDbHandle handle, WHICHSIDE side)
	{
		mutex.lock();
		if (this->isWriteEnabled())
		{
			assertWriteEnabled();
			if (handle.isNull())
			{
				MIMDEBUGASSERT(false);
				return;
			}
			m_hdlsd.get()->emplace(std::pair<AcDbHandle, WHICHSIDE>(handle, side));
		}
		else
		{	
			m_hdlsd.get()->emplace(std::pair<AcDbHandle, WHICHSIDE>(handle, side));
			BufPair buf;
			buf.type = ERoadwayNodeBuf::ePreHdlsd;
			buf.buf = std::pair<AcDbHandle, WHICHSIDE>(handle, side);
			preBuf.push_back(std::move(buf));
		}
		mutex.unlock();
	}


	void RoadwayNode::removeRoadway(const AcDbHandle& handle) 
	{
		mutex.lock();
		if (this->isWriteEnabled())
		{
			assertWriteEnabled();
			if (handle.isNull())
			{
				MIMDEBUGASSERT(false);
				return;
			}
			if (m_hdlsd.get()->size() > 0)
				m_hdlsd.get()->erase(handle);
		}
		else
		{
			if (m_hdlsd.get()->size() > 0)
				m_hdlsd.get()->erase(handle);
			BufPair buf;
			buf.type = ERoadwayNodeBuf::eInvalidHandle;
			buf.buf = handle;
			preBuf.push_back(std::move(buf));	
		}
		mutex.unlock();
	}

	bool RoadwayNode::tryErase()
	{
		assertWriteEnabled();

		if (preBuf.size() > 0)
		{
			setPreBuf();
		}

		if (m_hdlsd.get()->size() == 0)
			this->erase();

		return this->isErased();
	}


	Acad::ErrorStatus RoadwayNode::setPosition(const AcGePoint3d &position)
	{
		assertWriteEnabled();
		m_position = position;
		return Acad::eOk;
	}

	Acad::ErrorStatus RoadwayNode::setRadius(uint radius)
	{
		assertWriteEnabled();
		m_radius = radius;
		return Acad::eOk;
	}

	inline const AcGePoint3d& RoadwayNode::getPosition() const
	{
		assertReadEnabled();
		return m_position;
	}

	inline size_t RoadwayNode::getCountsOfBinded() const
	{
		assertReadEnabled();
		return m_hdlsd.get()->size();
	}


	const std::map<AcDbHandle, WHICHSIDE>& RoadwayNode::getHdlsd() const
	{
		assertReadEnabled();
		return *m_hdlsd.get();
	}

	void RoadwayNode::clearRoadway() const
	{
		this->m_hdlsd.get()->clear();
	}

	void RoadwayNode::copyRoadway(const RoadwayNode &other)
	{
		const std::map<AcDbHandle, WHICHSIDE>& hdlsd = other.getHdlsd();
		for (auto v : hdlsd)
		{
			this->appendRoadway(v.first, v.second);
		}
	}

	//*****************PRIVATE******************//

	void RoadwayNode::setPreBuf()
	{
		for (auto &v : preBuf)
		{
			switch (v.type)
			{
			case eInvalidHandle:
			{
				removeRoadway(v.buf.AnyCast<AcDbHandle>());
				break;
			}
			case ePreHdlsd:
			{
				auto r = v.buf.AnyCast<std::pair<AcDbHandle, WHICHSIDE>>();
				appendRoadway(r.first,r.second);
			}
			default:
				break;
			}
		}
		preBuf.clear();
	}


	//*****************STATIC******************//
	AcGePoint3d getPoint(double radius, double u, double v, const AcGeMatrix3d& xform)
	{
		double x = radius*sin(PI*v)*cos(2 * PI*u);
		double y = radius*sin(PI*v)*sin(2 * PI*u);
		double z = radius*cos(PI*v);

		AcGePoint3d point(x, y, z);
		point.transformBy(xform);

		return point;
	}

}