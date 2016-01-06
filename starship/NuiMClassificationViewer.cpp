/**
* NuiBLClassificationViewer.cpp
* alex.hong@mail.utoronto.ca
*/

#include "stdafx.h"
#include <limits>
#include "NuiMClassificationViewer.h"
#include "resource.h"

/**
* Constructor
* @param  pParent  The pointer to parent window
*/
NuiMClassificationViewer::NuiMClassificationViewer(const NuiViewer* pParent) : NuiViewer(pParent),
m_mmvalence(FLT_MAX),
m_mmarousal(FLT_MAX),
m_blvalence(FLT_MAX),
m_blarousal(FLT_MAX),
m_vvalence(FLT_MAX),
m_varousal(FLT_MAX)
{
}


/**
* Destructor
*/
NuiMClassificationViewer::~NuiMClassificationViewer()
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
LRESULT NuiMClassificationViewer::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	// Pass the message to default dialog procedure
	return FALSE;
}

/**
* Returns the id of the dialog
*/
UINT NuiMClassificationViewer::GetDlgId()
{
	return IDD_MCLASSIFICATION_VIEW;
}


void NuiMClassificationViewer::SetAffectReadings(FLOAT mmvalence, FLOAT mmarousal, FLOAT blvalence, FLOAT blarousal, FLOAT vvalence, FLOAT varousal)
{
	// Format the readings and update to static control
	// Fuck this doesn't scale... whatever
	CompareUpdateValue(mmvalence, m_mmvalence, m_hWnd, IDC_MVALENCE_READING, L"%.2f");
	CompareUpdateValue(mmarousal, m_mmarousal, m_hWnd, IDC_MAROUSAL_READING, L"%.2f");
	CompareUpdateValue(blvalence, m_mmvalence, m_hWnd, IDC_BLVALENCE_READING, L"%.2f");
	CompareUpdateValue(blarousal, m_mmarousal, m_hWnd, IDC_BLAROUSAL_READING, L"%.2f");
	CompareUpdateValue(vvalence, m_mmvalence, m_hWnd, IDC_VVALENCE_READING, L"%.2f");
	CompareUpdateValue(varousal, m_mmarousal, m_hWnd, IDC_VAROUSAL_READING, L"%.2f");
}