#include "StdAfx.h"
#include "Mark.h"


using namespace MIM;

//////////////////////////////////////////////////////////////////////////
// constructor
MIM::Mark::Mark()
	:Autodesk::AutoCAD::DatabaseServices::Entity(System::IntPtr(new DwgMark()), true)
{
}

//////////////////////////////////////////////////////////////////////////
MIM::Mark::Mark(System::IntPtr unmanagedPointer, bool autoDelete)
	: Autodesk::AutoCAD::DatabaseServices::Entity(unmanagedPointer, autoDelete)
{
}


void MIM::Mark::Uuid::set(String^ name)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setMark(StringToCIF(name)));
}

//////////////////////////////////////////////////////////////////////////
// get the name
String^ MIM::Mark::Uuid::get()
{
	return CIFToString(GetImpObj()->getMark());
}