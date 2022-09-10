let socket = new WebSocket("https://discord.com/channels/1004040791003512853/1004040791926251662")

socket.onopen = function(e) {
    alert("[open] Connection has been established with main server");
    alert("Sending message to current url paste");
    socket.send("Hello there random user");
};

socket.onmessage = function(event) {
    if (event.wasClean) {
        alert('[close] Connection closed cleanlyn code=${event.code} reason=${event.reason}')
    } else {
        // Server process killed or current network down
        // or URL not functioning correctly
        alert('[close] Connection has been lost with main source');
    }
};

socket.onerror = function(error) {
    alert('[error] ${error.message}');
}

// Get chad and host informations. 
socket.binaryType = "arraybuffer";
socket.onmessage = (event) => {
    // event.data is either a string (if text) or arraybuffer (if binary detected)
};

// every 100ms examine the socket and send more data
setInterval(() => {
    if (socket.bufferedAmount == 0) {
        socket.send(moreDat());
    }
}, 100);

// closing party:
socket.close(1000, "Task succedeed");
// other party currently closing
socket.onclose = event => {
    // event.code = 1000
    // event.reason = "Task succedeed"
    // event.wasClean === true (clean close)
}