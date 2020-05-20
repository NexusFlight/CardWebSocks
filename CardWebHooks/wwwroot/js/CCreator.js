"use strict";

var deckID = new URLSearchParams(window.location.search).get('deckId');
var connection = new signalR.HubConnectionBuilder().withUrl("/ccreatorhub").build();
//Disable send button until connection is established




connection.start().then(function () {
    connection.invoke("SetDataBase", deckID);
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("SetDbId", function (dbID) {
    document.location.href = "/CCreator?deckId=" + dbID;
});

document.getElementById("CreateDeck").addEventListener("click", function (event) {
    var name = document.getElementById("nameInput").value;

    connection.invoke("UpdateDeckName", deckID, name).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("CreateBlackCard").addEventListener("click", function (event) {
    var text = document.getElementById("blackCardText").value;
    var picks = document.getElementById("blackCardPick").value;

    connection.invoke("AddBlackCard", deckID, text, picks).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("blackCardText").value = "";
    document.getElementById("blackCardPick").value = "";
    event.preventDefault();
});

document.getElementById("CreateWhiteCard").addEventListener("click", function (event) {
    var text = document.getElementById("WhiteCardText").value;

    connection.invoke("AddWhiteCard", deckID, text).catch(function (err) {
        return console.error(err.toString());
    });

    document.getElementById("WhiteCardText").value = "";
    event.preventDefault();
});

document.getElementById("DownloadCards").addEventListener("click", function (event) {
    connection.invoke("GetDeckAsJson", deckID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("DownloadCards", function (cards) {
    var hiddenElement = document.createElement('a');

    hiddenElement.href = 'data:attachment/text,' + encodeURI(cards);
    hiddenElement.target = '_blank';
    hiddenElement.download = 'cards.Json';
    hiddenElement.click();
});
connection.on("RecieveBlackCards", function (blackCards) {
    document.getElementById("blackCards").innerHTML = "";

    for (var i = 0; i < blackCards.length; i++) {
        document.getElementById("blackCards").innerHTML += "<div id=\"blackCard\" onClick=\"DeleteConfirm(this,\'B"+i+"\')\">" + blackCards[i].text + "<br> Pick: " + blackCards[i].pick + "</div>";
    }
});

connection.on("RecieveWhiteCards", function (whiteCards) {
    document.getElementById("whiteCards").innerHTML = "";
    for (var i = 0; i < whiteCards.length; i++) {
        document.getElementById("whiteCards").innerHTML += "<div id=\"card\"onClick=\"DeleteConfirm(this,\'W"+i+"\')\">" + whiteCards[i] + "</div>";
    }
});

connection.on("DeckName", function (Name) {
    document.getElementById("nameInput").value = Name;
});

function DeleteConfirm(e,index) {
    if (!e.innerHTML.includes("Delete?")) {
        e.innerHTML += "<br/><a onClick=\"Delete(this,\'"+index+"\')\">Delete?</a><br/><a onClick=\"RefreshCards()\" id=\"No\">NO!</a>";
    }
}

function RefreshCards() {
    connection.invoke("GetAllCards", deckID).catch(function (err) {
        return console.error(err.toString());
    });
}

function Delete(e, index) {
    connection.invoke("RemoveCard", deckID, index).catch(function (err) {
        return console.error(err.toString());
    });
}