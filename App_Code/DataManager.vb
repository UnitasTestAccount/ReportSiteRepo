'======================================================================================================================
'Name   : DataManager.vb        Data Access Class
'Author : Paul Logan            Connecting to, querying and amending data in a SQL database.
'Date   : 14th August 2007      http://www.codeproject.com/vb/net/data_accessor_class.asp
'======================================================================================================================
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Collections.Generic
Imports System.Web.Configuration
Imports System.Configuration

Public Class DataManager

    Private DBConnectionSettings As String
    Private dataBaseConnection As SqlConnection
    Property CloseDBConnection As Boolean = True
    Property IsPartOfDBTransaction As Boolean = False
    Property ExpectsReturnValue As Boolean = False
    Property CommandTimeout As Integer = 30

    Public Structure ParameterData
        Dim ParamName As String
        Dim ParamValue As Object
        Dim ParamType As SqlDbType
        Dim ParamDirection As ParameterDirection

        Sub New(Name As String, Value As Object, Type As SqlDbType, Direction As ParameterDirection)
            Me.ParamName = Name
            Me.ParamValue = Value
            Me.ParamType = Type
            Me.ParamDirection = Direction
        End Sub
    End Structure

    Public Sub New()
        'dataBaseConnection = GetConnection()
    End Sub

    Public Sub New(IsDBTransaction As Boolean)
        IsPartOfDBTransaction = IsDBTransaction
    End Sub

    Public Sub New(NonDefaultConnectionString As String)
        DBConnectionSettings = NonDefaultConnectionString
    End Sub

    Public Function GetConnection(dbase As String) As SqlConnection
        If dataBaseConnection Is Nothing Then
            If DBConnectionSettings = String.Empty Then
                DBConnectionSettings = System.Configuration.ConfigurationManager.ConnectionStrings("ubs_" & dbase).ConnectionString
                dataBaseConnection = New SqlConnection(DBConnectionSettings)
            Else
                dataBaseConnection = New SqlConnection(DBConnectionSettings)
            End If
            dataBaseConnection.Open()
        ElseIf dataBaseConnection.State = ConnectionState.Closed Then
            dataBaseConnection.Open()
        End If
        Return dataBaseConnection
    End Function

    Protected Sub CloseConnection(openConn As SqlConnection)
        If CloseDBConnection Then
            openConn.Close()
            openConn = Nothing
        End If
    End Sub

    'Public Sub ExecSql(SQL_string As String)
    '    Dim command = NewSqlCommand(SQL_string)
    '    Try
    '        command.ExecuteNonQuery()
    '        command.Dispose()
    '    Finally
    '        CloseConnection(dataBaseConnection)
    '    End Try
    'End Sub

    'Public Sub ExecParamaterisedQuery(SQL_string As String, commandParameters As List(Of ParameterData))
    '    Dim command = NewSqlCommand(SQL_string)
    '    For Each parameter As ParameterData In commandParameters
    '        command.Parameters.Add(parameter.ParamName, parameter.ParamType).SqlValue = parameter.ParamValue
    '    Next

    '    Try
    '        command.ExecuteNonQuery()
    '        command.Dispose()
    '    Finally
    '        CloseConnection(dataBaseConnection)
    '    End Try
    'End Sub

    'Public Function ExecStoredProcedure(storedProcedure As String, ParamArray commandParameters() As ParameterData) As Object
    '    Dim command = NewSqlCommand(storedProcedure)
    '    command.CommandType = CommandType.StoredProcedure
    '    Dim SQL_Parameter As SqlParameter
    '    For Each parameter As ParameterData In commandParameters
    '        SQL_Parameter = New SqlParameter(parameter.ParamName, parameter.ParamType)
    '        SQL_Parameter.Value = parameter.ParamValue
    '        SQL_Parameter.Direction = parameter.ParamDirection
    '        command.Parameters.Add(SQL_Parameter)
    '    Next

    '    Dim returnValue As Integer = 0
    '    Try
    '        command.ExecuteNonQuery()
    '        If command.Parameters.Contains("@ReturnValue") Then
    '            returnValue = command.Parameters("@ReturnValue").Value
    '        End If
    '        command.Dispose()
    '    Finally
    '        CloseConnection(dataBaseConnection)
    '    End Try
    '    Return returnValue
    'End Function

    'Public Function ExecStoredProcedure(storedProcedure As String, commandParameters As List(Of ParameterData)) As Object
    '    Dim command = NewSqlCommand(storedProcedure)
    '    command.CommandType = CommandType.StoredProcedure
    '    For Each parameter As ParameterData In commandParameters
    '        command.Parameters.Add(parameter.ParamName, parameter.ParamType).SqlValue = parameter.ParamValue
    '    Next

    '    Dim returnValue As Decimal = 0
    '    Try
    '        If ExpectsReturnValue Then
    '            returnValue = command.ExecuteScalar()
    '        Else
    '            command.ExecuteNonQuery()
    '        End If
    '        command.Dispose()
    '    Finally
    '        CloseConnection(dataBaseConnection)
    '    End Try
    '    Return returnValue
    'End Function

    'Public Function ExecSqlToScalar(SQL_string As String) As Decimal
    '    Dim scalarResult As Object
    '    Dim resultConvert As Decimal
    '    Dim command = NewSqlCommand(SQL_string)
    '    Try
    '        scalarResult = command.ExecuteScalar()
    '        resultConvert = CDec(scalarResult)
    '        command.Dispose()
    '    Finally
    '        CloseConnection(dataBaseConnection)
    '    End Try
    '    Return resultConvert
    'End Function

    'Public Sub ExecSqlToDataReader(SQL_string As String, reader As DataReaderHandler)
    '    Dim command = NewSqlCommand(SQL_string)
    '    Using resultsDataReader As SqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
    '        reader(resultsDataReader)
    '    End Using
    'End Sub

    'Public Function ExecSqlToDataReader(SQL_string As String) As SqlDataReader
    '    Dim resultsDataReader As SqlDataReader
    '    Dim command = NewSqlCommand(SQL_string)
    '    resultsDataReader = command.ExecuteReader
    '    command.Dispose()
    '    Return resultsDataReader
    'End Function

    'Public Function ExecSPToDataReader(storedProcedure As String, ParamArray commandParameters() As SqlParameter) As SqlDataReader
    '    Dim resultsDataReader As SqlDataReader
    '    Dim command = NewSqlCommand(storedProcedure)
    '    command.CommandType = CommandType.StoredProcedure
    '    For Each parameter As SqlParameter In commandParameters
    '        parameter = command.Parameters.Add(parameter)
    '        parameter.Direction = ParameterDirection.Input
    '    Next
    '    resultsDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
    '    command.Dispose()
    '    Return resultsDataReader
    'End Function

    'Private Function BuildStoredProcCommand(storedProcedure As String, commandParameters As ParameterData()) As SqlCommand
    '    Return Me.BuildStoredProcCommand(storedProcedure, commandParameters.ToList)
    'End Function

    'Private Function BuildStoredProcCommand(storedProcedure As String, commandParameters As List(Of ParameterData)) As SqlCommand
    '    Dim command = NewSqlCommand(storedProcedure)
    '    command.CommandType = CommandType.StoredProcedure
    '    Dim parameter_count As Integer = 0
    '    For Each parameter As ParameterData In commandParameters
    '        command.Parameters.Add(parameter.ParamName, parameter.ParamType).SqlValue = parameter.ParamValue
    '        command.Parameters.Item(parameter_count).Direction = parameter.ParamDirection
    '        parameter_count += 1
    '    Next
    '    Return command
    'End Function

    'Public Function ExecSPToDataReader(storedProcedure As String, commandParameters As List(Of ParameterData)) As SqlDataReader
    '    Dim resultsDataReader As SqlDataReader
    '    Dim command = BuildStoredProcCommand(storedProcedure, commandParameters)
    '    resultsDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
    '    command.Dispose()
    '    Return resultsDataReader
    'End Function

    'Public Delegate Function DataReader(storedProcedure As String, commandParameters() As ParameterData) As SqlDataReader
    'Public Function ExecSPToDataReader(storedProcedure As String, commandParameters() As ParameterData) As SqlDataReader
    '    Dim resultsDataReader As SqlDataReader
    '    Dim command = BuildStoredProcCommand(storedProcedure, commandParameters)
    '    resultsDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
    '    command.Dispose()
    '    Return resultsDataReader
    'End Function

    'Public Sub ExecSPToDataReader(Of T)(storedProcedure As String, commandParameters() As ParameterData, reader As System.Func(Of SqlDataReader, T))
    '    Using (dataBaseConnection)
    '        Dim command = BuildStoredProcCommand(storedProcedure, commandParameters)
    '        Using resultsDataReader As SqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
    '            reader(resultsDataReader)
    '        End Using
    '    End Using
    'End Sub

    'Public Delegate Sub DataReaderHandler(reader As SqlDataReader)
    'Public Sub ExecSPToDataReader(storedProcedure As String, commandParameters() As ParameterData, reader As DataReaderHandler)
    '    ' Refer to : http://tipsstation.com/article/Dot-NET-Data-Access-Performance-Comparison.aspx
    '    '            and http://aspalliance.com/526_Using_Delegates_with_Data_Readers_to_Control_DAL_Responsibility.all
    '    Using (dataBaseConnection)
    '        Dim command = BuildStoredProcCommand(storedProcedure, commandParameters)
    '        Using resultsDataReader As SqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
    '            reader(resultsDataReader)
    '        End Using
    '    End Using
    'End Sub

    'Public Sub ExecSPToDataReader(storedProcedure As String, commandParameters As List(Of ParameterData), reader As DataReaderHandler)
    '    Using (dataBaseConnection)
    '        Dim command = BuildStoredProcCommand(storedProcedure, commandParameters)
    '        Using resultsDataReader As SqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
    '            reader(resultsDataReader)
    '        End Using

    '    End Using
    'End Sub

    'Public Sub ExecParamaterisedQuery(SQL_string As String, commandParameters As List(Of ParameterData), reader As DataReaderHandler)
    '    Dim command = NewSqlCommand(SQL_string)
    '    Dim sqlParam As New SqlParameter
    '    If Not commandParameters Is Nothing Then
    '        For Each parameter As ParameterData In commandParameters
    '            command.Parameters.Add(parameter.ParamName, parameter.ParamType).SqlValue = parameter.ParamValue
    '        Next
    '    End If

    '    Using resultsDataReader As SqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
    '        reader(resultsDataReader)
    '    End Using
    'End Sub

    'Public Function ExecSqlToDataSet(SQL_string As String, DataTableName As String) As DataSet
    '    Dim dataSet As New DataSet
    '    Dim dataAdapter As New SqlDataAdapter(SQL_string, GetConnection())
    '    dataAdapter.SelectCommand.CommandType = CommandType.Text
    '    Try
    '        dataAdapter.Fill(dataSet, DataTableName)
    '    Finally
    '        CloseConnection(dataBaseConnection)
    '        dataAdapter.Dispose()
    '    End Try
    '    Return dataSet
    'End Function

    'Public Overloads Function ExecSPToDataSet(storedProcedure As String, DataTableName As String, ParamArray commandParameters() As ParameterData) As DataSet
    '    Dim dataSet As New DataSet
    '    Dim dataAdapter As New SqlDataAdapter(storedProcedure, GetConnection())
    '    dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
    '    Dim SQL_Parameter As SqlParameter
    '    For Each parameter As ParameterData In commandParameters
    '        SQL_Parameter = New SqlParameter(parameter.ParamName, parameter.ParamType)
    '        SQL_Parameter.Value = parameter.ParamValue
    '        SQL_Parameter.Direction = ParameterDirection.Input
    '        dataAdapter.SelectCommand.Parameters.Add(SQL_Parameter)
    '    Next
    '    Try
    '        dataAdapter.Fill(dataSet, DataTableName)
    '    Finally
    '        CloseConnection(dataBaseConnection)
    '        dataAdapter.Dispose()
    '    End Try
    '    Return dataSet
    'End Function


    'Public Overloads Function ExecSPToDataSet(storedProcedure As String, DataTableNames() As String, ParamArray commandParameters() As ParameterData) As DataSet
    '    Dim dataSet As New DataSet
    '    Dim dataAdapter As New SqlDataAdapter(storedProcedure, GetConnection())
    '    dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure

    '    Dim SQL_Parameter As SqlParameter
    '    For Each parameter As ParameterData In commandParameters
    '        SQL_Parameter = New SqlParameter(parameter.ParamName, parameter.ParamType)
    '        SQL_Parameter.Value = parameter.ParamValue
    '        SQL_Parameter.Direction = ParameterDirection.Input
    '        dataAdapter.SelectCommand.Parameters.Add(SQL_Parameter)
    '    Next

    '    Dim tableCount As Integer
    '    Dim defaultTableName As String = ""
    '    For tableCount = 0 To DataTableNames.GetUpperBound(0)
    '        If tableCount > 0 Then
    '            defaultTableName = "Table" & tableCount
    '        Else
    '            defaultTableName = "Table"
    '        End If
    '        dataAdapter.TableMappings.Add(defaultTableName, DataTableNames(tableCount))
    '    Next

    '    dataAdapter.Fill(dataSet)
    '    CloseConnection(dataBaseConnection)
    '    dataAdapter.Dispose()
    '    Return dataSet
    'End Function

    Friend Shared Function ReplaceDBNull(Of T)(checkValue As Object) As Object
        If (Convert.IsDBNull(checkValue)) Then
            Return Nothing
        Else
            Return checkValue
        End If
    End Function

    Friend Shared Function ConvertNothingToDBNull(Of T)(checkValue As Object) As Object
        If (IsNothing(checkValue)) Then
            Return System.DBNull.Value
        Else
            Return checkValue
        End If
    End Function

    'Private Function NewSqlCommand(SQLstring As String) As SqlCommand
    '    Return New SqlCommand(SQLstring, GetConnection()) With {.CommandTimeout = CommandTimeout}
    'End Function

End Class