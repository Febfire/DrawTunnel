#pragma once
using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;


namespace MIM
{
	[Autodesk::AutoCAD::Runtime::Wrapper("Tunnel_Square")]
	public ref class SquareTunnel :public MIM::BaseTunnel
	{
	public:
		SquareTunnel();

	internal:
		SquareTunnel(System::IntPtr unmanagedPointer, bool autoDelete);
		inline Tunnel_Square*  GetImpObj()
		{
			return static_cast<Tunnel_Square*>(UnmanagedObject.ToPointer());
		}
	public:
		property System::Collections::Generic::List<Point3d>^ BasePoints
		{
			void set(System::Collections::Generic::List<Point3d>^ points) override;
			System::Collections::Generic::List<Point3d>^ get() override;
		}

		property double Height
		{
			void set(double height);
			double get();
		}

		property double Width_t
		{
			void set(double width);
			double get();
		}

		property double Width_b
		{
			void set(double width);
			double get();
		}

	};

}
