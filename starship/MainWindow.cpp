//------------------------------------------------------------------------------
// <copyright file="MainWindow.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"

#include "MainWindow.h"
#include "Utility.h"

//Define the global independent Direct resources
ID2D1Factory* g_pD2DFactory = nullptr;
IDWriteFactory* g_pDWriteFactory = nullptr;

/// <summary>
/// Ensure the independent Direct2D resources have been created
/// </summary>
void EnsureIndependentResourcesCreated()
{
    if (nullptr == g_pD2DFactory)
    {
        D2D1CreateFactory(D2D1_FACTORY_TYPE_MULTI_THREADED, &g_pD2DFactory);
    }

    if (nullptr == g_pDWriteFactory)
    {
        DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(IDWriteFactory), reinterpret_cast<IUnknown**>(&g_pDWriteFactory));
    }
}

/// <summary>
/// Release the independent Direct2D resources
/// </summary>
void DiscardIndependentResources()
{
    SafeRelease(g_pDWriteFactory);
    SafeRelease(g_pD2DFactory);
}

/// <summary>
/// Entry point for the application
/// </summary>
/// <param name="hInstance">Handle to the application instance</param>
/// <param name="hPrevInstance">Always 0</param>
/// <param name="lpCmdLine">Command line arguments</param>
/// <param name="nCmdShow">Whether to display minimized, maximized, or normally</param>
/// <returns>status</returns>
int APIENTRY wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{
    EnsureIndependentResourcesCreated();

    CMainWindow application;
    int result = application.Run();

    DiscardIndependentResources();

    return result;
}

// ---------------------------------------------------------------------------
//
// Class CMainWindow
//
// ---------------------------------------------------------------------------

HFONT CMainWindow::LargeTextFont;

/// <summary>
/// Constructor
/// </summary>
CMainWindow::CMainWindow()
    : NuiViewer(nullptr)
    , m_hWnd(nullptr)
    , m_pKinectWindowMgr(nullptr)
    , m_pSensorListControl(nullptr)
    , m_pStatusLogListControl(nullptr)
{
    EnsureFontCreated(LargeTextFont, 25, FW_MEDIUM);
}

/// <summary>
/// Deconstructor
/// </summary>
CMainWindow::~CMainWindow()
{
    SafeDelete(m_pKinectWindowMgr);
    SafeDelete(m_pSensorListControl);
    SafeDelete(m_pStatusLogListControl);
}

/// <summary>
/// Creates the main window and begins processing
/// </summary>
int CMainWindow::Run()
{
    // Create main application window
    CreateView();

    // Show the main window
    ShowView();

    // Show the kinect windows
    ShowAllKinectWindows();

    // Main message loop
    MSG msg = {0};

    while (WM_QUIT != msg.message)
    {
        if (GetMessageW(&msg, nullptr, 0, 0))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return static_cast<int>(msg.wParam);
}

/// <summary>
/// Handle windows messages for a class instance
/// </summary>
/// <param name="hWnd">Window message is for</param>
/// <param name="uMsg">Message</param>
/// <param name="wParam">Message data</param>
/// <param name="lParam">Additional message data</param>
/// <returns>result of message processing</returns>
LRESULT CMainWindow::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch (uMsg)
    {
    case WM_INITDIALOG:
        {
            // Bind application window handle
            m_hWnd = hWnd;

            // Set device status callback to monitor all sensor changes
            NuiSetDeviceStatusCallback(StatusChangeCallback, reinterpret_cast<void*>(hWnd));

            InitializeResource();
        }
        break;

    case WM_CTLCOLORSTATIC:
    case WM_CTLCOLORDLG:
        return (LRESULT)GetStockObject(WHITE_BRUSH);

    case WM_NOTIFY:
        switch (LOWORD(wParam))
        {
        case IDC_MOREINFOLINK:
            OnClickMoreInfoLink(lParam);
            break;

        case IDC_KINECTSENSORLIST:
            return m_pSensorListControl->HandleNotifyMessage(hWnd, (LPNMLISTVIEW)lParam);

        case IDC_STATUSLOGLIST:
            return m_pStatusLogListControl->HandleNotifyMessage(hWnd, (LPNMLISTVIEW)lParam);
        }
        break;

        // If the titlebar X is clicked, destroy the app
    case WM_CLOSE:
        {
            m_pKinectWindowMgr->CloseAllKinectWindows();
            DestroyWindow(hWnd);
        }
        break;

        // Quit the main message pump
    case WM_DESTROY:
        PostQuitMessage(0);
        break;

        // Handle the Kinect sensor status change case
    case WM_UPDATEMAINWINDOW:
        {
            UpdateMainWindow((PCWCHAR)wParam, (HRESULT)lParam);
            UpdateLayoutAndShowStatus();
        }
        break;

        // If the kinect window is closed, notify Kinect window manager
    case WM_CLOSEKINECTWINDOW:
        m_pKinectWindowMgr->ResetKinectWindow((PCWSTR)wParam);
        break;

    case WM_SIZE:
        Resize();
        break;

    case WM_PAINT:
        DrawBreakLine();
        break;

    case WM_GETMINMAXINFO:
        {
            POINT minTrackSize = {m_minTrackWidth, 0};

            auto pMinMaxInfo = (PMINMAXINFO)lParam;
            pMinMaxInfo->ptMinTrackSize = minTrackSize;
        }
        break;
    }

    return FALSE;
}

/// <summary>
/// This function will be called when Kinect device status changed
/// </summary>
void CALLBACK CMainWindow::StatusChangeCallback(
    HRESULT hrStatus,
    const OLECHAR* instanceName,
    const OLECHAR* uniqueDeviceName,
    void* pUserData)
{
    HWND hWnd = reinterpret_cast<HWND>(pUserData);

    if (nullptr != hWnd)
    {
        SendMessageW(hWnd, WM_UPDATEMAINWINDOW, (WPARAM)instanceName, (LPARAM)hrStatus);
    }
}

/// <summary>
/// Show all the kinect windows
/// </summary>
void CMainWindow::ShowAllKinectWindows()
{
    // Show windows
    m_pKinectWindowMgr->ShowAllKinectWindows();

    // Set the show parameter
    static const DWORD ShowWindowParam = 1;
    m_pKinectWindowMgr->SetKinectWindowShowParam(ShowWindowParam);
}

/// <summary>
/// Handle WM_SIZE message to auto layout all the controls in the window based
/// on the new window size.
/// </summary>
void CMainWindow::Resize()
{
    // Rearrange the layout of the main window
    UpdateLayout();

    // Rearrange the columns of the list control
    m_pSensorListControl->HandleResizeMessage();
    m_pStatusLogListControl->HandleResizeMessage();
}

/// <summary>
/// This method will construct all the member classes and enumerate all the sensors
/// </summary>
void CMainWindow::InitializeResource()
{
    // Set the dialog icon
    NuiViewer::SetIcon(m_hWnd);

    // Set window minimum track size
    m_minTrackWidth = GetWindowSize(m_hWnd).cx;

    // Load url string
    LoadStringW(GetModuleHandle(0), IDS_MOREINFOURL, m_moreInfoLinkUrl, MaxStringChars);

    // Set text font style
    SendMessageW(GetHandle(IDC_CONNECTEDDEVICESTEXT), WM_SETFONT, (WPARAM)LargeTextFont, 0);
    SendMessageW(GetHandle(IDC_REQUIREDSENSORTEXT), WM_SETFONT, (WPARAM)LargeTextFont, 0);
    SendMessageW(GetHandle(IDC_STATUSTEXT), WM_SETFONT, (WPARAM)LargeTextFont, 0);

    // Set the logo picture
    HBITMAP hLogoImage = nullptr;
    EnsureImageLoaded(hLogoImage, IDB_LOGO);
    SendMessageW(GetHandle(IDC_LOGOPICTURE), STM_SETIMAGE, IMAGE_BITMAP, (LPARAM)hLogoImage);

    // Create sensor list instance
    m_pSensorListControl = new SensorListControl(GetHandle(IDC_KINECTSENSORLIST));

    // Create status log list instance
    m_pStatusLogListControl = new StatusLogListControl(GetHandle(IDC_STATUSLOGLIST));

    // Create kinect window manager instance
    m_pKinectWindowMgr = new KinectWindowManager(m_hWnd);

    // Construct all the sensors
    EnumerateSensors();

    // Upate layout and show/hide status
    UpdateLayoutAndShowStatus();
}

/// <summary>
/// Enumerate and construct all the sensors when the app starts up
/// </summary>
void CMainWindow::EnumerateSensors()
{
    int iCount = 0;
    HRESULT hr = NuiGetSensorCount(&iCount);
    if (FAILED(hr))
    {
        return;
    }

    for (int i = 0; i < iCount; ++i)
    {
        INuiSensor* pNuiSensor = nullptr;

        if (SUCCEEDED(NuiCreateSensorByIndex(i, &pNuiSensor)))
        {
            UpdateMainWindow(pNuiSensor->NuiDeviceConnectionId(), pNuiSensor->NuiStatus());
        }

        SafeRelease(pNuiSensor);
    }
}

/// <summary>
/// Update the main window status
/// </summary>
void CMainWindow::UpdateMainWindow(PCWSTR instanceName, HRESULT sensorStatus)
{
    // The new status is "not connected"
    if (E_NUI_NOTCONNECTED == sensorStatus)
    {
        m_pKinectWindowMgr->HandleSensorDisconnected(instanceName);

        // Remove the corresponding sensor item from sensor list control
        m_pSensorListControl->RemoveRow(instanceName);
    }
    else
    {
        m_pKinectWindowMgr->HandleSensorConnected(instanceName, sensorStatus);

        // Update the sensor list control
        m_pSensorListControl->InsertOrUpdateSensorStatus(instanceName, sensorStatus);
    }

    // Insert the new log item to status log list
    m_pStatusLogListControl->AddLog(instanceName, sensorStatus);
}

/// <summary>
/// Update the window layout and show/hide status of the controls
/// </summary>
void CMainWindow::UpdateLayoutAndShowStatus()
{
    // Update layout
    UpdateLayout();

    // Update show status
    UpdateShowStatus();
}

/// <summary>
/// Respond to the click event of "moreinfo" link
/// </summary>
void CMainWindow::OnClickMoreInfoLink(LPARAM lParam)
{
    PNMLINK pnm = (PNMLINK)lParam;

    switch (pnm->hdr.code)
    {
    case NM_CLICK:
    case NM_RETURN:
        {
            if ((GetHandle(IDC_MOREINFOLINK) == pnm->hdr.hwndFrom) && (0 == pnm->item.iLink))
            {
                ShellExecute(0, 0, m_moreInfoLinkUrl, 0, 0, SW_SHOWNORMAL);
            }
            break;
        }
    }
}

/// <summary>
/// Draw the break line between the two list controls
/// </summary>
void CMainWindow::DrawBreakLine()
{
    PAINTSTRUCT ps;
    BeginPaint(m_hWnd, &ps);

    HWND hBreakLine = GetDlgItem(m_hWnd, IDC_BREAKLINE);
    HDC hdcBreakLine = GetDC(hBreakLine);
    RECT rc;
    GetClientRect(hBreakLine, &rc);

    // Fill the background rect
    FillRect(hdcBreakLine, &rc, (HBRUSH)GetStockObject(WHITE_BRUSH));
    // Draw the line
    MoveToEx(hdcBreakLine, 0, 0, 0);
    LineTo(hdcBreakLine, rc.right, 0);

    EndPaint(m_hWnd, &ps);
    ReleaseDC(hBreakLine, hdcBreakLine);
}

/// <summary>
/// Update the window layout
/// </summary>
void CMainWindow::UpdateLayout()
{
    SIZE windowSize = GetClientSize(m_hWnd);

    // Right align the text "Explorer Connected Devices"
    ClientRect upperRightTextRect(GetHandle(IDC_CONNECTEDDEVICESTEXT), m_hWnd);
    SetWindowPos(GetHandle(IDC_CONNECTEDDEVICESTEXT), 0, windowSize.cx - upperRightTextRect.GetWidth() - RightMargin, upperRightTextRect.top, 0, 0, SWP_NOSIZE);

    // Stretch align the sensor list control
    ClientRect sensorListRect(GetHandle(IDC_KINECTSENSORLIST), m_hWnd);
    SetWindowPos(GetHandle(IDC_KINECTSENSORLIST), 0, 0, 0, windowSize.cx - StretchMargin, sensorListRect.GetHeight(), SWP_NOMOVE);

    // Stretch align the break line
    LONG breakLineHeight = GetClientSize(GetHandle(IDC_BREAKLINE)).cy;
    SetWindowPos(GetHandle(IDC_BREAKLINE), 0, sensorListRect.left, sensorListRect.bottom + GenericGap, windowSize.cx - StretchMargin, breakLineHeight, 0);

    // Left align the text "Status"
    int breakLineBottom = sensorListRect.bottom + GenericGap + breakLineHeight;
    ClientRect statusTextRect(GetHandle(IDC_STATUSTEXT), m_hWnd);
    SetWindowPos(GetHandle(IDC_STATUSTEXT), 0, statusTextRect.left, breakLineBottom + GenericGap, 0, 0, SWP_NOSIZE);

    // Stretch align the status log list control
    int statusTextBottom = breakLineBottom + GenericGap + statusTextRect.GetHeight();
    LONG statusListHeight = max(windowSize.cy - statusTextBottom - GenericGap - BottomMargin, 0);
    SetWindowPos(GetHandle(IDC_STATUSLOGLIST), 0, sensorListRect.left, statusTextBottom + GenericGap, windowSize.cx - StretchMargin, statusListHeight, 0);
}

/// <summary>
/// Show/Hide the controls
/// </summary>
void CMainWindow::UpdateShowStatus()
{
    int hasSensorCmdShow = (HasSensor()) ? SW_SHOW : SW_HIDE;
    int noSensorCmdShow = (SW_SHOW == hasSensorCmdShow) ? SW_HIDE : SW_SHOW;

    // If there is no connected sensor, show these controls
    ShowWindow(GetHandle(IDC_MOREINFOLINK), noSensorCmdShow);
    ShowWindow(GetHandle(IDC_NOSENSORTEXT), noSensorCmdShow);
    ShowWindow(GetHandle(IDC_REQUIREDSENSORTEXT), noSensorCmdShow);

    // If there is any connected sensor, show these controls
    ShowWindow(GetHandle(IDC_KINECTSENSORLIST), hasSensorCmdShow);
    ShowWindow(GetHandle(IDC_STATUSLOGLIST), hasSensorCmdShow);
    ShowWindow(GetHandle(IDC_BREAKLINE), hasSensorCmdShow);
    ShowWindow(GetHandle(IDC_STATUSTEXT), hasSensorCmdShow);
}