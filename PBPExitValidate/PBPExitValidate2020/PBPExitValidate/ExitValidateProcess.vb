Public Class ExitValidateProcess
    Private _qid As String
    Private _pbpPath As String
    Private _pbpPlansPath As String
    Private _pbpVBIDPath As String

    Public Sub New(ByVal qid As String, ByVal pbpPath As String, ByVal pbpPlansPath As String, ByVal pbpVBIDPath As String)
        _qid = qid
        _pbpPath = pbpPath
        _pbpPlansPath = pbpPlansPath
        _pbpVBIDPath = pbpVBIDPath
    End Sub
    Public Function GetSectionsToValidate() As List(Of String)
        Dim sections As List(Of String) = New List(Of String)
        sections.Add("WKSHTA")
        sections.Add("WKSHTB1")
        sections.Add("WKSHTB2")
        sections.Add("WKSHTB3")
        sections.Add("WKSHTB4")
        sections.Add("WKSHTB5")
        sections.Add("WKSHTB6")
        sections.Add("WKSHTB7")
        sections.Add("WKSHTB8")
        sections.Add("WKSHTB9")
        sections.Add("WKSHTB10")
        sections.Add("WKSHTB11")
        sections.Add("WKSHTB12")
        sections.Add("WKSHTB13")
        sections.Add("WKSHTB14")
        sections.Add("WKSHTB15")
        sections.Add("WKSHTB16")
        sections.Add("WKSHTB17")
        sections.Add("WKSHTB18")
        sections.Add("WKSHTB19")
        sections.Add("WKSHTB20")
        sections.Add("WKSHTC")
        sections.Add("SCNSETD")
        sections.Add("MRXSS000")
        Return sections
    End Function

    Public Function GetQIDSToValidate() As List(Of String)
        Dim qids As List(Of String) = New List(Of String)
        qids.Add(_qid)
        Return qids
    End Function
    Public Function Process(ByVal resultFile As String)
        Dim resultStr As String
        resultStr = ""
        Dim wrap As PBP2020.PBPWrapper = New PBP2020.PBPWrapper()
        Dim qids As List(Of String) = GetQIDSToValidate()
        Dim sections As List(Of String) = GetSectionsToValidate()
        Try
            For Each qid In qids
                wrap.Initialize(_qid, _pbpPath, _pbpPlansPath, _pbpVBIDPath)
                Dim planType As String = wrap.GetPlanType(_qid)
                Dim sectionStatusArr As String() = wrap.GetSectionStatusList(_qid)
                For Each section In sections
                    Dim coll As Collection = New Collection()
                    Try
                        If GetSectionStatus(section, sectionStatusArr) <> "N/A" Then
                            coll = wrap.ValidateScrnSetA(section, planType)
                        End If
                    Catch ex1 As Exception
                        ex1 = ex1
                        coll = Nothing
                    End Try
                    Dim idx As Integer = 0
                    If Not coll Is Nothing And coll.Count > 1 Then
                        For Each col In coll
                            If idx = 0 Then
                                resultStr = resultStr + col.errorstring.Split(",")(0).Trim()
                                resultStr = resultStr + Environment.NewLine
                                idx = idx + 1
                            Else
                                resultStr = resultStr + col.errorstring
                                resultStr = resultStr + Environment.NewLine
                                resultStr = resultStr + col.description
                                resultStr = resultStr + Environment.NewLine
                                resultStr = resultStr + col.screentitle
                                resultStr = resultStr + Environment.NewLine
                                resultStr = resultStr + "FIELD: " + col.fieldname
                                resultStr = resultStr + Environment.NewLine
                                resultStr = resultStr + "COLUMN: " + col.id
                                resultStr = resultStr + Environment.NewLine
                            End If
                        Next
                    End If
                    If (System.IO.File.Exists(resultFile) <> True) Then
                        System.IO.File.WriteAllText(resultFile, resultStr)
                        resultStr = ""
                    Else
                        System.IO.File.AppendAllText(resultFile, resultStr)
                        resultStr = ""
                    End If
                Next
                resultStr = resultStr + Environment.NewLine
            Next
        Catch ex As Exception
            ex = ex
        End Try
        Return resultStr
    End Function

    Public Function GetSectionStatus(ByRef section As String, ByRef sectionStatusArray As String()) As String
        Dim retval As String = "N/A"
        Select Case section
            Case "WKSHTA"
                retval = "Incomplete"
            Case "WKSHTC"
                retval = Get_Status_BCD(sectionStatusArray(1))
            Case "SCNSETD"
                retval = Get_Status_BCD(sectionStatusArray(2))
            Case "MRXSS000"
                retval = Get_Status_BCD(sectionStatusArray(3))
            Case "WKSHTB1"
                retval = Get_Status_BCD(sectionStatusArray(4))
            Case "WKSHTB2"
                retval = Get_Status_BCD(sectionStatusArray(5))
            Case "WKSHTB3"
                retval = Get_Status_BCD(sectionStatusArray(6))
            Case "WKSHTB4"
                retval = Get_Status_BCD(sectionStatusArray(7))
            Case "WKSHTB5"
                retval = Get_Status_BCD(sectionStatusArray(8))
            Case "WKSHTB6"
                retval = Get_Status_BCD(sectionStatusArray(9))
            Case "WKSHTB7"
                retval = Get_Status_BCD(sectionStatusArray(10))
            Case "WKSHTB8"
                retval = Get_Status_BCD(sectionStatusArray(11))
            Case "WKSHTB9"
                retval = Get_Status_BCD(sectionStatusArray(12))
            Case "WKSHTB10"
                retval = Get_Status_BCD(sectionStatusArray(13))
            Case "WKSHTB11"
                retval = Get_Status_BCD(sectionStatusArray(14))
            Case "WKSHTB12"
                retval = Get_Status_BCD(sectionStatusArray(15))
            Case "WKSHTB13"
                retval = Get_Status_BCD(sectionStatusArray(16))
            Case "WKSHTB14"
                retval = Get_Status_BCD(sectionStatusArray(17))
            Case "WKSHTB15"
                retval = Get_Status_BCD(sectionStatusArray(18))
            Case "WKSHTB16"
                retval = Get_Status_BCD(sectionStatusArray(19))
            Case "WKSHTB17"
                retval = Get_Status_BCD(sectionStatusArray(20))
            Case "WKSHTB18"
                retval = Get_Status_BCD(sectionStatusArray(21))
            Case "WKSHTB19"
                retval = Get_Status_BCD(sectionStatusArray(22))
            Case "WKSHTB20"
                retval = Get_Status_BCD(sectionStatusArray(23))
        End Select
        Return retval
    End Function

    Private Function Get_Status_BCD(statusCode As String) As String
        Dim result As String = "N/A"
        If statusCode Is Nothing Then
            result = "N/A"
        Else
            If statusCode = "1" Then
                result = "New"
            ElseIf statusCode = "2" Then
                result = "Incomplete"
            ElseIf statusCode = "3" Then
                result = "Completed"
            ElseIf statusCode = "4" Then
                result = "Ready to upload"
            Else
                result = "N/A"
            End If
        End If
        Return result
    End Function

End Class
