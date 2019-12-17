#include "StdAfx.h"
#include "RoadwayMg.h"
#include "RoadwayNodeMg.h"
using namespace MIM;
Autodesk::AutoCAD::DatabaseServices::Handle ToHandle(const AcDbHandle& hdl);

//////////////////////////////////////////////////////////////////////////
// constructor
Autodesk::DefinedEnitity::RoadwayNodeWrapper::RoadwayNodeWrapper()
	:Autodesk::AutoCAD::DatabaseServices::Entity(System::IntPtr(new RoadwayNode()), true)
{
	acutPrintf(L"\nRoadwayNodeWrapper***********Constructor");
}

//////////////////////////////////////////////////////////////////////////
Autodesk::DefinedEnitity::RoadwayNodeWrapper::RoadwayNodeWrapper(System::IntPtr unmanagedPointer, bool autoDelete)
	: Autodesk::AutoCAD::DatabaseServices::Entity(unmanagedPointer, autoDelete)
{
}

void Autodesk::DefinedEnitity::RoadwayNodeWrapper::Position::set(Point3d point)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setPosition(GETPOINT3D(point)));
}

Point3d Autodesk::DefinedEnitity::RoadwayNodeWrapper::Position::get()
{
	return ToPoint3d(GetImpObj()->getPosition());
}


System::Void
Autodesk::DefinedEnitity::RoadwayNodeWrapper::appendRoadway(Autodesk::AutoCAD::DatabaseServices::Handle handle, int side)
{
	GetImpObj()->appendRoadway(GETHANDLE(handle), side);
}

size_t Autodesk::DefinedEnitity::RoadwayNodeWrapper::CountOfBound::get()
{
	return GetImpObj()->getCountsOfBinded();
}