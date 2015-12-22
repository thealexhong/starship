/**
* BLFeatureCalculator.cpp
* alex.hong@mail.utoronto.ca
* Some math: Search for Derek McColl thesis doc for more information
*/

#include "BLFeatureCalculator.h"

BLFeatureCalculator::BLFeatureCalculator(std::vector<NUI_SKELETON_DATA> skeletonData, unsigned int frames)
{
	m_skeletonData = skeletonData;
	if (frames > 0)
		m_frames = frames;
	else
		m_frames = 1;
}

BLFeatureCalculator::~BLFeatureCalculator()
{
}

FLOAT BLFeatureCalculator::bow_stretch_trunk()
{
	FLOAT result = 0;
	for (int i = 0; i < m_frames; i++)
	{
		FLOAT yshoulder = average(m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_LEFT].y,
			                     m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_RIGHT].y);
		FLOAT zshoulder = average(m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_LEFT].z,
			                      m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_RIGHT].z);
		FLOAT yhip = average(m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_LEFT].y,
			                 m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_RIGHT].y);
		FLOAT zhip = average(m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_LEFT].z,
			                 m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_RIGHT].z);
		if ((zshoulder - zhip) != 0)
			result += atan((yshoulder - yhip)/(zshoulder - zhip));
	}
	result /= m_frames;
	return result;
}
FLOAT BLFeatureCalculator::open_close_arms()
{
	FLOAT result = 0;
	for (int i = 0; i < m_frames; i++)
	{
		FLOAT xtrunkcenter = average(m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_LEFT].x,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_RIGHT].x,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_LEFT].x,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_RIGHT].x);
		FLOAT ytrunkcenter = average(m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_LEFT].y,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_RIGHT].y,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_LEFT].y,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_RIGHT].y);
		FLOAT ztrunkcenter = average(m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_LEFT].z,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_RIGHT].z,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_LEFT].z,
			m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HIP_RIGHT].z);

		FLOAT xleft = m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HAND_LEFT].x - xtrunkcenter;
		FLOAT yleft = m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HAND_LEFT].y - ytrunkcenter;
		FLOAT zleft = m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HAND_LEFT].z - ztrunkcenter;
		FLOAT xright = m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HAND_RIGHT].x - xtrunkcenter;
		FLOAT yright = m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HAND_RIGHT].y - ytrunkcenter;
		FLOAT zright = m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HAND_RIGHT].z - ztrunkcenter;

		result += 1 / 2 * norm(xleft, yleft, zleft) + 1 / 2 * norm(xright, yright, zright);
	}
	result /= m_frames;
	return result;
}

FLOAT BLFeatureCalculator::vert_head()
{
	FLOAT result = 0;
	for (int i = 0; i < m_frames; i++)
	{
		result += m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HEAD].y - m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_CENTER].y; // shoulder center = neck
	}

	result /= m_frames;
	return result;
}

FLOAT BLFeatureCalculator::fwd_bwd_head()
{
	FLOAT result = 0;
	for (int i = 0; i < m_frames; i++)
	{
		result += m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_HEAD].z - m_skeletonData[i].SkeletonPositions[NUI_SKELETON_POSITION_SHOULDER_CENTER].z; // shoulder center = neck
	}

	result /= m_frames;
	return result;
}

FLOAT BLFeatureCalculator::vert_motion_body()
{
	FLOAT result = 0;
	for (int i = 0; i < m_frames - 1; i++)
	{
		// TODO: could be improved by replacing NUI_SKELETON_POSITION_COUNT with active points instead
		// or detect if user is seated...
		for (int j = 0; j < NUI_SKELETON_POSITION_COUNT; j++)
		{
			result += m_skeletonData[i + 1].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].y - m_skeletonData[i].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].y;
		}
		result += 1 / NUI_SKELETON_POSITION_COUNT;
	}
	result /= (m_frames - 1);
	return result;
}

FLOAT BLFeatureCalculator::fwd_bwd_motion_body()
{
	FLOAT result = 0;
	for (int i = 0; i < m_frames - 1; i++)
	{
		// TODO: could be improved by replacing NUI_SKELETON_POSITION_COUNT with active points instead
		// or detect if user is seated...
		for (int j = 0; j < NUI_SKELETON_POSITION_COUNT; j++)
		{
			result += m_skeletonData[i + 1].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].z - m_skeletonData[i].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].z;
		}
		result += 1 / NUI_SKELETON_POSITION_COUNT;
	}
	result /= (m_frames - 1);
	return result;
}

FLOAT BLFeatureCalculator::expand_body()
{
	FLOAT result = 0;
	for (int i = 0; i < m_frames; i++)
	{
		FLOAT xmax = -1, xmin = FLT_MAX,
			  ymax = -1, ymin = FLT_MAX,
			  zmax = -1, zmin = FLT_MAX;

		// O(n) search for max, min for x,y,z
		for (int j = 0; j < NUI_SKELETON_POSITION_COUNT; j++)
		{
			FLOAT x = m_skeletonData[i].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].x;
			FLOAT y = m_skeletonData[i].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].y;
			FLOAT z = m_skeletonData[i].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].z;

			if (x > xmax) xmax = x;
			if (x < xmin) xmin = x;
			if (y > ymax) ymax = y;
			if (y < ymin) ymin = y;
			if (z > zmax) zmax = z;
			if (z < zmin) zmin = z;
		}
		result += (xmax - xmin) * (ymax - ymin) * (zmax - zmin);
	}
	result /= m_frames;
	return result;
}

FLOAT BLFeatureCalculator::spd_body()
{
	FLOAT result = 0;
	for (int i = 0; i < m_frames - 1; i++)
	{
		// TODO: could be improved by replacing NUI_SKELETON_POSITION_COUNT with active points instead
		// or detect if user is seated...
		for (int j = 0; j < NUI_SKELETON_POSITION_COUNT; j++)
		{
			FLOAT x = m_skeletonData[i + 1].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].x - m_skeletonData[i].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].x;
			FLOAT y = m_skeletonData[i + 1].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].y - m_skeletonData[i].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].y;
			FLOAT z = m_skeletonData[i + 1].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].z - m_skeletonData[i].SkeletonPositions[(NUI_SKELETON_POSITION_INDEX)j].z;
			int timeDiff = m_frames * 30; // 30 FPS
			result += (norm(x, y, z) / timeDiff);
		}
		result += 1 / NUI_SKELETON_POSITION_COUNT;
	}
	result /= (m_frames - 1);
	return result;
}

FLOAT BLFeatureCalculator::average(FLOAT a, FLOAT b)
{
	return 1 / 2 * (a + b);
}

FLOAT BLFeatureCalculator::average(FLOAT a, FLOAT b, FLOAT c, FLOAT d)
{
	return 1 / 4 * (a + b + c + d);
}

FLOAT BLFeatureCalculator::norm(FLOAT x, FLOAT y, FLOAT z)
{
	return sqrtf((x * x) + (y * y) + (z * z));
}


