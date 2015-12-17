//------------------------------------------------------------------------------
// <copyright file="NuiAudioStream.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <NuiApi.h>
#include "NuiAudioViewer.h"
#include "StaticMediaBuffer.h"

class NuiAudioStream
{
public:
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pNuiSensor">The pointer to Nui sensor object</param>
    NuiAudioStream(INuiSensor* pNuiSensor);

    /// <summary>
    /// Destructor
    /// </summary>
   ~NuiAudioStream();

public:
    /// <summary>
    /// Attach stream viewer
    /// </summary>
    /// <param name="pViewer">The pointer to the viewer to attach</param>
    void SetStreamViewer(NuiAudioViewer* pViewer);

    /// <summary>
    /// Start processing stream
    /// </summary>
    /// <returns>Indicates success or failure</returns>
    HRESULT StartStream();
    
    /// <summary>
    /// Get the audio readings from the stream
    /// </summary>
    void ProcessStream();

private:
    INuiSensor*         m_pNuiSensor;
    INuiAudioBeam*      m_pNuiAudioSource;
    IMediaObject*       m_pDMO;
    IPropertyStore*     m_pPropertyStore;
    NuiAudioViewer*     m_pAudioViewer;
    CStaticMediaBuffer  m_captureBuffer;
};