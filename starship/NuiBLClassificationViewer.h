/**
* NuiBLClassificationViewer.h
* alex.hong@mail.utoronto.ca
*/

#pragma once

#include "Utility.h"
#include "NuiViewer.h"

class KinectWindow;

class NuiBLClassificationViewer : public NuiViewer
{
public:
	/**
	* Constructor
	* @param  pParent  The pointer to parent window
	*/
	NuiBLClassificationViewer(const NuiViewer* pParent);

	/**
	* Destructor
	*/
	~NuiBLClassificationViewer();

public:
	/**
	* Set valence and arousal values
	*/
	void SetAffectReadings(FLOAT valence, FLOAT arousal);

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
	FLOAT m_valence;
	FLOAT m_arousal;
};