#pragma once
#pragma once
#ifndef TUNNEL_CYLINDER
#define TUNNEL_CYLINDER

namespace MIM
{
	class TunnelReactor;

	class __declspec(dllexport) Tunnel_Cylinder final:public Tunnel_Base
	{
	public:

		ACRX_DECLARE_MEMBERS(Tunnel_Cylinder);
		Tunnel_Cylinder();

		virtual ~Tunnel_Cylinder();

		virtual Acad::ErrorStatus dwgOutFields(AcDbDwgFiler *pFiler) const override;

		virtual Acad::ErrorStatus dwgInFields(AcDbDwgFiler *pFiler) override;

		virtual Acad::ErrorStatus subDeepClone(
			AcDbObject* pOwnerObject,
			AcDbObject*& pClonedObject,
			AcDbIdMapping& idMap,
			Adesk::Boolean isPrimary = true
		) const override;

		Acad::ErrorStatus setRadius(double radius);
		const double getRadius() const;

	private:

		virtual void setVetices(std::vector<AcGePoint3d>&, std::vector<AcGePoint3d>&) override;

		virtual void setFaceList(std::vector<int>&) override;

		virtual void setVerticesColorList(std::vector<AcCmEntityColor>&) override;

		virtual const double getHeight() const override;

		virtual const double getWidth() const override;

		virtual void createDerive(Tunnel_Base*& newTunnel) const override;
	private:
		double m_radius;

		UInt16 m_steps;
	};
}

#endif // !TUNNEL_SQUARE_H