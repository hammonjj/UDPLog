#include "stdafx.h"

#include <thread>
#include <QtWidgets/QApplication>

#include "udplog.h"


#include "BasicMessageContainer.h"
#include "NetworkListener.h"

int main(int argc, char *argv[])
{
	//Syslog: 514
	const int port = 514;
	const int maxMessages = 1000;

	QApplication a(argc, argv);

	BasicMessageContainer messageContainer{maxMessages};
	NetworkListener networkListener{port, &messageContainer};
	networkListener.start();

	UDPLog w{&messageContainer};
	w.show();

	return a.exec();
}
