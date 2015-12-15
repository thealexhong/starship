//------------------------------------------------------------------------------
// <copyright file="CameraSettingsViewer.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"

#include "CameraSettingsViewer.h"
#include "Utility.h"

LONG CameraSettingsViewer::TrackBarRange = 0;

/// <summary>
/// Constructor
/// </summary>
/// <param name="pParent">Pointer to the parent viewer.</param>
/// <param name="dialogId">The setting dialog resource Id.</param>
CameraSettingsViewer::CameraSettingsViewer(const NuiViewer* pParent, UINT dialogId)
    : NuiViewer(pParent)
    , m_dialogId(dialogId)
    , m_pNuiCameraSettings(nullptr)
{
}

CameraSettingsViewer::~CameraSettingsViewer()
{
    SafeRelease(m_pNuiCameraSettings);
}

/// <summary>
/// Initialize the setting dialog and the camera parameters
/// </summary>
/// <param name="pNuiCameraSettings">Pointer to the camera settings interface.</param>
void CameraSettingsViewer::Initialize(INuiColorCameraSettings* pNuiCameraSettings)
{
    m_pNuiCameraSettings = pNuiCameraSettings;
    m_pNuiCameraSettings->AddRef();

    TrackBarRange = MAKELONG(TrackBarStartRange, TrackBarEndRange);

    SetTrackBarsRange();

    UpdateControl();

    UpdateControlEnableStatus();
}

/// <summary>
/// Show the viewer at the Kinect window position
/// </summary>
void CameraSettingsViewer::ShowView()
{
    assert(nullptr != m_pParent);

    // Set the setting dialog show position
    POINT point = {0, 0};
    ClientToScreen(m_pParent->GetWindow(), &point);
    SetWindowPos(m_hWnd, 0, point.x, point.y, 0, 0, SWP_NOSIZE);

    NuiViewer::ShowView();
}

/// <summary>
/// Handle windows messages for a class instance
/// </summary>
/// <param name="hWnd">Window message is for</param>
/// <param name="uMsg">Message</param>
/// <param name="wParam">Message data</param>
/// <param name="lParam">Additional message data</param>
LRESULT CameraSettingsViewer::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch (uMsg)
    {
    case WM_INITDIALOG:
        NuiViewer::SetIcon(hWnd);
        break;

    case WM_COMMAND:
        OnCommand(LOWORD(wParam));
        break;

        // If the titlebar X is clicked, hide the dialog
    case WM_CLOSE:
        HideView();
        break;

    case WM_HSCROLL:
        OnScroll(GetDlgCtrlID((HWND)lParam));
        break;
    }

    return FALSE;
}

/// <summary>
/// Reset the setting dialog and the camera parameters
/// </summary>
void CameraSettingsViewer::ResetSettings()
{
    ResetSensorParameters();

    UpdateControl();

    UpdateControlEnableStatus();
}

/// <summary>
/// Enable/Disable the controls in the given range
/// </summary>
/// <param name="controlStartId">Start id of the given range.</param>
/// <param name="controlEndId">End id of the given range.</param>
/// <param name="controlEnabled">Indicate enable/disable controls.</param>
void CameraSettingsViewer::EnableControls(UINT controlStartId, UINT controlEndId, BOOL controlEnabled)
{
    for (UINT controlId = controlStartId; controlId <= controlEndId; ++controlId)
    {
        EnableWindow(GetDlgItem(m_hWnd, controlId), controlEnabled);
    }
}

/// <summary>
/// Retrieve the sensor current color settings
/// </summary>
ColorSetting CameraSettingsViewer::GetCurrentColorSetting()
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    ColorSetting colorSetting = {0};

    m_pNuiCameraSettings->GetAutoWhiteBalance(&colorSetting.AutoWhiteBalance);
    m_pNuiCameraSettings->GetWhiteBalance(&colorSetting.WhiteBalance);
    m_pNuiCameraSettings->GetContrast(&colorSetting.Contrast);
    m_pNuiCameraSettings->GetHue(&colorSetting.Hue);
    m_pNuiCameraSettings->GetSaturation(&colorSetting.Saturation);
    m_pNuiCameraSettings->GetGamma(&colorSetting.Gamma);
    m_pNuiCameraSettings->GetSharpness(&colorSetting.Sharpness);

    return colorSetting;
}

/// <summary>
/// Retrieve the sensor current exposure settings
/// </summary>
ExposureSetting CameraSettingsViewer::GetCurrentExposureSetting()
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    ExposureSetting exposureSetting = {0};

    m_pNuiCameraSettings->GetAutoExposure(&exposureSetting.AutoExposure);
    m_pNuiCameraSettings->GetBrightness(&exposureSetting.Brightness);
    m_pNuiCameraSettings->GetFrameInterval(&exposureSetting.FrameInterval);
    m_pNuiCameraSettings->GetExposureTime(&exposureSetting.ExposureTime);
    m_pNuiCameraSettings->GetGain(&exposureSetting.Gain);
    m_pNuiCameraSettings->GetPowerLineFrequency(&exposureSetting.PowerLineFrequency);
    m_pNuiCameraSettings->GetBacklightCompensationMode(&exposureSetting.BacklightCompensationMode);

    return exposureSetting;
}