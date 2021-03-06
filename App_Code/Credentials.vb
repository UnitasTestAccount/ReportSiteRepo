﻿Imports System.Data
Imports System.Configuration
Imports System.Net
Imports System.Security.Principal
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports Microsoft.Reporting.WebForms

Public Class CustomReportCredentials
    Implements Microsoft.Reporting.WebForms.IReportServerCredentials

    ' local variable for network credential.
    Private _UserName As String
    Private _PassWord As String
    Private _DomainName As String
    Public Sub New(ByVal UserName As String, ByVal PassWord As String, ByVal DomainName As String)
        _UserName = System.Configuration.ConfigurationManager.AppSettings("UserName").ToString()
        _PassWord = System.Configuration.ConfigurationManager.AppSettings("PassWord").ToString()
        _DomainName = System.Configuration.ConfigurationManager.AppSettings("DomainName").ToString()
    End Sub
    Public ReadOnly Property ImpersonationUser() As WindowsIdentity Implements IReportServerCredentials.ImpersonationUser
        Get
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property NetworkCredentials() As ICredentials Implements IReportServerCredentials.NetworkCredentials
        Get
            Return New NetworkCredential(_UserName, _PassWord, _DomainName)
        End Get
    End Property
    Public Function GetFormsCredentials(ByRef authCookie As Cookie, ByRef user As String, ByRef password As String, ByRef authority As String) As Boolean Implements IReportServerCredentials.GetFormsCredentials

        authCookie = Nothing
        user = InlineAssignHelper(password, InlineAssignHelper(authority, Nothing))
        Return False
    End Function
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function
End Class