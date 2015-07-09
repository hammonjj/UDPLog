#include "stdafx.h"
#include "AventuraMessageParser.h"
#include "ILogMessage.h"

AventuraMessageParser::AventuraMessageParser()
{
}

AventuraMessageParser::~AventuraMessageParser()
{
}

ILogMessage* AventuraMessageParser::ParseMessage(const std::string &raw)
{
	//Header Space Delimated
	//Severity: <XX>1 => ^<[0-9]+>
	//Timestamp: =>[0-9]+-[0-9]+-[0-9]+T[0-9]+:[0-9]+:[0-9]+.[0-9]+-[0-9]:[0-9]+
	//Source
	//Process
	//Process Id
	//Possible TI Error Code
	//Filler: " - "
	//[ => \[(.*?)\]
	//	lib=""
	//	file=""
	//	line=""
	//	thread=""
	//	project=""
	//]
	//Message => .*] *([^\n\r]*)

	/*
	std::string s = "scott>=tiger";
	std::string delimiter = ">=";
	std::string token = s.substr(0, s.find(delimiter)); // token is "scott"
	*/
	return nullptr;
}