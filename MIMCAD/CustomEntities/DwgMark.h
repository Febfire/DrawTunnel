#pragma once
#ifndef DWGMARK_H
#define DWGMARK_H

#define DWGMARK_VERSION 1

namespace MIM
{

	class __declspec(dllexport) DwgMark :public AcDbEntity
	{
	public:

		ACRX_DECLARE_MEMBERS(DwgMark);
		DwgMark():m_uuid(NULL) {}
		~DwgMark() {};

		virtual Acad::ErrorStatus   dwgInFields(AcDbDwgFiler *filer) override;
		virtual Acad::ErrorStatus   dwgOutFields(AcDbDwgFiler *filer) const override;

		inline Acad::ErrorStatus setMark(const TCHAR* mark);
		inline const WCHAR* getMark() const;

	private:

		TCHAR* m_uuid;
	};

#endif // !DWGMARK_H

}
