Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports Newtonsoft.Json

Namespace Utility

    Public Class HttpRequests
        Public RequestMimeType As String = "application/json"
        Public AcceptMimeType As String = "application/json"
        Public RequestEncoding As Encoding = Encoding.UTF8

#Region "GET Requests"
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
#End Region

#Region "POST Requests"
        ''' <summary>
        ''' HTTP Post request without authorization
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <param name="contentArguments"></param>
        ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
        Public Function MakePostRequest(ByVal uri As String, ByVal contentArguments As String) As Tuple(Of HttpStatusCode, String)
            If String.IsNullOrEmpty(uri) OrElse String.IsNullOrEmpty(contentArguments) Then
                Debug.WriteLine("MakePostRequest(): invalid arguments.")
                Return Nothing
            End If

            Dim request As HttpRequestMessage = CreateHttpRequestMessage(uri, contentArguments)
            Return CallService(request)
        End Function

        ''' <summary>
        ''' HTTP Post request with basic auth
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <param name="contentArguments"></param>
        ''' <param name="username"></param>
        ''' <param name="password"></param>
        ''' <returns>Tuple: item1 = HttpStatusCode, item2 = response</returns>
        Public Function MakePostRequestWithBasicAuth(ByVal uri As String, ByVal contentArguments As String, ByVal username As String, ByVal password As String) As Tuple(Of HttpStatusCode, String)
            If String.IsNullOrEmpty(uri) OrElse String.IsNullOrEmpty(contentArguments) OrElse String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
                Debug.WriteLine("MakePostRequestWithBasicAuth(): invalid arguments.")
                Return Nothing
            End If

            Dim request As HttpRequestMessage = CreateHttpRequestMessage(uri, contentArguments)
            SetBasicAuthHeaders(request, username, password)
            Return CallService(request)
        End Function

        Public Function MakePostRequestWithAppIdAndKey(ByVal uri As String, ByVal contentArguments As String, ByVal appId As String, ByVal appKey As String) As Tuple(Of HttpStatusCode, String)
            If String.IsNullOrEmpty(uri) OrElse String.IsNullOrEmpty(contentArguments) OrElse String.IsNullOrEmpty(appId) OrElse String.IsNullOrEmpty(appKey) Then
                Debug.WriteLine("MakePostRequestWithAppIdAndKey(): invalid arguments.")
                Return Nothing
            End If

            Dim request As HttpRequestMessage = CreateHttpRequestMessage(uri, contentArguments)
            SetAppIdAndAppKeyHeaders(request, appId, appKey)
            Return CallService(request)
        End Function
#End Region

#Region "HttpClient Methods"
        Private Function CreateHttpRequestMessage(ByVal uri As String, ByVal contentArguments As String) As HttpRequestMessage
            Dim httpContent = New StringContent(contentArguments, RequestEncoding, RequestMimeType)

            Dim request = New HttpRequestMessage()
            request.Method = HttpMethod.Post
            request.Content = httpContent
            request.RequestUri = New Uri(uri)
            request.Headers.Accept.Add(New Headers.MediaTypeWithQualityHeaderValue(AcceptMimeType))

            Return request
        End Function

        Private Sub SetBasicAuthHeaders(ByRef request As HttpRequestMessage, ByVal username As String, ByVal password As String)
            'Dim authenticationString As String = $"{username}:{password}"
            Dim sb = New StringBuilder()
            sb.Append(username)
            sb.Append(":")
            sb.Append(password)
            Dim authenticationString = sb.ToString()

            Dim base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(authenticationString))
            request.Headers.Add("Authorization", "Basic " & base64EncodedAuthenticationString)
        End Sub

        Private Sub SetAppIdAndAppKeyHeaders(ByRef request As HttpRequestMessage, ByVal appId As String, ByVal appKey As String)
            request.Headers.Add("APP_ID", appId)
            request.Headers.Add("APP_KEY", appKey)
        End Sub

        Private Function CallService(ByVal request As HttpRequestMessage) As Tuple(Of HttpStatusCode, String)
            Try
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                Dim client As HttpClient = New HttpClient()
                Dim httpResponse = client.SendAsync(request)
                httpResponse.Wait()

                If (httpResponse.Result.IsSuccessStatusCode) Then
                    Dim jsonResponse = httpResponse.Result.Content.ReadAsStringAsync()
                    Dim jsonResponseDeserialized = JsonConvert.DeserializeObject(jsonResponse.Result).ToString()
                    Return New Tuple(Of HttpStatusCode, String)(httpResponse.Result.StatusCode, jsonResponseDeserialized)
                Else
                    Debug.WriteLine("Error: CallService() failed: " + httpResponse.Result.StatusCode.ToString())
                    Return Nothing
                End If
            Catch ex As HttpRequestException
                Using thisEventLog = New EventLog("Application")

                    Dim errorMessage = "HTTPRequest's CallService() HttpRequestException: " + Helper.GetExceptionMessages(ex)
                    thisEventLog.Source = "Application"
                    thisEventLog.WriteEntry(errorMessage, EventLogEntryType.Error)

                    Debug.WriteLine(errorMessage)
                    Return Nothing
                End Using
            Catch ex As HttpException
                Using thisEventLog = New EventLog("Application")

                    Dim errorMessage = "HTTPRequest's CallService() HttpException: " + Helper.GetExceptionMessages(ex)
                    thisEventLog.Source = "Application"
                    thisEventLog.WriteEntry(errorMessage, EventLogEntryType.Error)

                    Debug.WriteLine(errorMessage)
                    Return Nothing
                End Using
            Catch ex As Exception
                Using thisEventLog = New EventLog("Application")

                    Dim errorMessage = "HTTPRequest's CallService() Exception: " + Helper.GetExceptionMessages(ex)
                    thisEventLog.Source = "Application"
                    thisEventLog.WriteEntry(errorMessage, EventLogEntryType.Error)

                    Debug.WriteLine(errorMessage)
                    Return Nothing
                End Using
            End Try
        End Function
#End Region

#Region "WebClient Methods"
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
                Debug.WriteLine("Error: CallService() failed. " & ex.Message)
                Return New Tuple(Of HttpStatusCode, String)(HttpStatusCode.InternalServerError, Nothing)
            End Try
        End Function
#End Region

    End Class
End Namespace
