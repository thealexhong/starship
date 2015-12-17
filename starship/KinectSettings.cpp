//------------------------------------------------------------------------------
// <copyright file="KinectSettings.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include "KinectSettings.h"
#include "Utility.h"
#include "resource.h"
#include "KinectWindow.h"
#include "CameraColorSettingsViewer.h"
#include "CameraExposureSettingsViewer.h"

/// <summary>
/// Return the chooser mode based on the given command Id
/// </summary>
/// <param name="commanId">ID of the menu item</param>
ChooserMode ConvertCommandIdToChooserMode(WORD commandId)
{
    switch (commandId)
    {
    case ID_CHOOSERMODE_DEFAULTSYSTEMTRACKING:
        return ChooserModeDefault;

    case ID_CHOOSERMODE_CLOSEST1PLAYER:
        return ChooserModeClosest1;

    case ID_CHOOSERMODE_CLOSEST2PLAYER:
        return ChooserModeClosest2;

    case ID_CHOOSERMODE_STICKY1PLAYER:
        return ChooserModeSticky1;

    case ID_CHOOSERMODE_STICKY2PLAYER:
        return ChooserModeSticky2;

    case ID_CHOOSERMODE_MOSTACTIVE1PLAYER:
        return ChooserModeActive1;

    case ID_CHOOSERMODE_MOSTACTIVE2PLAYER:
        return ChooserModeActive2;
    }

    return ChooserModeDefault;
}

/// <summary>
/// Constructor
/// </summary>
/// <param name="pNuiSensor">The pointer to NUI sensor instance</param>
/// <param name="pPrimaryView">The pointer to primary viewer instance</param>
/// <param name="pSecondaryView">The pointer to secondary viewer instance</param>
/// <param name="pColorStream">The pointer to color stream object instance</param>
/// <param name="pDepthStream">The pointer to depth stream object instance</param>
/// <param name="pSkeletonStream">The pointer to skeleton stream object instance</param>
KinectSettings::KinectSettings(INuiSensor* pNuiSensor, NuiStreamViewer* pPrimaryView, NuiStreamViewer* pSecondaryView, NuiColorStream* pColorStream, NuiDepthStream* pDepthStream, NuiSkeletonStream* pSkeletonStream, CameraSettingsViewer* pColorSettingsView, CameraSettingsViewer* pExposureSettingsView)
    : m_pNuiSensor(pNuiSensor)
    , m_pPrimaryView(pPrimaryView)
    , m_pSecondaryView(pSecondaryView)
    , m_pColorStream(pColorStream)
    , m_pDepthStream(pDepthStream)
    , m_pSkeletonStream(pSkeletonStream)
    , m_pColorSettingsView(pColorSettingsView)
    , m_pExposureSettingsView(pExposureSettingsView)
{
    m_pNuiSensor->AddRef();
}

/// <summary>
/// Destructor
/// </summary>
KinectSettings::~KinectSettings()
{
    SafeRelease(m_pNuiSensor);
}

/// <summary>
/// Process Kinect window menu commands
/// </summary>
/// <param name="commanId">ID of the menu item</param>
/// <param name="param">Parameter passed in along with the commmand ID</param>
/// <param name="previouslyChecked">Check status of menu item before command is issued</param>
void KinectSettings::ProcessMenuCommand(WORD commandId, WORD param, bool previouslyChecked)
{
    if (ID_COLORSTREAM_PAUSE == commandId)
    {
        // Pause color stream
        if (m_pColorStream)
        {
            m_pColorStream->PauseStream(!previouslyChecked);
        }
    }
    else if (ID_COLORSTREAM_RESOLUTION_START <= commandId && ID_COLORSTREAM_RESOLUTION_END >= commandId)
    {
        // Set color stream format and resolution
        if (!m_pColorStream)
        {
            return;
        }

        switch (commandId)
        {
        case ID_RESOLUTION_RGBRESOLUTION640X480FPS30:
            m_pColorStream->SetImageType(NUI_IMAGE_TYPE_COLOR);
            m_pColorStream->SetImageResolution(NUI_IMAGE_RESOLUTION_640x480);
            break;

        case ID_RESOLUTION_RGBRESOLUTION1280X960FPS12:
            m_pColorStream->SetImageType(NUI_IMAGE_TYPE_COLOR);
            m_pColorStream->SetImageResolution(NUI_IMAGE_RESOLUTION_1280x960);
            break;

        case ID_RESOLUTION_YUVRESOLUTION640X480FPS15:
            m_pColorStream->SetImageType(NUI_IMAGE_TYPE_COLOR_YUV);
            m_pColorStream->SetImageResolution(NUI_IMAGE_RESOLUTION_640x480);
            break;

        case ID_RESOLUTION_INFRAREDRESOLUTION640X480FPS30:
            m_pColorStream->SetImageType(NUI_IMAGE_TYPE_COLOR_INFRARED);
            m_pColorStream->SetImageResolution(NUI_IMAGE_RESOLUTION_640x480);
            break;

        case ID_RESOLUTION_RAWBAYERRESOLUTION640X480FPS30:
            m_pColorStream->SetImageType(NUI_IMAGE_TYPE_COLOR_RAW_BAYER);
            m_pColorStream->SetImageResolution(NUI_IMAGE_RESOLUTION_640x480);
            break;

        case ID_RESOLUTION_RAWBAYERRESOLUTION1280X960FPS12:
            m_pColorStream->SetImageType(NUI_IMAGE_TYPE_COLOR_RAW_BAYER);
            m_pColorStream->SetImageResolution(NUI_IMAGE_RESOLUTION_1280x960);
            break;

        default:
            return;
        }

        m_pColorStream->OpenStream();
    }
    else if (ID_DEPTHSTREAM_PAUSE == commandId)
    {
        // Pause depth stream
        if(m_pDepthStream)
        {
            m_pDepthStream->PauseStream(!previouslyChecked);
        }
    }
    else if (ID_DEPTHSTREAM_RANGEMODE_START <= commandId && ID_DEPTHSTREAM_RANGEMODE_END >= commandId)
    {
        // Set depth stream range mode
        bool nearMode = false;
        switch (commandId)
        {
        case ID_RANGEMODE_DEFAULT:
            nearMode = false;
            break;

        case ID_RANGEMODE_NEAR:
            nearMode = true;
            break;

        default:
            return;
        }

        if (m_pDepthStream)
        {
            m_pDepthStream->SetNearMode(nearMode);
        }

        if (m_pSkeletonStream)
        {
            m_pSkeletonStream->SetNearMode(nearMode);
        }
    }
    else if (ID_DEPTHSTREAM_RESOLUTION_START <= commandId && ID_DEPTHSTREAM_RESOLUTION_END >= commandId)
    {
        // Set depth stream resolution
        NUI_IMAGE_RESOLUTION resolution = (NUI_IMAGE_RESOLUTION)(commandId - ID_DEPTHSTREAM_RESOLUTION_START);
        if (m_pDepthStream)
        {
            m_pDepthStream->OpenStream(resolution);
        }
    }
    else if (ID_DEPTHSTREAM_DEPTHTREATMENT_START <= commandId && ID_DEPTHSTREAM_DEPTHTREATMENT_END >= commandId)
    {
        // Set depth stream treatment mode
        DEPTH_TREATMENT treatment = (DEPTH_TREATMENT)(commandId - ID_DEPTHSTREAM_DEPTHTREATMENT_START);
        if (m_pDepthStream)
        {
            m_pDepthStream->SetDepthTreatment(treatment);
        }
    }
    else if (ID_SKELETONSTREAM_PAUSE == commandId)
    {
        // Pause skeleton stream
        if (m_pSkeletonStream)
        {
            m_pSkeletonStream->PauseStream(!previouslyChecked);
        }
    }
    else if (ID_SKELETONSTREAM_TRACKINGMODE_START <= commandId && ID_SKELETONSTREAM_TRACKINGMODE_END >= commandId)
    {
        // Set skeleton track mode
        if (!m_pSkeletonStream)
        {
            return;
        }

        switch (commandId)
        {
        case ID_TRACKINGMODE_DEFAULT:
            m_pSkeletonStream->SetSeatedMode(false);
            break;

        case ID_TRACKINGMODE_SEATED:
            m_pSkeletonStream->SetSeatedMode(true);
            break;

        default:
            return;
        }
    }
    else if (ID_SKELETONSTREAM_CHOOSERMODE_START <= commandId && ID_SKELETONSTREAM_CHOOSERMODE_END >= commandId)
    {
        // Set skeleton chooser mode
        if(!m_pSkeletonStream)
        {
            return;
        }

        m_pSkeletonStream->SetChooserMode(ConvertCommandIdToChooserMode(commandId));
    }
    else
    {
        switch (commandId)
        {
            // Bring up camera color setting dialog
        case ID_CAMERA_COLORSETTINGS:
            m_pColorSettingsView->ShowView();
            break;

            // Bring up camera exposure setting dialog
        case ID_CAMERA_EXPOSURESETTINGS:
            m_pExposureSettingsView->ShowView();
            break;

            // Switch the stream display on primary and secondary stream viewers
        case ID_VIEWS_SWITCH:
            if (m_pColorStream && m_pDepthStream)
            {
                m_pColorStream->SetStreamViewer(m_pDepthStream->SetStreamViewer(m_pColorStream->SetStreamViewer(nullptr)));
            }
            break;

        case ID_FORCE_OFF_IR:
            m_pNuiSensor->NuiSetForceInfraredEmitterOff(param);
            break;

        default:
            break;
        }
    }
}