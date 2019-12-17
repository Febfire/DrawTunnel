#pragma once
#define CUSTOMENTITIES_MODULE

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


//-----------------------------------------------------------------------------
#include <windows.h>
#include <tchar.h>
//- ObjectARX and OMF headers needs this

#include <vector>
//-----------------------------------------------------------------------------
//- Include ObjectDBX/ObjectARX headers
//- Uncomment one of the following lines to bring a given library in your project.
//#define _BREP_SUPPORT_					//- Support for the BRep API
//#define _HLR_SUPPORT_						//- Support for the Hidden Line Removal API
//#define _AMODELER_SUPPORT_				//- Support for the AModeler API

#include <dbxHeaders.h>
#include <arxHeaders.h>
#include <dbSubD.h>

#pragma pack (pop) 


//-----------------------------------------------------------------------------
#ifdef _DEBUG_WAS_DEFINED
#define _DEBUG
#undef _DEBUG_WAS_DEFINED
#endif

#ifdef _DEBUG
#define MIMDEBUGASSERT assert
#else
#define MIMDEBUGASSERT(b) 

#endif // _DEBUG