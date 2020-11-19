''' <summary>
''' Examples hit the Postman Echo:
''' https://docs.postman-echo.com/
''' </summary>
Public Class HttpRequestExamples
    Public Sub MakeGetRequestExample()
        Dim url = "https://postman-echo.com/get?foo1=bar1&foo2=bar2"

        Dim response = (New Utility.HttpRequests).MakeGetRequest(url)
        Debug.WriteLine(response.Item1)
        Debug.WriteLine(response.Item2)
    End Sub

    Public Sub MakeGetRequestWithBasicAuthExample()
        Dim url = "https://postman-echo.com/basic-auth"
        Dim username = "postman"
        Dim password = "password"

        Dim response = (New Utility.HttpRequests).MakeGetRequest(url, username, password)
        Debug.WriteLine(response.Item1)
        Debug.WriteLine(response.Item2)
    End Sub

    Public Sub MakePostRequestExample()
        Dim uri = "https://postman-echo.com/post"
        Dim carMake = "Toyota"
        Dim carModel = "Celica"

        '{"carMake":"Toyota","carModel":"Celica"}
        Dim sb = New StringBuilder()
        sb.Append("{")
        sb.Append("""carMake""")
        sb.Append(":")
        sb.Append("""").Append(carMake).Append("""")
        sb.Append(",")
        sb.Append("""carModel""")
        sb.Append(":")
        sb.Append("""").Append(carModel).Append("""")
        sb.Append("}")

        Dim response = (New Utility.HttpRequests).MakePostRequest(uri, sb.ToString())
        Debug.WriteLine(response.Item1)
        Debug.WriteLine(response.Item2)
    End Sub

    Public Sub MakePostRequestWithAppIdAndKeyExample()
        Dim uri = ""
        Dim appId = ""
        Dim appKey = ""
        Dim carMake = "Toyota"
        Dim carModel = "Celica"

        '{"carMake":"Toyota","carModel":"Celica"}
        Dim sb = New StringBuilder()
        sb.Append("{")
        sb.Append("""carMake""")
        sb.Append(":")
        sb.Append("""").Append(carMake).Append("""")
        sb.Append(",")
        sb.Append("""carModel""")
        sb.Append(":")
        sb.Append("""").Append(carModel).Append("""")
        sb.Append("}")

        Dim response = (New Utility.HttpRequests).MakePostRequestWithAppIdAndKey(uri, sb.ToString(), appId, appKey)
        Debug.WriteLine(response.Item1)
        Debug.WriteLine(response.Item2)
    End Sub
End Class
