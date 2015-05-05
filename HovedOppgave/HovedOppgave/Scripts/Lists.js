/**
 * Inneholder alle javascript som har med list views å gjøre
 * 
 * @author Frederik Johnsen
 * Date 04.05.2015
 */

var GetColName = function (thead) {
    var child = thead.children[0];
    var ddl = document.getElementById("ddm");
    ddl.innerHTML = "";
    for (var i = 0; i < child.children.length; i++) {
        if (child.children[i].hidden) {
            ddl.innerHTML += "<li id='" + i + "'><a><input type='checkbox' disabled/>" + child.children[i].textContent; + "</a></li>";
        }
        else {
            ddl.innerHTML += "<li id='" + i + "'><a><input type='checkbox'checked='checked' disabled/>" + child.children[i].textContent; + "</a></li>";
        }
    }
}

var CheckColName = function (colNumber, thead, tbody, checkInput) {
    if (checkInput[0].type === 'checkbox')
        checkInput[0].onchange = HideColumn(colNumber, thead, tbody);     
}

var HideColumn = function (colNumber, thead, tbody) {
    var childTH = thead.children[0];
    var hideTH = childTH.children[colNumber];
    if (hideTH.hidden) {
        hideTH.hidden = false;
    }
    else {
        hideTH.hidden = true;
    }
    for (i = 0; i < tbody.children.length; i++) {
        var childTB = tbody.children[i];
        var hideCol = childTB.children[colNumber];
        if (hideTH.hidden) {
            hideCol.hidden = true;
        }
        else {
            hideCol.hidden = false;
        }
    }
};

var GetCalibrationTable = function (type, deviceTypeList, deviceList, logEventList, companyList,
    fileList, roomList, dateObject) {

    var nowDate = new Date();
    nowDate.setHours(0);
    nowDate.setMinutes(0);
    nowDate.setSeconds(0);

    jQuery.each(deviceTypeList, function (i, deviceType) {
        var h4 = document.createElement("h4");
        h4.textContent = String(deviceType["Type"]) + " - " + String(deviceType["Description"]);

        var table = document.createElement("table");
        table.className = "table";
        var thead = document.createElement("thead");
        thead.setAttribute("id", "thead");
        var tbody = document.createElement("tbody");
        tbody.setAttribute("id", "tbody")

        FillTableHead(thead, "calibration");

        jQuery.each(deviceList, function (i, device) {
            if (String(deviceType["DeviceTypeID"]).match(device["DeviceTypeID"])) {
                jQuery.each(logEventList, function (i, logEvent) {
                    if (String(logEvent["DeviceID"]).match(device["DeviceID"])) {
                        var endDate = new Date($.parseJSON(String(logEvent["EndDate"]).substr(6, 13)));
                        var startDate = new Date($.parseJSON(String(logEvent["StartDate"]).substr(6, 13)));

                        switch (type) {
                            case "is":
                                if (nowDate >= endDate)
                                {
                                    FillCalibrationTableBoby(tbody, startDate, endDate, companyList, fileList, roomList, logEvent, device, null);
                                    break;
                                }
                                else
                                    break;
                            case "goingTo":
                                if (nowDate <= startDate && dateObject >= startDate) {
                                    FillCalibrationTableBoby(tbody, startDate, endDate, companyList, fileList, roomList, logEvent, device, null);
                                    break;
                                }
                                else
                                    break;
                            case "planed":
                                if (startDate >= dateObject) {
                                    FillCalibrationTableBoby(tbody, startDate, endDate, companyList, fileList, roomList, logEvent, device, null);
                                    break;
                                }
                                else
                                    break;
                        }
                    }
                });
            }
        });
        AddTableToContainer(thead, tbody, h4, table, null, null, null, null);
    });
}

var FillTableHead = function (thead, type) {
    var array = null;

    if (type == "calibration")
        array = {
            1: "Planlagt start dato",
            2: "Gyldig t.o.m.",
            3: "Data 1",
            4: "Data 2",
            5: "Enhet",
            6: "Kalibreres av",
            7: "Fil navn",
            8: "Rom"
        }
    else if (type == "device")
        array = {
            1: "Enhet navn",
            2: "Beskrivelse",
            3: "Serie nummer",
            4: "Høyde(m)",
            5: "Vekt(kg)",
            6: "Rack monterbar",
            7: "Model",
            8: "Merke",
            9: "Input spenning(V)",
            10: "Rom"
        }
    else if (type == "mini")
        array = {
            1: "Planlagt dato",
            2: "Gyldig t.o.m.",
            3: "Kalibreres av",
            4: "Rom"
        }
    else if (type == "file")
        array = {
            1: "Fil navn",
            2: "Fil type",
            3: "Fil størrelse",
            4: "Dato lagt inn"
        }

    var tr = document.createElement("tr");
    jQuery.each(array, function (index, item) {
        var th = document.createElement("th");
        th.textContent = item;
        if (type != "mini") {
            var input = document.createElement("button");
            input.type = "button";
            input.className = "btn btn-xs";
            input.onclick = function () {
                asc1 *= -1; asc2 *= 1; asc3 *= 1;
                var rows = tbody.rows,
                rlength = rows.length,
                arr = new Array(),
                i, j, cells, clen;
                for (i = 0; i < rlength; i++) {
                    cells = rows[i].cells;
                    clen = cells.length;
                    arr[i] = new Array();
                    for (j = 0; j < clen; j++) {
                        arr[i][j] = cells[j].innerHTML;
                    }
                }
                arr.sort(function (a, b) {
                    var glyph = thead.children[0].children[index-1].children[0].children[0];
                    if (glyph.className === "glyphicon glyphicon-arrow-up")
                        glyph.className = "glyphicon glyphicon-arrow-down";
                    else
                        glyph.className = "glyphicon glyphicon-arrow-up";

                    return (a[index - 1] == b[index - 1]) ? 0 : ((a[index - 1] > b[index - 1]) ? asc1 : -1 * asc1);
                });
                for (i = 0; i < rlength; i++) {
                    rows[i].innerHTML = "<td>" + arr[i].join("</td><td>") + "</td>";
                }
            }
            var span = document.createElement("span");
            span.className = "glyphicon glyphicon-arrow-down";
            input.appendChild(span);
            th.appendChild(input);
            if (th.textContent == "Data 1" || th.textContent == "Data 2" || th.textContent == "Rom" || th.textContent == "Høyde(m)" ||
                th.textContent == "Vekt(kg)" || th.textContent == "Merke" || th.textContent == "Beskrivelse")
                th.hidden = true;
            tr.appendChild(th);
            thead.appendChild(tr);
        }
        else {
            $("#thead").empty();
            tr.appendChild(th);
            $("#thead").append(tr);
        }
    });
}

var FillCalibrationTableBoby = function (tbody, startDate, endDate, companyList, fileList, roomList, logEvent, device, deviceList) {
    tr = document.createElement("tr");

    var td = document.createElement("td");
    var month = startDate.getMonth() + 1;
    td.textContent = String(startDate.getDate() + "." + month + "." + startDate.getFullYear());
    tr.appendChild(td);

    td = document.createElement("td");
    var endMonth = endDate.getMonth() + 1;
    td.textContent = String(endDate.getDate() + "." + endMonth + "." + endDate.getFullYear());
    tr.appendChild(td);

    td = document.createElement("td");
    td.textContent = String(logEvent["Data1"]);
    td.hidden = true;
    tr.appendChild(td);

    td = document.createElement("td");
    td.textContent = String(logEvent["Data2"]);
    td.hidden = true;
    tr.appendChild(td);

    if (deviceList == null) {
        td = document.createElement("td");
        td.textContent = String(device["Name"]);
        tr.appendChild(td);
    }
    else {
        jQuery.each(deviceList, function (i, device) {
            if (String(logEvent["DeviceID"]).match(String(device["DeviceID"]))) {
                var td = document.createElement("td");
                td.textContent = String(device["Name"]);
                tr.appendChild(td);
            }
        });
    }
    jQuery.each(companyList, function(i, company) {
        if(String(company["CompanyID"]).match(String(logEvent["CompanyID"]))) {
            var td = document.createElement("td");
            td.textContent = String(company["Name"]);
            tr.appendChild(td);
        }
    });
    td = document.createElement("td");
    jQuery.each(fileList, function(i, file) {
        if(String(file["FileID"]).match(String(logEvent["FileID"])))
        {
            td.textContent = String(file["FileName"]);
            tr.appendChild(td);
        }
    });
    if(td.textContent == "")
    {
        td.textContent = "Eksisterer ikke fil";
        tr.appendChild(td)
    }
    tr = FillRoomTDInTR(logEvent, roomList, tr, null);

    var id = logEvent["LogEventID"];
    var td = document.createElement("td");
    var group = document.createElement("div");
    group.className = "btn-group";

    var inputDet = document.createElement("button");
    inputDet.type = "button";
    inputDet.className = "btn btn-success";
    inputDet.textContent = "Detaljer";
    inputDet.onclick = function() {
        window.location.replace("/Kalibrering/CalibrationViewDetails/" + id);
    };
    group.appendChild(inputDet);

    if (deviceList == null) {
        var inputEdit = document.createElement("button");
        inputEdit.type = "button";
        inputEdit.className = "btn btn-warning";
        inputEdit.textContent = "Rediger kalibrering";
        inputEdit.onclick = function () {
            window.location.replace("/Kalibrering/EditCalibration/" + id);
        };
        group.appendChild(inputEdit);
    }

    td.appendChild(group);
    tr.appendChild(td);

    tbody.appendChild(tr);
}

var AddTableToContainer = function (thead, tbody, h4, table, hr, div, br, brr) {
    if (tbody != null && tbody.childElementCount !== 0) {
        table.appendChild(thead);
        table.appendChild(tbody);
        $("#tableContainer").append(h4);
        if(hr != null)
            $("#tableContainer").append(hr);
        if (div != null)
            $("#tableContainer").append(div);
        if (br != null)
            $("#tableContainer").append(br);
        if (brr != null)
            $("#tableContainer").append(brr);
        $("#tableContainer").append(table);
        th = thead;
        tb = tbody;
    }
}

var FillRoomTDInTR = function (logEvent, roomList, tr, device) {
    jQuery.each(roomList, function (i, room) {
        if (logEvent != null && String(room["RoomID"]).match(String(logEvent["RoomID"])) || device != null && String(room["RoomID"]).match(String(device["RoomID"]))) {
            var td = document.createElement("td");
            td.textContent = String(room["Name"]);
            td.hidden = true;
            tr.appendChild(td);
        }
    });
    return tr;
}

var GetDeviceTable = function (deviceTypeList, deviceList, roomList) {
    jQuery.each(deviceTypeList, function (i, deviceType) {
        if (String(deviceType["CanCalibrateID"]) === "1") {
            var h4 = document.createElement("h4");
            h4.textContent = String(deviceType["Type"]) + " - " + String(deviceType["Description"]);

            var table = document.createElement("table");
            table.className = "table";
            var thead = document.createElement("thead");
            thead.setAttribute("id", "thead");
            var tbody = document.createElement("tbody");
            tbody.setAttribute("id", "tbody")

            FillTableHead(thead, "device");

            jQuery.each(deviceList, function (i, device) {
                if (String(deviceType["DeviceTypeID"]).match(device["DeviceTypeID"])) {
                    tr = document.createElement("tr");

                    var td = document.createElement("td");
                    td.textContent = String(device["Name"]);
                    tr.appendChild(td);

                    td = document.createElement("td");
                    td.textContent = String(device["Description"]);
                    td.hidden = true;
                    tr.appendChild(td);

                    td = document.createElement("td");
                    td.textContent = String(device["SerialNum"]);
                    tr.appendChild(td);

                    td = document.createElement("td");
                    var input = device["Height"];
                    var output = input / 1000;
                    td.textContent = String(output);
                    td.hidden = true;
                    tr.appendChild(td);

                    td = document.createElement("td");
                    var input = device["Weight"];
                    var output = input / 1000;
                    td.textContent = String(output);
                    td.hidden = true;
                    tr.appendChild(td);

                    td = document.createElement("td");
                    var input = document.createElement("input");
                    input.type = "checkbox";
                    input.disabled = true;
                    if (String(device["IsRackMountable"]) == "true")
                        input.checked = true;
                    td.appendChild(input);
                    tr.appendChild(td);

                    td = document.createElement("td");
                    td.textContent = String(device["Model"]);
                    tr.appendChild(td);

                    td = document.createElement("td");
                    td.textContent = String(device["Brand"]);
                    td.hidden = true;
                    tr.appendChild(td);

                    td = document.createElement("td");
                    td.textContent = String(device["InputVoltage"]);
                    tr.appendChild(td);

                    tr = FillRoomTDInTR(null, roomList, tr, device);

                    var id = device["DeviceID"];
                    var td = document.createElement("td");

                    var inputCre = document.createElement("button");
                    inputCre.type = "button";
                    inputCre.className = "btn btn-success";
                    inputCre.textContent = "Opprett kalibrering";
                    inputCre.onclick = function () {
                        window.location.replace("/Kalibrering/Create/" + id);
                    };
                    var inputDet = document.createElement("button");
                    inputDet.type = "button";
                    inputDet.className = "btn btn-success";
                    inputDet.textContent = "Detaljer";
                    inputDet.onclick = function () {
                        window.location.replace("" + id);
                    };
                    var inputEdit = document.createElement("button");
                    inputEdit.type = "button";
                    inputEdit.className = "btn btn-warning";
                    inputEdit.textContent = "Rediger enhet";
                    inputEdit.onclick = function () {
                        window.location.replace("" + id);
                    };

                    var group = document.createElement("div");
                    group.className = "btn-group";
                    group.appendChild(inputDet);
                    group.appendChild(inputEdit);
                    group.appendChild(inputCre);

                    td.appendChild(group);
                    tr.appendChild(td);

                    tbody.appendChild(tr);
                }
            });
        }
        AddTableToContainer(thead, tbody, h4, table, null, null, null, null);
    });
}
