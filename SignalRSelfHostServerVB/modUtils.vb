Imports System.ComponentModel
Module Utils

    Public MainForm As frmServer

    Public Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function SafeInvoke(Of T As ISynchronizeInvoke, TResult)(ByRef isi As T, ByRef [call] As Func(Of T, TResult)) As TResult
        If isi.InvokeRequired Then
            Dim result As IAsyncResult = isi.BeginInvoke([call], New Object() {isi})
            Dim endResult As Object = isi.EndInvoke(result)
            Return DirectCast(endResult, TResult)
        Else
            Return [call](isi)
        End If
    End Function
End Module

Module Start
    Public Sub Main()
        Dim MainForm As frmServer = New frmServer()
        Application.Run(MainForm)
    End Sub

End Module
