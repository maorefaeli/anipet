var weatherCallback = function (data) {
    var wind = data.query.results.channel.wind;
    var item = data.query.results.channel.item;
    var icon = "<i class='material-icons'>";

    if (item.condition.temp <= 20) {
        icon += "wb_cloudy</i >";
    }
    else if (item.condition.temp > 20 && item.condition.temp < 28) {
        icon += "wb_incandescent</i >";
    } else {
        icon += "wb_sunny</i >";
    }

    var text = icon + "&nbsp;&nbsp;Temperature: " + item.condition.temp + " °C";

    $("#temperatureDiv p").html(text);
};