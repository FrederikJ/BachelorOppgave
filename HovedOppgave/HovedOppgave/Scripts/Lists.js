/**
 * Inneholder alle javascript som har med list views å gjøre
 * 
 * @author Frederik Johnsen
 * Date 04.05.2015
 */

/**
 * henter tittelene til hver eneste kolonne i en tabell og legger dem til en drop down meny
 */
var GetColName = function (thead) {
    var child = thead.children[0];
    var ddl;
    if (thead.id == "theadFiles")
        ddl = document.getElementById("ddmFile");
    else if (thead.id == "theadEvents")
        ddl = document.getElementById("ddmEvent");
    else if (thead.id == "theadGoingTo")
        ddl = document.getElementById("ddmGoingTo");
    else if (thead.id == "theadIs")
        ddl = document.getElementById("ddmIs");
    else if (thead.id == "theadHave")
        ddl = document.getElementById("ddmHave");
    else
        ddl = document.getElementById("ddm");
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

/**
 * når man har trykt på en av tittelene, så setter du en sjekk eller tar bort
 * og den kolonnen vil gjemmes eller vises
 */
var CheckColName = function (colNumber, thead, tbody, checkInput) {
    if (checkInput[0].type === 'checkbox')
        checkInput[0].onchange = HideColumn(colNumber, thead, tbody);
}

/**
 * gjemmer en hel kolonne til en tabell
 */
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
}

/**
 * opprettet en tabell til en kalibreringer
 */
var GetCalibrationTable = function (type, deviceTypeList, deviceList, logEventList, companyList,
    fileList, roomList, dateObject) {

    //setter dags dato uten klokkeslett
    var nowDate = new Date();
    nowDate.setHours(0);
    nowDate.setMinutes(0);
    nowDate.setSeconds(0);

    //sortere tabellen i enhets typer
    jQuery.each(deviceTypeList, function (i, deviceType) {
        var h4 = document.createElement("h4");
        h4.textContent = String(deviceType["Type"]) + " - " + String(deviceType["Description"]);

        var table = document.createElement("table");
        table.className = "table";
        var thead = document.createElement("thead");
        thead.setAttribute("id", "thead");
        var tbody = document.createElement("tbody");
        tbody.setAttribute("id", "tbody")

        //fyller kolonne tittlene
        FillTableHead(thead, "calibration");

        jQuery.each(deviceList, function (i, device) {
            if (String(deviceType["DeviceTypeID"]).match(device["DeviceTypeID"])) {
                jQuery.each(logEventList, function (i, logEvent) {
                    if (String(logEvent["DeviceID"]).match(device["DeviceID"])) {
                        var endDate = new Date($.parseJSON(String(logEvent["EndDate"]).substr(6, 13)));
                        var startDate = new Date($.parseJSON(String(logEvent["StartDate"]).substr(6, 13)));

                        switch (type) {
                            //de som er kalibrert
                            case "is":
                                if (nowDate >= endDate)
                                {
                                    FillCalibrationTableBoby(tbody, startDate, endDate, companyList, fileList, roomList, logEvent, device, null);
                                    break;
                                }
                                else
                                    break;
                            //de som skal kalibreres innen den neste uke
                            case "goingTo":
                                if (nowDate <= startDate && dateObject >= startDate) {
                                    FillCalibrationTableBoby(tbody, startDate, endDate, companyList, fileList, roomList, logEvent, device, null);
                                    break;
                                }
                                else
                                    break;
                            //de som er planlagt til kalibrering fra en uke og mer
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
        //legger alt til i div blokken som inneholder alt
        AddTableToContainer(thead, tbody, h4, table, null, null, null, null);
    });
}

/**
 * fyller kolonne tittlene
 */
var FillTableHead = function (thead, type) {
    var array = null;

    //etter vilken type tabell det er 
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
    else if (type == "otherCalibration")
        array = {
            1: "Registrert dato",
            2: "Data 1",
            3: "Data 2",
            4: "Enhet",
            5: "Rom"
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
    else if (type == "user")
        array = {
            1: "Navn",
            2: "Epost",
            3: "Rettighet"
        }

    //går igjennom arrayet som blir bestemt ovenfor
    var tr = document.createElement("tr");
    jQuery.each(array, function (index, item) {
        var th = document.createElement("th");
        th.textContent = item;
        if (type != "mini") {
            var input = document.createElement("button");
            input.type = "button";
            input.className = "btn btn-xs";
            input.onclick = function () {
                //setter inn sortering og funksjonen til sorteringen
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
            //disse kolonnene skal være hidden, mindre viktige
            if (type != "otherCalibration")
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

/**
 * fyller inn kroppen på kalibrerings tabellen
 */
var FillCalibrationTableBoby = function (tbody, startDate, endDate, companyList, fileList, roomList, logEvent, device, deviceList) {
    tr = document.createElement("tr");

    var td = document.createElement("td");
    var month = startDate.getMonth() + 1;
    td.textContent = String(startDate.getDate() + "." + month + "." + startDate.getFullYear());
    tr.appendChild(td);

    td = document.createElement("td");
    month = endDate.getMonth() + 1;
    td.textContent = String(endDate.getDate() + "." + month + "." + endDate.getFullYear());
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

/**
 * fyller inn kroppen til en bruker tabell
 */
var FillUserTableBody = function(tbody, right, usersList) {
    jQuery.each(usersList, function (i, user) {
        if (String(user["RightsID"]).match(String(right["RightsID"]))) {
            var tr = document.createElement("tr");

            var td = document.createElement("td");
            td.textContent = user["Name"];
            tr.appendChild(td);

            td = document.createElement("td");
            td.textContent = user["Email"];
            tr.appendChild(td);

            var rightTd = document.createElement("td");
            var rightLable = document.createElement("lable");
            rightLable.textContent = right["Name"];
            rightTd.appendChild(rightLable);
            tr.appendChild(rightTd);

            var id = user["UserID"];
            var btnTd = document.createElement("td");
            var group = document.createElement("div");
            group.className = "btn-group";
            
            var inputEdit = document.createElement("button");
            inputEdit.type = "button";
            inputEdit.className = "btn btn-warning";
            inputEdit.textContent = "Endre bruker";
            inputEdit.onclick = function () {
                window.location.replace("/Account/Manage/" + id);
            };
            group.appendChild(inputEdit);

            var inputDel = document.createElement("button");
            inputDel.type = "button";
            inputDel.className = "btn btn-danger";
            inputDel.textContent = "Slett bruker";
            inputDel.onclick = function () {
                window.location.replace("/Administrator/DeleteUser/" + id);
            };
            group.appendChild(inputDel);
            btnTd.appendChild(group);
            
            tr.appendChild(btnTd);

            tbody.appendChild(tr);
        }
    });
}

/**
 * legger inn tabllene inn i div blokken slik at dem synes i viewet
 */
var AddTableToContainer = function (thead, tbody, h4, table, hr, div, br, brr) {
    if (tbody != null && tbody.childElementCount !== 0) {
        table.appendChild(thead);
        table.appendChild(tbody);
        if (h4 != null)
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

/**
 * fyller inn rommet i kalibrering og enhet tabeller
 */
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

/**
 * oppretter enhet tabell
 */
var GetDeviceTable = function (deviceTypeList, deviceList, roomList) {
    //sortere på enhet typer hver for seg
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

            //fyller inn kolonne tittlene
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
                        window.location.replace("/DeviceViews/DeviceDetIndex/" + id);
                    };
                    var inputEdit = document.createElement("button");
                    inputEdit.type = "button";
                    inputEdit.className = "btn btn-warning";
                    inputEdit.textContent = "Rediger enhet";
                    inputEdit.onclick = function () {
                        window.location.replace("/DeviceViews/EditDevice/" + id);
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
        //legger den til i div blokk i html koden slik at det vises
        AddTableToContainer(thead, tbody, h4, table, null, null, null, null);
    });
}

/**
 * oppretter bruker tabell 
 */
var GetUserTable = function (usersList, rightsList) {
    //sortere med henhold til rettigheter
    jQuery.each(rightsList, function (i, right) {
        var h4 = document.createElement("h4");
        h4.textContent = "Rettighet - " + String(right["Name"]);

        var table = document.createElement("table");
        table.className = "table";
        var thead = document.createElement("thead");
        var tbody = document.createElement("tbody");

        //fyller inn kolonne tittlene
        FillTableHead(thead, "user");
        //fyller inn tabell kroppen
        FillUserTableBody(tbody, right, usersList);

        //legger det til en div block i html koden
        AddTableToContainer(thead, tbody, h4, table, null, null, null, null);
    });
}

/**
 * oppretter log event tabell for en enhet
 */
var GetDeviceEventTable = function (type, joinQuery) {
    //dagens dato uten klokkeslett
    var nowDate = new Date();
    nowDate.setHours(0);
    nowDate.setMinutes(0);
    nowDate.setSeconds(0);

    var table = document.createElement("table");
    table.className = "table";
    var thead = document.createElement("thead");
    var tbody = document.createElement("tbody");
    var next = joinQuery.length - 1;
    //kommer ann på vilken log event det er
    if (type != "others")
        FillTableHead(thead, "calibration");
    else
        FillTableHead(thead, "otherCalibration");
    
    //mens det fortsatt er elementer i listen
    while (next >= 0) {
        //Setter forkjellige items fra listen
        var logEvent = joinQuery[next]["LogEvent"];
        var device = joinQuery[next]["Device"];
        var eventType = joinQuery[next]["EventType"];
        var file = joinQuery[next]["File"];
        var company = joinQuery[next]["Company"];
        var room = joinQuery[next]["Room"];

        var endDate = new Date($.parseJSON(String(logEvent["EndDate"]).substr(6, 13)));
        var startDate = new Date($.parseJSON(String(logEvent["StartDate"]).substr(6, 13)));

        switch (type) {
            //de som skal kalibreres
            case "goingTo":
                if (startDate.getFullYear() > 2000 && nowDate <= startDate) {
                    FillDeviceEventTableBoby(tbody, startDate, endDate, logEvent, device, file, company, room, type, eventType);
                    joinQuery.splice($.inArray(joinQuery[next], joinQuery), 1);
                    next = joinQuery.length - 1;
                    break;
                }
                else {
                    next -= 1;
                    break;
                }
            //de som kalibreres
            case "is":
                if (startDate.getFullYear() > 2000 && endDate.getFullYear() > 2000 && nowDate >= startDate && nowDate <= endDate) {
                    FillDeviceEventTableBoby(tbody, startDate, endDate, logEvent, device, file, company, room, type, eventType);
                    joinQuery.splice($.inArray(joinQuery[next], joinQuery), 1);
                    next = joinQuery.length - 1;
                    break;
                }
                else {
                    next -= 1;
                    break;
                }
            //de som har kalibrert
            case "have":
                if (endDate.getFullYear() > 2000 && nowDate >= endDate) {
                    FillDeviceEventTableBoby(tbody, startDate, endDate, logEvent, device, file, company, room, type, eventType);
                    joinQuery.splice($.inArray(joinQuery[next], joinQuery), 1);
                    next = joinQuery.length - 1;
                    break;
                }
                else {
                    next -= 1;
                    break;
                }
            //for de andre logeventene som ikke er kalibreringer
            case "others":
                FillDeviceEventTableBoby(tbody, startDate, endDate, logEvent, device, file, company, room, type, eventType);
                joinQuery.splice($.inArray(joinQuery[next], joinQuery), 1);
                next = joinQuery.length - 1;
                break;
        }
    }
    //legger det i de forskjellige div blokkene i html koden
    switch (type) {
        case "goingTo":
            table.appendChild(thead);
            table.appendChild(tbody);
            $("#goingTo").append(table);
            break;
        case "is":
            table.appendChild(thead);
            table.appendChild(tbody);
            $("#is").append(table);
            break;
        case "have":
            table.appendChild(thead);
            table.appendChild(tbody);
            $("#have").append(table);
            break;
        case "others":
            table.appendChild(thead);
            table.appendChild(tbody);
            $("#others").append(table);
            break;
    }
    //returnerer resten av listen som ikke ble brukt
    return joinQuery;
}

/**
 * fyller inn kroppen til tabellen i metoden over
 */
var FillDeviceEventTableBoby = function (tbody, startDate, endDate, logEvent, device, file, company, room, type, eventType) {
    tr = document.createElement("tr");
    //andre logeventer som ikke er kalibreringer har ikke start og slutt dato, dermed ikke viktig
    if (type != "others") {
        var td = document.createElement("td");
        var month = startDate.getMonth() + 1;
        td.textContent = String(startDate.getDate() + "." + month + "." + startDate.getFullYear());
        tr.appendChild(td);

        td = document.createElement("td");
        month = endDate.getMonth() + 1;
        td.textContent = String(endDate.getDate() + "." + month + "." + endDate.getFullYear());
        tr.appendChild(td);
    }
    else {
        var td = document.createElement("td");
        var date = new Date($.parseJSON(String(logEvent["RegisteredDate"]).substr(6, 13)));
        var month = date.getMonth() + 1;
        td.textContent = String(date.getDate() + "." + month + "." + date.getFullYear());
        tr.appendChild(td);
    }

    td = document.createElement("td");
    if (logEvent["Data1"] != null)
        td.textContent = String(logEvent["Data1"]);
    if(type != "others")
        td.hidden = true;
    tr.appendChild(td);

    td = document.createElement("td");
    if (logEvent["Data2"] != null)
        td.textContent = String(logEvent["Data2"]);
    if (type != "others")
        td.hidden = true;
    tr.appendChild(td);

    var td = document.createElement("td");
    td.textContent = String(device["Name"]);
    tr.appendChild(td);

    //de log event som ikke er kalibreringer har heller ikke firma eller filer siden dem er intern
    if (type != "others") {
        var td = document.createElement("td");
        td.textContent = String(company["Name"]);
        tr.appendChild(td);

        td = document.createElement("td");
        if (String(file["FileName"]) != "null")
            td.textContent = String(file["FileName"]);
        tr.appendChild(td);
    }

    var td = document.createElement("td");
    td.textContent = String(room["Name"]);
    if (type != "others")
        td.hidden = true;
    tr.appendChild(td);

    var id = logEvent["LogEventID"];
    var td = document.createElement("td");
    var group = document.createElement("div");
    group.className = "btn-group";

    var inputDet = document.createElement("button");
    inputDet.type = "button";
    inputDet.className = "btn btn-success";
    inputDet.textContent = "Detaljer om";
    inputDet.onclick = function() {
        $("#fillDetails").empty();
        CalibrationViewsDetails(logEvent, eventType, device, company, room, file);
        $("#showDet").trigger('click');
    };
    group.appendChild(inputDet);

    //de som ikke er kalibreringer skal heller ikke kunne endres
    if (type != "others") {
        var inputEdit = document.createElement("button");
        inputEdit.type = "button";
        inputEdit.className = "btn btn-warning";
        inputEdit.textContent = "Rediger event";
        inputEdit.onclick = function () {
            window.location.replace("/Kalibrering/EditCalibration/" + id);
        };
        group.appendChild(inputEdit);
    }

    //de kalibreringer som enda ikke har hent kan slettes med en gang
    if (type == "goingTo") {
        var inputDel = document.createElement("button");
        inputDel.type = "button";
        inputDel.className = "btn btn-danger";
        inputDel.textContent = "Slett event";
        inputDel.onclick = function () {
            $.ajax({
                type: 'POST',
                url: '/DeviceViews/DeleteLogEvent?logEventId=' + id,
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function () {
                    tbody.removeChild(tr);
                    if (tbody.childElementCount == 0) {
                        $("#goingTo").hidden = true;
                        $("#eventDivGoingTo").hidden = true;
                    }
                },
                error: function () { }
            });
        };
        group.appendChild(inputDel);
    }

    td.appendChild(group);
    tr.appendChild(td);

    tbody.appendChild(tr);
}