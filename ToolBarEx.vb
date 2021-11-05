Imports System.Windows.Controls.Primitives
Imports System.Windows.Media

''' <summary>
''' Custom Toolbar that hides the overflow panel
''' </summary>
Public Class ToolBarEx
    Inherits System.Windows.Controls.ToolBar

    Public Property OverflowPanelBackground() As Brush

    Public Sub New()
        OverflowPanelBackground = Brushes.Transparent
    End Sub

    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()

        Dim overflowPanel = TryCast(MyBase.GetTemplateChild("PART_ToolBarOverflowPanel"), ToolBarOverflowPanel)
        If overflowPanel IsNot Nothing Then
            overflowPanel.Background = Brushes.Transparent 'If(OverflowPanelBackground, Background)
            overflowPanel.Margin = New Thickness(0)
        End If

        Dim toolbarPanel = TryCast(MyBase.GetTemplateChild("PART_ToolBarPanel"), ToolBarPanel)
        If toolbarPanel IsNot Nothing Then

        End If

    End Sub




End Class
