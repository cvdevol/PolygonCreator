Imports System.IO
Imports System.Reflection
Imports System.Resources
Imports System.Runtime.InteropServices
Imports System.Text
Imports RTF

Public Class AboutWindow

    Public Sub New(Optional ByVal debug As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'RunDebugVersion = True
        Dim rb As New RTFBuilder
        Dim CurrentAssembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()

        Dim idattribute = CType(CurrentAssembly.GetCustomAttributes(GetType(GuidAttribute), True)(0), GuidAttribute)
        Dim id = idattribute.Value

        Dim langattribute As NeutralResourcesLanguageAttribute
        Dim lang As String



        rb.FontSize(28).FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Red).AppendLine(My.Application.Info.Title)
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Version: ").AppendLine(My.Application.Info.Version.ToString)
        rb.AppendLine()
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("GUID: ").AppendLine(id)
        Try
            langattribute = CType(CurrentAssembly.GetCustomAttributes(GetType(NeutralResourcesLanguageAttribute), True)(0), NeutralResourcesLanguageAttribute)
            lang = langattribute.CultureName
        Catch ex As Exception
            lang = "None"
        End Try
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Neutral Language: ").AppendLine(lang)

        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("File Location: ").AppendLine(My.Application.Info.DirectoryPath)
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Copyright: ").AppendLine(My.Application.Info.Copyright)
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Trademark: ").AppendLine(My.Application.Info.Trademark)
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Company: ").AppendLine(My.Application.Info.CompanyName)
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Product: ").AppendLine(My.Application.Info.ProductName)
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Description: ").AppendLine(My.Application.Info.Description)

        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("CLR Version: ").AppendLine(Environment.Version.ToString)
        Dim opsys As String = Environment.OSVersion.ToString()
        If Environment.Is64BitOperatingSystem Then opsys &= " (64-bit)" Else opsys &= " (32-bit)"
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("OS Version: ").AppendLine(opsys)
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("System Directory: ").AppendLine(Environment.SystemDirectory)
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("User: ").AppendLine(Environment.UserName)
        Try
            rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("User Domain: ").AppendLine(Environment.UserDomainName)
        Catch ex As Exception
            rb.FontStyle(System.Drawing.FontStyle.Bold).FontStyle(System.Drawing.FontStyle.Italic).ForeColor(System.Drawing.Color.Red).Append("None")
        End Try

        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Machine Name: ").AppendLine(Environment.MachineName & " (" & Environment.ProcessorCount.ToString & " Processors)")
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Page Size: ").AppendLine(Environment.SystemPageSize.ToString("#,#") & " Bytes")
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("Working Set: ").AppendLine(Environment.WorkingSet.ToString("#,#") & " Bytes")
        Dim ts As TimeSpan = TimeSpan.FromMilliseconds(Environment.TickCount)
        Dim uptime As String = ts.Days.ToString() & ":" & ts.Hours.ToString("0#") & ":" & ts.Minutes.ToString("0#") & ":" & ts.Seconds.ToString("0#")
        rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append("System Up Time: ").AppendLine(uptime)

        rb.AppendLine()
        rb.FontSize(28).FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Red).Append("Contact Information: ")
        rb.FontSize(28).FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).AppendLine("eat@joes.pub")

        Dim tr As New TextRange(P1.ContentStart, P1.ContentEnd)

        Using ms As New MemoryStream(System.Text.Encoding.UTF8.GetBytes(rb.ToString))
            tr.Load(ms, DataFormats.Rtf)
        End Using

    End Sub

    Private Sub btnOK_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub

    Private Sub btnCopy_Click(sender As Object, e As RoutedEventArgs)
        RTB.SelectAll()
        RTB.Copy()
    End Sub

    Private P2 As New Paragraph

    Private Sub btnMore_Checked(sender As Object, e As RoutedEventArgs)
        RTB.Document.Blocks.Add(P2)
        Dim rb = New RTFBuilder

        'rb.AppendLine()
        rb.FontSize(28).FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Red).AppendLine("Environment Variables:")
        rb.AppendLine()

        Dim environmentVariables As IDictionary = Environment.GetEnvironmentVariables()
        Dim de As DictionaryEntry
        For Each de In environmentVariables
            rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append(de.Key).FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Red).Append(" = ").FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Blue).AppendLine(de.Value)
        Next de

        rb.AppendLine()
        rb.FontSize(28).FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Red).AppendLine("Loaded Assemblies:")
        rb.AppendLine()

        'For Each Assembly As Assembly In GetAssemblies()
        '    Dim s() As String = Assembly.FullName.Split(","c)
        '    rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append(s(0))
        '    rb.ForeColor(System.Drawing.Color.Black).Append(s(1))
        '    rb.ForeColor(System.Drawing.Color.Blue).Append(s(2))
        '    rb.ForeColor(System.Drawing.Color.Purple).AppendLine(s(3))
        'Next

        For Each Assembly As Assembly In AppDomain.CurrentDomain.GetAssemblies
            Dim s() As String = Assembly.FullName.Split(","c)
            rb.FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).Append(s(0))
            rb.ForeColor(System.Drawing.Color.Black).Append(s(1))
            rb.ForeColor(System.Drawing.Color.Blue).Append(s(2))
            rb.ForeColor(System.Drawing.Color.Purple).AppendLine(s(3))
        Next

        'rb.AppendLine()
        'rb.FontSize(28).FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Red).Append("Contact Information: ")
        'rb.FontSize(28).FontStyle(System.Drawing.FontStyle.Bold).ForeColor(System.Drawing.Color.Green).AppendLine("eat@joes.pub")

        Dim tr = New TextRange(P2.ContentStart, P2.ContentEnd)
        Using ms As New MemoryStream(System.Text.Encoding.UTF8.GetBytes(rb.ToString))
            tr.Load(ms, DataFormats.Rtf)
        End Using
        btnMore.Content = "Show Less"
    End Sub

    Private Sub btnMore_Unchecked(sender As Object, e As RoutedEventArgs)
        Try
            RTB.Document.Blocks.Remove(P2)
            btnMore.Content = "Show More"
        Catch ex As Exception

        End Try

    End Sub

    Public Shared Function GetAssemblies() As List(Of Assembly)
        Dim returnAssemblies = New List(Of Assembly)
        Dim loadedAssemblies = New HashSet(Of String)
        Dim assembliesToCheck = New Queue(Of Assembly)

        assembliesToCheck.Enqueue(System.Reflection.Assembly.GetEntryAssembly())

        Do While assembliesToCheck.Any()
            Dim assemblyToCheck = assembliesToCheck.Dequeue()

            For Each reference In assemblyToCheck.GetReferencedAssemblies()
                If Not loadedAssemblies.Contains(reference.FullName) Then
                    Dim assembly = System.Reflection.Assembly.Load(reference)
                    assembliesToCheck.Enqueue(assembly)
                    loadedAssemblies.Add(reference.FullName)
                    returnAssemblies.Add(assembly)
                End If
            Next reference
        Loop
        Return returnAssemblies
    End Function

    ''' <summary>
    ''' If there are non-.NET assemblies in the BaseDirectory, this function will fail.
    ''' </summary>
    ''' <returns>All Assemblies in the BaseDirectory</returns>
    Public Function GetSolutionAssemblies() As System.Reflection.Assembly()
        Dim assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll").Select(Function(x) System.Reflection.Assembly.Load(AssemblyName.GetAssemblyName(x)))
        Return assemblies.ToArray()
    End Function

End Class



