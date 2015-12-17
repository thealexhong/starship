/**
* MainWindow.h
* Modfied from Kinect SDK v.1.8
* alex.hong@mail.utoronto.ca
*/

#pragma once

#include "KinectWindowManager.h"
#include "CustomDrawListControl.h"
#include "NuiViewer.h"
#include "resource.h"

/**
 * Helper class to calculate the position rectangle inside the parent window
 */
struct ClientRect : public RECT
{
    ClientRect(HWND hWnd, HWND hParent) : m_hWnd(hWnd), m_hParent(hParent)
    {
        GetClientRect(hWnd, this);
        MapWindowPoints(hWnd, m_hParent, (LPPOINT)this, sizeof(RECT) / sizeof(POINT));
    }

    RECT GetMargins()
    {
        RECT parentRect;
        GetClientRect(m_hParent, &parentRect);

        RECT marginRect;
        SetRect(&marginRect,
            left - parentRect.left,
            top - parentRect.top,
            parentRect.right - right,
            parentRect.bottom - bottom
            );

        return marginRect;
    }

    LONG GetWidth()
    {
        return right - left;
    }

    LONG GetHeight()
    {
        return bottom - top;
    }

private:

    HWND m_hWnd;
    HWND m_hParent;
};

/**
 * The main window is used to manage the Kinect status changes,
 * and mainly contains device view and log view.
 * Each connected sensor will have an entry in the device view,
 * and the device's status is indicated in the log view.
 */
class CMainWindow : public NuiViewer
{
public:

    CMainWindow();

    ~CMainWindow();

    
    /**
	 * Handle windows messages for a class instance
     * @param   hWnd    Window handler
     * @param   uMsg    Message
     * @param   wParam  Message data
     * @param   lParam  Additional message data
     * @return  result of message processing
	 */
    LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

    
    /**
	 * Creates the main window and begins processing
	 */
    int Run();

private:

    
    /**
	 * This function will be called when Kinect device status changed
	 */
    static void CALLBACK StatusChangeCallback(HRESULT hrStatus, const OLECHAR* instanceName, const OLECHAR* uniqueDeviceName, void* pUserData);

    
    /**
	 * Show all the kinect windows
	 */
    void ShowAllKinectWindows();

    
    /**
	 * Handle WM_SIZE message to auto layout all the controls in the window based
     * on the new window size.
	 */
    void Resize();

    
    /**
	 * This method will initialize all the members and enumerate all the sensors
	 */
    void InitializeResource();

    
    /**
	 * Enumerate and construct all the sensors when the app starts up
	 */
    void EnumerateSensors();

    
    /**
	 * Update the main window status
	 */
    void UpdateMainWindow(PCWSTR instanceName, HRESULT hrStatus);

    
    /**
	 * Respond to the click event of "moreinfo" link
	 */
    void OnClickMoreInfoLink(LPARAM lParam);

    
    /**
	 * Draw the break line between the two list controls
	 */
    void DrawBreakLine();

    
    /**
	 * Update the window layout and show/hide status of the controls
	 */
    void UpdateLayoutAndShowStatus();

    
    /**
	 * Get the handle of the specified child control
	 */
    HWND GetHandle(UINT controlId) { return GetDlgItem(m_hWnd, controlId); }

    
    /**
	 * Update the window layout
	 */
    void UpdateLayout();

    
    /**
	 * Show/Hide the controls
	 */
    void UpdateShowStatus();

    
    /**
	 * Returns the ID of the dialog
     */
    UINT GetDlgId()
    {
        return IDD_KEMAINWINDOW;
    }

    
    /**
	 * Check whether Kinect window manager has any sensor
	 */
    bool HasSensor()
    {
        return m_pKinectWindowMgr->GetSensorCount() > 0;
    }

private:

    HWND                            m_hWnd;
    KinectWindowManager*            m_pKinectWindowMgr;
    SensorListControl*              m_pSensorListControl;
    StatusLogListControl*           m_pStatusLogListControl;

    // Save the minimum width of the main window
    int m_minTrackWidth;

    // Save url of "MoreInfo" link which is showed in no sensor case
    WCHAR m_moreInfoLinkUrl[MaxStringChars];

    static HFONT LargeTextFont;

    // Define some parameters for layout
    static const int RightMargin   = 25;
    static const int StretchMargin = 50;
    static const int GenericGap    = 8;
    static const int BottomMargin  = 20;
};
