#pragma once
#ifndef MIM_TUNNELREACTOR_H
#define MIM_TUNNELREACTOR_H

namespace MIM
{

	class TunnelReactor :public AcDbEntityReactor
	{
	public:

		ACRX_DECLARE_MEMBERS(TunnelReactor);

		TunnelReactor() {}
		virtual ~TunnelReactor() {};

		virtual void erased(const AcDbObject* dbObj, Adesk::Boolean bErasing) override;

		virtual void modified(const AcDbObject* dbObj) override;
	};
}



#endif // !MIM_TUNNELREACTOR_H
