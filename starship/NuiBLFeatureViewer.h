/**
* NuiBLFeatureViewer.h
* alex.hong@mail.utoronto.ca
*/

#pragma once

#include "Utility.h"
#include "NuiViewer.h"

class KinectWindow;

class NuiBLFeatureViewer : public NuiViewer
{
public:
	/**
	* Constructor
	* @param  pParent  The pointer to parent window
	*/
	NuiBLFeatureViewer(const NuiViewer* pParent);

	/**
	* Destructor
	*/
	~NuiBLFeatureViewer();

public:
	/**
	* Set BL features
	*/
	void SetFeatureReadings(FLOAT bow_stretch_trunk,
		                   FLOAT open_close_arms,
						   FLOAT vert_head,
						   FLOAT fwd_bwd_head,
						   FLOAT vert_motion_body,
						   FLOAT fwd_bwd_motion_body,
						   FLOAT expand_body,
						   FLOAT spd_body);

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
	FLOAT m_bow_stretch_trunk;
	FLOAT m_open_close_arms;
	FLOAT m_vert_head;
	FLOAT m_fwd_bwd_head;
	FLOAT m_vert_motion_body;
	FLOAT m_fwd_bwd_motion_body;
	FLOAT m_expand_body;
	FLOAT m_spd_body;
};