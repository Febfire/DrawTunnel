#include "StdAfx.h"
#include "StandardLine.h"

ACRX_DXF_DEFINE_MEMBERS(StandardLine, AcDbEntity, AcDb::kDHL_CURRENT,
	AcDb::kMReleaseCurrent, 0, STANDARDLINE, /*MSG0*/"KLND");

StandardLine::StandardLine():
	m_startPoint(AcGePoint3d(0,0,0)),m_endPoint(AcGePoint3d(1000, 800, 800)),
	m_InflectionPoint(AcGePoint3d(500, 0, 0)),m_isBinded(false),m_text(NULL)
{
}
StandardLine::~StandardLine()
{
	acutDelString(m_text);
}

//----------------------------------------------------------------------

Acad::ErrorStatus 
StandardLine::dwgOutFields(AcDbDwgFiler *pFiler) const
{
	assertReadEnabled();
	Acad::ErrorStatus es = AcDbEntity::dwgOutFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version = VERSION;
	pFiler->writeItem(version);

	pFiler->writePoint3d(m_startPoint);
	pFiler->writePoint3d(m_InflectionPoint);
	pFiler->writePoint3d(m_endPoint);

	pFiler->writeBool(m_isBinded);

	if (m_text)
		pFiler->writeString(m_text);
	
	return pFiler->filerStatus();
}

//----------------------------------------------------------------------

Acad::ErrorStatus 
StandardLine::dwgInFields(AcDbDwgFiler *pFiler)
{
	assertWriteEnabled();
	Acad::ErrorStatus es = AcDbEntity::dwgInFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version;

	pFiler->readItem(&version);
	if (version > VERSION)
		return Acad::eMakeMeProxy;

	pFiler->readPoint3d(&m_startPoint);
	pFiler->readPoint3d(&m_InflectionPoint);
	pFiler->readPoint3d(&m_endPoint);

	pFiler->readBool(&m_isBinded);

	acutDelString(m_text);
	pFiler->readString(&m_text);

	return pFiler->filerStatus();
}

Adesk::Boolean StandardLine::subWorldDraw(AcGiWorldDraw *mode)
{
	assertReadEnabled();
	AcGePoint3d ptArray[3];
	ptArray[0] = m_startPoint;
	ptArray[1] = m_InflectionPoint;
	ptArray[2] = m_endPoint;

	mode->geometry().polyline(3,ptArray);

	AcGeVector3d vector = (m_InflectionPoint - m_startPoint)*0.07;
	
	AcGePoint3d points[4];
	points[0] = m_startPoint;
	AcGePoint3d tmpPoint = points[0] + vector;
	points[1].set(tmpPoint.x, tmpPoint.y, tmpPoint.z);
	points[2].set(tmpPoint.x, tmpPoint.y, tmpPoint.z);
	points[3].set(tmpPoint.x, tmpPoint.y, tmpPoint.z);
	
	AcGeVector3d vs[3];
	vs[0] = ((m_InflectionPoint - m_startPoint)*0.04).rotateBy(PI / 2, AcGeVector3d::kZAxis);
	vs[1] = ((m_InflectionPoint - m_startPoint)*0.04).rotateBy(PI / 2, AcGeVector3d::kZAxis);
	vs[2] = ((m_InflectionPoint - m_startPoint)*0.04).rotateBy(PI / 2, AcGeVector3d::kZAxis);

	vs[0].rotateBy(2 * PI / 3, vector);
	vs[1].rotateBy(2 * PI / 3 * 2, vector);
	points[1] += vs[0];
	points[2] += vs[1];
	points[3] += vs[2];


	Adesk::Int32 facelist[] = {
		3,0,1,2,
		3,0,1,3,
		3,0,2,3,
		3,1,2,3
	};
	mode->geometry().shell(4, points, 16, facelist,
			NULL, NULL, NULL);
	return (AcDbEntity::subWorldDraw(mode));
}

Acad::ErrorStatus StandardLine::subGetGripPoints(AcGePoint3dArray &gripPoints,
	AcDbIntArray &osnapModes,
	AcDbIntArray &geomIds) const
{
	assertReadEnabled();

	Acad::ErrorStatus es = Acad::eOk;
	gripPoints.append(m_startPoint);
	gripPoints.append(m_InflectionPoint);
	gripPoints.append(m_endPoint);

	return es;
}

Acad::ErrorStatus StandardLine::subMoveGripPointsAt(const AcDbIntArray &indices, const AcGeVector3d &offset)
{
	assertWriteEnabled();
	if (indices.length() == 0 || offset.isZeroLength())
		return Acad::eOk;

	for (int i = 0; i < indices.length(); i++)
	{
		int idx = indices.at(i);
		acutIsPrint(offset.x);
		if (idx == 0) m_startPoint += offset;
		if (idx == 1)
		{
			m_startPoint += offset;
			m_InflectionPoint += offset;
			m_endPoint += offset;

		}
		if (idx == 2) m_endPoint += offset;
	}

	return Acad::eOk;
}

Acad::ErrorStatus StandardLine::subGetOsnapPoints(
	AcDb::OsnapMode     osnapMode,
	Adesk::GsMarker     gsSelectionMark,
	const AcGePoint3d&  pickPoint,
	const AcGePoint3d&  lastPoint,
	const AcGeMatrix3d& viewXform,
	AcGePoint3dArray&   snapPoints,
	AcDbIntArray &   geomIds) const
{
	assertReadEnabled();
	return (AcDbEntity::subGetOsnapPoints(osnapMode, gsSelectionMark, pickPoint, lastPoint, viewXform, snapPoints, geomIds));
}