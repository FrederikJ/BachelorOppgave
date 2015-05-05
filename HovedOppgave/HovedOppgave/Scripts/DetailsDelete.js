/**
 * Inneholder alle javascript som har med details views å gjøre
 * 
 * @author Frederik Johnsen
 * Date 04.05.2015
 */

var CalibrationViewsDetails = function (device, company) {
    var lable = document.createElement("lable");
    lable.textContent = "Gyldig t.o.m."
    $("#endDate").append(lable);

    if(device["DeviceID"] != 0) {
        var a = document.createElement("a");
        a.href = "/Global/Device/" + device["DeviceID"];
        a.textContent = device["Name"];
        $("#device").append(a);
    }

    if(company["CompanyID"] != 0) {
        var a = document.createElement("a");
        a.href = "/Global/Company/" + company["CompanyID"];
        a.textContent = company["Name"];
        $("#company").append(a);
    }
}

var FileDetailsDelete = function (device, company, file, filePath) {
    $("#filePath").text(filePath);

    var lable = document.createElement("lable");
    var filesize = Math.round(file["FileSize"] / 1000);
    lable.textContent = filesize + " kb";
    $("#fileSize").append(lable);

    CalibrationViewsDetails(device, company);
}
