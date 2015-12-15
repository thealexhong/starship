//------------------------------------------------------------------------------
// <copyright file="NuiAccelerometerStream.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <NuiApi.h>
#include "NuiAccelerometerViewer.h"

class NuiAccelerometerStream
{
public:
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pNuiSensor">The pointer to Nui sensor object</param>
    NuiAccelerometerStream(INuiSensor* pNuiSensor);

    /// <summary>
    /// Destructor
    /// </summary>
   ~NuiAccelerometerStream();

public:
    /// <summary>
    /// Attach stream viewer
    /// </summary>
    /// <param name="pViewer">The pointer to the viewer to attach</param>
    void SetStreamViewer(NuiAccelerometerViewer* pViewer);

    /// <summary>
    /// Get accelerometer reading
    /// </summary>
    void ProcessStream();

    /// <summary>
    /// Start processing stream
    /// </summary>
    /// <returns>Always returns S_OK</returns>
    HRESULT StartStream();

private:
    INuiSensor*             m_pNuiSensor;
    NuiAccelerometerViewer* m_pAccelerometerViewer;
};