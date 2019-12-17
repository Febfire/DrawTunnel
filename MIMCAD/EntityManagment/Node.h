#pragma once
using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;


namespace MIM
{
	[Autodesk::AutoCAD::Runtime::Wrapper("TunnelNode")]
	public ref class Node :public Autodesk::AutoCAD::DatabaseServices::Entity
	{
	public:
		Node();

	internal:
		Node(System::IntPtr unmanagedPointer, bool autoDelete);
		inline TunnelNode*  GetImpObj()
		{
			return static_cast<TunnelNode*>(UnmanagedObject.ToPointer());
		}

	public:

		System::Void reflesh()
		{
			GetImpObj()->reflush();
		}

		System::Void AppendTunnel(Autodesk::AutoCAD::DatabaseServices::Handle, int);

		System::Void ChangeIndex(Autodesk::AutoCAD::DatabaseServices::Handle, int);

		System::Void RemoveTunnel(Autodesk::AutoCAD::DatabaseServices::Handle);

		System::Void TryErase();

		property Point3d Position
		{
			void set(Point3d point);
			Point3d get();
		}

		property size_t CountOfBind
		{
			size_t get();
		}

		property System::UInt32 NodeColor
		{
			void set(System::UInt32 color);
			System::UInt32 get();
		}

		property System::UInt32 Radius
		{
			void set(System::UInt32 radius);
			System::UInt32 get();
		}

		property String^ Name
		{
			void set(String^ name);
			String^ get();
		}

		property String^ Location
		{
			void set(String^ location);
			String^ get();
		}

	};

}

