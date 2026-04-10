console.log("[Thousand Eyes] Service worker loaded and ready.");

const NET_LISTENER_URL = "http://localhost:4030/";

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    console.log("[Thousand Eyes] Received message from content script:", message);

    if (message.type === "SONG_CHANGED") {
        sendPostRequest(message.payload);
        sendResponse({ status: "Received and processing" });
    }

    return true;
});

async function sendPostRequest(payload) {
    console.log(`[Thousand Eyes] Sending POST to ${NET_LISTENER_URL}...`, payload);
    
    try {
        const response = await fetch(NET_LISTENER_URL, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            console.log("[Thousand Eyes] POST successful. Status:", response.status);
        } else {
            console.warn("[Thousand Eyes] POST failed. Status:", response.status);
        }
    } catch (error) {
        console.warn("[Thousand Eyes] Fetch error. Ensure the .NET listener is running at", NET_LISTENER_URL, error);
    }
}