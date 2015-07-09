#pragma once

#include <string>

class ILogMessage;

class IMessageParser
{
public:
	virtual ILogMessage* ParseMessage(const std::string &raw) = 0;
	virtual ~IMessageParser() = default;
};