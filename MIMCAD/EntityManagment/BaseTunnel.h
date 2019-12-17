#pragma once
using namespace System;
using namespace System::Collections::Generic;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;


namespace MIM
{
	[Autodesk::AutoCAD::Runtime::Wrapper("Tunnel_Base")]
	public ref class BaseTunnel :public Autodesk::AutoCAD::DatabaseServices::Curve
	{
	public:
		//Tunnel();

	internal:
		BaseTunnel(System::IntPtr unmanagedPointer, bool autoDelete);
		inline Tunnel_Base*  GetImpObj()
		{
			return static_cast<Tunnel_Base*>(UnmanagedObject.ToPointer());
		}

	public:
		
		property System::Collections::Generic::List<Point3d>^ BasePoints
		{
			virtual void set(System::Collections::Generic::List<Point3d>^ points) {}
			virtual System::Collections::Generic::List<Point3d>^ get() { return nullptr; }
		}

		property System::Collections::Generic::List<Autodesk::AutoCAD::DatabaseServices::Handle>^ NodesHandle
		{
			void set(System::Collections::Generic::List<Autodesk::AutoCAD::DatabaseServices::Handle>^ points);
			System::Collections::Generic::List<Autodesk::AutoCAD::DatabaseServices::Handle>^ get();
		}

		property System::Collections::Generic::List<UINT32>^ Colors
		{
			void set(System::Collections::Generic::List<UINT32>^ points);
			System::Collections::Generic::List<UINT32>^ get();
		}

		property Autodesk::AutoCAD::DatabaseServices::Handle TagHandle
		{
			void set(Autodesk::AutoCAD::DatabaseServices::Handle handle);
			Autodesk::AutoCAD::DatabaseServices::Handle get();
		}

		property String^ TunnelType
		{
			void set(String^ type);
			String^ get();
		}

		property String^ Name
		{
			void set(String^ name);
			String^ get();
		}

		property String^ TagData
		{
			void set(String^ tagData);
			String^ get();
		}


		property String^ Location
		{
			void set(String^ location);
			String^ get();
		}

		property bool DisplayTag
		{
			void set(bool display);
			bool get();
		}

		property bool DisplayNodes
		{
			void set(bool display);
			bool get();
		}

		property System::Int32 Segment
		{
			void set(System::Int32 segment);
			System::Int32 get();
		}

		property System::Collections::Generic::List<INT16>^ Temperatures
		{
			void set(System::Collections::Generic::List<INT16>^ temperatures);
			System::Collections::Generic::List<INT16>^ get();
		}


		property System::Collections::Generic::List<Vector3d>^ CenterVectors
		{
			System::Collections::Generic::List<Vector3d>^ get();		
		}

		property System::Collections::Generic::List<Vector3d>^ HorizontalVectors
		{
			System::Collections::Generic::List<Vector3d>^ get();
		}

		property System::Collections::Generic::List<Vector3d>^ VerticalVectors
		{
			System::Collections::Generic::List<Vector3d>^ get();
		}


		System::Void ChangeVertice(Point3d point,int index)
		{
			GetImpObj()->changeVertice(GETPOINT3D(point), index);
		}

		System::Void Reflesh()
		{
			GetImpObj()->reflush();
		}

		System::Void SetClose(bool close)
		{
			GetImpObj()->setClose(close);
		}


		System::Collections::Generic::List<Point3d>^ GetAllVertices();
		System::Collections::Generic::List<INT32>^ GetAllFaces();
		System::Collections::Generic::List<UINT32>^ GetVerticesColors();


		////////////////////////////////////////////////////////////////////////////////////////////////
		static bool getIsNodifying()
		{
			return Tunnel_Base::getIsNodifying();
		}

		static System::Void endNodifying()
		{
			Tunnel_Base::endNodifying();
		}

		static bool getIsAnimateMode()
		{
			return Tunnel_Base::getAnimateMode();
		}

		static System::Void startAnimateMode()
		{
			Tunnel_Base::startAnimateMode();
		}

		static System::Void endAnimateMode()
		{
			Tunnel_Base::endAnimateMode();
		}

		static System::UInt32 getDisplayMode()
		{
			return Tunnel_Base::getDisplayMode();
		}

		static System::Void setDisplayMode(UINT16 mode)
		{
			Tunnel_Base::setDisplayMode(mode);
		}
	};

}

