#include "StdAfx.h"
#include "Node.h"

//using namespace MIN;

//Autodesk::AutoCAD::DatabaseServices::Handle ToHandle(const AcDbHandle& hdl);

//////////////////////////////////////////////////////////////////////////
// constructor
MIM::Node::Node()
	:Autodesk::AutoCAD::DatabaseServices::Entity(System::IntPtr(new TunnelNode()), true)
{
}

//////////////////////////////////////////////////////////////////////////
MIM::Node::Node(System::IntPtr unmanagedPointer, bool autoDelete)
	: Autodesk::AutoCAD::DatabaseServices::Entity(unmanagedPointer, autoDelete)
{
}

void MIM::Node::Position::set(Point3d point)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setPosition(GETPOINT3D(point)));
}

Point3d MIM::Node::Position::get()
{
	return ToPoint3d(GetImpObj()->getPosition());
}


System::Void
MIM::Node::AppendTunnel(Autodesk::AutoCAD::DatabaseServices::Handle handle, int index)
{
	GetImpObj()->appendTunnel(GETHANDLE(handle), index);
}

System::Void 
MIM::Node::ChangeIndex(Autodesk::AutoCAD::DatabaseServices::Handle handle, int index)
{
	GetImpObj()->changeIndex(GETHANDLE(handle), index);
}

System::Void 
MIM::Node::RemoveTunnel(Autodesk::AutoCAD::DatabaseServices::Handle handle)
{
	GetImpObj()->removeTunnel(GETHANDLE(handle));
}

System::Void MIM::Node::TryErase()
{
	GetImpObj()->tryErase();
}

size_t MIM::Node::CountOfBind::get()
{
	return GetImpObj()->getCountsOfBinded();
}

void MIM::Node::NodeColor::set(System::UInt32 color)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setNodeColor(color));
}

System::UInt32
MIM::Node::NodeColor::get()
{
	return GetImpObj()->getNodeColor();
}

void MIM::Node::Radius::set(System::UInt32 radius)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setRadius(radius));
}

System::UInt32
MIM::Node::Radius::get()
{
	return GetImpObj()->getRadius();
}


void MIM::Node::Name::set(String^ name)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setName(StringToCIF(name)));
}

String^ MIM::Node::Name::get()
{
	return CIFToString(GetImpObj()->getName());
}

void MIM::Node::Location::set(String^ colliery)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setLocation(StringToCIF(colliery)));
}

String^ MIM::Node::Location::get()
{
	return CIFToString(GetImpObj()->getLocation());
}