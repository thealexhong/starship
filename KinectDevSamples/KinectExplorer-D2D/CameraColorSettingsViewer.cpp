//------------------------------------------------------------------------------
// <copyright file="CameraColorSettingsViewer.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <NuiApi.h>
#include <NuiImageCamera.h>
#include <NuiSensor.h>
#include <NuiSkeleton.h>

#include "CameraColorSettingsViewer.h"
#include "resource.h"

/// <summary>
/// Constructor
/// </summary>
/// <param name="pParent">Pointer to the parent viewer.</param>
CameraColorSettingsViewer::CameraColorSettingsViewer(const NuiViewer* pParent)
    : CameraSettingsViewer(pParent, IDD_COLORSETTINGSDLG)
{
}

/// <summary>
/// Set range of all the track bar controls
/// </summary>
void CameraColorSettingsViewer::SetTrackBarsRange()
{
    SendTrackBarSetMessage(IDC_CSDSLIDERWB, TBM_SETRANGE, TrackBarRange);
    SendTrackBarSetMessage(IDC_CSDSLIDERCON, TBM_SETRANGE, TrackBarRange);
    SendTrackBarSetMessage(IDC_CSDSLIDERHUE, TBM_SETRANGE, TrackBarRange);
    SendTrackBarSetMessage(IDC_CSDSLIDERSAT, TBM_SETRANGE, TrackBarRange);
    SendTrackBarSetMessage(IDC_CSDSLIDERGAM, TBM_SETRANGE, TrackBarRange);
    SendTrackBarSetMessage(IDC_CSDSLIDERSHA, TBM_SETRANGE, TrackBarRange);
}

/// <summary>
/// Reset the sensor parameters
/// </summary>
void CameraColorSettingsViewer::ResetSensorParameters()
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    // Save the sensor current exposure setting values
    ExposureSetting exposureSetting = GetCurrentExposureSetting();

    // Reset the sensor setting
    m_pNuiCameraSettings->ResetCameraSettingsToDefault();

    // Set back the exposure setting values
    m_pNuiCameraSettings->SetAutoExposure(exposureSetting.AutoExposure);
    m_pNuiCameraSettings->SetBrightness(exposureSetting.Brightness);
    m_pNuiCameraSettings->SetFrameInterval(exposureSetting.FrameInterval);
    m_pNuiCameraSettings->SetExposureTime(exposureSetting.ExposureTime);
    m_pNuiCameraSettings->SetGain(exposureSetting.Gain);
    m_pNuiCameraSettings->SetPowerLineFrequency(exposureSetting.PowerLineFrequency);
    m_pNuiCameraSettings->SetBacklightCompensationMode(exposureSetting.BacklightCompensationMode);
}

/// <summary>
/// Update the chilid controls based on the setting values
/// </summary>
void CameraColorSettingsViewer::UpdateControl()
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    // Get the sensor current color setting values
    ColorSetting colorSetting = GetCurrentColorSetting();

    UINT checkedButtonId = (TRUE == colorSetting.AutoWhiteBalance) ? IDC_AWBRADIOENABLE : IDC_AWBRADIODISABLE;
    CheckRadioButton(m_hWnd, IDC_AWBRADIOENABLE, IDC_AWBRADIODISABLE, checkedButtonId);

    SendTrackBarSetMessage(IDC_CSDSLIDERWB, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_WHITEBALANCE_MIN, CAMERA_SETTING_WHITEBALANCE_MAX, colorSetting.WhiteBalance));
    SendTrackBarSetMessage(IDC_CSDSLIDERCON, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_CONTRAST_MIN, CAMERA_SETTING_CONTRAST_MAX, colorSetting.Contrast));
    SendTrackBarSetMessage(IDC_CSDSLIDERHUE, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_HUE_MIN, CAMERA_SETTING_HUE_MAX, colorSetting.Hue));
    SendTrackBarSetMessage(IDC_CSDSLIDERSAT, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_SATURATION_MIN, CAMERA_SETTING_SATURATION_MAX, colorSetting.Saturation));
    SendTrackBarSetMessage(IDC_CSDSLIDERGAM, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_GAMMA_MIN, CAMERA_SETTING_GAMMA_MAX, colorSetting.Gamma));
    SendTrackBarSetMessage(IDC_CSDSLIDERSHA, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_SHARPNESS_MIN, CAMERA_SETTING_SHARPNESS_MAX, colorSetting.Sharpness));
}

/// <summary>
/// Enable/Disable some child controls based on "auto white balance" option
/// </summary>
void CameraColorSettingsViewer::UpdateControlEnableStatus()
{
    EnableControls(IDC_CSDSTATICWB, IDC_CSDSLIDERWB, !IsButtonChecked(IDC_AWBRADIOENABLE));
}

/// <summary>
/// Handle the WM_COMMAND message
/// </summary>
/// <param name="controlId">Id of the control that triggers the message.</param>
void CameraColorSettingsViewer::OnCommand(UINT controlId)
{
    switch (controlId)
    {
    case ID_CSDCLOSE:
        HideView();
        break;

    case ID_CSDRESET:
        ResetSettings();
        break;

    case IDC_AWBRADIOENABLE:
        SetAutoWhiteBalance(TRUE);
        break;

    case IDC_AWBRADIODISABLE:
        SetAutoWhiteBalance(FALSE);
        break;
    }
}

/// <summary>
/// Handle the WM_HSCROLL message
/// </summary>
/// <param name="controlId">Id of the control that triggers the message.</param>
void CameraColorSettingsViewer::OnScroll(UINT controlId)
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    LONG trackValue = (LONG)SendMessageW(GetDlgItem(m_hWnd, controlId), TBM_GETPOS, 0, 0);

    switch (controlId)
    {
    case IDC_CSDSLIDERWB:
        m_pNuiCameraSettings->SetWhiteBalance((LONG)CalculateSettingValue(CAMERA_SETTING_WHITEBALANCE_MIN, CAMERA_SETTING_WHITEBALANCE_MAX, trackValue));
        break;

    case IDC_CSDSLIDERCON:
        m_pNuiCameraSettings->SetContrast(CalculateSettingValue(CAMERA_SETTING_CONTRAST_MIN, CAMERA_SETTING_CONTRAST_MAX, trackValue));
        break;

    case IDC_CSDSLIDERHUE:
        m_pNuiCameraSettings->SetHue(CalculateSettingValue(CAMERA_SETTING_HUE_MIN, CAMERA_SETTING_HUE_MAX, trackValue));
        break;

    case IDC_CSDSLIDERSAT:
        m_pNuiCameraSettings->SetSaturation(CalculateSettingValue(CAMERA_SETTING_SATURATION_MIN, CAMERA_SETTING_SATURATION_MAX, trackValue));
        break;

    case IDC_CSDSLIDERGAM:
        m_pNuiCameraSettings->SetGamma(CalculateSettingValue(CAMERA_SETTING_GAMMA_MIN, CAMERA_SETTING_GAMMA_MAX, trackValue));
        break;

    case IDC_CSDSLIDERSHA:
        m_pNuiCameraSettings->SetSharpness(CalculateSettingValue(CAMERA_SETTING_SHARPNESS_MIN, CAMERA_SETTING_SHARPNESS_MAX, trackValue));
        break;
    }
}

/// <summary>
/// Enable/Disable "auto white balance"
/// </summary>
/// <param name="autoWhiteBalanceEnabled">Indicate whether enable auto white balance option.</param>
void CameraColorSettingsViewer::SetAutoWhiteBalance(BOOL autoWhiteBalanceEnabled)
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    m_pNuiCameraSettings->SetAutoWhiteBalance(autoWhiteBalanceEnabled);

    UpdateControlEnableStatus();
}