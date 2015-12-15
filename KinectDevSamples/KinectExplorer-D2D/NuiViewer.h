//------------------------------------------------------------------------------
// <copyright file="NuiViewer.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <Windows.h>

//Forward declaration to avoid header file cross reference conflict
class KinectWindow;

class NuiViewer
{
public:
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pParent">The pointer to parent window</param>
    NuiViewer(const NuiViewer* pParent);

public:
    /// <summary>
    /// Create window of viewer
    /// </sumarry>
    /// <returns>Indicates success or failure</returns>
    virtual bool CreateView();

    /// <summary>
    /// Shows up the window of the viewer
    /// </summary>
    virtual void ShowView();

    /// <summary>
    /// Hide the window of the viewer
    /// </summary>
    virtual void HideView();

    /// <summary>
    /// Returns the window handle
    /// </summary>
    /// <returns>Handle to the window</returns>
    virtual HWND GetWindow() const;

    /// <summary>
    /// Set window with position and size
    /// </summary>
    /// <param name="rect">New position and size</param>
    /// <returns>Indicate success or failure</returns>
    virtual bool MoveView(const RECT& rect);

protected:
    /// <summary>
    /// Dispatch the message to the window object that it belongs to
    /// </summary>
    /// <param name="hWnd">The handle to the window which receives the message</param>
    /// <param name="uMsg">Message type</param>
    /// <param name="wParam">Message data</param>
    /// <param name="lParam">Additional message data</param>
    /// <returns>Result of message process. Depends on message type</returns>
    static LRESULT MessageRouter(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

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
    virtual LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam) = 0;

    /// <summary>
    /// Returns the ID of the dialog
    /// </summary>
    /// <returns>ID of dialog</returns>
    virtual UINT GetDlgId() = 0;

    /// <summary>
    /// Set the icon of the given window
    /// </summary>
    /// <param name="hWnd">Handle to the window</param>
    static void SetIcon(HWND hWnd);

protected:
            HWND        m_hWnd;
    const   NuiViewer*  m_pParent;
    static  HICON       SensorIcon;
};