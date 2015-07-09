#pragma once

namespace QtUtils
{
	template <typename Func1, typename Func2>
	bool connect(
		const typename QtPrivate::FunctionPointer<Func1>::Object *sender, 
		Func1 signal,
		const typename QtPrivate::FunctionPointer<Func2>::Object *receiver, 
		Func2 slot,
		const char *file,
		const int &line)
	{
		bool bOk = QObject::connect(sender, signal, receiver, slot);

#ifdef _DEBUG
		Q_ASSERT_X(
			bOk,
			"QtUtils::connect",
			QString("Failed to make connection - %1(%2)")
				.arg(file)
				.arg(line).toUtf8());
#endif
		return bOk;
	}
}