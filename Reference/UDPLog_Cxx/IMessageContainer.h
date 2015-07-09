#pragma once

#include <string>

class IMessageContainer
{
public:
	virtual void AddMessage(const std::string &message) = 0;
	virtual bool RemoveMessage(std::string &message) = 0;

	virtual ~IMessageContainer() = default;
};