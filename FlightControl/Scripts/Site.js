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
setInterval(function () {
    $.get("/api/Airport/GetLog", function (data) {
        if (data !== null) {
            var para = document.createElement("p");
            var number = parseInt(data.substring(data.indexOf('#') + 1));
            para.style.color = "#" + number;
            var node = document.createTextNode(data);
            para.appendChild(node);
            $("#logger").append(para);
        }
    });
}, 1000);