Imports System
Imports Microsoft.AspNet.SignalR

Imports Microsoft.Owin.Hosting
Imports Owin
Imports Microsoft.Owin.Cors
Imports System.Threading
Imports Microsoft.Owin.StaticFiles
Imports Microsoft.Owin.FileSystems
Imports System.IO
Imports System.Diagnostics.Contracts

Public Class frmServer
    'Const SERVERURL As String = "http://localhost:8080"
    Const SERVERURL As String = "http://+:8080"
    Private SignalR As IDisposable
    Private Delegate Sub UpdateTxt(message As String)

    '// a local reference to the instance of the Hub that clients are using
    Public Hub As MyHub

#Region "Startup and Cleanup"

    Private Sub frmServer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If SignalR IsNot Nothing Then
            SignalR.Dispose()
        End If
    End Sub

    Private Sub frmServer_Load(sender As Object, e As EventArgs) Handles Me.Load
        MainForm = Me
        Task.Run(AddressOf Init)    '// launch server on its own thread
    End Sub

#End Region

#Region "Web Server Startup"
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

#End Region

#Region "GUI Related"
    Public Sub WriteToConsole(message As String)
        If Me.InvokeRequired Then
            Me.Invoke(New UpdateTxt(AddressOf WriteToConsole), {message})
        Else
            'TextBox1.SafeInvoke(Function(y As TextBox) InlineAssignHelper(y.Text, y.Text & message & vbCrLf))
            TextBox1.Text += message & vbCrLf
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SendToClients("From Server", "https://pbs.twimg.com/profile_images/415128901115867137/j5SIBgWo.jpeg")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SendToClients("From Server", "https://www.google.com/images/srpr/logo11w.png")
    End Sub
#End Region
    
#Region "SignalR"
    Private Sub SendToClients(message As String, payload As String)
        If Hub IsNot Nothing Then
            Hub.Send(message, payload)
            WriteToConsole("Server Sending " & message & "|" & payload)
        Else
            WriteToConsole("No Clients to send to.")
        End If
    End Sub
#End Region


    
End Class


