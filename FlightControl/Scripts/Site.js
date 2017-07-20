function DoStuff() {
    $.get("api/Airport/AddPlane", function (data) {
        alert(data);
    });
}