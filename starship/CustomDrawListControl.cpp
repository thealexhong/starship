//------------------------------------------------------------------------------
// <copyright file="CustomDrawListControl.cpp" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <commctrl.h>
#include <time.h>

#define __No__NuiSensor_h__
#define __No__NuiSkeleton_h__
#include <NuiApi.h>

#include "CustomDrawListControl.h"
#include "Utility.h"
#include "resource.h"

/// <summary>
/// Retrieve the string text of the corresponding sensor status
/// </summary>
PCWSTR GetStringFromSensorStatus(HRESULT sensorStatus)
{
    switch (sensorStatus)
    {
    case S_OK:
        return L"Connected";
    case S_NUI_INITIALIZING:
        return L"Initializing";
    case E_NUI_NOTCONNECTED:
        return L"Disconnected";
    case E_NUI_NOTREADY:
        return L"NotReady";
    case E_NUI_NOTPOWERED:
        return L"NotPowered";
    case E_NUI_NOTGENUINE:
        return L"NotGenuine";
    case E_NUI_NOTSUPPORTED:
        return L"NotSupported";
    case E_NUI_INSUFFICIENTBANDWIDTH:
        return L"InsufficientBandwidth";
    default:
        ;
    }

    return L"Error";
}

/// <summary>
/// Helper function to retrieve the local current time
/// </summary>
inline PWCHAR GetLocalCurrentTime(PWCHAR buffer, UINT length)
{
    __time64_t currentTime = _time64(nullptr);
    tm localTime;
    _localtime64_s(&localTime, &currentTime);

    wcsftime(buffer, length, L"%#I:%M %p", &localTime);

    return buffer;
}

/// <summary>
/// Helper function to create a list view item
/// </summary>
inline LVITEM CreateListViewItem(int rowIndex, int cellIndex, PCWSTR text)
{
    LVITEM lvitem = {0};

    lvitem.mask = LVIF_IMAGE | LVIF_TEXT;
    lvitem.iItem = rowIndex;
    lvitem.iSubItem = cellIndex;
    lvitem.pszText = const_cast<LPWSTR>(text);

    return lvitem;
}

// ---------------------------------------------------------------------------
//
// Class KElistControl
//
// ---------------------------------------------------------------------------

HFONT CustomDrawListControl::DefaultFont;

/// <summary>
/// Constructor
/// </summary>
CustomDrawListControl::CustomDrawListControl(HWND hWnd, int columns)
    : m_hWnd(hWnd)
    , m_nColumns(columns)
{
    EnsureFontCreated(DefaultFont, 15, FW_NORMAL);

    // Set list control style
    ListView_SetExtendedListViewStyle(m_hWnd, LVS_EX_FULLROWSELECT);

    // Insert columns
    for (int i = 0; i < m_nColumns; ++i)
    {
        LVCOLUMN lvc = {0};
        ListView_InsertColumn(m_hWnd, i, (LPARAM)&lvc);
    }
}

/// <summary>
/// Process the WM_NOTIFY message from the parent window
/// </summary>
/// <param name="hParentWnd">Handle to the parent window.</param>
/// <param name="pNMLV">Information about the list-view notification message.</param>
LRESULT CustomDrawListControl::HandleNotifyMessage(HWND hParentWnd, LPNMLISTVIEW pNMLV)
{
    switch (pNMLV->hdr.code)
    {
    case NM_CUSTOMDRAW:
        {
            LRESULT result = OnCustomDraw((LPNMLVCUSTOMDRAW)pNMLV);
            SetWindowLongPtr(hParentWnd, DWLP_MSGRESULT, result);
        }
        break;

    case LVN_ITEMCHANGING:
        {
            // Don't highlight when the item is selected
            if ((pNMLV->uChanged) & LVIF_STATE)
            {
                SetWindowLongPtr(hParentWnd, DWLP_MSGRESULT, TRUE);
            }
        }
        break;
    }

    return TRUE;
}

/// <summary>
/// Process the WM_SIZE message
/// </summary>
void CustomDrawListControl::HandleResizeMessage()
{
    OnResize(GetClientSize(m_hWnd).cx);
}

/// <summary>
/// Insert or update a row by the given cell values.
/// </summary>
bool CustomDrawListControl::InsertOrUpdateRow(PCWSTR cells[], int length, bool isDirectlyInsert)
{
    assert(length >= 1);
    assert(length == m_nColumns);

    bool hasInserted = false;

    int index = GetRowIndex(cells[0]);
    if (index < 0 || isDirectlyInsert)
    {
        // If the row does not exist, create a new one
        index = AddNewRow();

        hasInserted = true;
    }

    for (int i = 0; i < length; ++i)
    {
        LVITEM lvitem = CreateListViewItem(index, i, cells[i]);
        ListView_SetItem(m_hWnd, &lvitem);
    }

    return hasInserted;
}

/// <summary>
/// Add a new row at the end of the list control, and return its index
/// </summary>
UINT CustomDrawListControl::AddNewRow()
{
    // Get the count of rows in the list control as the new row index
    int nextRowIndex = ListView_GetItemCount(m_hWnd);

    LVITEM lvitem = CreateListViewItem(nextRowIndex, 0, nullptr);

    // Insert a new row to the list control
    ListView_InsertItem(m_hWnd, (LPARAM)&lvitem);

    return nextRowIndex;
}

/// <summary>
/// Remove the first row which contains the specified text in its cells.
/// </summary>
bool CustomDrawListControl::RemoveRow(PCWSTR rowId)
{
    return TRUE == ListView_DeleteItem(m_hWnd, GetRowIndex(rowId));
}

/// <summary>
/// Get the first row index which contains the specified text in its cells.
/// </summary
int CustomDrawListControl::GetRowIndex(PCWSTR rowId)
{
    LVFINDINFO lvf = {0};

    lvf.flags = LVFI_STRING;
    lvf.psz = rowId;

    // Find the item starting from the beginning
    return ListView_FindItem(m_hWnd, -1, (LPARAM)&lvf);
}

/// <summary>
/// Rearrange the columns width of the list control.
/// </summary>
void CustomDrawListControl::ResizeColumns(const UINT columnWidths[], int length)
{
    assert(length == m_nColumns);

    for (int i = 0; i < length; ++i)
    {
        ListView_SetColumnWidth(m_hWnd, i, columnWidths[i]);
    }
}

/// <summary>
/// Handle the NM_CUSTOMDRAW message to implement custom draw.
/// </summary>
/// <param name="pNMLVCD">Information specific to an NM_CUSTOMDRAW notification code.</param>
LRESULT CustomDrawListControl::OnCustomDraw(LPNMLVCUSTOMDRAW pNMLVCD)
{
    switch(pNMLVCD->nmcd.dwDrawStage)
    {
    case CDDS_PREPAINT:
        return CDRF_NOTIFYITEMDRAW;

    case CDDS_ITEMPREPAINT:
        return CDRF_NOTIFYSUBITEMDRAW;

    case CDDS_SUBITEM | CDDS_ITEMPREPAINT:
        return OnCustomDrawCell(pNMLVCD);
    }

    return CDRF_DODEFAULT;
}

// ---------------------------------------------------------------------------
//
// Class SensorListControl
//
// ---------------------------------------------------------------------------

HFONT SensorListControl::StatusLargeFont;
HBITMAP SensorListControl::SensorPicture;

SensorListControl::SensorListControl(HWND hWnd)
    : CustomDrawListControl(hWnd, Columns)
{
    EnsureImageLoaded(SensorPicture, IDB_KINECTSENSORPICTURE);
    EnsureFontCreated(StatusLargeFont, 25, FW_MEDIUM);

    ListView_SetExtendedListViewStyle(m_hWnd, ListView_GetExtendedListViewStyle(m_hWnd) | LVS_EX_SUBITEMIMAGES);

    HIMAGELIST hImgList = ImageList_Create(SensorListImageWidth, SensorListRowHeight, ILC_COLORDDB, 1, 1);
    ListView_SetImageList(m_hWnd, hImgList, LVSIL_SMALL);
}

void SensorListControl::InsertOrUpdateSensorStatus(PCWSTR instanceId, HRESULT sensorStatus)
{
    PCWSTR cells[Columns];

    cells[DetailColumnIndex] = instanceId;
    cells[ImageColumnIndex] = GetStringFromSensorStatus(sensorStatus);

    // Update data and control height
    if (InsertOrUpdateRow(cells, Columns))
    {
        UpdateHeightByRow(true);
    }
}

void SensorListControl::RemoveRow(PCWSTR instanceId)
{
    // Update data and control height
    if (CustomDrawListControl::RemoveRow(instanceId))
    {
        UpdateHeightByRow(false);
    }
}

/// <summary>
/// Handle the WM_RESIZE message to adjust the width of each column automatically
/// </summary>
void SensorListControl::OnResize(LONG totalWidth)
{
    UINT widthValues[Columns];

    widthValues[DetailColumnIndex] = max(totalWidth - SensorListImageWidth, 0);
    widthValues[ImageColumnIndex] = min(totalWidth, SensorListImageWidth);

    ResizeColumns(widthValues, Columns);
}

/// <summary>
/// Custom draw column0 as combination of list control column0 and column1,
/// and custom draw column1 as a sensor picture
/// </summary>
LRESULT SensorListControl::OnCustomDrawCell(LPNMLVCUSTOMDRAW pContext)
{
    RECT rect = pContext->nmcd.rc;
    HDC hdc = pContext->nmcd.hdc;
    int rowIndex = (int)pContext->nmcd.dwItemSpec;
    int columnIndex = pContext->iSubItem;

    switch (columnIndex)
    {
    case DetailColumnIndex:
        {
            FillRect(hdc, &rect, (HBRUSH)GetStockObject(WHITE_BRUSH));

            int fontHeight = CustomDrawText(hdc, rowIndex, ImageColumnIndex, StatusLargeFont, rect);
            OffsetRect(&rect, 0, fontHeight);
            CustomDrawText(hdc, rowIndex, DetailColumnIndex, DefaultFont, rect);
        }
        break;

    case ImageColumnIndex:
        {
            HDC memDC = CreateCompatibleDC(hdc);

            SelectObject(memDC, SensorPicture);
            BitBlt(hdc, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top, memDC, 0, 0, SRCCOPY);

            DeleteDC(memDC);
        }
        break;
    }

    return CDRF_SKIPDEFAULT;
}

int SensorListControl::CustomDrawText(HDC hdc, int rowIndex, int columnIndex, HFONT hFont, RECT& rect)
{
    WCHAR text[MaxTextLength] = {0};
    ListView_GetItemText(m_hWnd, rowIndex, columnIndex, text, _countof(text));

    SelectObject(hdc, hFont);

    return DrawTextW(hdc, text, (int)wcsnlen_s(text, MaxTextLength), &rect, DT_LEFT | DT_SINGLELINE | DT_NOPREFIX);
}

/// <summary>
/// Update the height of the list control
/// </summary>
/// <param name="isIncreased">If true, increase a row height; otherwise, decrease a row height.</param>
void SensorListControl::UpdateHeightByRow(bool isIncreased)
{
    SIZE currentSize = GetClientSize(m_hWnd);
    int changedHeight = (true == isIncreased) ? SensorListRowHeight : -SensorListRowHeight;

    SetWindowPos(m_hWnd, 0, 0, 0, currentSize.cx, currentSize.cy + changedHeight, SWP_NOMOVE);
}

// ---------------------------------------------------------------------------
//
// Class StatusLogControl
//
// ---------------------------------------------------------------------------

HFONT StatusLogListControl::StatusBoldFont;

StatusLogListControl::StatusLogListControl(HWND hWnd)
    : CustomDrawListControl(hWnd, Columns)
{
    EnsureFontCreated(StatusBoldFont, 15, FW_BOLD);
    ShowScrollBar(m_hWnd, SB_HORZ, FALSE);
}

void StatusLogListControl::AddLog(PCWSTR instanceId, HRESULT sensorStatus)
{
    PCWSTR cells[Columns];

    cells[InstanceIdCellIndex] = instanceId;
    cells[SensorStatusCellIndex] = GetStringFromSensorStatus(sensorStatus);

    WCHAR timeText[MaxTextLength] = {0};
    cells[LogTimeCellIndex] = GetLocalCurrentTime(timeText, _countof(timeText));

    InsertOrUpdateRow(cells, Columns, true);
}

/// <summary>
/// Handle the WM_RESIZE message to adjust the width of each column automatically
/// </summary>
void StatusLogListControl::OnResize(LONG totalWidth)
{
    UINT widthValues[Columns];

    widthValues[InstanceIdCellIndex] = max(totalWidth - StatusLogListStatusWidth - StatusLogListTimeWidth, 0);
    widthValues[SensorStatusCellIndex] = min(totalWidth, StatusLogListStatusWidth);
    widthValues[LogTimeCellIndex] = totalWidth - widthValues[0] - widthValues[1] - GetSystemMetrics(SM_CXVSCROLL);

    ResizeColumns(widthValues, Columns);
}

/// <summary>
/// Assign a font style to each text column
/// </summary>
LRESULT StatusLogListControl::OnCustomDrawCell(LPNMLVCUSTOMDRAW pContext)
{
    HFONT selectedFont = (pContext->iSubItem == SensorStatusCellIndex) ? StatusBoldFont : DefaultFont;

    SelectObject(pContext->nmcd.hdc, selectedFont);

    return CDRF_DODEFAULT;
}