"use strict";

var gameID;
var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
//Disable send button until connection is established
document.getElementById("startButton").disabled = true;
document.getElementById("setName").disabled = true;



connection.start().then(function () {
    document.getElementById("setName").disabled = false;
    connection.invoke("GetGameFromID", localStorage.getItem("gameid"));
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("RecieveDeckConfig", function (deckNames) {
    document.getElementById("deckOptions").innerHTML = "";
    for (var i = 0; i < deckNames.length; i++) {
        console.log(deckNames[i]);
        document.getElementById("deckOptions").innerHTML += "<input type=\"checkbox\" id=\"deckOption\" name=\"" + i + " \" value =\"" + deckNames[i] + "\"/ > ";
        document.getElementById("deckOptions").innerHTML += "<label for=\"" + i + " \">" + deckNames[i] + "</label>";
    }

});


connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceivePlayerDetails", function (names) {
    document.getElementById("players").innerHTML = "";
    for (var i = 0; i < names.length; i++) {
        console.log(names[i]);
        document.getElementById("players").innerHTML += "<li>" + names[i] + "</li>";
    }

});

connection.on("GameStarter", function () {
    document.getElementById("startButton").disabled = false;
});

document.getElementById("startButton").addEventListener("click", function (event) {
    var decks = [];
    var options = document.getElementsByTagName("input");

    for (var i = 0; i < options.length; i++) {
        if (options[i].id == "deckOption") {
            if (options[i].checked) {
                decks.push(options[i].value);
            }
        }
    }
    connection.invoke("StartGame", decks).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


connection.on("CardCzar", function () {

    var divs = document.getElementsByTagName("div");

    for (var i = 0; i < divs.length; i++) {

        if (divs[i].id == "card") {
            console.log(divs[i].id);
            divs[i].style.backgroundColor = "gray";
        }
    }

});

document.getElementById("setName").addEventListener("click", function (event) {
    var user = document.getElementById("nameInput").value;
    connection.invoke("SetName", user).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("ReceiveHand", function (hand) {

    let dom = new DOMParser();
    document.getElementById("cards").innerHTML = "";
    for (var i = 0; i < hand.length; i++) {
        document.getElementById("cards").innerHTML += "<div id=\"card\" onclick=\"clickCard(this)\">" + dom.parseFromString(hand[i], "text/html").body.innerHTML + "</div>";
    }

});

connection.on("RecieveBlackCard", function (BCardt, BCardp) {
    document.getElementById("blackCard").innerHTML = "<div id=\"BCard\">" + BCardt + "<br>" + BCardp + "</div>";
});
connection.on("RecieveSelWCard", function (card) {
    document.getElementById("selectedCards").innerHTML += "<div id=\"selectedCard\">" + card + "</div>";
});
connection.on("UUIDHandler", function () {
    var Uuusid = localStorage.getItem('id');
    if (Uuusid == null) {
        Uuusid = uuid.v4()
        localStorage.setItem('id', Uuusid);
    }
    gameID = localStorage.getItem("gameid");
    connection.invoke("HandleUUID", Uuusid, gameID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function clickCard(card) {
    connection.invoke("ClickCard", localStorage.getItem('id'), card.innerHTML).catch(function (err) {
        return console.error(err.toString());
    });
}

function selectCard(card) {
    connection.invoke("SelectCard", localStorage.getItem('id'), card.innerHTML).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.on("ShowWhiteCards", function (whiteCards) {
    let dom = new DOMParser();
    document.getElementById("selectedCards").innerHTML = "";
    for (var i = 0; i < whiteCards.length; i++) {
        document.getElementById("selectedCards").innerHTML += "<div id=\"selectedCard\" onclick=\"selectCard(this)\">" + whiteCards[i] + "</div>";
    }

});


connection.on("RefreshHand", function () {

    connection.invoke("RefreshHand", localStorage.getItem('id')).catch(function (err) {
        return console.error(err.toString());
    });

});

connection.on("ClearSelectedCards", function () {

    document.getElementById("selectedCards").innerHTML = "";

}); 




