/**
* NuiViewer.h
* Modfied from Kinect SDK v.1.8
* alex.hong@mail.utoronto.ca
*/

#pragma once

#include <Windows.h>

//Forward declaration to avoid header file cross reference conflict
class KinectWindow;

class NuiViewer
{
public:
	/**
	 * Constructor
	 * @param  pParent  The pointer to parent window
	 */
	NuiViewer(const NuiViewer* pParent);

public:
	/**
	 * Create window of viewer
	 * @return  Indicates success or failure
	 */
	virtual bool CreateView();

	/**
	 * Shows up the window of the viewer
	 */
	virtual void ShowView();

	/**
	 * Hide the window of the viewer
	 */
	virtual void HideView();

	/**
	 * Returns a window handle
	 * @return  Handle the window
	 */
	virtual HWND GetWindow() const;

	/**
	 * Set window wiht position and size
	 * @param   rect  New position and size
	 * @return  Indicate success or failure
	 */
	virtual bool MoveView(const RECT& rect);

protected:
	/**
	 * Dispatch the message to the window object that it belongs to
	 * @param   hWnd    The handle to the window which receives the message
	 * @param   uMsg    Message type
	 * @param   wParam  Message data
	 * @param   lParam  Additional message data
	 * @return  Result of message process. Depends on message type
	 */
	static LRESULT MessageRouter(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

	/**
	 * Dispatch window message to message handlers
	 * @param   hWnd    Handle to the window
	 * @param   uMsg    Message type
	 * @param   wParam  Extra message parameter
	 * @param   lParam  Extra message parameter
	 * @return  If message is handled, non-zero is returned. Otherwise FALSE is returned and message is passed to default dialog procedure
	 */
	virtual LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam) = 0;

	/**
	 * Returns the ID of the dialog
	 * @return  ID of dialog
	 */
	virtual UINT GetDlgId() = 0;

	/**
	 * Set the icon of the given window
	 * @param  hWnd  Handle to the window
	 */
	static void SetIcon(HWND hWnd);

protected:
	HWND                m_hWnd;
	const   NuiViewer*  m_pParent;
	static  HICON       SensorIcon;
};