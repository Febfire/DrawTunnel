#include "StdAfx.h"
#include "StandardLineMg.h"

//////////////////////////////////////////////////////////////////////////
// constructor
Autodesk::DefinedEnitity::StandardLineWrapper::StandardLineWrapper()
	:Autodesk::AutoCAD::DatabaseServices::Entity(System::IntPtr(new StandardLine()), true)
{
	acutPrintf(L"\n*********************Constructor");
}

//////////////////////////////////////////////////////////////////////////
Autodesk::DefinedEnitity::StandardLineWrapper::StandardLineWrapper(System::IntPtr unmanagedPointer, bool autoDelete)
	: Autodesk::AutoCAD::DatabaseServices::Entity(unmanagedPointer, autoDelete)
{
}
//
////////////////////////////////////////////////////////////////////////////
//// set the start point
//void Autodesk::DefinedEnitity::StandardLineWrapper::StartPoint::set(Point3d point)
//{
//	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setStartPoint(GETPOINT3D(point)));
//}
////////////////////////////////////////////////////////////////////////////
//// get the start point
//Point3d Autodesk::DefinedEnitity::StandardLineWrapper::StartPoint::get()
//{
//	return ToPoint3d(GetImpObj()->getStartPoint());
//}
//
////////////////////////////////////////////////////////////////////////////
//// set the end point
//void Autodesk::DefinedEnitity::StandardLineWrapper::EndPoint::set(Point3d point)
//{
//	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setEndPoint(GETPOINT3D(point)));
//}
////////////////////////////////////////////////////////////////////////////
//// get the end point
//Point3d Autodesk::DefinedEnitity::StandardLineWrapper::EndPoint::get()
//{
//	return ToPoint3d(GetImpObj()->getEndPoint());
//}

//////////////////////////////////////////////////////////////////////////
// set the inflection point
void Autodesk::DefinedEnitity::StandardLineWrapper::InflectionPoint::set(Point3d point)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setInflectionPoint(GETPOINT3D(point)));
}

//////////////////////////////////////////////////////////////////////////
// get the inflection point
Point3d Autodesk::DefinedEnitity::StandardLineWrapper::InflectionPoint::get()
{
	return ToPoint3d(GetImpObj()->getInflectionPoint());
}