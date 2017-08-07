function AddPlane(i) {
    $.get("/api/Airport/AddPlane?state=" + i, function (data) {

    });
}
function CloseStation() {
    $.get("/api/Airport/CloseStation?station=" + $("#station").val(), function (data) {

    });
}
function OpenStation() {
    $.get("/api/Airport/OpenStation?station=" + $("#station").val(), function (data) {

    });
}
function EmergencyLanding() {
    $.get("/api/Airport/EmergencyLand?station=" + $("#station").val(), function (data) {

    });
}
function Save() {
    $.get("/api/Airport/Backup", function (data) {
        //TODO:figure out a way to save without exiting/exit without saving maybe
    });
}
function UpdateMap(data) {
    if (data.Code == 4) {//left the system

    }
    else if (data.Code == 5) {//moved

    }
    else if (data.Code == 6) {//open
        $("#station" + data.StationID).css("border", "5px solid lawngreen");
    }
    else if (data.Code == 7) {//closed
        $("#station" + data.StationID).css("border", "5px solid red");
    }
}

var scrolling = true;
setInterval(function () {
    try {
        $.get("/api/Airport/GetLog", function (data) {
            if (data !== null) {
                //data.StationID
                //data.Message
                //data.Code
                var para = document.createElement("p");
                var number = parseInt(data.Message.substring(data.Message.indexOf('#') + 1));
                para.style.color = "#" + number;
                var node = document.createTextNode(data.Message);
                para.appendChild(node);
                $("#logger").append(para);
                if (data.Code >= 4 && data.Code <= 7) {
                    UpdateMap(data);
                }

                if (scrolling)
                    $('#logger').scrollTop($('#logger')[0].scrollHeight);
            }
        });
    } catch (e) {

    }
    try {
        for (var i = 1; i < 10; i++) {
            (function (n) {
                $.get("/api/Airport/GetStation?station=" + n, function (data) {
                    //data.Station
                    //data.Plane.(ID,IsWorking,Landing,LifeSpan)
                    //data.Active
                    //data.Arrival

                    $("#station" + n).css("border", data.Active ? "5px solid lawngreen" : "5px solid red");
                    if (data.Plane != null) {
                        $("#station" + n).empty();
                        $("#station" + n).append("✈");
                        var planetime = new Date(new Date(Date.now()).getTime() - new Date(data.Arrival).getTime());
                        $("#station" + n).css('color', '#' + data.Plane.ID);
                        $("#station" + n).append(planetime.getMinutes() + ":" + planetime.getSeconds());
                        $("#station" + n).append("\n#" + data.Plane.ID);
                        $("#station" + n).append("\n" + (data.Plane.Landing ? "Landing" : "Departing"));

                    }
                    else $("#station" + n).empty();
                });
            })(i);
        }
    } catch (e) {

    }
    
    
}, 500);