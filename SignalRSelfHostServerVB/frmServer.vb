Imports System
Imports Microsoft.AspNet.SignalR

Imports Microsoft.Owin.Hosting
Imports Owin
Imports Microsoft.Owin.Cors
Imports System.Threading


Public Class frmServer
    'Const SERVERURL As String = "http://localhost:8080"
    Const SERVERURL As String = "http://+:8080"
    Private SignalR As IDisposable
    Private Delegate Sub UpdateTxt(message As String)
    'Private Shared ReadOnly _instance As New Lazy(Of StockTicker)(Function() New StockTicker(GlobalHost.ConnectionManager.GetHubContext(Of StockTickerHub)().Clients))
    Private Shared ReadOnly _instance As Hubs.IHubConnectionContext(Of Object) = GlobalHost.ConnectionManager.GetHubContext(Of MyHub)().Clients
    'Dim hubContext As IHubContext = GlobalHost.ConnectionManager.GetHubContext(Of MyHub)()
    Public Hub As MyHub
    Private Sub Init()
        Try
            WebApp.Start(SERVERURL)
        Catch ex As Exception
            MsgBox("Server Failed To Start")
            Exit Sub
        End Try
        'hubContext = GlobalHost.ConnectionManager.GetHubContext(Of MyHub)()
        WriteToConsole("Server Started @ " & SERVERURL)
        Debug.Print("Running..." & Thread.CurrentThread.ManagedThreadId.ToString & "|" & Me.Name)
    End Sub

    Public Sub WriteToConsole(message As String)
        If Me.InvokeRequired Then
            Me.Invoke(New UpdateTxt(AddressOf WriteToConsole), {message})
        Else
            'TextBox1.SafeInvoke(Function(y As TextBox) InlineAssignHelper(y.Text, y.Text & message & vbCrLf))
            TextBox1.Text += message & vbCrLf
        End If

    End Sub

    Private Sub frmServer_Load(sender As Object, e As EventArgs) Handles Me.Load
        MainForm = Me
        Task.Run(AddressOf Init)    '// launch server on its own thread


        'Dim context = GlobalHost.ConnectionManager.GetHubContext(Of MyHub)()
        'context.Clients..updateClock(DateTime.UtcNow)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SendToClients("From Server", "https://pbs.twimg.com/profile_images/415128901115867137/j5SIBgWo.jpeg")

        


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SendToClients("From Server", "https://www.google.com/images/srpr/logo11w.png")
    End Sub

    Private Sub SendToClients(message As String, payload As String)
        If Hub IsNot Nothing Then
            Hub.Send(message, payload)
            WriteToConsole("Server Sending " & message & "|" & payload)
        Else
            WriteToConsole("No Clients to send to.")
        End If
    End Sub
End Class

Class Startup
    Public Sub Configuration(app As IAppBuilder)
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

Class StockTicker

End Class