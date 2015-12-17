//------------------------------------------------------------------------------
// <copyright file="NuiAudioStream.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <uuids.h>
#include <wmcodecdsp.h>
#include "NuiAudioStream.h"
#include "Utility.h"

/// <summary>
/// Constructor
/// </summary>
/// <param name="pNuiSensor">The pointer to Nui sensor object</param>
NuiAudioStream::NuiAudioStream(INuiSensor* pNuiSensor)
    : m_pNuiSensor(pNuiSensor)
    , m_pNuiAudioSource(nullptr)
    , m_pDMO(nullptr)
    , m_pPropertyStore(nullptr)
    , m_pAudioViewer(nullptr)
{
    if (m_pNuiSensor)
    {
        m_pNuiSensor->AddRef();
    }
}

/// <summary>
/// Destructor
/// </summary>
NuiAudioStream::~NuiAudioStream()
{
    SafeRelease(m_pPropertyStore);
    SafeRelease(m_pDMO);
    SafeRelease(m_pNuiAudioSource);
    SafeRelease(m_pNuiSensor);
}

/// <summary>
/// Start processing stream
/// </summary>
/// <returns>Indicates success or failure</returns>
HRESULT NuiAudioStream::StartStream()
{
    // Get audio source interface
    HRESULT hr = m_pNuiSensor->NuiGetAudioSource(&m_pNuiAudioSource);
    if (FAILED(hr))
    {
        return hr;
    }

    // Query dmo interface
    hr = m_pNuiAudioSource->QueryInterface(IID_IMediaObject, (void**)&m_pDMO);
    if (FAILED(hr))
    {
        return hr;
    }

    // Query property store interface
    hr = m_pNuiAudioSource->QueryInterface(IID_IPropertyStore, (void**)&m_pPropertyStore);
    if (FAILED(hr))
    {
        return hr;
    }

    // Set AEC-MicArray DMO system mode. This must be set for the DMO to work properly.
    // Possible values are:
    //   SINGLE_CHANNEL_AEC = 0
    //   OPTIBEAM_ARRAY_ONLY = 2
    //   OPTIBEAM_ARRAY_AND_AEC = 4
    //   SINGLE_CHANNEL_NSAGC = 5

    PROPVARIANT pvSysMode;

    // Initialize the variable
    PropVariantInit(&pvSysMode);

    // Assign properties
    pvSysMode.vt   = VT_I4;
    pvSysMode.lVal = (LONG)(2); // Use OPTIBEAM_ARRAY_ONLY setting. Set OPTIBEAM_ARRAY_AND_AEC instead if you expect to have sound playing from speakers.

    // Set properties
    m_pPropertyStore->SetValue(MFPKEY_WMAAECMA_SYSTEM_MODE, pvSysMode);

    // Release the variable
    PropVariantClear(&pvSysMode);

    // Set DMO output format
    WAVEFORMATEX   wfxOut = {AudioFormat, AudioChannels, AudioSamplesPerSecond, AudioAverageBytesPerSecond, AudioBlockAlign, AudioBitsPerSample, 0};
    DMO_MEDIA_TYPE mt     = {0};

    // Initialize variable
    MoInitMediaType(&mt, sizeof(WAVEFORMATEX));

    // Assign format
    mt.majortype            = MEDIATYPE_Audio;
    mt.subtype              = MEDIASUBTYPE_PCM;
    mt.lSampleSize          = 0;
    mt.bFixedSizeSamples    = TRUE;
    mt.bTemporalCompression = FALSE;
    mt.formattype           = FORMAT_WaveFormatEx;	

    memcpy(mt.pbFormat, &wfxOut, sizeof(WAVEFORMATEX));

    // Set format
    hr = m_pDMO->SetOutputType(0, &mt, 0); 

    // Release variable
    MoFreeMediaType(&mt);

    return hr;
}

/// <summary>
/// Attach stream viewer
/// </summary>
/// <param name="pViewer">The pointer to the viewer to attach</param>
void NuiAudioStream::SetStreamViewer(NuiAudioViewer* pViewer)
{
    m_pAudioViewer = pViewer;
}

/// <summary>
/// Get the audio readings from the stream
/// </summary>
void NuiAudioStream::ProcessStream()
{
    if (m_pNuiAudioSource && m_pDMO)
    {
        // Set buffer
        DWORD dwStatus;
        DMO_OUTPUT_DATA_BUFFER outputBuffer = {0};
        outputBuffer.pBuffer = &m_captureBuffer;

        do
        {
            m_captureBuffer.Init(0);
            outputBuffer.dwStatus = 0;

            // Process audio data
            HRESULT hr = m_pDMO->ProcessOutput(0, 1, &outputBuffer, &dwStatus);
            if (S_OK == hr)
            {
                // Get the reading
                double beamAngle, sourceAngle, sourceConfidence;
                if (SUCCEEDED(m_pNuiAudioSource->GetBeam(&beamAngle)) &&
                    SUCCEEDED(m_pNuiAudioSource->GetPosition(&sourceAngle, &sourceConfidence)))
                {
                    if (m_pAudioViewer)
                    {
                        // Set reading sto viewer
                        m_pAudioViewer->SetAudioReadings(beamAngle, sourceAngle, sourceConfidence);
                    }
                }
            }
        }while (outputBuffer.dwStatus & DMO_OUTPUT_DATA_BUFFERF_INCOMPLETE);//Check if there is still remaining data
    }
}