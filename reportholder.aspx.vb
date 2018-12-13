Imports ReportService.ReportingService2010
Imports Microsoft.Reporting.WebForms

Public Class reportholder
    Inherits System.Web.UI.Page

    Private _ParameterDictionary As Dictionary(Of String, String)
    Public Property ParameterDictionary() As Dictionary(Of String, String)
        Get
            Return _ParameterDictionary
        End Get
        Set(ByVal value As Dictionary(Of String, String))
            _ParameterDictionary = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim reportPath As String

        ReportViewer1.KeepSessionAlive = True
        If Not Page.IsPostBack Then
            ReportViewer1.ServerReport.ReportServerCredentials = New CustomReportCredentials("", "", "")

            reportPath = IIf(Request.QueryString("report") Is Nothing, "/TestReports/FCI", Request.QueryString("report"))

            ReportViewer1.ServerReport.ReportPath = reportPath
            Dim parameter As New ReportParameter

            Dim paramCollection As Array
            paramCollection = Request.QueryString.AllKeys

            For Each param As String In paramCollection
                If param <> "report" Then
                    If param.Contains(":isNull") Then
                        param = param.Substring(0, param.Length - 7)
                    End If
                    parameter.Name = param
                    parameter.Values.Clear()
                    parameter.Values.Add(Request.QueryString(param))
                    ReportViewer1.ServerReport.SetParameters(parameter)
                End If
            Next

            ReportViewer1.ShowParameterPrompts = False
            ReportViewer1.ShowPrintButton = True
            ReportViewer1.Width = 1200
            ReportViewer1.Height = 1200
            ReportViewer1.ShowReportBody = True
            ReportViewer1.ShowReportBody = True
            ReportViewer1.Visible = True
            ReportViewer1.ShowParameterPrompts = True
            ReportViewer1.ServerReport.Refresh()
        End If

    End Sub
End Class