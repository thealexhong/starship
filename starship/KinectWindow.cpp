/**
* KinectWindow.cpp
* Modfied from Kinect SDK v.1.8
* alex.hong@mail.utoronto.ca
*/

#include "stdafx.h"
#include "KinectWindow.h"
#include "NuiStreamViewer.h"
#include "NuiStream.h"
#include "Utility.h"
#include "resource.h"
#include "CameraColorSettingsViewer.h"
#include "CameraExposureSettingsViewer.h"

// Window size definations
#define PRIMARY_VIEW_MIN_WIDTH      480
#define PRIMARY_VIEW_MIN_HEIGHT     360
#define SECOND_VIEW_FIXED_WIDTH     480
#define SECOND_VIEW_FIXED_HEIGHT    360
#define TABBED_VIEW_FIXED_HEIGHT    600
#define TAB_CONTROL_FIXED_HEIGHT    25
#define GAP_BETWEEN_VIEWS           5

// Reoccurence period in millisecond of waitable timer. This timer is used to trigger processing of timed stream data.
#define TIMER_PERIOD                20

// Titles of tab control items
#define TAB_TITLE_SKELETON            L"Skeleton"
#define TAB_TITLE_BLFEATURES          L"BL Features"
#define TAB_TITLE_BLCLASSIFICATION    L"BL Classification"
#define TAB_TITLE_VFEATURES           L"V Features"
#define TAB_TITLE_VCLASSIFICATION     L"V Classification"
/**
#define TAB_TITLE_AUDIO               L"Audio"
#define TAB_TITLE_ACCELEROMETER       L"Accelerometer"
#define TAB_TITLE_TILTANGLE           L"Sensor Settings"
*/

// Index of tab control items
#define TAB_INDEX_SKELETON            0
#define TAB_INDEX_BLFEATURES          1
#define TAB_INDEX_BLCLASSIFICATION    2
#define TAB_INDEX_VFEATURES           3
#define TAB_INDEX_VCLASSIFICATION     4
/*
#define TAB_INDEX_AUDIO               3
#define TAB_INDEX_ACCELEROMETER       4
#define TAB_INDEX_TILTANGLE           5
*/

#define ERROR_MESSAGE_BUFFER_SIZE   1024

// Menu item positions
static const int ColorStreamMenuPosition            = 0;
static const int DepthStreamMenuPosition            = 1;
static const int SkeletonStreamMenuPosition         = 2;
static const int CameraSettingMenuPositon           = 3;

static const int ColorResolutionMenuPosition        = 1;
static const int DepthRangeModeMenuPosition         = 1;
static const int DepthResolutionMenuPosition        = 2;
static const int DepthTreatmentMenuPosition         = 3;
static const int SkeletonTrackingModeMenuPosition   = 1;
static const int SkeletonChooserModeMenuPosition    = 2;

/**
 * Constructor
 * @param  hInstance   Handle to the application instance
 * @param  hWndParent  Handle to main console window
 * @param  pNuiSensor  Pointer to Nui sensor instance
 */
KinectWindow::KinectWindow(HINSTANCE hInstance, HWND hWndParent, INuiSensor* pNuiSensor) : NuiViewer(nullptr),
                                                                                           m_hWndTab(nullptr),
																						   m_hWndParent(hWndParent),
																						   m_hInstance(hInstance),
																						   m_hTimer(nullptr),
																						   m_hThread(nullptr),
																						   m_pNuiSensor(pNuiSensor),
																						   m_bSupportCameraSettings(true),
																						   m_hStartWindow(INVALID_HANDLE_VALUE),
																						   m_hStopStreamEventThread(INVALID_HANDLE_VALUE)
{
    assert(m_pNuiSensor);
    m_pNuiSensor->AddRef();

    // Create instances of sub views
    m_pPrimaryView          = new NuiStreamViewer(this, TRUE);
    m_pSecondaryView        = new NuiStreamViewer(this, FALSE);
	m_pSkeletonPointsView   = new NuiSkeletonPointsViewer(this);
	m_pBLFeatureView        = new NuiBLFeatureViewer(this);
	m_pBLClassificationView = new NuiBLClassificationViewer(this);
	m_pPrimaryView->SetStreamViewer(m_pSkeletonPointsView, m_pBLFeatureView, m_pBLClassificationView);
	/*
    m_pAudioView            = new NuiAudioViewer(this);
    m_pAccelView            = new NuiAccelerometerViewer(this);
    m_pTiltAngleView        = new NuiTiltAngleViewer(this, pNuiSensor);
	*/
    m_pCurTabbedView        = nullptr;
    m_pColorSettingsView    = new CameraColorSettingsViewer(this);
    m_pExposureSettingsView = new CameraExposureSettingsViewer(this);

    m_views.push_back(m_pPrimaryView);
    m_views.push_back(m_pSecondaryView);
	m_views.push_back(m_pSkeletonPointsView);
	m_views.push_back(m_pBLFeatureView);
	m_views.push_back(m_pBLClassificationView);
	/*
    m_views.push_back(m_pAudioView);
    m_views.push_back(m_pAccelView);
    m_views.push_back(m_pTiltAngleView);
	*/

    // Group camera setting views
    m_settingViews.push_back(m_pColorSettingsView);
    m_settingViews.push_back(m_pExposureSettingsView);

    // Group tabbed sub views together

	m_tabbedViews.push_back((m_pSkeletonPointsView));
	m_tabbedViews.push_back((m_pBLFeatureView));
	m_tabbedViews.push_back((m_pBLClassificationView));
	/*
    m_tabbedViews.push_back((m_pAudioView));
    m_tabbedViews.push_back((m_pAccelView));
    m_tabbedViews.push_back((m_pTiltAngleView));
	*/

    // Create stream objects
    m_pColorStream         = new NuiColorStream(m_pNuiSensor);
    m_pDepthStream         = new NuiDepthStream(m_pNuiSensor);
    m_pSkeletonStream      = new NuiSkeletonStream(m_pNuiSensor);
	// m_pSkeletonPointsStream = new NuiSkeletonPointsStream(m_pNuiSensor);
	/*
    m_pAudioStream         = new NuiAudioStream(m_pNuiSensor);
    m_pAccelerometerStream = new NuiAccelerometerStream(m_pNuiSensor);
	*/

    // Attach stream objects to viewers
    m_pColorStream->SetStreamViewer(m_pPrimaryView);
    m_pDepthStream->SetStreamViewer(m_pSecondaryView);
    m_pSkeletonStream->SetStreamViewer(m_pPrimaryView);
    m_pSkeletonStream->SetSecondStreamViewer(m_pSecondaryView);
	// m_pSkeletonPointsStream->SetStreamViewer(m_pSkeletonPointsView);
	/*
    m_pAudioStream->SetStreamViewer(m_pAudioView);
    m_pAccelerometerStream->SetStreamViewer(m_pAccelView);
	*/

    // Create settings object
    m_pSettings = new KinectSettings(m_pNuiSensor,
                                     m_pPrimaryView,
                                     m_pSecondaryView,
                                     m_pColorStream,
                                     m_pDepthStream,
                                     m_pSkeletonStream,
                                     m_pColorSettingsView,
                                     m_pExposureSettingsView);
}

/**
 * Destructor
 */
KinectWindow::~KinectWindow()
{
    CleanUp();
}


/**
 * Initialization
 * @return  Indicate success or failure
 */
bool KinectWindow::Initialize()
{
    // Check Nui sensor pointer
    if (!m_pNuiSensor)
    {
        return false;
    }

    // Initialize common control for tab control
    if (!InitializeCommonControl())
    {
        return false;
    }

    if (S_OK != m_pNuiSensor->NuiStatus())
    {
        return false;
    }

    // Initialize Nui sensor
    HRESULT hr = m_pNuiSensor->NuiInitialize(
        NUI_INITIALIZE_FLAG_USES_DEPTH_AND_PLAYER_INDEX
        | NUI_INITIALIZE_FLAG_USES_SKELETON
        | NUI_INITIALIZE_FLAG_USES_COLOR
        | NUI_INITIALIZE_FLAG_USES_AUDIO);

    // Ensure infrared emitter enabled
    if (SUCCEEDED(hr))
    {
        m_pNuiSensor->NuiSetForceInfraredEmitterOff(FALSE);
    }

    return SUCCEEDED(hr) || E_NUI_DEVICE_IN_USE == hr;
}


/**
 * Initialize common control.
 * @return  Indicates success or failure
 */
bool KinectWindow::InitializeCommonControl()
{
    static bool initialized = false;
    if (!initialized)
    {
        // Initliaze common control for tab control.
        INITCOMMONCONTROLSEX icex;
        icex.dwSize = sizeof(INITCOMMONCONTROLSEX);
        icex.dwICC  = ICC_TAB_CLASSES;
        initialized = (FALSE != InitCommonControlsEx(&icex));
    }

    return initialized;
}


/**
 * Initialize camera setting viewers
 * @return  Indicate success or failure
 */
bool KinectWindow::CreateCameraSettingViews()
{
    INuiColorCameraSettings* pNuiCameraSettings = nullptr;

    // The device support camera settings
    if (S_OK == m_pNuiSensor->NuiGetColorCameraSettings(&pNuiCameraSettings))
    {
        m_bSupportCameraSettings = true;

        for (auto iter = m_settingViews.begin(); iter != m_settingViews.end(); ++iter)
        {
            if (!(*iter)->CreateView())
            {
                SafeRelease(pNuiCameraSettings);
                return false;
            }
            (*iter)->Initialize(pNuiCameraSettings);
        }
    }
    else
    {
        // The device does not support camera settings
        m_bSupportCameraSettings = false;
    }

    SafeRelease(pNuiCameraSettings);

    return true;
}


/**
 * Start a new thread and Kinect window runs in it
 * @return  Handle to thread
 */
HANDLE KinectWindow::StartWindow()
{
    if (!m_hThread)
    {
        // Create the sync event
        m_hStartWindow = CreateEventW(nullptr, TRUE, FALSE, nullptr);

        // Create a new thread
        m_hThread = CreateThread(nullptr,
                                 0,
                                 (LPTHREAD_START_ROUTINE)ThreadProc,
                                 (LPVOID)this,
                                 0,
                                 nullptr);

        // Ensure the window has been created before the the function return,
        // otherwise, if sensor status changes quickly, Kinecet Window manager may close a uncreated window
        WaitForSingleObject(m_hStartWindow, INFINITE);
    }

    return m_hThread;
}


/// Explictly called by main window to close Kinect window and stop the thread on exit of main window

void KinectWindow::NotifyOfExit()
{
    HideView();

    m_hWndParent = nullptr; // Don't need Kinect window send back message of quit in this case
    PostMessageW(m_hWnd, WM_CLOSE, 0, 0);
}


/**
 * Returns the handle to the thread of the Kinect window
 * @return  The thread handle
 */
HANDLE KinectWindow::GetThreadHandle() const
{
    return m_hThread;
}


/**
 * The thread procedure in which the window runs
 * @param   pThis  The pointer to Kinect window instance
 * @return  The result from message loop
 */
DWORD WINAPI KinectWindow::ThreadProc(KinectWindow* pThis)
{
    DWORD result = 0;

    // Kinect window runs
    if (pThis->Initialize() && pThis->CreateWindows())
    {
        pThis->InitializeMenu();
        pThis->StartStreams();

        // Signal the event to inform that Kinect window has started
        SetEvent(pThis->m_hStartWindow);

        result = (DWORD)pThis->MessageLoop();
    }
    else
    {
        // Signal the event to avoid main thread hang due to initialization error
        SetEvent(pThis->m_hStartWindow);
    }

    if (pThis->m_hWndParent)
    {
        // Send message back to console window notifying the exit of Kinect window
        SendMessageW(pThis->m_hWndParent, WM_CLOSEKINECTWINDOW, (WPARAM)(pThis->m_pNuiSensor ? pThis->m_pNuiSensor->NuiDeviceConnectionId() : nullptr), 0);
    }

    // Delete Kinect window before thread ends
    delete pThis;

    return result;
}


/**
 * Window message loop
 * @return  wParam of last received message
 */
WPARAM KinectWindow::MessageLoop()
{
    m_hStopStreamEventThread = CreateEventW(nullptr, TRUE, FALSE, nullptr);

    HANDLE hEventThread = CreateThread(nullptr, 0, (LPTHREAD_START_ROUTINE)StreamEventThread, this, 0, nullptr);

    MSG  msg = {0};
    BOOL ret;
    while (0 != (ret = GetMessageW(&msg, nullptr, 0, 0)))
    {
        if (-1 == ret)
        {
            break;
        }

        if (IsDialogMessageW(m_hWnd, &msg))
        {
            continue;
        }

        TranslateMessage(&msg);
        DispatchMessageW(&msg);
    }

    WaitForSingleObject(hEventThread, INFINITE);
    CloseHandle(hEventThread);

    return msg.wParam;
}


/**
 * Dispatch the message to the handler function
 * @param   hWnd    The handle to the window which receives the message
 * @param   uMsg    The message identifier
 * @param   wParam  The additional message information
 * @param   lParam  The additional message information
 * @return  If the message has been processed by handler function, TRUE is returned. Otherwise FALSE is returned and the message is handled by default dialog procedure
 */
LRESULT KinectWindow::DialogProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch (uMsg)
    {
    case WM_STREAMEVENT:
        UpdateStreams();
        break;

    case WM_TIMEREVENT:
        UpdateTimedStreams();
        break;

    case WM_INITDIALOG:
        NuiViewer::SetIcon(hWnd);
        break;

    case WM_SIZE:
        OnResize();
        break;

    case WM_COMMAND:
        OnCommand(wParam);
        break;

    case WM_NOTIFY:
        OnNotify(lParam);
        break;

    case WM_CLOSE:
        OnClose(hWnd, wParam);
        break;

    case WM_DESTROY:
        PostQuitMessage(0);
        break;

    case WM_SHOWKINECTWINDOW:
        {
            if (0 != wParam)
            {
                ShowWindows();
            }
        }
        break;

    default:
        break;
    }

    return FALSE;
}


/**
 * Returns ID of the dialog
 * @return  ID of dialog
 */
UINT KinectWindow::GetDlgId()
{
    return IDD_KINECT_WINDOW;
}


/**
 * Create Kinect window and its sub views
 * @return  Indicates success or failure
 */
bool KinectWindow::CreateWindows()
{
    // Create window for Kinect window
    if (!this->CreateView())
    {
        return false;
    }

    // Create window for sub views
    for (auto itr = m_views.begin(); itr != m_views.end(); itr++)
    {
        if (!(*itr)->CreateView())
        {
            return false;
        }
    }

    // Creata camera setting views
    if (!CreateCameraSettingViews())
    {
        return false;
    }

    return CreateTabControl();
}


/**
 * Create tab control
 * @return  Indicates success or failure
 */
bool KinectWindow::CreateTabControl()
{
    if (!m_hWndTab)
    {
        m_hWndTab = CreateWindowW(WC_TABCONTROL,
                                  L"",
                                  WS_CHILD | WS_CLIPSIBLINGS,
                                  CW_USEDEFAULT,
                                  CW_USEDEFAULT,
                                  CW_USEDEFAULT,
                                  CW_USEDEFAULT,
                                  m_hWnd,
                                  nullptr,
                                  m_hInstance,
                                  nullptr);

        if (m_hWndTab)
        {
            TCITEMW tci     = {0};
            tci.mask        = TCIF_TEXT;
            tci.iImage      = -1;

			InsertTabItem(TAB_TITLE_SKELETON,         TAB_INDEX_SKELETON);
			InsertTabItem(TAB_TITLE_BLFEATURES,       TAB_INDEX_BLFEATURES);
			InsertTabItem(TAB_TITLE_BLCLASSIFICATION, TAB_INDEX_BLCLASSIFICATION);
			InsertTabItem(TAB_TITLE_VFEATURES,        TAB_INDEX_VFEATURES);
			InsertTabItem(TAB_TITLE_VCLASSIFICATION,  TAB_INDEX_VCLASSIFICATION);
			
			/*
            InsertTabItem(TAB_TITLE_AUDIO,          TAB_INDEX_AUDIO);
            InsertTabItem(TAB_TITLE_ACCELEROMETER,  TAB_INDEX_ACCELEROMETER);
            InsertTabItem(TAB_TITLE_TILTANGLE,      TAB_INDEX_TILTANGLE);
			*/
        }
    }

    return (nullptr != m_hWndTab);
}


/**
 * Insert a tab item to tab control
 * @param   title  Text displayed on tab item
 * @param   index  Index of tab item
 */
void KinectWindow::InsertTabItem(LPWSTR title, int index)
{
    TCITEMW tci    = {0};
    tci.mask       = TCIF_TEXT;
    tci.iImage     = -1;
    tci.pszText    = title;
    tci.cchTextMax = (int)wcslen(title) + 1;

    TabCtrl_InsertItem(m_hWndTab, index, &tci);
}


/**
 * Display Kinect window and its sub views.
 */
void KinectWindow::ShowWindows()
{
    // Kinect window
    this->ShowView();

    // Tab control
    ShowWindow(m_hWndTab, SW_SHOW);
    UpdateWindow(m_hWndTab);

    // Primary and secondary views
    m_pPrimaryView->ShowView();
    m_pSecondaryView->ShowView();

    // Show first tabbed view
    if (m_tabbedViews.size() > 0)
    {
        m_pCurTabbedView = m_tabbedViews[0];
        m_pCurTabbedView->ShowView();
    }

    // Bring Kinect window to top
    BringUpWindow();
}


/**
 * Bring up Kinect window on top of main console window
 */
void KinectWindow::BringUpWindow()
{
    SetForegroundWindow(m_hWnd);
}


/**
 * Start all streams and timer
 */
void KinectWindow::StartStreams()
{
    // Color stream
    m_pColorStream->StartStream();

    // Depth stream
    m_pDepthStream->StartStream();

    // Skeleton stream
    m_pSkeletonStream->StartStream();

	// m_pSkeletonPointsStream->StartStream();
	/*
    // Audio reading stream
    m_pAudioStream->StartStream();

    // Accelerometer reading stream
    m_pAccelerometerStream->StartStream();
	*/

    // Start waitble timer
    StartTimer();
}


/**
 * Start the waitable timer to trigger process of timed streams
 */
void KinectWindow::StartTimer()
{
    m_hTimer = CreateWaitableTimerW(nullptr, FALSE, nullptr);
    if (m_hTimer)
    {
        LARGE_INTEGER dueTime = {0};
        SetWaitableTimer(m_hTimer, &dueTime, TIMER_PERIOD, nullptr, nullptr, FALSE);
    }
}


/**
 * Release all resources
 */
void KinectWindow::CleanUp()
{
    if (m_hTimer)
    {
        CloseHandle(m_hTimer);
        m_hTimer = nullptr;
    }

    SafeDelete(m_pColorStream);
    SafeDelete(m_pDepthStream);
    SafeDelete(m_pSkeletonStream);
	SafeDelete(m_pCurTabbedView);
	// SafeDelete(m_pSkeletonPointsStream);
	/*
    SafeDelete(m_pAudioStream);
    SafeDelete(m_pAccelerometerStream);
	*/
    SafeDelete(m_pPrimaryView);
    SafeDelete(m_pSecondaryView);
	//SafeDelete(m_pSkeletonPointsView);
	//SafeDelete(m_pBLFeatureView);
	//SafeDelete(m_pBLClassificationView);
	/*
    SafeDelete(m_pAudioView);
    SafeDelete(m_pAccelView);
    SafeDelete(m_pTiltAngleView);
	*/
    SafeDelete(m_pColorSettingsView);
    SafeDelete(m_pExposureSettingsView);
    SafeDelete(m_pSettings);

    SafeRelease(m_pNuiSensor);

    if (INVALID_HANDLE_VALUE != m_hStartWindow)
    {
        CloseHandle(m_hStartWindow);
        m_hStartWindow = INVALID_HANDLE_VALUE;
    }

    if (INVALID_HANDLE_VALUE != m_hStopStreamEventThread)
    {
        CloseHandle(m_hStopStreamEventThread);
        m_hStopStreamEventThread = INVALID_HANDLE_VALUE;
    }
}


/**
 * Handler function for WM_SIZE message
 */
void KinectWindow::OnResize()
{
    // Move window of sub views to their positions.
    RECT priRect, secRect, tabRect, tabbedRect;
    if (CalculateViewRects(priRect, secRect, tabRect, tabbedRect))
    {
        // Primary stream view.
        m_pPrimaryView->MoveView(priRect);

        // Secondary stream view.
        m_pSecondaryView->MoveView(secRect);

        // Tab control.
        MoveWindow(m_hWndTab,
                     tabRect.left,
                     tabRect.top,
                     tabRect.right - tabRect.left,
                     tabRect.bottom - tabRect.top,
                     TRUE);

        // Tabbed views.
        for (auto itr = m_tabbedViews.begin(); itr != m_tabbedViews.end(); itr++)
        {
            (*itr)->MoveView(tabbedRect);
        }
    }
}


/**
 * Handler function for menu commands
 * @param   wParam  Command parameter
 */
void KinectWindow::OnCommand(WPARAM wParam)
{
    WORD id     = LOWORD(wParam);   // Get menu item ID
    WORD param  = HIWORD(wParam);   // Get command parameter

    bool itemChecked = false;

    // Update memu item status
    if (ProcessMenuItem(id, itemChecked))
    {
        // Process menu item command
        m_pSettings->ProcessMenuCommand(id, param, itemChecked);
    }
}


/**
 * Initialize menu control on Kinect window. Set initial check status for menu items
 */
void KinectWindow::InitializeMenu()
{
    HMENU hMenu = GetMenu(m_hWnd);
    if (hMenu)
    {
        // Set initial check status for radio items in menu
        CheckMenuRadioItem(hMenu,
                             ID_COLORSTREAM_RESOLUTION_START,
                             ID_COLORSTREAM_RESOLUTION_END,
                             ID_RESOLUTION_RGBRESOLUTION640X480FPS30,
                             MF_BYCOMMAND);
        CheckMenuRadioItem(hMenu,
                             ID_DEPTHSTREAM_RANGEMODE_START,
                             ID_DEPTHSTREAM_RANGEMODE_END,
                             ID_RANGEMODE_DEFAULT,
                             MF_BYCOMMAND);
        CheckMenuRadioItem(hMenu,
                             ID_DEPTHSTREAM_RESOLUTION_START,
                             ID_DEPTHSTREAM_RESOLUTION_END,
                             ID_RESOLUTION_RESOLUTION640X480FPS30,
                             MF_BYCOMMAND);
        CheckMenuRadioItem(hMenu,
                             ID_DEPTHSTREAM_DEPTHTREATMENT_START,
                             ID_DEPTHSTREAM_DEPTHTREATMENT_END,
                             ID_DEPTHTREATMENT_CLAMPUNRELIABLEDEPTHS,
                             MF_BYCOMMAND);
        CheckMenuRadioItem(hMenu,
                             ID_SKELETONSTREAM_TRACKINGMODE_START,
                             ID_SKELETONSTREAM_TRACKINGMODE_END,
                             ID_TRACKINGMODE_DEFAULT,
                             MF_BYCOMMAND);
        CheckMenuRadioItem(hMenu,
                             ID_SKELETONSTREAM_CHOOSERMODE_START,
                             ID_SKELETONSTREAM_CHOOSERMODE_END,
							 ID_CHOOSERMODE_CLOSEST1PLAYER,
                             MF_BYCOMMAND);

        // This device does not support camera settings
        if (!m_bSupportCameraSettings)
        {
            // Delete camera setting menu
            DeleteMenu(hMenu, CameraSettingMenuPositon, MF_BYPOSITION);

            // Disable IR force off button
            // EnableWindow(GetDlgItem(m_pTiltAngleView->GetWindow(), IDC_FORCE_OFF_IR), FALSE);
        }
    }
}


/**
 * Update menu item status
 * @param   id       Identifier of menu item
 * @param   checked  Check status of menu item
 * @return  Indicates success or failure
 */
bool KinectWindow::ProcessMenuItem(UINT id, bool& checked)
{
    if (ID_FORCE_OFF_IR == id)
    {
        return true;
    }

    // Menu item command
    HMENU hMenu = GetMenu(m_hWnd);
    if (hMenu)
    {
        if (!GetMenuItemCheckStatus(hMenu, id, checked))
        {
            // Failed to get menu item check status
            return false;
        }

        switch(id)
        {
        // For "Pause" menu items, invert check status and enable or disable associated stream setting menus
        case ID_COLORSTREAM_PAUSE:
            if (EnablePopupMenuItem(hMenu, ColorStreamMenuPosition, ColorResolutionMenuPosition, checked))
            {
                return InvertCheckMenuItem(hMenu, id, checked);
            }
            break;

        case ID_DEPTHSTREAM_PAUSE:
            if (EnablePopupMenuItem(hMenu, DepthStreamMenuPosition, DepthRangeModeMenuPosition,  checked) &&
                EnablePopupMenuItem(hMenu, DepthStreamMenuPosition, DepthResolutionMenuPosition, checked) &&
                EnablePopupMenuItem(hMenu, DepthStreamMenuPosition, DepthTreatmentMenuPosition,  checked))
            {
                return InvertCheckMenuItem(hMenu, id, checked);
            }
            break;

        case ID_SKELETONSTREAM_PAUSE:
            if (EnablePopupMenuItem(hMenu, SkeletonStreamMenuPosition, SkeletonTrackingModeMenuPosition, checked) &&
                EnablePopupMenuItem(hMenu, SkeletonStreamMenuPosition, SkeletonChooserModeMenuPosition,  checked))
            {
                return InvertCheckMenuItem(hMenu, id, checked);
            }
            break;

        case ID_VIEWS_SWITCH:
        case ID_CAMERA_COLORSETTINGS:
        case ID_CAMERA_EXPOSURESETTINGS:
            // These item don't need to modify their check status
            return true;

        // For rest of the menu items (radio menu items)
        default:
            {
                // Process radio button check status
                if (checked)
                {
                    // Selection doesn't change
                    break;
                }

                if (CheckRadioItem(id, ID_SKELETONSTREAM_CHOOSERMODE_START, ID_SKELETONSTREAM_CHOOSERMODE_END, hMenu))
                {
                    // Skeleton stream chooser mode
                    return true;
                }
                else if (CheckRadioItem(id, ID_SKELETONSTREAM_TRACKINGMODE_START, ID_SKELETONSTREAM_TRACKINGMODE_END, hMenu))
                {
                    // Skeleton stream tracking mode
                    return true;
                }
                else if (CheckRadioItem(id, ID_DEPTHSTREAM_DEPTHTREATMENT_START, ID_DEPTHSTREAM_DEPTHTREATMENT_END, hMenu))
                {
                    // Depth stream depth treatment
                    return true;
                }
                else if (CheckRadioItem(id, ID_DEPTHSTREAM_RESOLUTION_START, ID_DEPTHSTREAM_RESOLUTION_END, hMenu))
                {
                    // Depth stream image resolution
                    return true;
                }
                else if (CheckRadioItem(id, ID_DEPTHSTREAM_RANGEMODE_START, ID_DEPTHSTREAM_RANGEMODE_END, hMenu))
                {
                    // Depth stream range mode
                    return true;
                }
                else if (CheckRadioItem(id, ID_COLORSTREAM_RESOLUTION_START, ID_COLORSTREAM_RESOLUTION_END, hMenu))
                {
                    // Color stream image resolution
                    return true;
                }
            }
        }
    }

    return false;
}


/**
 * Enable or disable popup menu
 * @param   hMenu">Handle to window menu
 * @param   subMenuPosition    Position of sub menu which owns the pop up menu item
 * @param   popupMenuPosition  Position of popup menu to enable or disable
 * @param   enable">True to enable and false to disable the menu
 * @return  Indicates success or failure
 */
bool KinectWindow::EnablePopupMenuItem(HMENU hMenu, UINT subMenuPosition, UINT popupMenuPosition, bool enable)
{
    HMENU hSubMenu = GetSubMenu(hMenu, subMenuPosition);
    if (hSubMenu)
    {
        return (-1 != EnableMenuItem(hSubMenu, popupMenuPosition, MF_BYPOSITION | (enable ? MF_ENABLED : MF_GRAYED)));
    }

    return false;
}


/**
 * Invert the check status of menu item
 * @param   hMenu              Handle to window menu
 * @param   id                 ID of menu item to check or uncheck
 * @param   previouslyChecked  Previous check status of menu item
 * @return  Indicates success or failure
 */
bool KinectWindow::InvertCheckMenuItem(HMENU hMenu, UINT id, bool previouslyChecked)
{
    return (-1 != CheckMenuItem(hMenu, id, MF_BYCOMMAND | (previouslyChecked ? MF_UNCHECKED : MF_CHECKED)));
}


/**
 * Check radio menu item
 * @param   id     ID of menu item to check
 * @param   start  Start ID of grouped radio items
 * @param   end    End ID of grouped radio items
 * @param   hMenu  Handle to menu
 * @returnb Indicate success or failure
 */
bool KinectWindow::CheckRadioItem(UINT id, UINT start, UINT end, HMENU hMenu)
{
    if (id >= start && id <= end)
    {
        return FALSE != CheckMenuRadioItem(hMenu, start, end, id, MF_BYCOMMAND);
    }

    return false;
}


/**
 * Retrieve check status of menu item
 * @param   hMenu    Handle to menu control
 * @param   id       Identifier of menu item
 * @param   checked  Retrieve the check status of menu item
 * @return  Indicates success or failure
 */
bool KinectWindow::GetMenuItemCheckStatus(HMENU hMenu, UINT id, bool& checked)
{
    MENUITEMINFOW mii = {0};
    mii.cbSize = sizeof(mii);
    mii.fMask  = MIIM_STATE;
    if (GetMenuItemInfoW(hMenu, id, FALSE, &mii))
    {
        checked = (mii.fState & MFS_CHECKED) != FALSE;
        return true;
    }

    return false;
}


/**
 * Handler function to process tab control events
 * @param   lParam  Additional message information
 */
void KinectWindow::OnNotify(LPARAM lParam)
{
    LPNMHDR pNMHDR = (LPNMHDR)lParam;

    // Check if command comes from tab control and selection has changed
    if (m_hWndTab == pNMHDR->hwndFrom && TCN_SELCHANGE == pNMHDR->code)
    {
        // Tab control selection changed
        int index = TabCtrl_GetCurSel(m_hWndTab);
        if (index < (int)m_tabbedViews.size())
        {
            // Hide current tabbed viewer
            if (m_pCurTabbedView)
                m_pCurTabbedView->HideView();

            // Set selected viewer as current and show it
            m_pCurTabbedView = m_tabbedViews[index];
            m_pCurTabbedView->ShowView();
        }
    }
}


/**
 * Handler function to process WM_CLOSE message
 * @param   hWnd    Handle to the window
 * @param   wParam  Command parameter
 */
void KinectWindow::OnClose(HWND hWnd, WPARAM wParam)
{
    // The reason of closing is from status changed
    if (CLOSING_FROM_STATUSCHANGED == wParam)
    {
        m_hWndParent = nullptr; // Don't need Kinect window send back message of quit in this case
    }

    // Stop stream event thread
    if (INVALID_HANDLE_VALUE != m_hStopStreamEventThread)
    {
        SetEvent(m_hStopStreamEventThread);
    }

    // Shut down the device
    if (nullptr != m_pNuiSensor)
    {
        m_pNuiSensor->NuiShutdown();
    }

    // Destroy the window
    if (nullptr != hWnd)
    {
        DestroyWindow(hWnd);
    }
}


/**
 * Calculates new positions and sizes for sub windows when Kinect window is resized
 * @param   priRect     New position and size for primary display viewer
 * @param   secRect     New position and size for secondary display viewer
 * @param   tabRect     New position and size for tab control
 * @param   tabbedRect  New position and size for tabbed viewers
 * @return  Indicates success or failure
 */
bool KinectWindow::CalculateViewRects(RECT& priRect, RECT& secRect, RECT& tabRect, RECT& tabbedRect)
{
    RECT client;
    if (m_hWnd && GetClientRect(m_hWnd, &client))
    {
        int clientWidth   = client.right  - client.left;
        int clientHeight  = client.bottom - client.top;

        int noClipWidth   = PRIMARY_VIEW_MIN_WIDTH + SECOND_VIEW_FIXED_WIDTH + GAP_BETWEEN_VIEWS;
        int noClipHeight  = PRIMARY_VIEW_MIN_HEIGHT;

        priRect.left      = client.left;
        priRect.top       = client.top;
        priRect.right     = clientWidth  > noClipWidth  ? (client.right - SECOND_VIEW_FIXED_WIDTH - GAP_BETWEEN_VIEWS) : (client.left + PRIMARY_VIEW_MIN_WIDTH);
        priRect.bottom    = clientHeight > noClipHeight ?  client.bottom : (client.top + PRIMARY_VIEW_MIN_HEIGHT);

        secRect.left      = priRect.right + GAP_BETWEEN_VIEWS;
        secRect.right     = secRect.left + SECOND_VIEW_FIXED_WIDTH;
        secRect.top       = client.top;
        secRect.bottom    = client.top + SECOND_VIEW_FIXED_HEIGHT;

        tabRect.left      = secRect.left;
        tabRect.right     = secRect.right;
        tabRect.top       = secRect.bottom + GAP_BETWEEN_VIEWS;
        tabRect.bottom    = tabRect.top + TAB_CONTROL_FIXED_HEIGHT;

        tabbedRect.left   = secRect.left;
        tabbedRect.right  = secRect.right;
        tabbedRect.top    = tabRect.bottom;
        tabbedRect.bottom = tabbedRect.top + TABBED_VIEW_FIXED_HEIGHT;

        return true;
    }

    return false;
}


/**
 * Process color, depth and skeleton streams
 */
void KinectWindow::UpdateStreams()
{
    m_pColorStream->ProcessStreamFrame();
    m_pDepthStream->ProcessStreamFrame();
    m_pSkeletonStream->ProcessStreamFrame();
}


/**
 * Process audio, accelerometer and tilt angle streams
 */
void KinectWindow::UpdateTimedStreams()
{
	// m_pSkeletonPointsStream->ProcessStream();
	/*
    m_pAudioStream->ProcessStream();
    m_pAccelerometerStream->ProcessStream();
	*/
}


/**
 * Thread to handle stream events
 * @param   pThis Pointer to Kinect window instance
 * @return  Exit result from thread
 */
DWORD KinectWindow::StreamEventThread(KinectWindow* pThis)
{
    HANDLE events[] = {pThis->m_hStopStreamEventThread, 
                       pThis->m_hTimer, 
                       pThis->m_pColorStream->GetFrameReadyEvent(), 
                       pThis->m_pDepthStream->GetFrameReadyEvent(), 
                       pThis->m_pSkeletonStream->GetFrameReadyEvent()};

    while (true)
    {
        DWORD ret = WaitForMultipleObjects(ARRAYSIZE(events), events, FALSE, INFINITE);

        if (WAIT_OBJECT_0 == ret)
            break;

        if (WAIT_OBJECT_0 + 1 == ret)
        {
            SendMessageW(pThis->GetWindow(), WM_TIMEREVENT, 0, 0);
        }
        else if(WAIT_OBJECT_0 + 4 >= ret)
        {
            SendMessageW(pThis->GetWindow(), WM_STREAMEVENT, 0, 0);
        }
    }

    return 0;
}
