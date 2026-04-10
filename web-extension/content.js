console.log("[Thousand Eyes] Content script injected and running.");

let lastTrackHash = "";
let trackChangeTimeout = null;

function extractMediaData() {
    let payload = {
        title: document.title.replace(" - Spotify", "").trim(),
        artist: "Unknown Artist",
        thumbnail: ""
    };

    // Approach 1: The Native Media Session API (Bulletproof for modern players)
    if ('mediaSession' in navigator && navigator.mediaSession.metadata) {
        const meta = navigator.mediaSession.metadata;
        payload.title = meta.title || payload.title;
        payload.artist = meta.artist || payload.artist;
        
        if (meta.artwork && meta.artwork.length > 0) {
            payload.thumbnail = meta.artwork[meta.artwork.length - 1].src;
        }
    } else {
        // Approach 2: DOM Scraping Fallback specifically for Spotify's web player structure
        const titleEl = document.querySelector('[data-testid="context-item-info-title"]');
        const artistEl = document.querySelector('[data-testid="context-item-info-artist"]');
        const imgEl = document.querySelector('img[data-testid="cover-art-image"]');

        if (titleEl) payload.title = titleEl.textContent;
        if (artistEl) payload.artist = artistEl.textContent;
        if (imgEl) payload.thumbnail = imgEl.src;
    }

    return payload;
}

function notifyChange(trackData) {
    console.log(`[Thousand Eyes] Song change detected:`, trackData);
    
    chrome.runtime.sendMessage({
        type: "SONG_CHANGED",
        payload: trackData
    }, (response) => {
        if (chrome.runtime.lastError) {
             console.error("[Thousand Eyes] Error sending message to background:", chrome.runtime.lastError);
        } else {
             console.log("[Thousand Eyes] Background script acknowledged:", response);
        }
    });
}

const titleObserver = new MutationObserver(() => {
    clearTimeout(trackChangeTimeout);
    
    trackChangeTimeout = setTimeout(() => {
        const trackData = extractMediaData();

        if (trackData.title.trim() === "Spotify - Web Player") return;

        const currentHash = `${trackData.title}-${trackData.artist}`;
        
        if (currentHash !== lastTrackHash && trackData.title.trim() !== "") {
            lastTrackHash = currentHash;
            notifyChange(trackData);
        }
    }, 500); 
});

const targetTitle = document.querySelector('title');
if (targetTitle) {
    console.log("[Thousand Eyes] Observing document title for changes.");
    titleObserver.observe(targetTitle, { childList: true });
    
    setTimeout(() => {
        const trackData = extractMediaData();
        lastTrackHash = `${trackData.title}-${trackData.artist}`;
        if(trackData.title) notifyChange(trackData);
    }, 1000);
} else {
    console.warn("[Thousand Eyes] No <title> element found to observe.");
}