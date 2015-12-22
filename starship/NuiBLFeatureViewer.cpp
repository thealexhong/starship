/**
* NuiBLFeatureViewer.cpp
* alex.hong@mail.utoronto.ca
*/

#include "stdafx.h"
#include <limits>
#include "NuiBLFeatureViewer.h"
#include "resource.h"

/**
* Constructor
* @param  pParent  The pointer to parent window
*/
NuiBLFeatureViewer::NuiBLFeatureViewer(const NuiViewer* pParent) : NuiViewer(pParent),
                                                                             m_bow_stretch_trunk(FLT_MAX),
																			 m_open_close_arms(FLT_MAX),
																			 m_vert_head(FLT_MAX),
																			 m_fwd_bwd_head(FLT_MAX),
																			 m_vert_motion_body(FLT_MAX),
																			 m_fwd_bwd_motion_body(FLT_MAX),
																			 m_expand_body(FLT_MAX),
																			 m_spd_body(FLT_MAX)
{
}


/**
* Destructor
*/
NuiBLFeatureViewer::~NuiBLFeatureViewer()
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
LRESULT NuiBLFeatureViewer::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	// Pass the message to default dialog procedure
	return FALSE;
}

/**
* Returns the id of the dialog
*/
UINT NuiBLFeatureViewer::GetDlgId()
{
	return IDD_BLFEATURES_VIEW;
}


void NuiBLFeatureViewer::SetFeatureReadings(FLOAT bow_stretch_trunk,
	                                       FLOAT open_close_arms,
										   FLOAT vert_head,
										   FLOAT fwd_bwd_head,
										   FLOAT vert_motion_body,
										   FLOAT fwd_bwd_motion_body,
										   FLOAT expand_body,
										   FLOAT spd_body)
{
	// Format the readings and update to static control
	// Fuck this doesn't scale... whatever
	CompareUpdateValue(bow_stretch_trunk, m_bow_stretch_trunk, m_hWnd, IDC_BOW_STRETCH_TRUNK_READING, L"%.2f");
	CompareUpdateValue(open_close_arms, m_open_close_arms, m_hWnd, IDC_OPEN_CLOSE_ARMS_READING, L"%.2f");
	CompareUpdateValue(vert_head, m_vert_head, m_hWnd, IDC_VERT_HEAD_READING, L"%.2f");
	CompareUpdateValue(fwd_bwd_head, m_fwd_bwd_head, m_hWnd, IDC_FWD_BWD_HEAD_READING, L"%.2f");
	CompareUpdateValue(vert_motion_body, m_vert_motion_body, m_hWnd, IDC_VERT_MOTION_BODY_READING, L"%.2f");
	CompareUpdateValue(fwd_bwd_motion_body, m_fwd_bwd_motion_body, m_hWnd, IDC_FWD_BWD_MOTION_BODY_READING, L"%.2f");
	CompareUpdateValue(expand_body, m_expand_body, m_hWnd, IDC_EXPAND_BODY_READING, L"%.2f");
	CompareUpdateValue(spd_body, m_spd_body, m_hWnd, IDC_SPD_BODY_READING, L"%.2f");
}