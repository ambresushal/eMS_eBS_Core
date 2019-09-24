Module Program
    Sub Main(ByVal cmdArgs() As String)
        'pass QID
        'pass paths for MDB files
        Dim qid As String = ""
        Dim pbpPath As String = ""
        Dim pbpPlansPath As String = ""
        Dim pbpVBIDPath As String = ""
        Dim resultFile As String = ""

        If cmdArgs.Length > 0 Then
            For argNum As Integer = 0 To UBound(cmdArgs, 1)
                If argNum = 0 Then
                    qid = cmdArgs(argNum)
                End If
                If argNum = 1 Then
                    pbpPath = cmdArgs(argNum)
                End If
                If argNum = 2 Then
                    pbpPlansPath = cmdArgs(argNum)
                End If
                If argNum = 3 Then
                    pbpVBIDPath = cmdArgs(argNum)
                End If
                If argNum = 4 Then
                    resultFile = cmdArgs(argNum)
                End If
                ' Insert code to examine cmdArgs(argNum) and take  
                ' appropriate action based on its value.  
            Next argNum
        End If
        Dim ev As ExitValidateProcess = New ExitValidateProcess(qid, pbpPath, pbpPlansPath, pbpVBIDPath)
        Dim resultStr As String
        resultStr = ev.Process()
        System.IO.File.WriteAllText(resultFile, resultStr)
    End Sub
End Module
