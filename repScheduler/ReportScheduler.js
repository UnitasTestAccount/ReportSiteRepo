var dataTableRef;
var listName = "";

var reportScheduler = {
    canvases: null,
    selectedCanvas: null,
    canvasPermissions: null,
    validationOptions: "",
    fieldTypeOptions: "",
    sectionOptions: "",
    fieldGroupOptions: "",
    selectedPermissions: {},
    permissionCount: 0,
    newCanvasCreated: false,
    unsavedChanges: false,
    canvasSections: [],
    fieldGroups: [],
    schedulesToStore: [],

    //=====================================================
    ready: function () {
        cM = this;
        utilities.ajaxPageDefaults();
       
        cM.fieldValidation();
        cM.assignDatePickerToNewFields();
        cM.prepareListPopup();
        cM.getLists();
        cM.fieldPropertiesBinding();
    },

    //=====================================================
    assignActions: function () {
               
        $("#store").click(function () { cM.store(); });

        $("body").delegate(".Schedule", "click", function () {
            var noSchedules = true;
            var permID = Number($(this).parent().parent().find(".CanvasID").attr("data-permissionKey"));
            for (i in cM.canvasPermissions) {
                if (cM.canvasPermissions[i].PermissionKey === permID) {
                    for (j in cM.schedules) {
                        if (cM.schedules[j].Key === permID) {
                            canvasScheduler.build(cM.schedules[j], updateSchedules);
                            noSchedules = false;
                        }
                    }
                }
            }
            if (noSchedules) { canvasScheduler.build(undefined, updateSchedules); }

            $("#ScheduleProperties").dialog("open");

            function updateSchedules(schedules) {

                for (i in cM.canvasPermissions) {
                    if (cM.canvasPermissions[i].PermissionKey === permID) {
                        if (typeof schedules.Id !== "undefined") { existingSchedule(); }
                        if (typeof schedules.Id === "undefined") { brandNewSchedule(); }
                    }
                }

                if ($("#MeasureAgeIn").val() === "Weeks") {
                    schedules.StartAge = schedules.StartAge * 7;
                    schedules.EndAge = schedules.EndAge * 7;
                }

                schedules.StartDate = schedules.StartDate === "" ? "" : utilities.formatDate(schedules.StartDate);
                schedules.EndDate = schedules.StartDate === "" ? "" : utilities.formatDate(schedules.EndDate);
                schedules.DaysSelected = schedules.DaysSelected.join("|");
                schedules.MonthsSelected = schedules.MonthsSelected.join("|");
                schedules.DatesInMonthSelected = schedules.DatesInMonthSelected.join("|");
                schedules.MonthWeekOrdinals = schedules.MonthWeekOrdinals.join("|");
                schedules.MonthDays = schedules.MonthDays.join("|");
                schedules.AgesSelected = schedules.AgesSelected.join("|");
                var selectedDates = [];
                for (i = 0; i < schedules.DatesSelected.length; i++) {
                    selectedDates.push(schedules.DatesSelected[i].date);
                }
                schedules.DatesSelected = selectedDates.join("|");
                
                cM.fieldDisabling();
                cM.displayScheduleOnMainView();

                function brandNewSchedule() {
                    if (cM.schedules.length === 0) {
                        schedules.Id = cM.schedules.length + 1;
                        schedules.Key = permID;
                        cM.schedules.push(schedules);
                    } else {
                        cM.schedules[0] = schedules;
                    }
                }

                function existingSchedule() {
                    for (schedule in cM.schedules) {
                        if (cM.schedules[schedule].Key === permID) {
                            cM.schedules[schedule] = schedules;
                        }
                    }
                }
            }
        });
        utilities.dropdownNumbers($(".hours"), 0, 23, 1, true);
        utilities.dropdownNumbers($(".minutes"), 0, 50, 10, true);

    },

  
   
    //=====================================================
    assignDatePickerToNewFields: function () {
        $(".dates").on(
            "focus", function () {
                if (!$(this).is(':data(datepicker)')) {
                    $(this).datepicker({ dateFormat: 'dd/mm/yy', clickInput: true, createButton: false });
                }
            });
    },

    //=====================================================
   
    highlightRow: function (tableId, row) {
        $("tr", tableId).children().children().removeClass("rowSelected");
        row.children().children().not(".picklist").addClass("rowSelected");
    },

   
    //=====================================================
    store: function () {
       
        if (utilities.validateFieldsX($(".report tbody"))) { return false; }
        cM.fieldNamingError = false

        var params = buildStoreParameters()

        dataContainer = { 'canvasSetup': params.canvasSetup, 'permValues': cM.selectedPermissions, 'canvasDetails': params.canvas, 'uploadedFromFile': false };
        $.ajax({
            url: "../canvas/CreateCanvasWebService.asmx/UpdateCanvasSetup",
            data: JSON.stringify(dataContainer),
            async: false,
            success: function (result) {
                utilities.displaySuccessfulSave();
                if (cM.newCanvasCreated) {
                    var newData = dataTableRef.fnAddData([$("#Canvas").val(), $("#CanvasTitle").html(), "FARMS", "A", "No", "SO", null, "", "", ""]);
                    var newRow = dataTableRef.fnSettings().aoData[newData[0]].nTr;
                    $("td:eq(2)", newRow).hide();
                    $("td:eq(3)", newRow).hide();
                    $("td:eq(4)", newRow).hide();
                    $("td:eq(5)", newRow).hide();
                    $("td:eq(6)", newRow).hide();
                    $("td:eq(7)", newRow).hide();
                    $("td:eq(8)", newRow).hide();
                    $("td:eq(9)", newRow).hide();
                    cM.newCanvasCreated = false;
                    $("#NewPermission").attr("disabled", false);
                    cM.canvases = result.d;
                };
                cM.getCanvasInfo();
                cM.onPermissionSelection();
                $(".Schedule").show();
            }
        });

        function buildStoreParameters() {

            var params = {};
            params.canvas = cM.selectedCanvas;
            params.canvas.CanvasID = params.canvas.CanvasID === "" ? 0 : params.canvas.CanvasID;
            params.canvas.DisplayDate = "";
            params.canvas.EffectiveFromDate = "";
            params.canvas.EffectiveToDate = "";

            params.canvasContainer = [], params.scheduleContainer = [], params.sectionContainer = [], params.fieldGroupContainer = [], params.fieldContainer = [], params.sectionGroupContainer = [];

            $("#CanvasPermissionsExisting tr").each(function (i, item) {
                params.canvasContainer[i] = new cM.parsePermissionData(item);
            });

            for (schedule in cM.schedules) {
                if (cM.schedules[schedule].Key === cM.selectedPermissions.key) {
                    params.scheduleContainer.push(cM.schedules[schedule]);
                }
            }
            params.canvasSetup.schedules = params.scheduleContainer;
            
            return params;
        }
        return true;
    },

    parseScheduleData: function (schedule, id) {
        this.Id = id;
        this.CanvasId = schedule.CanvasId;
        this.Type = schedule.Type;
        if (this.Type === "Date") {
            this.StartDate = utilities.formatDate(schedule.StartDate);
        } else {
            this.StartAge = schedule.StartAge;
        }
        this.CompleteBy = schedule.CompleteBy;
        this.Key = schedule.Key;
    },

 
    //=====================================================
    getReportParameters: function () {
        //        if (cM.unsavedChanges) {
        //            //if (window.confirm("You have unsaved changes!\nClick Cancel to allow you to save your changes.")) { cM.unsavedChanges = false; } else { return 0; };
        //        }
        $.ajax({
            url: "../repScheduler/ReportScheduleWebService.asmx/GetParameters",
            data: "{ reportID: '" + $("#Canvas").val() + "', project: '" + cM.selectedPermissions.division + "'}",
            success: function (result) {
                
                cM.schedules = result.d.Schedules;
               
                for (schedule in cM.schedules) {
                    cM.schedules[schedule].StartDate = utilities.getDateFromJSON(cM.schedules[schedule].StartDate);
                    cM.schedules[schedule].EndDate = utilities.getDateFromJSON(cM.schedules[schedule].EndDate);
                }

                //
                if ($("#PageVersion").val() !== "audit") {
                    cM.buildFields();
                    $("#CanvasFieldsContainer").show();
                    $("#CanvasSectionsContainer").show();
                }
                cM.buildSections(cM.canvasSections);
                cM.buildGroups(cM.fieldGroups);
                if (!$("#CanvasPermissionsExisting tr").hasClass("rowSelected")) {
                    $("#CanvasPermissionsExisting tr:first").addClass("rowSelected");
                    cM.selectedPermissions.key = Number($("#CanvasID", $("#CanvasPermissionsExisting tr:first")).attr("data-permissionKey"));
                }
                cM.fieldDisabling();
                cM.displayScheduleOnMainView();

                
            }
        });

      
    },

    displayScheduleOnMainView: function () {
        $("tr", "#CanvasPermissions tbody").each(function (i, row) {
            var scheduledValues = "";
            multipleSchedules = false;
            for (schedule in cM.schedules) {
                if (cM.schedules[schedule].Key === Number($(".CanvasID", row).attr("data-permissionKey"))) {
                    scheduledValues = cM.schedules[schedule].Frequency;
                    if (scheduledValues.length > 21) {
                        scheduledValues = scheduledValues.substring(0, 20) + "...";
                    }
                    $(".scheduleStatus", row).html(scheduledValues);
                }
            }
        });
    }
};

