//------------------------------------------------------------------------------
// <copyright file="NuiStream.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include "NuiStream.h"
#include "NuiStreamViewer.h"

/// <summary>
/// Constructor
/// </summary>
/// <param name="pNuiSensor">The pointer to Nui sensor device instance</param>
NuiStream::NuiStream(INuiSensor* pNuiSensor)
    : m_pNuiSensor(pNuiSensor)
    , m_pStreamViewer(nullptr)
    , m_hStreamHandle(INVALID_HANDLE_VALUE)
    , m_paused(false)
{
    if (m_pNuiSensor)
    {
        m_pNuiSensor->AddRef();
    }

    m_hFrameReadyEvent = CreateEventW(nullptr, TRUE, FALSE, nullptr);
}

/// <summary>
/// Destructor
/// </summary>
NuiStream::~NuiStream()
{
    if(m_pStreamViewer)
    {
        // Clear reference to image buffer in stream viewer
        m_pStreamViewer->SetImage(nullptr);
    }

    CloseHandle(m_hFrameReadyEvent);
    SafeRelease(m_pNuiSensor);
}

/// <summary>
/// Get stream frame ready event handle
/// </summary>
/// <returns>Handle to event</returns>
HANDLE NuiStream::GetFrameReadyEvent()
{
    return m_hFrameReadyEvent;
}

/// <summary>
/// Pause the stream
/// </summary>
/// <param name="pause">Pause or resume the stream</param>
void NuiStream::PauseStream(bool pause)
{
    m_paused = pause;

    // And meanwhile pause the skeleton
    m_pStreamViewer->PauseSkeleton(pause);
}

/// <summary>
/// Attach viewer object to stream object
/// </summary>
/// <param name="pStreamViewer">The pointer to viewer object to attach</param>
/// <returns>Previously attached viewer object. If none, returns nullptr</returns>
NuiStreamViewer* NuiStream::SetStreamViewer(NuiStreamViewer * pStreamViewer)
{
    NuiStreamViewer* pOldViewer = m_pStreamViewer;
    m_pStreamViewer = pStreamViewer;

    // Synchronize the skeleton pause status with the stream pause status
    if (m_pStreamViewer)
    {
        m_pStreamViewer->PauseSkeleton(m_paused);
    }

    return pOldViewer;
}