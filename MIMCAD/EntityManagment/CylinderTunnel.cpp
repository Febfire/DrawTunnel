#include "StdAfx.h"
#include "BaseTunnel.h"
#include "CylinderTunnel.h"


//using namespace MIN;


//////////////////////////////////////////////////////////////////////////
// constructor

MIM::CylinderTunnel::CylinderTunnel()
	:MIM::BaseTunnel(System::IntPtr(new Tunnel_Cylinder()), true)
{
}

//////////////////////////////////////////////////////////////////////////
MIM::CylinderTunnel::CylinderTunnel(System::IntPtr unmanagedPointer, bool autoDelete)
	: MIM::BaseTunnel(unmanagedPointer, autoDelete)
{
}


void MIM::CylinderTunnel::BasePoints::set(System::Collections::Generic::List<Point3d>^ points)
{
	std::vector<AcGePoint3d> pts;
	for (int i = 0; i < points->Count; i++)
	{
		pts.emplace_back(GETPOINT3D(points[i]));
	}

	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setBasePoints(pts));
}

System::Collections::Generic::List<Point3d>^ MIM::CylinderTunnel::BasePoints::get()
{
	const std::vector<AcGePoint3d> points = GetImpObj()->getBasePoints();
	System::Collections::Generic::List<Point3d>^ pointsList = gcnew System::Collections::Generic::List<Point3d>();
	for (auto &point : points)
	{
		pointsList->Add(ToPoint3d(point));
	}
	Point3dCollection pc;

	return pointsList;
}

void MIM::CylinderTunnel::Radius::set(double radius)
{
	GetImpObj()->setRadius(radius);
}

double MIM::CylinderTunnel::Radius::get()
{
	return GetImpObj()->getRadius();
}