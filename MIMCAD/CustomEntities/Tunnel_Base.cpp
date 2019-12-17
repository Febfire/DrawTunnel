#include "StdAfx.h"
#include "Tunnel_Base.h"
#include "TunnelReactor.h"
#include "TunnelNode.h"
#include "TunnelTag.h"
#include "ObjectOpener.h"
#include <iterator>

using namespace MIM;

static bool isNodifying = false;
static bool isAnimationMode = false;
static DisplayMode displayMode = DisplayMode::Real;

bool Tunnel_Base::getIsNodifying()
{
	return isNodifying;
}

void Tunnel_Base::startNodifying()
{
	isNodifying = true;
}

void Tunnel_Base::endNodifying()
{
	isNodifying = false;
}

bool Tunnel_Base::getAnimateMode()
{
	return isAnimationMode;
}

void Tunnel_Base::startAnimateMode()
{
	isAnimationMode = true;
}

void Tunnel_Base::endAnimateMode()
{
	isAnimationMode = false;
}

UINT16 Tunnel_Base::getDisplayMode()
{
	switch (displayMode)
	{
	case DisplayMode::Line:
		return 1;
	case DisplayMode::DoubleLine:
		return 2;
	case DisplayMode::Real:
		return 3;
	default:
		return 3;
	}
}
void Tunnel_Base::setDisplayMode(UINT16 mode)
{
	switch (mode)
	{
	case 1:
		displayMode = DisplayMode::Line;
		break;
	case 2:
		displayMode = DisplayMode::DoubleLine;
		break;
	case 3:
		displayMode = DisplayMode::Real;
		break;
	default:
		displayMode = DisplayMode::Real;
		break;
	}
}

static Acad::ErrorStatus intLine(const Tunnel_Base*         pTunnel,
	const AcGeLine3d        line,
	AcGePoint3dArray& points);

static Acad::ErrorStatus intLine(const Tunnel_Base*         pTunnel,
	const AcDbLine*         line,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points);

static Acad::ErrorStatus intLine(const Tunnel_Base*         pTunnel,
	const AcGeLineSeg3d     line,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points);

static Acad::ErrorStatus intArc(const Tunnel_Base*         pTunnel,
	const AcDbArc*          arc,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points);

static Acad::ErrorStatus intArc(const Tunnel_Base*         pTunnel,
	const AcGeCircArc3d     arc,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points);

static Acad::ErrorStatus intCircle(const Tunnel_Base*         pTunnel,
	const AcDbCircle*       circle,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points);

static Acad::ErrorStatus intCircle(const Tunnel_Base*         pTunnel,
	const AcGeCircArc3d     circle,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points);

static Acad::ErrorStatus intPline(const Tunnel_Base*         pTunnel,
	AcDb2dPolyline*   pline,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points);

static Acad::ErrorStatus intPline(const Tunnel_Base*         pTunnel,
	AcDb3dPolyline*   pline,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points);


//*************************************************************************
// Code for the Class Body. 
//*************************************************************************

ACRX_NO_CONS_DEFINE_MEMBERS(Tunnel_Base, AcDbCurve);


Tunnel_Base::Tunnel_Base() :
	m_type(nullptr),
	m_geo_segments(0),
	m_color_segments(1),
	m_colors(std::vector<uint>(m_color_segments+1, 0x0000b3ff)),
	m_temperatures(std::vector<int16_t>(m_color_segments + 1, 30)),
	m_closed(false),
	m_name(nullptr),
	m_tagData(nullptr),
	m_location(nullptr),
	m_reactor(TunnelReactorPtr(new TunnelReactor()))
	
{
	this->addReactor(m_reactor.get());
}


Tunnel_Base::~Tunnel_Base()
{
	if (m_type != nullptr)
		acutDelString(m_type);

	if(m_name!=nullptr)
	acutDelString(m_name);

	if (m_tagData != nullptr)
	acutDelString(m_tagData);

	if (m_location != nullptr)
		acutDelString(m_location);
}

//----------------------------------------------------------------------

Acad::ErrorStatus
Tunnel_Base::dwgOutFields(AcDbDwgFiler *pFiler) const
{
	assertReadEnabled();
	Acad::ErrorStatus es = AcDbCurve::dwgOutFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	if (m_geo_basePoints.size() == 0)return Acad::eOk;

	Adesk::Int16 version = TUNNEL_VERSION;
	pFiler->writeItem(version);

	if (m_type)
		pFiler->writeString(m_type);
	else
		pFiler->writeString(TEXT(""));

	pFiler->writeUInt16(m_geo_segments);

	pFiler->writeUInt16(m_color_segments);

	pFiler->writeBool(m_closed);

	for (auto& point : m_geo_basePoints)
	{
		pFiler->writePoint3d(point);
	}

	for (auto& handle : m_nodesHandle)
	{
		pFiler->writeAcDbHandle(handle);
	}

	for (auto& color : m_colors)
	{
		pFiler->writeUInt32(color);
	}

	for (auto& temperature : m_temperatures)
	{
		pFiler->writeInt16(temperature);
	}

	if (m_name)
		pFiler->writeString(m_name);
	else
		pFiler->writeString(TEXT(""));

	if (m_tagData)
		pFiler->writeString(m_tagData);
	else
		pFiler->writeString(TEXT(""));

	if (m_location)
		pFiler->writeString(m_location);
	else
		pFiler->writeString(TEXT(""));


	pFiler->writeAcDbHandle(m_tagHandle);

	return pFiler->filerStatus();
}

//----------------------------------------------------------------------------------------------------------------//

Acad::ErrorStatus
Tunnel_Base::dwgInFields(AcDbDwgFiler *pFiler)
{
	assertWriteEnabled();
	Acad::ErrorStatus es = AcDbCurve::dwgInFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version;

	pFiler->readItem(&version);
	if (version > TUNNEL_VERSION)
		return Acad::eMakeMeProxy;

	acutDelString(m_type);
	pFiler->readString(&m_type);

	pFiler->readUInt16(&m_geo_segments);

	pFiler->readUInt16(&m_color_segments);

	pFiler->readBool(&m_closed);

	//m_basePoints.clear();
	std::vector<AcGePoint3d> basePoints;
	for (size_t i = 0; i < m_geo_segments + 1; i++)
	{
		AcGePoint3d point;
		pFiler->readPoint3d(&point);
		basePoints.emplace_back(point);
	}
	setBasePoints(basePoints);

	//m_nodesHandle.clear();
	std::vector<AcDbHandle> handles;
	for (size_t i = 0; i < m_geo_segments + 1; i++)
	{
		AcDbHandle handle;
		pFiler->readAcDbHandle(&handle);
		handles.emplace_back(handle);
	}
	setNodesHandle(handles);

	//m_nodesColor.clear();
	std::vector<UInt32> colors;
	for (uint i = 0; i < m_color_segments + 1; i++)
	{
		UINT32 color = 0;
		pFiler->readUInt32(&color);
		colors.push_back(color);

	}
	setColors(colors);

	std::vector<Int16> temperatures;
	for (uint i = 0; i < m_color_segments + 1; i++)
	{
		int16_t temperature = 0;
		pFiler->readInt16(&temperature);
		temperatures.push_back(temperature);
	}
	setTemperatures(temperatures);

	acutDelString(m_name);
	pFiler->readString(&m_name);

	acutDelString(m_tagData);
	pFiler->readString(&m_tagData);

	acutDelString(m_location);
	pFiler->readString(&m_location);

	pFiler->readAcDbHandle(&m_tagHandle);

	return pFiler->filerStatus();
}


Acad::ErrorStatus Tunnel_Base::subGetOsnapPoints(
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
		//snapPoints.append(m_startPoint);
		//snapPoints.append(m_endPoint);
		break;
	}
	case AcDb::kOsModeMid:
	{
		//snapPoints.append(AcGePoint3d((m_startPoint.x + m_endPoint.x) / 2, (m_startPoint.y + m_endPoint.y) / 2, (m_startPoint.z + m_endPoint.z) / 2));
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
		/*AcGeLineSeg3d tmpSeg(m_startPoint, m_endPoint);
		AcGePoint3dArray pointArray;

		tmpSeg.getSamplePoints(100, pointArray);
		for (auto &point : pointArray)
		{
			snapPoints.append(point);
		}*/
		break;
	}
	default:
		break;
	}
	return es;
}

Adesk::Boolean Tunnel_Base::subWorldDraw(AcGiWorldDraw *mode)
{
	if (getBasePoints().size() < 2)return false;

	if (mode->regenAbort()) {
		//clear the drag flags once we are drawn
		return Adesk::kTrue;
	}
	assertReadEnabled();

	UINT32 segmentCount = getGeoSegments();

	if (displayMode == DisplayMode::Line)  //单线模式
	{
		setResultBuffer();

		std::unique_ptr<AcGePoint3d[]> ptArray = std::make_unique<AcGePoint3d[]>(segmentCount +1);
		for (int i = 0; i < m_geo_basePoints.size(); i++)
		{
			ptArray[i] = m_geo_basePoints.at(i);
		}

		mode->geometry().polyline(segmentCount +1, ptArray.get());
	}
	else if (displayMode == DisplayMode::DoubleLine)  //双线模式
	{		
		setResultBuffer();

		for (size_t i = 0; i < segmentCount; i++)
		{
			mode->geometry().polyline(2, &m_allPoints2.at(i * 5));
			mode->geometry().polyline(2, &m_allPoints2.at(i * 5 + 2));
		}

		drawArrow2(mode);

	}
	else if (displayMode == DisplayMode::Real)   //真实模式
	{
		//在动画模式下，不进行计算只从缓存读数据
		if (isAnimationMode == true && m_allPoints3.size()>0)
		{
			AcGiVertexData VertexData;
			if (mode->regenType() != AcGiRegenType::kAcGiStandardDisplay)
			{
				VertexData.setTrueColors(m_allColors.data());
			}

			mode->geometry().shell(
				m_allPoints3.size(),
				m_allPoints3.data(),
				m_allFaces.size(),
				m_allFaces.data(),
				NULL,
				NULL,
				&VertexData
			);

			drawArrow3(mode);
		}
		else
		{
			setResultBuffer();


			AcGiVertexData VertexData;
			if (mode->regenType() != AcGiRegenType::kAcGiStandardDisplay)
			{
				VertexData.setTrueColors(m_allColors.data());
			}
			mode->geometry().shell(
				m_allPoints3.size(),
				m_allPoints3.data(),
				m_allFaces.size(),
				m_allFaces.data(),
				NULL,
				NULL,
				&VertexData
			);

			drawArrow3(mode);
		}
	}
	return (AcDbCurve::subWorldDraw(mode));
}


//----------------------------------------------------------------------------------------------------------------//

Acad::ErrorStatus Tunnel_Base::subGetGripPoints(
	AcGePoint3dArray &gripPoints, AcDbIntArray &osnapModes, AcDbIntArray &geomIds
) const {
	assertReadEnabled();

	Acad::ErrorStatus es = Acad::eOk;

	if (Tunnel_Base::getDisplayMode() != DisplayMode::Line)
		return es;

	for (auto& point : m_geo_basePoints)
	{
		gripPoints.append(point);
	}

	return es;
}


//----------------------------------------------------------------------------------------------------------------//

Acad::ErrorStatus Tunnel_Base::subMoveGripPointsAt(const AcDbIntArray &indices, const AcGeVector3d &offset)
{
	Acad::ErrorStatus es = Acad::eOk;
	if (indices.length() == 0 || offset.isZeroLength() || Tunnel_Base::getDisplayMode() != DisplayMode::Line)
		return es; //that's easy :-)
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

			m_geo_basePoints.at(idx)+= offset;
		}
		return Acad::eOk;
	}
}

//----------------------------------------------------------------------------------------------------------------//

Acad::ErrorStatus  Tunnel_Base::subTransformBy(const AcGeMatrix3d& xform)
{
	assertWriteEnabled();
	for (auto& point : m_geo_basePoints)
	{
		point.transformBy(xform);
	}
	return Acad::eOk;
}

//----------------------------------------------------------------------------------------------------------------//

void  Tunnel_Base::subList() const
{
	assertReadEnabled();

	AcDbCurve::subList();

	acutPrintf(TEXT("%20s %s \n"), TEXT("名称: "), getName());

	acutPrintf(TEXT("%20s %-9.16q0 \n"), TEXT("长度: "), getLengthSum());

	acutPrintf(TEXT("%20s %d \n"), TEXT("段数: "), getColorSegments());

}


Acad::ErrorStatus
Tunnel_Base::subIntersectWith(
	const AcDbEntity* ent,
	AcDb::Intersect intType,
	AcGePoint3dArray& points,
	Adesk::GsMarker /*thisGsMarker*/,
	Adesk::GsMarker /*otherGsMarker*/) const
{
	assertReadEnabled();
	Acad::ErrorStatus es = Acad::eOk;
	if (ent == NULL)
		return Acad::eNullEntityPointer;

	// The idea is to intersect each side of the polygon
	// with the given entity and return all the points.
	// 
	// For non-R12-entities, with
	// intersection methods defined, we call that method for
	// each of the sides of the polygon. For R12-entities,
	// we use the locally defined intersectors, since their
	// protocols are not implemented.
	//
	if (ent->isKindOf(AcDbLine::desc())) {
		if ((es = intLine(this, AcDbLine::cast(ent),
			intType, NULL, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(AcDbArc::desc())) {
		if ((es = intArc(this, AcDbArc::cast(ent), intType,
			NULL, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(AcDbCircle::desc())) {
		if ((es = intCircle(this, AcDbCircle::cast(ent),
			intType, NULL, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(AcDb2dPolyline::desc())) {
		if ((es = intPline(this, AcDb2dPolyline::cast(ent),
			intType, NULL, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(AcDb3dPolyline::desc())) {
		if ((es = intPline(this, AcDb3dPolyline::cast(ent),
			intType, NULL, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(TunnelNode::desc())) {

		TunnelNode* node = TunnelNode::cast(ent);

		std::vector<AcGeVector3d> vvs;
		getVerticalVector(vvs);

		AcDbCircle circle(node->getPosition(), vvs.at(0), node->getRadius());

		es = intCircle(this, &circle, intType, NULL, points);

	}
	else if (ent->isKindOf(Tunnel_Base::desc())) {
		/*AcGePoint3dArray vertexArray;
		if ((es = getVertices3(vertexArray))
			!= Acad::eOk)
		{
			return es;
		}
		if (intType == AcDb::kExtendArg
			|| intType == AcDb::kExtendBoth)
		{
			intType = AcDb::kExtendThis;
		}

		AcDbLine *pAcadLine;
		std::vector<AcGeVector3d> vvs;
		getVerticalVector(vvs);
		for (int i = 0; i < vertexArray.length() - 1; i++) {
			pAcadLine = new AcDbLine();
			pAcadLine->setStartPoint(vertexArray[i]);
			pAcadLine->setEndPoint(vertexArray[i + 1]);
			pAcadLine->setNormal(vvs.at(i));

			if ((es = ent->intersectWith(pAcadLine, intType,
				points)) != Acad::eOk)
			{
				delete pAcadLine;
				return es;
			}
			delete pAcadLine;
		}*/
	}
	return es;
}

Acad::ErrorStatus
Tunnel_Base::subIntersectWith(
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

	// The idea is to intersect each side of the polygon
	// with the given entity and return all the points.
	// 
	// For non-R12-entities, with
	// intersection methods defined, we call that method for
	// each of the sides of the polygon. For R12-entities,
	// we use the locally defined intersectors, since their
	// protocols are not implemented.
	//

	if (ent->isKindOf(AcDbLine::desc())) {
		if ((es = intLine(this, AcDbLine::cast(ent),
			intType, &projPlane, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(AcDbArc::desc())) {
		if ((es = intArc(this, AcDbArc::cast(ent), intType,
			&projPlane, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(AcDbCircle::desc())) {
		if ((es = intCircle(this, AcDbCircle::cast(ent),
			intType, &projPlane, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(AcDb2dPolyline::desc())) {
		if ((es = intPline(this, AcDb2dPolyline::cast(ent),
			intType, &projPlane, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if (ent->isKindOf(AcDb3dPolyline::desc())) {
		if ((es = intPline(this, AcDb3dPolyline::cast(ent),
			intType, &projPlane, points)) != Acad::eOk)
		{
			return es;
		}
	}
	else if(ent->isKindOf(Tunnel_Base::desc())){
		AcGePoint3dArray vertexArray;
		if ((es = getVertices3(vertexArray))
			!= Acad::eOk)
		{
			return es;
		}
		if (intType == AcDb::kExtendArg
			|| intType == AcDb::kExtendBoth)
		{
			intType = AcDb::kExtendThis;
		}

		AcDbLine *pAcadLine;
		int i;
		std::vector<AcGeVector3d> vvs;
		getVerticalVector(vvs);
		for (i = 0; i < vertexArray.length() - 1; i++) {
			pAcadLine = new AcDbLine();
			pAcadLine->setStartPoint(vertexArray[i]);
			pAcadLine->setEndPoint(vertexArray[i + 1]);
			pAcadLine->setNormal(vvs.at(i));

			if ((es = ent->intersectWith(pAcadLine, intType,
				projPlane, points)) != Acad::eOk)
			{
				delete pAcadLine;
				return es;
			}
			delete pAcadLine;
		}

		// All the points that we selected in this process are on
		// the other curve; we are dealing with apparent
		// intersection. If the other curve is 3D or is not
		// on the same plane as poly, the points are not on
		// poly.
		// 
		// In this case, we need to do some more work. Project the
		// points back onto the plane. They should lie on
		// the projected poly. Find points on real poly
		// corresponding to the projected points.

		AcGePoint3d projPt, planePt;
		AcGePoint3dArray pts;
		AcGeLine3d line;

		AcGePlane polyPlane;
		AcDb::Planarity plnrty;
		getPlane(polyPlane, plnrty);

		for (i = 0; i < points.length(); i++) {

			// Define a line starting from the projPt and
			// along the normal.  Intersect the polygon with
			// that line. Find all the points and pick the
			// one closest to the given point.
			//


			projPt = points[i].orthoProject(projPlane);
			line.set(projPt, projPlane.normal());
			if ((es = intLine(this, line, pts))
				!= Acad::eOk)
			{
				return es;
			}

			planePt = projPt.project(polyPlane,
				projPlane.normal());
			points[i] = pts[0];
			double length = (planePt - pts[0]).length();
			double length2;

			for (int j = 1; j < pts.length(); j++) {
				if ((length2 = (planePt - pts[j]).length())
					< length)
				{
					points[i] = pts[j];
					length = length2;
				}
			}
		}
	}
	
	return es;
}

Acad::ErrorStatus Tunnel_Base::subExplode(AcDbVoidPtrArray& entitySet) const
{
	assertReadEnabled();

	Acad::ErrorStatus es = Acad::eOk;

	uint32_t pointsCount = m_allPoints3.size();
	uint32_t facesCount = m_allFaces.size();

	AcGePoint3dArray vertexArray;
	vertexArray.setPhysicalLength(pointsCount);

	AcArray<AcCmEntityColor> clrArray;
	for (uint32_t i = 0; i < pointsCount; i++)
	{
		vertexArray.append(m_allPoints3.at(i));
		clrArray.append(m_allColors.at(i));
	}

	AcArray<Adesk::Int32> faceArray;
	faceArray.setPhysicalLength(facesCount);

	for (uint32_t i = 0; i < facesCount; i++)
	{
		faceArray.append(m_allFaces.at(i));
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


bool Tunnel_Base::displayTag(bool display, bool set) const
{
	assertReadEnabled();
	Acad::ErrorStatus es;

	const AcDbHandle& tagHandle = this->getTagHandle();
	TunnelTag* pTag = nullptr;
	ObjectOpener<TunnelTag> opener(pTag, tagHandle, AcDb::kForWrite);
	if ((es = opener.open()) == Acad::eOk)
	{
		if (set == false)
		{
			if (pTag->visibility() == AcDb::Visibility::kVisible)
				return true;
			else
			{
				return false;
			}
		}
		else
		{
			if (display == true)
			{
				pTag->setVisibility(AcDb::Visibility::kVisible);
				return true;
			}
			else
			{
				pTag->setVisibility(AcDb::Visibility::kInvisible);
				return false;
			}
		}	
	}
}

bool Tunnel_Base::displayNodes(bool display, bool set) const
{
	assertReadEnabled();
	Acad::ErrorStatus es;

	const std::vector<AcDbHandle>& handles = this->getNodesHandle();
	bool visible = false;
	for (auto& handle : handles)
	{
		TunnelNode *pNode = nullptr;
		ObjectOpener<TunnelNode> opener(pNode, handle, AcDb::kForWrite);
		if ((es = opener.open()) == Acad::eOk)
		{
			if (set == false)
			{
				if (pNode->visibility() == AcDb::Visibility::kVisible)
					return true;
				else
				{
					return false;
				}
			}
			else
			{
				if (display == true)
				{
					pNode->setVisibility(AcDb::Visibility::kVisible);
					visible = true;
				}
				else
				{
					pNode->setVisibility(AcDb::Visibility::kInvisible);
					visible = false;
				}
			}
		}
	}
	return visible;
}



Acad::ErrorStatus Tunnel_Base::addVertexAt(unsigned int index, const AcGePoint3d& pt)
{
	assertWriteEnabled();
	UInt32 segmentCount = getGeoSegments();
	if (index > segmentCount + 2)
		return Acad::eInvalidIndex;
}

Acad::ErrorStatus
Tunnel_Base::getVertices2(AcGePoint3dArray& vertexArray) const
{
	assertReadEnabled();

	for (size_t i = 0; i < getGeoSegments() * 4; i++)
	{
		vertexArray.append(m_allPoints2.at(i));
	}
	return Acad::eOk;
}

Acad::ErrorStatus
Tunnel_Base::getVertices3(AcGePoint3dArray& vertexArray) const
{
	assertReadEnabled();
	for (size_t i = 0; i < m_allPoints3.size(); i++)
	{
		vertexArray.append(m_allPoints3.at(i));
	}
	return Acad::eOk;
}

Acad::ErrorStatus 
Tunnel_Base::getFaces3(AcGeIntArray& faceArray) const
{
	assertReadEnabled();
	for (size_t i = 0; i <m_allFaces.size(); i++)
	{
		faceArray.append(m_allFaces.at(i));
	}
	return Acad::eOk;
}

Acad::ErrorStatus  
Tunnel_Base::getVerticesColors(AcGeIntArray& colorArray) const
{
	assertReadEnabled();
	for (size_t i = 0; i < m_allColors.size(); i++)
	{
		AcCmEntityColor color = m_allColors.at(i);
		UInt32 icolor = 0;
		RGBToUint32(color.red(), color.green(), color.blue(), icolor);
		colorArray.append(icolor);
	}
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::setBasePoints(const std::vector<AcGePoint3d>& pts)
{
	assertWriteEnabled();

	MIMDEBUGASSERT(pts.size() > 1);

	m_geo_segments = pts.size() - 1;

	m_geo_basePoints = pts;
	//allocateBuffer();
	m_drawable_basePoints = m_geo_basePoints;
	return Acad::eOk;
}


Acad::ErrorStatus Tunnel_Base::setColors(const std::vector<UInt32>& colors)
{
	assertWriteEnabled();
	if (colors.size() != m_color_segments + 1)
	{
		MIMDEBUGASSERT(colors.size() == m_color_segments + 1);
		return Acad::eInvalidIndex;
	}
	m_colors = colors;
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::setNodesHandle(const std::vector<AcDbHandle>& handles)
{
	UInt32 segmentCount = getGeoSegments();

	if (handles.size() != segmentCount + 1)
	{
		MIMDEBUGASSERT(handles.size() == segmentCount + 1);
		return Acad::eInvalidIndex;
	}


	//删除无效的节点
	for (auto& v : m_nodesHandle)
	{
		bool none = std::none_of(handles.begin(), handles.end(), [&](AcDbHandle handle) {return handle == v; });
		if (none)
		{
			TunnelNode* pNode = nullptr;
			ObjectOpener<TunnelNode> opener(pNode, v, AcDb::kForWrite);
			Acad::ErrorStatus es = opener.open();
			if (es == Acad::eOk)
			{
				AcDbHandle myHandle = nullptr;
				this->getAcDbHandle(myHandle);
				pNode->removeTunnel(myHandle);
				pNode->tryErase();
			}		
		}
	}
	m_nodesHandle = handles;
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::setTagHandle(const AcDbHandle& handle)
{
	m_tagHandle = handle;
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::setType(const TCHAR* type)
{
	assertWriteEnabled();
	acutDelString(m_type);
	m_type = NULL;
	if (type != NULL)
	{
		acutUpdString(type, m_type);
	}
	return Acad::eOk;
}


Acad::ErrorStatus Tunnel_Base::setName(const TCHAR* name)
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

Acad::ErrorStatus Tunnel_Base::setTagData(const TCHAR* tag)
{
	assertWriteEnabled();
	acutDelString(m_tagData);
	m_tagData = NULL;
	if (tag != NULL)
	{
		acutUpdString(tag, m_tagData);
	}
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::setLocation(const TCHAR* location)
{
	acutDelString(m_location);
	m_location = NULL;
	if (location != NULL)
	{
		acutUpdString(location, m_location);
	}
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::setColorSegments(int num)
{
	if (num == 0)
		return Acad::eOk;

	assertWriteEnabled();
	m_color_segments = num;
	m_colors = std::vector<uint32_t>(m_color_segments + 1, 0x0000b3ff);
	return Acad::eOk;
}


Acad::ErrorStatus Tunnel_Base::setTemperatures(const std::vector<Int16> temperatures)
{
	assertWriteEnabled();
	m_temperatures = temperatures;

	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::setClose(bool close)
{
	if (m_closed == close)
		return Acad::eOk;
	if (close == true && getGeoSegments() < 2)
		return Acad::eOk;

	assertWriteEnabled();
	
	m_closed = close;
	m_drawable_basePoints = m_geo_basePoints;
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::changeVertice(const AcGePoint3d& vertice, int index)
{
	assertWriteEnabled();

	MIMDEBUGASSERT(index <= m_geo_basePoints.size() - 1);
	m_geo_basePoints.at(index) = vertice;
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Base::changeNodeColor(UInt32 color, int index)
{
	assertWriteEnabled();
	if (m_colors.size() <= index)
		return Acad::eInvalidIndex;

	m_colors.at(index) = color;
	return Acad::eOk;
}

const Adesk::UInt16 Tunnel_Base::getGeoSegments() const
{
	assertReadEnabled();

    return m_geo_segments;
}

const std::vector<AcGePoint3d>& Tunnel_Base::getBasePoints() const
{
	assertReadEnabled();
	return  m_geo_basePoints;
}
const TCHAR* Tunnel_Base::getType() const
{
	assertReadEnabled();
	return m_type;
}

const TCHAR* Tunnel_Base::getName() const
{
	assertReadEnabled();
	return m_name;
}

const std::vector<AcDbHandle>& Tunnel_Base::getNodesHandle() const
{
	assertReadEnabled();
	return m_nodesHandle;
}

const AcDbHandle& Tunnel_Base::getTagHandle() const
{
	assertReadEnabled();
	return m_tagHandle;
}

const TCHAR* Tunnel_Base::getTagData() const
{
	assertReadEnabled();
	return m_tagData;
}
const TCHAR* Tunnel_Base::getLocation() const
{
	assertReadEnabled();
	return m_location;
}

const int Tunnel_Base::getColorSegments() const
{
	assertReadEnabled();
	return m_color_segments;
}

const std::vector<Int16> Tunnel_Base::getTemperatures() const
{
	assertReadEnabled();
	return m_temperatures;
}

const std::vector<UInt32>& Tunnel_Base::getColors() const
{
	assertReadEnabled();
	return m_colors;
}


void Tunnel_Base::setResultBuffer()
{
	
	m_allPoints3.clear();
	m_allPoints2.clear();
	m_allFaces.clear();
	m_allColors.clear();
	m_allArrawPosition.clear();

	setDrawableBasePoints();

	calculateColorBaseVertice(m_drawable_basePoints);
	//计算出每个顶点
	setVetices(m_allPoints3, m_allPoints2);

	//计算出每个面索引
	setFaceList(m_allFaces);


	setVerticesColorList(m_allColors);
}

void Tunnel_Base::getCenterVector(std::vector<AcGeVector3d>& cvs) const
{
	UInt32 segmentCount = getDrawableSegments();

	for (size_t i = 0; i < segmentCount; i++)
	{
		cvs.emplace_back((m_drawable_basePoints.at(i + 1) - m_drawable_basePoints.at(i)));
	}

}

void Tunnel_Base::getHorizontalVector(std::vector<AcGeVector3d>& hvs) const
{
	std::vector<AcGeVector3d> cvs;
	getCenterVector(cvs);
	for (auto& vector : cvs)
	{
		hvs.emplace_back(vector.perpVector());
	}
}
void  Tunnel_Base::getVerticalVector(std::vector<AcGeVector3d>& vvs) const
{
	std::vector<AcGeVector3d> hvs, cvs;
	getHorizontalVector(hvs);
	getCenterVector(cvs);
	int i = 0;
	for (auto vector : hvs)
	{
		vector.rotateBy(PI*0.5, cvs.at(i));
		vvs.emplace_back(vector);
		i++;
	}
}

double Tunnel_Base::getLengthSum() const
{
	UInt16 segmentCount = getDrawableSegments();
	double length = 0;

	for (int i = 0; i < segmentCount; i++)
	{
		length += (m_drawable_basePoints.at(i + 1) - m_drawable_basePoints.at(i)).length();
	}

	return length;
}

std::vector<AcGePoint3d> Tunnel_Base::getCenterPoints() const
{
	UInt16 segmentCount = getDrawableSegments();
	std::vector<AcGePoint3d> cps;
	for (size_t i = 0; i < segmentCount; i++)
	{
		cps.emplace_back(AcGePoint3d(
			(m_drawable_basePoints.at(i).x + m_drawable_basePoints.at(i+1).x)*0.5,
			(m_drawable_basePoints.at(i).y + m_drawable_basePoints.at(i+1).y)*0.5,
			(m_drawable_basePoints.at(i).z + m_drawable_basePoints.at(i+1).z)*0.5
		));
	}
	return cps;
}


void Tunnel_Base::calculateColorBaseVertice(const std::vector<AcGePoint3d>& points)
{
	m_color_basePoints.clear();
	m_turningPointIndexes.clear();
	m_hasColorPointIndexes.clear();
	if (m_color_segments == 1)
	{
		m_color_basePoints.assign(points.begin(), points.end());
		for (int i = 0; i < points.size(); i++)
		{
			m_turningPointIndexes.push_back(i);	
		}
		m_hasColorPointIndexes.push_back(0);
	}
	else
	{
		double lengthSum = getLengthSum();
		UINT16 geoSegments = getGeoSegments();
		std::vector<double> everyGeoSegmentLength;
		everyGeoSegmentLength.reserve(geoSegments);
		for (int i = 0; i < geoSegments; i++)
		{
			everyGeoSegmentLength.push_back((points.at(i + 1) - points.at(i)).length());
		}

		double everyColorSegmentLength = lengthSum / m_color_segments;
		double record_length = 0;

		for (int i = 0; i < geoSegments; i++)
		{
			record_length = setBaseColorPoints(points,record_length, i, everyGeoSegmentLength.at(i), everyColorSegmentLength);
		}		
	}
	m_hasColorPointIndexes.push_back(m_color_basePoints.size() - 1);
}


//record：几何分段长度减去n个颜色分段长度剩下的值，是大于0的值
//numOfGs：第m个几何分段
//gsl：当前的几何分段的长度
//csl：平均颜色分段的长度，
double Tunnel_Base::setBaseColorPoints(const std::vector<AcGePoint3d>& points, double record, int numOfGs, double gsl, double csl)
{
	double record_length = record;

	if (DoubleBiger(gsl, record_length))
	{
		if (numOfGs == 0 && DoubleEq(record_length, 0))
		{
			m_color_basePoints.push_back(points.at(numOfGs));
			m_turningPointIndexes.push_back(m_color_basePoints.size() - 1);
			m_hasColorPointIndexes.push_back(m_color_basePoints.size() - 1);
		}
		else if (DoubleBiger(record_length, 0))
		{
			std::vector<AcGeVector3d> cvs;
			getCenterVector(cvs);
			AcGeVector3d vector = cvs.at(numOfGs);

			AcGePoint3d point = points.at(numOfGs);
			point += vector.normalize()*record_length;

			m_color_basePoints.push_back(point);
			m_hasColorPointIndexes.push_back(m_color_basePoints.size() - 1);
		}
		record_length += csl;
		setBaseColorPoints(points,record_length, numOfGs, gsl, csl);
	}
	else
	{
		m_color_basePoints.push_back(points.at(numOfGs + 1));
		m_turningPointIndexes.push_back(m_color_basePoints.size() - 1);
		return record_length - gsl;
	}
}

void Tunnel_Base::setAllColorArray(std::vector<uint32_t>& allColor)
{
	std::vector<int> difference;
	std::insert_iterator<std::vector<int>> it(difference, difference.begin());
	std::set_difference(m_turningPointIndexes.begin(), m_turningPointIndexes.end(),
		m_hasColorPointIndexes.begin(), m_hasColorPointIndexes.end(),
		it);

	for (auto v : difference)
	{
		double length1 = (m_color_basePoints.at(v) - m_color_basePoints.at(v - 1)).length();
		double length2 = (m_color_basePoints.at(v + 1) - m_color_basePoints.at(v)).length();
		uint8_t r1, r2, g1, g2, b1, b2;
		UInt32ToRGB(allColor.at(v - 1), r1, g1, b1);
		UInt32ToRGB(allColor.at(v), r2, g2, b2);
		uint8_t r = (r1*length1 + r2*length2) / (length1 + length2);
		uint8_t g = (g1*length1 + g2*length2) / (length1 + length2);
		uint8_t b = (b1*length1 + b2*length2) / (length1 + length2);

		uint32_t color;
		RGBToUint32(r, g, b, color);
		allColor.insert(allColor.begin() + v, color);
	}
}

UInt16 Tunnel_Base::getDrawableSegments() const
{
	return m_drawable_basePoints.size() - 1;
}



void Tunnel_Base::drawArrow2(AcGiWorldDraw *mode)
{
	double w = getWidth();
	double h = getHeight();

	std::vector<AcGeVector3d> hvs;
	std::vector<AcGeVector3d> vvs;
	std::vector<AcGeVector3d> cvs;
	getHorizontalVector(hvs);
	getVerticalVector(vvs);
	getCenterVector(cvs);

	for (auto& nhv : hvs)
	{
		nhv.normalize();
	}

	for (auto& nvv : vvs)
	{
		nvv.normalize();
	}

	for (auto& ncv : cvs)
	{
		ncv.normalize();
	}

	int numofpoints = 5;
	int segmentCount = getGeoSegments();

	std::vector<AcGePoint3d> points = m_geo_basePoints;


	for (int i = 0; i < segmentCount; i++)
	{
		AcGePoint3d pts[8] = {};
		m_allArrawPosition.emplace_back(AcGePoint3d
		(
			0.5*(points.at(i).x + points.at(i + 1).x),
			0.5*(points.at(i).y + points.at(i + 1).y),
			0.5*(points.at(i).z + points.at(i + 1).z)
		));

		if (isAnimationMode == false)  //静态模式时
		{
			m_arrowPoints.at(i) = m_allArrawPosition.at(i);
		}
		else
		{
			AcGeVector3d v1, v2;
			v1 = m_arrowPoints.at(i) - points.at(i);
			v2 = points.at(i + 1) - points.at(i);
			if (v1.length() > v2.length())
				m_arrowPoints.at(i) = points.at(i);
			else
			{
				m_arrowPoints.at(i) += cvs.at(i) * 1;
			}
		}

		//箭头尖
		pts[7] = pts[0] = m_arrowPoints.at(i) + cvs.at(i)*w*2;
		//箭头末尾两个点
		pts[3] = m_arrowPoints.at(i) + hvs.at(i)*w*0.3 + cvs.at(i);
	    pts[4] = m_arrowPoints.at(i) - hvs.at(i)*w*0.3 + cvs.at(i);
		//箭头左右两个尖
		pts[1] = m_arrowPoints.at(i) + hvs.at(i)*w*0.5 + cvs.at(i)*w*1.5;
		pts[6] = m_arrowPoints.at(i) - hvs.at(i)*w*0.5 + cvs.at(i)*w*1.5;
		//箭头拐角处两个点
		pts[2] = m_arrowPoints.at(i) + hvs.at(i)*w*0.3 + cvs.at(i)*w*1.5;
		pts[5] = m_arrowPoints.at(i) - hvs.at(i)*w*0.3 + cvs.at(i)*w*1.5;

		mode->geometry().polyline(8,pts);

	}
}

void Tunnel_Base::drawArrow3(AcGiWorldDraw *mode)
{
	double w = getWidth();
	double h = getHeight();

	std::vector<AcGeVector3d> hvs;
	std::vector<AcGeVector3d> vvs;
	std::vector<AcGeVector3d> cvs;
	getHorizontalVector(hvs);
	getVerticalVector(vvs);
	getCenterVector(cvs);

	for (auto& nhv : hvs)
	{
		nhv.normalize();
	}

	for (auto& nvv : vvs)
	{
		nvv.normalize();
	}

	for (auto& ncv : cvs)
	{
		ncv.normalize();
	}

	int numofpoints = 4;
	int segmentCount = getGeoSegments();

	std::vector<AcGePoint3d> points = m_geo_basePoints;

	m_arrowPoints.resize(segmentCount);
	for (int i = 0; i < segmentCount; i++)
	{
		AcGePoint3d p0, p1, p2, p3;
		m_allArrawPosition.emplace_back(AcGePoint3d
		(
			0.5*(points.at(i).x + points.at(i+1).x),
			0.5*(points.at(i).y + points.at(i + 1).y),
			0.5*(points.at(i).z + points.at(i + 1).z)
		));

		if (isAnimationMode == false)  //静态模式时
		{
			m_arrowPoints.at(i) = m_allArrawPosition.at(i);
		}
		else
		{
			AcGeVector3d v1, v2;
			v1 = m_arrowPoints.at(i) - points.at(i);
			v2 = points.at(i+1) - points.at(i);
			if (v1.length() > v2.length())
				m_arrowPoints.at(i) = points.at(i);
			else
			{
				m_arrowPoints.at(i) += cvs.at(i) * 1;
			}
		}

		//前后两个点
		p0 = m_arrowPoints.at(i) + cvs.at(i)*w + vvs.at(i)*h;
		p2 = m_arrowPoints.at(i) + cvs.at(i)*w*0.3 + vvs.at(i)*h;
		//左右两个点
		p1 = m_arrowPoints.at(i) + hvs.at(i)*w*0.5 + vvs.at(i)*h;
		p3 = m_arrowPoints.at(i) - hvs.at(i)*w*0.5 + vvs.at(i)*h;

		AcGePoint3d pts[] = { p0,p1,p2,p3 };
		Adesk::Int32 faceList[] = { 3, 0, 1, 2, 3, 0, 2, 3 };

		mode->geometry().shell(numofpoints, pts, 8, faceList, nullptr, nullptr, nullptr);

		if (displayMode == DisplayMode::Real)
		{
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					pts[k].rotateBy(PI / 2, cvs.at(i), m_arrowPoints.at(i));
				}
				mode->geometry().shell(numofpoints, pts, 8, faceList, nullptr, nullptr, nullptr);
			}
		}	
	}
}


static Acad::ErrorStatus intLine(const Tunnel_Base*         poly,
	const AcGeLine3d        line,
	AcGePoint3dArray& points)
{
	Acad::ErrorStatus es = Acad::eOk;

	AcGePoint3dArray vertexArray;
	if ((es = poly->getVertices3(vertexArray)) != Acad::eOk) {
		return es;
	}

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


static Acad::ErrorStatus intLine(const Tunnel_Base*         poly,
	const AcDbLine*         line,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points)
{
	Acad::ErrorStatus es = Acad::eOk;

	AcGeLineSeg3d lnsg(line->startPoint(), line->endPoint());
	es = intLine(poly, lnsg, intType, projPlane, points);

	return es;
}


static Acad::ErrorStatus intLine(const Tunnel_Base*         poly,
	const AcGeLineSeg3d     lnsg,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points)
{
	Acad::ErrorStatus es = Acad::eOk;

	AcGePoint3dArray vertexArray;
	if ((es = poly->getVertices3(vertexArray)) != Acad::eOk) {
		return es;
	}

	AcGeLine3d aline(lnsg.startPoint(), lnsg.endPoint());
	AcGeLineSeg3d tlnsg;
	AcGePoint3d   pt;
	AcGePoint3d   dummy;

	for (int i = 0; i < vertexArray.length() - 1; i++) {

		tlnsg.set(vertexArray[i], vertexArray[i + 1]);

		AcGePlane ff;

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

	return es;
}

static Acad::ErrorStatus intArc(const Tunnel_Base*         poly,
	const AcDbArc*          arc,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points)
{
	Acad::ErrorStatus es = Acad::eOk;

	AcGeCircArc3d aarc(arc->center(), arc->normal(),
		arc->normal().perpVector(), arc->radius(),
		arc->startAngle(), arc->endAngle());
	es = intArc(poly, aarc, intType, projPlane, points);

	return es;
}


static Acad::ErrorStatus intArc(const Tunnel_Base*         poly,
	const AcGeCircArc3d     arc,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points)
{
	Acad::ErrorStatus es = Acad::eOk;

	AcGePoint3dArray vertexArray;
	if ((es = poly->getVertices3(vertexArray)) != Acad::eOk) {
		return es;
	}

	AcGeCircArc3d  acircle(arc.center(), arc.normal(), arc.radius());
	AcGeLineSeg3d lnsg;
	AcGePoint3d   pt1, pt2;
	AcGePoint3d   dummy1, dummy2;
	int           howMany;

	for (int i = 0; i < vertexArray.length() - 1; i++) {

		lnsg.set(vertexArray[i], vertexArray[i + 1]);

		if (intType == AcDb::kExtendArg || intType == AcDb::kExtendBoth) {
			if (projPlane == NULL) {
				if (!acircle.intersectWith(lnsg, howMany, pt1, pt2)) {
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
				if (!acircle.projIntersectWith(lnsg, projPlane->normal(),
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
		else {
			if (projPlane == NULL) {
				if (!arc.intersectWith(lnsg, howMany, pt1, pt2)) {
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
				if (!arc.projIntersectWith(lnsg, projPlane->normal(),
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

	for (int i = 0; i < vertexArray.length(); i+=2) {

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


static Acad::ErrorStatus intPline(const Tunnel_Base*         poly,
	AcDb2dPolyline*   pline,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points)
{
	Acad::ErrorStatus es = Acad::eOk;

	AcGePoint3dArray vertexArray;
	if ((es = poly->getVertices3(vertexArray)) != Acad::eOk) {
		return es;
	}

	AcDbSpline*      spline = NULL;
	AcDbLine*        acadLine = NULL;

	AcGePoint3dArray    pts;
	AcGeDoubleArray  bulges;
	int              numPoints, i;
	AcGePoint3d      pt1, pt2;

	AcGeCircArc3d    arc;
	AcGeLineSeg3d    lnsg;

	AcGeVector3d entNorm = pline->normal();
	AcDb::Intersect type = AcDb::kOnBothOperands;

	std::vector<AcGeVector3d> vvs;
	poly->getVerticalVector(vvs);

	switch (pline->polyType()) {

	case AcDb::k2dSimplePoly:
	case AcDb::k2dFitCurvePoly:

		// Intersect with each line or arc segment of the polyline. 
		// Depending on the intType, extend the last segment.

		if ((es = rx_scanPline(pline, pts, bulges)) != Acad::eOk) {
			return es;
		}
		numPoints = pts.length();

		for (i = 0; i < numPoints - 1; i++) {

			pt1 = pts[i]; pt2 = pts[i + 1];
			if (i == numPoints - 2)
				type = intType;

			if (bulges[i] > 1.0e-10) {     // create an arc

				acdbWcs2Ecs(asDblArray(pt1), asDblArray(pt1), asDblArray(entNorm),
					Adesk::kFalse);
				acdbWcs2Ecs(asDblArray(pt2), asDblArray(pt2), asDblArray(entNorm),
					Adesk::kFalse);

				rx_makeArc(pt1, pt2, bulges[i], entNorm, arc);
				intArc(poly, arc, type, projPlane, points);

			}
			else {                       // create a line

				lnsg.set(pt1, pt2);
				intLine(poly, lnsg, type, projPlane, points);
			}
		}
		break;

	case AcDb::k2dQuadSplinePoly:
	case AcDb::k2dCubicSplinePoly:

		if ((es = pline->getSpline(spline)) != Acad::eOk) {
			return es;
		}

		if (intType == AcDb::kExtendArg || intType == AcDb::kExtendBoth) {
			intType = AcDb::kExtendThis;
		}

		for (i = 0; i < vertexArray.length() - 1; i++) {

			acadLine = new AcDbLine();
			acadLine->setStartPoint(vertexArray[i]);
			acadLine->setEndPoint(vertexArray[i + 1]);
			acadLine->setNormal(vvs.at(i).normalize());

			if (projPlane == NULL) {
				spline->intersectWith(acadLine, intType, points);
			}
			else {
				spline->intersectWith(acadLine, intType, *projPlane, points);
			}

			delete acadLine;
		}

		delete spline;
		break;

	default:
		return Acad::eInvalidInput;
	}

	return es;
}


static Acad::ErrorStatus intPline(const Tunnel_Base*         poly,
	AcDb3dPolyline*   pline,
	AcDb::Intersect   intType,
	const AcGePlane*        projPlane,
	AcGePoint3dArray& points)
{
	Acad::ErrorStatus es = Acad::eOk;

	AcGePoint3dArray vertexArray;
	if ((es = poly->getVertices3(vertexArray)) != Acad::eOk) {
		return es;
	}

	AcDbSpline*      spline = NULL;
	AcDbLine*        acadLine = NULL;

	AcGePoint3dArray    pts;
	int              numPoints, i;

	AcGeLineSeg3d    lnsg;
	AcDb::Intersect  type = AcDb::kOnBothOperands;
	std::vector<AcGeVector3d> vvs;
	poly->getVerticalVector(vvs);

	switch (pline->polyType()) {

	case AcDb::k3dSimplePoly:

		if ((es = rx_scanPline(pline, pts)) != Acad::eOk) {
			return es;
		}
		numPoints = pts.length();

		// Intersect with each line segment of the polyline. 
		// Depending on the intType, extend the last segment.

		for (i = 0; i < numPoints - 1; i++) {

			if (i == numPoints - 2)
				type = intType;

			lnsg.set(pts[i], pts[i + 1]);
			if ((es = intLine(poly, lnsg, type, projPlane, points))
				!= Acad::eOk) {
				return es;
			}
		}

	case AcDb::k3dQuadSplinePoly:
	case AcDb::k3dCubicSplinePoly:

		if ((es = pline->getSpline(spline)) != Acad::eOk) {
			delete spline;
			return es;
		}

		if (intType == AcDb::kExtendArg || intType == AcDb::kExtendBoth) {
			intType = AcDb::kExtendThis;
		}

		for (i = 0; i < vertexArray.length() - 1; i++) {

			acadLine = new AcDbLine();
			acadLine->setStartPoint(vertexArray[i]);
			acadLine->setEndPoint(vertexArray[i + 1]);
			acadLine->setNormal(vvs.at(i).normalize());

			if (projPlane == NULL) {
				spline->intersectWith(acadLine, intType, points);
			}
			else {
				spline->intersectWith(acadLine, intType, *projPlane, points);
			}

			delete acadLine;
		}

		delete spline;
		break;

	default:
		return Acad::eInvalidInput;
	}

	return es;
}
