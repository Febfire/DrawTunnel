#pragma once

using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;

namespace Autodesk
{
	namespace DefinedEnitity
	{

		[Autodesk::AutoCAD::Runtime::Wrapper("STANDARDLINE")]
		public ref class StandardLineWrapper :public Autodesk::AutoCAD::DatabaseServices::Entity
		{
		public:
			StandardLineWrapper();

		internal:
			StandardLineWrapper(System::IntPtr unmanagedPointer, bool autoDelete);
			inline StandardLine*  GetImpObj()
			{
				return static_cast<StandardLine*>(UnmanagedObject.ToPointer());
			}

		public:
			/*property Point3d StartPoint
			{
				void set(Point3d point);
				Point3d get();
			}

			property Point3d EndPoint
			{
				void set(Point3d point);
				Point3d get();
			}*/

			property Point3d InflectionPoint
			{
				void set(Point3d point);
				Point3d get();
			}
		};

	}
}
