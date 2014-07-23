Imports Owin
Imports Microsoft.Owin.StaticFiles
Imports System.IO
Imports Microsoft.AspNet.SignalR
Imports System.Diagnostics.Contracts
Imports Microsoft.Owin.FileSystems
Imports Microsoft.Owin.Cors

'// auto called by signal when the webserver initiates.
Class Startup

    Public Sub Configuration(app As IAppBuilder)
        '// set the default paths for html files and scripts
        app.UseFileServer(New FileServerOptions() With {.FileSystem = New PhysicalFileSystem(GetRootDirectory()), .EnableDirectoryBrowsing = True, .RequestPath = New Microsoft.Owin.PathString("/html")})
        app.UseFileServer(New FileServerOptions() With {.FileSystem = New PhysicalFileSystem(GetScriptsDirectory()), .EnableDirectoryBrowsing = True, .RequestPath = New Microsoft.Owin.PathString("/scripts")})
        app.UseCors(CorsOptions.AllowAll)
        '// set the path to SignalR
        app.MapSignalR()
    End Sub


    Private Shared Function GetRootDirectory() As String
        Dim currentDirectory = Directory.GetCurrentDirectory()
        Dim rootDirectory = Directory.GetParent(currentDirectory).Parent
        Contract.Assume(rootDirectory IsNot Nothing)
        Return Path.Combine(rootDirectory.FullName, "WebContent")
    End Function

    Private Shared Function GetScriptsDirectory() As String
        Dim currentDirectory = Directory.GetCurrentDirectory()
        Dim rootDirectory = Directory.GetParent(currentDirectory).Parent
        Contract.Assume(rootDirectory IsNot Nothing)
        Return Path.Combine(rootDirectory.FullName, "Scripts")
    End Function


End Class

'// the class that communicates with the web client
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
