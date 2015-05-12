/**
 * Inneholder alle javascript som har med create og edit views å gjøre
 * 
 * @author Frederik Johnsen
 * Date 04.05.2015
 */

var Calibration = {
    FancyboxSearch: function (radioGroupeVal, searchVal, roomList, companyList, eventTypeList, deviceList, fileList) {
        if (radioGroupeVal === "room" && roomList != null) {
            jQuery.each(roomList, function(i, item) {
                var str = String(item["Name"]).toLowerCase();
                if(str.match(searchVal)) {
                    $("#tbody").append("<tr><td>" + str + "</td><td><input id='" + str + "' name='Checked' type='checkbox' value='" + item["RoomID"] + "' /><input name='Checked' type='hidden' value='" + item["RoomID"] + "' /></td></tr>");
                }
            });
        }
        else if (radioGroupeVal === "company" && companyList != null) {
            jQuery.each(companyList, function(i, item) {
                var str = String(item["Name"]).toLowerCase();
                if(str.match(searchVal)) {
                    $("#tbody").append("<tr><td>" + str + "</td><td><input id='" + str + "' name='Checked' type='checkbox' value='" + item["CompanyID"] + "' /><input name='Checked' type='hidden' value='" + item["CompanyID"] + "' /></td></tr>");
                }
            });
        }
        else if (radioGroupeVal === "eventType" && eventTypeList != null) {
            jQuery.each(eventTypeList, function(i, item) {
                var str = String(item["Name"]).toLowerCase();
                if(str.match(searchVal)) {
                    $("#tbody").append("<tr><td>" + str + "</td><td><input id='" + str + "' name='Checked' type='checkbox' value='" + item["EventTypeID"] + "' /><input name='Checked' type='hidden' value='" + item["EventTypeID"] + "' /></td></tr>");
                }
            });
        }
        else if (radioGroupeVal === "device" && deviceList != null) {
            jQuery.each(deviceList, function(i, item) {
                var str = String(item["Name"]).toLowerCase();
                if(str.match(searchVal)) {
                    $("#tbody").append("<tr><td>" + str + "</td><td><input id='" + str + "' name='Checked' type='checkbox' value='" + item["DeviceID"] + "' /><input name='Checked' type='hidden' value='" + item["DeviceID"] + "' /></td></tr>");
                }
            });
        }
        else if (radioGroupeVal === "file" && fileList != null) {
            jQuery.each(fileList, function(i, item) {
                var str = String(item["FileName"]).toLowerCase();
                if(str.match(searchVal)) {
                    $("#tbody").append("<tr><td>" + str + "</td><td><input id='" + str + "' name='Checked' type='checkbox' value='" + item["FileID"] + "' /><input name='Checked' type='hidden' value='" + item["FileID"] + "' /></td></tr>");
                }
            });
        }
    },

    FancyboxPutIn: function (radioGroupeVal, checkBoxGroupeVal, roomList, companyList, eventTypeList, deviceList, fileList) {
        if (radioGroupeVal === "room" && roomList != null) {
            jQuery.each(roomList, function (i, item) {
                var str = String(item["RoomID"]);
                if (str.match(checkBoxGroupeVal)) {
                    $("#Room_RoomID").val(item["RoomID"]);
                    $("#Room_Name").val(item["Name"]);
                }
            });
        }
        else if (radioGroupeVal === "company" && companyList != null) {
            jQuery.each(companyList, function (i, item) {
                var str = String(item["CompanyID"]);
                if (str.match(checkBoxGroupeVal)) {
                    $("#Company_CompanyID").val(item["CompanyID"]);
                    $("#Company_Name").val(item["Name"]);
                }
            });
        }
        else if (radioGroupeVal === "eventType" && eventTypeList != null) {
            jQuery.each(eventTypeList, function (i, item) {
                var str = String(item["EventTypeID"]);
                if (str.match(checkBoxGroupeVal)) {
                    $("#EventType_EventTypeID").val(item["EventTypeID"]);
                    $("#EventType_Name").val(item["Name"]);
                }
            });
        }
        else if (radioGroupeVal === "device" && deviceList != null) {
            jQuery.each(deviceList, function (i, item) {
                var str = String(item["DeviceID"]);
                if (str.match(checkBoxGroupeVal)) {
                    $("#Device_DeviceID").val(item["DeviceID"]);
                    $("#Device_Name").val(item["Name"]);
                }
            });
        }
        else if (radioGroupeVal === "file" && fileList != null) {
            jQuery.each(fileList, function (i, item) {
                var str = String(item["FileID"]);
                if (str.match(checkBoxGroupeVal)) {
                    $("#FileTo_FileID").val(item["FileID"]);
                    $("#FileTo_FileName").val(item["FileName"]);
                }
            });
        }
    },

    SaveButton: function (radioGroupeVal) {
        var test = $("#fileInput").val();
        var test2 = $("#File_FileName")[0];
        if (radioGroupeVal === "external") {
            if ($("#fileInput").val() === "")
                $("#File_FileName")[0].setAttribute("required", "true");
            else
                $("#File_FileName")[0].removeAttribute("required");
        }
    }
}

var UserAccount = {
    FancyboxTable: function (rightList) {
        jQuery.each(rightList, function (i, item) {
            var tr = document.createElement("tr");

            var td = document.createElement("td");
            td.textContent = item["Name"];
            tr.appendChild(td);

            td = document.createElement("td");
            var input = document.createElement("input");
            input.type = "Radio";
            input.name = "group";
            input.value = item["RightsID"];
            td.appendChild(input);
            tr.appendChild(td);
            $("#tbody").append(tr);
        });
    },

    FancyboxPutIn: function (radioGroupeVal, rightList) {
        jQuery.each(rightList, function (i, item) {
            if (String(item["RightsID"]).match(String(radioGroupeVal))) {
                $("#Right_RightsID").val(item["RightsID"]);
                $("#Right_Name").val(item["Name"]);
            }
        });
    }
}
