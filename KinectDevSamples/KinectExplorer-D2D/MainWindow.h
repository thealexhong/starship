//------------------------------------------------------------------------------
// <copyright file="MainWindow.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include "KinectWindowManager.h"
#include "CustomDrawListControl.h"
#include "NuiViewer.h"
#include "resource.h"

/// <summary>
/// Helper class to calculate the position rectangle inside the parent window
/// </summary>
struct ClientRect : public RECT
{
    ClientRect(HWND hWnd, HWND hParent)
        : m_hWnd(hWnd)
        , m_hParent(hParent)
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

/// <summary>
/// The main window is used to manage the Kinect status changes,
/// and mainly contains device view and log view.
/// Each connected sensor will have an entry in the device view,
/// and the device's status is indicated in the log view.
/// </summary>
class CMainWindow : public NuiViewer
{
public:

    CMainWindow();

    ~CMainWindow();

    /// <summary>
    /// Handle windows messages for a class instance
    /// </summary>
    /// <param name="hWnd">Window message is for</param>
    /// <param name="uMsg">Message</param>
    /// <param name="wParam">Message data</param>
    /// <param name="lParam">Additional message data</param>
    /// <returns>result of message processing</returns>
    LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

    /// <summary>
    /// Creates the main window and begins processing
    /// </summary>
    int                     Run();

private:

    /// <summary>
    /// This function will be called when Kinect device status changed
    /// </summary>
    static void CALLBACK StatusChangeCallback(HRESULT hrStatus, const OLECHAR* instanceName, const OLECHAR* uniqueDeviceName, void* pUserData);

    /// <summary>
    /// Show all the kinect windows
    /// </summary>
    void ShowAllKinectWindows();

    /// <summary>
    /// Handle WM_SIZE message to auto layout all the controls in the window based
    /// on the new window size.
    /// </summary>
    void Resize();

    /// <summary>
    /// This method will initialize all the members and enumerate all the sensors
    /// </summary>
    void InitializeResource();

    /// <summary>
    /// Enumerate and construct all the sensors when the app starts up
    /// </summary>
    void EnumerateSensors();

    /// <summary>
    /// Update the main window status
    /// </summary>
    void UpdateMainWindow(PCWSTR instanceName, HRESULT hrStatus);

    /// <summary>
    /// Respond to the click event of "moreinfo" link
    /// </summary>
    void OnClickMoreInfoLink(LPARAM lParam);

    /// <summary>
    /// Draw the break line between the two list controls
    /// </summary>
    void DrawBreakLine();

    /// <summary>
    /// Update the window layout and show/hide status of the controls
    /// </summary>
    void UpdateLayoutAndShowStatus();

    /// <summary>
    /// Get the handle of the specified child control
    /// </summary>
    HWND GetHandle(UINT controlId) { return GetDlgItem(m_hWnd, controlId); }

    /// <summary>
    /// Update the window layout
    /// </summary>
    void UpdateLayout();

    /// <summary>
    /// Show/Hide the controls
    /// </summary>
    void UpdateShowStatus();

    /// <summary>
    /// Returns the ID of the dialog
    /// </summary>
    UINT GetDlgId()
    {
        return IDD_KEMAINWINDOW;
    }

    /// <summary>
    /// Check whether Kinect window manager has any sensor
    /// </summary>
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
    static const int RightMargin = 25;
    static const int StretchMargin = 50;
    static const int GenericGap = 8;
    static const int BottomMargin = 20;
};
