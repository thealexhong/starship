/**
* NuiBLClassificationViewer.cpp
* alex.hong@mail.utoronto.ca
*/

#include "stdafx.h"
#include <limits>
#include "NuiBLClassificationViewer.h"
#include "resource.h"

/**
* Constructor
* @param  pParent  The pointer to parent window
*/
NuiBLClassificationViewer::NuiBLClassificationViewer(const NuiViewer* pParent) : NuiViewer(pParent),
m_valence(FLT_MAX),
m_arousal(FLT_MAX)
{
}


/**
* Destructor
*/
NuiBLClassificationViewer::~NuiBLClassificationViewer()
{
}

/**
* Process message or send it to coreponding handler
* @param   hWnd    The handle to the window which receives the message
* @param   uMsg    Message identifier
* @param   wParam  Additional message information
* @param   lParam  Additional message information
* @return  Result of message process. Depends on message type
*/
LRESULT NuiBLClassificationViewer::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	// Pass the message to default dialog procedure
	return FALSE;
}

/**
* Returns the id of the dialog
*/
UINT NuiBLClassificationViewer::GetDlgId()
{
	return IDD_BLCLASSIFICATION_VIEW;
}


void NuiBLClassificationViewer::SetAffectReadings(FLOAT valence, FLOAT arousal)
{
	// Format the readings and update to static control
	// Fuck this doesn't scale... whatever
	CompareUpdateValue(valence, m_valence, m_hWnd, IDC_BLVALENCE_READING, L"%.2f");
	CompareUpdateValue(arousal, m_arousal, m_hWnd, IDC_BLAROUSAL_READING, L"%.2f");
}