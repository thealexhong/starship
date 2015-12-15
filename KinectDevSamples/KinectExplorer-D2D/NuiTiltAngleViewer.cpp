//------------------------------------------------------------------------------
// <copyright file="NuiTiltAngleViewer.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <limits>
#include "NuiTiltAngleViewer.h"
#include "resource.h"

/// <summary>
/// Coerce the requested elevation angle to a valid angle
/// </summary>
/// <param name="tiltAngle">The requested angle</param>
/// <returns>The angle after coerced</returns>
inline LONG CoerceElevationAngle(LONG tiltAngle)
{
    return min(max(NUI_CAMERA_ELEVATION_MINIMUM, tiltAngle), NUI_CAMERA_ELEVATION_MAXIMUM);
}

/// <summary>
/// Constructor
/// </summary>
/// <param name="pParent">The pointer to parent window</param>
/// <param name="pNuiSensor">The pointer to Nui sensor device instance</param>
NuiTiltAngleViewer::NuiTiltAngleViewer(const NuiViewer* pParent, INuiSensor* pNuiSensor)
    : NuiViewer(pParent)
    , m_pNuiSensor(pNuiSensor)
    , m_tiltAngle(LONG_MAX)
    , m_hElevationTaskThread(nullptr)
{
    if (m_pNuiSensor)
    {
        m_pNuiSensor->AddRef();
    }

    // Start the elevation task thread
    StartElevationThread();
}

/// <summary>
/// Destructor
/// </summary>
NuiTiltAngleViewer::~NuiTiltAngleViewer()
{
    CleanUp();
}

/// <summary>
/// Dispatch window message to message handlers.
/// </summary>
/// <param name="hWnd">Handle to window</param>
/// <param name="uMsg">Message type</param>
/// <param name="wParam">Extra message parameter</param>
/// <param name="lParam">Extra message parameter</param>
/// <returns>
/// If message is handled, non-zero is returned. Otherwise FALSE is returned and message is passed to default dialog procedure
/// </returns>
LRESULT NuiTiltAngleViewer::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch (uMsg)
    {
    case WM_VSCROLL:
        OnSliderScroll(hWnd, wParam, lParam);
        break;

    case WM_COMMAND:
        OnCommand(hWnd, wParam);
        break;

    case WM_INITDIALOG:
        OnInit(hWnd);
        break;

    default:
        break;
    }

    return FALSE;
}

/// <summary>
/// Handler for WM_INITDIALOG message
/// </summary>
/// <param name="hWnd">The handle to the window which receives the message</param>
void NuiTiltAngleViewer::OnInit(HWND hWnd)
{
    HWND hWndSlider = GetDlgItem(hWnd, IDC_TILTANGLE_SLIDER);
    if (hWndSlider)
    {
        // Bind the window handle
        m_hWnd = hWnd;

        // Set slider's range
        SendMessageW(hWndSlider, TBM_SETRANGE, TRUE, MAKELPARAM(0, NUI_CAMERA_ELEVATION_MAXIMUM - NUI_CAMERA_ELEVATION_MINIMUM));

        // Update the text interpretation
        LONG degree = 0;
        if (SUCCEEDED(m_pNuiSensor->NuiCameraElevationGetAngle(&degree)))
        {
            CompareUpdateValue(CoerceElevationAngle(degree), m_tiltAngle, hWnd, IDC_TILTANGLE_READING, L"%d\x00B0");
        }

        // Set slider's initial position
        SendMessageW(hWndSlider, TBM_SETPOS, TRUE, (LPARAM)(NUI_CAMERA_ELEVATION_MAXIMUM - degree));
    }
}

/// <summary>
/// Handler for WM_COMMAND message
/// </summary>
/// <param name="hWnd">The handle to the window which receives the message</param>
/// <param name = "wParam">Command parameter</param>
void NuiTiltAngleViewer::OnCommand(HWND hWnd, WPARAM wParam)
{
    WORD id   = LOWORD(wParam);
    WORD code = HIWORD(wParam);
    if (IDC_FORCE_OFF_IR == id && BN_CLICKED == code)
    {
        bool checked = (BST_CHECKED == IsDlgButtonChecked(hWnd, id));
        PostMessageW(m_pParent->GetWindow(), WM_COMMAND, MAKEWPARAM(ID_FORCE_OFF_IR, checked ? 1 : 0), 0);
    }
}


/// <summary>
/// Handler for slider control scroll event
/// </summary>
/// <param name="hWnd">The handle to the window which receives the message</param>
/// <param name="wParam">HIWORD specifies the current position of the slider if LOWORD is SB_THUMBPOSIION or SB_THUMBTRACK. Otherwise not used</param>
/// <param name="lParam">If the message is sent by a scroll bar, this parameter is the handle to the scroll bar control. If the message is not sent by a scroll bar, this parameter is NULL</param>
void NuiTiltAngleViewer::OnSliderScroll(HWND hWnd, WPARAM wParam, LPARAM lParam)
{
    HWND hWndSlider = GetDlgItem(hWnd, IDC_TILTANGLE_SLIDER);
    if (hWndSlider && hWndSlider == (HWND)lParam)
    {
        WORD lo = LOWORD(wParam);
        if (TB_ENDTRACK == lo) // Mouse button released
        {
            LONG trackValue = (LONG)SendMessageW(hWndSlider, TBM_GETPOS, 0, 0);
            LONG degree = NUI_CAMERA_ELEVATION_MAXIMUM - trackValue;

            // Update the text interpretation
            CompareUpdateValue(CoerceElevationAngle(degree), m_tiltAngle, hWnd, IDC_TILTANGLE_READING, L"%d\x00B0");

            // Resume the elevation task thread to set tilt angle
            SetEvent(m_hSetTiltAngleEvent);
        }
    }
}

/// <summary>
/// Returns the ID of the dialog
/// </summary>
UINT NuiTiltAngleViewer::GetDlgId()
{
    return IDD_TILTANGLE_VIEW;
}

/// <summary>
/// Start a new thread to run the elevation task
/// </summary>
void NuiTiltAngleViewer::StartElevationThread()
{
    // Create the events for elevation task thread
    m_hSetTiltAngleEvent = CreateEventW(nullptr, FALSE, FALSE, nullptr);
    m_hExitThreadEvent = CreateEventW(nullptr, TRUE, FALSE, nullptr);

    m_hElevationTaskThread = CreateThread(
        nullptr,
        0,
        (LPTHREAD_START_ROUTINE)NuiTiltAngleViewer::ThreadProc,
        (LPVOID)this,
        0,
        nullptr);
}

/// <summary>
/// The thread procedure in which the elevation task runs
/// </summary>
/// <param name="pThis">The pointer to NuiTiltAngleViewer instance</param>
DWORD WINAPI NuiTiltAngleViewer::ThreadProc(NuiTiltAngleViewer* pThis)
{
    const int numEvents = 2;
    HANDLE events[numEvents] = {pThis->m_hSetTiltAngleEvent, pThis->m_hExitThreadEvent};

    while(true)
    {
        // Check if we have a setting tilt angle event or an exiting thread event
        DWORD dwEvent = WaitForMultipleObjects(numEvents, events, FALSE, INFINITE);

        if (WAIT_OBJECT_0 == dwEvent)
        {
            // Set the tilt angle
            pThis->m_pNuiSensor->NuiCameraElevationSetAngle(pThis->m_tiltAngle);
        }
        else if (WAIT_OBJECT_0 + 1 == dwEvent)
        {
            // Close the handles and exit the thread
            CloseHandle(pThis->m_hSetTiltAngleEvent);
            CloseHandle(pThis->m_hExitThreadEvent);

            break;
        }
        else
        {
            return 1;
        }
    }

    return 0;
}

/// <summary>
/// Release all the resources
/// </summary>
void NuiTiltAngleViewer::CleanUp()
{
    // Exit the elevation task thread
    SetEvent(m_hExitThreadEvent);

    // Wait for the elevation task thread
    WaitForSingleObject(m_hElevationTaskThread, INFINITE);

    SafeRelease(m_pNuiSensor);

    if (m_hElevationTaskThread)
    {
        CloseHandle(m_hElevationTaskThread);
    }
}
