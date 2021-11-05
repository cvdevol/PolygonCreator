Imports System
Imports System.IO
Imports System.Runtime.ExceptionServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.AccessControl
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.Win32
Imports System.Math

Public Enum EditType
    Create
    Edit
End Enum

Public Module Globals

    Public WithEvents ErrorLogFile As String = My.Application.Info.DirectoryPath & "\ErrorLog.txt"
    Public WithEvents ErrorLog As StreamWriter

    Public ppDip As Double 'pixels per Dip (Pixels Per Density)
    Public ppiX As Double 'pixels per inch
    Public ppiY As Double 'pixels per inch

    Public taskbarHeight As Double

    Public Sub RunException(ByVal ex As Exception)
        ErrorLog.WriteLine(Now.ToString)
        ErrorLog.WriteLine(ex.ToString)
        ErrorLog.WriteLine(New String("*"c, 50))
    End Sub

    Public Sub RunExceptionWithMessageBox(ByVal ex As Exception)
        ErrorLog.WriteLine(Now.ToString)
        ErrorLog.WriteLine(ex.ToString)
        ErrorLog.WriteLine(New String("*"c, 50))
        MessageBox.Show(String.Format("{0} Error:  {1}" & vbCrLf & vbCrLf & "{2}", ex.Source, ex.Message, ex.StackTrace, "Initialize Error", MessageBoxButton.OK, MessageBoxImage.Error))
    End Sub

#Region "ToolBarEx Loaded"

    Public Sub RemoveToolbarHandleAndOverflow(ByVal TB As ToolBarEx)

        Dim overflowPanel = TryCast(TB.Template.FindName("PART_ToolBarOverflowPanel", TB), FrameworkElement)
        If overflowPanel IsNot Nothing Then
            overflowPanel.Opacity = 0
            overflowPanel.Visibility = Visibility.Hidden
        End If

        Dim overflowGrid = TryCast(TB.Template.FindName("OverflowGrid", TB), FrameworkElement)
        If overflowGrid IsNot Nothing Then
            overflowGrid.Opacity = 0

            overflowGrid.Visibility = Visibility.Hidden
        End If

        Dim mainPanelBorder = TryCast(TB.Template.FindName("MainPanelBorder", TB), FrameworkElement)
        If mainPanelBorder IsNot Nothing Then
            mainPanelBorder.Margin = New Thickness(0)
        End If
    End Sub

#End Region

#Region "Centroid"

    Public Function Centroid(ByVal points As IEnumerable(Of Point)) As Point
        Dim pt As Point = points.Aggregate(New With {
    Key .xSum = 0.0,
    Key .ySum = 0.0,
    Key .n = 0
}, Function(acc, p) New With {
    Key .xSum = acc.xSum + p.X,
    Key .ySum = acc.ySum + p.Y,
    Key .n = acc.n + 1
}, Function(acc) New Point(acc.xSum / acc.n, acc.ySum / acc.n))
        Return pt
    End Function



    Public Function PolygonArea(ByVal points As IEnumerable(Of Point)) As Double
        Dim e = points.GetEnumerator()
        If Not e.MoveNext() Then
            Return 0
        End If
        Dim first As Point = e.Current, last As Point = first

        Dim area As Double = 0
        Do While e.MoveNext()
            Dim [next] As Point = e.Current
            area += [next].X * last.Y - last.X * [next].Y
            last = [next]
        Loop
        area += first.X * last.Y - last.X * first.Y
        Return Abs(area)
    End Function

    ''' <summary>
    ''' Works for any regular polygon.
    ''' </summary>
    ''' <param name="points">Vertices of the polygon/</param>
    ''' <returns></returns>
    Public Function Perimeter(ByVal points As IEnumerable(Of Point)) As Double
        Dim len As Double
        len = points.Count * Point.Subtract(points(1), points(0)).Length
        Return len
    End Function

    ''' <summary>
    ''' Works for any polygon.
    ''' </summary>
    ''' <param name="points">Vertices of the polygon.</param>
    ''' <returns></returns>
    Public Function GeneralPerimeter(ByVal points As IEnumerable(Of Point)) As Double
        Dim len As Double
        For i = 0 To points.Count - 2
            len += Point.Subtract(points(i + 1), points(i)).Length
        Next
        len += Point.Subtract(points(0), points(points.Count - 1)).Length
        Return len
    End Function

#End Region



    Public Function BitmapToBitmapSource(img As System.Drawing.Bitmap) As System.Windows.Media.Imaging.BitmapSource
        Try
            Return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
        Catch ex As Exception
            ErrorLog.WriteLine(Now.ToString)
            ErrorLog.WriteLine(ex.ToString)
            Return Nothing
        End Try
    End Function




End Module

