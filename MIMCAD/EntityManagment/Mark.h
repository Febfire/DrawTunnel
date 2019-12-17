#pragma once
using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;


namespace MIM
{
	[Autodesk::AutoCAD::Runtime::Wrapper("DwgMark")]
	public ref class Mark :public Autodesk::AutoCAD::DatabaseServices::Entity
	{
	public:
		Mark();

	internal:
		Mark(System::IntPtr unmanagedPointer, bool autoDelete);
		inline DwgMark*  GetImpObj()
		{
			return static_cast<DwgMark*>(UnmanagedObject.ToPointer());
		}

	public:

		property String^ Uuid
		{
			void set(String^ str);
			String^ get();
		}

	};

}

