Public Class Helper

    ''' <summary>
    ''' Returns the Exception messages
    ''' Usage: 
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <returns></returns>
    Public Shared Function GetExceptionMessages(ex As Exception) As String
        Dim message = New StringBuilder()
        message.Append(ex.Message).Append(". ")

        'Message
        If ex.TargetSite IsNot Nothing Then
            message.Append(ex.TargetSite.ToString()).Append(". ")
        End If

        'InnerException.Message
        If (ex IsNot Nothing AndAlso ex.InnerException IsNot Nothing AndAlso Not String.IsNullOrEmpty(ex.InnerException.Message)) Then
            message.Append(ex.InnerException.Message).Append(". ")

            'InnerException.InnerException.Message
            If (ex.InnerException.InnerException IsNot Nothing AndAlso Not String.IsNullOrEmpty(ex.InnerException.InnerException.Message)) Then
                message.Append(ex.InnerException.InnerException.Message).Append(". ")
            End If

        End If

        'StackTrace
        If Not String.IsNullOrEmpty(ex.StackTrace) Then
            message.Append(ex.StackTrace).Append(". ")
        End If

        Return message.ToString()
    End Function
End Class
