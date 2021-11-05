Imports System.IO
Imports System.Windows.Threading

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Public ReadOnly Property ThemeDictionary() As ResourceDictionary
        ' You could probably get it via its name with some query logic as well.
        Get
            Return Resources.MergedDictionaries(0)
        End Get
    End Property

    Public Sub ChangeTheme(ByVal uri As Uri)
        Dim resourceDict As ResourceDictionary = TryCast(Application.LoadComponent(uri), ResourceDictionary)
        Application.Current.Resources.MergedDictionaries.Clear()
        Application.Current.Resources.MergedDictionaries.Add(resourceDict)
    End Sub


    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        ErrorLog = New StreamWriter(ErrorLogFile, True) With {.AutoFlush = True}

        Try

        Catch ex As Exception
            MessageBox.Show("Data files not found", ex.Message)
            Application.Current.Shutdown()
        End Try
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
    End Sub

    Private Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim ex As Exception = TryCast(e.ExceptionObject, Exception)
        RunException(ex)
        Me.Shutdown()
    End Sub

    Private Sub Application_DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        RunException(e.Exception)
        e.Handled = True
        Me.Shutdown(e.Exception.HResult)
    End Sub

    Private Sub Application_Exit(sender As Object, e As ExitEventArgs) Handles Me.[Exit]
        ErrorLog.Flush()
        ErrorLog.Close()
    End Sub
End Class
