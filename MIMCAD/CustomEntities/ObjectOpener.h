#pragma once

#ifndef OPENOBJECT_H
#define OPENOBJECT_H

#include "stdafx.h"
//对打开acdbOpenObject的RAII封装
template<typename T>
class ObjectOpener
{
private:
	enum ByWhat
	{
		ByHandle,
		ByObjectId
	};

	T*& m_obj;
	const AcDbHandle m_handle;
	const AcDbObjectId m_id;
	OpenMode m_mode;
	bool m_openErased;
	ByWhat m_byWhat;
public:
	ObjectOpener() = delete;
	//通过AcdbObjectHandle打开
	ObjectOpener(T*& outObj, const AcDbHandle& handle, OpenMode mode, bool openErased = false);
	//通过AcdbObjectId打开
	ObjectOpener(T*& outObj, const AcDbObjectId& id, OpenMode mode, bool openErased = false);

	~ObjectOpener();

	Acad::ErrorStatus  open();

	void setOpenMode(OpenMode mode);

};

template<typename T>
ObjectOpener<T>::ObjectOpener(T*& outObj, const AcDbHandle& handle, OpenMode mode, bool openErased = false) :
	m_obj(outObj),
	m_handle(handle),
	m_mode(mode),
	m_openErased(openErased),
	m_byWhat(ByWhat::ByHandle)
{
}

template<typename T>
ObjectOpener<T>::ObjectOpener(T*& outObj, const AcDbObjectId& id, OpenMode mode, bool openErased = false) :
	m_obj(outObj),
	m_id(id),
	m_mode(mode),
	m_openErased(openErased),
	m_byWhat(ByWhat::ByObjectId)
{

}

template<typename T>
ObjectOpener<T>::~ObjectOpener()
{
	if (m_obj != nullptr)
		m_obj->close();
}

template<typename T>
Acad::ErrorStatus  ObjectOpener<T>::open()
{
	Acad::ErrorStatus es = Acad::eOk;

	AcDbObjectId objectID = nullptr;

	if (m_byWhat == ByWhat::ByHandle)
	{
		es = acdbHostApplicationServices()->workingDatabase()
			->getAcDbObjectId(objectID, false, m_handle, 0);
		if (es != Acad::eOk)
		{
			//MIMDEBUGASSERT(false);
			return es;
		}

		es = acdbOpenObject(m_obj, objectID, m_mode, m_openErased);
	}
	else if (m_byWhat == ByWhat::ByObjectId)
	{
		if (m_id.isNull())
		{
			es = Acad::eNullObjectId;
			MIMDEBUGASSERT(!m_id.isNull());
			return es;

		}
		es = acdbOpenObject(m_obj, m_id, m_mode, m_openErased);
	}

	return es;
}

template<typename T>
void  ObjectOpener<T>::setOpenMode(OpenMode mode)
{
	m_mode = mode;
}
#endif // !OPENOBJECT_H
