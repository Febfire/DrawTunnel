#include "StdAfx.h"
#include "TunnelTag.h"
#include "utils.h"

using namespace MIM;
ACRX_DXF_DEFINE_MEMBERS(TunnelTag, AcDbEntity, AcDb::kDHL_CURRENT,
	AcDb::kMReleaseCurrent, 0, TUNNELTAG, /*MSG0*/"KLND");

TunnelTag::TunnelTag() :
	m_startPoint(AcGePoint3d(0, 0, 0)), m_endPoint(AcGePoint3d(100, 0, 0)),
	m_InflectionPoint(AcGePoint3d(100, 100, 0)),m_text(nullptr),m_canErase(false)
{
}
TunnelTag::~TunnelTag()
{
	acutDelString(m_text);
}

//----------------------------------------------------------------------

Acad::ErrorStatus
TunnelTag::dwgOutFields(AcDbDwgFiler *pFiler) const
{
	assertReadEnabled();
	Acad::ErrorStatus es = AcDbEntity::dwgOutFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version = TAG_VERSION;
	pFiler->writeItem(version);

	pFiler->writePoint3d(m_startPoint);
	pFiler->writePoint3d(m_InflectionPoint);
	pFiler->writePoint3d(m_endPoint);

	if (m_text)
		pFiler->writeString(m_text);
	else
		pFiler->writeString(TEXT(""));


	pFiler->writeBool(m_canErase);

	return pFiler->filerStatus();
}

//----------------------------------------------------------------------

Acad::ErrorStatus
TunnelTag::dwgInFields(AcDbDwgFiler *pFiler)
{
	assertWriteEnabled();
	Acad::ErrorStatus es = AcDbEntity::dwgInFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version;

	pFiler->readItem(&version);
	if (version > TAG_VERSION)
		return Acad::eMakeMeProxy;

	pFiler->readPoint3d(&m_startPoint);
	pFiler->readPoint3d(&m_InflectionPoint);
	pFiler->readPoint3d(&m_endPoint);

	acutDelString(m_text);
	pFiler->readString(&m_text);

	pFiler->readBool(&m_canErase);

	return pFiler->filerStatus();
}

Acad::ErrorStatus TunnelTag::subErase(Adesk::Boolean erasing)
{
	if (m_canErase == false)
		return Acad::ErrorStatus::eCannotBeErasedByCaller;

	assertWriteEnabled();
	m_canErase = erasing;
	Acad::ErrorStatus es = AcDbEntity::subErase(erasing);
	if (es != Acad::eOk)
		return (es);

	return Acad::eOk;
}

Adesk::Boolean TunnelTag::subWorldDraw(AcGiWorldDraw *mode)
{
	if (m_text == NULL || lstrcmpW(m_text,L"")==0) return (AcDbEntity::subWorldDraw(mode));

	assertReadEnabled();
	AcGePoint3d ptArray[2];
	ptArray[0] = m_startPoint;
	ptArray[1] = m_InflectionPoint;
	//ptArray[2] = m_endPoint;

	mode->geometry().polyline(2, ptArray);

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

	size_t textLen = wcslen(m_text);

	AcGeVector3d normal = (m_endPoint - m_InflectionPoint).perpVector();
	normal.rotateBy(PI, (m_endPoint - m_InflectionPoint));
	AcGeVector3d distance = (m_endPoint - m_InflectionPoint);

	AcGiTextStyle style;
	style.loadStyleRec();
	style.setStyleName(L"HZ_SF");
	style.setFileName(L"HZTXT.shx");
	style.setBigFontFileName(L"HZTXT.shx");
	style.setTextSize(distance.length() / textLen / 1.5);
	style.setXScale(1);
	style.setUnderlined(true);
	style.setOverlined(true);

	mode->geometry().text(m_InflectionPoint, normal, distance, m_text, textLen, 0, style);

	return (AcDbEntity::subWorldDraw(mode));
}

Acad::ErrorStatus TunnelTag::subGetGripPoints(AcGePoint3dArray &gripPoints,
	AcDbIntArray &osnapModes,
	AcDbIntArray &geomIds) const
{
	assertReadEnabled();

	Acad::ErrorStatus es = Acad::eOk;
	gripPoints.append(m_InflectionPoint);
	//gripPoints.append(m_endPoint);

	return es;
}

Acad::ErrorStatus TunnelTag::subMoveGripPointsAt(const AcDbIntArray &indices, const AcGeVector3d &offset)
{
	assertWriteEnabled();
	if (indices.length() == 0 || offset.isZeroLength())
		return Acad::eOk;

	for (int i = 0; i < indices.length(); i++)
	{
		int idx = indices.at(i);

		acutIsPrint(offset.x);

		if (idx == 0)
		{
			m_InflectionPoint += offset;
			m_endPoint += offset;
		}

		//if (idx == 1)m_endPoint += offset;
	}

	return Acad::eOk;
}

Acad::ErrorStatus TunnelTag::subGetOsnapPoints(
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

Acad::ErrorStatus TunnelTag::subTransformBy(const AcGeMatrix3d& xform)
{
	assertWriteEnabled();
	m_startPoint.transformBy(xform);
	m_InflectionPoint.transformBy(xform);
	m_endPoint.transformBy(xform);

	return Acad::eOk;
}

