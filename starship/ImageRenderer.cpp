//------------------------------------------------------------------------------
// <copyright file="ImageRenderer.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include "ImageRenderer.h"
#include "Utility.h"

extern ID2D1Factory* g_pD2DFactory;
extern IDWriteFactory* g_pDWriteFactory;

/// <summary>
/// Constructor
/// </summary>
ImageRenderer::ImageRenderer()
    : m_hWnd(nullptr)
    , m_pBitmap(nullptr)
    , m_pRenderTarget(nullptr)
    , m_pEdgeBrush(nullptr)
{
    ZeroMemory(m_brushes, sizeof(m_brushes));
    ZeroMemory(m_formats, sizeof(m_formats));
}

/// <summary>
/// Destructor
/// </summary>
ImageRenderer::~ImageRenderer()
{
    DiscardDependentResources();
}

/// <summary>
/// Begin drawing process. Called before drawing to the window
/// </summary>
/// <param name="hWnd">Handle of window to which renderer draws</param>
/// <returns>Indicates success or failure</returns>
HRESULT ImageRenderer::BeginDraw(HWND hWnd)
{
    HRESULT hr = EnsureDependentResourcesCreated(hWnd);

    if (SUCCEEDED(hr))
    {
        m_pRenderTarget->BeginDraw();
        m_pRenderTarget->Clear();
    }
    else
    {
        DiscardDependentResources();
    }

    return hr;
}

/// <summary>
/// End drawing process. Called after finish drawing
/// </summary>
void ImageRenderer::EndDraw()
{
    HRESULT hr = m_pRenderTarget->EndDraw();

    if (D2DERR_RECREATE_TARGET == hr)
    {
        DiscardDependentResources();
    }
}

/// <summary>
/// Ensure necesary dependent Direct2D resources are created
/// </summary>
/// <param name="hWnd">Handle of window to which renderer draws</param>
/// <returns>Indicates success or failure</returns>
HRESULT ImageRenderer::EnsureDependentResourcesCreated(HWND hWnd)
{
    if (!hWnd)
    {
        return E_INVALIDARG;
    }

    if (!g_pDWriteFactory)
    {
        return E_FAIL;
    }

    if (m_hWnd != hWnd)
    {
        // Renderer window changed. Re-create the resources
        DiscardDependentResources();
        m_hWnd = hWnd;
    }

    if (m_pRenderTarget)
    {
        return S_OK;
    }

    HRESULT hr = EnsureRenderTarget();

    if (SUCCEEDED(hr))
    {
        // Create solid brushes to draw skeleton, fps & resolution text
        CreateSolidBrushes();

        // Create gradient brush to draw red edge on image
        CreateGradientBrush();

        // Create text format to draw text
        CreateTextFormats();
    }

    return hr;
}

/// <summary>
/// Ensure render target is created
/// </summary>
/// <returns>Indicates success or failure</returns>
HRESULT ImageRenderer::EnsureRenderTarget()
{
    // Create render target properties
    D2D1_RENDER_TARGET_PROPERTIES rtProps = D2D1::RenderTargetProperties();
    rtProps.type        = D2D1_RENDER_TARGET_TYPE_DEFAULT;
    rtProps.pixelFormat = D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_IGNORE);
    rtProps.usage       = D2D1_RENDER_TARGET_USAGE_GDI_COMPATIBLE;
    rtProps.minLevel    = D2D1_FEATURE_LEVEL_DEFAULT;

    // Create a hWnd render target, in order to render to the window set in initialize.
    D2D1_SIZE_U client = GetClientSize(m_hWnd);
    HRESULT hr = g_pD2DFactory->CreateHwndRenderTarget(rtProps,
                                                       D2D1::HwndRenderTargetProperties(m_hWnd, client),
                                                       &m_pRenderTarget);

    if (SUCCEEDED(hr))
    {
        // Set render target to client area
        ResizeRenderTarget(client.width, client.height);
    }

    return hr;
}

/// <summary>
/// Ensure bitmap is created
/// </summary>
/// <param name=imageSize>Size of bit map</param>
/// <returns>Indicates success or failure</returns>
HRESULT ImageRenderer::EnsureBitmap(const D2D1_SIZE_U& imageSize)
{
    if (m_pBitmap)
    {
        D2D1_SIZE_U size = m_pBitmap->GetPixelSize();
        if (size.width == imageSize.width && size.height == imageSize.height)
        {
            return S_OK;
        }
    }

    SafeRelease(m_pBitmap);

    if (m_pRenderTarget)
    {
        // Create a bitmap that we can copy image data into it and then render to the target.
        return m_pRenderTarget->CreateBitmap(imageSize,
                                             D2D1::BitmapProperties(D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_IGNORE)),
                                             &m_pBitmap);
    }

    return E_FAIL;
}

/// <summary>
/// Create D2D solid color brushes
/// </summary>
void ImageRenderer::CreateSolidBrushes()
{
    // Create solid brushes to draw skeleton, fps & resolution text
    CreateSolidBrush(D2D1::ColorF::LightGreen,  ImageRendererBrushJointTracked);
    CreateSolidBrush(D2D1::ColorF::Yellow,      ImageRendererBrushJointInferred);
    CreateSolidBrush(D2D1::ColorF::Green,       ImageRendererBrushBoneTracked);
    CreateSolidBrush(D2D1::ColorF::Gray,        ImageRendererBrushBoneInferred);
    CreateSolidBrush(D2D1::ColorF::White,       ImageRendererBrushWhite);
    CreateSolidBrush(D2D1::ColorF::Gray,        ImageRendererBrushGray);
    CreateSolidBrush(D2D1::ColorF::Green,       ImageRendererBrushGreen);
}

/// <summary>
/// Create a specific D2D solid color brush from parameters
/// </summary>
/// <param name="color">The color used by brush</param>
/// <param name="brush">The index of brush</param>
void ImageRenderer::CreateSolidBrush(D2D1::ColorF::Enum color, ImageRendererBrush brush)
{
    m_pRenderTarget->CreateSolidColorBrush(D2D1::ColorF(color, 1.0f), &m_brushes[brush]);
}

/// <summary>
/// Create linear gradient brush to draw image red edge
/// </summary>
void ImageRenderer::CreateGradientBrush()
{
    // Create gradient stops
    D2D1_GRADIENT_STOP gradientStops[2];
    gradientStops[0].color    = D2D1::ColorF(D2D1::ColorF::Red, 0.5f);
    gradientStops[0].position = 0.0f;
    gradientStops[1].color    = D2D1::ColorF(D2D1::ColorF::Red, 0.0f);
    gradientStops[1].position = 1.0f;

    // Get gradient stop collection
    ID2D1GradientStopCollection* pGradientStops = nullptr;
    HRESULT hr = m_pRenderTarget->CreateGradientStopCollection(gradientStops, 2, &pGradientStops);
    if (SUCCEEDED(hr))
    {
        // Create linear gradient brush
        hr = m_pRenderTarget->CreateLinearGradientBrush(D2D1::LinearGradientBrushProperties(D2D1::Point2F(0.0f, 0.0f), D2D1::Point2F(0.0f, 0.0f)),
                                                        pGradientStops,
                                                        &m_pEdgeBrush);
    }

    //Release gradient stop collection
    SafeRelease(pGradientStops);
}

/// <summary>
/// Create text formats to draw FPS and image resolution lable
/// </summary>
void ImageRenderer::CreateTextFormats()
{
    CreateTextFormat(L"Segoe UI", 25.0f, DWRITE_TEXT_ALIGNMENT_CENTER,  DWRITE_PARAGRAPH_ALIGNMENT_CENTER, &m_formats[ImageRendererTextFormatFps]);        // FPS text format
    CreateTextFormat(L"Segoe UI", 12.0f, DWRITE_TEXT_ALIGNMENT_LEADING, DWRITE_PARAGRAPH_ALIGNMENT_NEAR,   &m_formats[ImageRendererTextFormatResolution]); // Resolution text format
}

/// <summary>
/// Create a specific DWrite text format from parameters
/// </summary>
/// <param name="fontFamilyName">Family name of created font</param>
/// <param name="fontSize">Size of created font</param>
/// <param name="textAlignment">Alignment of text</param>
/// <param name="paragraphAlignment">Alignment of paragraph</param>
/// <param name="ppTextFormat">Created text format</param>
void ImageRenderer::CreateTextFormat(const WCHAR* fontFamilyName, FLOAT fontSize, DWRITE_TEXT_ALIGNMENT textAlignment, DWRITE_PARAGRAPH_ALIGNMENT paragraphAlignment, IDWriteTextFormat** ppTextFormat)
{
    HRESULT hr = g_pDWriteFactory->CreateTextFormat(fontFamilyName, nullptr, DWRITE_FONT_WEIGHT_NORMAL, DWRITE_FONT_STYLE_NORMAL, DWRITE_FONT_STRETCH_NORMAL, fontSize, L"", ppTextFormat);
    if (SUCCEEDED(hr))
    {
        (*ppTextFormat)->SetTextAlignment(textAlignment);
        (*ppTextFormat)->SetParagraphAlignment(paragraphAlignment);
    }
}

/// <summary>
/// Release the dependent Direct2D resources
/// </summary>
void ImageRenderer::DiscardDependentResources()
{
    // Release all solid brushes
    for (int i = 0; i < ImageRendererBrushCount; i++)
    {
        SafeRelease(m_brushes[i]);
    }

    // Release all text formats
    for (int i = 0; i < ImageRendererTextFormatCount; i++)
    {
        SafeRelease(m_formats[i]);
    }

    // Release edge brush
    SafeRelease(m_pEdgeBrush);

    // Release D2D bitmap
    SafeRelease(m_pBitmap);

    // Release render target
    SafeRelease(m_pRenderTarget);
}

/// <summary>
/// Retrieve client area rect and transform into D2D size structure
/// </summary>
/// <param name="hWnd">The handle to window</param>
/// <returns>Client area size in D2D1_SIZE_U structure</returns>
D2D1_SIZE_U ImageRenderer::GetClientSize(HWND hWnd)
{
    RECT rect = {0};
    if (hWnd)
    {
        GetClientRect(hWnd, &rect);
    }

    return D2D1::SizeU(rect.right, rect.bottom);
}

/// <summary>
/// Resize render target to client area
/// </summary>
/// <param name="width">Width of client area</param>
/// <param name="height">Height of client area</param>
/// <returns>Indicates the success or failure</returns>
HRESULT ImageRenderer::ResizeRenderTarget(UINT width, UINT height)
{
    if (m_pRenderTarget)
    {
        return m_pRenderTarget->Resize(D2D1::SizeU(width, height));
    }

    return E_FAIL;
}

/// <summary>
/// Draws a 32 bits per pixel image of previously specified width, height and stride, to the associated hwnd
/// </summary>
/// <param name="pImage">The pointer to image buffer</param>
/// <param name="size">The image size</param>
/// <param name="rect">The rectangle in the window to render the image</param>
void ImageRenderer::DrawImage(const BYTE* pImage, const D2D1_SIZE_U& size, const D2D1_RECT_F& rect)
{
    // Check image buffer pointer
    if (!pImage)
    {
        return;
    }

    // Validate image size
    if (0 == size.width || 0 == size.height)
    {
        return;
    }

    // Ensure bitmap is created.
    HRESULT hr = EnsureBitmap(size);

    if (SUCCEEDED(hr))
    {
        // Copy the image from image buffer to D2D1 bit map
        hr = m_pBitmap->CopyFromMemory(NULL, pImage, size.width * sizeof(UINT));
        if (SUCCEEDED(hr))
        {
            // Draw the bitmap stretched to the size of the window
            m_pRenderTarget->DrawBitmap(m_pBitmap, rect);
        }
    }
}

/// <summary>
/// Draw the line representing the bone
/// </summary>
/// <param name="point0">The start point of the line</param>
/// <param name="point1">The end point of the line</param>
/// <param name="brush">The index of brush</param>
/// <param name="strokeWidth">Width of the line</param>
/// <param name="strokeStyle">Style of the line</param>
void ImageRenderer::DrawLine(const D2D1_POINT_2F& point0, const D2D1_POINT_2F& point1, ImageRendererBrush brush, float strokeWidth, ID2D1StrokeStyle* strokeStyle)
{
    // Validate brush index
    if (brush < 0 || brush >= ImageRendererBrushCount)
    {
        return;
    }

    // Select created brush
    ID2D1SolidColorBrush* pBrush = m_brushes[brush];

    // Draw the line
    m_pRenderTarget->DrawLine(point0, point1, pBrush, strokeWidth, strokeStyle);
}

/// <summary>
/// Draw the circle representing the joint
/// </summary>
/// <param name="center">The center of the circle</param>
/// <param name="radius">The radius of the circle</param>
/// <param name="brush">Index of brush</param>
/// <param name="strokeWidth">Width of the line</param>
/// <param name="strokeStyle">Style of the line</param>
void ImageRenderer::DrawCircle(const D2D1_POINT_2F& center, float radius, ImageRendererBrush brush, float strokeWidth, ID2D1StrokeStyle* strokeStyle)
{
    // Validate brush index
    if (brush < 0 || brush >= ImageRendererBrushCount)
    {
        return;
    }

    // Select created brush
    ID2D1SolidColorBrush* pBrush = m_brushes[brush];

    // Create ellipse with same radius on both long and short axis. Identical to the circle
    D2D1_ELLIPSE ellipse = D2D1::Ellipse(center, radius, radius);

    // Draw ellipse
    m_pRenderTarget->DrawEllipse(ellipse, pBrush, strokeWidth, strokeStyle);
}

/// <summary>
/// Draw FPS text
/// </summary>
/// <param name="pText">Text to draw</param>
/// <param name="cch">Count of charaters in text</param>
/// <param name="rect">The area to draw text</param>
/// <param name="brush">Index of brush</param>
/// <param name="format">Text format</param>
void ImageRenderer::DrawText(const WCHAR* pText, UINT cch, const D2D1_RECT_F &rect, ImageRendererBrush brush, ImageRendererTextFormat format)
{
    // Validate pointer to text
    if (!pText)
    {
        return;
    }

    // Validate brush index
    if (brush < 0 || brush >= ImageRendererBrushCount)
    {
        return;
    }

    // Validate text format index
    if (format < 0 || format >= ImageRendererTextFormatCount)
    {
        return;
    }

    // Select created text format
    IDWriteTextFormat* pFormat = m_formats[format];

    // Select created brush
    ID2D1SolidColorBrush* pBrush = m_brushes[brush];

    // Draw text to rectangle area
    m_pRenderTarget->DrawText(pText, cch, pFormat, rect, pBrush);
}

/// <summary>
/// Draw red edge on the image when skeleton is outside the image area
/// </summary>
/// <param name="rect">The rectangle on image edge to render in red</param>
/// <param name="start">The start point of gradient brush</param>
/// <param name="end">The end point of gradient brush</param>
void ImageRenderer::DrawEdge(const D2D1_RECT_F& rect, const D2D1_POINT_2F& start, const D2D1_POINT_2F& end)
{
    // Set start and end point to gradient brush
    m_pEdgeBrush->SetStartPoint(start);
    m_pEdgeBrush->SetEndPoint(end);

    // Draw rectangle
    m_pRenderTarget->FillRectangle(rect, m_pEdgeBrush);
}

/// <summary>
/// Draw a rectangle and fill it
/// </summary>
/// <param name="rect">The rectangle to fill</param>
/// <param name="brush">The index of brush</param>
void ImageRenderer::FillRectangle(const D2D1_RECT_F& rect, ImageRendererBrush brush)
{
    // Validate brush index
    if (brush < 0 || brush >= ImageRendererBrushCount)
    {
        return;
    }

    // Select brush
    ID2D1SolidColorBrush* pBrush = m_brushes[brush];

    // Draw rectangle
    m_pRenderTarget->FillRectangle(rect, pBrush);
}

/// <summary>
/// Set clip area to prevent joints and bones are drawn outside the image
/// </summary>
/// <param name="rect">The clip area</param>
void ImageRenderer::SetClipRect(const D2D1_RECT_F& rect)
{
    m_pRenderTarget->PushAxisAlignedClip(&rect, D2D1_ANTIALIAS_MODE_PER_PRIMITIVE);
}

/// <summary>
/// Clear previously set clip area.
/// </summary>
void ImageRenderer::ResetClipRect()
{
    m_pRenderTarget->PopAxisAlignedClip();
}