#include "stdafx.h"

#include "IMessageContainer.h"
#include "NetworkListener.h"

NetworkListener::NetworkListener(int nPort, IMessageContainer *messageContainer) :
	m_messageContainer(messageContainer)
{
	m_udpSocket.bind(QHostAddress::LocalHost, nPort);
}

NetworkListener::~NetworkListener()
{
}

void NetworkListener::run()
{
	qDebug() << "Starting Listening Thread";

	QtUtils::connect(
		&m_udpSocket,
		&QUdpSocket::readyRead,
		this,
		&NetworkListener::readPendingData,
		__FILE__,
		__LINE__);

	exec();
}

void NetworkListener::readPendingData()
{
	qDebug() << "NetworkListener::readPendingData";
	while (m_udpSocket.hasPendingDatagrams()) 
	{
		QByteArray datagram;
		datagram.resize(m_udpSocket.pendingDatagramSize());
		QHostAddress sender;
		quint16 senderPort;

		m_udpSocket.readDatagram(
			datagram.data(), 
			datagram.size(),
			&sender, 
			&senderPort);

		qDebug() << datagram;
		m_messageContainer->AddMessage(datagram.data());
	}
}