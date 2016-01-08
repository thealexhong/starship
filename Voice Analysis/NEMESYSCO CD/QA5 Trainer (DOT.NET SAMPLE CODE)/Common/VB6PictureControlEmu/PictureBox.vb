Imports System.Drawing.Drawing2D

Public Class PictureBox
    Dim formBitmap As Bitmap
    Dim formGraphics As Graphics
    Dim lastX As Single = 0
    Dim lastY As Single = 0
    Dim virtualX As Double = Width
    Dim virtualY As Double = Height
    Dim scaleX As Double
    Dim scaleY As Double
    Dim scaleT As Double = 0
    Dim scaleL As Double = 0
    Dim drawW As Integer = 1

    Private Sub PictureBox_Load (ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
    End Sub

    Public Sub Cls()
        formGraphics.Clear (Me.BackColor)
    End Sub

    Public Sub MoveTo (ByVal X As Double, ByVal Y As Double)
        lastX = X
        lastY = Y
    End Sub

    Public Property DrawWidth()
        Get
            DrawWidth = drawW
        End Get
        Set (ByVal value)
            drawW = value
        End Set
    End Property

    Public Sub Circle (ByVal centerX As Double, ByVal centerY As Double, ByVal radius As Double, _
                       ByVal ParamArray params() As Object)
        Dim drawColor As Color = Me.ForeColor
        If params.Length <> 0 Then
            drawColor = params (0)
        End If

        centerX = centerX*scaleX - scaleL*scaleX
        centerY = centerY*scaleY - scaleT*scaleY
        Dim radiusScaled As Double = radius
'* Math.Max(scaleX, scaleY)

        Dim l As Double = (centerX - radiusScaled)
        Dim t As Double = (centerY - radiusScaled)
        Dim w As Double = (radiusScaled*2)
        Dim h As Double = (radiusScaled*2)

        Dim rect As RectangleF = New RectangleF (l, t, w, h)

        formGraphics.DrawEllipse (New Pen (drawColor, drawW), rect)

        Me.BackgroundImage = formBitmap
    End Sub

    Private Sub PictureBox_Resize (ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Resize
        Dim c As Control = sender

        If formBitmap Is Nothing Then
            formBitmap = New Bitmap (c.Width, c.Height)
            formGraphics = Graphics.FromImage (formBitmap)
            formGraphics.SmoothingMode = SmoothingMode.AntiAlias
        Else
            If c.Width = 0 Or c.Height = 0 Then
                Exit Sub
            End If

            If c.Width <> formBitmap.Width Or c.Height <> formBitmap.Height Then
                Dim newBitmap As Bitmap = New Bitmap (c.Width, c.Height)
                Dim newGraphics As Graphics = Graphics.FromImage (newBitmap)
                newGraphics.SmoothingMode = SmoothingMode.AntiAlias
                newGraphics.DrawImage (formBitmap, 0, 0)

                formBitmap = newBitmap
                formGraphics = newGraphics
            End If
        End If

        scaleX = Width/virtualX
        scaleY = Height/virtualY
    End Sub

    Public Sub Line (ByVal startX As Single, ByVal startY As Single, ByVal endX As Single, ByVal endY As Single, _
                     ByVal ParamArray params() As Object)
        Dim drawColor As Color = Me.ForeColor
        If params.Length <> 0 Then
            drawColor = params (0)
        End If

        startX = startX*scaleX - scaleL*scaleX
        startY = startY*scaleY - scaleT*scaleY

        Dim _endX As Single = endX*scaleX - scaleL*scaleX
        Dim _endY As Single = endY*scaleY - scaleT*scaleY

        formGraphics.DrawLine (New Pen (drawColor, drawW), startX, startY, _endX, _endY)
        Me.BackgroundImage = formBitmap

        lastX = endX
        lastY = endY
    End Sub

    Public Sub Box (ByVal startX As Single, ByVal startY As Single, ByVal endX As Single, ByVal endY As Single, _
                    ByVal ParamArray params() As Object)
        Dim drawColor As Color = Me.ForeColor
        If params.Length <> 0 Then
            drawColor = params (0)
        End If

        Dim temp As Single

        If (startX > endX) Then
            temp = endX
            endX = startX
            startX = temp
        End If

        If (startY > endY) Then
            temp = endY
            endY = startY
            startY = temp
        End If

        startX = startX*scaleX - scaleL*scaleX
        startY = startY*scaleY - scaleT*scaleY

        Dim _endX As Single = endX*scaleX - scaleL*scaleX
        Dim _endY As Single = endY*scaleY - scaleT*scaleY

        formGraphics.FillRectangle (New SolidBrush (drawColor), startX, startY, _endX - startX, _endY - startY)
        Me.BackgroundImage = formBitmap

        lastX = endX
        lastY = endY
    End Sub

    Public Sub LineTo (ByVal endX As Single, ByVal endY As Single, ByVal ParamArray params() As Object)
        Dim drawColor As Color = Me.ForeColor
        If params.Length <> 0 Then
            drawColor = params (0)
        End If

        lastX = lastX*scaleX - scaleL*scaleX
        lastY = lastY*scaleY - scaleT*scaleY

        Dim _endX As Single = endX*scaleX - scaleL*scaleX
        Dim _endY As Single = endY*scaleY - scaleT*scaleY

        formGraphics.DrawLine (New Pen (drawColor, drawW), lastX, lastY, _endX, _endY)
        Me.BackgroundImage = formBitmap


        lastX = endX
        lastY = endY
    End Sub

    Public Property ScaleWidth() As Double
        Get
            ScaleWidth = virtualX
        End Get
        Set (ByVal value As Double)
            virtualX = value
            If virtualX = 0 Then
                scaleX = 0
            Else
                scaleX = Width/virtualX
            End If

            scaleL = 0
            scaleT = 0
        End Set
    End Property

    Public Property ScaleHeight() As Double
        Get
            ScaleHeight = virtualY
        End Get
        Set (ByVal value As Double)
            virtualY = value
            If virtualY = 0 Then
                scaleY = 0
            Else
                scaleY = Height/virtualY
            End If

            scaleL = 0
            scaleT = 0
        End Set
    End Property

    Public Property ScaleLeft() As Double
        Get
            ScaleLeft = scaleL
        End Get
        Set (ByVal value As Double)
            scaleL = value
        End Set
    End Property

    Public Property ScaleTop() As Double
        Get
            ScaleTop = scaleT
        End Get
        Set (ByVal value As Double)
            scaleT = value
        End Set
    End Property

    Public Sub UpdateImage()
        Me.BackgroundImage = formBitmap
        Me.Refresh()
    End Sub
End Class
