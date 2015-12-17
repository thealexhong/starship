//------------------------------------------------------------------------------
// <copyright file="NuiStreamViewer.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <NuiApi.h>
#include "Utility.h"
#include "NuiViewer.h"
#include "NuiImageBuffer.h"
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
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pParent">The pointer to parent window</param>
    NuiStreamViewer(const NuiViewer* pParent);

    /// <summary>
    /// Destructor
    /// </summary>
    ~NuiStreamViewer();

public:
    /// <summary>
    /// Set the buffer containing the image pixels.
    /// </summary>
    /// <param name="pImage">The pointer to image buffer object</param>
    void SetImage(const NuiImageBuffer* pImage);

    /// <summary>
    /// Attach skeleton data.
    /// </summary>
    /// <param name="pFrame">The pointer to skeleton frame</param>
    void SetSkeleton(const NUI_SKELETON_FRAME* pFrame);

    /// <summary>
    /// Pause the skeleton
    /// </summary>
    /// <param name="pause">Pause or resume the skeleton</param>
    void PauseSkeleton(bool pause);

    /// <summary>
    /// Set image type.
    /// </summary>
    /// <param name="type">Image type to be set</param>
    void SetImageType(NUI_IMAGE_TYPE type)
    {
        m_imageType = type;
    }

private:
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
    virtual LRESULT DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

    /// <summary>
    /// Returns the ID of the dialog
    /// </summary>
    /// <returns>ID of dialog</returns>
    virtual UINT GetDlgId();

    /// <summary>
    /// Message handler of WM_PAINT.
    /// </summary>
    /// <param name="wParam">Extra message parameter</param>
    /// <param name="lParam">Extra message parameter</param>
    void OnPaint(WPARAM wParma, LPARAM lParam);

    /// <summary>
    /// Draw the image on screen by D2D
    /// </summary>
    /// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
    void DrawImage(const D2D1_RECT_F& imageRect);

    /// <summary>
    /// Draw skeletons
    /// </summary>
    /// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
    void DrawSkeletons(const D2D1_RECT_F& imageRect);

    /// <summary>
    /// Draw a skeleton and overlay it on color or depth image
    /// </summary>
    /// <param name="skeletonData">Skeleton coordinates</param>
    /// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
    void DrawSkeleton(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect);

    /// <summary>
    /// Draw a circle to indicate a skeleton of which only position info is available
    /// </summary>
    /// <param name="skeletonData">Skeleton coordinates</param>
    /// <param name="imageRect">The rect which the color or depth stream image is streched to fit</param>
    void DrawPosition(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect);

    /// <summary>
    /// Draw a bone between 2 tracked joint.
    /// <summary>
    /// <param name="skeletonData">Skeleton coordinates</param>
    /// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
    /// <param name="joint0">Index for the first joint</param>
    /// <param name="joint1">Index for the second joint</param>
    void DrawBone(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect, NUI_SKELETON_POSITION_INDEX joint0, NUI_SKELETON_POSITION_INDEX joint1);

    /// <summary>
    /// Draw a joint of the skeleton
    /// </summary>
    /// <param name="skeletonData">Skeleton coordinates</param>
    /// <param name="imageRect">The rect which the color or depth image is streched to fit</param>
    /// <param name="joint">Index for the joint to be drawn</param>
    void DrawJoint(const NUI_SKELETON_DATA& skeletonData, const D2D1_RECT_F& imageRect, NUI_SKELETON_POSITION_INDEX joint);

    /// <summary>
    /// Draw frame FPS counter
    /// </summary>
    /// <param name="clientRect">Client area of viewer's window</param>
    void DrawFPS(const RECT& clientRect);

    /// <summary>
    /// Draw image resolution text
    /// </summary>
    /// <param name="clientRect">Client area of viewer's window</param>
    void DrawResolution(const RECT& clientRect);

    /// <summary>
    /// Draw red edge on image when skeleton is close to or out of the image edge
    /// </summary>
    /// <param name="imageRect">The rectangle of the image</param>
    void DrawRedEdges(const D2D1_RECT_F& imageRect);

    /// <summary>
    /// Update frame rate
    /// </summary>
    void UpdateFrameRate();

    /// <summary>
    /// Check which red edge should be drawn
    /// </summary>
    /// <param name="point">Coordinates of a tracked joint</param>
    /// <param name="imageRect">Rectangle of the image</param>
    void UpdateDrawEdgeFlags(const D2D1_POINT_2F& point, const D2D1_RECT_F& imageRect);

    /// <summary>
    /// Decide if skeleton is out of image
    /// </summary>
    /// <param name="point">Coordinates of a tracked joint</param>
    /// <param name="imageRect">Rectangle of the image</param>
    bool IsOutOfImageRect(const D2D1_POINT_2F& point, const D2D1_RECT_F& imageRect);

    /// <summary>
    /// Calculate the coordinates of the image to be displayed in client area. The image should be centered in client area and streched to client area with ratio fixed
    /// </summary>
    /// <param name="client">Client area of viewer's window</param>
    D2D1_RECT_F GetImageRect(const RECT& client);

    /// <summary>
    /// Map skeleton point to window coordinate in image rect.
    /// </summary>
    /// <param name="skeletonPoint">Skeleton point to map.</param>
    /// <param name="imageRect">The rectangle of image</param>
    /// <returns>Mapped coordinate in client area</returns>
    D2D1_POINT_2F ToImageRect(const Vector4& skeletonPoint, const D2D1_RECT_F& imageRect);

private:
    NUI_IMAGE_TYPE              m_imageType;

    const NuiImageBuffer*       m_pImage;
    const NUI_SKELETON_FRAME*   m_pSkeletonFrame;

    bool                m_pauseSkeleton;
    UINT                m_fps;
    UINT                m_frameCount;
    UINT                m_lastFrameCount;
    DWORD               m_lastTick;
    DWORD               m_drawEdgeFlags;

    ImageRenderer*      m_pImageRenderer;
};