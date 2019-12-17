#include "StdAfx.h"
#include "BaseTunnel.h"

//using namespace MIN;

Autodesk::AutoCAD::DatabaseServices::Handle ToHandle(const AcDbHandle& hdl);

//////////////////////////////////////////////////////////////////////////
// constructor
//////////////////////////////////////////////////////////////////////////
MIM::BaseTunnel::BaseTunnel(System::IntPtr unmanagedPointer, bool autoDelete)
	: Autodesk::AutoCAD::DatabaseServices::Curve(unmanagedPointer, autoDelete)
{
}



void MIM::BaseTunnel::NodesHandle::set(System::Collections::Generic::List<Autodesk::AutoCAD::DatabaseServices::Handle>^ handles)
{
	std::vector<AcDbHandle> hs;
	for (int i = 0; i < handles->Count; i++)
	{
		hs.emplace_back(GETHANDLE(handles[i]));
	}

	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setNodesHandle(hs));
}

System::Collections::Generic::List<Autodesk::AutoCAD::DatabaseServices::Handle>^ MIM::BaseTunnel::NodesHandle::get()
{
	const std::vector<AcDbHandle>& hs = GetImpObj()->getNodesHandle();
	System::Collections::Generic::List<Autodesk::AutoCAD::DatabaseServices::Handle>^ handleList = 
		gcnew System::Collections::Generic::List<Autodesk::AutoCAD::DatabaseServices::Handle>();
	for (auto &h : hs)
	{
		handleList->Add(ToHandle(h));
	}
	Point3dCollection pc;

	return handleList;
}

void MIM::BaseTunnel::Colors::set(System::Collections::Generic::List<UINT32>^ colors)
{
	std::vector<UINT32> cs;
	for (int i = 0; i < colors->Count; i++)
	{
		cs.emplace_back(colors[i]);
	}

	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setColors(cs));
}
System::Collections::Generic::List<UINT32>^ MIM::BaseTunnel::Colors::get()
{
	const std::vector<UINT32>& cs = GetImpObj()->getColors();

	System::Collections::Generic::List<UINT32>^ colorList =
		gcnew System::Collections::Generic::List<UINT32>();

	for (auto &c : cs)
	{
		colorList->Add(c);
	}

	return colorList;
}


void MIM::BaseTunnel::TagHandle::set(Autodesk::AutoCAD::DatabaseServices::Handle handle)
{
	GetImpObj()->setTagHandle(GETHANDLE(handle));
}

Autodesk::AutoCAD::DatabaseServices::Handle MIM::BaseTunnel::TagHandle::get()
{
	return ToHandle(GetImpObj()->getTagHandle());
}


void MIM::BaseTunnel::TunnelType::set(String^ type)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setType(StringToCIF(type)));
}


String^ MIM::BaseTunnel::TunnelType::get()
{
	return CIFToString(GetImpObj()->getType());
}


//////////////////////////////////////////////////////////////////////////
// set the name
void MIM::BaseTunnel::Name::set(String^ name)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setName(StringToCIF(name)));
}

//////////////////////////////////////////////////////////////////////////
// get the name
String^ MIM::BaseTunnel::Name::get()
{
	return CIFToString(GetImpObj()->getName());
}


void MIM::BaseTunnel::TagData::set(String^ tagData)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setTagData(StringToCIF(tagData)));
}

String^ MIM::BaseTunnel::TagData::get()
{
	return CIFToString(GetImpObj()->getTagData());
}

void MIM::BaseTunnel::Location::set(String^ colliery)
{
	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setLocation(StringToCIF(colliery)));
}

String^ MIM::BaseTunnel::Location::get()
{
	return CIFToString(GetImpObj()->getLocation());
}


void MIM::BaseTunnel::DisplayTag::set(bool display)
{
	GetImpObj()->displayTag(display,true);
}

bool MIM::BaseTunnel::DisplayTag::get()
{
	return GetImpObj()->displayTag(0, false);
}

void MIM::BaseTunnel::DisplayNodes::set(bool display)
{
	GetImpObj()->displayNodes(display, true);
}

bool MIM::BaseTunnel::DisplayNodes::get()
{
	return GetImpObj()->displayNodes(0, false);
}

void MIM::BaseTunnel::Segment::set(System::Int32 segment)
{
	GetImpObj()->setColorSegments(segment);
}

System::Int32 MIM::BaseTunnel::Segment::get()
{
	return GetImpObj()->getColorSegments();
}


void MIM::BaseTunnel::Temperatures::set(System::Collections::Generic::List<INT16>^ temperatures)
{
	std::vector<INT16> cs;
	for (int i = 0; i < temperatures->Count; i++)
	{
		cs.emplace_back(temperatures[i]);
	}

	Autodesk::AutoCAD::Runtime::Interop::Check(GetImpObj()->setTemperatures(cs));
}
System::Collections::Generic::List<INT16>^ MIM::BaseTunnel::Temperatures::get()
{
	const std::vector<INT16>& cs = GetImpObj()->getTemperatures();

	System::Collections::Generic::List<INT16>^ temperatureList =
		gcnew System::Collections::Generic::List<INT16>();

	for (auto &c : cs)
	{
		temperatureList->Add(c);
	}

	return temperatureList;
}

//////////////////////////////////////////////////////////////////////////

System::Collections::Generic::List<Vector3d>^ MIM::BaseTunnel::CenterVectors::get()
{
	std::vector<AcGeVector3d> cvs;
	GetImpObj()->getCenterVector(cvs);

	System::Collections::Generic::List<Vector3d>^ cvl =
		gcnew System::Collections::Generic::List<Vector3d>();

	for (auto& v : cvs)
	{
		cvl->Add(ToVector3d(v));
	}
	return cvl;
}

System::Collections::Generic::List<Vector3d>^ MIM::BaseTunnel::HorizontalVectors::get()
{
	std::vector<AcGeVector3d> hvs;
	GetImpObj()->getHorizontalVector(hvs);

	System::Collections::Generic::List<Vector3d>^ hvl =
		gcnew System::Collections::Generic::List<Vector3d>();

	for (auto& v : hvs)
	{
		hvl->Add(ToVector3d(v));
	}
	return hvl;
}
System::Collections::Generic::List<Vector3d>^ MIM::BaseTunnel::VerticalVectors::get()
{
	std::vector<AcGeVector3d> vvs;
	GetImpObj()->getVerticalVector(vvs);

	System::Collections::Generic::List<Vector3d>^ vvl =
		gcnew System::Collections::Generic::List<Vector3d>();

	for (auto& v : vvs)
	{
		vvl->Add(ToVector3d(v));
	}
	return vvl;
}


System::Collections::Generic::List<Point3d>^ MIM::BaseTunnel::GetAllVertices()
{
	AcGePoint3dArray vertexArray;
	GetImpObj()->getVertices3(vertexArray);
	System::Collections::Generic::List<Point3d>^ list = gcnew System::Collections::Generic::List<Point3d>();
	for (auto& vertex : vertexArray)
	{
		list->Add(ToPoint3d(vertex));
	}
	return list;
}

System::Collections::Generic::List<INT32>^ MIM::BaseTunnel::GetAllFaces()
{
	AcGeIntArray intArray;
	GetImpObj()->getFaces3(intArray);
	System::Collections::Generic::List<INT32>^ list = gcnew System::Collections::Generic::List<INT32>();
	for (auto i : intArray)
	{
		list->Add(i);
	}
	return list;
}

System::Collections::Generic::List<UINT32>^ MIM::BaseTunnel::GetVerticesColors()
{
	AcGeIntArray intArray;
	GetImpObj()->getVerticesColors(intArray);
	System::Collections::Generic::List<UINT32>^ list = gcnew System::Collections::Generic::List<UINT32>();
	for (auto i : intArray)
	{
		list->Add(i);
	}
	return list;
}

/*********************************************************/
