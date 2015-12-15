//------------------------------------------------------------------------------
// <copyright file="NuiActivityWatcher.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <NuiApi.h>

/// <summary>
/// This class used to track the activity of a given player over time, which can be 
/// used to assist the NuiSkeletonStream when determing which player to track.
/// </summary>
class NuiActivityWatcher
{
public:
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="skeleton">Referece to skeleton data</param>
    NuiActivityWatcher(NUI_SKELETON_DATA& skeleton);

    /// <summary>
    /// Destructor
    /// </summary
   ~NuiActivityWatcher();

public:
    /// <summary>
    /// Set or reset update status
    /// </summary>
    /// <param name="updated">True to set. False to reset</param>
    void SetUpdateFlag(bool updated);

    /// <summary>
    /// Get update status
    /// </summary>
    /// <returns>Indicates if it's updated</returns>
    bool GetUpdateFlag();

    /// <summary>
    /// Calculate new activity level based on skeleton new position and old activity level
    /// </summary>
    /// <param name="skeleton">Skeleton data containing skeleton positions</param>
    void UpdateActivity(NUI_SKELETON_DATA& skeleton);

    /// <summary>
    /// Get calculated activity level
    /// </summary>
    FLOAT GetActivityLevel();

private:
    bool    m_updated;
    DWORD   m_trackingID;
    FLOAT   m_activityLevel;
    Vector4 m_prevPosition;
    Vector4 m_prevDelta;
};