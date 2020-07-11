const connection = new signalR.HubConnectionBuilder()
    .withUrl("/mainHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("connected");
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};

connection.onclose(async () => {
    await start();
});

// Start the connection.
start();

connection.on("BroadcastData", (imageUrl) => {
    const div = document.createElement("div");
    div.margin = 15;
    const img = document.createElement("img");
    img.src = imageUrl;
    img.height = 200;
    img.weight = 200;
    div.appendChild(img);
    document.getElementById("imagesList").appendChild(div);
});