/********************************************************************************
** Form generated from reading UI file 'udplog.ui'
**
** Created by: Qt User Interface Compiler version 5.4.0
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_UDPLOG_H
#define UI_UDPLOG_H

#include <QtCore/QVariant>
#include <QtWidgets/QAction>
#include <QtWidgets/QApplication>
#include <QtWidgets/QButtonGroup>
#include <QtWidgets/QHeaderView>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QMenuBar>
#include <QtWidgets/QStatusBar>
#include <QtWidgets/QToolBar>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_UDPLogClass
{
public:
    QMenuBar *menuBar;
    QToolBar *mainToolBar;
    QWidget *centralWidget;
    QStatusBar *statusBar;

    void setupUi(QMainWindow *UDPLogClass)
    {
        if (UDPLogClass->objectName().isEmpty())
            UDPLogClass->setObjectName(QStringLiteral("UDPLogClass"));
        UDPLogClass->resize(600, 400);
        menuBar = new QMenuBar(UDPLogClass);
        menuBar->setObjectName(QStringLiteral("menuBar"));
        UDPLogClass->setMenuBar(menuBar);
        mainToolBar = new QToolBar(UDPLogClass);
        mainToolBar->setObjectName(QStringLiteral("mainToolBar"));
        UDPLogClass->addToolBar(mainToolBar);
        centralWidget = new QWidget(UDPLogClass);
        centralWidget->setObjectName(QStringLiteral("centralWidget"));
        UDPLogClass->setCentralWidget(centralWidget);
        statusBar = new QStatusBar(UDPLogClass);
        statusBar->setObjectName(QStringLiteral("statusBar"));
        UDPLogClass->setStatusBar(statusBar);

        retranslateUi(UDPLogClass);

        QMetaObject::connectSlotsByName(UDPLogClass);
    } // setupUi

    void retranslateUi(QMainWindow *UDPLogClass)
    {
        UDPLogClass->setWindowTitle(QApplication::translate("UDPLogClass", "UDPLog", 0));
    } // retranslateUi

};

namespace Ui {
    class UDPLogClass: public Ui_UDPLogClass {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_UDPLOG_H
