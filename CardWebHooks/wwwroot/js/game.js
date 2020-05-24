"use strict";

var gameID = new URLSearchParams(window.location.search).get('gid');
var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
let dom = new DOMParser();
//Disable send button until connection is established
document.getElementById("startButton").hidden = true;
document.getElementById("setName").disabled = true;
document.getElementById("openSideNav").hidden = true;




connection.start().then(function () {
    document.getElementById("setName").disabled = false;
    connection.invoke("GetGameFromID", gameID);
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("RecieveDeckConfig", function (decks) {
    document.getElementById("deckOptions").innerHTML = "";
    for (var i = 0; i < decks.length; i++) {
        if (decks[i].item2 == true) {
            document.getElementById("deckOptions").innerHTML += "<input type=\"checkbox\" id=\"deckOption\" name=\"" + i + " \" value =\"" + decks[i].item1 + "\"/ checked> ";
        } else {
            document.getElementById("deckOptions").innerHTML += "<input type=\"checkbox\" id=\"deckOption\" name=\"" + i + " \" value =\"" + decks[i].item1 + "\"/ > ";
        }
        document.getElementById("deckOptions").innerHTML += "<label for=\"" + i + " \">" + decks[i].item1 + "</label>";
    }

});

connection.on("ReturnToLobby", function (user, message) {
    document.location.href = "/Index";
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
    console.log(names);
    for (var i = 0; i < names.length; i++) {
        document.getElementById("players").innerHTML += "<li id=\"" + names[i].substr(0, names[i].indexOf(' '))+"\">" + names[i] + "</li>";
    }

});
connection.on("RecieveWinner", function (winner) {
    document.getElementById(winner).className = "winner";
    
});


connection.on("GameStarter", function () {
    console.log("YOU ARE THE OVERLORD");
    document.getElementById("startButton").hidden = false;
    document.getElementById("openSideNav").hidden = false;

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
    connection.invoke("StartGame", decks, gameID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


connection.on("CardCzar", function () {

    var divs = document.getElementsByTagName("div");

    for (var i = 0; i < divs.length; i++) {

        if (divs[i].id == "card") {
            divs[i].style.backgroundColor = "gray";
        }
    }

});

document.getElementById("setName").addEventListener("click", function (event) {
    var user = document.getElementById("nameInput").value;
    connection.invoke("SetName", user, gameID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("ReceiveHand", function (hand) {

    
    document.getElementById("cards").innerHTML = "";
    for (var i = 0; i < hand.length; i++) {
        document.getElementById("cards").innerHTML += "<div id=\"card\" onclick=\"clickCard(" + i + ")\">" + dom.parseFromString(hand[i], "text/html").body.innerHTML + "</div>";
    }

});

connection.on("RecieveBlackCard", function (BCardt, BCardp) {
    document.getElementById("gameSpace").innerHTML = "<hr /><div id=\"topLine\"><div id=\"blackCard\"></div><div id=\"selectedCards\"></div></div><div id=\"cards\"></div>";
    document.getElementById("blackCard").innerHTML = "<div id=\"BCard\">" + BCardt + "<br>" + BCardp + "</div>";
});
connection.on("RecieveSelWCard", function (card) {
    document.getElementById("selectedCards").innerHTML += "<div id=\"selectedCard\">" + dom.parseFromString(card, "text/html").body.innerHTML + "</div>";
});
connection.on("UUIDHandler", function () {
    var Uuusid = localStorage.getItem('id');
    if (Uuusid == null) {
        Uuusid = uuid.v4()
        localStorage.setItem('id', Uuusid);
    }
    connection.invoke("HandleUUID", Uuusid, gameID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function clickCard(card) {
    connection.invoke("ClickCard", localStorage.getItem('id'), card, gameID).catch(function (err) {
        return console.error(err.toString());
    });
}

function selectCard(card) {

    connection.invoke("SelectWinningCard", localStorage.getItem('id'), card.innerHTML, gameID).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.on("ShowWhiteCards", function (whiteCards) {
    document.getElementById("selectedCards").innerHTML = "";
    for (var i = 0; i < whiteCards.length; i++) {
        document.getElementById("selectedCards").innerHTML += "<div id=\"selectedCard\" onclick=\"selectCard(this)\">" + dom.parseFromString(whiteCards[i], "text/html").body.innerHTML + "</div>";
    }

});


connection.on("RefreshHand", function () {

    connection.invoke("RefreshHand", localStorage.getItem('id'), gameID).catch(function (err) {
        return console.error(err.toString());
    });

});

connection.on("ClearSelectedCards", function () {

    document.getElementById("selectedCards").innerHTML = "";

}); 

connection.on("GameOver", function (player) {
    console.log(player);
    document.getElementById("gameSpace").innerHTML = "GAME OVER! "+player.name+" Won with "+player.points+" points!";

});

document.getElementById("AddDeck").addEventListener("click", function (event) {
    var deckID = document.getElementById("DeckInput").value;
    connection.invoke("ImportFromDB", gameID, deckID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});



