//------------------------------------------------------------------------------
// <copyright file="NuiViewer.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include "NuiViewer.h"
#include "KinectWindow.h"
#include "resource.h"

HICON NuiViewer::SensorIcon;

/// <summary>
/// Constructor
/// </summary>
/// <param name="pParent">The pointer to parent window</param>
NuiViewer::NuiViewer(const NuiViewer* pParent)
    : m_hWnd(nullptr)
    , m_pParent(pParent)
{
    EnsureIconLoaded(SensorIcon, IDI_APP);
}

/// <summary>
/// Create window of viewer
/// </sumarry>
/// <returns>Indicates success or failure</returns>
bool NuiViewer::CreateView()
{
    if (!m_hWnd)
    {
        HWND hWndParent = nullptr;
        if (m_pParent)
        {
            hWndParent = m_pParent->GetWindow();
        }

        m_hWnd = CreateDialogParamW(nullptr,
                                    MAKEINTRESOURCE(GetDlgId()),
                                    hWndParent,
                                    (DLGPROC)MessageRouter,
                                    reinterpret_cast<LPARAM>(this));
    }

    return (nullptr != m_hWnd);
}

/// <summary>
/// Dispatch the message to the window object that it belongs to
/// </summary>
/// <param name="hWnd">The handle to the window which receives the message</param>
/// <param name="uMsg">Message type</param>
/// <param name="wParam">Message data</param>
/// <param name="lParam">Additional message data</param>
/// <returns>Result from message process</returns>
LRESULT NuiViewer::MessageRouter(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    NuiViewer* pThis = nullptr;

    if (WM_INITDIALOG == uMsg)
    {
        pThis = reinterpret_cast<NuiViewer*>(lParam);
        SetWindowLongPtrW(hWnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(pThis));
    }
    else
    {
        pThis = reinterpret_cast<NuiViewer*>(::GetWindowLongPtr(hWnd, GWLP_USERDATA));
    }

    if (pThis)
    {
        return (pThis->DialogProc(hWnd, uMsg, wParam, lParam));
    }

    return (LRESULT)FALSE;
}

/// <summary>
/// Shows up the window of the viewer
/// </summary>
void NuiViewer::ShowView()
{
    if (m_hWnd)
    {
        ShowWindow(m_hWnd, SW_SHOW);
        UpdateWindow(m_hWnd);
    }
}

/// <summary>
/// Hide the window of the viewer
/// </summary>
void NuiViewer::HideView()
{
    if (m_hWnd)
    {
        ShowWindow(m_hWnd, SW_HIDE);
    }
}

/// <summary>
/// Returns the window handle
/// </summary>
/// <returns>Handle to the window</returns>
HWND NuiViewer::GetWindow() const
{
    return m_hWnd;
}

/// <summary>
/// Set window with position and size
/// </summary>
/// <param name="rect">New position and size</param>
bool NuiViewer::MoveView(const RECT& rect)
{
    if (m_hWnd)
    {
        return FALSE != MoveWindow(m_hWnd,
                                   rect.left,
                                   rect.top,
                                   rect.right - rect.left,
                                   rect.bottom - rect.top,
                                   TRUE);
    }

    return false;
}

/// <summary>
/// Set the icon of the given window
/// </summary>
/// <param name="hWnd">Handle to the window</param>
void NuiViewer::SetIcon(HWND hWnd)
{
    if (nullptr != hWnd)
    {
        SendMessageW(hWnd, WM_SETICON, (WPARAM)ICON_SMALL, (LPARAM)SensorIcon);
    }
}
