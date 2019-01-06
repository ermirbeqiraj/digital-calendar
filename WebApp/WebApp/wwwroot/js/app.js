let ALL_IMAGES = [];
let CURRENT_IMAGE = '';
let CURRENT_IMAGE_INDEX = -1;

window.addEventListener('load', function () {
    // init calendar
    vanillaCalendar.init({
        disablePastDays: true
    });

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
    console.log('get weather');
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            console.log(this.responseText);
            const weatherInfo = JSON.parse(this.responseText);
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