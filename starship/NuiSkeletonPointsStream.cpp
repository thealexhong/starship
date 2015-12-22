/**
* NuiSkeletonPointsStream.cpp
* alex.hong@mail.utoronto.ca
*/

#include "stdafx.h"
#include "NuiSkeletonPointsStream.h"
#include "Utility.h"

NuiSkeletonPointsStream::NuiSkeletonPointsStream(INuiSensor* pNuiSensor)
	: m_pNuiSensor(pNuiSensor)
	, m_pSkeletonPointsViewer(nullptr)
{
	if (m_pNuiSensor)
	{
		m_pNuiSensor->AddRef();
	}
}

NuiSkeletonPointsStream::~NuiSkeletonPointsStream()
{
	SafeRelease(m_pNuiSensor);
}


HRESULT NuiSkeletonPointsStream::StartStream()
{
	return S_OK;
}

void NuiSkeletonPointsStream::SetStreamViewer(NuiSkeletonPointsViewer* pViewer)
{
	m_pSkeletonPointsViewer = pViewer;
}

void NuiSkeletonPointsStream::ProcessStream()
{
	// Get the reading
	// NEED TO CHANGE
	Vector4 reading;
	//NUI_SKELETON_FRAME skeletonFrame = { 0 };
	HRESULT hr = m_pNuiSensor->NuiAccelerometerGetCurrentReading(&reading);
	// HRESULT hr = m_pNuiSensor->NuiSkeletonGetNextFrame(0, &skeletonFrame);
	

	if (SUCCEEDED(hr) && m_pSkeletonPointsViewer)
	{
		// Set the reading to viewer
		// NUI_SKELETON_TRACKING_STATE trackingState = skeletonFrame.SkeletonData[0].eTrackingState;
		//if (trackingState == NUI_SKELETON_TRACKED) {
		/*
			NUI_SKELETON_DATA * skeletonData = &skeletonFrame.SkeletonData[0];
			m_pSkeletonPointsViewer->SetSkeletonPointsReadings(skeletonData->SkeletonPositions[NUI_SKELETON_POSITION_HAND_RIGHT].y, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0);
				*/
		//}
		m_pSkeletonPointsViewer->SetSkeletonPointsReadings(reading.x, reading.y, reading.z,
			                                               reading.x, reading.y, reading.z,
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z, 
														   reading.x, reading.y, reading.z);
														  
	}
}