Imports System.Data
Imports System.Data.SqlClient
Imports GenerateReport
Imports Microsoft.Reporting.WebForms

Partial Class Reporting
    Inherits System.Web.UI.Page
    ' Im adding in another commented out line to check that it works.
    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim dsName As String

    '    dsName = "select * from feeddeliveries"
    '    Dim dt As New DataTable()
    '    Dim con As New SqlConnection("Data Source = WALDORF; Initial Catalog = pm_abagri; User ID = sa; Password = Un1p455!;")
    '    Dim cmd As New SqlCommand(dsName, con)
    '    Dim da As New SqlDataAdapter(cmd)
    '    da.Fill(dt)

    '    Dim gen As New GenerateReport.ReportGenerator(dt, dsName)
    '    Dim ds As New ReportDataSource(dsName, dt)
    '    ReportViewer1.Reset()
    '    ReportViewer1.LocalReport.DataSources.Add(ds)
    '    ReportViewer1.LocalReport.DisplayName = "Test"
    '    ReportViewer1.LocalReport.LoadReportDefinition(gen.GenerateReport())
    'End Sub

End Class
