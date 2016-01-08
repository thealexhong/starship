Imports System.Runtime.CompilerServices
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.CompilerServices
Imports VB6PictureControlEmu

Namespace HistoryBarsControls
    <DesignerGenerated> _
    Public Class nmsHSAhistoryBar
        Inherits UserControl
        ' Events
        Public Event BackColorChange As BackColorChangeEventHandler
        Public Event BorderStyleChange As BorderStyleChangeEventHandler
        Public Shadows Event Click As ClickEventHandler
        Public Event colorAtmosChange As colorAtmosChangeEventHandler
        Public Event colorContentChange As colorContentChangeEventHandler
        Public Event colorUpsetChange As colorUpsetChangeEventHandler
        Public Event DblClick As DblClickEventHandler
        Public Event EnabledChange As EnabledChangeEventHandler
        Public Shadows Event MouseDown As MouseDownEventHandler
        Public Shadows Event MouseMove As MouseMoveEventHandler
        Public Shadows Event MouseUp As MouseUpEventHandler
        Public Event showNoSegmentsChange As showNoSegmentsChangeEventHandler

        ' Methods
        <DebuggerNonUserCode> _
        Public Sub New()
            AddHandler MyBase.Resize, New EventHandler (AddressOf Me.nmsHSAhistoryBar_Resize)
            Me.InitializeComponent()
        End Sub

        Public Function addNewValues (ByVal cContent As Short, ByVal cUpset As Short, ByVal cAtmos As Short) As Short
            Dim num As Short
            Dim num2 As Short
            Dim num8 As Short
            If Not Me.DataSet Then
                Me.Reset()
            End If
            Me.dData (Me.dDataPointer, 0) = cContent
            Me.dData (Me.dDataPointer, 1) = cUpset
            Me.dData (Me.dDataPointer, 2) = CShort (Math.Round (CDbl ((CDbl (cAtmos)/3))))
            PicView.Visible = False
            PicView.Cls()
            Dim num3 As Short = 1
            Dim num4 As Short = 1
            Dim num5 As Short = CShort ((Me.m_showNoSegments - 1))
            num2 = CShort ((Me.dDataPointer + 1))
            Do While (num2 <= num5)
                PicView.ForeColor = (Me.m_colorContent)
                PicView.Box (CSng (num3), 30.0!, CSng ((num3 + 1)), CSng ((30 - Me.dData (num2, 0))))
                PicView.ForeColor = (Me.m_colorUpset)
                PicView.Box (CSng (num3), 30.0!, CSng ((num3 + 1)), CSng ((30 + Me.dData (num2, 1))))
                num3 = CShort ((num3 + 1))
                num2 = CShort ((num2 + 1))
            Loop
            PicView.ForeColor = (Me.m_colorAtmos)
            PicView.MoveTo (CDbl (num4), CDbl ((30 - Me.dData ((Me.dDataPointer), 2))))
            num2 = CShort ((Me.dDataPointer))
            Label_0196:
            num8 = &H63
            If (num2 <= num8) Then
                PicView.LineTo (CSng (num4), CSng ((30 - Me.dData (num2, 2))))
                num4 = CShort ((num4 + 1))
                num2 = CShort ((num2 + 1))
                GoTo Label_0196
            End If
            num3 = CShort ((num3 - 1))
            Dim dDataPointer As Short = Me.dDataPointer
            num2 = 0
            Do While (num2 <= dDataPointer)
                PicView.ForeColor = (Me.m_colorContent)
                PicView.Box (CSng (num3), 30.0!, CSng ((num3 + 1)), CSng ((30 - Me.dData (num2, 0))))
                PicView.ForeColor = (Me.m_colorUpset)
                PicView.Box (CSng (num3), 30.0!, CSng ((num3 + 1)), CSng ((30 + Me.dData (num2, 1))))
                num3 = CShort ((num3 + 1))
                num2 = CShort ((num2 + 1))
            Loop
            num4 = CShort ((num4 - 1))
            PicView.ForeColor = (Me.m_colorAtmos)
            PicView.MoveTo (CDbl (num4), CDbl ((30 - Me.dData (0, 2))))
            Dim num7 As Short = Me.dDataPointer
            num2 = 0
            Do While (num2 <= num7)
                PicView.LineTo (CSng (num4), CSng ((30 - Me.dData (num2, 2))))
                num4 = CShort ((num4 + 1))
                num2 = CShort ((num2 + 1))
            Loop
            PicView.Visible = True
            Me.dDataPointer = CShort ((Me.dDataPointer + 1))
            If (Me.dDataPointer >= Me.m_showNoSegments) Then
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
            Me.PicView.Size = New Size (353, 45)
            Me.PicView.TabIndex = 0
            '
            'nmsHSAhistoryBar
            '
            Me.Controls.Add (Me.PicView)
            Me.Name = "nmsHSAhistoryBar"
            Me.Size = New Size (353, 45)
            Me.ResumeLayout (False)

        End Sub

        Private Sub nmsHSAhistoryBar_Resize (ByVal eventSender As Object, ByVal eventArgs As EventArgs)
            Me.PicView.Left = 0
            Me.PicView.Top = 0
            Me.PicView.Width = MyBase.Width
            Me.PicView.Height = MyBase.Height

            If Me.m_showNoSegments = 0 Then
                Me.PicView.ScaleWidth = 60
            Else
                Me.PicView.ScaleWidth = Me.m_showNoSegments
            End If
            Me.PicView.ScaleWidth = Me.m_showNoSegments

            Me.PicView.ScaleHeight = 60
            Me.PicView.DrawWidth = 2
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
            Dim x As Single = eventArgs.X
            Dim y As Single = eventArgs.Y
            Dim mouseDownEvent As MouseDownEventHandler = Me.MouseDownEvent
            If (Not mouseDownEvent Is Nothing) Then
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
            Me.dData = New Short((Me.m_showNoSegments + 2), 3) {}
            Me.dDataPointer = 0
            Me.PicView.Cls()
            Me.PicView.ScaleWidth = Me.m_showNoSegments
            Me.DataSet = True
            Return num
        End Function


        ' Properties
        Public Overrides Property BackColor() As Color
            Get
                Return Color.Black
' Me.PicView.BackColor
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

        Public Property colorAtmos() As Color
            Get
                Return Me.m_colorAtmos
            End Get
            Set (ByVal Value As Color)
                Me.m_colorAtmos = Value
                Dim colorAtmosChangeEvent As colorAtmosChangeEventHandler = Me.colorAtmosChangeEvent
                If (Not colorAtmosChangeEvent Is Nothing) Then
                    colorAtmosChangeEvent.Invoke()
                End If
            End Set
        End Property

        Public Property colorContent() As Color
            Get
                Return Me.m_colorContent
            End Get
            Set (ByVal Value As Color)
                Me.m_colorContent = Value
                Dim colorContentChangeEvent As colorContentChangeEventHandler = Me.colorContentChangeEvent
                If (Not colorContentChangeEvent Is Nothing) Then
                    colorContentChangeEvent.Invoke()
                End If
            End Set
        End Property

        Public Property colorUpset() As Color
            Get
                Return Me.m_colorUpset
            End Get
            Set (ByVal Value As Color)
                Me.m_colorUpset = Value
                Dim colorUpsetChangeEvent As colorUpsetChangeEventHandler = Me.colorUpsetChangeEvent
                If (Not colorUpsetChangeEvent Is Nothing) Then
                    colorUpsetChangeEvent.Invoke()
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

        Public Property NumberOfSegmentsToShow() As Short
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
                    Me.dData = New Short((Me.m_showNoSegments + 1) - 1, 3 - 1) {}
                    Me.dDataPointer = 0
                    Me.PicView.Cls()
                    Me.DataSet = True
                End If
            End Set
        End Property

        ' Fields
        <AccessedThroughProperty ("PicView")> Friend WithEvents PicView As PictureBox
        Private components As IContainer
        Private DataSet As Boolean
        Private dData(,) As Short
        Private dDataPointer As Short
        Private m_colorAtmos As Color
        Private m_colorContent As Color
        Private m_colorUpset As Color
        Private Const m_def_colorAtmos As Integer = &HFFFFFF
        Private Const m_def_colorContent As Integer = &HFF00
        Private Const m_def_colorUpset As Integer = &H80FF
        Private Const m_def_showNoSegments As Short = 100
        Private m_showNoSegments As Short

        ' Nested Types
        Public Delegate Sub BackColorChangeEventHandler()

        Public Delegate Sub BorderStyleChangeEventHandler()

        Public Delegate Sub ClickEventHandler (ByVal Sender As Object, ByVal e As EventArgs)

        Public Delegate Sub colorAtmosChangeEventHandler()

        Public Delegate Sub colorContentChangeEventHandler()

        Public Delegate Sub colorUpsetChangeEventHandler()

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

