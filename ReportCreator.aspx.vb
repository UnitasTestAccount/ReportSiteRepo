Imports System.Data
Imports System.Data.SqlClient
Imports GenerateReport
Imports Microsoft.Reporting.WebForms
Partial Class ReportCreator
    Inherits System.Web.UI.Page

    Private RepExport As String
    Private _ParameterDictionary As Dictionary(Of String, String)
    Public Property ParameterDictionary() As Dictionary(Of String, String)
        Get
            Return _ParameterDictionary
        End Get
        Set(ByVal value As Dictionary(Of String, String))
            _ParameterDictionary = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Call ExportReports()

    End Sub

    Private Sub ExportReports()
        Dim dataAccess As New DataManager
        Dim reportString As String = ""
        Dim dsName As String
        Dim SQLcmd As String = ""
        Dim optionString As String = ""
        Dim joiner As String = ""
        Dim sqlTable = ""
        Dim repCode As String = ""
        Dim dateField As String = ""
        Dim conDB As String = ""
        Dim sqlString As String = ""
	Dim activity As Boolean = False

        Try

            ReportViewer1.KeepSessionAlive = True
            If Not Page.IsPostBack Then

                conDB = Request.QueryString("db")
                repCode = IIf(Request.QueryString("report") Is Nothing, "", Request.QueryString("report"))

                Select Case repCode
                    Case 4300
                        sqlTable = "ClientDetails"
                        RepExport = "Client Details"
                        activity = True
                        sqlString = "SELECT ClientDetails.ID, ClientDetails.FirstName, ClientDetails.Surname, ClientDetails.Address1, ClientDetails.Town, " &
                        "ClientDetails.County, ClientDetails.PostCode1, ClientDetails.PostCode2, ClientDetails.Telephone, ClientDetails.Mobile, " &
                        "ClientDetails.Email, ClientDetails.DateOfBirth, ClientDetails.Gender, GPPractices.GPDescription, ClientDetails.Doctor, CoOrdinators.StaffFullName, ClientDetails.Notes, ClientDetails.Active, " &
                        "ClientDetails.ClientYN FROM ClientDetails WITH(NOLOCK) LEFT JOIN " &
                        "GPPractices ON ClientDetails.GPPractice = GPPractices.GPCypher LEFT JOIN " &
                        "CoOrdinators ON ClientDetails.CoOrdinator = CoOrdinators.Id"

                    Case 4301
                        sqlTable = "AssignedCarers"
                        RepExport = "Assigned Carers"
                        activity = True
                        sqlString = "SELECT AssignedCarers.ID,ClientDetails.ID as 'ClientID', ClientDetails.FirstName AS 'ClientFirstName', ClientDetails.Surname AS 'ClientSurname', AssignedCarers.Carer, AssignedCarers.RelativeType, AssignedCarers.MainContact, " &
                         "ClientDetails_1.FirstName AS 'CarerFirstName', ClientDetails_1.Surname AS 'CarerSurname' " &
                        " From AssignedCarers WITH(NOLOCK) LEFT OUTER JOIN " &
                         "ClientDetails ON AssignedCarers.ClientID = ClientDetails.ID LEFT OUTER JOIN " &
                         "ClientDetails AS ClientDetails_1 ON AssignedCarers.CarerID = ClientDetails_1.ID "

                    Case 4302
                        sqlTable = "ExternalReferences"
                        dateField = "EReferralDate"
                        RepExport = "External References"
                        activity = True
                        sqlString = "SELECT ExternalReferences.ID,ClientDetails.ID as 'ClientID', ClientDetails.FirstName, ClientDetails.Surname, ExternalReferences.EReferralDate, JobTitles.JobDescription, ExternalReferences.PrimaryCondition, ExternalReferences.FirstContactDate, " &
                         "CoOrdinators.StaffFullName, ExternalReferences.FirstHomeVisitDate, ExternalReferences.PhoneNumber, ExternalReferences.HomeVisitPossible, ExternalReferences.DeclinedDate, " &
                         "ExternalReferences.ServiceDeclined,ExternalReferences.DateOfHeartAttackStroke, ExternalReferences.CallbackAllowed, ExternalReferences.InfoPack, ExternalReferences.HomeVisit, ExternalReferences.FirstContactmadeVia, TrustAreas.TrustDescription, " &
                         "ExternalReferences.ReferrerName, ExternalReferences.ReferrerEmail, CoOrdinators_1.StaffFullName AS 'ReferredBy' " &
                         "FROM ExternalReferences WITH(NOLOCK) LEFT JOIN " &
                         "CoOrdinators AS CoOrdinators_1 ON ExternalReferences.ReferredBy = CoOrdinators_1.Id LEFT OUTER JOIN " &
                         "ClientDetails ON ExternalReferences.ClientID = ClientDetails.ID LEFT OUTER JOIN " &
                         "JobTitles ON ExternalReferences.JobTitle = JobTitles.ID LEFT OUTER JOIN " &
                         "CoOrdinators ON ExternalReferences.FirstContactmadeBy = CoOrdinators.Id LEFT OUTER JOIN " &
                         "TrustAreas ON ExternalReferences.ReferringTrust = TrustAreas.TrustCode"

                    Case 4303
                        sqlTable = "Assessments"
                        dateField = "Assessments.ModifiedDate"
                        RepExport = "Assessments"
                        activity = True
                        sqlString = "SELECT Assessments.ID,ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, AssessmentGroups.Name AS 'Group', AssessmentTypes.Name, Assessments.ModifiedDate, Assessments.ModifiedBy " &
                        "FROM Assessments WITH(NOLOCK) LEFT OUTER JOIN " &
                         "ClientDetails ON Assessments.ClientID = ClientDetails.ID LEFT OUTER JOIN " &
                         "AssessmentTypes ON Assessments.AssessmentTypeID = AssessmentTypes.ID LEFT OUTER JOIN " &
                         "AssessmentGroups ON AssessmentTypes.AssessmentGroupID = AssessmentGroups.ID "


                    Case 4304
                        sqlTable = "ClientWellBeing"
                        dateField = "WBSDate"
                        RepExport = "Client Well Being"
                        activity = True
                        sqlString = "SELECT ClientWBS.ID, ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, ClientWBS.WBSDate, ClientWBS.LifeStyle, ClientWBS.Money, ClientWBS.Symptoms, ClientWBS.LookAfterYourself, ClientWBS.WorkVolAct, " &
                        "ClientWBS.WhereULive, ClientWBS.FamilyFriends, ClientWBS.FeelPositive " &
                        "FROM ClientWBS WITH(NOLOCK) LEFT JOIN " &
                        "ClientDetails ON ClientWBS.ClientID = ClientDetails.ID"

                    Case 4305
                        sqlTable = "CarerWBS"
                        dateField = "CarerWBS.WBSDate"
                        RepExport = "Carer WBS"
                        activity = True
                        sqlString = "SELECT CarerWBS.ID, ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, CarerWBS.WBSDate, CarerWBS.Health, CarerWBS.Role, CarerWBS.Home, CarerWBS.Time, CarerWBS.Feel, CarerWBS.Finances,CarerWBS.YourWork " &
                        "FROM CarerWBS WITH(NOLOCK) LEFT OUTER JOIN " &
                         "ClientDetails ON CarerWBS.CarerID = ClientDetails.ID"

                    Case 4306
                        sqlTable = "PersonalGoals"
                        dateField = "PersonalGoals.GoalSetDate"
                        RepExport = "Personal Goals"
                        activity = True
                        sqlString = "SELECT PersonalGoals.ID, ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, PersonalGoals.GoalSetDate, PersonalGoals.SmartGoalDetails, PersonalGoals.GoalReviewDate, PersonalGoals.GoalsMetDate " &
                        "FROM PersonalGoals WITH(NOLOCK) LEFT OUTER JOIN " &
                        "ClientDetails ON PersonalGoals.ClientID = ClientDetails.ID"

                    Case 4307
                        sqlTable = "InternalReferences"
                        dateField = "IreferralDate"
                        RepExport = "Internal References"
                        activity = True
                        sqlString = "SELECT DISTINCT InternalReferences.ID, ClientDetails.ID as 'ClientID', " &
                        "ClientDetails.FirstName, ClientDetails.Surname, C1.StaffFullName as 'ReferredBy', " &
                        "N1.ServiceDescription AS 'ReferralService', InternalReferences.IreferralDate as 'InternalReferralDate', InternalReferences.ReferralNotes, " &
                        "InternalReferences.EndDate, InternalReferences.DidNotFinish, InternalReferences.EreferralID as 'ExternalReferralID',  " &
                        "InternalReferences.ReferralRecdBy, InternalReferences.DeclinedDate, InternalReferences.ProposedStartDate," &
                        "N3.ServiceDescription as 'ServiceDescription', C2.StaffFullName as 'ReferredTo',  " &
                        "ServiceDeclineds.ServiceDeclinedDesc " &
                        "FROM InternalReferences WITH(NOLOCK)  " &
                        "LEFT OUTER JOIN (select cast(ID as varchar(20)) as 'ID',StaffFullName,StaffTelNo,Activity from CoOrdinators union select 'SR','Self Referral','','A' from CoOrdinators) C1 ON cast(InternalReferences.ReferredBy as varchar(20)) = cast(C1.ID as varchar(20))  " &
                        "LEFT OUTER JOIN (select cast(ID as varchar(20)) as 'ID',StaffFullName,StaffTelNo,Activity from CoOrdinators union select 'SR','Self Referral','','A' from CoOrdinators) C2 ON cast(InternalReferences.ReferredTo as varchar(20)) = cast(C2.ID as varchar(20))  " &
                        "LEFT OUTER JOIN ClientDetails ON InternalReferences.ClientID = ClientDetails.ID  " &
                        "LEFT OUTER JOIN ServiceDeclineds ON cast(InternalReferences.ServiceDeclined as varchar(20)) = cast(ServiceDeclineds.ID as varchar(20))  " &
                        "LEFT OUTER JOIN (select * from NICHSServices where Activity <> 'D') N1 ON cast(InternalReferences.ReferringService as varchar(20)) = cast(N1.ServiceCode as varchar(20))  " &
                        "LEFT OUTER JOIN (select * from NICHSServices where Activity <> 'D') N3 ON cast(InternalReferences.InternalReferringService as varchar(20)) = cast(N3.ServiceCode as varchar(20))  "

                    Case 4308
                        sqlTable = "Prep"
                        RepExport = "Prep"
                        activity = True
                        sqlString = "SELECT PrepDetails.ID, ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, PrepDetails.InternalRefID, PrepDetails.PrepDate, PrepDetails.PrepWk, PrepDetails.Useful, PrepDetails.Attendance, PrepDetails.Aid, " &
                        "PrepDetails.TenM, PrepDetails.Tug, PrepDetails.Tuss, PrepDetails.Optimistic, PrepDetails.Relaxed, PrepDetails.Problems, PrepDetails.ThinkingClearly, PrepDetails.CloseWith, PrepDetails.MakingMind, " &
                        "PrepDetails.TenM_min, PrepDetails.TenM_sec, PrepDetails.TenM_mil, PrepDetails.Tug_min, PrepDetails.Tug_sec, PrepDetails.Tug_mil, PrepDetails.Tuss_min, PrepDetails.Tuss_sec, " &
                        "PrepDetails.Tuss_mil " &
                        "FROM PrepDetails WITH(NOLOCK) LEFT JOIN " &
                         "ClientDetails ON PrepDetails.ClientID = ClientDetails.ID"

                    Case 4309
                        sqlTable = "TakingControl"
                        RepExport = "Taking Control"
                        activity = True
                        sqlString = "SELECT TakingControlDetails.ID, ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, TakingControlDetails.Referral, TakingControlDetails.TakingControlDate, TakingControlDetails.TakingControlWeek, " &
                        "TakingControlDetails.Pain, TakingControlDetails.Fatigue, TakingControlDetails.Emotions, TakingControlDetails.OtherSymptons, TakingControlDetails.DifferentTasks, TakingControlDetails.OtherThanMeds, " &
                        "TakingControlDetails.Comments, TakingControlDetails.Attendance " &
                        "FROM TakingControlDetails WITH(NOLOCK) LEFT JOIN " &
                         "ClientDetails ON TakingControlDetails.ClientID = ClientDetails.ID"

                    Case 4310
                        sqlTable = "Contacts"
                        RepExport = "Contacts"
                        activity = True
                        sqlString = "SELECT ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, InternalReferences.ReferringService, FollowUps.FollowUpDate, FollowUpTypes.FollowUpDescription, FollowUps.Notes, CoOrdinators.StaffFullName " &
                        "FROM ClientDetails WITH(NOLOCK) INNER JOIN " &
                         "FollowUps ON ClientDetails.ID = FollowUps.ClientID INNER JOIN " &
                         "FollowUpTypes ON FollowUps.FollowUpType = FollowUpTypes.FollowUpCode INNER JOIN " &
                         "CoOrdinators ON FollowUps.CoOrdinator = CoOrdinators.Id INNER JOIN " &
                         "InternalReferences ON FollowUps.Referral = InternalReferences.EreferralID "

                    Case 4311
                        sqlTable = "SignPost"
                        RepExport = "SignPosting"
                        dateField = "Signpostings.SignpostDate"
                        activity = True
                        sqlString = "SELECT ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, Signpostings.Referral, Signpostings.SignpostDate, Signpostings.SignpostType, TrustAreas.TrustDescription, Signpostings.Notes, " &
                        "ExternalGroups.ExternalGroupDescription " &
                        "FROM Signpostings WITH(NOLOCK) INNER JOIN " &
                         "ClientDetails ON Signpostings.ClientID = ClientDetails.ID INNER JOIN " &
                         "InternalReferences on InternalReferences.id = Signpostings.referral  INNER JOIN " &
                         "ExternalReferences on ExternalReferences.ID = InternalReferences.EreferralID INNER JOIN " &
                         "TrustAreas ON ExternalReferences.ReferringTrust = TrustAreas.TrustCode LEFT OUTER JOIN " &
                         "ExternalGroups ON Signpostings.ExternalGroup = ExternalGroups.ExternalGroupCode"

                    Case 4312
                        sqlTable = "Discharge"
                        RepExport = "Discharge"
                        dateField = "Discharges.DischargeDate"
                        activity = True
                        sqlString = "SELECT ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, Discharges.DischargeDate, Discharges.GoalsCompletion, " &
                            "CASE Discharges.ReasonForDischarge WHEN 1 THEN 'Goals Met' WHEN 2 THEN 'Goals Partially Met' WHEN 3 THEN 'Goals Unmet' ELSE 'Goals Unmet' END AS 'ReasonForDischarge' " &
                        ", Discharges.Notes " &
                        "FROM Discharges WITH(NOLOCK) INNER JOIN " &
                         "ClientDetails ON Discharges.ClientID = ClientDetails.ID INNER JOIN " &
                         "ReasonForDischarge ON Discharges.ReasonForDischarge = ReasonForDischarge.ReasonCode"

                    Case 4313
                        sqlTable = "Groups"
                        RepExport = "Groups"
                        sqlString = "SELECT GroupID, GroupService, GroupLocation, GroupDescription FROM GroupMaintenances"

                    Case 4314
                        sqlTable = "Participants"
                        RepExport = "Participants"
                        activity = True
                        sqlString = "SELECT GroupMaintenances.GroupDescription, GroupParticipants.PersonID, ClientDetails.ID as 'ClientID',ClientDetails.FirstName, ClientDetails.Surname, ClientDetails.Telephone, ClientDetails.PersonType " &
                        "FROM GroupParticipants WITH(NOLOCK) INNER JOIN " &
                         "ClientDetails ON GroupParticipants.PersonID = ClientDetails.ID INNER JOIN " &
                         "GroupMaintenances ON GroupParticipants.GroupID = GroupMaintenances.GroupID"

                    Case 4315
                        sqlTable = "Attendance"
                        RepExport = "Attendance"
                        dateField = "GroupAttendances.GroupDate"
                        activity = True
                        sqlString = "SELECT GroupMaintenances.GroupID, GroupMaintenances.GroupDescription, GroupAttendances.PersonID,ClientDetails.ID as 'ClientID',ClientDetails.FirstName, " &
                        "ClientDetails.Surname, GroupAttendances.GroupDate, GroupAttendances.GroupAttendance, " &
                        "GroupAttendances.PersonType " &
                        "FROM GroupMaintenances WITH(NOLOCK) INNER JOIN " &
                         "GroupAttendances ON GroupMaintenances.GroupID = GroupAttendances.GroupID INNER JOIN " &
                         "ClientDetails ON GroupAttendances.PersonID = ClientDetails.ID"
                End Select

                Dim parameter As New ReportParameter
                Dim paramCollection As Array
                paramCollection = Request.QueryString.AllKeys

                reportString = sqlString


                For Each param As String In paramCollection
                    If param <> "report" And param <> "db" Then
                        If param.Contains(":isNull") Then
                            param = param.Substring(0, param.Length - 7)
                        End If
                        If InStr(reportString, "WHERE") > 0 Then
                            joiner = " AND "
                        Else
                            joiner = " WHERE "
                        End If

                    	If InStr(param, "FromDate") > 0 Then
                        	If Request.QueryString(param) <> "" Then
                            		reportString = reportString & joiner & "(" & dateField & " >= CONVERT(datetime,'" & Request.QueryString(param) & "',103) OR '" & Request.QueryString(param) & "' IS NULL)"
                        	End If
                    	ElseIf InStr(param, "ToDate") > 0 Then
                        	If Request.QueryString(param) <> "" Then
                            		reportString = reportString & joiner & "(" & dateField & " <= CONVERT(datetime,'" & Request.QueryString(param) & "',103) OR '" & Request.QueryString(param) & "' IS NULL)"
                        	End If
                    	ElseIf InStr(param, "StartDateFrom") > 0 Then
                        	If Request.QueryString(param) <> "" Then
                            		reportString = reportString & joiner & "(ProposedStartDate >= CONVERT(datetime,'" & Request.QueryString(param) & "',103) OR '" & Request.QueryString(param) & "' IS NULL)"
                        	End If
                    	ElseIf InStr(param, "StartDateTo") > 0 Then
                        	If Request.QueryString(param) <> "" Then
                            		reportString = reportString & joiner & "(ProposedStartDate <= CONVERT(datetime,'" & Request.QueryString(param) & "',103) OR '" & Request.QueryString(param) & "' IS NULL)"
                        	End If

                    	ElseIf InStr(param, "ClientID") > 0 Then
                        	If Request.QueryString(param) <> "" Then
                            		reportString = reportString & joiner & "(ClientDetails.ID = '" & Request.QueryString(param) & "' OR '" & Request.QueryString(param) & "' IS NULL)"
                        	End If

                    	ElseIf InStr(param, "GroupID") > 0 Then
                        	If Request.QueryString(param) <> "" Then
                            		reportString = reportString & joiner & "(GroupMaintenances.GroupID = '" & Request.QueryString(param) & "' OR '" & Request.QueryString(param) & "' IS NULL)"
                        	End If

                    	ElseIf InStr(param, "ReferredBy") > 0 Then
                        	If Request.QueryString(param) <> "" Then
                            		reportString = reportString & joiner & "(ReferredBy = '" & Request.QueryString(param) & "' OR '" & Request.QueryString(param) & "' IS NULL)"
                        	End If

                    	ElseIf InStr(param, "ReferredTo") > 0 Then
                        	If Request.QueryString(param) <> "" Then
                            		reportString = reportString & joiner & "(ReferredTo = '" & Request.QueryString(param) & "' OR '" & Request.QueryString(param) & "' IS NULL)"
                        	End If

			End If

                    End If

                Next

                If activity = True Then
                    If InStr(reportString, "WHERE") > 0 Then
                        joiner = " AND ClientDetails.Activity <> 'D'"
                    Else
                        joiner = " WHERE ClientDetails.Activity <> 'D'"
                    End If
		    reportString = reportString & joiner
                End If

                'ClientScript.RegisterStartupScript(Me.GetType(), "AlertMessageBox2", "alert(""'" & reportString & "'"");", True)

                dsName = "Export"
                SQLcmd = reportString

                Dim dt As New DataTable()
                Dim cmd As New SqlCommand(SQLcmd, dataAccess.GetConnection(conDB))
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                ShowReportViewer(dt, dsName)
            End If

        Catch ex As Exception
            'ClientScript.RegisterStartupScript(Me.GetType(), "AlertMessageBox", "alert(""'" & reportString & "'"");", True)
            'ClientScript.RegisterStartupScript(Me.GetType(), "AlertMessageBox", "alert(""No results found"");", True)
        End Try

    End Sub

    Private Sub ShowReportViewer(dtable As DataTable, dname As String)

        Dim gen As New GenerateReport.ReportGenerator(dtable, dname)
        Dim ds As New ReportDataSource(dname, dtable)
        ReportViewer1.Reset()
        ReportViewer1.LocalReport.DataSources.Add(ds)
        ReportViewer1.LocalReport.DisplayName = RepExport
        ReportViewer1.LocalReport.LoadReportDefinition(gen.GenerateReport())

    End Sub

    Private Function GetColumnNames(dBase As String, tableName As String) As String
        Dim dataAccess As New DataManager
        Dim dsColumnName As New DataSet

        Dim strSql As String = "SELECT column_name FROM INFORMATION_SCHEMA.Columns where TABLE_NAME = '" & tableName & "'"
        Dim cmd As New SqlCommand(strSql, dataAccess.GetConnection(dBase))
        Dim dAdapter As New SqlDataAdapter(cmd)
        dAdapter.Fill(dsColumnName)
        Dim ColumnsName_String As String = ""
        For i As Integer = 0 To dsColumnName.Tables(0).Rows.Count - 1
            ColumnsName_String = ColumnsName_String & "," & tableName & "." & dsColumnName.Tables(0).Rows(i).Item(0)
        Next
        If ColumnsName_String.StartsWith(",") Then
            ColumnsName_String = ColumnsName_String.Remove(0, 1)
        End If
        Return ColumnsName_String

    End Function

    
   
End Class
