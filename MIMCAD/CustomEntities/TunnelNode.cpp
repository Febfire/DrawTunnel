#include "StdAfx.h"
#include "TunnelNode.h"
#include "Tunnel_Base.h"
#include "ObjectOpener.h"

namespace MIM
{
	//***************************************************************//
	//******************TunnelNodeReactor****************************//
	//***************************************************************//

	ACRX_NO_CONS_DEFINE_MEMBERS(TunnelNodeReactor, AcDbEntityReactor);

	void TunnelNodeReactor::modified(const AcDbObject* dbObj)
	{
		if (dbObj->isEraseStatusToggled() || dbObj->isUndoing() || dbObj->isErased())
			return;

		Acad::ErrorStatus es = Acad::eOk;

		const TunnelNode *node = static_cast<const TunnelNode*>(dbObj);
		AcDbHandle myHandle;
		node->getAcDbHandle(myHandle);

		const std::map<AcDbHandle, INDEX>& hdlsd = node->getHdlsd();
		
		const AcGePoint3d& newPosition = node->getPosition();
		const UInt32 newColor = node->getNodeColor();

		for (auto &v : hdlsd)
		{
			int index = v.second;

			Tunnel_Base *tunnel = nullptr;
			ObjectOpener<Tunnel_Base> open1(tunnel, v.first, AcDb::kForWrite, true);

			es = open1.open();
			if (es != Acad::eOk)
			{
				//MIMDEBUGASSERT(es == Acad::eWasOpenForNotify);
				continue;
			}

			//const std::vector<UInt32> colors = tunnel->getColors();
			//if (colors.size()>0 && !(colors.at(index) == newColor))
			//	tunnel->changeNodeColor(newColor, index);

			if (Tunnel_Base::getDisplayMode() != DisplayMode::Line)
			{
				const std::vector<AcGePoint3d> bsPts = tunnel->getBasePoints();
				if (!(bsPts.at(index) == newPosition))
					tunnel->changeVertice(newPosition, index);
			}
	
		}
	}

	//***************************************************************//
	//******************TunnelNode**********************************//
	//***************************************************************//


	static AcGePoint3d getPoint(double radius, double u, double v, const AcGeMatrix3d& xform);


	ACRX_DXF_DEFINE_MEMBERS(TunnelNode, AcDbEntity, AcDb::kDHL_CURRENT,
		AcDb::kMReleaseCurrent, 0, TUNNELNODE, /*MSG0*/"KLND");

	TunnelNode::TunnelNode() :
		m_position(AcGePoint3d(0, 0, 0)),
		m_radius(15), 
		m_color(0x0000b3ff), 
		m_name(nullptr),
		m_location(nullptr),
		m_nodeReactor(std::make_shared<TunnelNodeReactor>()),
		m_hdlsd(std::make_shared<HdlSd>())
	{
		this->addReactor(m_nodeReactor.get());
	}

	TunnelNode::~TunnelNode()
	{

	}

	Acad::ErrorStatus
		TunnelNode::dwgOutFields(AcDbDwgFiler *pFiler) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = AcDbEntity::dwgOutFields(pFiler);
		if (es != Acad::eOk)
			return (es);

		pFiler->writePoint3d(m_position);

		pFiler->writeInt32(m_radius);

		pFiler->writeUInt32(m_color);

		if (m_name)
			pFiler->writeString(m_name);
		else
			pFiler->writeString(TEXT(""));

		if (m_location)
			pFiler->writeString(m_location);
		else
			pFiler->writeString(TEXT(""));
		

		HdlSd *hdlsd = m_hdlsd.get();

		pFiler->writeUInt32(hdlsd->size());

		for (auto v : *hdlsd)
		{
			pFiler->writeAcDbHandle(v.first);
			pFiler->writeInt32(v.second);
		}

		return pFiler->filerStatus();
	}

	Acad::ErrorStatus
		TunnelNode::dwgInFields(AcDbDwgFiler *pFiler)
	{
		assertWriteEnabled();
		Acad::ErrorStatus es = AcDbEntity::dwgInFields(pFiler);
		if (es != Acad::eOk)
			return (es);

		pFiler->readPoint3d(&m_position);

		pFiler->readInt32(&m_radius);

		pFiler->readUInt32(&m_color);
		
		acutDelString(m_name);
		pFiler->readString(&m_name);

		acutDelString(m_location);
		pFiler->readString(&m_location);

		HdlSd *HdlSdPtr = m_hdlsd.get();

		Adesk::UInt32 length = 0;
		pFiler->readUInt32(&length);
		AcDbHandle *tmphandle = new AcDbHandle;
		int32_t tmpside = 0;
		for (uint i = 0; i < length; i++)
		{
			pFiler->readAcDbHandle(tmphandle);
			pFiler->readInt32(&tmpside);
			HdlSdPtr->insert(std::pair<AcDbHandle, INDEX>(*tmphandle, tmpside));
			
/*			if (!registratST.count(*tmphandle))
			{
				SampleTransactionPtr stptr = std::make_shared<SampleTransaction>();
				registratST.emplace(*tmphandle, stptr);
				m_hdlst.get()->emplace(*tmphandle, stptr);
			}	*/	
		}

		AcDbHandle hhh = HdlSdPtr->begin()->first;
		int iii = HdlSdPtr->begin()->second;

		/*std::map<AcDbHandle, int> *pp = new std::map<AcDbHandle, int>();
		pFiler->readBytes(pp,sizeof(std::map<AcDbHandle, int>));
		m_hdlsd.swap(*pp);
		delete pp;*/

		return pFiler->filerStatus();
	}

	Acad::ErrorStatus
		TunnelNode::subErase(Adesk::Boolean erasing)
	{
		if (m_hdlsd.get()->size() > 0)
		{
			return Acad::ErrorStatus::eCannotBeErasedByCaller;
		}

		assertWriteEnabled();
		Acad::ErrorStatus es = AcDbEntity::subErase(erasing);
		if (es != Acad::eOk)
			return (es);

		return Acad::eOk;
	}

	Acad::ErrorStatus
		TunnelNode::subClose()
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
			return Acad::eOk;
		}
	}

	Adesk::Boolean
		TunnelNode::subWorldDraw(AcGiWorldDraw *mode)
	{
		assertReadEnabled();

		mode->geometry().polypoint(1, &m_position);

		if (Tunnel_Base::getDisplayMode() == DisplayMode::Line)
		{

		}
		else if (Tunnel_Base::getDisplayMode() == DisplayMode::DoubleLine)
		{
			draw3d(mode);
		}
		else if (Tunnel_Base::getDisplayMode() == DisplayMode::Real)
		{
			draw3d(mode);
		}
		return (AcDbEntity::subWorldDraw(mode));
	}


	Acad::ErrorStatus TunnelNode::subGetGripPoints(AcGePoint3dArray &gripPoints,
		AcDbIntArray &osnapModes,
		AcDbIntArray &geomIds) const
	{
		assertReadEnabled();

		Acad::ErrorStatus es = Acad::eOk;
		gripPoints.append(m_position);

		return es;
	}

	Acad::ErrorStatus TunnelNode::subMoveGripPointsAt(const AcDbIntArray &indices, const AcGeVector3d &offset)
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

	Acad::ErrorStatus TunnelNode::subGetOsnapPoints(
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
		TunnelNode::subIntersectWith(
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



	static Acad::ErrorStatus intCircle(const Tunnel_Base*         poly,
		const AcGeCircArc3d     circle,
		AcDb::Intersect   intType,
		const AcGePlane*        projPlane,
		AcGePoint3dArray& points)
	{
		Acad::ErrorStatus es = Acad::eOk;

		AcGePoint3dArray vertexArray;

		if ((es = poly->getVertices2(vertexArray)) != Acad::eOk) {
			return es;
		}

		AcGeLineSeg3d lnsg;
		AcGePoint3d   pt1, pt2;
		AcGePoint3d   dummy1, dummy2;
		int           howMany;

		for (int i = 0; i < vertexArray.length(); i += 2) {

			lnsg.set(vertexArray[i], vertexArray[i + 1]);

			if (projPlane == NULL) {
				if (!circle.intersectWith(lnsg, howMany, pt1, pt2)) {
					continue;
				}
				else {
					if (howMany > 1) {
						points.append(pt1);
						points.append(pt2);
					}
					else {
						points.append(pt1);
					}
				}
			}
			else {
				if (!circle.projIntersectWith(lnsg, projPlane->normal(),
					howMany, pt1, pt2, dummy1, dummy2))
				{
					continue;
				}
				else {
					if (howMany > 1) {
						points.append(pt1);
						points.append(pt2);
					}
					else {
						points.append(pt1);
					}
				}
			}
		}

		return es;
	}

	static Acad::ErrorStatus intCircle(const Tunnel_Base*         poly,
		const AcDbCircle*       circle,
		AcDb::Intersect   intType,
		const AcGePlane*        projPlane,
		AcGePoint3dArray& points)
	{
		Acad::ErrorStatus es = Acad::eOk;

		AcGeCircArc3d  acircle(circle->center(), circle->normal(),
			circle->radius());
		es = intCircle(poly, acircle, intType, projPlane, points);

		return es;
	}


	Acad::ErrorStatus   TunnelNode::subIntersectWith(
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

		if (ent->isKindOf(TunnelNode::desc())) {

			points.append(this->m_position);

		}
		else if (ent->isKindOf(Tunnel_Base::desc()))
		{
			Tunnel_Base* tunnel = Tunnel_Base::cast(ent);
			std::vector<AcGeVector3d> vvs;
			tunnel->getVerticalVector(vvs);
			AcDbCircle *circle = new AcDbCircle(getPosition(), vvs.at(0),getRadius());
			es = intCircle(tunnel, circle,intType, NULL, points);
			delete circle;
		}

		return es;
	}



	Acad::ErrorStatus  TunnelNode::subTransformBy(const AcGeMatrix3d& xform)
	{
		assertWriteEnabled();
		m_position.transformBy(xform);
		return Acad::eOk;
	}

	Acad::ErrorStatus TunnelNode::subGetGeomExtents(AcDbExtents& extents) const
	{
		assertReadEnabled();
		Acad::ErrorStatus es = Acad::eOk;

		AcGePoint3d minPoint, maxPoint;
		minPoint = m_position;
		maxPoint = m_position;
		extents.set(minPoint, maxPoint);
		return es;
	}

	void TunnelNode::appendTunnel(AcDbHandle handle, INDEX index)
	{
		if (this->isWriteEnabled())
		{
			assertWriteEnabled();
			if (handle.isNull())
			{
				MIMDEBUGASSERT(false);
				return;
			}
			m_hdlsd.get()->emplace(handle, index);

		}
	}


	void TunnelNode::removeTunnel(const AcDbHandle& handle)
	{
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
	}

	bool TunnelNode::tryErase()
	{
		assertWriteEnabled();

		if (m_hdlsd.get()->size() == 0)
			this->erase();

		return this->isErased();
	}


	Acad::ErrorStatus TunnelNode::setPosition(const AcGePoint3d &position)
	{
		assertWriteEnabled();
		
		m_position = position;
		return Acad::eOk;
	}

	Acad::ErrorStatus TunnelNode::setRadius(uint radius)
	{
		assertWriteEnabled();
		m_radius = radius;
		return Acad::eOk;
	}

	Acad::ErrorStatus TunnelNode::setNodeColor(UINT32 color)
	{
		assertWriteEnabled();
		m_color = color;
		return Acad::eOk;
	}

	Acad::ErrorStatus TunnelNode::setName(const TCHAR* name)
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

	Acad::ErrorStatus TunnelNode::setLocation(const TCHAR* location)
	{
		assertWriteEnabled();
		acutDelString(m_location);
		m_location = NULL;
		if (location != NULL)
		{
			acutUpdString(location, m_location);
		}
		return Acad::eOk;
	}

	Acad::ErrorStatus TunnelNode::changeIndex(AcDbHandle handle, INDEX index)
	{
		assertWriteEnabled();
		auto p = m_hdlsd.get()->find(handle);
		p->second = index;
		return Acad::eOk;
	}

	const AcGePoint3d& TunnelNode::getPosition() const
	{
		assertReadEnabled();
		return m_position;
	}

	double TunnelNode::getRadius() const
	{
		return m_radius;
	}

	size_t TunnelNode::getCountsOfBinded() const
	{
		assertReadEnabled();
		return m_hdlsd.get()->size();
	}

	UINT32 TunnelNode::getNodeColor() const
	{
		return m_color;
	}

	const TCHAR* TunnelNode::getName() const
	{
		assertReadEnabled();
		return m_name;
	}

	const TCHAR* TunnelNode::getLocation() const
	{
		assertReadEnabled();
		return m_location;
	}

	const std::map<AcDbHandle, INDEX>& TunnelNode::getHdlsd() const
	{
		assertReadEnabled();
		return *m_hdlsd.get();
	}

	void TunnelNode::clearTunnel() const
	{
		this->m_hdlsd.get()->clear();
	}

	void TunnelNode::copyTunnel(const TunnelNode &other)
	{
		const std::map<AcDbHandle, INDEX>& hdlsd = other.getHdlsd();
		for (auto v : hdlsd)
		{
			this->appendTunnel(v.first, v.second);
		}
	}

	//*****************PRIVATE******************//

	void TunnelNode::draw2d(AcGiWorldDraw *mode) const
	{

		if (mode->regenType() == AcGiRegenType::kAcGiStandardDisplay)
		{
			mode->geometry().circularArc(m_position, (double)m_radius, AcGeVector3d::kZAxis, AcGeVector3d::kXAxis, 2 * PI);
			return;
		}

		AcGeMatrix3d matrix;
		AcGeVector3d vec = m_position - AcGePoint3d(0, 0, 0);

		matrix.setToTranslation(vec);

		AcGiVertexData VertexData;


		int step = 50;
		AcGePoint3d ptArray[50];
		Adesk::Int32 faceList[51];
		faceList[0] = 50;
		for (int i = 0; i < step; i++)
		{
			ptArray[i] = AcGePoint3d(m_radius*cos(2 * PI*i / step), m_radius*sin(2 * PI*i / step), 0);
			ptArray[i].transformBy(matrix);

			faceList[i + 1] = i;
		}	
		uint8_t r, g, b;
		UInt32ToRGB(m_color, r, g, b);

		if (mode->regenType() == AcGiRegenType::kAcGiRenderCommand)
		{
			AcCmEntityColor colorList3[50];
			for (int i = 0; i < 50; i++)
			{
				colorList3[i].setRGB(r, g, b);
			}
			VertexData.setTrueColors(colorList3);
		}	

		mode->geometry().shell(50, ptArray, 51, faceList, nullptr, nullptr, &VertexData);
	
	}


	void TunnelNode::draw3d(AcGiWorldDraw *mode) const
	{

		AcGiVertexData VertexData3, VertexData4;
		uint8_t r, g, b;
		UInt32ToRGB(m_color, r, g, b);

		if (mode->regenType() == AcGiRegenType::kAcGiRenderCommand)
		{
			AcCmEntityColor colorList3[3], colorList4[4];
			for (int i = 0; i < 4; i++)
			{
				if (i < 3)
				{
					colorList3[i].setRGB(r,g,b);
				}
				colorList4[i].setRGB(r, g, b);
			}

			VertexData3.setTrueColors(colorList3);
			VertexData4.setTrueColors(colorList4);
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


			mode->geometry().shell(3, ptArray, 4, faceList, nullptr, nullptr, &VertexData3);

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
				mode->geometry().shell(4, ptArray, 5, faceList, nullptr, nullptr, &VertexData4);
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
			mode->geometry().shell(3, ptArray, 4, faceList, nullptr, nullptr, &VertexData3);

			u += ustep;
		}
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