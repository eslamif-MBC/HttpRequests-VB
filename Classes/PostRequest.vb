Imports System.Net
Imports System.Net.Http
Imports Newtonsoft.Json

''' <summary>
''' HTTP POST request with or without basic auth
''' Usage: Dim response = (New PostRequest).MakePostRequest(uri, contentArguments)
''' </summary>
Public Class PostRequest
    Public RequestMimeType As String = "application/json"
    Public RequestEncoding As Encoding = Encoding.UTF8
    Public AcceptMimeType As String = "application/json"

    ''' <summary>
    ''' Make POST request without authorization.
    ''' </summary>
    ''' <param name="uri"></param>
    ''' <param name="contentArguments"></param>
    ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
    Public Overridable Function MakePostRequest(ByVal uri As String, ByVal contentArguments As String) As Tuple(Of HttpStatusCode, String)
        If String.IsNullOrEmpty(uri) OrElse String.IsNullOrEmpty(contentArguments) Then
            Debug.WriteLine("Post(): invalid arguments.")
            Return Nothing
        End If

        Dim request As HttpRequestMessage = CreateHttpRequestMessage(uri, contentArguments)
        Return PostRequest(request)
    End Function

    ''' <summary>
    ''' Make POST request basic auth
    ''' </summary>
    ''' <param name="uri"></param>
    ''' <param name="contentArguments"></param>
    ''' <param name="username"></param>
    ''' <param name="password"></param>
    ''' <returns></returns>
    Public Function Post(ByVal uri As String, ByVal contentArguments As String, ByVal username As String, ByVal password As String) As Tuple(Of HttpStatusCode, String)
        If String.IsNullOrEmpty(uri) OrElse String.IsNullOrEmpty(contentArguments) OrElse String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Debug.WriteLine("Post(): invalid arguments.")
            Return Nothing
        End If

        Dim request As HttpRequestMessage = CreateHttpRequestMessage(uri, contentArguments)
        SetBasicAuthHeaders(request, username, password)
        Return PostRequest(request)
    End Function

    Private Sub SetBasicAuthHeaders(ByVal request As HttpRequestMessage, ByVal username As String, ByVal password As String)
        Dim authenticationString As String = $"{username}:{password}"
        Dim base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(authenticationString))
        request.Headers.Add("Authorization", "Basic " & base64EncodedAuthenticationString)
    End Sub

    Private Function CreateHttpRequestMessage(ByVal uri As String, ByVal contentArguments As String) As HttpRequestMessage
        Dim httpContent = New StringContent(contentArguments, RequestEncoding, RequestMimeType)

        Dim request = New HttpRequestMessage()
        request.Content = httpContent
        request.Method = HttpMethod.Post
        request.RequestUri = New Uri(uri)
        request.Headers.Accept.Add(New Headers.MediaTypeWithQualityHeaderValue(AcceptMimeType))

        Return request
    End Function

    ''' <summary>
    ''' Post to URI
    ''' </summary>
    ''' <param name="request"></param>
    ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
    Private Function PostRequest(ByVal request As HttpRequestMessage) As Tuple(Of HttpStatusCode, String)
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim client As HttpClient = New HttpClient()
        Dim httpResponse = client.SendAsync(request)
        httpResponse.Wait()

        If (httpResponse.Result.IsSuccessStatusCode) Then
            Dim jsonResponse = httpResponse.Result.Content.ReadAsStringAsync()
            Dim jsonResponseDeserialized = JsonConvert.DeserializeObject(jsonResponse.Result).ToString()
            Return New Tuple(Of HttpStatusCode, String)(httpResponse.Result.StatusCode, jsonResponseDeserialized)
        Else
            Debug.WriteLine("Error: PostRequest() failed: " + httpResponse.Result.StatusCode.ToString())
            Return Nothing
        End If
    End Function

End Class

