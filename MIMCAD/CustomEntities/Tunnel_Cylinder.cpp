#include "StdAfx.h"
#include "Tunnel_Base.h"
#include "Tunnel_Cylinder.h"
#include "TunnelNode.h"

using namespace MIM;

ACRX_DXF_DEFINE_MEMBERS(Tunnel_Cylinder, Tunnel_Base, AcDb::kDHL_CURRENT,
	AcDb::kMReleaseCurrent, 0, TUNNEL_CYLINDER, /*MSG0*/"KLND");

Tunnel_Cylinder::Tunnel_Cylinder() :m_radius(10), m_steps(50)
{
}


Tunnel_Cylinder::~Tunnel_Cylinder() {}


Acad::ErrorStatus
Tunnel_Cylinder::dwgOutFields(AcDbDwgFiler *pFiler) const
{
	assertReadEnabled();
	Acad::ErrorStatus es = Tunnel_Base::dwgOutFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version = TUNNEL_VERSION;
	pFiler->writeItem(version);

	pFiler->writeDouble(m_radius);
	pFiler->writeUInt16(m_steps);

	return pFiler->filerStatus();
}

//----------------------------------------------------------------------------------------------------------------//

Acad::ErrorStatus
Tunnel_Cylinder::dwgInFields(AcDbDwgFiler *pFiler)
{
	assertWriteEnabled();
	Acad::ErrorStatus es = Tunnel_Base::dwgInFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version;

	pFiler->readItem(&version);
	if (version > TUNNEL_VERSION)
		return Acad::eMakeMeProxy;

	pFiler->readDouble(&m_radius);

	pFiler->readUInt16(&m_steps);

	return pFiler->filerStatus();
}


Acad::ErrorStatus Tunnel_Cylinder::subDeepClone(
	AcDbObject* pOwnerObject,
	AcDbObject*& pClonedObject,
	AcDbIdMapping& idMap,
	Adesk::Boolean isPrimary
) const
{
	// You should always pass back pClonedObject == NULL
	// if, for any reason, you do not actually clone it
	// during this call.  The caller should pass it in
	// as NULL, but to be safe, we set it here as well.
	//
	pClonedObject = NULL;

	// If this object is in the idMap and is already
	// cloned, then return.
	//
	bool isPrim = false;
	if (isPrimary)
		isPrim = true;
	AcDbIdPair idPair(objectId(), (AcDbObjectId)NULL,
		false, isPrim);
	if (idMap.compute(idPair) && (idPair.value() != NULL))
		return Acad::eOk;

	// Create the clone
	//
	Tunnel_Cylinder *pClone = (Tunnel_Cylinder*)isA()->create();
	if (pClone != NULL)
		pClonedObject = pClone;    // set the return value
	else
		return Acad::eOutOfMemory;

	AcDbDeepCloneFiler filer;
	dwgOut(&filer);

	filer.seek(0L, AcDb::kSeekFromStart);
	pClone->dwgIn(&filer);
	bool bOwnerXlated = false;

	if (isPrimary)
	{
		AcDbBlockTableRecord *pBTR =
			AcDbBlockTableRecord::cast(pOwnerObject);
		if (pBTR != NULL)
		{
			pBTR->appendAcDbEntity(pClone);
			bOwnerXlated = true;
		}
		else
		{
			pOwnerObject->database()->addAcDbObject(pClone);
		}
	}
	else {
		pOwnerObject->database()->addAcDbObject(pClone);
		pClone->setOwnerId(pOwnerObject->objectId());
		bOwnerXlated = true;
	}

	// This must be called for all newly created objects
	// in deepClone.  It is turned off by endDeepClone()
	// after it has translated the references to their
	// new values.
	//
	pClone->setAcDbObjectIdsInFlux();
	pClone->disableUndoRecording(true);

	// Add the new information to the idMap.  We can use
	// the idPair started above.
	//
	idPair.setValue(pClonedObject->objectId());
	idPair.setIsCloned(Adesk::kTrue);
	idPair.setIsOwnerXlated(bOwnerXlated);

	Tunnel_Cylinder* pOwner = static_cast<Tunnel_Cylinder*>(pClonedObject);
	AcDbHandle handle;
	pOwner->getAcDbHandle(handle);

	AcDbBlockTableRecord *pBTR =
		AcDbBlockTableRecord::cast(pOwnerObject);
	std::vector<AcDbHandle> nodesHandle;
	int index = 0;
	for (auto& point : pOwner->m_geo_basePoints)
	{
		TunnelNode* pNode = new TunnelNode;
		pNode->setPosition(point);
		pNode->setName(L"节点");
		pNode->setLocation(pOwner->getLocation());
		pNode->appendTunnel(handle, index);
		pBTR->appendAcDbEntity(pNode);
		AcDbHandle nodeHandle;
		pNode->getAcDbHandle(nodeHandle);
		nodesHandle.emplace_back(nodeHandle);
		pNode->close();
		index++;
	}
	pOwner->setNodesHandle(nodesHandle);

	idMap.assign(idPair);

	AcDbObjectId id;

	while (filer.getNextOwnedObject(id)) {

		AcDbObject *pSubObject;
		AcDbObject *pClonedSubObject;

		// Some object's references may be set to NULL, 
		// so don't try to clone them.
		//
		if (id == NULL)
			continue;

		// Open the object and clone it.  Note that we now
		// set "isPrimary" to kFalse here because the object
		// is being cloned, not as part of the primary set,
		// but because it is owned by something in the
		// primary set.
		//
		acdbOpenAcDbObject(pSubObject, id, AcDb::kForRead);
		pClonedSubObject = NULL;
		pSubObject->deepClone(pClonedObject,
			pClonedSubObject,
			idMap, Adesk::kFalse);

		// If this is a kDcInsert context, the objects
		// may be "cheapCloned".  In this case, they are
		// "moved" instead of cloned.  The result is that
		// pSubObject and pClonedSubObject will point to
		// the same object.  So, we only want to close
		// pSubObject if it really is a different object
		// than its clone.
		//
		if (pSubObject != pClonedSubObject)
			pSubObject->close();

		// The pSubObject may either already have been
		// cloned, or for some reason has chosen not to be
		// cloned.  In that case, the returned pointer will
		// be NULL.  Otherwise, since we have no immediate
		// use for it now, we can close the clone.
		//
		if (pClonedSubObject != NULL)
			pClonedSubObject->close();
	}

	// Leave pClonedObject open for the caller
	//
	return Acad::eOk;
}


void Tunnel_Cylinder::setVetices(std::vector<AcGePoint3d>& vertices3, std::vector<AcGePoint3d>& vertices2)
{
	//段数
	UINT32 segmentCount = getDrawableSegments();

	//巷道中心线向量
	std::vector<AcGeVector3d> centerVectors;
	getCenterVector(centerVectors);

	//巷道截面的“宽”方向向量
	std::vector<AcGeVector3d>  horizontalVector;
	getHorizontalVector(horizontalVector);

	for (uint j = 0; j < segmentCount; j++)
	{
		//断面形状，最初的截面
		AcGePoint3dArray pts;
		for (int i = 0; i < m_steps; i++)
		{
			AcGePoint3d pt = AcGePoint3d(m_radius*cos(2 * PI*i / m_steps), m_radius*sin(2 * PI*i / m_steps), 0);
			pts.append(std::move(pt));
		}

		//巷道方向到Z方向角度 90度
		double angleToZ = AcGeVector3d::kZAxis.angleTo(centerVectors.at(j));
		for (int i = 0; i < pts.length(); i++)
		{
			pts.at(i).rotateBy(angleToZ, horizontalVector.at(j));
		}
		double angleToCenter = (-AcGeVector3d::kYAxis).angleTo(centerVectors.at(j), AcGeVector3d::kZAxis);
		//点的旋转
		for (int i = 0; i < pts.length(); i++)
		{
			pts.at(i).rotateBy(angleToCenter, centerVectors.at(j));
		}

		//点的平移
		for (int i = 0; i < pts.length(); i++)
		{
			pts.at(i) += (m_drawable_basePoints.at(j) - AcGePoint3d(0, 0, 0));
		}

		//这个几何分段下包含的颜色分段数
		int sg = m_turningPointIndexes.at(j + 1) - m_turningPointIndexes.at(j);
		for (int i = 0; i < sg + 1; i++)
		{
			//每个面偏移的长度
			double length =
				(m_color_basePoints.at(m_turningPointIndexes.at(j) + i) - m_color_basePoints.at(m_turningPointIndexes.at(j))).length();

			for (int k = 0; k < m_steps; k++)
			{
				AcGePoint3d p = pts.at(k) + centerVectors.at(j).normal()*length;
				vertices3.emplace_back(p);
			}

		}

		//设置双线模式要显示的点
		auto nhv = horizontalVector.at(j).normalize();
		vertices2.push_back(m_drawable_basePoints.at(j) + nhv*m_radius);
		vertices2.push_back(vertices2.at(5 * j) + centerVectors.at(j));
		vertices2.push_back(vertices2.at(5 * j + 1) - nhv*m_radius * 2);
		vertices2.push_back(vertices2.at(5 * j + 2) - centerVectors.at(j));
		vertices2.push_back(m_drawable_basePoints.at(j) + nhv*m_radius);
	}	
}

void Tunnel_Cylinder::setFaceList(std::vector<int>& faceList)
{
	//段数
	UINT32 segmentCount = getDrawableSegments();

	for (uint32_t i = 0; i < segmentCount; i++)
	{
		for (UINT16 j = m_turningPointIndexes.at(i); j < m_turningPointIndexes.at(i + 1); j++)
		{
			for (int k = 0; k < m_steps; k++)
			{
				if (k < m_steps - 1)
				{
					faceList.push_back(4);
					faceList.push_back((i + j)* m_steps + k);
					faceList.push_back((i + j)* m_steps + k + 1);
					faceList.push_back((i + j)* m_steps + k + m_steps + 1);
					faceList.push_back((i + j)* m_steps + k + m_steps);
				}
				else
				{
					faceList.push_back(4);
					faceList.push_back((i + j)* m_steps + k);
					faceList.push_back((i + j)* m_steps + 0);
					faceList.push_back((i + j)* m_steps + k + 1);
					faceList.push_back((i + j)* m_steps + k + m_steps);

				}
			}
		}
	}
}

void Tunnel_Cylinder::setVerticesColorList(std::vector<AcCmEntityColor>& colorList)
{
	std::vector<uint32_t> allColor(m_colors);
	setAllColorArray(allColor);
	int segmentCount = getDrawableSegments();
	for (int j = 0; j < segmentCount; j++)
	{
		//这个几何分段下包含的颜色分段数
		int sg = m_turningPointIndexes.at(j + 1) - m_turningPointIndexes.at(j);
		for (int i = 0; i < sg + 1; i++)
		{
			for (int k = 0; k < m_steps; k++)
			{
				uint8_t r, g, b;
				UInt32ToRGB(allColor.at(m_turningPointIndexes.at(j) + i), r, g, b);

				AcCmEntityColor c;
				c.setRGB(r, g, b);
				colorList.emplace_back(c);

			}
		}
	}
}
Acad::ErrorStatus Tunnel_Cylinder::setRadius(double radius)
{
	assertWriteEnabled();
	m_radius = radius;
	return Acad::eOk;
}

const double Tunnel_Cylinder::getRadius() const
{
	return m_radius;
}

const double Tunnel_Cylinder::getHeight() const
{
	return m_radius;
}

const double Tunnel_Cylinder::getWidth() const
{
	return m_radius;
}

void Tunnel_Cylinder::createDerive(Tunnel_Base*& newTunnel) const
{
	newTunnel = new Tunnel_Cylinder();
}

