/**
* NuiSkeletonPointsViewer.h
* Modfied from Kinect SDK v.1.8
* alex.hong@mail.utoronto.ca
*/

#pragma once

#include "Utility.h"
#include "NuiViewer.h"

class KinectWindow;

class NuiSkeletonPointsViewer : public NuiViewer
{
public:
	/**
	* Constructor
	* @param  pParent  The pointer to parent window
	*/
	NuiSkeletonPointsViewer(const NuiViewer* pParent);

	/**
	 * Destructor
	 */
	~NuiSkeletonPointsViewer();

public:
	/**
	 * Set skeleton 3D position readings to display
	 * @param  headx
	 * ...
	 * @param lfoot
	 */
	void SetSkeletonPointsReadings(FLOAT headx,      FLOAT heady,      FLOAT headz,
		                           FLOAT cshoulderx, FLOAT cshouldery, FLOAT cshoulderz,
								   FLOAT spinex,     FLOAT spiney,     FLOAT spinez, 
								   FLOAT chipx,      FLOAT chipy,      FLOAT chipz, 
								   FLOAT rhandx,     FLOAT rhandy,     FLOAT rhandz, 
								   FLOAT rwristx,    FLOAT rwristy,    FLOAT rwristz,
								   FLOAT relbowx,    FLOAT relbowy,    FLOAT relbowz,
								   FLOAT rshoulderx, FLOAT rshouldery, FLOAT rshoulderz,
								   FLOAT rhipx,      FLOAT rhipy,      FLOAT rhipz,
								   FLOAT rkneex,     FLOAT rkneey,     FLOAT rkneez, 
								   FLOAT ranklex,    FLOAT rankley,    FLOAT ranklez,
								   FLOAT rfootx,     FLOAT rfooty,     FLOAT rfootz,
								   FLOAT lhandx,     FLOAT lhandy,     FLOAT lhandz,
								   FLOAT lwristx,    FLOAT lwristy,    FLOAT lwristz,
								   FLOAT lelbowx,    FLOAT lelbowy,    FLOAT lelbowz,
								   FLOAT lshoulderx, FLOAT lshouldery, FLOAT lshoulderz,
								   FLOAT lhipx,      FLOAT lhipy,      FLOAT lhipz,
								   FLOAT lkneex,     FLOAT lkneey,     FLOAT lkneez,
								   FLOAT lanklex,    FLOAT lankley,    FLOAT lanklez,
								   FLOAT lfootx,     FLOAT lfooty,     FLOAT lfootz);

private:
	/**
	 * Returns the id of the dialog
	 * @return Id of dialog
	 */
	virtual UINT GetDlgId();


	/**
	 * Process message or send it to coreponding handler
	 * @param   hWnd    The handle to the window which receives the message
	 * @param   uMsg    Message identifier
	 * @param   wParam  Additional message information
	 * @param   lParam  Additional message information
	 * @return  Result of message process. Depends on message type
	 */
	virtual LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

private:
	// 3D position coordinates of skeletal points
	// TODO: should probably put this in an array
	FLOAT m_headX;
	FLOAT m_headY;
	FLOAT m_headZ;

	FLOAT m_cshouldersX;
	FLOAT m_cshouldersY;
	FLOAT m_cshouldersZ;

	FLOAT m_spineX;
	FLOAT m_spineY;
	FLOAT m_spineZ;

	FLOAT m_chipX;
	FLOAT m_chipY;
	FLOAT m_chipZ;

	FLOAT m_rhandX;
	FLOAT m_rhandY;
	FLOAT m_rhandZ;

	FLOAT m_rwristX;
	FLOAT m_rwristY;
	FLOAT m_rwristZ;

	FLOAT m_relbowX;
	FLOAT m_relbowY;
	FLOAT m_relbowZ;

	FLOAT m_rshoulderX;
	FLOAT m_rshoulderY;
	FLOAT m_rshoulderZ;

	FLOAT m_rhipX;
	FLOAT m_rhipY;
	FLOAT m_rhipZ;

	FLOAT m_rkneeX;
	FLOAT m_rkneeY;
	FLOAT m_rkneeZ;

	FLOAT m_rankleX;
	FLOAT m_rankleY;
	FLOAT m_rankleZ;

	FLOAT m_rfootX;
	FLOAT m_rfootY;
	FLOAT m_rfootZ;

	FLOAT m_lhandX;
	FLOAT m_lhandY;
	FLOAT m_lhandZ;

	FLOAT m_lwristX;
	FLOAT m_lwristY;
	FLOAT m_lwristZ;

	FLOAT m_lelbowX;
	FLOAT m_lelbowY;
	FLOAT m_lelbowZ;

	FLOAT m_lshoulderX;
	FLOAT m_lshoulderY;
	FLOAT m_lshoulderZ;

	FLOAT m_lhipX;
	FLOAT m_lhipY;
	FLOAT m_lhipZ;

	FLOAT m_lkneeX;
	FLOAT m_lkneeY;
	FLOAT m_lkneeZ;

	FLOAT m_lankleX;
	FLOAT m_lankleY;
	FLOAT m_lankleZ;

	FLOAT m_lfootX;
	FLOAT m_lfootY;
	FLOAT m_lfootZ;
};