#include "StdAfx.h"
#include "ObjectOpener.h"
#include "Tunnel_Base.h"
#include "TunnelNode.h"
#include "TunnelTag.h"
#include "TunnelReactor.h"

using namespace MIM;

static void handleTunnelNode(int i, TunnelNode* otherNode, const Tunnel_Base* tunnel);

static void modifyNode(const Tunnel_Base* pTunnel);
template<typename T>
static void handleTunnel(int i, Tunnel_Base* otherTunnel, const Tunnel_Base* tunnel);

ACRX_NO_CONS_DEFINE_MEMBERS(TunnelReactor, AcDbEntityReactor);

void TunnelReactor::erased(const AcDbObject* dbObj, Adesk::Boolean bErasing)
{
	if (bErasing == true)
	{
		Acad::ErrorStatus es = Acad::eOk;

		const Tunnel_Base *pTunnel = static_cast<const Tunnel_Base*>(dbObj);

		//自己的句柄
		AcDbHandle handle;
		pTunnel->getAcDbHandle(handle);

		const std::vector<AcDbHandle>& nodesHandle = pTunnel->getNodesHandle();

		for (auto& nodeHandle : nodesHandle)
		{
			TunnelNode* Node = nullptr;
			ObjectOpener<TunnelNode> opener(Node, nodeHandle, AcDb::kForWrite);
			es = opener.open();
			if (es == Acad::eOk)
			{
				//从节点中移除对该巷道的引用计数
				Node->removeTunnel(handle);

				//如果引用为0，删除
				if (bErasing == true)
				{
					bool isErased = Node->tryErase();
				}
			}

			//删除标注
			const AcDbHandle& tagHandle = pTunnel->getTagHandle();
			TunnelTag* pTag = nullptr;
			ObjectOpener<TunnelTag> opener1(pTag, tagHandle, AcDb::kForWrite);
			if ((es = opener1.open()) == Acad::eOk)
			{
				pTag->canErase(true);
				pTag->erase(true);
			}
		}
	}
}

void TunnelReactor::modified(const AcDbObject* dbObj)
{
	//动画模式下不执行
	if (Tunnel_Base::getAnimateMode() == true)
		return;
	//被删除或即将被删除时不执行
	if (dbObj->isEraseStatusToggled() || dbObj->isErased())
		return;

	Acad::ErrorStatus es = Acad::eOk;

	const Tunnel_Base *pTunnel = static_cast<const Tunnel_Base*>(dbObj);
	


	const std::vector<AcGePoint3d>& points = pTunnel->getBasePoints();
	const std::vector<UInt32>& colors = pTunnel->getColors();

	const std::vector<AcDbHandle>& nodeHandles = pTunnel->getNodesHandle();

	int i = 0;
	for (auto & handle : nodeHandles)
	{
		TunnelNode* pNode = nullptr;
		ObjectOpener<TunnelNode> opener(pNode, handle, AcDb::kForWrite);

		if ((es = opener.open()) == Acad::eOk)
		{			
			if(pNode->getPosition() != points.at(i))
			pNode->setPosition(points.at(i));
		}
		i++;
	}

	//移动后改变标注的位置
	const AcDbHandle& tagHandle = pTunnel->getTagHandle();
	TunnelTag* pTag = nullptr;
	ObjectOpener<TunnelTag> opener(pTag, tagHandle, AcDb::kForWrite);
	if ((es = opener.open()) == Acad::eOk)
	{
		auto av = points.at(points.size() / 2 - 1) - points.at(points.size() / 2);
		auto newStdPoint = points.at(points.size() / 2) + av / 2;
		auto oldStdPoint = pTag->getStartPoint();

		auto tr = newStdPoint - oldStdPoint;
		AcGeMatrix3d xform;
		xform.setToTranslation(tr);
		pTag->transformBy(xform);

		pTag->setText(pTunnel->getTagData());
	}
}