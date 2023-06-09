#include "INSane_IDS_EntryWrapper.h"
#include "INSane_DS_EntryWrapper.h"
#include <msclr/marshal_cppstd.h>

using namespace System;
using namespace System::Reflection;

public ref class VBDLL
{
	static Object^ _managedObject = nullptr;
	static Assembly^ assem = Assembly::LoadFrom(gcnew System::String(GetAssemblyDirectory()) + "\\INSane.dll");

public:
	static Type^ MyType = assem->GetType("INSane.classTWAIN");
	static Object^ GetWrapper()
	{
		try
		{
			if (_managedObject == nullptr)
			{
				_managedObject = Activator::CreateInstance(MyType);
				EventInfo^ MTDSEvent = MyType->GetEvent("Message_From_DS");
				Type^ handlerType = MTDSEvent->EventHandlerType;
				if (handlerType != nullptr)
				{
					Type^ delegateType = Type::GetType("Message_From_DS_EventReceiver");
					if (delegateType != nullptr)
					{
						Delegate^ d = Delegate::CreateDelegate(handlerType, delegateType, "OnMessage_From_DS");
						MTDSEvent->AddEventHandler(_managedObject, d);
					}
				}
			}
			return _managedObject;
		}
		catch (exception& e)
		{
			//XXX
		}
	}

	static System::String^ GetAssemblyDirectory()
	{
        System::String^ codeBase = Assembly::GetExecutingAssembly()->CodeBase;
		System::String^ filePath = Uri(codeBase).LocalPath;
        return System::IO::Path::GetDirectoryName(filePath);
	}
};

public ref class Message_From_DS_EventReceiver
{
public:
	static void OnMessage_From_DS(System::IntPtr _pOrigin, System::IntPtr _pDest, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, System::IntPtr _pData)
	{
		TW_INT16 result = _DSM_Entry((pTW_IDENTITY) (__int32) _pOrigin, (pTW_IDENTITY) (__int32) _pDest, _DG, _DAT, _MSG, (TW_MEMREF) _pData);
	}
};

TW_INT16 DS_EntryWrapper::Message_To_DS(pTW_IDENTITY _pOrigin, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, TW_MEMREF _pData)
{
	try
	{
		VBDLL vb;
		_managedObject = vb.GetWrapper();

		TCHAR szExeFileName[MAX_PATH];
		GetModuleFileName(NULL, szExeFileName, MAX_PATH);

		System::String^ _dsPath = VBDLL::GetAssemblyDirectory();
		std::string uM_dsPath = msclr::interop::marshal_as<std::string>(_dsPath);
		std::string _dsOrigin(uM_dsPath.substr(uM_dsPath.rfind("\\") + 1));

		MethodInfo^ MTDSMethod = vb.MyType->GetMethod("Message_To_DS");
		cli::array<System::Object^>^params = gcnew cli::array<System::Object^>(6);
		params[0] = (System::IntPtr) _pOrigin;
		params[1] = (System::UInt32) _DG;
		params[2] = (System::UInt32) _DAT;
		params[3] = (System::UInt16) _MSG;
		params[4] = (System::IntPtr) _pData;
		params[5] = gcnew String(_dsOrigin.c_str());
		return (TW_INT16) MTDSMethod->Invoke(_managedObject, params);
	}
	catch (exception& e) 
	{
		//XXX
	}
}

IDS_EntryWrapper  *IDS_EntryWrapper::CreateInstance()
{
    return ((IDS_EntryWrapper *) new DS_EntryWrapper());
}

void IDS_EntryWrapper::Destroy(IDS_EntryWrapper *instance)
{
    delete instance;
}