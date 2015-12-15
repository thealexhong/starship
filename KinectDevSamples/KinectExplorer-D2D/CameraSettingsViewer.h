//------------------------------------------------------------------------------
// <copyright file="CameraSettingsViewer.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <map>
#include <Windows.h>
#include <NuiApi.h>

#include "NuiViewer.h"

// Describe Kinect sensor color setting parameters
struct ColorSetting
{
    BOOL AutoWhiteBalance;
    LONG WhiteBalance;
    double Contrast;
    double Hue;
    double Saturation;
    double Gamma;
    double Sharpness;
};

// Describe Kinect sensor exposure setting parameters
struct ExposureSetting
{
    BOOL AutoExposure;
    double Brightness;
    double FrameInterval;
    double ExposureTime;
    double Gain;
    NUI_POWER_LINE_FREQUENCY PowerLineFrequency;
    NUI_BACKLIGHT_COMPENSATION_MODE BacklightCompensationMode;
};

// Define min/max values of camera setting parameters
#define CAMERA_SETTING_WHITEBALANCE_MIN 2700
#define CAMERA_SETTING_WHITEBALANCE_MAX 6500
#define CAMERA_SETTING_CONTRAST_MIN 0.5
#define CAMERA_SETTING_CONTRAST_MAX 2.0
#define CAMERA_SETTING_HUE_MIN -22.0
#define CAMERA_SETTING_HUE_MAX 22.0
#define CAMERA_SETTING_SATURATION_MIN 0.0
#define CAMERA_SETTING_SATURATION_MAX 2.0
#define CAMERA_SETTING_GAMMA_MIN 1.0
#define CAMERA_SETTING_GAMMA_MAX 2.8
#define CAMERA_SETTING_SHARPNESS_MIN 0.0
#define CAMERA_SETTING_SHARPNESS_MAX 1.0
#define CAMERA_SETTING_EXPOSURETIME_MIN 1
#define CAMERA_SETTING_EXPOSURETIME_MAX 4000
#define CAMERA_SETTING_FRAMEINTERVAL_MIN 0
#define CAMERA_SETTING_FRAMEINTERVAL_MAX 4000
#define CAMERA_SETTING_BRIGHTNESS_MIN 0.0
#define CAMERA_SETTING_BRIGHTNESS_MAX 1.0
#define CAMERA_SETTING_GAIN_MIN 1.0
#define CAMERA_SETTING_GAIN_MAX 16.0

class CameraSettingsViewer : public NuiViewer
{
public:

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pParent">Pointer to the parent viewer.</param>
    /// <param name="dialogId">The setting dialog resource Id.</param>
    CameraSettingsViewer(const NuiViewer* pParent, UINT dialogId);

    /// <summary>
    /// Destructor
    /// </summary>
    virtual ~CameraSettingsViewer();

    /// <summary>
    /// Initialize the setting dialog and the camera parameters
    /// </summary>
    /// <param name="pNuiCameraSettings">Pointer to the camera settings interface.</param>
    void Initialize(INuiColorCameraSettings* pNuiCameraSettings);

    /// <summary>
    /// Show the viewer at the Kinect window position
    /// </summary>
    void ShowView();

protected:

    /// <summary>
    /// Handle windows messages for a class instance
    /// </summary>
    /// <param name="hWnd">Window message is for</param>
    /// <param name="uMsg">Message</param>
    /// <param name="wParam">Message data</param>
    /// <param name="lParam">Additional message data</param>
    virtual LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

    /// <summary>
    /// Reset the setting dialog and the camera parameters
    /// </summary>
    void ResetSettings();

    /// <summary>
    /// Reset the sensor parameters
    /// </summary>
    virtual void ResetSensorParameters() = 0;

    /// <summary>
    /// Update the chilid controls based on the setting values
    /// </summary>
    virtual void UpdateControl() = 0;

    /// <summary>
    /// Enable/Disable some child controls based on current option
    /// </summary>
    virtual void UpdateControlEnableStatus() = 0;

    /// <summary>
    /// Set range of all the track bar controls
    /// </summary>
    virtual void SetTrackBarsRange() = 0;

    /// <summary>
    /// Handle the WM_COMMAND message
    /// </summary>
    /// <param name="controlId">Id of the control that triggers the message.</param>
    virtual void OnCommand(UINT controlId) = 0;

    /// <summary>
    /// Handle the WM_HSCROLL message
    /// </summary>
    /// <param name="controlId">Id of the control that triggers the message.</param>
    virtual void OnScroll(UINT controlId) = 0;

    /// <summary>
    /// Enable/Disable the controls in the given range
    /// </summary>
    /// <param name="controlStartId">Start id of the given range.</param>
    /// <param name="controlEndId">End id of the given range.</param>
    /// <param name="controlEnabled">Indicate enable/disable controls.</param>
    void EnableControls(UINT controlStartId, UINT controlEndId, BOOL controlEnabled);

    /// <summary>
    /// Returns the ID of the dialog
    /// </summary>
    UINT GetDlgId()
    {
        return m_dialogId;
    }

    /// <summary>
    /// Check whether the child button is in "checked"
    /// </summary>
    /// <param name="buttonId">Control Id of the child button.</param>
    bool IsButtonChecked(UINT buttonId)
    {
        return BST_CHECKED == IsDlgButtonChecked(m_hWnd, buttonId);
    }

    /// <summary>
    /// Send message to the child track bar control
    /// </summary>
    /// <param name="controlId">Id of the child track bar.</param>
    /// <param name="msg">Message.</param>
    /// <param name="lParame">Additional message data.</param>
    void SendTrackBarSetMessage(UINT controlId, UINT msg, LONG lParam)
    {
        SendMessageW(GetDlgItem(m_hWnd, controlId), msg, (WPARAM)TRUE, (LPARAM)lParam);
    }

    /// <summary>
    /// Return the track bar value mapping the setting value to the track bar
    /// </summary>
    /// <param name="minSettingValue">Minimum setting value.</param>
    /// <param name="maxSettingValue">Maximum setting value.</param>
    /// <param name="settingValue">The current setting value.</param>
    LONG CalculateTrackValue(double minSettingValue, double maxSettingValue, double settingValue)
    {
        return (LONG)((settingValue - minSettingValue) / (maxSettingValue - minSettingValue) * TrackBarEndRange);
    }

    /// <summary>
    /// Return the setting value mapping the setting value to the track bar
    /// </summary>
    /// <param name="minSettingValue">Minimum setting value.</param>
    /// <param name="maxSettingValue">Maximum setting value.</param>
    /// <param name="trackValue">The current track bar value.</param>
    double CalculateSettingValue(double minSettingValue, double maxSettingValue, LONG trackValue)
    {
        return minSettingValue + (double)trackValue / (double)TrackBarEndRange * (maxSettingValue - minSettingValue);
    }

    /// <summary>
    /// Retrieve the sensor current color settings
    /// </summary>
    ColorSetting GetCurrentColorSetting();

    /// <summary>
    /// Retrieve the sensor current exposure settings
    /// </summary>
    ExposureSetting GetCurrentExposureSetting();

protected:

    /// Id of the dialog
    UINT m_dialogId;

    /// Pointer to camera settings interface
    INuiColorCameraSettings* m_pNuiCameraSettings;

    static const WORD TrackBarStartRange = 0;
    static const WORD TrackBarEndRange = 100;
    static LONG TrackBarRange;
};