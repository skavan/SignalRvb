Imports System
Imports Microsoft.AspNet.SignalR

Imports Microsoft.Owin.Hosting
Imports Owin
Imports Microsoft.Owin.Cors
Imports System.Threading
Imports Microsoft.Owin.StaticFiles
Imports Microsoft.Owin.FileSystems


Public Class frmServer

    Const SERVERURL As String = "http://+:8080"
    Private SignalR As IDisposable
    Private Delegate Sub HandleUpdateText(message As String)

    'Private Shared ReadOnly _instance As New Lazy(Of StockTicker)(Function() New StockTicker(GlobalHost.ConnectionManager.GetHubContext(Of StockTickerHub)().Clients))
    'Private Shared ReadOnly _instance As Hubs.IHubConnectionContext(Of Object) = GlobalHost.ConnectionManager.GetHubContext(Of MyHub)().Clients
    'Dim hubContext As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of MyHub)()

    '// we insert a reference to myHub into this variable for messaging
    Public Hub As MyHub

    '// start the server
    Private Sub Init()
        Try

            Dim root = AppDomain.CurrentDomain.BaseDirectory
            Dim filesystem = New PhysicalFileSystem(root)
            Dim options As New FileServerOptions

            options.EnableDirectoryBrowsing = True
            options.FileSystem = filesystem

            WebApp.Start(SERVERURL, Function(builder) builder.UseFileServer(options))
            'WebApp.Start(SERVERURL)
        Catch ex As Exception
            MsgBox("Server Failed To Start")
            Exit Sub
        End Try
        WriteToConsole("Server Started @ " & SERVERURL)
    End Sub

    '// leverages HandleUpdateText Delegate
    Public Sub WriteToConsole(message As String)
        If Me.InvokeRequired Then
            Me.Invoke(New HandleUpdateText(AddressOf WriteToConsole), {message})
        Else
            'TextBox1.SafeInvoke(Function(y As TextBox) InlineAssignHelper(y.Text, y.Text & message & vbCrLf))
            TextBox1.Text += message & vbCrLf
        End If

    End Sub

    Private Sub frmServer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If SignalR IsNot Nothing Then
            SignalR.Dispose()
        End If

    End Sub

    '// on form load
    Private Sub frmServer_Load(sender As Object, e As EventArgs) Handles Me.Load
        MainForm = Me
        Task.Run(AddressOf Init)    '// launch server on its own thread
    End Sub

    '// example image 1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SendToClients("From Server", "https://pbs.twimg.com/profile_images/415128901115867137/j5SIBgWo.jpeg")
    End Sub

    '// example image 2
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SendToClients("From Server", "https://www.google.com/images/srpr/logo11w.png")
    End Sub

    '// SignalR! Send a message to clients via the Hub
    Private Sub SendToClients(message As String, payload As String)
        If Hub IsNot Nothing Then
            Hub.Send(message, payload)
            WriteToConsole("Server Sending " & message & "|" & payload)
        Else
            WriteToConsole("No Clients to send to.")
        End If
    End Sub
End Class

'// used by SignalR
Class Startup
    Public Sub Configuration(app As IAppBuilder)
        'Dim config As New HubConfiguration

        app.UseCors(CorsOptions.AllowAll)
        app.MapSignalR()
    End Sub
End Class


Public Class MyHub
    Inherits Hub
    Public Sub Send(name As String, message As String)

        Clients.All.addMessage(name, message)
    End Sub
    Public Overrides Function OnConnected() As Task
        MainForm.WriteToConsole("Client Connected: " + Context.ConnectionId)
        MainForm.Hub = Me
        Return MyBase.OnConnected()
    End Function
    Public Overrides Function OnDisconnected() As Task
        MainForm.WriteToConsole("Client Disconnected: " + Context.ConnectionId)
        Return MyBase.OnDisconnected()
    End Function

End Class

