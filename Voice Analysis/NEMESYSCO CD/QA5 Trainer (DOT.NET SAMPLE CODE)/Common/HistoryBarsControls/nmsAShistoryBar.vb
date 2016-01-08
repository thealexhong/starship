Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.CompilerServices
Imports VB6PictureControlEmu

Namespace HistoryBarsControls
    <DesignerGenerated> _
    Public Class nmsAShistoryBar
        Inherits UserControl
        ' Events
        Public Event BackColorChange As BackColorChangeEventHandler
        Public Event BorderStyleChange As BorderStyleChangeEventHandler
        Public Shadows Event Click As ClickEventHandler
        Public Event colorAngerChange As colorAngerChangeEventHandler
        Public Event colorStressChange As colorStressChangeEventHandler
        Public Event DblClick As DblClickEventHandler
        Public Event EnabledChange As EnabledChangeEventHandler
        Public Shadows Event MouseDown As MouseDownEventHandler
        Public Shadows Event MouseMove As MouseMoveEventHandler
        Public Shadows Event MouseUp As MouseUpEventHandler
        Public Event showNoSegmentsChange As showNoSegmentsChangeEventHandler

        Public Function addNewValues (ByVal cStress As Short, ByVal cAnger As Short) As Short
            Dim num As Short
            Dim num2 As Short
            If Not Me.DataSet Then
                Me.Reset()
            End If
            Me.dData (Me.dDataPointer, 0) = cStress
            Me.dData (Me.dDataPointer, 1) = cAnger

            PicView.Visible = False
            PicView.Cls()
            Dim num3 As Short = 0
            Dim num4 As Short = CShort ((Me.m_showNoSegments - 1))
            num2 = CShort ((Me.dDataPointer + 1))
            Do While (num2 <= num4)
                Me.PicView.Box (CSng (num3), 30.0!, CSng ((num3 + 1)), CSng ((30 - Me.dData (num2, 0))), m_colorStress)
                Me.PicView.Box (CSng (num3), 30.0!, CSng ((num3 + 1)), CSng ((30 - Me.dData (num2, 1))), m_colorAnger)
                num3 = CShort ((num3 + 1))
                num2 = CShort ((num2 + 1))
            Loop
            Dim dDataPointer As Short = Me.dDataPointer
            num2 = 0
            Do While (num2 <= dDataPointer)
                Me.PicView.Box (CSng (num3), 30.0!, CSng ((num3 + 1)), CSng ((30 - Me.dData (num2, 0))), m_colorStress)
                Me.PicView.Box (CSng (num3), 30.0!, CSng ((num3 + 1)), CSng ((30 - Me.dData (num2, 1))), m_colorAnger)
                num3 = CShort ((num3 + 1))
                num2 = CShort ((num2 + 1))
            Loop
            PicView.Visible = True
            Me.dDataPointer = CShort ((Me.dDataPointer + 1))
            If (Me.dDataPointer >= (Me.m_showNoSegments - 1)) Then
                Me.dDataPointer = 0
            End If
            Return num
        End Function

        <DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose (ByVal Disposing As Boolean)
            If (Disposing AndAlso (Not Me.components Is Nothing)) Then
                Me.components.Dispose()
            End If
            MyBase.Dispose (Disposing)
        End Sub

        <DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.PicView = New PictureBox
            Me.SuspendLayout()
            '
            'PicView
            '
            Me.PicView.BackColor = Color.Black
            Me.PicView.Dock = DockStyle.Fill
            Me.PicView.DrawWidth = 1
            Me.PicView.Location = New Point (0, 0)
            Me.PicView.Margin = New Padding (0)
            Me.PicView.Name = "PicView"
            Me.PicView.ScaleHeight = 150
            Me.PicView.ScaleLeft = 0
            Me.PicView.ScaleTop = 0
            Me.PicView.ScaleWidth = 150
            Me.PicView.Size = New Size (281, 49)
            Me.PicView.TabIndex = 0
            '
            'nmsAShistoryBar
            '
            Me.Controls.Add (Me.PicView)
            Me.Name = "nmsAShistoryBar"
            Me.Size = New Size (281, 49)
            Me.ResumeLayout (False)

        End Sub

        Private Sub nmsAShistoryBar_Resize (ByVal eventSender As Object, ByVal eventArgs As EventArgs)
            PicView.Left = 0
            PicView.Top = 0
            PicView.Width = Width
            PicView.Height = Height

            If m_showNoSegments <> 0 Then
                PicView.ScaleWidth = m_showNoSegments
            Else
                PicView.ScaleWidth = 30
            End If

            PicView.ScaleHeight = 30
        End Sub

        Private Sub PicView_Click (ByVal eventSender As Object, ByVal eventArgs As EventArgs)
            Dim clickEvent As ClickEventHandler = Me.ClickEvent
            If (Not clickEvent Is Nothing) Then
                clickEvent.Invoke (Me, Nothing)
            End If
        End Sub

        Private Sub PicView_DoubleClick (ByVal eventSender As Object, ByVal eventArgs As EventArgs)
            Dim dblClickEvent As DblClickEventHandler = Me.DblClickEvent
            If (Not dblClickEvent Is Nothing) Then
                dblClickEvent.Invoke (Me, Nothing)
            End If
        End Sub

        Private Sub PicView_MouseDown (ByVal eventSender As Object, ByVal eventArgs As MouseEventArgs)
            Dim button As Short = CShort ((eventArgs.Button/MouseButtons.Left))
            Dim shift As Short = CShort ((ModifierKeys/Keys.Shift))
            Dim mouseDownEvent As MouseDownEventHandler = Me.MouseDownEvent
            If (Not mouseDownEvent Is Nothing) Then
                Dim x As Single = eventArgs.X
                Dim y As Single = eventArgs.Y
                mouseDownEvent.Invoke (Me, New MouseDownEventArgs ((button), (shift), (x), (y)))
            End If
        End Sub

        Private Sub PicView_MouseMove (ByVal eventSender As Object, ByVal eventArgs As MouseEventArgs)
            Dim button As Short = CShort ((eventArgs.Button/MouseButtons.Left))
            Dim shift As Short = CShort ((ModifierKeys/Keys.Shift))
            Dim x As Single = eventArgs.X
            Dim y As Single = eventArgs.Y
            Dim mouseMoveEvent As MouseMoveEventHandler = Me.MouseMoveEvent
            If (Not mouseMoveEvent Is Nothing) Then
                mouseMoveEvent.Invoke (Me, New MouseMoveEventArgs ((button), (shift), (x), (y)))
            End If
        End Sub

        Private Sub PicView_MouseUp (ByVal eventSender As Object, ByVal eventArgs As MouseEventArgs)
            Dim button As Short = CShort ((eventArgs.Button/MouseButtons.Left))
            Dim shift As Short = CShort ((ModifierKeys/Keys.Shift))
            Dim x As Single = eventArgs.X
            Dim y As Single = eventArgs.Y
            Dim mouseUpEvent As MouseUpEventHandler = Me.MouseUpEvent
            If (Not mouseUpEvent Is Nothing) Then
                mouseUpEvent.Invoke (Me, New MouseUpEventArgs ((button), (shift), (x), (y)))
            End If
        End Sub

        Public Function Reset() As Short
            Dim num As Short
            Me.dData = New Short((Me.m_showNoSegments + 1) - 1, 2 - 1) {}
            Me.dDataPointer = 0
            Me.PicView.Cls()
            Me.PicView.ScaleWidth = Me.m_showNoSegments
            Me.PicView.ScaleHeight = 30
            Me.DataSet = True
            Return num
        End Function


        ' Properties
        Public Overrides Property BackColor() As Color
            Get
                If Me.PicView Is Nothing Then
                    Return Color.Black
                End If

                Return Me.PicView.BackColor
            End Get
            Set (ByVal Value As Color)
                Me.PicView.BackColor = Value
                Dim backColorChangeEvent As BackColorChangeEventHandler = Me.BackColorChangeEvent
                If (Not backColorChangeEvent Is Nothing) Then
                    backColorChangeEvent.Invoke()
                End If
            End Set
        End Property

        Public Property BorderType() As BorderStyle
            Get
                Return CShort (Me.PicView.BorderStyle)
            End Get
            Set (ByVal Value As BorderStyle)
                Me.PicView.BorderStyle = Value
                Dim borderStyleChangeEvent As BorderStyleChangeEventHandler = Me.BorderStyleChangeEvent
                If (Not borderStyleChangeEvent Is Nothing) Then
                    borderStyleChangeEvent.Invoke()
                End If
            End Set
        End Property

        Public Property colorAnger() As Color
            Get
                Return Me.m_colorAnger
            End Get
            Set (ByVal Value As Color)
                Me.m_colorAnger = Value
                Dim colorAngerChangeEvent As colorAngerChangeEventHandler = Me.colorAngerChangeEvent
                If (Not colorAngerChangeEvent Is Nothing) Then
                    colorAngerChangeEvent.Invoke()
                End If
            End Set
        End Property

        Public Property colorStress() As Color
            Get
                Return Me.m_colorStress
            End Get
            Set (ByVal Value As Color)
                Me.m_colorStress = Value
                Dim colorStressChangeEvent As colorStressChangeEventHandler = Me.colorStressChangeEvent
                If (Not colorStressChangeEvent Is Nothing) Then
                    colorStressChangeEvent.Invoke()
                End If
            End Set
        End Property

        Public Overloads Property Enabled() As Boolean
            Get
                Return MyBase.Enabled
            End Get
            Set (ByVal Value As Boolean)
                MyBase.Enabled = Value
                Dim enabledChangeEvent As EnabledChangeEventHandler = Me.EnabledChangeEvent
                If (Not enabledChangeEvent Is Nothing) Then
                    enabledChangeEvent.Invoke()
                End If
            End Set
        End Property

        Public Property showNoSegments() As Short
            Get
                Return Me.m_showNoSegments
            End Get
            Set (ByVal Value As Short)
                If ((Value >= 10) Or (Value <= 200)) Then
                    Me.m_showNoSegments = Value
                    Dim showNoSegmentsChangeEvent As showNoSegmentsChangeEventHandler = Me.showNoSegmentsChangeEvent
                    If (Not showNoSegmentsChangeEvent Is Nothing) Then
                        showNoSegmentsChangeEvent.Invoke()
                    End If
                    Me.dData = New Short((Me.m_showNoSegments + 1) - 1, 2 - 1) {}
                    Me.dDataPointer = 0
                    Me.PicView.Cls()
                    Me.DataSet = True
                End If
            End Set
        End Property

        ' Fields
        '<AccessedThroughProperty("PicView")> 
        Private WithEvents PicView As PictureBox
        Private components As IContainer
        Private DataSet As Boolean
        Private dData(,) As Short
        Private dDataPointer As Short
        Private m_colorAnger As Color
        Private m_colorStress As Color
        Private Const m_def_colorAnger As Integer = &HFF
        Private Const m_def_colorStress As Integer = &HFFFF
        Private Const m_def_showNoSegments As Short = 100
        Private m_showNoSegments As Short

        ' Nested Types
        Public Delegate Sub BackColorChangeEventHandler()

        Public Delegate Sub BorderStyleChangeEventHandler()

        Public Delegate Sub ClickEventHandler (ByVal Sender As Object, ByVal e As EventArgs)

        Public Delegate Sub colorAngerChangeEventHandler()

        Public Delegate Sub colorStressChangeEventHandler()

        Public Delegate Sub DblClickEventHandler (ByVal Sender As Object, ByVal e As EventArgs)

        Public Delegate Sub EnabledChangeEventHandler()

        <ProgId ("MouseDownEventArgs_NET.MouseDownEventArgs")> _
        Public NotInheritable Class MouseDownEventArgs
            Inherits EventArgs
            ' Methods
            Public Sub New (ByRef Button As Short, ByRef Shift As Short, ByRef X As Single, ByRef Y As Single)
                Me.Button = Button
                Me.Shift = Shift
                Me.X = X
                Me.Y = Y
            End Sub


            ' Fields
            Public Button As Short
            Public Shift As Short
            Public X As Single
            Public Y As Single
        End Class

        Public Delegate Sub MouseDownEventHandler (ByVal Sender As Object, ByVal e As MouseDownEventArgs)

        <ProgId ("MouseMoveEventArgs_NET.MouseMoveEventArgs")> _
        Public NotInheritable Class MouseMoveEventArgs
            Inherits EventArgs
            ' Methods
            Public Sub New (ByRef Button As Short, ByRef Shift As Short, ByRef X As Single, ByRef Y As Single)
                Me.Button = Button
                Me.Shift = Shift
                Me.X = X
                Me.Y = Y
            End Sub


            ' Fields
            Public Button As Short
            Public Shift As Short
            Public X As Single
            Public Y As Single
        End Class

        Public Delegate Sub MouseMoveEventHandler (ByVal Sender As Object, ByVal e As MouseMoveEventArgs)

        <ProgId ("MouseUpEventArgs_NET.MouseUpEventArgs")> _
        Public NotInheritable Class MouseUpEventArgs
            Inherits EventArgs
            ' Methods
            Public Sub New (ByRef Button As Short, ByRef Shift As Short, ByRef X As Single, ByRef Y As Single)
                Me.Button = Button
                Me.Shift = Shift
                Me.X = X
                Me.Y = Y
            End Sub


            ' Fields
            Public Button As Short
            Public Shift As Short
            Public X As Single
            Public Y As Single
        End Class

        Public Delegate Sub MouseUpEventHandler (ByVal Sender As Object, ByVal e As MouseUpEventArgs)

        Public Delegate Sub showNoSegmentsChangeEventHandler()
    End Class
End Namespace

