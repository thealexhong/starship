//------------------------------------------------------------------------------
// <copyright file="CameraColorSettingsViewer.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include "CameraSettingsViewer.h"

class CameraColorSettingsViewer : public CameraSettingsViewer
{
public:

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pParent">Pointer to the parent viewer.</param>
    CameraColorSettingsViewer(const NuiViewer* pParent);

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
    /// Enable/Disable some child controls based on "auto white balance" option
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
    /// Set auto white balance
    /// </summary>
    /// <param name="autoWhiteBalanceEnabled">Indicate whether enable auto white balance option.</param>
    void SetAutoWhiteBalance(BOOL autoWhiteBalanceEnabled);
};