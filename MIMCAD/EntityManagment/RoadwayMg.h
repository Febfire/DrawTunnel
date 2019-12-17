#pragma once
using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;
using namespace MIM;

namespace Autodesk
{
	namespace DefinedEnitity
	{
		[Autodesk::AutoCAD::Runtime::Wrapper("ROADWAY")]
		public ref class RoadwayWrapper :public Autodesk::AutoCAD::DatabaseServices::Entity
		{
		public:
			RoadwayWrapper();

		internal:
			RoadwayWrapper(System::IntPtr unmanagedPointer, bool autoDelete);
			inline Roadway*  GetImpObj()
			{
				return static_cast<Roadway*>(UnmanagedObject.ToPointer());
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

			property String^ Name
			{
				void set(String^ name);
				String^ get();
			}

			property String^ ExtensionData
			{
				void set(String^ extensionData);
				String^ get();
			}

			property bool AnimateSwitch
			{
				void set(bool swtch);
				bool get();
			}

			property Autodesk::AutoCAD::DatabaseServices::Handle StartNodeHandle
			{
				void set(Autodesk::AutoCAD::DatabaseServices::Handle handle);
				Autodesk::AutoCAD::DatabaseServices::Handle get();
			}

			property Autodesk::AutoCAD::DatabaseServices::Handle EndNodeHandle
			{
				void set(Autodesk::AutoCAD::DatabaseServices::Handle handle);
				Autodesk::AutoCAD::DatabaseServices::Handle get();
			}
		public:

			static bool getIsNodifying()
			{
				return Roadway::getIsNodifying();
			}

			static System::Void endNodifying()
			{
				Roadway::endNodifying();
			}
		};

	}
}
