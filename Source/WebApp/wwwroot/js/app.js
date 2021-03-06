﻿let ALL_IMAGES = [];
let CURRENT_IMAGE = '';
let CURRENT_IMAGE_INDEX = -1;

window.addEventListener('load', function () {
    // init calendar
    vanillaCalendar.init({
        disablePastDays: false
    });

    // get calendar events
    getEvents();

    // setup time & display
    showTime();

    // weather area
    getWeather();

    // get & setup images
    getImages();
});

function showTime() {
    setInterval(() => {
        const now = new Date();
        const hour = now.getHours();
        const min = now.getMinutes();

        const hh = hour < 10 ? `0${hour}` : hour;
        const mm = min < 10 ? `0${min}` : min;
        document.querySelector("#time").innerHTML = `${hh}:${mm}`;
    }, 10000);
}

function showImages() {
    if (ALL_IMAGES.length > 0) {
        setInterval(() => {
            if (CURRENT_IMAGE_INDEX < ALL_IMAGES.length - 1) {
                CURRENT_IMAGE_INDEX += 1;
            } else {
                CURRENT_IMAGE_INDEX = 0;
            }
            CURRENT_IMAGE = ALL_IMAGES[CURRENT_IMAGE_INDEX];
            document.querySelector("#mainPhoto").setAttribute("src", `/images/${CURRENT_IMAGE}`);
        }, 10000);
    }
}

function getImages() {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            ALL_IMAGES = JSON.parse(this.responseText);
            showImages();
            return;
        }
    };

    xhttp.open("GET", "/api/getimages");
    xhttp.send();
}

function getWeather() {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            let weatherInfo = JSON.parse(this.responseText);
            let weatherTD = "";

            for (var i = 0; i < weatherInfo.length; i++) {
                weatherTD += `<td>
                                <span class="forecast hour">${weatherInfo[i].time}</span>
                                <span class="forecast temp">
                                    ${weatherInfo[i].temp} °C
                                </span>
                                <img class="forecast-icon" src="/img/${weatherInfo[i].icon}.png" />
                                <span class="forecast description">${weatherInfo[i].description.toUpperCase()}</span>
                            </td>`;
            }

            document.querySelector("#forecast-row").innerHTML = weatherTD;
        }
    };

    xhttp.open("GET", "/api/getweather", true);
    xhttp.send();
}

function getEvents() {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            let eventsObj = JSON.parse(this.responseText);
            if (eventsObj) {
                // set holidays
                let holidays = eventsObj["holidays"];
                for (let i = 0; i < holidays.length; i++) {
                    let div = document.querySelector(`div[data-id="${holidays[i]}"]`);
                    if (div)
                        div.classList.add('weekend');
                }
                // show thisday events
                let thisDay = eventsObj["today"];
                let msg = "";
                for (let i = 0; i < thisDay.length; i++) {
                    msg += `&bull; ${thisDay[i]}`;
                }

                if (msg) {
                    document.querySelector('.marquee').innerHTML = `<span>${msg}</span>`;
                }
                else {
                    document.querySelector('.marquee').classList.add('hidden');
                }
            }
        }
    };

    xhttp.open("GET", "/api/getevents", true);
    xhttp.send();
}