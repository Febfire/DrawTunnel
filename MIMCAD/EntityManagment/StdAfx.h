//-----------------------------------------------------------------------------
//- StdAfx.h : include file for standard system include files,
//-      or project specific include files that are used frequently,
//-      but are changed infrequently
//-----------------------------------------------------------------------------
#pragma once
#define ENTITYMANAGMENT_MODULE


//-----------------------------------------------------------------------------
//- 'DEBUG workaround' below prevents the MFC or ATL #include-s from pulling 
//- in "Afx.h" that would force the debug CRT through #pragma-s.
#if defined(_DEBUG) && !defined(AC_FULL_DEBUG)
#define _DEBUG_WAS_DEFINED
#undef _DEBUG
#pragma message ("     Compiling MFC / STL / ATL header files in release mode.")
#endif

#pragma pack (push, 8)
#pragma warning(disable: 4786 4996)
//#pragma warning(disable: 4098)

//-----------------------------------------------------------------------------
#include <windows.h>

//- ObjectARX and OMF headers needs this

//-----------------------------------------------------------------------------
#using <mscorlib.dll>
#using <System.dll>

#using <acdbmgd.dll>
#using <acmgd.dll>
#using <AcCui.dll>

#include <vcclr.h>
#include <vector>
//-----------------------------------------------------------------------------
//- Include ObjectDBX/ObjectARX headers
//- Uncomment one of the following lines to bring a given library in your project.
//#define _BREP_SUPPORT_					//- Support for the BRep API
//#define _HLR_SUPPORT_						//- Support for the Hidden Line Removal API
//#define _AMODELER_SUPPORT_				//- Support for the AModeler API
//#define _ASE_SUPPORT_							//- Support for the ASI/ASE API
//#define _RENDER_SUPPORT_					//- Support for the AutoCAD Render API
//#define _ARX_CUSTOM_DRAG_N_DROP_	//- Support for the ObjectARX Drag'n Drop API
//#define _INC_LEAGACY_HEADERS_			//- Include legacy headers in this project
#include "arxHeaders.h"


//-----------------------------------------------------------------------------
#include "../CustomEntities/Tunnel_Base.h"
#include "../CustomEntities/Tunnel_Square.h"
#include "../CustomEntities/Tunnel_Cylinder.h"
#include "../CustomEntities/TunnelNode.h"
#include "../CustomEntities/TunnelTag.h"
#include "../CustomEntities/DwgMark.h"

#pragma pack (pop)

//-----------------------------------------------------------------------------
#ifdef _DEBUG_WAS_DEFINED
#define _DEBUG
#undef _DEBUG_WAS_DEFINED
#endif
