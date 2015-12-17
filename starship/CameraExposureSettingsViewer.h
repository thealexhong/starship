//------------------------------------------------------------------------------
// <copyright file="CameraExposureSettingsViewer.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include "CameraSettingsViewer.h"

class CameraExposureSettingsViewer : public CameraSettingsViewer
{
public:

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pParent">Pointer to the parent viewer.</param>
    CameraExposureSettingsViewer(const NuiViewer* pParent);

protected:

    /// <summary>
    /// Set range of all the track bar controls
    /// </summary>
    void SetTrackBarsRange();

    /// <summary>
    /// Reset the sensor parameters
    /// </summary>
    void ResetSensorParameters();

    /// <summary>
    /// Update the chilid controls based on the setting values
    /// </summary>
    void UpdateControl();

    /// <summary>
    /// Enable/Disable some child controls based on "auto exposure" option
    /// </summary>
    void UpdateControlEnableStatus();

    /// <summary>
    /// Handle the WM_COMMAND message
    /// </summary>
    /// <param name="controlId">Id of the control that triggers the message.</param>
    void OnCommand(UINT controlId);

    /// <summary>
    /// Handle the WM_HSCROLL message
    /// </summary>
    /// <param name="controlId">Id of the control that triggers the message.</param>
    void OnScroll(UINT controlId);

    /// <summary>
    /// Set auto exposure
    /// </summary>
    /// <param name="autoExposureEnabled">Indicate whether enable auto exposure option.</param>
    void SetAutoExposure(BOOL autoExposureEnabled);

    /// <summary>
    /// Retrieve the checked button Id of group "power line frequency"
    /// </summary>
    /// <param name="powerLineFrequency">Current PLF setting value.</param>
    UINT GetPLFCheckedButtonId(NUI_POWER_LINE_FREQUENCY powerLineFrequency);

    /// <summary>
    /// Retrieve the checked button Id of group "backlight compensation mode"
    /// </summary>
    /// <param name="backlightCompensationMode">Current BCM setting value.</param>
    UINT GetBCMCheckedButtonId(NUI_BACKLIGHT_COMPENSATION_MODE backlightCompensationMode);
};