#include "StdAfx.h"
#include "Tunnel_Base.h"
#include "Tunnel_Square.h"
#include "TunnelNode.h"
#include <iterator>
using namespace MIM;

ACRX_DXF_DEFINE_MEMBERS(Tunnel_Square, Tunnel_Base, AcDb::kDHL_CURRENT,
	AcDb::kMReleaseCurrent, 0, TUNNEL_SQUARE, /*MSG0*/"KLND");

Tunnel_Square::Tunnel_Square() :m_height(10), m_width_t(10), m_width_b(10)
{
}


Tunnel_Square::~Tunnel_Square() {}


Acad::ErrorStatus
Tunnel_Square::dwgOutFields(AcDbDwgFiler *pFiler) const
{
	assertReadEnabled();
	Acad::ErrorStatus es = Tunnel_Base::dwgOutFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version = TUNNEL_VERSION;
	pFiler->writeItem(version);

	pFiler->writeDouble(m_height);

	pFiler->writeDouble(m_width_t);

	pFiler->writeDouble(m_width_b);

	return pFiler->filerStatus();
}

//----------------------------------------------------------------------------------------------------------------//

Acad::ErrorStatus
Tunnel_Square::dwgInFields(AcDbDwgFiler *pFiler)
{
	assertWriteEnabled();
	Acad::ErrorStatus es = Tunnel_Base::dwgInFields(pFiler);
	if (es != Acad::eOk)
		return (es);

	Adesk::Int16 version;

	pFiler->readItem(&version);
	if (version > TUNNEL_VERSION)
		return Acad::eMakeMeProxy;

	pFiler->readDouble(&m_height);

	pFiler->readDouble(&m_width_t);

	pFiler->readDouble(&m_width_b);

	return pFiler->filerStatus();
}



Acad::ErrorStatus Tunnel_Square::subDeepClone(
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
	Tunnel_Square *pClone = (Tunnel_Square*)isA()->create();
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

	Tunnel_Square* pOwner = static_cast<Tunnel_Square*>(pClonedObject);
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
		pNode->appendTunnel(handle,index);
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


void Tunnel_Square::setVetices(std::vector<AcGePoint3d>& vertices3, std::vector<AcGePoint3d>& vertices2)
{
	//段数
	UINT32 segmentCount = getDrawableSegments();

	//巷道中心线向量
	std::vector<AcGeVector3d> centerVectors;
	getCenterVector(centerVectors);

	//巷道截面的“宽”方向向量
	std::vector<AcGeVector3d>  horizontalVectors;
	getHorizontalVector(horizontalVectors);

	for (uint j = 0; j < segmentCount; j++)
	{
		//断面形状，最初的截面,四个点
		AcGePoint3dArray pts;

		pts.append(AcGePoint3d(m_width_b*0.5, 0, 0));
		pts.append(AcGePoint3d(m_width_t*0.5, m_height, 0));
		pts.append(AcGePoint3d(-m_width_t*0.5, m_height, 0));
		pts.append(AcGePoint3d(-m_width_b*0.5, 0, 0));

		//巷道方向到Z方向角度 90度
		double angleToZ = AcGeVector3d::kZAxis.angleTo(centerVectors.at(j));
		for (int i = 0; i < pts.length(); i++)
		{
			pts.at(i).rotateBy(angleToZ, horizontalVectors.at(j));
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
			pts.at(i) += AcGeVector3d(0, 0, -m_height*0.5);
		}

		//这个几何分段下包含的颜色分段数
		int sg = m_turningPointIndexes.at(j + 1) - m_turningPointIndexes.at(j);
		for (int i = 0; i < sg + 1; i++)
		{
			//每个面偏移的长度
			double length =
				(m_color_basePoints.at(m_turningPointIndexes.at(j) + i) - m_color_basePoints.at(m_turningPointIndexes.at(j))).length();

			vertices3.emplace_back(pts.at(0) + centerVectors.at(j).normal()*length);
			vertices3.emplace_back(pts.at(1) + centerVectors.at(j).normal()*length);
			vertices3.emplace_back(pts.at(2) + centerVectors.at(j).normal()*length);
			vertices3.emplace_back(pts.at(3) + centerVectors.at(j).normal()*length);

			//m_resultBuffer->PointPtrArray.get()[(m_turningPointIndexes.at(j) + j) * 4 + i * 4]
			//	= pts.at(0) + centerVectors.at(j).normal()*length;
			//m_resultBuffer->PointPtrArray.get()[(m_turningPointIndexes.at(j) + j) * 4 + i * 4 + 1]
			//	= pts.at(1) + centerVectors.at(j).normal()*length;
			//m_resultBuffer->PointPtrArray.get()[(m_turningPointIndexes.at(j) + j) * 4 + i * 4 + 2]
			//	= pts.at(2) + centerVectors.at(j).normal()*length;
			//m_resultBuffer->PointPtrArray.get()[(m_turningPointIndexes.at(j) + j) * 4 + i * 4 + 3]
			//	= pts.at(3) + centerVectors.at(j).normal()*length;
		}

		//设置双线模式要显示的点
		double width = (m_width_b > m_width_t) ? m_width_b : m_width_t;
		auto nhv = horizontalVectors.at(j).normalize();
		vertices2.push_back(m_drawable_basePoints.at(j) + nhv*width);
		vertices2.push_back(vertices2.at(5 * j) + centerVectors.at(j));
		vertices2.push_back(vertices2.at(5 * j + 1) - nhv*width * 2);
		vertices2.push_back(vertices2.at(5 * j + 2) - centerVectors.at(j));
		vertices2.push_back(m_drawable_basePoints.at(j) + nhv*width);


		//m_resultBuffer->PointPtrArray2[5 * j] = m_drawable_basePoints.at(j) + nhv*width;
		//m_resultBuffer->PointPtrArray2[5 * j + 1] = m_resultBuffer->PointPtrArray2[5 * j] + centerVectors.at(j);
		//m_resultBuffer->PointPtrArray2[5 * j + 2] = m_resultBuffer->PointPtrArray2[5 * j + 1] - nhv*width * 2;
		//m_resultBuffer->PointPtrArray2[5 * j + 3] = m_resultBuffer->PointPtrArray2[5 * j + 2] - centerVectors.at(j);
	}
}

void Tunnel_Square::setFaceList(std::vector<int>& faceList)
{
	//段数
	UINT16 segmentCount = getDrawableSegments();

	for (UINT16 i = 0; i < segmentCount; i++)
	{

		for (UINT16 j = m_turningPointIndexes.at(i); j < m_turningPointIndexes.at(i + 1); j++)
		{
			//20 = 5 * 4
			//下面
			faceList.push_back(4);
			faceList.push_back(i * 4 + j * 4);
			faceList.push_back(i * 4 + j * 4 + 4);
			faceList.push_back(i * 4 + j * 4 + 5);
			faceList.push_back(i * 4 + j * 4 + 1);

			//背面
			faceList.push_back(4);
			faceList.push_back(i * 4 + j * 4 + 1);
			faceList.push_back(i * 4 + j * 4 + 2);
			faceList.push_back(i * 4 + j * 4 + 6);
			faceList.push_back(i * 4 + j * 4 + 5);

			//上面
			faceList.push_back(4);
			faceList.push_back(i * 4 + j * 4 + 2);
			faceList.push_back(i * 4 + j * 4 + 3);
			faceList.push_back(i * 4 + j * 4 + 7);
			faceList.push_back(i * 4 + j * 4 + 6);
			//前面
			faceList.push_back(4);
			faceList.push_back(i * 4 + j * 4 + 3);
			faceList.push_back(i * 4 + j * 4 + 7);
			faceList.push_back(i * 4 + j * 4 + 4);
			faceList.push_back(i * 4 + j * 4 + 0);
		}
	}
}

void Tunnel_Square::setVerticesColorList(std::vector<AcCmEntityColor>& colorList)
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
			uint8_t r, g, b;
			UInt32ToRGB(allColor.at(m_turningPointIndexes.at(j) + i), r, g, b);

			AcCmEntityColor c;
			c.setRGB(r, g, b);
			colorList.emplace_back(c);
			colorList.emplace_back(c);
			colorList.emplace_back(c);
			colorList.emplace_back(c);

			//colorList[(m_turningPointIndexes.at(j) + j) * 4 + i * 4].setRGB(r, g, b);
			//colorList[(m_turningPointIndexes.at(j) + j) * 4 + i * 4 + 1].setRGB(r, g, b);
			//colorList[(m_turningPointIndexes.at(j) + j) * 4 + i * 4 + 2].setRGB(r, g, b);
			//colorList[(m_turningPointIndexes.at(j) + j) * 4 + i * 4 + 3].setRGB(r, g, b);
		}
	}
}

Acad::ErrorStatus Tunnel_Square::setHeight(double height)
{
	assertWriteEnabled();
	m_height = height;
	return Acad::eOk;
}
Acad::ErrorStatus Tunnel_Square::setWidth_t(double width)
{
	assertWriteEnabled();
	m_width_t = width;
	return Acad::eOk;
}

Acad::ErrorStatus Tunnel_Square::setWidth_b(double width)
{
	assertWriteEnabled();
	m_width_b = width;
	return Acad::eOk;
}

const double Tunnel_Square::getHeight() const
{
	return m_height;
}

double const Tunnel_Square::getWidth_t() const
{
	return m_width_t;
}

double const Tunnel_Square::getWidth_b() const
{
	return m_width_b;
}

const double Tunnel_Square::getWidth() const
{
	return m_width_b > m_width_t ? m_width_b: m_width_t;
}

void Tunnel_Square::createDerive(Tunnel_Base*& newTunnel) const
{
	newTunnel = new Tunnel_Square();
}

