//------------------------------------------------------------------------------
// <copyright file="NuiAudioViewer.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <limits>
#include "NuiAudioViewer.h"
#include "resource.h"

/// <summary>
/// Constructor
/// </summary>
/// <param name="pParent">The pointer to parent window</param>
NuiAudioViewer::NuiAudioViewer(const NuiViewer* pParent)
    : NuiViewer(pParent)
    , m_beamAngle(FLT_MAX)
    , m_sourceAngle(FLT_MAX)
    , m_sourceConfidence(FLT_MAX)
{
}

/// <summary>
/// Destructor
/// </summary>
NuiAudioViewer::~NuiAudioViewer()
{
}

/// <summary>
/// Dispatch window message to message handlers.
/// </summary>
/// <param name="hWnd">Handle to window</param>
/// <param name="uMsg">Message type</param>
/// <param name="wParam">Extra message parameter</param>
/// <param name="lParam">Extra message parameter</param>
LRESULT NuiAudioViewer::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    // Pass the message to default dialog procedure
    return FALSE;
}

/// <summary>
/// Returns the ID of the dialog
/// </summary>
UINT NuiAudioViewer::GetDlgId()
{
    return IDD_AUDIO_VIEW;
}

/// <summary>
/// Set and update audio readings to display
/// </summary>
/// <param name="beamAngle">Beam angle reading</param>
/// <param name="sourceAngle">Source angle reading</param>
/// <param name="sourceConfidence">Source confidence reading</param>
void NuiAudioViewer::SetAudioReadings(double beamAngle, double sourceAngle, double sourceConfidence)
{
    // Format the readings and update to static control
    CompareUpdateValue(beamAngle, m_beamAngle, m_hWnd, IDC_AUDIO_BEAM_ANGLE_READING, L"%.2f");
    CompareUpdateValue(sourceAngle, m_sourceAngle, m_hWnd, IDC_AUDIO_SOURCE_ANGLE_READING, L"%.2f");
    CompareUpdateValue(sourceConfidence, m_sourceConfidence, m_hWnd, IDC_AUDIO_SOURCE_CONFIDENCE_READING, L"%.2f");
}