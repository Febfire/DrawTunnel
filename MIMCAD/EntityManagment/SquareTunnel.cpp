#include "StdAfx.h"
#include "BaseTunnel.h"
#include "SquareTunnel.h"

//using namespace MIN;

//Autodesk::AutoCAD::DatabaseServices::Handle ToHandle(const AcDbHandle& hdl);

//////////////////////////////////////////////////////////////////////////
// constructor

MIM::SquareTunnel::SquareTunnel()
	:MIM::BaseTunnel(System::IntPtr(new Tunnel_Square()), true)
{
}

//////////////////////////////////////////////////////////////////////////
MIM::SquareTunnel::SquareTunnel(System::IntPtr unmanagedPointer, bool autoDelete)
	: MIM::BaseTunnel(unmanagedPointer, autoDelete)
{
}


void MIM::SquareTunnel::BasePoints::set(System::Collections::Generic::List<Point3d>^ points)
{
	std::vector<AcGePoint3d> pts;
	for (int i = 0; i < points->Count; i++)
	{
		pts.emplace_back(GETPOINT3D(points[i]));
	}

	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setBasePoints(pts));
}

System::Collections::Generic::List<Point3d>^ MIM::SquareTunnel::BasePoints::get()
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


void MIM::SquareTunnel::Height::set(double height)
{
	GetImpObj()->setHeight(height);
}

double MIM::SquareTunnel::Height::get()
{
	return GetImpObj()->getHeight();
}


void MIM::SquareTunnel::Width_t::set(double width)
{
	GetImpObj()->setWidth_t(width);
}

double MIM::SquareTunnel::Width_t::get()
{
	return GetImpObj()->getWidth_t();
}

void MIM::SquareTunnel::Width_b::set(double width)
{
	GetImpObj()->setWidth_b(width);
}

double MIM::SquareTunnel::Width_b::get()
{
	return GetImpObj()->getWidth_b();
}