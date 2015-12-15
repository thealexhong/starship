//------------------------------------------------------------------------------
// <copyright file="CameraExposureSettingsViewer.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"

#include "CameraExposureSettingsViewer.h"
#include "resource.h"

/// <summary>
/// Constructor
/// </summary>
/// <param name="pParent">Pointer to the parent viewer.</param>
CameraExposureSettingsViewer::CameraExposureSettingsViewer(const NuiViewer* pParent)
    : CameraSettingsViewer(pParent, IDD_EXPOSURESETTINGSDLG)
{
}

/// <summary>
/// Set range of all the track bar controls
/// </summary>
void CameraExposureSettingsViewer::SetTrackBarsRange()
{
    SendTrackBarSetMessage(IDC_ESDSLIDERBRI, TBM_SETRANGE, TrackBarRange);
    SendTrackBarSetMessage(IDC_ESDSLIDERFI, TBM_SETRANGE, TrackBarRange);
    SendTrackBarSetMessage(IDC_ESDSLIDERET, TBM_SETRANGE, TrackBarRange);
    SendTrackBarSetMessage(IDC_ESDSLIDERGAIN, TBM_SETRANGE, TrackBarRange);
}

/// <summary>
/// Reset the sensor exposure parameters
/// </summary>
void CameraExposureSettingsViewer::ResetSensorParameters()
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    // Save the current sensor color setting values
    ColorSetting colorSetting = GetCurrentColorSetting();

    // Reset the sensor setting
    m_pNuiCameraSettings->ResetCameraSettingsToDefault();

    // Set back the color setting values
    m_pNuiCameraSettings->SetAutoWhiteBalance(colorSetting.AutoWhiteBalance);
    m_pNuiCameraSettings->SetWhiteBalance(colorSetting.WhiteBalance);
    m_pNuiCameraSettings->SetContrast(colorSetting.Contrast);
    m_pNuiCameraSettings->SetHue(colorSetting.Hue);
    m_pNuiCameraSettings->SetSaturation(colorSetting.Saturation);
    m_pNuiCameraSettings->SetGamma(colorSetting.Gamma);
    m_pNuiCameraSettings->SetSharpness(colorSetting.Sharpness);
}

/// <summary>
/// Update the chilid controls based on the setting values
/// </summary>
void CameraExposureSettingsViewer::UpdateControl()
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    // Get the current sensor exposure setting values
    ExposureSetting exposureSetting = GetCurrentExposureSetting();

    UINT checkedButtonId = (TRUE == exposureSetting.AutoExposure) ? IDC_AERADIOENABLE : IDC_AERADIODISABLE;
    CheckRadioButton(m_hWnd, IDC_AERADIOENABLE, IDC_AERADIODISABLE, checkedButtonId);
    CheckRadioButton(m_hWnd, IDC_PLFRADIODISABLE, IDC_PLFRADIOSH, GetPLFCheckedButtonId(exposureSetting.PowerLineFrequency));
    CheckRadioButton(m_hWnd, IDC_BCMRADIOAB, IDC_BCMRADIOCO, GetBCMCheckedButtonId(exposureSetting.BacklightCompensationMode));

    SendTrackBarSetMessage(IDC_ESDSLIDERBRI, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_BRIGHTNESS_MIN, CAMERA_SETTING_BRIGHTNESS_MAX, exposureSetting.Brightness));
    SendTrackBarSetMessage(IDC_ESDSLIDERFI, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_FRAMEINTERVAL_MIN, CAMERA_SETTING_FRAMEINTERVAL_MAX, exposureSetting.FrameInterval));
    SendTrackBarSetMessage(IDC_ESDSLIDERET, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_EXPOSURETIME_MIN, CAMERA_SETTING_EXPOSURETIME_MAX, exposureSetting.ExposureTime));
    SendTrackBarSetMessage(IDC_ESDSLIDERGAIN, TBM_SETPOS, CalculateTrackValue(CAMERA_SETTING_GAIN_MIN, CAMERA_SETTING_GAIN_MAX, exposureSetting.Gain));
}

/// <summary>
/// Enable/Disable some child controls based on "auto exposure" option
/// </summary>
void CameraExposureSettingsViewer::UpdateControlEnableStatus()
{
    bool autoExposureChecked = IsButtonChecked(IDC_AERADIOENABLE);

    EnableControls(IDC_ESDSTATICBRI, IDC_ESDSLIDERBRI, autoExposureChecked);
    EnableControls(IDC_ESDSTATICFI, IDC_ESDSLIDERGAIN, !autoExposureChecked);
}

/// <summary>
/// Handle the WM_COMMAND message
/// </summary>
/// <param name="controlId">Id of the control that triggers the message.</param>
void CameraExposureSettingsViewer::OnCommand(UINT controlId)
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    switch (controlId)
    {
    case ID_ESDCLOSE:
        HideView();
        break;

    case ID_ESDRESET:
        ResetSettings();
        break;

    case IDC_AERADIOENABLE:
        SetAutoExposure(TRUE);
        break;

    case IDC_AERADIODISABLE:
        SetAutoExposure(FALSE);
        break;

    case IDC_PLFRADIODISABLE:
        m_pNuiCameraSettings->SetPowerLineFrequency(NUI_POWER_LINE_FREQUENCY_DISABLED);
        break;

    case IDC_PLFRADIOFH:
        m_pNuiCameraSettings->SetPowerLineFrequency(NUI_POWER_LINE_FREQUENCY_50HZ);
        break;

    case IDC_PLFRADIOSH:
        m_pNuiCameraSettings->SetPowerLineFrequency(NUI_POWER_LINE_FREQUENCY_60HZ);
        break;

    case IDC_BCMRADIOAB:
        m_pNuiCameraSettings->SetBacklightCompensationMode(NUI_BACKLIGHT_COMPENSATION_MODE_AVERAGE_BRIGHTNESS);
        break;

    case IDC_BCMRADIOCP:
        m_pNuiCameraSettings->SetBacklightCompensationMode(NUI_BACKLIGHT_COMPENSATION_MODE_CENTER_PRIORITY);
        break;

    case IDC_BCMRADIOLP:
        m_pNuiCameraSettings->SetBacklightCompensationMode(NUI_BACKLIGHT_COMPENSATION_MODE_LOWLIGHTS_PRIORITY);
        break;

    case IDC_BCMRADIOCO:
        m_pNuiCameraSettings->SetBacklightCompensationMode(NUI_BACKLIGHT_COMPENSATION_MODE_CENTER_ONLY);
        break;
    }
}

/// <summary>
/// Handle the WM_HSCROLL message
/// </summary>
/// <param name="controlId">Id of the control that triggers the message.</param>
void CameraExposureSettingsViewer::OnScroll(UINT controlId)
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    LONG trackValue = (LONG)SendMessageW(GetDlgItem(m_hWnd, controlId), TBM_GETPOS, 0, 0);

    switch (controlId)
    {
    case IDC_ESDSLIDERBRI:
        m_pNuiCameraSettings->SetBrightness(CalculateSettingValue(CAMERA_SETTING_BRIGHTNESS_MIN, CAMERA_SETTING_BRIGHTNESS_MAX, trackValue));
        break;

    case IDC_ESDSLIDERFI:
        m_pNuiCameraSettings->SetFrameInterval(CalculateSettingValue(CAMERA_SETTING_FRAMEINTERVAL_MIN, CAMERA_SETTING_FRAMEINTERVAL_MAX, trackValue));
        break;

    case IDC_ESDSLIDERET:
        m_pNuiCameraSettings->SetExposureTime(CalculateSettingValue(CAMERA_SETTING_EXPOSURETIME_MIN, CAMERA_SETTING_EXPOSURETIME_MAX, trackValue));
        break;

    case IDC_ESDSLIDERGAIN:
        m_pNuiCameraSettings->SetGain(CalculateSettingValue(CAMERA_SETTING_GAIN_MIN, CAMERA_SETTING_GAIN_MAX, trackValue));
        break;
    }
}

/// <summary>
/// Set auto exposure
/// </summary>
/// <param name="autoExposureEnabled">Indicate whether enable auto exposure.</param>
void CameraExposureSettingsViewer::SetAutoExposure(BOOL autoExposureEnabled)
{
    // This assert will fire if CameraSettingViewer::Initialize was never called
    assert(nullptr != m_pNuiCameraSettings);

    m_pNuiCameraSettings->SetAutoExposure(autoExposureEnabled);

    UpdateControlEnableStatus();
}

/// <summary>
/// Retrieve the checked button Id of group "power line frequency"
/// </summary>
/// <param name="powerLineFrequency">Current PLF setting value.</param>
UINT CameraExposureSettingsViewer::GetPLFCheckedButtonId(NUI_POWER_LINE_FREQUENCY powerLineFrequency)
{
    switch (powerLineFrequency)
    {
    case NUI_POWER_LINE_FREQUENCY_DISABLED:
        return IDC_PLFRADIODISABLE;

    case NUI_POWER_LINE_FREQUENCY_50HZ:
        return IDC_PLFRADIOFH;

    case NUI_POWER_LINE_FREQUENCY_60HZ:
        return IDC_PLFRADIOSH;
    }

    return IDC_PLFRADIODISABLE;
}

/// <summary>
/// Retrieve the checked button Id of group "backlight compensation mode"
/// </summary>
/// <param name="backlightCompensationMode">Current BCM setting value.</param>
UINT CameraExposureSettingsViewer::GetBCMCheckedButtonId(NUI_BACKLIGHT_COMPENSATION_MODE backlightCompensationMode)
{
    switch (backlightCompensationMode)
    {
    case NUI_BACKLIGHT_COMPENSATION_MODE_AVERAGE_BRIGHTNESS:
        return IDC_BCMRADIOAB;

    case NUI_BACKLIGHT_COMPENSATION_MODE_CENTER_ONLY:
        return IDC_BCMRADIOCO;

    case NUI_BACKLIGHT_COMPENSATION_MODE_CENTER_PRIORITY:
        return IDC_BCMRADIOCP;

    case NUI_BACKLIGHT_COMPENSATION_MODE_LOWLIGHTS_PRIORITY:
        return IDC_BCMRADIOLP;
    }

    return IDC_BCMRADIOAB;
}