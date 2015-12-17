//------------------------------------------------------------------------------
// <copyright file="KinectWindowManager.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <vector>

#include "KinectWindowManager.h"
#include "Utility.h"

/// <summary>
/// Constructor
/// </summary>
/// <param name="hWnd">Handle to the kinect window.</param>
KinectWindowManager::KinectWindowManager(HWND hWnd)
    : m_hWnd(hWnd)
    , m_kinectWindowShowParam(0)
{
}

/// <summary>
/// Destructor
/// </summary>
KinectWindowManager::~KinectWindowManager()
{
    for (auto iter = m_sensorMap.begin(); iter != m_sensorMap.end(); ++iter)
    {
        SafeRelease(iter->second.NuiSensor);
        iter->second.KinectWindow = nullptr;
    }
}

/// <summary>
/// Update the sensor resource when sensor status changed and the new status is NuiSensorDisconnected
/// </summary>
void KinectWindowManager::HandleSensorDisconnected(PCWSTR instanceName)
{
    auto iter = m_sensorMap.find(instanceName);

    if (iter != m_sensorMap.end())
    {
        // Close the kinect window
        CloseKinectWindow(&(iter->second.KinectWindow));

        RemoveSensor(iter);
    }
}

/// <summary>
/// Update the sensor resource when sensor status changed and the new status is not NuiSensorDisconnected
/// </summary>
void KinectWindowManager::HandleSensorConnected(PCWSTR instanceName, HRESULT hrSensorCallbackStatus)
{
    auto iter = m_sensorMap.find(instanceName);

    // The changed sensor has not been saved
    if (m_sensorMap.end() == iter)
    {
        INuiSensor* pNuiSensor = nullptr;

        if (SUCCEEDED(NuiCreateSensorById(instanceName, &pNuiSensor)))
        {
            iter = AddSensor(instanceName, pNuiSensor);
        }
        else
        {
            return;
        }
    }

    // Show/Hide the Kinect window
    UpdateKinectWindow(hrSensorCallbackStatus, iter->second.NuiSensor, &(iter->second.KinectWindow));
}

/// <summary>
/// Reset the Kinect window of the specified sensor to null
/// </summary>
void KinectWindowManager::ResetKinectWindow(PCWSTR instanceName)
{
    auto iter = m_sensorMap.find(instanceName);

    if (iter != m_sensorMap.end())
    {
        iter->second.KinectWindow = nullptr;
    }
}

/// <summary>
/// Close all the Kinect windows
/// </summary>
void KinectWindowManager::CloseAllKinectWindows()
{
    std::vector<HANDLE> kinectWindowThreadHandleVec;
    for (auto iter = m_sensorMap.begin(); iter != m_sensorMap.end(); ++iter)
    {
        if (nullptr != iter->second.KinectWindow)
        {
            kinectWindowThreadHandleVec.push_back(iter->second.KinectWindow->GetThreadHandle());
            iter->second.KinectWindow->NotifyOfExit();
        }
    }

    if (!kinectWindowThreadHandleVec.empty())
    {
        WaitForMultipleObjects((DWORD)kinectWindowThreadHandleVec.size(), kinectWindowThreadHandleVec.data(), TRUE, INFINITE);
    }

    // Close all the kinect window thread handles
    for (auto iter = kinectWindowThreadHandleVec.begin(); iter != kinectWindowThreadHandleVec.end(); ++iter)
    {
        CloseHandle(*iter);
    }
}

/// <summary>
/// Insert a new sensor to the resource map
/// </summary>
SensorMapIterator KinectWindowManager::AddSensor(PCWSTR instanceName, INuiSensor* pNuiSensor)
{
    assert(nullptr != pNuiSensor);

    SensorWindowPair resourceInstance = {pNuiSensor, nullptr};
    return m_sensorMap.insert(std::make_pair(std::wstring(instanceName), resourceInstance)).first;
}

/// <summary>
/// Remove the specified item from the resource map
/// </summary>
void KinectWindowManager::RemoveSensor(SensorMapIterator iter)
{
    assert (iter != m_sensorMap.end());

    SafeRelease(iter->second.NuiSensor);

    m_sensorMap.erase(iter);
}

/// <summary>
/// Update the sensor related Kinect window status
/// </summary>
void KinectWindowManager::UpdateKinectWindow(HRESULT hrSensorCallbackStatus, INuiSensor* pNuiSensor, KinectWindow** ppKinectWindow)
{
    if (S_OK == hrSensorCallbackStatus && nullptr != ppKinectWindow)
    {
        *ppKinectWindow = CreateKinectWindow(pNuiSensor);
    }
    else
    {
        CloseKinectWindow(ppKinectWindow);
    }
}

/// <summary>
/// Create a Kinect window to display the sensor
/// </summary>
KinectWindow* KinectWindowManager::CreateKinectWindow(INuiSensor* pNuiSensor)
{
    KinectWindow* pKinectWindow = new KinectWindow(GetModuleHandle(0), m_hWnd, pNuiSensor);
    pKinectWindow->StartWindow();
    PostMessageW(pKinectWindow->GetWindow(), WM_SHOWKINECTWINDOW, m_kinectWindowShowParam, 0);
    return pKinectWindow;
}

/// <summary>
/// Notify the specified Kinect window to close and delete itself
/// </summary>
void KinectWindowManager::CloseKinectWindow(KinectWindow** ppKinectWindow)
{
    if (nullptr != ppKinectWindow && nullptr != *ppKinectWindow)
    {
        SendMessageW((*ppKinectWindow)->GetWindow(), WM_CLOSE, CLOSING_FROM_STATUSCHANGED, 0);
        *ppKinectWindow = nullptr;
    }
}

/// <summary>
/// Show all the kinect windows
/// </summary>
void KinectWindowManager::ShowAllKinectWindows()
{
    for (auto iter = m_sensorMap.begin(); iter != m_sensorMap.end(); ++iter)
    {
        if (nullptr != iter->second.KinectWindow)
        {
            PostMessageW(iter->second.KinectWindow->GetWindow(), WM_SHOWKINECTWINDOW, 1, 0);
        }
    }
}