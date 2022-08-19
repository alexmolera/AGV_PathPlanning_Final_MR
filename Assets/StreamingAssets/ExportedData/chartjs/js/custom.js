var xmlhttp = new XMLHttpRequest();
var url = "./jsonPoints.json";

let data;
xmlhttp.open("GET", url, true);
xmlhttp.send();
xmlhttp.onreadystatechange = function(){
    if(xmlhttp.readyState === XMLHttpRequest.DONE && xmlhttp.status === 200){
        data = JSON.parse(this.responseText);
        //console.log(data);

        //Get data from JSON

        var road_ = data.Items.map(function (elem) {
            return elem.road;
        });
        var x_ = data.Items.map(function(elem){
            return elem.x;
        });
        var y_ = data.Items.map(function(elem){
            return elem.y;
        });

        //Create struct with coordinates:
        function point(_x, _y){
            var xy_ = {
                x: _x,
                y: _y
            }  
            return xy_; 
        }

        //Create array of struct:
        var points = [];
        var points2 = [];
        for (let i = 0; i < road_.length; i++) {
            if (road_[i] == 1) {
                points.push(point(x_[i], y_[i]));
            }
            else if (road_[i] == 2) {
                points2.push(point(x_[i], y_[i]));
            }
        }

        //Create Canvas
        const ctx = document.getElementById('canvas').getContext('2d');
        const myChart = new Chart(ctx, {
            type: 'scatter',
            data: {
                datasets: [{
                    label: 'Estimated Waypoints',
                    data: points,
                    backgroundColor: 'rgb(255, 99, 132)',
                    borderColor: 'red',
                    showLine : true,
                    tension: 0
                    },
                    {
                    label: 'Real Waypoints',
                    data: points2,
                    backgroundColor: 'rgb(0, 200, 132)',
                    borderColor: 'green',
                    showLine : true,
                    tension: 0.1
                    },
                    {
                    label: 'QR Marker Origin',
                    data: [{
                        x:0,
                        y:0
                    }],
                    backgroundColor: 'black',
                    borderColor: 'black',
                    showLine : false,
                    tension: 0,
                    radius: 10,
                    pointStyle: 'cross'
                    },
                    {
                    label: 'START',
                    data: [{
                        x: x_[0],
                        y: y_[0]
                    }],
                    backgroundColor: 'green',
                    borderColor: 'green',
                    showLine : false,
                    tension: 0,
                    radius: 7 
                    }],
        },
            options: {
                scales: {
                    x:{
                        type: 'linear',
                        reverse: true,
                        suggestedMin: -1.5,
                        suggestedMax: 1.5,
                        beginAtZero: true,
                        offset: true,
                        title: {
                            display: true, 
                            text: 'X Axis'
                        }
                    },
                    y:{
                        type: 'linear',
                        reverse: true,
                        beginAtZero: true,
                        suggestedMin: -1.5,
                        suggestedMax: 1.5,
                        offset: true,
                        title: {
                            display: true, 
                            text: 'Z Axis'
                        }
                    }
                },
                aspectRatio:1 //Square
            }
        });
            
        
    }
}



//In order to read a JSON file with JavaScript you need 
//to make a network request to the file that contains your JSON data.
//https://www.youtube.com/watch?v=uox7TYW2fEg
//https://vegibit.com/render-html-in-node-js/
//Instrucciones: https://stackoverflow.com/questions/16333790/node-js-quick-file-server-static-files-over-http

/*Server:
Instalar:   npm install http-server -g 
Usar:       cd D:\Folder
            http-server -a localhost -p 80
*/