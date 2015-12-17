//------------------------------------------------------------------------------
// <copyright file="CustomDrawListControl.h" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include <Windows.h>

/// <summary>
/// The base class for an owner draw list view. The first column of the list view is an unique key column.
/// Its subclasses implement their own custom draw methods and resize methods.
/// </summary>
class CustomDrawListControl
{
public:

    /// <summary>
    /// Process the WM_NOTIFY message from the parent window
    /// </summary>
    /// <param name="hParentWnd">Handle to the parent window.</param>
    /// <param name="pNMLV">Information about the list-view notification message.</param>
    LRESULT HandleNotifyMessage(HWND hParentWnd, LPNMLISTVIEW pNMLV);

    /// <summary>
    /// Process the WM_SIZE message
    /// </summary>
    void HandleResizeMessage();

protected:

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hWnd">Handle to the binded list control.</param>
    /// <param name="columnCount">Count of columns in the binded list control.</param>
    CustomDrawListControl(HWND hWnd, int columns);

    /// <summary>
    /// Insert or update a row by the given cell values.
    /// </summary>
    /// <param name="cells">The cells value array.</param>
    /// <param name="length">Element count of the value array.</param>
    /// <param name="isDirectlyInsert">If true, directly insert a new row.</param>
    bool    InsertOrUpdateRow(PCWSTR cells[], int length, bool isDirectlyInsert = false);

    /// <summary>
    /// Add a new row at the end of the list control, and return its index
    /// </summary>
    UINT    AddNewRow();

    /// <summary>
    /// Remove the specified row
    /// </summary>
    /// <param name="keyColumnText">The key column text of the specified row.</param>
    bool    RemoveRow(PCWSTR cellText);

    /// <summary>
    /// Get the first row index which contains the specified text in its cells
    /// </summary>
    int     GetRowIndex(PCWSTR cellText);

    /// <summary>
    /// Rearrange the columns width of the list control.
    /// </summary>
    void    ResizeColumns(const UINT columnWidths[], int length);

    /// <summary>
    /// Handle the NM_CUSTOMDRAW message to implement custom draw
    /// </summary>
    /// <param name="pNMLVCD">Information specific to an NM_CUSTOMDRAW notification code.</param>
    LRESULT OnCustomDraw(LPNMLVCUSTOMDRAW pNMLVCD);

    /// <summary>
    /// Custom draw the cell, must be overwrited by subclass
    /// </summary>
    virtual LRESULT OnCustomDrawCell(LPNMLVCUSTOMDRAW pContext) = 0;

    /// <summary>
    /// Reset the column width based on the given total width
    /// </summary>
    virtual void OnResize(LONG totalWidth) = 0;

protected:

    /// Window handle of the binded list control
    HWND   m_hWnd;

    /// Count of columns
    int    m_nColumns;

    static HFONT DefaultFont;

    /// Note that although the list-view control allows any length string to
    /// be stored as item text, only the first 260 TCHARs are displayed.
    static const UINT MaxTextLength = MAX_PATH;
};

/// <summary>
/// The sensor list control contains two columns,
/// and column0 displays the text while column1 displays a logo picture
/// </summary>
class SensorListControl : public CustomDrawListControl
{
public:

    SensorListControl(HWND hWnd);

    void InsertOrUpdateSensorStatus(PCWSTR instanceId, HRESULT sensorStatus);

    void RemoveRow(PCWSTR instanceId);

protected:

    /// <summary>
    /// Implement the custom draw operation. Note, both he sensor instance id and
    /// sensor status will be custom drown on the same column of the list control.
    /// </summary>
    LRESULT OnCustomDrawCell(LPNMLVCUSTOMDRAW pContext);

    void OnResize(LONG totalWidth);

private:

    int CustomDrawText(HDC hdc, int rowIndex, int cellIndex, HFONT hFont, RECT& rect);

    /// <summary>
    /// Update the height of the list control
    /// </summary>
    /// <param name="isIncreased">If true, increase a row height; otherwise, decrease a row height.</param>
    void UpdateHeightByRow(bool isIncreased);

private:

    static HBITMAP SensorPicture;
    static HFONT StatusLargeFont;

    // Define size parameters of sensor list control
    static const int SensorListImageWidth = 100;
    static const int SensorListRowHeight = 55;

    static const int Columns = 2;
    static const int DetailColumnIndex = 0;
    static const int ImageColumnIndex = 1;
};

/// <summary>
/// Its corresponding list control contains three columns
/// </summary>
class StatusLogListControl : public CustomDrawListControl
{
public:

    StatusLogListControl(HWND hWnd);

    void AddLog(PCWSTR instanceId, HRESULT sensorStatus);

protected:

    /// <summary>
    /// Implement the custom draw operation. It will set a font for each text column.
    /// </summary>
    LRESULT OnCustomDrawCell(LPNMLVCUSTOMDRAW pContext);

    void OnResize(LONG totalWidth);

private:

    static HFONT StatusBoldFont;

    // The width for the status column in the log list control
    static const int StatusLogListStatusWidth = 150;
    // The width for the tiem column in the log list control
    static const int StatusLogListTimeWidth = 100;

    static const int Columns = 3;
    static const int InstanceIdCellIndex= 0;
    static const int SensorStatusCellIndex = 1;
    static const int LogTimeCellIndex = 2;
};
