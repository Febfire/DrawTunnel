#pragma once
#ifndef TAG_H
#define TAG_H

#define TAG_VERSION 1

namespace MIM
{

	class __declspec(dllexport) TunnelTag final:public AcDbEntity
	{

	public:
		ACRX_DECLARE_MEMBERS(TunnelTag);
		TunnelTag();
		~TunnelTag();

	private:
		AcGePoint3d m_startPoint;
		AcGePoint3d m_endPoint;
		AcGePoint3d m_InflectionPoint;

		TCHAR*	 m_text;

		bool m_canErase;

	public:
		virtual Acad::ErrorStatus dwgOutFields(AcDbDwgFiler *pFiler) const override;

		virtual Acad::ErrorStatus dwgInFields(AcDbDwgFiler *pFiler) override;

		//重写删除操作控制不让直接删除标注
		virtual Acad::ErrorStatus subErase(Adesk::Boolean erasing) override;

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

	public:
		//起点与终点坐标
	    Acad::ErrorStatus setStartPoint(const AcGePoint3d& sp)
		{
			assertWriteEnabled();
			m_startPoint = sp;
			return Acad::eOk;
		}
		Acad::ErrorStatus setEndPoint(const AcGePoint3d& ep)
		{
			assertWriteEnabled();
			m_endPoint = ep;
			return Acad::eOk;
		}

		Acad::ErrorStatus setInflectionPoint(const AcGePoint3d& ip)
		{
			assertWriteEnabled();
			m_InflectionPoint = ip;
			return Acad::eOk;
		}
		
		Acad::ErrorStatus setText(const TCHAR* text)
		{
			assertWriteEnabled();
			acutDelString(m_text);
			m_text = NULL;
			if (text != NULL)
			{
				acutUpdString(text, m_text);
			}
			return Acad::eOk;
		}

		void canErase(bool can)
		{
			assertWriteEnabled();
			m_canErase = can;
		}

		const AcGePoint3d& getStartPoint() const
		{
			assertReadEnabled();
			return m_startPoint;
		}
		const AcGePoint3d& getEndPoint() const
		{
			assertReadEnabled();
			return m_endPoint;
		}

		const AcGePoint3d& getInflectionPoint() const
		{
			assertReadEnabled();
			return m_InflectionPoint;
		}

		const TCHAR* getText() const
		{
			assertReadEnabled();
			return m_text;
		}

	};
}
#endif //TAG_H