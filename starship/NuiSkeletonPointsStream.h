/**
* NuiSkeletonPointsStream.h
* alex.hong@mail.utoronto.ca
*/
#pragma once

#include <NuiApi.h>
#include "NuiSkeletonPointsViewer.h"

class NuiSkeletonPointsStream
{
public:
	NuiSkeletonPointsStream(INuiSensor* pNuiSensor);
	~NuiSkeletonPointsStream();

public:
	void SetStreamViewer(NuiSkeletonPointsViewer* pViewer);
	void ProcessStream();
	HRESULT StartStream();

private:
	INuiSensor*              m_pNuiSensor;
	NuiSkeletonPointsViewer* m_pSkeletonPointsViewer;
};