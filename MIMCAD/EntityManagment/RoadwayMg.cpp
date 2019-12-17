#include "StdAfx.h"
#include "RoadwayNodeMg.h"
#include "RoadwayMg.h"

using namespace MIM;

Autodesk::AutoCAD::DatabaseServices::Handle ToHandle(const AcDbHandle& hdl);

//////////////////////////////////////////////////////////////////////////
// constructor
Autodesk::DefinedEnitity::RoadwayWrapper::RoadwayWrapper()
	:Autodesk::AutoCAD::DatabaseServices::Entity(System::IntPtr(new Roadway()), true)
{
}

//////////////////////////////////////////////////////////////////////////
Autodesk::DefinedEnitity::RoadwayWrapper::RoadwayWrapper(System::IntPtr unmanagedPointer, bool autoDelete)
	: Autodesk::AutoCAD::DatabaseServices::Entity(unmanagedPointer, autoDelete)
{
}

//////////////////////////////////////////////////////////////////////////
// set the start point
void Autodesk::DefinedEnitity::RoadwayWrapper::StartPoint::set(Point3d point)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setStartPoint(GETPOINT3D(point)));
}

//////////////////////////////////////////////////////////////////////////
// get the start point
Point3d Autodesk::DefinedEnitity::RoadwayWrapper::StartPoint::get()
{
	return ToPoint3d(GetImpObj()->getStartPoint());
}

//////////////////////////////////////////////////////////////////////////
// set the end point
void Autodesk::DefinedEnitity::RoadwayWrapper::EndPoint::set(Point3d point)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setEndPoint(GETPOINT3D(point)));
}
//////////////////////////////////////////////////////////////////////////
// get the end point
Point3d Autodesk::DefinedEnitity::RoadwayWrapper::EndPoint::get()
{
	return ToPoint3d(GetImpObj()->getEndPoint());
}

//////////////////////////////////////////////////////////////////////////
// set the name
void Autodesk::DefinedEnitity::RoadwayWrapper::Name::set(String^ name)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setName(StringToCIF(name)));
}

//////////////////////////////////////////////////////////////////////////
// get the name
String^ Autodesk::DefinedEnitity::RoadwayWrapper::Name::get()
{
	return CIFToString(GetImpObj()->getName());
}

//////////////////////////////////////////////////////////////////////////
// set the ExtensionData
void Autodesk::DefinedEnitity::RoadwayWrapper::ExtensionData::set(String^ extensionData)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setExtensionData(StringToCIF(extensionData)));
}

//////////////////////////////////////////////////////////////////////////
// get the ExtensionData
String^ Autodesk::DefinedEnitity::RoadwayWrapper::ExtensionData::get()
{
	return CIFToString(GetImpObj()->getExtensionData());
}

//////////////////////////////////////////////////////////////////////////
// set the startNode
void Autodesk::DefinedEnitity::RoadwayWrapper::StartNodeHandle::set(Autodesk::AutoCAD::DatabaseServices::Handle handle)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setStartNodeHandle(GETHANDLE(handle)));
}

//////////////////////////////////////////////////////////////////////////
//get the startNodeId
Autodesk::AutoCAD::DatabaseServices::Handle
Autodesk::DefinedEnitity::RoadwayWrapper::StartNodeHandle::get()
{
	return ToHandle(GetImpObj()->getStartNodeHandle());
}

//////////////////////////////////////////////////////////////////////////
// set the endNode
void Autodesk::DefinedEnitity::RoadwayWrapper::EndNodeHandle::set(Autodesk::AutoCAD::DatabaseServices::Handle handle)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setEndNodeHandle(GETHANDLE(handle)));
}

//////////////////////////////////////////////////////////////////////////
// get the endNodeId
Autodesk::AutoCAD::DatabaseServices::Handle 
Autodesk::DefinedEnitity::RoadwayWrapper::EndNodeHandle::get()
{
	return ToHandle(GetImpObj()->getEndNodeHandle());
}

//////////////////////////////////////////////////////////////////////////
bool Autodesk::DefinedEnitity::RoadwayWrapper::AnimateSwitch::get()
{
	return GetImpObj()->getAnimateSwich();
}
void Autodesk::DefinedEnitity::RoadwayWrapper::AnimateSwitch::set(bool swtch)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setAnimateSwich(swtch));
}

/*********************************************************/
