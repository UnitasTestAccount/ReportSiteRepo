<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ReportScheduler.aspx.vb" Inherits="ReportScheduler" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<%--<%@ Register TagPrefix="ubs" TagName="Scheduler" Src="~/controls/Scheduler.ascx" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table id="ReportSchedules" class="report" style="display:none;margin-top:30px;margin-bottom:10px;margin-left:20px;width:80%">
                <thead>
                    <tr>
                        <th></th>
                        <th><asp:Literal ID="KeyL" runat="server" Text="Report" /></th>
                        
                        <th style="display:none;"><asp:Literal ID="Label2" runat="server" Text="ContractStatus" /></th>
                        <th style="display:none"><asp:Literal ID="CreatedByL" runat="server" Text="CreatedBy" /></th>
                        <th><asp:Literal ID="Label37" runat="server" Text="Scheduler " /></th>
                        <th><asp:Literal ID="AssignL" runat="server" Text="Assign" /></th>
                        <th class="noBackground"><img id="NewPermission" class="NewPermission" alt="Add line" src="../images/add16x16.png" class="clickable AddLine newRecord" style="margin:2px;"/></th>
                    </tr>
                </thead>
                <tbody id="ReportSchedulesExisting"></tbody>
            </table>
            <script type="text/html" id="ReportSchedulesContainer">
                {#foreach $T as ReportSchedule}
                    <tr>
                        <td class="showIfEditing"></td>
                        <td class="smallfieldcell">
                            <input id="ReportScheduleID" class="ReportScheduleID" readonly disabled value="{$T.ReportSchedule.ReportScheduleID}" data-permissionKey="{$T.ReportSchedule.PermissionKey}" />
                        </td>

                        <td style="display:none;">
                            <select id="ContractStatus" class="mediumfieldcell reqd pick" style="width:96%">
                                <option value="All" {#if $T.ReportSchedule.ContractStatus == "All"} selected {#/if}>All</option>
                                <option value="Contract" {#if $T.ReportSchedule.ContractStatus == "Contract"} selected {#/if}>Contract</option>
                                <option value="NonContract" {#if $T.ReportSchedule.ContractStatus == "NonContract"} selected {#/if}>Non-Contract</option>
                            </select>
                        </td>

                        <td style="display:none">
                            <select id="CreatedBy" class="mediumfieldcell reqd inputs" style="width:120px">
                                <option value="FARMS">Farms</option>
                                <option value="SYSTEM">System</option>
                                <option value="AUDIT">Audit</option>
                            </select>
                        </td>
                        <td class="mediumfieldcell"><span style="text-align:center" class="scheduleStatus" disabled readonly/></td>
                        <td class="assigned {#if $T.ReportSchedule.Assign == 1} checked {#/if}"></td>
                        <td>
                            <img src="../images/calendar24x24.png" id="Schedule" class="Schedule"{#if typeof $T.ReportSchedule.PermissionKey === "undefined"} style="display:none" {#/if}/>
                        </td>
                    </tr>
                {#/for}
            </script>
    </div>
    </form>
    <script type="text/javascript" src="../javascript/jquery-1.7.1.min.js"></script>
        <script type="text/javascript" src="../javascript/jquery-ui-1.8.11.custom.min.js"></script>
        <script type="text/javascript" src="../javascript/json2.js"></script>
        <script type="text/javascript" src="../javascript/jquery-jtemplates.js"></script>
        <script type="text/javascript" src="../javascript/jquery.tmpl.min.js"></script>
        <script type="text/javascript" src="../javascript/jquery.dataTables.js"></script>
        <script type="text/javascript" src="../javascript/knockout-3.0.0.js"></script>
        <script type="text/javascript" src="../javascript/knockout.mapping.js"></script>
        <script type="text/javascript" src="../javascript/utilities.js"></script>
        <script type="text/javascript" src="../javascript/poultryCommon.js"></script>
        <script type="text/javascript" src="../repScheduler/canvas.js"></script>
        <script type="text/javascript" src="../repScheduler/ReportScheduler.js"></script>
        <script type="text/javascript" src="../javascript/scheduler.js"></script>
        <script type="text/javascript">
            $(document).ready(function () {
                reportScheduler.ready();
            });
        </script>
</body>
</html>
