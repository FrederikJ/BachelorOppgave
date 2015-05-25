/**
 * Inneholder alle javascript som har med details views å gjøre
 * 
 * @author Frederik Johnsen
 * Date 04.05.2015
 */

/**
 * setter inn informasjon til et log event i en detlje side eller slette side
 */
var CalibrationViewsDetails = function (logevent, eventType, device, company, room, file) {
    //Log event
    if (logevent != null) {
        //oppretter den ytteste div blokken
        var div = document.createElement("div");
        div.className = "col-lg-12";
        //overskrift
        var h4 = document.createElement("h4");
        h4.textContent = "Kalibrering";
        //skiller overskriften fra informasjonen
        var hr = document.createElement("hr");
        //oppretter dl blokk hvor all infoen ligger
        var dl = document.createElement("dl");
        dl.className = "dl-horizontal";
        var dt = document.createElement("dt");
        dt.textContent = "Registrert dato";
        var dd = document.createElement("dd");
        var date = new Date($.parseJSON(String(logevent["RegisteredDate"]).substr(6, 13)));
        var month = date.getMonth() + 1;
        if(date.getFullYear() > 2000)
            dd.textContent = String(date.getDate() + "." + month + "." + date.getFullYear());
        dl.appendChild(dt);
        dl.appendChild(dd);

        dt = document.createElement("dt");
        dt.textContent = "Planlagt dato";
        dd = document.createElement("dd");
        date = new Date($.parseJSON(String(logevent["StartDate"]).substr(6, 13)));
        month = date.getMonth() + 1;
        if (date.getFullYear() > 2000)
            dd.textContent = String(date.getDate() + "." + month + "." + date.getFullYear());
        dl.appendChild(dt);
        dl.appendChild(dd);

        dt = document.createElement("dt");
        var lable = document.createElement("lable");
        lable.textContent = "Gyldig t.o.m.";
        dt.appendChild(lable);
        dd = document.createElement("dd");
        date = new Date($.parseJSON(String(logevent["EndDate"]).substr(6, 13)));
        month = date.getMonth() + 1;
        if (date.getFullYear() > 2000)
            dd.textContent = String(date.getDate() + "." + month + "." + date.getFullYear());
        dl.appendChild(dt);
        dl.appendChild(dd);

        dt = document.createElement("dt");
        dt.textContent = "Data 1";
        dd = document.createElement("dd");
        dd.textContent = logevent["Data1"];
        dl.appendChild(dt);
        dl.appendChild(dd);

        dt = document.createElement("dt");
        dt.textContent = "Data 2";
        dd = document.createElement("dd");
        dd.textContent = logevent["Data2"];
        dl.appendChild(dt);
        dl.appendChild(dd);

        //legger alt til div blokken
        div.appendChild(h4);
        div.appendChild(hr);
        div.appendChild(dl);
        //legger div blokken til viewet
        $("#fillDetails").append(div);
    }
    
    //Event Type
    if (eventType != null) {
        var div = document.createElement("div");
        div.className = "col-lg-12";
        h4 = document.createElement("h4");
        h4.textContent = "Event type";
        hr = document.createElement("hr");
        dl = document.createElement("dl");
        dl.className = "dl-horizontal";
        dt = document.createElement("dt");
        dt.textContent = "Event type";
        dd = document.createElement("dd");
        dd.textContent = eventType["Name"];
        dl.appendChild(dt);
        dl.appendChild(dd);

        div.appendChild(h4);
        div.appendChild(hr);
        div.appendChild(dl);
        $("#fillDetails").append(div);
    }

    //Enhet
    if (device != null) {
        div = document.createElement("div");
        div.className = "col-lg-12";
        h4 = document.createElement("h4");
        h4.textContent = "Utstyrsenhet";
        hr = document.createElement("hr");
        dl = document.createElement("dl");
        dl.className = "dl-horizontal";
        dt = document.createElement("dt");
        dt.textContent = "Enhet";
        dd = document.createElement("dd");
        if (device["DeviceID"] != 0) {
            var a = document.createElement("a");
            a.href = "/DeviceViews/DeviceDetIndex" + device["DeviceID"];
            a.textContent = device["Name"];
            dd.appendChild(a);
        }
        dl.appendChild(dt);
        dl.appendChild(dd);

        div.appendChild(h4);
        div.appendChild(hr);
        div.appendChild(dl);
        $("#fillDetails").append(div);
    }
    
    //Organisasjon
    if (company != null) {
        div = document.createElement("div");
        div.className = "col-lg-12";
        h4 = document.createElement("h4");
        h4.textContent = "Organisasjon";
        hr = document.createElement("hr");
        dl = document.createElement("dl");
        dl.className = "dl-horizontal";
        dt = document.createElement("dt");
        dt.textContent = "Kalibreres av";
        dd = document.createElement("dd");
        if (company["CompanyID"] != 0) {
            var a = document.createElement("a");
            a.href = "/CompanyViews/Company/" + company["CompanyID"];
            a.textContent = company["Name"];
            dd.appendChild(a);
        }
        dl.appendChild(dt);
        dl.appendChild(dd);

        div.appendChild(h4);
        div.appendChild(hr);
        div.appendChild(dl);
        $("#fillDetails").append(div);
    }

    //Rom
    if (room != null) {
        div = document.createElement("div");
        div.className = "col-lg-12";
        h4 = document.createElement("h4");
        h4.textContent = "Rom";
        hr = document.createElement("hr");
        dl = document.createElement("dl");
        dl.className = "dl-horizontal";
        dt = document.createElement("dt");
        dt.textContent = "Navn";
        dd = document.createElement("dd");
        dd.textContent = room["Name"];
        dl.appendChild(dt);
        dl.appendChild(dd);

        div.appendChild(h4);
        div.appendChild(hr);
        div.appendChild(dl);
        $("#fillDetails").append(div);
    }
    
    //Sertifikat
    if (file != null) {
        div = document.createElement("div");
        div.className = "col-lg-12";
        h4 = document.createElement("h4");
        h4.textContent = "Sertifikat";
        hr = document.createElement("hr");
        dl = document.createElement("dl");
        dl.className = "dl-horizontal";
        dt = document.createElement("dt");
        dt.textContent = "Fil navn";
        dd = document.createElement("dd");
        dd.textContent = file["FileName"];
        dl.appendChild(dt);
        dl.appendChild(dd);

        div.appendChild(h4);
        div.appendChild(hr);
        div.appendChild(dl);
        $("#fillDetails").append(div);
    }
}

/**
 * legger til informasjon til et fil details/delete view som trengs
 */
var FileDetailsDelete = function (device, company, file, filePath) {
    $("#filePath").text(filePath);

    var lable = document.createElement("lable");
    var filesize = Math.round(file["FileSize"] / 1000);
    lable.textContent = filesize + " kb";
    $("#fileSize").append(lable);

    CalibrationViewsDetails(device, company);
}
