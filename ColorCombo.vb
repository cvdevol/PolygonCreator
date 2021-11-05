Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Reflection

Public Class ColorCombo
    Inherits ComboBoxEx
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub NotifyPropertyChanged(ByVal prop As String)
        Select Case prop

        End Select

    End Sub

    Public Shared ColorItemsProperty As DependencyProperty = DependencyProperty.Register("ColorItems", GetType(ObservableCollection(Of ColorItem)), GetType(ColorCombo))
    Public Property ColorItems As ObservableCollection(Of ColorItem)
        Get
            Return CType(GetValue(ColorItemsProperty), ObservableCollection(Of ColorItem))
        End Get
        Set(value As ObservableCollection(Of ColorItem))
            SetValue(ColorItemsProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Brush currently showing.
    ''' </summary>
    Public Shared SelectedBrushProperty As DependencyProperty = DependencyProperty.Register("SelectedBrush", GetType(Brush), GetType(ColorCombo))
    Public Property SelectedBrush() As Brush
        Get
            Return CType(GetValue(SelectedBrushProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(SelectedBrushProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Color currently showing.
    ''' </summary>
    Public Shared SelectedColorProperty As DependencyProperty = DependencyProperty.Register("SelectedColor", GetType(Color), GetType(ColorCombo))
    Public Property SelectedColor() As Color
        Get
            Return CType(GetValue(SelectedColorProperty), Color)
        End Get
        Set(value As Color)
            SetValue(SelectedColorProperty, value)
        End Set
    End Property

    Public Shared SelectedColorNameProperty As DependencyProperty = DependencyProperty.Register("SelectedColorName", GetType(String), GetType(ColorCombo))
    Public Property SelectedColorName() As String
        Get
            Return CType(GetValue(SelectedColorNameProperty), String)
        End Get
        Set(value As String)
            SetValue(SelectedColorNameProperty, value)
        End Set
    End Property

    Public ReadOnly DefaultColor = Colors.AliceBlue

    Public Sub New()
        MyBase.New
        LoadColors()
        Me.ItemsSource = ColorItems
        Me.MinWidth = 100
        Me.SelectColor(DefaultColor)
    End Sub

    Private Sub LoadColors()
        ColorItems = New ObservableCollection(Of ColorItem)
        Dim colType As Type = GetType(System.Windows.Media.Colors)
        Dim i As Integer
        For Each prop As PropertyInfo In colType.GetProperties()
            If prop.PropertyType Is GetType(System.Windows.Media.Color) Then
                Dim c As Color = prop.GetValue(prop)
                ColorItems.Add(New ColorItem(c, prop.Name) With {.Tag = i})
                i += 1
            End If
        Next
    End Sub

    Public Sub SelectColor(ByVal clr As Color)
        Dim q = From c In ColorItems
                Where clr = c.Color
                Select c

        Me.SelectedItem = q(0)


    End Sub

    Protected Overrides Sub OnMouseWheel(e As MouseWheelEventArgs)
        MyBase.OnMouseWheel(e)

        If e.Delta > 0 Then
            If SelectedIndex > 0 Then SelectedIndex -= 1
        ElseIf e.Delta < 0 Then
            If SelectedIndex < Items.Count - 1 Then SelectedIndex += 1
        End If
    End Sub

    'Protected Overrides Sub OnSelectionChanged(e As SelectionChangedEventArgs)
    '    MyBase.OnSelectionChanged(e)
    '    Dim c As ColorItem = CType(e.AddedItems(0), ColorItem)
    '    SelectedItem = c
    '    SelectedBrush = c.Brush
    '    SelectedColor = c.Color
    '    SelectedColorName = c.Name
    'End Sub


End Class

Public Class ColorItem
    Inherits ContentControl

    Public Property Color As Color
    Public Property Brush As SolidColorBrush

    Public Property Name As String

    Public Sub New(ByVal clr As Color, n As String)
        MyBase.New
        Me.VerticalContentAlignment = VerticalAlignment.Center
        Name = n
        Color = clr
        Brush = New SolidColorBrush(Color)
        Dim tb As New TextBlock With {.Text = Name, .Margin = New Thickness(2, 0, 0, 0), .HorizontalAlignment = HorizontalAlignment.Stretch, .VerticalAlignment = VerticalAlignment.Center}
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
        Return Name
    End Function

End Class
