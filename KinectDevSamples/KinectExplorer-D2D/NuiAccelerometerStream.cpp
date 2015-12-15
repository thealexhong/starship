//------------------------------------------------------------------------------
// <copyright file="NuiAccelerometerStream.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include "NuiAccelerometerStream.h"
#include "Utility.h"

/// <summary>
/// Constructor
/// </summary>
/// <param name="pNuiSensor">The pointer to Nui sensor object</param>
NuiAccelerometerStream::NuiAccelerometerStream(INuiSensor* pNuiSensor)
    : m_pNuiSensor(pNuiSensor)
    , m_pAccelerometerViewer(nullptr)
{
    if (m_pNuiSensor)
    {
        m_pNuiSensor->AddRef();
    }
}

/// <summary>
/// Destructor
/// </summary>
NuiAccelerometerStream::~NuiAccelerometerStream()
{
    SafeRelease(m_pNuiSensor);
}

/// <summary>
/// Start processing stream
/// </summary>
/// <returns>Always returns S_OK</returns>
HRESULT NuiAccelerometerStream::StartStream()
{
    return S_OK;
}

/// <summary>
/// Attach stream viewer
/// </summary>
/// <param name="pViewer">The pointer to the viewer to attach</param>
void NuiAccelerometerStream::SetStreamViewer(NuiAccelerometerViewer* pViewer)
{
    m_pAccelerometerViewer = pViewer;
}

/// <summary>
/// Get accelerometer reading
/// </summary>
void NuiAccelerometerStream::ProcessStream()
{
    // Get the reading
    Vector4 reading;
    HRESULT hr = m_pNuiSensor->NuiAccelerometerGetCurrentReading(&reading);

    if (SUCCEEDED(hr) && m_pAccelerometerViewer)
    {
        // Set the reading to viewer
        m_pAccelerometerViewer->SetAccelerometerReadings(reading.x, reading.y, reading.z);
    }
}