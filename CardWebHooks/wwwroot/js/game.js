"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
//Disable send button until connection is established
document.getElementById("joinButton").disabled = true;

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
        document.getElementById("players").innerHTML += "<li>"+names[i]+"</li>";
    }
    
});

connection.start().then(function () {
    document.getElementById("joinButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("joinButton").addEventListener("click", function (event) {
    var Uuusid = localStorage.getItem('id');
    var user = document.getElementById("nameInput").value;
    if (Uuusid == null) {
        Uuusid = uuid.v4()
        localStorage.setItem('id', Uuusid);
    }
    connection.invoke("handleUUID", Uuusid, user).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});



connection.on("ReceiveHand", function (hand) {

    //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //var encodedMsg = user + " says " + msg;
    //var li = document.createElement("li");
    //li.textContent = encodedMsg;
    let dom = new DOMParser();
    document.getElementById("cards").innerHTML = "";
    for (var i = 0; i < hand.length; i++) {
        document.getElementById("cards").innerHTML += "<div id=\"card\" onclick=\"clickCard(this)\">" + dom.parseFromString(hand[i], "text/html").body.innerHTML + "</div>";
        //document.getElementById("cards").innerHTML += "<br>";


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
    connection.invoke("handleUUID", Uuusid).catch(function (err) {
        return console.error(err.toString());
    });

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


connection.on("CardCzar", function () {

    var divs = document.getElementsByTagName("div");

    for (var i = 0; i < divs.length; i++) {
        
        if (divs[i].id == "card") {
            console.log(divs[i].id);
            divs[i].style.backgroundColor = "gray";
        }
    }

});