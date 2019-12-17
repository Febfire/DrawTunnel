#pragma once
using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;

namespace Autodesk
{
	namespace DefinedEnitity
	{
		using namespace MIM;

		ref class RoadwayWrapper;

		[Autodesk::AutoCAD::Runtime::Wrapper("ROADWAYNODE")]
		public ref class RoadwayNodeWrapper :public Autodesk::AutoCAD::DatabaseServices::Entity
		{
		public:
			RoadwayNodeWrapper();

		internal:
			RoadwayNodeWrapper(System::IntPtr unmanagedPointer, bool autoDelete);
			inline RoadwayNode*  GetImpObj()
			{
				return static_cast<RoadwayNode*>(UnmanagedObject.ToPointer());
			}

		public:

			System::Void appendRoadway(Autodesk::AutoCAD::DatabaseServices::Handle, int);

			property Point3d Position
			{
				void set(Point3d point);
				Point3d get();
			}

			property size_t CountOfBound
			{
				size_t get();
			}
		};

	}
}
