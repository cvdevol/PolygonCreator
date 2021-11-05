Imports System.Text.RegularExpressions
Imports System.Windows.Controls.Primitives
Imports SharpVectors.Converters

''' <summary>
''' A custom ComboBox that limits the amount of text that can be entered to 7 characters that are Capital letters.
''' </summary>
Public Class ComboBoxEx
    Inherits System.Windows.Controls.ComboBox

    Public Shared TextBoxProperty As DependencyProperty = DependencyProperty.RegisterAttached("TextBox", GetType(TextBox), GetType(ComboBoxEx))
    Public Property TextBox As TextBox
        Get
            Return CType(GetValue(TextBoxProperty), TextBox)
        End Get
        Set(value As TextBox)
            SetValue(TextBoxProperty, value)
        End Set
    End Property

    Public Shared PopupProperty As DependencyProperty = DependencyProperty.RegisterAttached("Popup", GetType(Popup), GetType(ComboBoxEx))
    Public Property Popup As Popup
        Get
            Return CType(GetValue(PopupProperty), Popup)
        End Get
        Set(value As Popup)
            SetValue(PopupProperty, value)
        End Set
    End Property

    Private Shared ReadOnly regex As New Regex("^[A-Z]+$", RegexOptions.Singleline)


    Public Sub New()
        MyBase.New

    End Sub

    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()

        TextBox = TryCast(MyBase.GetTemplateChild("PART_EditableTextBox"), TextBox)
        If TextBox IsNot Nothing Then
            TextBox.VerticalContentAlignment = VerticalAlignment.Center

        End If

        Popup = TryCast(MyBase.GetTemplateChild("PART_Popup"), Popup)
        If Popup IsNot Nothing Then

        End If
    End Sub

    Private Sub tbox_PreviewTextInput(ByVal sender As Object, e As TextCompositionEventArgs)
        'If Not regex.IsMatch(e.Text) Then
        '    e.Handled = True
        'End If

    End Sub

    Private Sub tbox_TextChanged(ByVal sender As Object, e As TextChangedEventArgs)

    End Sub

End Class
