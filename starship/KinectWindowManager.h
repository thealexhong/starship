//------------------------------------------------------------------------------
// <copyright file="KinectWindowManager.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <string>
#include <map>
#include <NuiApi.h>

#include "KinectWindow.h"

/// <summary>
/// Each connected sensor is displayed with a kinect window,
/// and the data struct saves a sensor and its associated window
/// </summary>
struct SensorWindowPair
{
    INuiSensor* NuiSensor;

    KinectWindow* KinectWindow;
};

typedef std::map<std::wstring, SensorWindowPair>::iterator SensorMapIterator;

class KinectWindowManager
{
public:

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hWnd">Handle to the kinect window.</param>
    KinectWindowManager(HWND hWnd);

    /// <summary>
    /// Destructor
    /// </summary>
    ~KinectWindowManager();

    /// <summary>
    /// Close all the Kinect windows
    /// </summary>
    void CloseAllKinectWindows();

    /// <summary>
    /// Update when the new sensor status is NuiSensorDisconnected
    /// </summary>
    void HandleSensorDisconnected(PCWSTR instanceName);

    /// <summary>
    /// Update when the new sensor status is not NuiSensorDisconnected
    /// </summary>
    void HandleSensorConnected(PCWSTR instanceName, HRESULT hrSensorCallbackStatus);

    /// <summary>
    /// Reset the Kinect window of the specified sensor to null
    /// </summary>
    void ResetKinectWindow(PCWSTR instanceName);

    /// <summary>
    /// Get the count of saved sensors
    /// </summary>
    size_t GetSensorCount()
    {
        return m_sensorMap.size();
    }

    /// <summary>
    /// Show all the kinect windows
    /// </summary>
    void ShowAllKinectWindows();

    /// <summary>
    /// Set the parameter of showing kinect windows
    /// </summary>
    void SetKinectWindowShowParam(DWORD showParam)
    {
        m_kinectWindowShowParam = showParam;
    }

private:

    /// <summary>
    /// Insert a new sensor to the resource map
    /// </summary>
    SensorMapIterator AddSensor(PCWSTR instanceName, INuiSensor* pNuiSensor);

    /// <summary>
    /// Remove the specified item from the resource map
    /// </summary>
    void RemoveSensor(SensorMapIterator iter);

    /// <summary>
    /// Update the sensor related Kinect window status
    /// </summary>
    void UpdateKinectWindow(HRESULT hrSensorCallbackStatus, INuiSensor* pNuiSensor, KinectWindow** ppKinectWindow);

    /// <summary>
    /// Create a Kinect window to display the sensor
    /// </summary>
    KinectWindow* CreateKinectWindow(INuiSensor* pNuiSensor);

    /// <summary>
    /// Notify the specified Kinect window to close and delete itself
    /// </summary>
    void CloseKinectWindow(KinectWindow** ppKinectWindow);

private:

    /// Handle of the main window
    HWND    m_hWnd;

    /// Save all the sensors and their associated windows, and the sensor instance name is the map key
    std::map<std::wstring, SensorWindowPair> m_sensorMap;

    /// The parameter of showing kinect windows
    DWORD    m_kinectWindowShowParam;
};
