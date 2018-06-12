﻿Public Class IFconditions
    Public Shared Function start(line As String, code As String, linescounter As Long)
        If FormatConverters.removeSpacesAtBeginningAndEnd(line).ToLower.StartsWith("if") Then
            Dim getCondition As String = FormatConverters.ConvertToAbleToRead(FormatConverters.removeSpacesAtBeginningAndEnd(line).Substring(2)).Substring(0, FormatConverters.ConvertToAbleToRead(FormatConverters.removeSpacesAtBeginningAndEnd(line).Substring(2)).LastIndexOf("{"))

            If getCondition.Contains("//") Or getCondition.Contains("><") Then

                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                'examples

                '// = or
                '>< = and

                'if("5" + "5" = "10" >< "8" + "8" = "16"){
                '
                '}

                'if("5" + "5" = "10" // "5" + "8" = "10"){
                '
                '}

                'if("5" + "5" = "10" >< "8" + "8" = "16" // "2" + "2" = "14"){
                '
                '}

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                If Not getCondition.Contains("//") Then
                    If getCondition.Contains("><") Then

                        If verifyandofcondition(getCondition) Then

                            'allow to execute code

                        Else

                            Dim lineStart As Long = linescounter
                            Dim lineStop As Long = 0
                            Dim ouvrantes As Long = 0
                            Dim exitforsecond As Boolean = False

                            For countLines As Long = lineStart + 1 To code.Split(ControlChars.Lf).Count - 1

                                Dim i As String = FormatConverters.removeSpacesAtBeginningAndEnd(code.Split(ControlChars.Lf)(countLines))

                                For Each countchar As String In i

                                    If countchar = "{" Then
                                        ouvrantes += 1
                                    ElseIf countchar = "}" Then
                                        If ouvrantes > 0 Then
                                            ouvrantes -= 1
                                        Else
                                            lineStop = countLines
                                            exitforsecond = True
                                            Exit For
                                        End If
                                    End If

                                Next

                                If exitforsecond = True Then
                                    Exit For
                                End If

                            Next

                            Return Long.Parse(lineStop - 1).ToString
                        End If
                    End If

                ElseIf getCondition.Contains("//") Then

                    Dim amountofor As Long = 0
                    Dim amountOfTrueConditions As Long = 0

                    For Each countAmountOfOr As String In getCondition.Split("//")

                        If Not countAmountOfOr = "" Then
                            amountofor += 1
                        End If

                    Next

                    For Each i As String In getCondition.Split("//")

                        If Not i = "" Then
                            If verifyandofcondition(i) Then
                                amountOfTrueConditions += 1
                            End If
                        End If

                    Next

                    If Not amountOfTrueConditions > 0 Then

                        Dim lineStart As Long = linescounter
                        Dim lineStop As Long = 0
                        Dim ouvrantes As Long = 0
                        Dim exitforsecond As Boolean = False

                        For countLines As Long = lineStart + 1 To code.Split(ControlChars.Lf).Count - 1

                            Dim i As String = FormatConverters.removeSpacesAtBeginningAndEnd(code.Split(ControlChars.Lf)(countLines))

                            For Each countchar As String In i

                                If countchar = "{" Then
                                    ouvrantes += 1
                                ElseIf countchar = "}" Then

                                    If ouvrantes > 0 Then
                                        ouvrantes -= 1
                                    Else
                                        lineStop = countLines
                                        exitforsecond = True
                                        Exit For
                                    End If

                                End If

                            Next

                            If exitforsecond = True Then
                                Exit For
                            End If

                        Next

                        Return Long.Parse(lineStop - 1).ToString
                    Else
                        'allow to execute code
                    End If
                End If
            Else
                Return startcondition(getCondition, code, line, linescounter)
            End If
        End If
    End Function
    Public Shared Function verifyandofcondition(getconditionOriginal As String) As Boolean
        Dim listOfVerifyConditions As New ListBox
        Dim counterOfCondition As Long = 0

        Dim getCondition As String = getconditionOriginal
        getCondition = getCondition.Replace("><", "[[AND_separator]]").Replace(">", "{{greater_than}}") _
            .Replace("<", "{{minus_than}}").Replace(">=", "{{greater_or_equal_than}}") _
            .Replace("<=", "{{minus_or_equal_than}}")

        For Each i As String In getCondition.Split("[[AND_separator]]")

            If Not FormatConverters.isNothingOrSpace(i) Then

                '> : {{greater_than}}
                '< : {{minus_than}}
                '>= : {{greater_or_equal_than}}
                '<= : {{minus_or_equal_than}}

                Dim conditionPartOriginal As String = i

                If conditionPartOriginal.StartsWith("AND_separator]]") Then
                    conditionPartOriginal = conditionPartOriginal.Substring(15)
                End If

                If counterOfCondition = getCondition.Split("><").Count - 1 Then
                        conditionPartOriginal = conditionPartOriginal
                    End If

                    If conditionPartOriginal.StartsWith("<") Then
                        conditionPartOriginal = conditionPartOriginal.Substring(1)
                    End If

                    conditionPartOriginal = FormatConverters.removeSpacesAtBeginningAndEnd(FormatConverters.ConvertToAbleToRead(conditionPartOriginal)) _
                    .Replace("{{greater_than}}", ">").Replace("{{minus_than}}", "<").Replace("{{greater_or_equal_than}}", ">=") _
                    .Replace("{{minus_or_equal_than}}", "<=")

                listOfVerifyConditions.Items.Add(verifycondition(conditionPartOriginal))

                End If

                counterOfCondition += 1
        Next

        For Each i2 As String In listOfVerifyConditions.Items

            If i2 = "0" Then
                Return False
                Exit Function
            End If

        Next

        Return True
    End Function
    Public Shared Function verifycondition(getconditions As String)
        If FormatConverters.getExpression(FormatConverters.ConvertToAbleToRead(getconditions)) = "1" Then
            Return "1"
            Exit Function
        ElseIf FormatConverters.getExpression(FormatConverters.ConvertToAbleToRead(getconditions)) = "0" Then
            Return "0"
            Exit Function
        End If

        Dim getcondition As String = getconditions
        Dim getfirstobject As String = ""
        Dim getsecondobject As String = ""


        If getcondition.Contains("!=") Then

            getfirstobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split("!=")(0)))
            getsecondobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split("!=")(1).Substring(1).Substring(0, getcondition.Split("!=")(1).Substring(1).Length)))

            If getfirstobject <> getsecondobject Then
                Return "1"
            Else
                Return "0"
            End If

        ElseIf getcondition.Contains("<=") Then

            getfirstobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split("<=")(0)))
            getsecondobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split("<=")(1).Substring(1).Substring(0, getcondition.Split("<=")(1).Substring(1).Length)))

            If Long.Parse(getfirstobject) <= Long.Parse(getsecondobject) Then
                Return "1"
            Else
                Return "0"
            End If

        ElseIf getcondition.Contains(">=") Then

            getfirstobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split(">=")(0)))
            getsecondobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split(">=")(1).Substring(1).Substring(0, getcondition.Split(">=")(1).Substring(1).Length)))


            If Long.Parse(getfirstobject) >= Long.Parse(getsecondobject) Then
                Return "1"
            Else
                Return "0"
            End If

        ElseIf getcondition.Contains("=") Then

            getfirstobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split("=")(0)))
            getsecondobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split("=")(1).Substring(1).Substring(0, getcondition.Split("=")(1).Substring(1).Length)))

            If getfirstobject = getsecondobject Then
                Return "1"
            Else
                Return "0"
            End If

        ElseIf getcondition.Contains("<") Then

            getfirstobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split("<")(0)))
            getsecondobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split("<")(1).Substring(1).Substring(0, getcondition.Split("<")(1).Substring(1).Length)))

            If Long.Parse(getfirstobject) < Long.Parse(getsecondobject) Then
                Return "1"
            Else
                Return "0"
            End If

            Return ""

        ElseIf getcondition.Contains(">") Then

            getfirstobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split(">")(0)))
            getsecondobject = FormatConverters.getExpression(FormatConverters.removeSpacesAtBeginningAndEnd(getcondition.Split(">")(1).Substring(1).Substring(0, getcondition.Split(">")(1).Substring(1).Length)))

            If Long.Parse(getfirstobject) > Long.Parse(getsecondobject) Then
                Return "1"
            Else
                Return "0"
            End If

        Else
            Return ""
        End If
    End Function
    Public Shared Function startcondition(getcondition As String, code As String, line As String, linescounter As String)
        If Not verifycondition(getcondition) = "1" Then

            Dim lineStart As Long = linescounter
            Dim lineStop As Long = 0
            Dim ouvrantes As Long = 0
            Dim exitforsecond As Boolean = False

            For countLines As Long = lineStart + 1 To code.Split(ControlChars.Lf).Count - 1

                Dim i As String = FormatConverters.removeSpacesAtBeginningAndEnd(code.Split(ControlChars.Lf)(countLines))

                For Each countchar As String In i

                    If countchar = "{" Then
                        ouvrantes += 1
                    ElseIf countchar = "}" Then

                        If ouvrantes > 0 Then
                            ouvrantes -= 1
                        Else
                            lineStop = countLines
                            exitforsecond = True
                            Exit For
                        End If

                    End If

                Next

                If exitforsecond = True Then
                    Exit For
                End If
            Next

            Return Long.Parse(lineStop - 1).ToString
            Exit Function

        End If

        Return "abc"
    End Function
End Class