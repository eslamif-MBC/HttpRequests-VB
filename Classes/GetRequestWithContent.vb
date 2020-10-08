Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json

''' <summary>
''' HTTP GET requests with WebClient with content-type set
''' Usage 1: Dim response = (New GetRequestWithContent).MakeGetRequest(url, username, password)
''' Usage 2: Dim response = (New GetRequestWithContent).MakeGetRequest(url)
''' </summary>
Public Class GetRequestWithContent
    Public RequestMimeType As String = "application/json"
    Public AcceptMimeType As String = "application/json"

    ''' <summary>
    ''' Make HTTP GET request with content-type set and no authorization
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
    Public Function MakeGetRequest(ByVal url As String) As Tuple(Of HttpStatusCode, String)
        If String.IsNullOrEmpty(url) Then
            Debug.WriteLine("MakeGetRequest(): invalid arguments.")
            Return Nothing
        End If

        Dim request As WebClient = CreateWebClientRequest()
        Return CallService(request, url)
    End Function

    ''' <summary>
    ''' Make HTTP Get request with content-type set and with basic authorization
    ''' </summary>
    ''' <param name="url"></param>
    ''' <param name="username"></param>
    ''' <param name="password"></param>
    ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
    Public Function MakeGetRequest(ByVal url As String, ByVal username As String, ByVal password As String) As Tuple(Of HttpStatusCode, String)
        If String.IsNullOrEmpty(url) OrElse String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Debug.WriteLine("MakeGetRequest(): invalid arguments.")
            Return Nothing
        End If

        Dim request As WebClient = CreateWebClientRequest(username, password)
        Return CallService(request, url)
    End Function

    Private Function CreateWebClientRequest() As WebClient
        Dim request As WebClient = New WebClient()
        request.Headers.Add(HttpRequestHeader.ContentType, RequestMimeType)
        Return request
    End Function

    Private Function CreateWebClientRequest(ByVal username As String, ByVal password As String) As WebClient
        Dim request As WebClient = New WebClient()
        request.Headers.Add(HttpRequestHeader.Authorization, "Basic " & Convert.ToBase64String(Encoding.ASCII.GetBytes(username & ":" & password)))
        request.Headers.Add(HttpRequestHeader.ContentType, RequestMimeType)
        Return request
    End Function

    Private Function CallService(ByVal request As WebClient, ByVal url As String) As Tuple(Of HttpStatusCode, String)
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Try
            Dim responseStream As Stream = request.OpenRead(url)
            Dim reader = New StreamReader(responseStream)
            Dim response = reader.ReadToEnd()

            If response.Length > 2 Then     '2 characters are reserved for empty set brackets
                Dim responseDeserialized = JsonConvert.DeserializeObject(response).ToString()
                Return New Tuple(Of HttpStatusCode, String)(HttpStatusCode.OK, responseDeserialized)
            Else
                Return New Tuple(Of HttpStatusCode, String)(HttpStatusCode.OK, Nothing)
            End If
        Catch ex As Exception
            Return New Tuple(Of HttpStatusCode, String)(HttpStatusCode.InternalServerError, Nothing)
            Debug.WriteLine("Error: CallService() failed. " & ex.Message)
        End Try
    End Function
End Class
