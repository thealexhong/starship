/**
* NuiSkeletonPointsViewer.cpp
* Modfied from Kinect SDK v.1.8
* alex.hong@mail.utoronto.ca
*/

#include "stdafx.h"
#include <limits>
#include "NuiSkeletonPointsViewer.h"
#include "resource.h"

/**
 * Constructor
 * @param  pParent  The pointer to parent window
 */
NuiSkeletonPointsViewer::NuiSkeletonPointsViewer(const NuiViewer* pParent) : NuiViewer(pParent),
                                                                           m_headX(FLT_MAX),       m_headY(FLT_MAX),       m_headZ(FLT_MAX),
																		   m_cshouldersX(FLT_MAX), m_cshouldersY(FLT_MAX), m_cshouldersZ(FLT_MAX),
																		   m_spineX(FLT_MAX),      m_spineY(FLT_MAX),      m_spineZ(FLT_MAX),
																		   m_chipX(FLT_MAX),       m_chipY(FLT_MAX),       m_chipZ(FLT_MAX),
																		   m_rhandX(FLT_MAX),      m_rhandY(FLT_MAX),      m_rhandZ(FLT_MAX),
																		   m_rwristX(FLT_MAX),     m_rwristY(FLT_MAX),     m_rwristZ(FLT_MAX),
																		   m_relbowX(FLT_MAX),     m_relbowY(FLT_MAX),     m_relbowZ(FLT_MAX),
																		   m_rshoulderX(FLT_MAX),  m_rshoulderY(FLT_MAX),  m_rshoulderZ(FLT_MAX),
																		   m_rhipX(FLT_MAX),       m_rhipY(FLT_MAX),       m_rhipZ(FLT_MAX),
																		   m_rkneeX(FLT_MAX),      m_rkneeY(FLT_MAX),      m_rkneeZ(FLT_MAX),
																		   m_rankleX(FLT_MAX),     m_rankleY(FLT_MAX),     m_rankleZ(FLT_MAX),
																		   m_rfootX(FLT_MAX),      m_rfootY(FLT_MAX),      m_rfootZ(FLT_MAX),
																		   m_lhandX(FLT_MAX),      m_lhandY(FLT_MAX),      m_lhandZ(FLT_MAX),
																		   m_lwristX(FLT_MAX),     m_lwristY(FLT_MAX),     m_lwristZ(FLT_MAX),
																		   m_lelbowX(FLT_MAX),     m_lelbowY(FLT_MAX),     m_lelbowZ(FLT_MAX),
																		   m_lshoulderX(FLT_MAX),  m_lshoulderY(FLT_MAX),  m_lshoulderZ(FLT_MAX),
																		   m_lhipX(FLT_MAX),       m_lhipY(FLT_MAX),       m_lhipZ(FLT_MAX),
																		   m_lkneeX(FLT_MAX),      m_lkneeY(FLT_MAX),      m_lkneeZ(FLT_MAX),
																		   m_lankleX(FLT_MAX),     m_lankleY(FLT_MAX),     m_lankleZ(FLT_MAX),
																		   m_lfootX(FLT_MAX),      m_lfootY(FLT_MAX),      m_lfootZ(FLT_MAX)

{
}


/** 
 * Destructor
 */
NuiSkeletonPointsViewer::~NuiSkeletonPointsViewer()
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
LRESULT NuiSkeletonPointsViewer::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	// Pass the message to default dialog procedure
	return FALSE;
}

/**
 * Returns the id of the dialog
 */
UINT NuiSkeletonPointsViewer::GetDlgId()
{
	return IDD_SKELETON_VIEW;
}

/**
* Set skeleton 3D position readings to display
* @param  headx
* ...
* @param lfoot
*/
void NuiSkeletonPointsViewer::SetSkeletonPointsReadings(FLOAT headx, FLOAT heady, FLOAT headz,
                                                        FLOAT cshoulderx, FLOAT cshouldery, FLOAT cshoulderz,
														FLOAT spinex, FLOAT spiney, FLOAT spinez,
														FLOAT chipx, FLOAT chipy, FLOAT chipz,
														FLOAT rhandx, FLOAT rhandy, FLOAT rhandz,
														FLOAT rwristx, FLOAT rwristy, FLOAT rwristz,
														FLOAT relbowx, FLOAT relbowy, FLOAT relbowz,
														FLOAT rshoulderx, FLOAT rshouldery, FLOAT rshoulderz,
														FLOAT rhipx, FLOAT rhipy, FLOAT rhipz,
														FLOAT rkneex, FLOAT rkneey, FLOAT rkneez,
														FLOAT ranklex, FLOAT rankley, FLOAT ranklez,
														FLOAT rfootx, FLOAT rfooty, FLOAT rfootz,
														FLOAT lhandx, FLOAT lhandy, FLOAT lhandz,
														FLOAT lwristx, FLOAT lwristy, FLOAT lwristz,
														FLOAT lelbowx, FLOAT lelbowy, FLOAT lelbowz,
														FLOAT lshoulderx, FLOAT lshouldery, FLOAT lshoulderz,
														FLOAT lhipx, FLOAT lhipy, FLOAT lhipz,
														FLOAT lkneex, FLOAT lkneey, FLOAT lkneez,
														FLOAT lanklex, FLOAT lankley, FLOAT lanklez,
														FLOAT lfootx, FLOAT lfooty, FLOAT lfootz)
{
	// Format the readings and update to static control
	// Fuck this doesn't scale... whatever
	CompareUpdateValue(headx, m_headX, m_hWnd, IDC_SKELETON_HEAD_X_READING, L"%.2f");
	CompareUpdateValue(heady, m_headY, m_hWnd, IDC_SKELETON_HEAD_Y_READING, L"%.2f");
	CompareUpdateValue(headz, m_headZ, m_hWnd, IDC_SKELETON_HEAD_Z_READING, L"%.2f");
	CompareUpdateValue(cshoulderx, m_cshouldersX, m_hWnd, IDC_SKELETON_CSHOULDER_X_READING, L"%.2f");
	CompareUpdateValue(cshouldery, m_cshouldersY, m_hWnd, IDC_SKELETON_CSHOULDER_Y_READING, L"%.2f");
	CompareUpdateValue(cshoulderz, m_cshouldersZ, m_hWnd, IDC_SKELETON_CSHOULDER_Z_READING, L"%.2f");
	CompareUpdateValue(spinex, m_spineX, m_hWnd, IDC_SKELETON_SPINE_X_READING, L"%.2f");
	CompareUpdateValue(spiney, m_spineY, m_hWnd, IDC_SKELETON_SPINE_Y_READING, L"%.2f");
	CompareUpdateValue(spinez, m_spineZ, m_hWnd, IDC_SKELETON_SPINE_Z_READING, L"%.2f");
	CompareUpdateValue(chipx, m_chipX, m_hWnd, IDC_SKELETON_CHIP_X_READING, L"%.2f");
	CompareUpdateValue(chipy, m_chipY, m_hWnd, IDC_SKELETON_CHIP_Y_READING, L"%.2f");
	CompareUpdateValue(chipz, m_chipZ, m_hWnd, IDC_SKELETON_CHIP_Z_READING, L"%.2f");
	CompareUpdateValue(rhandx, m_rhandX, m_hWnd, IDC_SKELETON_RHAND_X_READING, L"%.2f");
	CompareUpdateValue(rhandy, m_rhandY, m_hWnd, IDC_SKELETON_RHAND_Y_READING, L"%.2f");
	CompareUpdateValue(rhandz, m_rhandZ, m_hWnd, IDC_SKELETON_RHAND_Z_READING, L"%.2f");
	CompareUpdateValue(rwristx, m_rwristX, m_hWnd, IDC_SKELETON_RWRIST_X_READING, L"%.2f");
	CompareUpdateValue(rwristy, m_rwristY, m_hWnd, IDC_SKELETON_RWRIST_Y_READING, L"%.2f");
	CompareUpdateValue(rwristz, m_rwristZ, m_hWnd, IDC_SKELETON_RWRIST_Z_READING, L"%.2f");
	CompareUpdateValue(relbowx, m_relbowX, m_hWnd, IDC_SKELETON_RELBOW_X_READING, L"%.2f");
	CompareUpdateValue(relbowy, m_relbowY, m_hWnd, IDC_SKELETON_RELBOW_Y_READING, L"%.2f");
	CompareUpdateValue(relbowz, m_relbowZ, m_hWnd, IDC_SKELETON_RELBOW_Z_READING, L"%.2f");
	CompareUpdateValue(rshoulderx, m_rshoulderX, m_hWnd, IDC_SKELETON_RSHOULDER_X_READING, L"%.2f");
	CompareUpdateValue(rshouldery, m_rshoulderY, m_hWnd, IDC_SKELETON_RSHOULDER_Y_READING, L"%.2f");
	CompareUpdateValue(rshoulderz, m_rshoulderZ, m_hWnd, IDC_SKELETON_RSHOULDER_Z_READING, L"%.2f");
	CompareUpdateValue(rhipx, m_rhipX, m_hWnd, IDC_SKELETON_RHIP_X_READING, L"%.2f");
	CompareUpdateValue(rhipy, m_rhipY, m_hWnd, IDC_SKELETON_RHIP_Y_READING, L"%.2f");
	CompareUpdateValue(rhipz, m_rhipZ, m_hWnd, IDC_SKELETON_RHIP_Z_READING, L"%.2f");
	CompareUpdateValue(rkneex, m_rkneeX, m_hWnd, IDC_SKELETON_RKNEE_X_READING, L"%.2f");
	CompareUpdateValue(rkneey, m_rkneeY, m_hWnd, IDC_SKELETON_RKNEE_Y_READING, L"%.2f");
	CompareUpdateValue(rkneez, m_rkneeZ, m_hWnd, IDC_SKELETON_RKNEE_Z_READING, L"%.2f");
	CompareUpdateValue(ranklex, m_rankleX, m_hWnd, IDC_SKELETON_RANKLE_X_READING, L"%.2f");
	CompareUpdateValue(rankley, m_rankleY, m_hWnd, IDC_SKELETON_RANKLE_Y_READING, L"%.2f");
	CompareUpdateValue(ranklez, m_rankleZ, m_hWnd, IDC_SKELETON_RANKLE_Z_READING, L"%.2f");
	CompareUpdateValue(rfootx, m_rfootX, m_hWnd, IDC_SKELETON_RFOOT_X_READING, L"%.2f");
	CompareUpdateValue(rfooty, m_rfootY, m_hWnd, IDC_SKELETON_RFOOT_Y_READING, L"%.2f");
	CompareUpdateValue(rfootz, m_rfootZ, m_hWnd, IDC_SKELETON_RFOOT_Z_READING, L"%.2f");
	CompareUpdateValue(lhandx, m_lhandX, m_hWnd, IDC_SKELETON_LHAND_X_READING, L"%.2f");
	CompareUpdateValue(lhandy, m_lhandY, m_hWnd, IDC_SKELETON_LHAND_Y_READING, L"%.2f");
	CompareUpdateValue(lhandz, m_lhandZ, m_hWnd, IDC_SKELETON_LHAND_Z_READING, L"%.2f");
	CompareUpdateValue(lwristx, m_lwristX, m_hWnd, IDC_SKELETON_LWRIST_X_READING, L"%.2f");
	CompareUpdateValue(lwristy, m_lwristY, m_hWnd, IDC_SKELETON_LWRIST_Y_READING, L"%.2f");
	CompareUpdateValue(lwristz, m_lwristZ, m_hWnd, IDC_SKELETON_LWRIST_Z_READING, L"%.2f");
	CompareUpdateValue(lelbowx, m_lelbowX, m_hWnd, IDC_SKELETON_LELBOW_X_READING, L"%.2f");
	CompareUpdateValue(lelbowy, m_lelbowY, m_hWnd, IDC_SKELETON_LELBOW_Y_READING, L"%.2f");
	CompareUpdateValue(lelbowz, m_lelbowZ, m_hWnd, IDC_SKELETON_LELBOW_Z_READING, L"%.2f");
	CompareUpdateValue(lshoulderx, m_lshoulderX, m_hWnd, IDC_SKELETON_LSHOULDER_X_READING, L"%.2f");
	CompareUpdateValue(lshouldery, m_lshoulderY, m_hWnd, IDC_SKELETON_LSHOULDER_Y_READING, L"%.2f");
	CompareUpdateValue(lshoulderz, m_lshoulderZ, m_hWnd, IDC_SKELETON_LSHOULDER_Z_READING, L"%.2f");
	CompareUpdateValue(lhipx, m_lhipX, m_hWnd, IDC_SKELETON_LHIP_X_READING, L"%.2f");
	CompareUpdateValue(lhipy, m_lhipY, m_hWnd, IDC_SKELETON_LHIP_Y_READING, L"%.2f");
	CompareUpdateValue(lhipz, m_lhipZ, m_hWnd, IDC_SKELETON_LHIP_Z_READING, L"%.2f");
	CompareUpdateValue(lkneex, m_lkneeX, m_hWnd, IDC_SKELETON_LKNEE_X_READING, L"%.2f");
	CompareUpdateValue(lkneey, m_lkneeY, m_hWnd, IDC_SKELETON_LKNEE_Y_READING, L"%.2f");
	CompareUpdateValue(lkneez, m_lkneeZ, m_hWnd, IDC_SKELETON_LKNEE_Z_READING, L"%.2f");
	CompareUpdateValue(lanklex, m_lankleX, m_hWnd, IDC_SKELETON_LANKLE_X_READING, L"%.2f");
	CompareUpdateValue(lankley, m_lankleY, m_hWnd, IDC_SKELETON_LANKLE_Y_READING, L"%.2f");
	CompareUpdateValue(lanklez, m_lankleZ, m_hWnd, IDC_SKELETON_LANKLE_Z_READING, L"%.2f");
	CompareUpdateValue(lfootx, m_lfootX, m_hWnd, IDC_SKELETON_LFOOT_X_READING, L"%.2f");
	CompareUpdateValue(lfooty, m_lfootY, m_hWnd, IDC_SKELETON_LFOOT_Y_READING, L"%.2f");
	CompareUpdateValue(lfootz, m_lfootZ, m_hWnd, IDC_SKELETON_LFOOT_Z_READING, L"%.2f");
}