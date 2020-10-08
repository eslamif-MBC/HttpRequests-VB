Imports System.EnterpriseServices
Imports System.Net
Imports System.Net.Http
Imports Newtonsoft.Json

''' <summary>
''' HTTP GET Requests with HttpClient. Request cannot have content or content-type set
''' Usage: Dim response = (New GetRequest).MakeGetRequest(url)
''' </summary>
Public Class GetRequest
    Public RequestMimeType As String = "application/json"
    Public AcceptMimeType As String = "application/json"
    Public RequestEncoding As Encoding = Encoding.UTF8

    ''' <summary>
    ''' Make HTTP GET request with no authorization
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
    Public Function MakeGetRequest(ByVal url As String) As Tuple(Of HttpStatusCode, String)
        If String.IsNullOrEmpty(url) Then
            Debug.WriteLine("GetService(): invalid arguments.")
            Return Nothing
        End If

        Dim request As HttpRequestMessage = CreateHttpRequestMessage(url)
        Return CallService(request)
    End Function

    ''' <summary>
    ''' Make HTTP GET request with basic auth
    ''' </summary>
    ''' <param name="url"></param>
    ''' <param name="username"></param>
    ''' <param name="password"></param>
    ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
    Public Function MakeGetRequest(ByVal url As String, ByVal username As String, ByVal password As String) As Tuple(Of HttpStatusCode, String)
        If String.IsNullOrEmpty(url) OrElse String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Debug.WriteLine("GetService(): invalid arguments.")
            Return Nothing
        End If

        Dim request As HttpRequestMessage = CreateHttpRequestMessage(url)
        SetBasicAuthHeaders(request, username, password)
        Return CallService(request)
    End Function

    Private Function CreateHttpRequestMessage(ByVal url As String) As HttpRequestMessage
        Dim request = New HttpRequestMessage()
        request.Method = HttpMethod.Get
        request.RequestUri = New Uri(url)
        request.Headers.Accept.Add(New Headers.MediaTypeWithQualityHeaderValue(AcceptMimeType))
        Return request
    End Function

    Private Sub SetBasicAuthHeaders(ByVal request As HttpRequestMessage, ByVal username As String, ByVal password As String)
        Dim authenticationString As String = $"{username}:{password}"
        Dim base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(authenticationString))
        request.Headers.Add("Authorization", "Basic " & base64EncodedAuthenticationString)
    End Sub

    Private Function CallService(ByVal request As HttpRequestMessage) As Tuple(Of HttpStatusCode, String)
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim client As HttpClient = New HttpClient()
        Dim httpResponse = client.SendAsync(request)
        httpResponse.Wait()

        If (httpResponse.Result.IsSuccessStatusCode) Then
            Dim jsonResponse = httpResponse.Result.Content.ReadAsStringAsync()
            Dim jsonResponseDeserialized = JsonConvert.DeserializeObject(jsonResponse.Result).ToString()
            Return New Tuple(Of HttpStatusCode, String)(httpResponse.Result.StatusCode, jsonResponseDeserialized)
        Else
            Debug.WriteLine("Error: MakeGetRequest() failed: " + httpResponse.Result.StatusCode.ToString())
            Return Nothing
        End If
    End Function
End Class
