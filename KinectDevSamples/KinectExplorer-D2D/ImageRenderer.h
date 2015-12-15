//------------------------------------------------------------------------------
// <copyright file="ImageRenderer.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <d2d1.h>
#include <dwrite.h>

enum ImageRendererBrush
{
    ImageRendererBrushJointTracked = 0,
    ImageRendererBrushJointInferred,
    ImageRendererBrushBoneTracked,
    ImageRendererBrushBoneInferred,
    ImageRendererBrushWhite,
    ImageRendererBrushGray,
    ImageRendererBrushGreen,
    ImageRendererBrushCount
};

enum ImageRendererTextFormat
{
    ImageRendererTextFormatFps = 0,
    ImageRendererTextFormatResolution,
    ImageRendererTextFormatCount
};


class ImageRenderer
{
public:
    /// <summary>
    /// Constructor
    /// </summary>
    ImageRenderer();

    /// <summary>
    /// Destructor
    /// </summary>
    ~ImageRenderer();

public:
    /// <summary>
    /// Resize render target to client area
    /// </summary>
    /// <param name="width">Width of client area</param>
    /// <param name="height">Height of client area</param>
    /// <returns>Indicates the success or failure</returns>
    HRESULT ResizeRenderTarget(UINT width, UINT height);

    /// <summary>
    /// Begin drawing process. Called before drawing to the window
    /// </summary>
    /// <param name="hWnd">Handle to window to which renderer draws</param>
    /// <returns>Indicates success or failure</returns>
    HRESULT BeginDraw(HWND hWnd);

    /// <summary>
    /// Set clip area to prevent joints and bones are drawn outside the image
    /// </summary>
    /// <param name="rect">The clip area</param>
    void SetClipRect(const D2D1_RECT_F& rect);

    /// <summary>
    /// Clear previously set clip area.
    /// </summary>
    void ResetClipRect();

    /// <summary>
    /// Draws a 32 bits per pixel image of previously specified width, height and stride, to the associated hwnd
    /// </summary>
    /// <param name="pImage">The pointer to image buffer</param>
    /// <param name="size">The image size</param>
    /// <param name="rect">The rectangle in the window to render the image</param>
    void DrawImage(const BYTE* pImage, const D2D1_SIZE_U& size, const D2D1_RECT_F& rect);

    /// <summary>
    /// Draw FPS text
    /// </summary>
    /// <param name="pText">Text to draw</param>
    /// <param name="cch">Count of charaters in text</param>
    /// <param name="rect">The area to draw text</param>
    /// <param name="brush">Index of brush</param>
    /// <param name="format">Text format</param>
    void DrawText(const WCHAR* pText, UINT cch, const D2D1_RECT_F &rect, ImageRendererBrush brush, ImageRendererTextFormat format);

    /// <summary>
    /// Draw the line representing the bone
    /// </summary>
    /// <param name="point0">The start point of the line</param>
    /// <param name="point1">The end point of the line</param>
    /// <param name="brush">The index of brush</param>
    /// <param name="strokeWidth">Width of the line</param>
    /// <param name="strokeStyle">Style of the line</param>
    void DrawLine(const D2D1_POINT_2F& point0, const D2D1_POINT_2F& point1, ImageRendererBrush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle* strokeStyle = nullptr);

    /// <summary>
    /// Draw the circle representing the joint
    /// </summary>
    /// <param name="center">The center of the circle</param>
    /// <param name="radius">The radius of the circle</param>
    /// <param name="brush">Index of brush</param>
    /// <param name="strokeWidth">Width of the line</param>
    /// <param name="strokeStyle">Style of the line</param>
    void DrawCircle(const D2D1_POINT_2F& center, float radius, ImageRendererBrush brush, float strokeWidth = 1.0f, ID2D1StrokeStyle* strokeStyle = nullptr);

    /// <summary>
    /// Draw red edge on the image when skeleton is outside the image area
    /// </summary>
    /// <param name="rect">The rectangle on image edge to render in red</param>
    /// <param name="start">The start point of gradient brush</param>
    /// <param name="end">The end point of gradient brush</param>
    void DrawEdge(const D2D1_RECT_F& rect, const D2D1_POINT_2F& start, const D2D1_POINT_2F& end);

    /// <summary>
    /// Draw a rectangle and fill it
    /// </summary>
    /// <param name="rect">The rectangle to fill</param>
    /// <param name="brush">The index of brush</param>
    void FillRectangle(const D2D1_RECT_F& rect, ImageRendererBrush brush);

    /// <summary>
    /// End drawing process. Called after finish drawing
    /// </summary>
    void EndDraw();

private:
    /// <summary>
    /// Ensure necesary dependent Direct2D resources are created
    /// </summary>
    /// <param name="hWnd">Handle to window to which renderer draws</param>
    /// <returns>Indicates success or failure</returns>
    HRESULT EnsureDependentResourcesCreated(HWND hWnd);

    /// <summary>
    /// Release the dependent Direct2D resources
    /// </summary>
    void DiscardDependentResources();

    /// <summary>
    /// Ensure render target is created
    /// </summary>
    /// <returns>Indicates success or failure</returns>
    HRESULT EnsureRenderTarget();

    /// <summary>
    /// Ensure bitmap is created
    /// </summary>
    /// <param name=imageSize>Size of bit map</param>
    /// <returns>Indicates success or failure</returns>
    HRESULT EnsureBitmap(const D2D1_SIZE_U& imageSize);

    /// <summary>
    /// Create D2D solid color brushes
    /// </summary>
    void CreateSolidBrushes();

    /// <summary>
    /// Create a specific D2D solid color brush from parameters
    /// </summary>
    /// <param name="color">The color used by brush</param>
    /// <param name="brush">The index of brush</param>
    void CreateSolidBrush(D2D1::ColorF::Enum color, ImageRendererBrush brush);

    /// <summary>
    /// Create linear gradient brush to draw image red edge
    /// </summary>
    void CreateGradientBrush();

    /// <summary>
    /// Create text formats to draw FPS and image resolution lable
    /// </summary>
    void CreateTextFormats();

    /// <summary>
    /// Create a specific DWrite text format from parameters
    /// </summary>
    /// <param name="fontFamilyName">Family name of created font</param>
    /// <param name="fontSize">Size of created font</param>
    /// <param name="textAlignment">Alignment of text</param>
    /// <param name="paragraphAlignment">Alignment of paragraph</param>
    /// <param name="ppTextFormat">Created text format</param>
    void CreateTextFormat(const WCHAR* fontFamilyName, FLOAT fontSize, DWRITE_TEXT_ALIGNMENT textAlignment, DWRITE_PARAGRAPH_ALIGNMENT paragraphAlignment, IDWriteTextFormat** ppTextFormat);

    /// <summary>
    /// Retrieve client area rect and transform into D2D size structure
    /// </summary>
    /// <param name="hWnd">The handle to window</param>
    /// <returns>Client area size in D2D1_SIZE_U structure</returns>
    D2D1_SIZE_U GetClientSize(HWND hWnd);

private:
    HWND                        m_hWnd;

    // Dependent Direct2D resources
    ID2D1HwndRenderTarget*      m_pRenderTarget;
    ID2D1Bitmap*                m_pBitmap;
    IDWriteTextFormat*          m_formats[ImageRendererTextFormatCount];
    ID2D1SolidColorBrush*       m_brushes[ImageRendererBrushCount];
    ID2D1LinearGradientBrush*   m_pEdgeBrush;
};