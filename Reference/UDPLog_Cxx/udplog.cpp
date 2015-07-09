#include "stdafx.h"

#include "IMessageContainer.h"
#include "udplog.h"

UDPLog::UDPLog(IMessageContainer *messageContainer, QWidget *parent) :
	QMainWindow(parent),
	m_messageContainer(messageContainer)
{
	ui.setupUi(this);
}

UDPLog::~UDPLog()
{
}
