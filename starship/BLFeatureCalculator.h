/**
* BLFeatureCalculator.h
* alex.hong@mail.utoronto.ca
*/

#pragma once

#include <NuiApi.h>
#include <vector>
#include <cmath>

class BLFeatureCalculator
{
public:
	BLFeatureCalculator(std::vector<NUI_SKELETON_DATA> skeletonData, UINT frames, UINT fps, bool seated);
	~BLFeatureCalculator();

public:
	FLOAT bow_stretch_trunk();
	FLOAT open_close_arms();
	FLOAT vert_head();
	FLOAT fwd_bwd_head();
	FLOAT vert_motion_body();
	FLOAT fwd_bwd_motion_body();
	FLOAT expand_body();
	FLOAT spd_body();
private:
	FLOAT average(FLOAT a, FLOAT b);
	FLOAT average(FLOAT a, FLOAT b, FLOAT c, FLOAT d);
	FLOAT norm(FLOAT x, FLOAT y, FLOAT z);


private:
	UINT m_frames;
	UINT m_fps;
	bool m_seated;
	std::vector<NUI_SKELETON_DATA>m_skeletonData;
};
