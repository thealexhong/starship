/**
* NuiStreamViewer.h
* Modfied from Kinect SDK v.1.8
* alex.hong@mail.utoronto.ca
*/

#pragma once

#include <NuiApi.h>
#include "Utility.h"
#include "NuiViewer.h"
#include "NuiImageBuffer.h"
#include "NuiSkeletonPointsViewer.h"
#include "NuiBLFeatureViewer.h"
#include "NuiBLClassificationViewer.h"
#include "ImageRenderer.h"

enum DRAW_EDGE_FLAG
{
    DRAW_EDGE_FLAG_LEFT   = 0x00000001,
    DRAW_EDGE_FLAG_TOP    = 0x00000010,
    DRAW_EDGE_FLAG_RIGHT  = 0x00000100,
    DRAW_EDGE_FLAG_BOTTOM = 0x00001000,
};

class NuiStreamViewer : public NuiViewer
{
public:
	/**
	 * Constructor
	 * @param  pParent  The pointer to parent window
	 */
    NuiStreamViewer(const NuiViewer* pParent, bool displayCoordinates);

    /**
	 * Destructor
	 */
    ~NuiStreamViewer();

public:
    /// Set the buffer containing the image pixels.
    /// <param name="pImage">The pointer to image buffer object</param>
    void SetImage(const NuiImageBuffer* pImage);

    
    /// Attach skeleton data.
    
    /// <param name="pFrame">The pointer to skeleton frame</param>
    void SetSkeleton(const NUI_SKELETON_FRAME* pFrame);

    
    /// Pause the skeleton
    
    /// <param name="pause">Pause or resume the skeleton</param>
    void PauseSkeleton(bool pause);

    
    /// Set image type.
    
    /// <param name="type">Image type to be set</param>
    void SetImageType(NUI_IMAGE_TYPE type)
    {
        m_imageType = type;
    }

	void SetStreamViewer(NuiSkeletonPointsViewer* pViewer, NuiBLFeatureViewer* pBLFeatureViewer, NuiBLClassificationViewer* pBLClassificationViewer);
	void SetSeated(bool seated);

private:
    
    /// Dispatch window message to message handlers.
    
    /// <param name="hWnd">Handle to window</param>
    /// <param name="uMsg">Message type</param>
    /// <param name="wParam">Extra message parameter</param>
    /// <param name="lParam">Extra message parameter</param>
    /// <returns>
    /// If message is handled, non-zero is returned. Otherwise FALSE is returned and message is passed to default dialog procedure
    /// </returns>
    virtual LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

    
    /// Returns the ID of the dialog
    
    /// <returns>ID of dialog</returns>
    virtual UINT GetDlgId();

    
    /// Message handler of WM_PAINT.
    
    /// <param name="wParam">Extra message parameter</param>
    /// <param name="lParam">Extra message parameter</param>
    void OnPaint(WPARAM wParma, LPARAM lParam);

    
    /// Draw the image on screen by D2D
    
    /// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
    void DrawImage(const D2D1_RECT_F& imageRect);

    
    /// Draw skeletons
    
    /// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
    void DrawSkeletons(const D2D1_RECT_F& imageRect);

    
    /// Draw a skeleton and overlay it on color or depth image
    
    /// <param name="skeletonData">Skeleton coordinates</param>
    /// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
    void DrawSkeleton(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect);

    
    /// Draw a circle to indicate a skeleton of which only position info is available
    
    /// <param name="skeletonData">Skeleton coordinates</param>
    /// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
    void DrawPosition(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect);

    
    /// Draw a bone between 2 tracked joint.
    
    /// <param name="skeletonData">Skeleton coordinates</param>
    /// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
    /// <param name="joint0">Index for the first joint</param>
    /// <param name="joint1">Index for the second joint</param>
    void DrawBone(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect, NUI_SKELETON_POSITION_INDEX joint0, NUI_SKELETON_POSITION_INDEX joint1);

    
    /// Draw a joint of the skeleton
    
    /// <param name="skeletonData">Skeleton coordinates</param>
    /// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
    /// <param name="joint">Index for the joint to be drawn</param>
    void DrawJoint(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect, NUI_SKELETON_POSITION_INDEX joint);

    
    /// Draw frame FPS counter
    
    /// <param name="clientRect">Client area of viewer's window</param>
    void DrawFPS(const RECT& clientRect);

    
    /// Draw image resolution text
    
    /// <param name="clientRect">Client area of viewer's window</param>
    void DrawResolution(const RECT& clientRect);

    
    /// Draw red edge on image when skeleton is close to or out of the image edge
    
    /// <param name="imageRect">The rectangle of the image</param>
    void DrawRedEdges(const D2D1_RECT_F& imageRect);

    
    /// Update frame rate
    
    void UpdateFrameRate();

    
    /// Check which red edge should be drawn
    
    /// <param name="point">Coordinates of a tracked joint</param>
    /// <param name="imageRect">Rectangle of the image</param>
    void UpdateDrawEdgeFlags(const D2D1_POINT_2F& point, const D2D1_RECT_F& imageRect);

    
    /// Decide if skeleton is out of image
    
    /// <param name="point">Coordinates of a tracked joint</param>
    /// <param name="imageRect">Rectangle of the image</param>
    bool IsOutOfImageRect(const D2D1_POINT_2F& point, const D2D1_RECT_F& imageRect);

    
    /// Calculate the coordinates of the image to be displayed in client area. The image should be centered in client area and streched to client area with ratio fixed
    
    /// <param name="client">Client area of viewer's window</param>
    D2D1_RECT_F GetImageRect(const RECT& client);

    
    /// Map skeleton point to window coordinate in image rect.
    
    /// <param name="skeletonPoint">Skeleton point to map.</param>
    /// <param name="imageRect">The rectangle of image</param>
    /// <returns>Mapped coordinate in client area</returns>
    D2D1_POINT_2F ToImageRect(const Vector4& skeletonPoint, const D2D1_RECT_F& imageRect);

private:
    NUI_IMAGE_TYPE              m_imageType;

    const NuiImageBuffer*       m_pImage;
    const NUI_SKELETON_FRAME*   m_pSkeletonFrame;

    bool                        m_pauseSkeleton;
	bool                        m_displayCoordinates;
	bool                        m_seated;
    UINT                        m_fps;
    UINT                        m_frameCount;
    UINT                        m_lastFrameCount;
	UINT                        m_frameTracker;
    DWORD                       m_lastTick;
    DWORD                       m_drawEdgeFlags;

    ImageRenderer*              m_pImageRenderer;
	NuiSkeletonPointsViewer*    m_pSkeletonPointsViewer;
	NuiBLFeatureViewer*         m_pBLFeatureViewer;
	NuiBLClassificationViewer*  m_pBLClassificationViewer;
};