/**
* NuiStreamViewer.cpp
* Modfied from Kinect SDK v.1.8
* alex.hong@mail.utoronto.ca
*/

#include "stdafx.h"
#include <stdio.h>
#include <iostream>
#include <string>
#include <sstream>
#include <fstream>

#include "NuiStreamViewer.h"
#include "resource.h"
#include "BLFeatureCalculator.h"

// TODO: a struct of arousal and valence. They always come in a pair, makes code more readable. but I'm lazy...
// TODO: all classification stuff should happen in a seperate file

/// <summary>
/// Constructor
/// </summary>
/// <param name="pParent">The pointer to parent window</param>
NuiStreamViewer::NuiStreamViewer(const NuiViewer* pParent, bool displayCoordinates) : NuiViewer(pParent),
                                                             m_imageType(NUI_IMAGE_TYPE_COLOR),
															 m_pImage(nullptr),
															 m_pauseSkeleton(false),
															 m_seated(false),
															 m_pSkeletonFrame(nullptr),
															 m_drawEdgeFlags(0),
															 m_frameCount(0),
															 m_lastFrameCount(0),
															 m_frameTracker(0),
															 m_fps(0)
{
    m_pImageRenderer = new ImageRenderer();
	m_displayCoordinates = displayCoordinates;
    m_lastTick = GetTickCount();
}

/// <summary>
/// Destructor
/// </summary>
NuiStreamViewer::~NuiStreamViewer()
{
    SafeDelete(m_pImageRenderer);
	//SafeDelete(m_pSkeletonPointsViewer);
	//SafeDelete(m_pBLFeatureViewer);
	//SafeDelete(m_pBLClassificationViewer);
}

void NuiStreamViewer::SetStreamViewer(NuiSkeletonPointsViewer* pViewer, NuiBLFeatureViewer* pBLFeatureViewer, NuiBLClassificationViewer* pBLClassificationViewer, 
	                                  NuiMClassificationViewer* pMClassificationViewer)
{
	m_pSkeletonPointsViewer = pViewer;
	m_pBLFeatureViewer = pBLFeatureViewer;
	m_pBLClassificationViewer = pBLClassificationViewer;
	m_pMClassificationViewer = pMClassificationViewer;
}

void NuiStreamViewer::SetSeated(bool seated)
{
	m_seated = seated;
}

/// <summary>
/// Dispatch window message to message handlers.
/// </summary>
/// <param name="hWnd">Handle to window</param>
/// <param name="uMsg">Message type</param>
/// <param name="wParam">Extra message parameter</param>
/// <param name="lParam">Extra message parameter</param>
/// <returns>
/// If message is handled, non-zero is returned. Otherwise FALSE is returned and message is passed to default dialog procedure
/// </returns>
LRESULT NuiStreamViewer::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch (uMsg)
    {
    case WM_CTLCOLORDLG:
        // Set background color as black
        return (LRESULT)GetStockObject(BLACK_BRUSH);

    case WM_PAINT:
        OnPaint(wParam, lParam);
        break;

    case WM_SIZE:
        {
            UINT width  = LOWORD(lParam);
            UINT height = HIWORD(lParam);
            m_pImageRenderer->ResizeRenderTarget(width, height);
        }
        break;

    default:
        break;
    }

    return (LRESULT)FALSE;
}

/// <summary>
/// Returns the ID of the dialog
/// </summary>
/// <returns>ID of dialog</returns>
UINT NuiStreamViewer::GetDlgId()
{
    return IDD_STREAM_VIEW;
}

/// <summary>
/// Message handler of WM_PAINT.
/// </summary>
/// <param name="wParam">Extra message parameter</param>
/// <param name="lParam">Extra message parameter</param>
void NuiStreamViewer::OnPaint(WPARAM wParam, LPARAM lParam)
{
    HRESULT hr = m_pImageRenderer->BeginDraw(m_hWnd);
    if (FAILED(hr))
        return;

    // Get viewer window client rect
    RECT clientRect;
    if (!::GetClientRect(m_hWnd, &clientRect))
    {
        return;
    }

    // Calculate the area the stream image is to streched to fit
    D2D1_RECT_F imageRect = GetImageRect(clientRect);

    // Draw stream images
    DrawImage(imageRect);

    // Draw skeletons
    DrawSkeletons(imageRect);

    // Draw image resolution
    DrawResolution(clientRect);

    // Draw FPS
    DrawFPS(clientRect);

    // Draw red edges if skeleton is close to edges of image
    DrawRedEdges(imageRect);

    m_pImageRenderer->EndDraw();
}

/// <summary>
/// Draw the image on screen by D2D
/// </summary>
/// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
void NuiStreamViewer::DrawImage(const D2D1_RECT_F& imageRect)
{
    if (m_pImage && m_pImage->GetBufferSize())
    {
        D2D1_SIZE_U imageSize = D2D1::SizeU(m_pImage->GetWidth(), m_pImage->GetHeight());
        m_pImageRenderer->DrawImage(m_pImage->GetBuffer(), imageSize, imageRect);
    }
}

/// <summary>
/// Draw skeletons
/// </summary>
/// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
void NuiStreamViewer::DrawSkeletons(const D2D1_RECT_F& imageRect)
{
    if (m_pSkeletonFrame && !m_pauseSkeleton)
    {
        // Clip the area to avoid drawing outside the image
        m_pImageRenderer->SetClipRect(imageRect);

        for (int i = 0; i < NUI_SKELETON_COUNT; i++)
        {
            NUI_SKELETON_TRACKING_STATE state = m_pSkeletonFrame->SkeletonData[i].eTrackingState;
            if (NUI_SKELETON_TRACKED == state)
            {
                // Draw bones and joints of tracked skeleton
                DrawSkeleton(m_pSkeletonFrame->SkeletonData[i], imageRect);
            }
            else if (NUI_SKELETON_POSITION_ONLY == state)
            {
                DrawPosition(m_pSkeletonFrame->SkeletonData[i], imageRect);
            }
        }

        m_pImageRenderer->ResetClipRect();
    }
}

/*
 * Create ARFF file for body language (Derek's format)
 */
void createBLTestData(std::string filename, std::string response, std::string nomAttribute,
	float volume, float speed, float armopen,
	float headfb, float headud, float trunkangle,
	float freq, float deltay, float deltaz) {
	std::ofstream myfile;
	myfile.open(filename);
	myfile << "@relation " << response.c_str() << "\n\n"
		<< "@attribute volume real\n"
		<< "@attribute speed real\n"
		<< "@attribute armopen real\n"
		<< "@attribute headfb real\n"
		<< "@attribute headud real\n"
		<< "@attribute trunkangle real\n"
		<< "@attribute freq real\n"
		<< "@attribute deltay real\n"
		<< "@attribute deltaz real\n"
		<< "@attribute " << nomAttribute.c_str() << "\n\n"
		//<< "@attribute arousal{ 0, 1, 2, 3, 4 }\n\n"
		<< "@data\n"
		<< volume << "," << speed << "," << armopen << ","
		<< headfb << "," << headud << "," << trunkangle << ","
		<< freq << "," << deltay << "," << deltaz << ",?";
	myfile.close();

	/* Sample output
	@relation BLArousalResponse

	@attribute volume real
	@attribute speed real
	@attribute armopen real
	@attribute headfb real
	@attribute headud real
	@attribute trunkangle real
	@attribute freq real
	@attribute deltay real
	@attribute deltaz real
	@attribute arousal {0,1,2,3,4}

	@data
	1.096175289,0.031499748,0.403615896,0.170590255,0.375560715,0.952752455,0,0.223848659,-0.070440821,?
	*/
}

/*
 * Create a batch file for running weka
 */
void createBatWekaFile(std::string filename, std::string path_to_java,
                                             std::string path_to_weka,
                                             std::string weka_classifier,
                                             std::string path_to_weka_model,
                                             std::string path_to_test_data,
	                                         std::string outFilename) {
	std::ofstream myfile;
	myfile.open(filename);	
	myfile << "\"" << path_to_java.c_str() << "\"" << " -cp "
		   << "\"" << path_to_weka.c_str() << "\"" << " "
		   << weka_classifier.c_str() << " -l "
		   << "\""<<path_to_weka_model.c_str()<<"\"" << " -T "
		   << "\""<<path_to_test_data.c_str()<<"\"" << " -p 0 > " 
		   << "\"" << outFilename.c_str() << "\"";
	myfile.close();

	/* Sample Output
	"C:\Program Files\Java\jdk1.8.0_05\bin\java.exe" -cp "C:\Program Files (x86)/Weka-3-6/weka.jar" weka.classifiers.trees.RandomForest -l C:\Users\Alex\Desktop\starship\starship\TrainingData\BLArousalTrain.model -T C:\Users\Alex\Desktop\starship\starship\TrainingData\BLArousalTest.arff -p 0
	*/
}

/*
 * Get predicted value from Weka
 */
FLOAT getWekaResult(std::string execFile, std::string outFilename) {
	system(("\"" + execFile + "\"").c_str());
	// parse outFilename for results
	// some bad code because i'm tired
	std::string line;
	std::ifstream myfile(outFilename);
	if (myfile.is_open())
	{
		while (getline(myfile, line))
		{
			std::size_t found = line.find(":", 0);
			found = line.find(":", found + 1);
			if (found != std::string::npos) {
				std::string out = "";
				for (int i = (int)(found + 1); i < (int)line.length(); i++)
				{
					if (i == ' ')
						return std::stof(out);
					out += line[i];
				}
			}
		}
		myfile.close();
	}
	return 0.0;
}

/// <summary>
/// Draw skeleton.
/// </summary>
/// <param name="skeletonData">Skeleton coordinates</param>
/// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
void NuiStreamViewer::DrawSkeleton(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect)
{
    // Torso
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_HEAD,               NUI_SKELETON_POSITION_SHOULDER_CENTER);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_SHOULDER_CENTER,    NUI_SKELETON_POSITION_SHOULDER_LEFT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_SHOULDER_CENTER,    NUI_SKELETON_POSITION_SHOULDER_RIGHT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_SHOULDER_CENTER,    NUI_SKELETON_POSITION_SPINE);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_SPINE,              NUI_SKELETON_POSITION_HIP_CENTER);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_HIP_CENTER,         NUI_SKELETON_POSITION_HIP_LEFT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_HIP_CENTER,         NUI_SKELETON_POSITION_HIP_RIGHT);

    // Left arm
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_SHOULDER_LEFT,      NUI_SKELETON_POSITION_ELBOW_LEFT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_ELBOW_LEFT,         NUI_SKELETON_POSITION_WRIST_LEFT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_WRIST_LEFT,         NUI_SKELETON_POSITION_HAND_LEFT);

    // Right arm
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_SHOULDER_RIGHT,     NUI_SKELETON_POSITION_ELBOW_RIGHT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_ELBOW_RIGHT,        NUI_SKELETON_POSITION_WRIST_RIGHT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_WRIST_RIGHT,        NUI_SKELETON_POSITION_HAND_RIGHT);

    // Left leg
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_HIP_LEFT,           NUI_SKELETON_POSITION_KNEE_LEFT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_KNEE_LEFT,          NUI_SKELETON_POSITION_ANKLE_LEFT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_ANKLE_LEFT,         NUI_SKELETON_POSITION_FOOT_LEFT);

    // Right leg
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_HIP_RIGHT,          NUI_SKELETON_POSITION_KNEE_RIGHT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_KNEE_RIGHT,         NUI_SKELETON_POSITION_ANKLE_RIGHT);
    DrawBone(skeletonData, imageRect, NUI_SKELETON_POSITION_ANKLE_RIGHT,        NUI_SKELETON_POSITION_FOOT_RIGHT);

    // Draw joints
    for (int i = 0; i < NUI_SKELETON_POSITION_COUNT; i++)
    {
        DrawJoint(skeletonData, imageRect, (NUI_SKELETON_POSITION_INDEX)i);
    }

	if (m_displayCoordinates) {
		m_pSkeletonPointsViewer->SetSkeletonPointsReadings(skeletonData);
		m_skeletonData.push_back(skeletonData);
		m_frameTracker++;
		

		// Frequency of classification
		int time = 5;
		int frames = time * m_fps;

		// When starting, somethimes m_fps = 1.. This avoids lag issues when Kinect is connected
		if (m_fps < 10)
			frames = time * 30;

		if ((int)m_frameTracker >= frames)
		{

			m_frameTracker = 0;

			/*
			   Calculate Body Language
			*/
			
			BLFeatureCalculator * myBLFeatures = new BLFeatureCalculator(m_skeletonData, frames, m_fps, m_seated);

			m_pBLFeatureViewer->SetFeatureReadings(myBLFeatures->bow_stretch_trunk(),
				                                   myBLFeatures->open_close_arms(), 
												   myBLFeatures->vert_head(), 
												   myBLFeatures->fwd_bwd_head(), 
												   myBLFeatures->vert_motion_body(), 
												   myBLFeatures->fwd_bwd_motion_body(), 
												   myBLFeatures->expand_body(), 
												   myBLFeatures->spd_body()); // another parameter for seated
			
			/**
			 * ARFF file format (by Derek)
			 * volume      = Expansiveness               expand_body()
			 * speed       = Speed of Body               spd_body()
			 * armopen     = Open / Close arms           open_close_arms()
			 * headfb      = Forward / Backward Head     fwd_bwd_head()
			 * headud      = Vertical Head               vert_head()
			 * trunkangle  = Bowing / Stretching Trunk   bow_stretch_trunk()
			 * freq        = ALWAYS ZERO
			 * deltay      = Vertical motion Body        vert_motion_body()
			 * deltaz      = Forward /Backward Body      fwd_bwd_motion_body()
			 */

			/* Some bad program design here because I'm tired */

			// create test data
			std::string path_to_blvalenceTest = "testDataFiles\\blvalence_test.arff";
			std::string path_to_blarousalTest = "testDataFiles\\blarousal_test.arff";
			
			createBLTestData(path_to_blvalenceTest, "BLValenceResponse", "valence {-2,-1,0,1,2}", myBLFeatures->expand_body(),
				myBLFeatures->spd_body(), myBLFeatures->open_close_arms(), myBLFeatures->fwd_bwd_head(), myBLFeatures->vert_head(),
				myBLFeatures->bow_stretch_trunk(), 0, myBLFeatures->vert_motion_body(), myBLFeatures->fwd_bwd_motion_body());
			createBLTestData(path_to_blarousalTest, "BLArousalResponse", "arousal {0,1,2,3,4}", myBLFeatures->expand_body(),
				myBLFeatures->spd_body(), myBLFeatures->open_close_arms(), myBLFeatures->fwd_bwd_head(), myBLFeatures->vert_head(),
				myBLFeatures->bow_stretch_trunk(), 0, myBLFeatures->vert_motion_body(), myBLFeatures->fwd_bwd_motion_body());

			// create batch file with classifier
			// TODO: change these to relative directories and use environment variables instead
			// TODO: these variables shouldn't be in loop...

			// ATTENTION: Change these variables when using another computer, you can make code better by changing to local directory by copying .exe,.jar file locally
			/*****************************************************************************************************************************************************************************/
			/*****************************************************************************************************************************************************************************/
			// Personal Workstation
			std::string path_to_local_dir = "C:\\Users\\Alex\\Desktop\\starship\\starship\\";
			std::string path_to_java = "C:\\Program Files\\Java\\jdk1.8.0_05\\bin\\java.exe";
			std::string path_to_weka = "C:\\Program Files (x86)\\Weka-3-6\\weka.jar";
			/*
			// Workstation that powers Brian. ASBlab.
			std::string path_to_local_dir = "C:\\Users\\ASB Workstation\\Desktop\\starship\\starship\\";
			std::string path_to_java = "C:\\Program Files (x86)\\Java\\jre7\\bin\\java.exe";
			std::string path_to_weka = "C:\\Program Files\\Weka-3-6\\weka.jar";
			*/
			/*****************************************************************************************************************************************************************************/
			/*****************************************************************************************************************************************************************************/

			std::string blvalence_classifier = "weka.classifiers.functions.RBFNetwork";
			std::string blarousal_classifier = "weka.classifiers.trees.RandomForest";
			
			std::string path_to_BLArousalTrainingModel = path_to_local_dir + "TrainingData\\BLArousalTrain.model";
			std::string path_to_BLValenceTrainingModel = path_to_local_dir + "TrainingData\\BLValenceTrain.model";
			std::string path_to_BLArousalTestData = path_to_local_dir + "testDataFiles\\blarousal_test.arff";
			std::string path_to_BLValenceTestData = path_to_local_dir+"testDataFiles\\blvalence_test.arff";
			std::string path_to_BLarousalBat = path_to_local_dir + "batFiles\\blarousal.bat";
			std::string path_to_BLvalenceBat = path_to_local_dir + "batFiles\\blvalence.bat";
			std::string path_to_BLoutArousal = path_to_local_dir + "wekaOutputFiles\\bloutArousal.txt";
			std::string path_to_BLoutValence = path_to_local_dir + "wekaOutputFiles\\bloutValence.txt";

			/*
			 This is a hack to use Weka with C++. Weka comes with command line functionality. We'll write a .bat file and execute that, then read it's output.
			*/
			createBatWekaFile(path_to_BLvalenceBat, path_to_java, path_to_weka, blvalence_classifier, path_to_BLValenceTrainingModel, path_to_BLValenceTestData, path_to_BLoutValence);
			createBatWekaFile(path_to_BLarousalBat, path_to_java, path_to_weka, blarousal_classifier, path_to_BLArousalTrainingModel, path_to_BLArousalTestData, path_to_BLoutArousal);
			
			// execute batch file and read the output
			FLOAT blvalence = getWekaResult(path_to_BLvalenceBat, path_to_BLoutValence);
			FLOAT blarousal = getWekaResult(path_to_BLarousalBat, path_to_BLoutArousal);
			
			m_pBLClassificationViewer->SetAffectReadings(blvalence,blarousal); // display it on GUI


			// Voice retrieval from Yuma's code
			// TODO: Open TCP/IP socket is better than writing and reading to a file... but we're short on time :P
			// This should be in a method......................... But I hope you follow along.

			// calculate average voice affect over time
			FLOAT vvalence = 0;
			FLOAT varousal = 0;

			std::vector<FLOAT> vvalencevalues;
			std::vector<FLOAT> varousalvalues;

			std::string line;
			std::string path_to_voicefile ="..\\Voice Analysis\\voiceOutput.txt";
			std::ifstream voicefile(path_to_voicefile);
			
			// Open voice file
			if (voicefile.is_open())
			{
				while(getline(voicefile, line)){
					// replace , with space
					line.replace(line.find(","), 1, " ");
					std::istringstream in(line);
					FLOAT tmp_valence, tmp_arousal;
					// read
					in >> tmp_valence >> tmp_arousal;
					vvalencevalues.push_back(tmp_valence);
					varousalvalues.push_back(tmp_arousal);
				}
				voicefile.close();
			}

			// check if empty, then calculate averages
			if (!vvalencevalues.empty()) {
				for (int i = 0; i < vvalencevalues.size(); i++)
				{
					vvalence += vvalencevalues[i];
					varousal += varousalvalues[i];
				}
				vvalence /= (FLOAT)vvalencevalues.size();
				varousal /= (FLOAT)varousalvalues.size();
			}

			/* 
			 Delete file, so voice writes to it again. A very poor program design decision was made due to lack of time. This is a hack, but it works.
			 The voice program will write to a file, and I'll attempt to read it. If there's something there, I'll take the average of all the values for
			 valence and arousal. After I do that, I delete the file. The reason why you can't overwrite is because there is conflict as the other program
			 is also writing to the file. Deleting the file clears the values (but make sure it's logged elsewhere) for the next classification step. Again,
			 the correct way to do this is through TCP/IP socket communication
			*/ 
			remove(path_to_voicefile.c_str());


			/* 
			       +--------------------------------------------+
				   |          Multimodal Calculation            |
				   +--------------------------------------------+
				   This should be inside another method. Short on time, so bad organization here.
				   TODO: modular code, same code as BL, make it more modular, no time....
		     */


			// Normalization: Scale: [-1, 1]
			// insert normalization here


			// Decision-level fusion
			FLOAT mmvalence = 0;
			FLOAT mmarousal = 0;

			/* Average: Naive Approach */
			/*
			FLOAT nu = 0.5f;
			FLOAT mu = 0.5f; 
			mmvalence = nu * blvalence + (1-nu) * vvalence;
			mmarousal = mu * blarousal + (1-mu) * varousal;			
			*/
			
			/* Multimodal Classification */
			/*

			// create test data
			std::string path_to_MvalenceTest = "testDataFiles\\Mvalence_test.arff";
			std::string path_to_MarousalTest = "testDataFiles\\Marousal_test.arff";

			createMTestData(path_to_MvalenceTest, "MValenceResponse", "valence {-2,-1,0,1,2}", blvalence, blarousal, vvalence, varousal);
			createMTestData(path_to_MarousalTest, "MArousalResponse", "arousal {0,1,2,3,4}", blvalence, blarousal, vvalence, varousal);

			std::string mvalence_classifier = "weka.classifiers.functions.RBFNetwork";
			std::string marousal_classifier = "weka.classifiers.trees.RandomForest";

			std::string path_to_MArousalTrainingModel = path_to_local_dir + "TrainingData\\MArousalTrain.model";
			std::string path_to_MValenceTrainingModel = path_to_local_dir + "TrainingData\\MValenceTrain.model";
			std::string path_to_MArousalTestData = path_to_local_dir + "testDataFiles\\Marousal_test.arff";
			std::string path_to_MValenceTestData = path_to_local_dir + "testDataFiles\\Mvalence_test.arff";
			std::string path_to_MarousalBat = path_to_local_dir + "batFiles\\Marousal.bat";
			std::string path_to_MvalenceBat = path_to_local_dir + "batFiles\\Mvalence.bat";
			std::string path_to_MoutArousal = path_to_local_dir + "wekaOutputFiles\\MoutArousal.txt";
			std::string path_to_MoutValence = path_to_local_dir + "wekaOutputFiles\\MoutValence.txt";

			createBatWekaFile(path_to_MvalenceBat, path_to_java, path_to_weka, mvalence_classifier, path_to_MValenceTrainingModel, path_to_MValenceTestData, path_to_MoutValence);
			createBatWekaFile(path_to_MarousalBat, path_to_java, path_to_weka, marousal_classifier, path_to_MArousalTrainingModel, path_to_MArousalTestData, path_to_MoutArousal);

			mmarousal = getWekaResult(path_to_MarousalBat, path_to_MoutArousal);
			mmvalence = getWekaResult(path_to_MvalenceBat, path_to_MoutValence);
			*/


			// Display!
			m_pMClassificationViewer->SetAffectReadings(mmvalence, mmarousal, blvalence, blarousal, vvalence, varousal);
			// log everything! Put this in another method
			std::ofstream myfile;
			myfile.open(".\\logs\\log.csv", std::ios::app);
			myfile << myBLFeatures->expand_body() << ","
			   	   << myBLFeatures->spd_body() << ","
				   << myBLFeatures->open_close_arms() << ","
				   << myBLFeatures->fwd_bwd_head() << ","
				   << myBLFeatures->vert_head() << ","
				   << myBLFeatures->bow_stretch_trunk() << ","
				   << 0 << ","
				   << myBLFeatures->vert_motion_body() << ","
				   << myBLFeatures->fwd_bwd_motion_body() << ","
				   << blvalence << ","
				   << blarousal << ","
				   << vvalence << ","
				   << varousal << ","
				   << mmvalence << ","
				   << mmarousal << "\n";
			myfile.close();


			// Clean up
			SafeDelete(myBLFeatures);
			m_skeletonData.clear();
		}
	}
}

/// <summary>
/// Draw a circle to indicate a skeleton of which only position info is available
/// </summary>
/// <param name="skeletonData">Skeleton coordinates</param>
/// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
void NuiStreamViewer::DrawPosition(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect)
{
    D2D1_POINT_2F center = ToImageRect(skeletonData.Position, imageRect);
    m_pImageRenderer->DrawCircle(center, 5.0f, ImageRendererBrushGreen, 2.5f);
}

/// <summary>
/// Draw a bone between 2 tracked joint.
/// <summary>
/// <param name="skeletonData">Skeleton coordinates</param>
/// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
/// <param name="joint0">Index for the first joint</param>
/// <param name="joint1">Index for the second joint</param>
void NuiStreamViewer::DrawBone(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect, NUI_SKELETON_POSITION_INDEX joint0, NUI_SKELETON_POSITION_INDEX joint1)
{
    NUI_SKELETON_POSITION_TRACKING_STATE state0 = skeletonData.eSkeletonPositionTrackingState[joint0];
    NUI_SKELETON_POSITION_TRACKING_STATE state1 = skeletonData.eSkeletonPositionTrackingState[joint1];

    // Any is not tracked
    if (NUI_SKELETON_POSITION_NOT_TRACKED == state0 || NUI_SKELETON_POSITION_NOT_TRACKED == state1)
    {
        return;
    }

    // Both are inferred
    if (NUI_SKELETON_POSITION_INFERRED == state0 && NUI_SKELETON_POSITION_INFERRED == state1)
    {
        return;
    }

    D2D1_POINT_2F point0 = ToImageRect(skeletonData.SkeletonPositions[joint0], imageRect);
    D2D1_POINT_2F point1 = ToImageRect(skeletonData.SkeletonPositions[joint1], imageRect);

    // We assume all drawn bones are inferred unless BOTH joints are tracked
    if (NUI_SKELETON_POSITION_TRACKED == state0 && NUI_SKELETON_POSITION_TRACKED == state1)
    {
        m_pImageRenderer->DrawLine(point0, point1, ImageRendererBrushBoneTracked, 4.0f);
    }
    else
    {
        m_pImageRenderer->DrawLine(point0, point1, ImageRendererBrushBoneInferred, 4.0f);
    }
}

/// <summary>
/// Draw a joint of the skeleton
/// </summary>
/// <param name="skeletonData">Skeleton coordinates</param>
/// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
/// <param name="joint">Index for the joint to be drawn</param>
void NuiStreamViewer::DrawJoint(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect, NUI_SKELETON_POSITION_INDEX joint)
{
    NUI_SKELETON_POSITION_TRACKING_STATE state = skeletonData.eSkeletonPositionTrackingState[joint];

    // Not tracked
    if (NUI_SKELETON_POSITION_NOT_TRACKED == state)
    {
        return;
    }

    D2D1_POINT_2F point = ToImageRect(skeletonData.SkeletonPositions[joint], imageRect);

    if (NUI_SKELETON_POSITION_TRACKED == state)
    {
        m_pImageRenderer->DrawCircle(point, 3.0f, ImageRendererBrushJointTracked, 3.0f);
    }
    else
    {
        if (!IsOutOfImageRect(point, imageRect))
        {
            m_pImageRenderer->DrawCircle(point, 3.0f, ImageRendererBrushJointInferred, 3.0f);
        }

        UpdateDrawEdgeFlags(point, imageRect);
    }
}

/// <summary>
/// Draw frame FPS counter
/// </summary>
/// <param name="clientRect">Client area of viewer's window</param>
void NuiStreamViewer::DrawFPS(const RECT& clientRect)
{
    // Get rectangle position and size
    D2D1_RECT_F rect = D2D1::RectF((FLOAT)clientRect.right - 50.0f, 0.0f, (FLOAT)clientRect.right, (FLOAT)clientRect.top + 50.0f);

    // Fill rectangle
    m_pImageRenderer->FillRectangle(rect, ImageRendererBrushGray);

    // Draw a while circle
    D2D1_POINT_2F center = D2D1::Point2F((rect.right + rect.left) / 2.0f, (rect.bottom + rect.top) / 2.0f);
    m_pImageRenderer->DrawCircle(center, 23.0f, ImageRendererBrushWhite, 4.0f);
    
    // Draw FPS text
    WCHAR text[MaxStringChars];
    swprintf_s(text, sizeof(text) / sizeof(WCHAR) - 1, L"%d", m_fps);
    UINT cch = (UINT)wcsnlen_s(text, MaxStringChars);
    m_pImageRenderer->DrawText(text, cch, rect, ImageRendererBrushWhite, ImageRendererTextFormatFps);
}

/// <summary>
/// Draw image resolution text
/// </summary>
/// <param name="clientRect">Client area of viewer's window</param>
void NuiStreamViewer::DrawResolution(const RECT& clientRect)
{
    if (m_pImage)
    {
        WCHAR buffer[MaxStringChars];
        D2D1_RECT_F rect = D2D1::RectF((FLOAT)clientRect.left, (FLOAT)clientRect.top, (FLOAT)clientRect.right, 10.0f);
        swprintf_s(buffer, sizeof(buffer) / sizeof(WCHAR), L"Resolution: %dx%d", m_pImage->GetWidth(), m_pImage->GetHeight());
        m_pImageRenderer->DrawText(buffer, (UINT)wcsnlen_s(buffer, MaxStringChars), rect, ImageRendererBrushGreen, ImageRendererTextFormatResolution);
    }
}

/// <summary>
/// Draw red edge on image when skeleton is close to or out of the image edge
/// </summary>
/// <param name="imageRect">The rectangle of the image</param>
void NuiStreamViewer::DrawRedEdges(const D2D1_RECT_F& imageRect)
{
    D2D1_RECT_F   rect;
    D2D1_POINT_2F start;
    D2D1_POINT_2F end;

    FLOAT edgeWidth = (imageRect.right - imageRect.left) / 40.0f;

    if (m_drawEdgeFlags & DRAW_EDGE_FLAG_LEFT)
    {
        rect.left   = imageRect.left;
        rect.top    = imageRect.top;
        rect.right  = imageRect.left + edgeWidth;
        rect.bottom = imageRect.bottom;

        start.x = rect.left;
        start.y = (rect.top + rect.bottom) / 2.0f;

        end.x   = rect.left + edgeWidth;
        end.y   = start.y;

        m_pImageRenderer->DrawEdge(rect, start, end);
    }

    if (m_drawEdgeFlags & DRAW_EDGE_FLAG_RIGHT)
    {
        rect.left   = imageRect.right - edgeWidth;
        rect.right  = imageRect.right;
        rect.top    = imageRect.top;
        rect.bottom = imageRect.bottom;

        start.x = rect.right;
        start.y = (rect.top + rect.bottom) / 2.0f;

        end.x   = rect.left;
        end.y   = start.y;

        m_pImageRenderer->DrawEdge(rect, start, end);
    }

    if (m_drawEdgeFlags & DRAW_EDGE_FLAG_TOP)
    {
        rect.left   = imageRect.left;
        rect.top    = imageRect.top;
        rect.right  = imageRect.right;
        rect.bottom = imageRect.top + edgeWidth;

        start.x = (rect.left + rect.right) / 2.0f;
        start.y = rect.top;

        end.x   = start.x;
        end.y   = rect.top + edgeWidth;

        m_pImageRenderer->DrawEdge(rect, start, end);
    }

    if (m_drawEdgeFlags & DRAW_EDGE_FLAG_BOTTOM)
    {
        rect.left   = imageRect.left;
        rect.top    = imageRect.bottom - edgeWidth;
        rect.right  = imageRect.right;
        rect.bottom = imageRect.bottom;

        start.x = (rect.left + rect.right) / 2.0f;
        start.y = rect.bottom;

        end.x   = start.x;
        end.y   = rect.top;

        m_pImageRenderer->DrawEdge(rect, start, end);
    }

    m_drawEdgeFlags = 0;
}

/// <summary>
/// Set the buffer containing the image pixels.
/// </summary>
/// <param name="pImage">The pointer to image buffer object</param>
void NuiStreamViewer::SetImage(const NuiImageBuffer* pImage)
{
    m_pImage = pImage;
    if (m_pImage &&  m_pImage->GetBufferSize() && m_hWnd)
    {
        InvalidateRect(m_hWnd, nullptr, FALSE);

        UpdateFrameRate();
    }
}

/// <summary>
/// Attach skeleton data.
/// </summary>
/// <param name="pFrame">The pointer to skeleton frame</param>
void NuiStreamViewer::SetSkeleton(const NUI_SKELETON_FRAME* pFrame)
{
    if (!m_hWnd)
    {
        return;
    }

    m_pSkeletonFrame = pFrame;

    InvalidateRect(m_hWnd, nullptr, FALSE);
}

/// <summary>
/// Update frame rate
/// </summary>
void NuiStreamViewer::UpdateFrameRate()
{
    m_frameCount++;

    DWORD tickCount = GetTickCount();
    DWORD span      = tickCount - m_lastTick;
    if (span >= 1000)
    {
        m_fps            = (UINT)((double)(m_frameCount - m_lastFrameCount) * 1000.0 / (double)span + 0.5);
        m_lastTick       = tickCount;
        m_lastFrameCount = m_frameCount;
    }
}

/// <summary>
/// Check which red edge should be drawn
/// </summary>
/// <param name="point">Coordinates of a tracked joint</param>
/// <param name="imageRect">Rectangle of the image</param>
void NuiStreamViewer::UpdateDrawEdgeFlags(const D2D1_POINT_2F& point, const D2D1_RECT_F& imageRect)
{
    FLOAT detectWidth = (imageRect.right - imageRect.left) / 40.0f;

    if (point.x < imageRect.left + detectWidth)
    {
        m_drawEdgeFlags |= DRAW_EDGE_FLAG_LEFT;
    }

    if (point.x > imageRect.right - detectWidth)
    {
        m_drawEdgeFlags |= DRAW_EDGE_FLAG_RIGHT;
    }

    if (point.y < imageRect.top + detectWidth)
    {
        m_drawEdgeFlags |= DRAW_EDGE_FLAG_TOP;
    }

    if (point.y > imageRect.bottom - detectWidth)
    {
        m_drawEdgeFlags |= DRAW_EDGE_FLAG_BOTTOM;
    }
}

/// <summary>
/// Decide if skeleton is out of image
/// </summary>
/// <param name="point">Coordinates of a tracked joint</param>
/// <param name="imageRect">Rectangle of the image</param>
bool NuiStreamViewer::IsOutOfImageRect(const D2D1_POINT_2F& point, const D2D1_RECT_F& imageRect)
{
    if (point.x < imageRect.left || point.x > imageRect.right)
    {
        return true;
    }
    
    if (point.y < imageRect.top || point.y > imageRect.bottom)
    {
        return true;
    }

    return false;
}

/// <summary>
/// Calculate the coordinates of the image to be displayed in client area.
/// </summary>
/// <param name="client">Client area of viewer's window</param>
D2D1_RECT_F NuiStreamViewer::GetImageRect(const RECT &client)
{
    D2D1_RECT_F imageRect = D2D1::RectF();
    if (m_pImage && m_pImage->GetBuffer())
    {
        float ratio  = static_cast<float>(m_pImage->GetWidth()) / static_cast<float>(m_pImage->GetHeight());
        float width  = static_cast<float>(client.right);
        float height = width / ratio;

        if (height > client.bottom)
        {
            height = static_cast<float>(client.bottom);
            width  = height * ratio;
        }

        imageRect.left   = (client.right  - width  + 1) / 2.0f;
        imageRect.top    = (client.bottom - height + 1) / 2.0f;
        imageRect.right  = imageRect.left + width;
        imageRect.bottom = imageRect.top  + height;
    }

    return imageRect;
}

/// <summary>
/// Map skeleton point to window coordinate in image rect.
/// </summary>
/// <param name="skeletonPoint">Skeleton point to map.</param>
/// <param name="imageRect">The rectangle of image</param>
/// <returns>Mapped coordinate in client area</returns>
D2D1_POINT_2F NuiStreamViewer::ToImageRect(const Vector4& skeletonPoint, const D2D1_RECT_F& imageRect)
{
    const NUI_IMAGE_RESOLUTION imageResolution = NUI_IMAGE_RESOLUTION_640x480;
    LONG x = 0, y = 0;
    USHORT depthValue = 0;
    NuiTransformSkeletonToDepthImage(skeletonPoint, &x, &y, &depthValue, imageResolution); // Returns coordinates in depth space

    if (NUI_IMAGE_TYPE_COLOR == m_imageType || NUI_IMAGE_TYPE_COLOR_INFRARED == m_imageType
        || NUI_IMAGE_TYPE_COLOR_RAW_BAYER == m_imageType || NUI_IMAGE_TYPE_COLOR_RAW_YUV == m_imageType
        || NUI_IMAGE_TYPE_COLOR_YUV == m_imageType)
    {
        LONG backupX = x, backupY = y;
        if (FAILED(NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(imageResolution, imageResolution, nullptr, x, y, depthValue, &x, &y)))
        {
            x = backupX;
            y = backupY;
        }
    }

    DWORD imageWidth, imageHeight;
    NuiImageResolutionToSize(imageResolution, imageWidth, imageHeight);

    FLOAT resultX, resultY;
    resultX = x * (imageRect.right  - imageRect.left + 1.0f) / imageWidth + imageRect.left;
    resultY = y * (imageRect.bottom - imageRect.top  + 1.0f) / imageHeight + imageRect.top;

    return D2D1::Point2F(resultX, resultY);
}

/// <summary>
/// Pause the skeleton
/// </summary>
/// <param name="pause">Pause or resume the skeleton</param>
void NuiStreamViewer::PauseSkeleton(bool pause)
{
    m_pauseSkeleton = pause;
}
