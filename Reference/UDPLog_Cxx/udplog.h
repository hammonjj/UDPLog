#ifndef UDPLOG_H
#define UDPLOG_H

#include <QtWidgets/QMainWindow>
#include "ui_udplog.h"

class IMessageContainer;

class UDPLog : public QMainWindow
{
	Q_OBJECT

public:
	UDPLog(IMessageContainer *messageContainer, QWidget *parent = nullptr);
	~UDPLog();

private:
	IMessageContainer *m_messageContainer;
	Ui::UDPLogClass ui;
};

#endif // UDPLOG_H
