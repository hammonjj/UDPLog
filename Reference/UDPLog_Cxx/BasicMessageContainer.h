#pragma once

#include <deque>
#include <mutex>

#include "IMessageContainer.h"

//Thread safe container for storing incoming log messages
class BasicMessageContainer : public IMessageContainer
{
public:
	explicit BasicMessageContainer(const int maxMessages);
	~BasicMessageContainer();

	virtual void AddMessage(const std::string &message) override;
	virtual bool RemoveMessage(std::string &message) override;

private:
	int m_maxMessages;
	std::mutex m_mutex;
	std::deque<std::string> m_messages;
};

