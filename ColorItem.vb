Imports System.Reflection
''' <summary>
''' Custom Class for representing a Color as a System.Windows.Media.Color/System.Windows.SystemColors, a System.Windows.Media.SolidColorBrush, or a Color name.
''' </summary>
Public Class ColorItem
    Inherits ContentControl

    Public Property Color As Color
    Public Property Brush As SolidColorBrush
    Public Property ColorName As String
    Public Property Index As Integer

    Public Sub New(ByVal clr As Color, clrname As String)
        MyBase.New
        ColorName = clrname
        Color = clr
        Brush = New SolidColorBrush(Color)
        Dim tb As New TextBlock With {.Text = ColorName, .Margin = New Thickness(0), .HorizontalAlignment = HorizontalAlignment.Stretch, .VerticalAlignment = VerticalAlignment.Center}
        Dim r As New Rectangle
        With r
            .Width = 23
            .Height = 13
            .Fill = Brush
            .Stroke = Brushes.Black
            .StrokeThickness = 1
            .VerticalAlignment = VerticalAlignment.Center
            .Margin = New Thickness(0, 0, 2, 0)
        End With
        Dim sp As New StackPanel With {.Orientation = Orientation.Horizontal, .HorizontalAlignment = HorizontalAlignment.Stretch, .VerticalAlignment = VerticalAlignment.Center}
        sp.Children.Add(r)
        sp.Children.Add(tb)
        Me.Content = sp

    End Sub

    Public Overrides Function ToString() As String
        Return ColorName
    End Function

End Class
