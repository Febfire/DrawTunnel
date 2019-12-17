#include "StdAfx.h"
#include "DwgMark.h"

using namespace MIM;

ACRX_DXF_DEFINE_MEMBERS(DwgMark, AcDbEntity,
	AcDb::kDHL_CURRENT, AcDb::kMReleaseCurrent,
	0, DWGMARK, /*MSG0*/"KLND");


Acad::ErrorStatus  
DwgMark::dwgInFields(AcDbDwgFiler *pFiler)
{
	assertWriteEnabled();
	Acad::ErrorStatus es = AcDbObject::dwgInFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version;

	pFiler->readItem(&version);
	if (version > DWGMARK_VERSION)
		return Acad::eMakeMeProxy;

	acutDelString(m_uuid);
	pFiler->readString(&m_uuid);

	pFiler->filerStatus();
}

Acad::ErrorStatus   
DwgMark::dwgOutFields(AcDbDwgFiler *pFiler) const
{
	assertReadEnabled();
	Acad::ErrorStatus es = AcDbObject::dwgOutFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version = DWGMARK_VERSION;
	pFiler->writeItem(version);

	if (m_uuid)
		pFiler->writeString(m_uuid);
	else
		pFiler->writeString(TEXT(""));

	return pFiler->filerStatus();

}

Acad::ErrorStatus DwgMark::setMark(const TCHAR* mark)
{
	assertWriteEnabled();
	acutDelString(m_uuid);
	m_uuid = NULL;
	if (mark != NULL)
	{
		acutUpdString(mark, m_uuid);
	}
	return Acad::eOk;
}

const TCHAR* DwgMark::getMark() const
{
	return m_uuid;
}

