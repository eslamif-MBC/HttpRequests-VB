Imports System.Net
Imports System.Net.Http

Public Class PostRequest
    Public RequestMimeType As String = "application/json"
    Public RequestEncoding As Encoding = Encoding.UTF8
    Public AcceptMimeType As String = "application/json"

    ''' <summary>
    ''' Make POST request to URI without authorization.
    ''' </summary>
    ''' <param name="uri"></param>
    ''' <param name="queryString"></param>
    ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
    Public Overridable Function Post(ByVal uri As String, ByVal queryString As String) As Tuple(Of HttpStatusCode, String)
        If String.IsNullOrEmpty(uri) OrElse String.IsNullOrEmpty(queryString) Then
            Debug.WriteLine("Post(): invalid arguments.")
            Return Nothing
        End If

        Dim request As HttpRequestMessage = CreateHttpRequestMessage(uri, queryString)
        Return PostRequest(request)
    End Function

    Protected Function CreateHttpRequestMessage(ByVal uri As String, ByVal queryString As String) As HttpRequestMessage
        Dim httpContent = New StringContent(queryString, RequestEncoding, RequestMimeType)

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
    Protected Function PostRequest(ByVal request As HttpRequestMessage) As Tuple(Of HttpStatusCode, String)
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim client As HttpClient = New HttpClient()
        Dim httpResponse = client.SendAsync(request)
        httpResponse.Wait()

        If (httpResponse.Result.IsSuccessStatusCode) Then
            Dim response = httpResponse.Result.Content.ReadAsStringAsync()
            Return New Tuple(Of HttpStatusCode, String)(httpResponse.Result.StatusCode, response.Result)
        Else
            Debug.WriteLine("Error: PostRequest() to failed: " + httpResponse.Result.StatusCode.ToString())
            Return Nothing
        End If
    End Function

End Class

