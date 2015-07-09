#pragma once

#include "IMessageParser.h"

class AventuraMessageParser : public IMessageParser
{
public:
	AventuraMessageParser();
	~AventuraMessageParser();

	virtual ILogMessage* ParseMessage(const std::string &rawData) override;
};

