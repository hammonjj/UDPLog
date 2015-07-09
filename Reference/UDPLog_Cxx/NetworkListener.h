#pragma once

#include <QUdpSocket>

class IMessageContainer;

class NetworkListener : public QThread
{
	Q_OBJECT
public:
	NetworkListener(int nPort, IMessageContainer *messageContainer);
	~NetworkListener();

	void run();

public slots:
	void readPendingData();

private:
	QUdpSocket m_udpSocket;
	IMessageContainer *m_messageContainer;
};

