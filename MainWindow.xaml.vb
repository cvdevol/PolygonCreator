Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Automation.Peers
Imports System.Math
Imports Xceed.Wpf.Toolkit
Imports System.Text
Imports RTF
Imports System.IO
Imports System.Collections.Specialized
Imports System.Windows.Markup
Imports System.Xml
Imports System.Xml.Linq
Imports System.Xml.Serialization
Imports Microsoft.Win32
Imports SharpVectors.Converters

Public Enum HitType
    None
    Box
    Vertex
End Enum

Class MainWindow
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub NotifyPropertyChanged(ByVal prop As String)
        Select Case prop
            Case "Bounds"
                'AbsoluteBounds = SetAbsoluteBounds()
        End Select
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    'Public Shared VerticesProperty As DependencyProperty = DependencyProperty.Register("Vertices", GetType(PointCollection), GetType(MainWindow))
    'Public Property Vertices As PointCollection
    '    Get
    '        Return CType(GetValue(VerticesProperty), PointCollection)
    '    End Get
    '    Set(value As PointCollection)
    '        SetValue(VerticesProperty, value)
    '        NotifyPropertyChanged("Vertices")
    '    End Set
    'End Property

    'Public Shared VertexMarkerPositionsProperty As DependencyProperty = DependencyProperty.Register("VertexMarkers", GetType(PointCollection), GetType(MainWindow))
    'Public Property VertexMarkerPositions As PointCollection
    '    Get
    '        Return CType(GetValue(VertexMarkerPositionsProperty), PointCollection)
    '    End Get
    '    Set(value As PointCollection)
    '        SetValue(VertexMarkerPositionsProperty, value)
    '    End Set
    'End Property

    ''Public Shared VertexMarkersProperty As DependencyProperty = DependencyProperty.Register("VertexMarkers", GetType(ObservableCollection(Of Ellipse)), GetType(MainWindow))
    ''Public Property VertexMarkers As ObservableCollection(Of Ellipse)
    ''    Get
    ''        Return CType(GetValue(VertexMarkersProperty), ObservableCollection(Of Ellipse))
    ''    End Get
    ''    Set(value As ObservableCollection(Of Ellipse))
    ''        SetValue(VertexMarkersProperty, value)
    ''    End Set
    ''End Property

    Public Shared BoundsProperty As DependencyProperty = DependencyProperty.Register("Bounds", GetType(Rect), GetType(MainWindow))
    Public Property Bounds As Rect
        Get
            Return CType(GetValue(BoundsProperty), Rect)
        End Get
        Set(value As Rect)
            SetValue(BoundsProperty, value)
            NotifyPropertyChanged("Bounds")
        End Set
    End Property

    Public Shared CanvasBoundsProperty As DependencyProperty = DependencyProperty.Register("CanvasBounds", GetType(Rect), GetType(MainWindow))
    Public Property CanvasBounds As Rect
        Get
            Return CType(GetValue(CanvasBoundsProperty), Rect)
        End Get
        Set(value As Rect)
            SetValue(CanvasBoundsProperty, value)
            NotifyPropertyChanged("CanvasBounds")
        End Set
    End Property

    Public Shared CentroidProperty As DependencyProperty = DependencyProperty.Register("Centroid", GetType(Point), GetType(MainWindow), New PropertyMetadata(New Point(0, 0)))
    Public Property Centroid As Point
        Get
            Return CType(GetValue(CentroidProperty), Point)
        End Get
        Set(value As Point)
            SetValue(CentroidProperty, value)
            NotifyPropertyChanged("Centroid")
        End Set
    End Property

    Public Shared SidesProperty As DependencyProperty = DependencyProperty.Register("Sides", GetType(Integer), GetType(MainWindow))
    Public Property Sides As Integer
        Get
            Return CType(GetValue(SidesProperty), Integer)
        End Get
        Set(value As Integer)
            SetValue(SidesProperty, value)
            NotifyPropertyChanged("Sides")
        End Set
    End Property

    'Public Shared VertexLinesProperty As DependencyProperty = DependencyProperty.Register("VertexLines", GetType(ObservableCollection(Of Line)), GetType(MainWindow))
    'Public Property VertexLines As ObservableCollection(Of Line)
    '    Get
    '        Return CType(GetValue(VertexLinesProperty), ObservableCollection(Of Line))
    '    End Get
    '    Set(value As ObservableCollection(Of Line))
    '        SetValue(VertexLinesProperty, value)
    '        NotifyPropertyChanged("VertexLines")
    '    End Set
    'End Property


    Public Property IncreaseSizeFactor As Double = 1.05
    Public Property DecreaseSizeFactor As Double = 0.95
    Public Property RotationIncrement As Double = PI / 30
    Public Property TotalRotation As Double = 0

    Public DashValuesCollection As New DoubleCollection From {0.5, 0.5}



    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = Me
        ppiX = VisualTreeHelper.GetDpi(Me).PixelsPerInchX
        ppiY = VisualTreeHelper.GetDpi(Me).PixelsPerInchY
        ppDip = VisualTreeHelper.GetDpi(Me).PixelsPerDip

        cbOutline.SelectItem(Brushes.Blue)
        cbInterior.SelectItem(Brushes.LightBlue)
        cbBox.SelectItem(Brushes.Black)
        cbLines.SelectItem(Brushes.Green)
        cbCentroid.SelectItem(Brushes.Red)


        NewPolygon()



    End Sub


#Region "New Polygon"

    Private Sub btnNewPolygon_Click(sender As Object, e As RoutedEventArgs)
        NewPolygon()
    End Sub

    Public Sub NewPolygon()
        Dim dc As New DoubleCollection From {0.5, 0.5}
        PGon = New Polygon With {.Name = "PGon", .Stroke = Brushes.Blue, .StrokeThickness = 1, .Fill = Brushes.LightBlue, .Tag = "PGon"}
        PBox = New Polygon With {.Name = "PBox", .Stroke = Brushes.Black, .StrokeThickness = 1, .StrokeDashArray = dc, .Fill = Brushes.Transparent, .Tag = "PBox"}
        PVertex = New Ellipse With {.Name = "PVertex", .Stroke = Brushes.Black, .StrokeThickness = 1, .Fill = Brushes.Transparent, .Width = 11, .Height = 11, .Tag = "PVertex"}
        PCentroid = New Polygon With {.Name = "PCentroid", .Stroke = Brushes.Black, .StrokeThickness = 1, .Fill = Brushes.Red, .Tag = "PCentroid"}
        PLines = New Polyline With {.Name = "PLines", .Stroke = Brushes.Green, .StrokeThickness = 1, .Fill = Brushes.Transparent, .StrokeDashArray = dc, .StrokeLineJoin = PenLineJoin.Round, .Tag = "PLines"}
        cnv.Children.Clear()
        cnv.Children.Add(PBox)
        cnv.Children.Add(PGon)
        cnv.Children.Add(PLines)
        cnv.Children.Add(PVertex)
        cnv.Children.Add(PCentroid)

        PVertex.Visibility = Visibility.Hidden
        PolygonFinalized = False
    End Sub

#End Region

#Region "Create Polygon"


    Private PolygonDragging As Boolean
    Private PolygonDraggingStartPoint As Point
    Private VertexDragging As Boolean
    Private VertexDraggingStartPoint As Point
    Private SelectedVertex As Integer?

    Private Sub cnv_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        PVertex.Visibility = Visibility.Visible
        Dim pt As Point = e.GetPosition(cnv)
        If PolygonFinalized = False Then
            Dim ptcenter As New Point(pt.X - 5, pt.Y - 5)
            PGon.Points.Add(pt)
            Canvas.SetLeft(PVertex, ptcenter.X)
            Canvas.SetTop(PVertex, ptcenter.Y)
            PVertex.Visibility = Visibility.Visible
        Else
            PVertex.Visibility = Visibility.Hidden
            VertexDraggingStartPoint = pt
            PolygonDraggingStartPoint = pt
            SelectedVertex = GetPointNearVertex(e)
        End If

    End Sub

    Private PolygonFinalized As Boolean = False
    Private Sub cnv_MouseRightButtonDown(sender As Object, e As MouseButtonEventArgs)
        SetCursor(e)
        FinalizePolygon()
    End Sub

    Public Sub FinalizePolygon()
        Sides = PGon.Points.Count
        If Sides = 0 Then Return
        SetBoundingBox2()
        SetCentroid()
        SetVertexLines()
        PLines.Visibility = Visibility.Visible
        PolygonFinalized = True
        Canvas.SetLeft(PGon, 0)
        Canvas.SetLeft(PBox, 0)
        Canvas.SetTop(PGon, 0)
        Canvas.SetTop(PBox, 0)
        PVertex.Visibility = Visibility.Hidden
        Canvas.SetLeft(PLines, 0)
        Canvas.SetTop(PLines, 0)
        ShowInfo()
    End Sub

    Private rlrHorizontalrelativePoint As Point
    Private rlrVerticalrelativePoint As Point


    Private ttHoriz As New ToolTip With {.PlacementTarget = rlrHorizontal, .IsEnabled = True, .Background = Brushes.Green, .Foreground = Brushes.White, .FontSize = 10}
    Private ttVert As New ToolTip With {.PlacementTarget = rlrVertical, .IsEnabled = True, .Background = Brushes.Green, .Foreground = Brushes.White, .FontSize = 10}
    Private Sub cnv_MouseMove(sender As Object, e As MouseEventArgs)
        Dim rlrHorizLocation As Point = rlrHorizontal.TransformToAncestor(Application.Current.MainWindow).Transform(New Point(0, 0))
        Dim rlrVertLocation As Point = rlrVertical.TransformToAncestor(Application.Current.MainWindow).Transform(New Point(0, 0))
        Dim pt As Point = e.GetPosition(cnv)
        rlrHorizontal.RaiseHorizontalRulerMoveEvent(e)
        rlrVertical.RaiseVerticalRulerMoveEvent(e)
        If miShowValueOnRulers.IsChecked Then
            ttHoriz.Content = CInt(pt.X)
            ttHoriz.IsOpen = True
            ttHoriz.Placement = Controls.Primitives.PlacementMode.AbsolutePoint
            ttHoriz.HorizontalOffset = e.GetPosition(rlrHorizontal).X + rlrHorizontalrelativePoint.X 'e.GetPosition(rlrHorizontal).X + ttHoriz.ActualWidth / 2
            ttHoriz.VerticalOffset = rlrHorizontalrelativePoint.Y + rlrHorizontal.ActualHeight 'rlrHorizLocation.Y + rlrHorizontal.ActualHeight

            ttVert.Content = CInt(pt.Y)
            ttVert.IsOpen = True
            ttVert.Placement = Controls.Primitives.PlacementMode.AbsolutePoint
            ttVert.VerticalOffset = e.GetPosition(rlrVertical).Y + rlrVertLocation.Y + ttVert.ActualHeight / 2
            ttVert.HorizontalOffset = rlrVertLocation.X
        Else
            ttHoriz.IsOpen = False
            ttVert.IsOpen = False
        End If

        If PolygonFinalized Then
            If Not PolygonDragging AndAlso Not VertexDragging Then SetCursor(e)
        End If

        If Mouse.LeftButton = MouseButtonState.Pressed Then
            If Cursor Is Cursors.Cross Then
                VertexDragging = True
                If SelectedVertex IsNot Nothing Then

                    MoveVertex(e, SelectedVertex, VertexDraggingStartPoint)
                End If
            ElseIf Cursor Is Cursors.SizeAll Then
                PolygonDragging = True
                MovePolygon(e, PolygonDraggingStartPoint)
            End If
        End If
        ShowInfo()
    End Sub

    Public Sub SetCursor(ByVal e As MouseEventArgs)
        Select Case GetHitType(e)
            Case HitType.Vertex
                Cursor = Cursors.Cross
            Case HitType.Box
                Cursor = Cursors.SizeAll
            Case HitType.None
                Cursor = Cursors.Arrow
        End Select
    End Sub

    Public Sub SetCursor(ByVal e As MouseButtonEventArgs)
        Select Case GetHitType(e)
            Case HitType.Vertex
                Cursor = Cursors.Cross
            Case HitType.Box
                Cursor = Cursors.ScrollAll
            Case HitType.None
                Cursor = Cursors.Arrow
        End Select
    End Sub


    Public Function GetHitType(ByVal e As MouseEventArgs) As HitType
        Dim pt As Point = e.GetPosition(cnv)
        Dim vp As Integer? = GetPointNearVertex(e)
        If vp.HasValue = True Then
            Return HitType.Vertex

        ElseIf vp.HasValue = False Then
            If Bounds.Contains(pt) Then
                Return HitType.Box
            End If
        End If
        Return HitType.None
    End Function

    Public Function GetHitType(ByVal e As MouseButtonEventArgs) As HitType
        Dim pt As Point = e.GetPosition(cnv)
        Dim vp As Integer? = GetPointNearVertex(e)
        If vp.HasValue = True Then
            Return HitType.Vertex

        ElseIf vp.HasValue = False Then
            If Bounds.Contains(pt) Then
                Return HitType.Box
            End If
        End If
        Return HitType.None
    End Function

    Public Function GetPointNearVertex(ByVal e As MouseButtonEventArgs) As Integer?
        Dim pt As Point = e.GetPosition(cnv)
        For i = 0 To Sides - 1
            If PGon.Points(i).DistanceToPoint(pt) < 5 Then
                Cursor = Cursors.Cross
                Return i
            End If
        Next
        Return Nothing
    End Function

    Public Function GetPointNearVertex(ByVal e As MouseEventArgs) As Integer?
        Dim pt As Point = e.GetPosition(cnv)
        For i = 0 To PGon.Points.Count - 1
            If PGon.Points(i).DistanceToPoint(pt) < 5 Then
                Cursor = Cursors.Cross
                Return i
            End If
        Next
        Return Nothing
    End Function

    Private Sub cnv_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim pt As Point = e.GetPosition(cnv)
        Cursor = Cursors.Arrow
        VertexDragging = False
        PolygonDragging = False
        PGon.ReleaseMouseCapture()
    End Sub

    Private Sub cnv_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim pt As Point = e.GetPosition(cnv)
        'Cursor = Cursors.Arrow
        SetCursor(e)
    End Sub

#End Region

#Region "Centroid, Bounding Box, and Interior Lines"

    Public Sub SetCentroid()
        PCentroid.Points.Clear()
        Dim C As Point = GetCentroid(PGon.Points.ToArray)
        PCentroid.Points.Add(New Point(C.X - 3, C.Y - 3))
        PCentroid.Points.Add(New Point(C.X + 3, C.Y - 3))
        PCentroid.Points.Add(New Point(C.X + 3, C.Y + 3))
        PCentroid.Points.Add(New Point(C.X - 3, C.Y + 3))
        Centroid = New Point(C.X, C.Y)
    End Sub

    Public Sub SetBoundingBox()
        Dim r As Rect = GetPathGeometry(PGon.Points.ToArray).Bounds
        PBox.Points.Clear()
        PBox.Points.Add(r.TopLeft)
        PBox.Points.Add(r.TopRight)
        PBox.Points.Add(r.BottomRight)
        PBox.Points.Add(r.BottomLeft)
        Bounds = r
    End Sub


    ''' <summary>
    ''' No need to get PathGeometry for this one.
    ''' </summary>
    Public Sub SetBoundingBox2()
        Dim minx As Double = PGon.Points.Select(Function(p) p.X).Min()
        Dim miny As Double = PGon.Points.Select(Function(p) p.Y).Min()
        Dim maxx As Double = PGon.Points.Select(Function(p) p.X).Max()
        Dim maxy As Double = PGon.Points.Select(Function(p) p.Y).Max()

        Dim r As New Rect(New Point(minx, miny), New Point(maxx, maxy))
        Dim r1 As Rect
        PBox.Points.Clear()
        PBox.Points.Add(r.TopLeft)
        PBox.Points.Add(r.TopRight)
        PBox.Points.Add(r.BottomRight)
        PBox.Points.Add(r.BottomLeft)
        Bounds = r
        r1 = GetPathGeometry(PGon.Points.ToArray).Bounds
    End Sub

    Public Sub SetVertexLines()
        Dim pc As New PointCollection
        PLines.Points = New PointCollection
        For i = 0 To Sides - 1
            For j = i + 1 To Sides - 2

                pc.Add(PGon.Points(i))
                pc.Add(PGon.Points(j + 1))
            Next
        Next
        PLines.Points = pc
    End Sub


#End Region

#Region "Mouse Wheel"

    Private Sub cnv_MouseWheel(sender As Object, e As MouseWheelEventArgs)
        If Not PolygonFinalized Then Return
        RotationIncrement = cbRotationAngle.SelectedItem.tag

        If Keyboard.Modifiers = ModifierKeys.Control Then

            If e.Delta > 0 Then
                RotatePolygon(RotationIncrement)
            ElseIf e.Delta < 0 Then
                RotatePolygon(-RotationIncrement)
            End If
        Else

            If e.Delta > 0 Then
                ResizePolygon(IncreaseSizeFactor)
            ElseIf e.Delta < 0 Then
                ResizePolygon(DecreaseSizeFactor)
            End If
        End If

    End Sub





#End Region

#Region "Rotate Polygon"

    Public Sub RotatePolygon(ByVal angle As Double)
        CheckPolygonInsideCanvas()
        'If PreventOutOfView() = True Then Return
        Dim c As Point = GetCentroid(PGon.Points.ToArray)
        For sidenumber = 0 To Sides - 1
            Dim dX = Math.Cos(angle) * (PGon.Points(sidenumber).X - c.X) - Math.Sin(angle) * (PGon.Points(sidenumber).Y - c.Y) + c.X
            Dim dY = Math.Sin(angle) * (PGon.Points(sidenumber).X - c.X) + Math.Cos(angle) * (PGon.Points(sidenumber).Y - c.Y) + c.Y
            Dim pt As New Point(dX, dY)
            PGon.Points(sidenumber) = pt
        Next
        TotalRotation += (angle * 180 / PI)
        If TotalRotation >= 360 Then TotalRotation -= 360
        If TotalRotation <= -360 Then TotalRotation += 360
        SetBoundingBox2()
        SetVertexLines()
        ShowInfo()
    End Sub

#End Region

#Region "Resize Polygon"

    Public Sub ResizePolygon(ByVal scalefactor As Double)
        CheckPolygonInsideCanvas()
        Dim C As Point = GetCentroid(PGon.Points.ToArray)
        Dim vc As New Vector(0, 0)
        If PBox.ActualWidth >= PCentroid.ActualWidth + 10 Then
            For i = 0 To Sides - 1
                Dim v1 As New Vector(PGon.Points(i).X - C.X, PGon.Points(i).Y - C.Y)
                Dim v As Vector = Vector.Subtract(v1, vc)
                Dim v2 As Vector = v * scalefactor
                PGon.Points(i) = New Point(v2.X + C.X, v2.Y + C.Y)
            Next

        End If
        SetCentroid()
        SetBoundingBox2()
        SetVertexLines()
        ShowInfo()
    End Sub

#End Region

#Region "Move Vertices"


    Public Sub MoveVertex(ByVal e As MouseEventArgs, vertex As Integer, start As Point)
        CheckPointsInsideCanvas()
        If e.LeftButton = MouseButtonState.Pressed Then


            If VertexDragging = True Then
                Dim pt As Point = e.GetPosition(cnv)

                Dim dx As Double = pt.X - start.X
                Dim dy As Double = pt.Y - start.Y
                Dim new_point As New Point(PGon.Points(vertex).X + dx, PGon.Points(vertex).Y + dy)
                PGon.Points(vertex) = new_point

                SetCentroid()
                SetBoundingBox2()
                SetVertexLines()
                VertexDraggingStartPoint = new_point
            End If
        End If

        ShowInfo()
    End Sub

#End Region

#Region "Move Polygon"

    Public Sub MovePolygon(ByVal e As MouseEventArgs, start As Point)
        CheckPolygonInsideCanvas()
        Dim pt As Point = e.GetPosition(cnv)

        Dim num_points As Integer = Sides


        If e.LeftButton = MouseButtonState.Pressed Then
            If PolygonDragging = True Then
                Dim new_point As Point
                Dim dx As Double = pt.X - start.X
                Dim dy As Double = pt.Y - start.Y
                For i As Integer = 0 To num_points - 1
                    'PreventOutOfView()
                    new_point = New Point(PGon.Points(i).X + dx, PGon.Points(i).Y + dy)
                    PGon.Points(i) = new_point
                Next i
                SetCentroid()
                SetBoundingBox2()
                SetVertexLines()
                PolygonDraggingStartPoint = pt 'new_point
            End If
        End If
        ShowInfo()
    End Sub


    ''' <summary>
    ''' Moves the Polygon left or right horizontally.
    ''' </summary>
    ''' <param name="distance">Negative to move left, Positive to move right.</param>
    Public Sub MoveHorizontal(ByVal distance As Double)
        Dim p As Point
        For i = 0 To Sides - 1
            p = PGon.Points(i)
            p.Offset(distance, 0)
            PGon.Points(i) = p
        Next
        SetCentroid()
        SetBoundingBox2()
    End Sub

    ''' <summary>
    ''' Moves the Polygon up or down vertically.
    ''' </summary>
    ''' <param name="distance">Negative to move up, Positive to move down.</param>
    Public Sub MoveVertical(ByVal distance As Double)
        Dim p As Point
        For i = 0 To Sides - 1
            p = PGon.Points(i)
            p.Offset(0, distance)
            PGon.Points(i) = p
        Next
        SetCentroid()
        SetBoundingBox2()
    End Sub

#End Region

#Region "Bounds Conflict"

    Private BoundsConflict As Boolean
    Public Function CheckPolygonInsideCanvas() As Boolean
        If Not CanvasBounds.Contains(Bounds.TopLeft) Then
            BoundsConflict = True
            Return False
        ElseIf Not CanvasBounds.Contains(Bounds.TopRight) Then
            BoundsConflict = True
            Return False
        ElseIf Not CanvasBounds.Contains(Bounds.BottomRight) Then
            BoundsConflict = True
            Return False
        ElseIf Not CanvasBounds.Contains(Bounds.BottomLeft) Then
            BoundsConflict = True
            Return False
        Else
            BoundsConflict = False
            Return True
        End If
    End Function

    Public Function CheckPointsInsideCanvas() As Boolean
        BoundsConflict = False
        For Each pt As Point In PGon.Points
            If Not CanvasBounds.Contains(pt) Then
                BoundsConflict = True
            End If
        Next
    End Function

#End Region

#Region "Information Display"

    Public Sub ShowInfo()
        'Dim dg As DrawingGroup
        'dg = VisualTreeHelper.GetDrawing(PGon)
        'Dim r As Rect = dg.Bounds
        TB.Inlines.Clear()
        'TB.Inlines.Add("P Bounds")
        'TB.Inlines.Add(vbCrLf)
        'TB.Inlines.Add("Left " & r.Left)
        'TB.Inlines.Add(vbCrLf)
        'TB.Inlines.Add("Top " & r.Top)
        'TB.Inlines.Add(vbCrLf)
        'TB.Inlines.Add("Right " & r.Width)
        'TB.Inlines.Add(vbCrLf)
        'TB.Inlines.Add("Bottom " & r.Height)
        'TB.Inlines.Add(vbCrLf)
        'TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("Polygon Bounds")
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("X = " & Bounds.X)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("Y = " & Bounds.Y)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("W = " & Bounds.Width)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("H = " & Bounds.Height)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("Polygon Centroid")
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("X = " & Centroid.X)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("Y = " & Centroid.Y)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("Polygon Sides = " & Sides)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("Polygon Area = " & PolygonArea(PGon.Points.ToArray) & " px")
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add("Polygon Rotation = " & TotalRotation & "°")
        TB.Inlines.Add(vbCrLf)
        TB.Inlines.Add(vbCrLf)

        Dim p As New Span With {.Foreground = Brushes.Red, .FontWeight = FontWeights.Bold, .FontSize = 20}
        If BoundsConflict = True Then
            p.Inlines.Add("OUT OF BOUNDS")
            TB.Inlines.Add(p)
        End If



    End Sub



#End Region

#Region "Menu Events"

    Private Sub miShowBox_Click(sender As Object, e As RoutedEventArgs)
        If miShowBox.IsChecked Then
            PBox.Visibility = Visibility.Visible
        Else
            PBox.Visibility = Visibility.Hidden
        End If
    End Sub

    Private Sub miShowCenter_Click(sender As Object, e As RoutedEventArgs)
        If miShowCenter.IsChecked Then
            PCentroid.Visibility = Visibility.Visible
        Else
            PCentroid.Visibility = Visibility.Hidden
        End If
    End Sub

    Private Sub miShowLines_Click(sender As Object, e As RoutedEventArgs)
        If miShowLines.IsChecked Then
            PLines.Visibility = Visibility.Visible
        Else
            PLines.Visibility = Visibility.Hidden
        End If
    End Sub

    Private Sub miAbout_Click(sender As Object, e As RoutedEventArgs)
        Dim w As New AboutWindow
        w.Show()
    End Sub

#End Region

#Region "Other Events"

    Private Sub ToolBarEx_Loaded(sender As Object, e As RoutedEventArgs)
        RemoveToolbarHandleAndOverflow(e.Source)
    End Sub

    Private Sub cnv_Loaded(sender As Object, e As RoutedEventArgs)
        Dim r As Rect
        r = VisualTreeHelper.GetContentBounds(cnv)
        Dim v As Vector
        v = VisualTreeHelper.GetOffset(cnv)
        Dim source As PresentationSource = PresentationSource.FromVisual(cnv)
        Dim dpiX, dpiY As Double
        If source IsNot Nothing Then
            dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11
            dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22
        End If

        Dim generalTransform1 As GeneralTransform = cnv.TransformToAncestor(Me)
        ' Retrieve the point value relative to the parent.
        Dim UpperLeftPointOfCanvasRelativeToMainWindow As Point = generalTransform1.Transform(New Point(0, 0))

        Dim pt As Point = GetCanvasPointRelativeToMainWindow(New Point(0, 0))

        rlrHorizontalrelativePoint = rlrHorizontal.TransformToAncestor(Application.Current.MainWindow).Transform(New Point(0, 0))
        rlrVerticalrelativePoint = rlrVertical.TransformToAncestor(Application.Current.MainWindow).Transform(New Point(0, 0))

    End Sub

    Public Function GetCanvasPointRelativeToMainWindow(ByVal pt As Point) As Point
        Dim generalTransform1 As GeneralTransform = cnv.TransformToAncestor(Application.Current.MainWindow)
        ' Retrieve the point value relative to the parent.
        Return generalTransform1.Transform(pt)
    End Function

    Private Sub cnv_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        CanvasBounds = New Rect(New Point(0, 0), e.NewSize)
    End Sub


#End Region

#Region "Load From XML"



    Private Sub miLoadXml_Click(sender As Object, e As RoutedEventArgs)
        Load()
    End Sub

    Public Sub Load()
        NewPolygon()
        Dim c As Canvas
        Dim dlg As New OpenFileDialog With {.Filter = "Xml Files (*.xml)|*.xml", .Multiselect = False}
        If dlg.ShowDialog = True Then
            c = DeserializeFromXML(dlg.FileName)
        Else
            Return
        End If
        cnv.Children.Clear()
        For i = 0 To c.Children.Count - 1
            Dim sh As Shape = CType(c.Children(0), Shape)
            If sh.Name = "PGon" Then PGon = CType(sh, Polygon)
            If sh.Name = "PBox" Then PBox = CType(sh, Polygon)
            If sh.Name = "PLines" Then PLines = CType(sh, Polyline)
            If sh.Name = "PVertex" Then PVertex = CType(sh, Ellipse)
            If sh.Name = "PCentroid" Then PCentroid = CType(sh, Polygon)
            RemoveFromParent(sh, c)
            cnv.Children.Add(sh)
        Next
        FinalizePolygon()
    End Sub

    Public Shared Function DeserializeFromXML(ByVal filename As String) As Canvas
        Dim c As Canvas
        Using fs As New FileStream(filename, FileMode.Open)
            Using SR As New StreamReader(fs)
                Dim mystrXAML As String = SR.ReadToEnd
                c = CType(XamlReader.Parse(mystrXAML), Canvas)
            End Using
        End Using
        Return c
    End Function

    Public Sub RemoveFromParent(ByVal item As FrameworkElement, pr As Panel)
        If item IsNot Nothing Then
            Dim parentControl = pr
            If parentControl IsNot Nothing Then
                parentControl.Children.Remove(TryCast(item, UIElement))
            End If
        End If
    End Sub

#End Region

#Region "Save To XML"

    Private Sub miSaveAsXml_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg As New SaveFileDialog With {.Filter = "Xml Files (*.xml)|*.xml", .DefaultExt = "xml", .AddExtension = True}
        If dlg.ShowDialog = True Then
            SerializeToXML(Application.Current.MainWindow, cnv, 96, dlg.FileName)
        End If
    End Sub

    Public Shared Sub SerializeToXML(ByVal window As MainWindow, ByVal canvas As Canvas, ByVal dpi As Integer, ByVal filename As String)
        Dim mystrXAML As String = XamlWriter.Save(canvas)
        Using filestream As FileStream = File.Create(filename)
            Using streamwriter As New StreamWriter(filestream)
                streamwriter.Write(mystrXAML)
            End Using
        End Using
    End Sub


    Public Function PrettyXml(ByVal xml As String) As String
        Dim stringBuilder As New StringBuilder()

        Dim element = XElement.Parse(xml)

        Dim settings = New XmlWriterSettings()
        settings.OmitXmlDeclaration = True
        settings.Indent = True
        settings.NewLineOnAttributes = True

        Using xWriter = XmlWriter.Create(stringBuilder, settings)
            element.Save(xWriter)
        End Using

        Return stringBuilder.ToString()
    End Function


#End Region

#Region "Save to PNG Image"

    Private Sub btnSaveCanvas_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg As New SaveFileDialog With {.Filter = "Png Files (*.png)|*.png", .DefaultExt = "png", .AddExtension = True}
        If dlg.ShowDialog = True Then
            SaveControlImage(CanvasGrid, dlg.FileName)
        End If

    End Sub

    ' Save a control's image.
    Private Sub SaveControlImage(ByVal control As FrameworkElement, ByVal filename As String)
        ' Get the size of the Visual and its descendants.
        Dim rect As Rect = VisualTreeHelper.GetDescendantBounds(control)

        ' Make a DrawingVisual to make a screen
        ' representation of the control.
        Dim dv As New DrawingVisual()

        ' Fill a rectangle the same size as the control
        ' with a brush containing images of the control.
        Using ctx As DrawingContext = dv.RenderOpen()
            Dim brush As New VisualBrush(control)
            ctx.DrawRectangle(brush, Nothing, New Rect(rect.Size))
        End Using

        ' Make a bitmap and draw on it.
        Dim width As Integer = CInt(Math.Truncate(control.ActualWidth))
        Dim height As Integer = CInt(Math.Truncate(control.ActualHeight))

        Dim rtb As New RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)

        ' Make a PNG encoder.
        Dim encoder As New PngBitmapEncoder()
        encoder.Frames.Add(BitmapFrame.Create(rtb))

        ' Save the file.
        Using fs As New FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None)
            encoder.Save(fs)
        End Using
    End Sub




#End Region

#Region "Change Colors"

    Private Sub btnResetColors_Click(sender As Object, e As RoutedEventArgs)
        PGon.Stroke = Brushes.Blue
        cbOutline.SelectItem(Brushes.Blue)
        PGon.Fill = Brushes.LightBlue
        cbInterior.SelectItem(Brushes.LightBlue)
        PBox.Stroke = Brushes.Black
        cbBox.SelectItem(Brushes.Black)
        PLines.Stroke = Brushes.Green
        cbLines.SelectItem(Brushes.Green)
        PCentroid.Fill = Brushes.Red
        cbCentroid.SelectItem(Brushes.Red)
        cnv.Background = Brushes.AliceBlue
        cbCanvas.SelectItem(Brushes.AliceBlue)
    End Sub

    Private Sub cbOutline_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        PGon.Stroke = cbOutline.SelectedItem.Brush
    End Sub

    Private Sub cbInterior_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        PGon.Fill = cbInterior.SelectedItem.Brush
    End Sub

    Private Sub cbBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        PBox.Stroke = cbBox.SelectedItem.Brush
    End Sub

    Private Sub cbLines_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        PLines.Stroke = cbLines.SelectedItem.Brush
    End Sub

    Private Sub cbCentroid_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        PCentroid.Fill = cbCentroid.SelectedItem.Brush
    End Sub

    Private Sub cbCanvas_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        cnv.Background = cbCanvas.SelectedItem.brush
    End Sub

#End Region

#Region "Change Rotation Angle"

    Private Sub cbRotationAngle_Loaded(sender As Object, e As RoutedEventArgs)
        Dim sc As New ObservableCollection(Of TextBlock)
        Dim tb0 As New TextBlock With {.Text = ("π (180°)"), .Tag = PI}
        Dim tb1 As New TextBlock With {.Text = ("π/2 (90°)"), .Tag = PI / 2}
        Dim tb2 As New TextBlock With {.Text = ("π/3 (60°)"), .Tag = PI / 3}
        Dim tb3 As New TextBlock With {.Text = ("π/4 (45°)"), .Tag = PI / 4}
        Dim tb4 As New TextBlock With {.Text = ("π/6 (30°)"), .Tag = PI / 6}
        Dim tb5 As New TextBlock With {.Text = ("π/12 (15°)"), .Tag = PI / 12}
        Dim tb6 As New TextBlock With {.Text = ("π/36 (5°)"), .Tag = PI / 36}
        Dim tb7 As New TextBlock With {.Text = ("π/180 (1°))"), .Tag = PI / 60}
        sc.Add(tb0)
        sc.Add(tb1)
        sc.Add(tb2)
        sc.Add(tb3)
        sc.Add(tb4)
        sc.Add(tb5)
        sc.Add(tb6)
        sc.Add(tb7)
        cbRotationAngle.ItemsSource = sc
    End Sub

    Private Sub cbRotationAngle_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        RotationIncrement = cbRotationAngle.Tag
    End Sub

#End Region

    Public Sub Dummy()

    End Sub

    Private Sub miShowValueOnRulers_Click(sender As Object, e As RoutedEventArgs)

    End Sub



    Private Sub miExit_Click(sender As Object, e As RoutedEventArgs)
        Application.Current.MainWindow.Close()
    End Sub


End Class
