/**
* ImageRenderer.h
* Modfied from Kinect SDK v.1.8
* alex.hong@mail.utoronto.ca
*/

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
	/**
	 * Constructor
	 */
	ImageRenderer();

	/**
	 * Destructor
	 */
	~ImageRenderer();

public:
	/**
	 * Resize render target to client area
	 * @param   width   Width of client area
	 * @param   height  height  Height of client area
	 * @return  Indicates the success or failure
	 */
	HRESULT ResizeRenderTarget(UINT width, UINT height);

	/**
	 * Begin drawing process. Called before drawing to the window
	 * @param   handle  to window to which renderer draws
	 * @return  Indicates success of failure
	 */
	HRESULT BeginDraw(HWND hWnd);

	/**
	 * Set clip area to prevent joints and bones are drawn outside the image
	 * @param  rect  The clip area
	 */
	void SetClipRect(const D2D1_RECT_F& rect);

	/**
	 * Clear previously set clip area
	 */
	void ResetClipRect();

	/**
	 * Draws a 32 bits per pixel image of previously specified width, height and stride, to the associated hwnd
	 * @param  pImage  The pointer to image buffer
	 * @param  size    The image size
	 * @param  rect    The rectangle in the window to render the image
	 */
	void DrawImage(const BYTE* pImage, const D2D1_SIZE_U& size, const D2D1_RECT_F& rect);

	
	/** 
	  * Draw FPS text
	  * @param  pText   Text to draw
	  * @param  cch     Count of charaters in text
	  * @param  rect    The area to draw text
	  * @param  brush   Index of brush
	  * @param  format  Text format
	  */
	void DrawText(const WCHAR* pText, UINT cch, const D2D1_RECT_F &rect, 
		          ImageRendererBrush brush, ImageRendererTextFormat format);

	
	/** 
	 * Draw the line representing the bone
	 * @param  point0       The start point of the line
	 * @param  point1       The end point of the line
	 * @param  brush        The index of brush
	 * @param  strokeWidth  Width of the line
	 * @param  strokeStyle  Style of the line
	 */
	void DrawLine(const D2D1_POINT_2F& point0, const D2D1_POINT_2F& point1, ImageRendererBrush brush, 
		          float strokeWidth = 1.0f, ID2D1StrokeStyle* strokeStyle = nullptr);

	
	/**
	 * Draw the circle representing the joint
	 * @param  center       The center of the circle
	 * @param  radius       The radius of the circle
	 * @param  brush        Index of brush
	 * @param  strokeWidth  Width of the line
	 * @param  strokeStyle  Style of the line
	 */
	void DrawCircle(const D2D1_POINT_2F& center, float radius, ImageRendererBrush brush, 
		            float strokeWidth = 1.0f, ID2D1StrokeStyle* strokeStyle = nullptr);

	
	/**
	 * Draw red edge on the image when skeleton is outside the image area
	 * @param  rect   The rectangle on image edge to render in red
	 * @param  start  The start point of gradient brush
	 * @param  end    The end point of gradient brush
	 */
	void DrawEdge(const D2D1_RECT_F& rect, const D2D1_POINT_2F& start, const D2D1_POINT_2F& end);

	
	/**
	 * Draw a rectangle and fill it
	 * @param  rect   The rectangle to fill
	 * @param  brush  The index of brush
	 */
	void FillRectangle(const D2D1_RECT_F& rect, ImageRendererBrush brush);

	
	/**
	 * End drawing process. Called after finish drawing
	 */
	void EndDraw();

private:

	/**
	 *Ensure necesary dependent Direct2D resources are created
	 * @param   hWnd  Handle to window to which renderer draws
	 * @return  Indicates success or failure
	 */
	HRESULT EnsureDependentResourcesCreated(HWND hWnd);

	
	/**
	 * Release the dependent Direct2D resources
	 */
	void DiscardDependentResources();

	
	/**
	 * Ensure render target is created
	 * @return  Indicates success or failure
	 */
	HRESULT EnsureRenderTarget();

	
	/**
	 * Ensure bitmap is created
	 * @param   imageSize  Size of bit map
	 * @retrun  Indicate success or failure
	 */
	HRESULT EnsureBitmap(const D2D1_SIZE_U& imageSize);

	
	/**
	 * Create D2D solid color brushes
	 */
	void CreateSolidBrushes();

	
	/**
	 * Create a specific D2D solid color brush from parameters
	 * @param  color  The color used by brush
	 * @param  brush  The index of brush
	 */
	void CreateSolidBrush(D2D1::ColorF::Enum color, ImageRendererBrush brush);

	
	/**
	 * Create linear gradient brush to draw image red edge
	 */
	void CreateGradientBrush();

	/**
	 * Create text formats to draw FPS and image resolution lable
	 */
	void CreateTextFormats();

	
	/** 
	 * Create a specific DWrite text format from parameters
	 * @param  fontFamilyName      Family name of created font
	 * @param  fontSize            Size of created font
	 * @param  textAlignment       Alignment of text
	 * @param  paragraphAlignment  Alignment of paragraph
	 * @param  ppTextFormat        Created text format
	 */
	void CreateTextFormat(const WCHAR* fontFamilyName, FLOAT fontSize, DWRITE_TEXT_ALIGNMENT textAlignment, 
                          DWRITE_PARAGRAPH_ALIGNMENT paragraphAlignment, IDWriteTextFormat** ppTextFormat);

	/**
	 * Retrieve client area rect and transform into D2D size structure
	 * @param   hWnd  The handle to window
	 * @return  Client area size in D2D1_SIZE_U structure
	 */
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