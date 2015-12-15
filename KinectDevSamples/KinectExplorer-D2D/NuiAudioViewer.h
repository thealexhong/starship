//------------------------------------------------------------------------------
// <copyright file="NuiAudioViewer.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include "Utility.h"
#include "NuiViewer.h"

class KinectWindow;

class NuiAudioViewer : public NuiViewer
{
public:
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pParent">The pointer to parent window</param>
    NuiAudioViewer(const NuiViewer* pParent);

    /// <summary>
    /// Destructor
    /// </summary>
   ~NuiAudioViewer();

public:
    /// <summary>
    /// Set and update audio readings to display
    /// </summary>
    /// <param name="beamAngle">Beam angle reading</param>
    /// <param name="sourceAngle">Source angle reading</param>
    /// <param name="sourceConfidence">Source confidence reading</param>
    void SetAudioReadings(double beamAngle, double sourceAngle, double sourceConfidence);

private:
    /// <summary>
    /// Returns the ID of the dialog
    /// </summary>
    /// <returns>ID of dialog</returns>
    virtual UINT GetDlgId();

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
    virtual LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

private:
    double  m_beamAngle;
    double  m_sourceAngle;
    double  m_sourceConfidence;
};