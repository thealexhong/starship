//------------------------------------------------------------------------------
// <copyright file="NuiActivityWatcher.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <cmath>
#include "NuiActivityWatcher.h"

#define ACTIVITY_FALLOFF    0.98f

/// <summary>
/// Constructor
/// </summary>
/// <param name="skeleton">Referece to skeleton data</param>
NuiActivityWatcher::NuiActivityWatcher(NUI_SKELETON_DATA& skeleton)
{
    m_updated       = false;
    m_trackingID    = skeleton.dwTrackingID;
    m_prevPosition  = skeleton.Position;
    m_activityLevel = 0.0f;

    ZeroMemory(&m_prevDelta, sizeof(m_prevDelta));
}

/// <summary>
/// Destructor
/// </summary>
NuiActivityWatcher::~NuiActivityWatcher()
{
}

/// <summary>
/// Set or reset update status
/// </summary>
/// <param name="updated">True to set. False to reset</param>
void NuiActivityWatcher::SetUpdateFlag(bool updated)
{
    m_updated = updated;
}

/// <summary>
/// Get update status
/// </summary>
/// <returns>Indicates if it's updated</returns>
bool NuiActivityWatcher::GetUpdateFlag()
{
    return m_updated;
}

/// <summary>
/// Calculate new activity level based on skeleton new position and old activity level
/// </summary>
/// <param name="skeleton">Skeleton data containing skeleton positions</param>
void NuiActivityWatcher::UpdateActivity(NUI_SKELETON_DATA& skeleton)
{
    // Caculate skeleton movement
    FLOAT deltaX = skeleton.Position.x - m_prevPosition.x;
    FLOAT deltaY = skeleton.Position.y - m_prevPosition.y;
    FLOAT deltaZ = skeleton.Position.z - m_prevPosition.z;

    // Save skeleton new position
    m_prevPosition = skeleton.Position;

    // Calculate different between new movement and old movement
    FLOAT diffX = deltaX - m_prevDelta.x;
    FLOAT diffY = deltaY - m_prevDelta.y;
    FLOAT diffZ = deltaZ - m_prevDelta.z;

    // Save skeleton new movement
    m_prevDelta.x = deltaX;
    m_prevDelta.y = deltaY;
    m_prevDelta.z = deltaZ;

    float deltaLength = sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
    m_activityLevel *= ACTIVITY_FALLOFF;
    m_activityLevel += deltaLength;
}

/// <summary>
/// Get calculated activity level
/// </summary>
FLOAT NuiActivityWatcher::GetActivityLevel()
{
    return m_activityLevel;
}