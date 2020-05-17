"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/lobbyhub").build();
//Disable send button until connection is established
document.getElementById("joinGameButton").disabled = true;



connection.start().then(function () {
    document.getElementById("joinGameButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("joinGameButton").addEventListener("click", function (event) {
    var name = document.getElementById("nameInput").value;
    connection.invoke("AddGame", name).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function JoinGame(id) {
    console.log(id);
    document.location.href = "/Game?gid=" + id;
}


connection.on("ReceiveAllGames", function (games) {
    document.getElementById("games").innerHTML = "";

    console.log(games);
    for (var i = 0; i < games.length; i++) {
        document.getElementById("games").innerHTML += "<div id=\"game\" onClick=\"JoinGame('" + games[i].id + "')\">" + games[i].name + "<br> Players: " + games[i].playerCount+"</div >";
    }
    
});

connection.on("SetGameID", function (ID) {
    JoinGame(ID)
});