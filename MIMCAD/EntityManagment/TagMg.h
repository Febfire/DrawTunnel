#pragma once

using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;

namespace MIM
{
	[Autodesk::AutoCAD::Runtime::Wrapper("TUNNELTAG")]
	public ref class Tag :public Autodesk::AutoCAD::DatabaseServices::Entity
	{
	public:
		Tag();

	internal:
		Tag(System::IntPtr unmanagedPointer, bool autoDelete);
		inline TunnelTag*  GetImpObj()
		{
			return static_cast<TunnelTag*>(UnmanagedObject.ToPointer());
		}

	public:
		property Point3d StartPoint
		{
			void set(Point3d point);
			Point3d get();
		}

		property Point3d EndPoint
		{
			void set(Point3d point);
			Point3d get();
		}

		property Point3d InflectionPoint
		{
			void set(Point3d point);
			Point3d get();
		}

		property String^ Text
		{
			void set(String^ text);
			String^ get();
		}
	};
}
