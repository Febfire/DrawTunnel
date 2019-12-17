#pragma once
using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;


namespace MIM
{
	[Autodesk::AutoCAD::Runtime::Wrapper("Tunnel_Cylinder")]
	public ref class CylinderTunnel :public MIM::BaseTunnel
	{
	public:
		CylinderTunnel();

	internal:
		CylinderTunnel(System::IntPtr unmanagedPointer, bool autoDelete);
		inline Tunnel_Cylinder*  GetImpObj()
		{
			return static_cast<Tunnel_Cylinder*>(UnmanagedObject.ToPointer());
		}
	public:
		property System::Collections::Generic::List<Point3d>^ BasePoints
		{
			void set(System::Collections::Generic::List<Point3d>^ points) override;
			System::Collections::Generic::List<Point3d>^ get() override;
		}

		property double Radius
		{
			void set(double radius);
			double get();
		}

	};

}
