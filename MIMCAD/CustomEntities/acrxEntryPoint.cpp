// (C) Copyright 2002-2012 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

//-----------------------------------------------------------------------------
//----- acrxEntryPoint.cpp
//-----------------------------------------------------------------------------
#include "StdAfx.h"
#include "resource.h"
#include "TunnelTag.h"
#include "Tunnel_Base.h"
#include "Tunnel_Square.h"
#include "Tunnel_Cylinder.h"
#include "TunnelNode.h"
#include "DwgMark.h"
//-----------------------------------------------------------------------------
#define szRDS _RXST("KL")

//-----------------------------------------------------------------------------
//----- ObjectARX EntryPoint
class CCustomEntitiesApp : public AcRxDbxApp {

public:
	CCustomEntitiesApp () : AcRxDbxApp () {}

	virtual AcRx::AppRetCode On_kInitAppMsg (void *pkt) {
		// TODO: Load dependencies here

		// You *must* call On_kInitAppMsg here
		AcRx::AppRetCode retCode =AcRxDbxApp::On_kInitAppMsg (pkt) ;
		
		// TODO: Add your initialization code here
		MIM::TunnelTag::rxInit();
		MIM::Tunnel_Base::rxInit();
		MIM::Tunnel_Square::rxInit();
		MIM::Tunnel_Cylinder::rxInit();
		MIM::TunnelNode::rxInit();
		MIM::DwgMark::rxInit();
		

		// Register a service using the class name.

		if (!acrxServiceIsRegistered(TEXT("TUNNEL_BASE")))
			acrxRegisterService(TEXT("TUNNEL_BASE"));

		if (!acrxServiceIsRegistered(TEXT("TUNNEL_SQUARE")))
			acrxRegisterService(TEXT("TUNNEL_SQUARE"));

		if (!acrxServiceIsRegistered(TEXT("TUNNEL_CYLINDER")))
			acrxRegisterService(TEXT("TUNNEL_CYLINDER"));

		if (!acrxServiceIsRegistered(TEXT("TUNNELNODE")))
			acrxRegisterService(TEXT("TUNNELNODE"));

		if (!acrxServiceIsRegistered(TEXT("TUNNELTAG")))
			acrxRegisterService(TEXT("TUNNELTAG"));

		if (!acrxServiceIsRegistered(TEXT("DWGMARK")))
			acrxRegisterService(TEXT("DWGMARK"));

		acrxBuildClassHierarchy();
		return (retCode) ;
	}

	virtual AcRx::AppRetCode On_kUnloadAppMsg (void *pkt) {
		// TODO: Add your code here

		// You *must* call On_kUnloadAppMsg here
		AcRx::AppRetCode retCode =AcRxDbxApp::On_kUnloadAppMsg (pkt) ;

		// TODO: Unload dependencies here

		AcRxObject *obj1 = acrxServiceDictionary->remove(_T("TUNNEL_BASE"));
		if (obj1 != NULL)
			delete obj1;
		deleteAcRxClass(MIM::Tunnel_Base::desc());

		AcRxObject *obj2 = acrxServiceDictionary->remove(_T("TUNNEL_SQUARE"));
		if (obj2 != NULL)
			delete obj2;
		deleteAcRxClass(MIM::Tunnel_Square::desc());

		AcRxObject *obj3 = acrxServiceDictionary->remove(_T("TUNNEL_CYLINDER"));
		if (obj3 != NULL)
			delete obj3;
		deleteAcRxClass(MIM::Tunnel_Cylinder::desc());

		AcRxObject *obj4 = acrxServiceDictionary->remove(_T("TUNNELNODE"));
		if (obj4 != NULL)
			delete obj4;
		deleteAcRxClass(MIM::TunnelNode::desc());

		AcRxObject *obj5 = acrxServiceDictionary->remove(_T("TUNNELTAG"));
		if (obj5 != NULL)
			delete obj5;
		deleteAcRxClass(MIM::TunnelTag::desc());

		AcRxObject *obj6 = acrxServiceDictionary->remove(_T("DWGMARK"));
		if (obj6 != NULL)
			delete obj6;
		deleteAcRxClass(MIM::DwgMark::desc());
		
		acrxBuildClassHierarchy();
		return (retCode) ;
	}

	virtual void RegisterServerComponents () {
	}
	
} ;

//-----------------------------------------------------------------------------
IMPLEMENT_ARX_ENTRYPOINT(CCustomEntitiesApp)

