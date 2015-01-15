// Copyright (C) 2013-2015 MetaMorph Software, Inc

// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this data, including any software or models in source or binary
// form, as well as any drawings, specifications, and documentation
// (collectively "the Data"), to deal in the Data without restriction,
// including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Data, and to
// permit persons to whom the Data is furnished to do so, subject to the
// following conditions:

// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Data.

// THE DATA IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS, SPONSORS, DEVELOPERS, CONTRIBUTORS, OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE DATA OR THE USE OR OTHER DEALINGS IN THE DATA.  

// =======================
// This version of the META tools is a fork of an original version produced
// by Vanderbilt University's Institute for Software Integrated Systems (ISIS).
// Their license statement:

// Copyright (C) 2011-2014 Vanderbilt University

// Developed with the sponsorship of the Defense Advanced Research Projects
// Agency (DARPA) and delivered to the U.S. Government with Unlimited Rights
// as defined in DFARS 252.227-7013.

// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this data, including any software or models in source or binary
// form, as well as any drawings, specifications, and documentation
// (collectively "the Data"), to deal in the Data without restriction,
// including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Data, and to
// permit persons to whom the Data is furnished to do so, subject to the
// following conditions:

// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Data.

// THE DATA IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS, SPONSORS, DEVELOPERS, CONTRIBUTORS, OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE DATA OR THE USE OR OTHER DEALINGS IN THE DATA.  

#pragma once


#include "ComHelp.h"
#include "GMECOM.h"
#include "Exceptions.h"
#include "MgaUtil.h"
#include "Core.h"
#include "Mga.h"

#include "ComponentConfig.h"

#if defined(BUILDER_OBJECT_NETWORK)
#else
#ifdef BUILDER_OBJECT_NETWORK_V2
#include "BON.h"
#include <BON2Component.h>
#else
#include <RawComponent.h>
#endif // BUILDER_OBJECT_NETWORK_V2
#endif // BUILDER_OBJECT_NETWORK

#pragma once

class CComponentObj;

#ifdef GME_ADDON

#ifndef RAWCOMPONENT_H
// BY PAKA BEGIN
#ifndef BON2Component_h
#error GME AddOn-s must be built with the RAW Component interface or BON2 Component Interface
#endif // BON2Component_h
// BY PAKA END
#endif // RAWCOMPONENT_H

class CEventSink : public CCmdTarget {
	DECLARE_DYNCREATE(CEventSink)
	CEventSink();           // protected constructor used by dynamic creation
public:
	CComponentObj *comp;

	IMgaEventSink* GetInterface() { return &m_xComponent; }

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CEventSink)
	public:
	virtual void OnFinalRelease();
	//}}AFX_VIRTUAL

// Implementation

public:
	virtual ~CEventSink();

	DECLARE_MESSAGE_MAP()
	DECLARE_OLECREATE(CEventSink)

	// Generated OLE dispatch map functions
	//{{AFX_DISPATCH(CEventSink)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_DISPATCH
	DECLARE_DISPATCH_MAP()

	DECLARE_INTERFACE_MAP()
public:
	BEGIN_INTERFACE_PART(Component, IMgaEventSink)
		STDMETHODIMP GlobalEvent(globalevent_enum event);
		STDMETHODIMP ObjectEvent(IMgaObject * obj, unsigned long eventmask, VARIANT v);
	END_INTERFACE_PART(Component)
};

#endif // GME_ADDON


/////////////////////////////////////////////////////////////////////////////
// CComponentObj command target

class __declspec(uuid(COCLASS_UUID)) CComponentObj : public CCmdTarget
{

	DECLARE_DYNCREATE(CComponentObj)

	CComponentObj();           // protected constructor used by dynamic creation
	void RegisterActiveObject();
	unsigned long registeractiveobjectret;

// Attributes
public:

// Operations
public:
	IMgaComponentEx* GetInterface() { return &m_xComponent; }

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CComponentObj)
	public:
	virtual void OnFinalRelease();
	//}}AFX_VIRTUAL

// Implementation
protected:
	virtual ~CComponentObj();
	void UnregisterActiveObject();

	// Generated message map functions
	//{{AFX_MSG(CComponentObj)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
	DECLARE_OLECREATE(CComponentObj)

	// Generated OLE dispatch map functions
	//{{AFX_DISPATCH(CComponentObj)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_DISPATCH
	DECLARE_DISPATCH_MAP()

	DECLARE_INTERFACE_MAP()
	BEGIN_INTERFACE_PART(Component, IMgaComponentEx)
		STDMETHODIMP InvokeEx( IMgaProject *project,  IMgaFCO *currentobj,  IMgaFCOs *selectedobjs, long param);
		STDMETHODIMP ObjectsInvokeEx( IMgaProject *project,  IMgaObject *currentobj,  IMgaObjects *selectedobjs,  long param);
		STDMETHODIMP Invoke(IMgaProject* gme, IMgaFCOs *models, long param);
		STDMETHODIMP Initialize(struct IMgaProject *);
		STDMETHODIMP Enable(VARIANT_BOOL newVal);
        STDMETHODIMP get_InteractiveMode(VARIANT_BOOL *enabled);

        STDMETHODIMP get_ComponentParameter(BSTR name, VARIANT *pVal);
        STDMETHODIMP put_ComponentParameter(BSTR name, VARIANT newVal);


        STDMETHODIMP put_InteractiveMode(VARIANT_BOOL enabled);
        STDMETHODIMP get_ComponentType( componenttype_enum *t);
        STDMETHODIMP get_ComponentProgID(BSTR *pVal) {
			*pVal = CComBSTR(COCLASS_PROGID).Detach();
			return S_OK;
		};
        STDMETHODIMP get_ComponentName(BSTR *pVal) {
			*pVal = CComBSTR(COMPONENT_NAME).Detach();
			return S_OK;
		};
        STDMETHODIMP get_Paradigm( BSTR *pVal) {
#ifdef PARADIGM_INDEPENDENT
			*pVal = CComBSTR("*").Detach();
#else
			*pVal = CComBSTR(PARADIGMS).Detach();
#endif // PARADIGM_INDEPENDENT
			return S_OK;
		};
	END_INTERFACE_PART(Component)

	BEGIN_INTERFACE_PART(VersionInfo, IGMEVersionInfo)
		STDMETHODIMP get_version(enum GMEInterfaceVersion *pVal);
	END_INTERFACE_PART(VersionInfo)

	BEGIN_INTERFACE_PART(SupportErrorInfo, ISupportErrorInfo)
		STDMETHODIMP InterfaceSupportsErrorInfo(REFIID riid)
		{
			if (riid == __uuidof(IMgaComponentEx) || riid == __uuidof(IMgaComponent))
			{
				return S_OK;
			}
			return S_FALSE;
		}
	END_INTERFACE_PART(SupportErrorInfo)
public:
	bool interactive;

#ifdef BUILDER_OBJECT_NETWORK
	typedef CMap<CString, LPCTSTR, CString, LPCTSTR> CStringMap;
	CStringMap parmap;
#endif

#ifdef RAWCOMPONENT_H
	RawComponent rawcomp;

#ifdef GME_ADDON
	CComPtr<IMgaEventSink> e_sink;
#endif // GME_ADDON

#endif // RAWCOMPONENT_H

#ifdef BUILDER_OBJECT_NETWORK_V2
	BON::Component 				bon2Comp;

#ifdef GME_ADDON
	CComPtr<IMgaAddOn> 		addon;
	CComPtr<IMgaEventSink> 	e_sink;
#endif // GME_ADDON

#endif // BUILDER_OBJECT_NETWORK_V2

	void HandleError( Util::Exception* pEx );
}; // CComponentObj


#ifndef __AFXPRIV_H__

class CPushRoutingFrame
{
protected:
	CFrameWnd* pOldRoutingFrame;
	_AFX_THREAD_STATE* pThreadState;

public:
	CPushRoutingFrame(CFrameWnd* pNewRoutingFrame)
	{
		pThreadState = AfxGetThreadState();
		pOldRoutingFrame = pThreadState->m_pRoutingFrame;
		pThreadState->m_pRoutingFrame = pNewRoutingFrame;
	}
	~CPushRoutingFrame()
	{ pThreadState->m_pRoutingFrame = pOldRoutingFrame; }
};

#endif // __AFXPRIV_H__

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.