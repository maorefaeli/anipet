var queryURL = "https://openweathermap.org/data/2.5/weather?q=Rishon LeZiyyon&appid=b6907d289e10d714a6e88b30761fae22";

$.getJSON(queryURL, function (data) {
    var temp = Math.floor(data.main.temp);

    $("#temperatureDiv p").html(temp + " degrees");

    if (temp >= 30) {
        $("#temperatureDiv").append("<img class=\"tempartureImg\" src=\"/content/Images/dog-sweating.png\">");
    } else if (temp >= 18) {
        $("#temperatureDiv").append("<img class=\"tempartureImg\" src=\"/content/Images/cool-dog.png\">");
    } else {
        $("#temperatureDiv").append("<img class=\"tempartureImg\" src=\"/content/Images/cold-dog.png\">");
    }
})