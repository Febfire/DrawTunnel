#pragma once
#ifndef STANDARDLINE_H
#define STANDARDLINE_H

#define PI 3.14159265358979323846
class __declspec(dllexport) StandardLine final:public AcDbEntity
{
#ifndef VERSION
#define VERSION 1

#endif // !VERSION

public:
	ACRX_DECLARE_MEMBERS(StandardLine);
    StandardLine();
	~StandardLine();

private:
	bool        m_isBinded;

	AcGePoint3d m_startPoint;
	AcGePoint3d m_endPoint;
	AcGePoint3d m_InflectionPoint;
	
	TCHAR*	 m_text;

public:
	virtual Acad::ErrorStatus dwgOutFields(AcDbDwgFiler *pFiler) const override;

	virtual Acad::ErrorStatus dwgInFields(AcDbDwgFiler *pFiler) override;

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

public:
	//起点与终点坐标
	/*inline Acad::ErrorStatus setStartPoint(const AcGePoint3d& sp)
	{
		assertWriteEnabled();
		m_startPoint = sp;
		return Acad::eOk;
	}
	inline Acad::ErrorStatus setEndPoint(const AcGePoint3d& ep)
	{
		assertWriteEnabled();
		m_endPoint = ep;
		return Acad::eOk;
	}*/

	inline Acad::ErrorStatus setInflectionPoint(const AcGePoint3d& ip)
	{
		assertWriteEnabled();
		m_InflectionPoint = ip;
		return Acad::eOk;
	}

	inline AcGePoint3d getStartPoint() const
	{
		assertReadEnabled();
		return m_startPoint;
	}
	inline AcGePoint3d getEndPoint() const
	{
		assertReadEnabled();
		return m_endPoint;
	}

	inline AcGePoint3d getInflectionPoint() const
	{
		assertReadEnabled();
		return m_InflectionPoint;
	}
	
};

#endif //STANDARDLINE_H