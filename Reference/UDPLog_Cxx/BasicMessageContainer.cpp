#include "stdafx.h"
#include "BasicMessageContainer.h"


BasicMessageContainer::BasicMessageContainer(const int maxMessages) :
	m_maxMessages(maxMessages)
{
}

BasicMessageContainer::~BasicMessageContainer()
{
}

void BasicMessageContainer::AddMessage(const std::string &message)
{
	std::lock_guard<std::mutex> lock(m_mutex);
	if(m_maxMessages > m_messages.size()) { return; }

	m_messages.push_back(message);
}

bool BasicMessageContainer::RemoveMessage(std::string &message)
{
	std::lock_guard<std::mutex> lock(m_mutex);
	if(m_messages.empty()) { return false; }

	message = m_messages.front();
	m_messages.pop_front();
	return true;
}
