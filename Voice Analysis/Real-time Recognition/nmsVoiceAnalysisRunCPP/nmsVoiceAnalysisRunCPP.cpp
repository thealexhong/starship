#include "stdafx.h"
#include <iostream>

//Import the type library
#import "..\nmsVoiceAnalysisLibrary\bin\Debug\nmsVoiceAnalysisLibrary.tlb" raw_interfaces_only
using namespace nmsVoiceAnalysisLibrary;
using namespace System;
using namespace std;

int _tmain(int argc, _TCHAR* argv[]) 
{
	//Initialize COM
	HRESULT hr = CoInitialize(NULL);

	//Create the interface pointer
	nmsFunctionsPtr nmsPtr(__uuidof(ManagedClass));
	
	VARIANT_BOOL success = false;
	//pIProcessFunctions->ProcessController();
	
	nmsPtr->Initialize(&success);
	if(success)
	{
		Console::ForegroundColor = ConsoleColor::Green;
		printf("Voice Analysis Program is running...\n") ;
		nmsPtr->Start();
	}
	//Uninitialize COM
	CoUninitialize();

	return 0;
}