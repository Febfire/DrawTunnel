#pragma once
#ifndef TUNNEL_SQUARE_H
#define TUNNEL_SQUARE_H

namespace MIM
{
	class TunnelReactor;

	class __declspec(dllexport) Tunnel_Square final:public Tunnel_Base
	{
	public:

		ACRX_DECLARE_MEMBERS(Tunnel_Square);
		Tunnel_Square();

		virtual ~Tunnel_Square();

		virtual Acad::ErrorStatus dwgOutFields(AcDbDwgFiler *pFiler) const override;

		virtual Acad::ErrorStatus dwgInFields(AcDbDwgFiler *pFiler) override;

		virtual Acad::ErrorStatus subDeepClone(
			AcDbObject* pOwnerObject,
			AcDbObject*& pClonedObject,
			AcDbIdMapping& idMap,
			Adesk::Boolean isPrimary = true
		) const override;

		Acad::ErrorStatus setHeight(double height);
		Acad::ErrorStatus setWidth_t(double width);
		Acad::ErrorStatus setWidth_b(double width);

		double const getWidth_t() const;

		double const getWidth_b() const;

		virtual double const getHeight() const override;
		
	private:
		virtual double const getWidth() const override;

		virtual void setVetices(std::vector<AcGePoint3d>&, std::vector<AcGePoint3d>&) override;

		virtual void setFaceList(std::vector<int>&) override;

		virtual void setVerticesColorList(std::vector<AcCmEntityColor>&) override;
		
		virtual void createDerive(Tunnel_Base*& newTunnel) const override;
	private:
		double m_height;
		double m_width_t;
		double m_width_b;
	};
}

#endif // !TUNNEL_SQUARE_H