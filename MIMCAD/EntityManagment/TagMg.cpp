#include "StdAfx.h"
#include "TagMg.h"
using namespace MIM;
//////////////////////////////////////////////////////////////////////////
// constructor
MIM::Tag::Tag()
	:Autodesk::AutoCAD::DatabaseServices::Entity(System::IntPtr(new TunnelTag()), true)
{
}

//////////////////////////////////////////////////////////////////////////
MIM::Tag::Tag(System::IntPtr unmanagedPointer, bool autoDelete)
	: Autodesk::AutoCAD::DatabaseServices::Entity(unmanagedPointer, autoDelete)
{
}

//////////////////////////////////////////////////////////////////////////
// set the start point
void MIM::Tag::StartPoint::set(Point3d point)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setStartPoint(GETPOINT3D(point)));
}
//////////////////////////////////////////////////////////////////////////
// get the start point
Point3d MIM::Tag::StartPoint::get()
{
	return ToPoint3d(GetImpObj()->getStartPoint());
}

//////////////////////////////////////////////////////////////////////////
// set the end point
void MIM::Tag::EndPoint::set(Point3d point)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setEndPoint(GETPOINT3D(point)));
}
//////////////////////////////////////////////////////////////////////////
// get the end point
Point3d MIM::Tag::EndPoint::get()
{
	return ToPoint3d(GetImpObj()->getEndPoint());
}

//////////////////////////////////////////////////////////////////////////
// set the inflection point
void MIM::Tag::InflectionPoint::set(Point3d point)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setInflectionPoint(GETPOINT3D(point)));
}

//////////////////////////////////////////////////////////////////////////
// get the inflection point
Point3d MIM::Tag::InflectionPoint::get()
{
	return ToPoint3d(GetImpObj()->getInflectionPoint());
}

void MIM::Tag::Text::set(String^ text)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setText(StringToCIF(text)));
}

//////////////////////////////////////////////////////////////////////////
// get the name
String^ MIM::Tag::Text::get()
{
	return CIFToString(GetImpObj()->getText());
}