Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Call (New HttpRequestExamples).MakeGetRequestExample()
            Call (New HttpRequestExamples).MakeGetRequestWithBasicAuthExample()
            Call (New HttpRequestExamples).MakePostRequestExample()
        End If
    End Sub
End Class