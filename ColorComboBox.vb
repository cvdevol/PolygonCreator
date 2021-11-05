Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Reflection

''' <summary>
''' The Type of Colors to be displayed.
''' </summary>
Public Enum ColorType
    WebColors
    SystemColors
    AllColors
End Enum


''' <summary>
''' A Custom ComboBox for choosing a Brush or Color or Color name.
''' </summary>
Public Class ColorComboBox
    Inherits ComboBoxEx
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' Raises a PropertyChanged Event
    ''' </summary>
    ''' <param name="prop">The Property that has been changed</param>
    Public Sub NotifyPropertyChanged(ByVal prop As String)
        Select Case prop
            Case "DisplayType"
                Select Case DisplayType
                    Case ColorType.WebColors
                        ItemsSource = WebColorItems
                    Case ColorType.SystemColors
                        ItemsSource = SystemColorItems
                    Case ColorType.AllColors
                        ItemsSource = AllColorItems
                End Select
                SelectedIndex = 0
        End Select

    End Sub

    ''' <summary>
    ''' Gets or Sets the selected Brush, i.e. Brushes.Black
    ''' </summary>
    Public Shared SelectedBrushProperty As DependencyProperty = DependencyProperty.Register("SelectedBrush", GetType(Brush), GetType(ColorComboBox))
    Public Property SelectedBrush() As Brush
        Get
            Return CType(GetValue(SelectedBrushProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(SelectedBrushProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Gets or Sets the selected Color, i.e. Colors.Black
    ''' </summary>
    Public Shared SelectedColorProperty As DependencyProperty = DependencyProperty.Register("SelectedColor", GetType(Color), GetType(ColorComboBox))
    Public Property SelectedColor() As Color
        Get
            Return CType(GetValue(SelectedColorProperty), Color)
        End Get
        Set(value As Color)
            SetValue(SelectedColorProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the selected color name, i.e. "Black"
    ''' </summary>
    Public Shared SelectedColorNameProperty As DependencyProperty = DependencyProperty.Register("SelectedColorName", GetType(String), GetType(ColorComboBox))
    Public Property SelectedColorName() As String
        Get
            Return CType(GetValue(SelectedColorNameProperty), String)
        End Get
        Set(value As String)
            SetValue(SelectedColorNameProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' List of all Web Colors (System.Windows.Media.Colors)
    ''' </summary>
    Public Shared WebColorItemsProperty As DependencyProperty = DependencyProperty.Register("ColorItems", GetType(ObservableCollection(Of ColorItem)), GetType(ColorComboBox))
    Public Property WebColorItems As ObservableCollection(Of ColorItem)
        Get
            Return GetValue(WebColorItemsProperty)
        End Get
        Set(value As ObservableCollection(Of ColorItem))
            SetValue(WebColorItemsProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' List of all System Colors (System.Windows.SystemColors)
    ''' </summary>
    Public Shared SystemColorItemsProperty As DependencyProperty = DependencyProperty.Register("SystemColorItems", GetType(ObservableCollection(Of ColorItem)), GetType(ColorComboBox))
    Public Property SystemColorItems As ObservableCollection(Of ColorItem)
        Get
            Return GetValue(SystemColorItemsProperty)
        End Get
        Set(value As ObservableCollection(Of ColorItem))
            SetValue(SystemColorItemsProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' List of all colors.
    ''' </summary>
    Public Shared AllColorItemsProperty As DependencyProperty = DependencyProperty.Register("AllColorItems", GetType(ObservableCollection(Of ColorItem)), GetType(ColorComboBox))
    Public Property AllColorItems As ObservableCollection(Of ColorItem)
        Get
            Return GetValue(AllColorItemsProperty)
        End Get
        Set(value As ObservableCollection(Of ColorItem))
            SetValue(AllColorItemsProperty, value)
        End Set
    End Property

    ''' <summary>
    '''  Choice of color types, either System.Windows.Media.Colors ("Web" Colors), System.Windows.SystemColors (Windows System Colors), or both.
    ''' </summary>
    Public Shared DisplayTypeProperty As DependencyProperty = DependencyProperty.Register("DisplayType", GetType(ColorType), GetType(ColorComboBox), New PropertyMetadata(ColorType.WebColors))
    Public Property DisplayType() As ColorType
        Get
            Return GetValue(DisplayTypeProperty)
        End Get
        Set(value As ColorType)
            SetValue(DisplayTypeProperty, value)
            NotifyPropertyChanged("DisplayType")
        End Set
    End Property

    Public Sub New()
        MyBase.New
        Me.DataContext = Me
        DisplayType = ColorType.WebColors

        LoadWebColors()
        LoadSystemColors()
        LoadAllColors()
        ItemsSource = WebColorItems
    End Sub

#Region "Load Colors"

    ''' <summary>
    ''' Loads the Collection of regular Colors, i.e. "WebColors"
    ''' </summary>
    Private Sub LoadWebColors()
        WebColorItems = New ObservableCollection(Of ColorItem)
        Dim colType As Type = GetType(System.Windows.Media.Colors)
        Dim i As Integer
        For Each prop As PropertyInfo In colType.GetProperties()
            If prop.PropertyType Is GetType(System.Windows.Media.Color) Then
                Dim c As Color = prop.GetValue(prop)
                WebColorItems.Add(New ColorItem(c, prop.Name) With {.Tag = i})
                i += 1
            End If
        Next
    End Sub

    ''' <summary>
    ''' Load the Collection of System Colors
    ''' </summary>
    Private Sub LoadSystemColors()
        SystemColorItems = New ObservableCollection(Of ColorItem)
        Dim colType = GetType(System.Windows.SystemColors)
        Dim i As Integer
        For Each prop As PropertyInfo In colType.GetProperties()
            If prop.PropertyType Is GetType(System.Windows.Media.Color) Then
                Dim c As Color = prop.GetValue(prop)
                SystemColorItems.Add(New ColorItem(c, prop.Name) With {.Tag = i})
                i += 1
            End If
        Next
    End Sub

    ''' <summary>
    ''' Loads the Collection of All colors, i.e. regular colors ("WebColors") + System Colors
    ''' </summary>
    Private Sub LoadAllColors()
        AllColorItems = New ObservableCollection(Of ColorItem)
        Dim colType As Type = GetType(System.Windows.Media.Colors)
        Dim i As Integer
        For Each prop As PropertyInfo In colType.GetProperties()
            If prop.PropertyType Is GetType(System.Windows.Media.Color) Then
                Dim c As Color = prop.GetValue(prop)
                AllColorItems.Add(New ColorItem(c, prop.Name) With {.Tag = i})
                i += 1
            End If
        Next
        colType = GetType(System.Windows.SystemColors)
        For Each prop As PropertyInfo In colType.GetProperties()
            If prop.PropertyType Is GetType(System.Windows.Media.Color) Then
                Dim c As Color = prop.GetValue(prop)
                AllColorItems.Add(New ColorItem(c, prop.Name) With {.Tag = i})
                i += 1
            End If
        Next
    End Sub

#End Region

#Region "Select Item"

    ''' <summary>
    ''' Select An item by Brush
    ''' </summary>
    ''' <param name="br">A SolidColorBrush, for example Brushes.Black</param>
    Public Sub SelectItem(ByVal br As SolidColorBrush)
        Dim q = From ci As ColorItem In ItemsSource
                Where ci.Brush.Color = br.Color
                Select ci

        If q.Count > 0 Then
            SelectedItem = CType(q(0), ColorItem)
            SelectedBrush = SelectedItem.Brush
            SelectedColor = SelectedItem.Color
            SelectedColorName = SelectedItem.ColorName
        End If

    End Sub

    ''' <summary>
    ''' Select An item by Color
    ''' </summary>
    ''' <param name="clr">A Color, for example Colors.Black</param>
    Public Sub SelectItem(ByVal clr As Color)
        Dim q = From ci As ColorItem In ItemsSource
                Where ci.Color = clr
                Select ci

        If q.Count > 0 Then
            SelectedItem = CType(q(0), ColorItem)
            SelectedBrush = SelectedItem.Brush
            SelectedColor = SelectedItem.Color
            SelectedColorName = SelectedItem.ColorName
        End If

    End Sub

    ''' <summary>
    ''' Select an item by Color name.
    ''' </summary>
    ''' <param name="clrname">A Color name, for example "Black"</param>
    Public Sub SelectItem(ByVal clrname As String)
        Dim q = From ci As ColorItem In ItemsSource
                Where ci.ColorName = clrname
                Select ci

        If q.Count > 0 Then
            SelectedItem = CType(q(0), ColorItem)
            SelectedBrush = SelectedItem.Brush
            SelectedColor = SelectedItem.Color
            SelectedColorName = SelectedItem.ColorName
        End If

    End Sub

#End Region




    Protected Overrides Sub OnSelectionChanged(e As SelectionChangedEventArgs)
        MyBase.OnSelectionChanged(e)
        If e.AddedItems.Count = 0 Then Return
        SelectedItem = CType(e.AddedItems(0), ColorItem)
        SelectedBrush = SelectedItem.Brush
        SelectedColor = SelectedItem.Color
        SelectedColorName = SelectedItem.ColorName
        Dim c As ObservableCollection(Of ColorItem) = CType(ItemsSource, ObservableCollection(Of ColorItem))
        SelectedIndex = c.IndexOf(SelectedItem)
        SelectedItem.Index = SelectedIndex
    End Sub

    Protected Overrides Sub OnMouseWheel(e As MouseWheelEventArgs)
        MyBase.OnMouseWheel(e)
        If e.Delta > 0 Then
            If SelectedIndex > 0 Then SelectedIndex -= 1
        ElseIf e.Delta < 0 Then
            If SelectedIndex < Items.Count - 1 Then SelectedIndex += 1
        End If
    End Sub

End Class
