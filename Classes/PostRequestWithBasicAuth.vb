Imports System.Net.Http

Public Class PostRequestWithBasicAuth
    Inherits PostRequest

    ''' <summary>
    ''' Make POST request to URI with basic auth
    ''' </summary>
    ''' <param name="uri"></param>
    ''' <param name="queryString"></param>
    ''' <param name="username"></param>
    ''' <param name="password"></param>
    ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
    Public Function PostWithBasicAuth(ByVal uri As String, ByVal queryString As String, ByVal username As String, ByVal password As String) As Tuple(Of String, String)
        If String.IsNullOrEmpty(uri) OrElse String.IsNullOrEmpty(queryString) OrElse String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Debug.WriteLine("PostWithBasicAuth(): invalid arguments.")
            Return Nothing
        End If

        Dim request As HttpRequestMessage = CreateHttpRequestMessage(uri, queryString)
        SetBasicAuthHeaders(request, username, password)
        Return PostRequest(request)
    End Function

    Private Sub SetBasicAuthHeaders(ByVal request As HttpRequestMessage, ByVal username As String, ByVal password As String)
        Dim authenticationString As String = $"{username}:{password}"
        Dim base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(authenticationString))
        request.Headers.Add("Authorization", "Basic " & base64EncodedAuthenticationString)
    End Sub
End Class
