console.log("[Thousand Eyes] Content script injected and running.");

let lastTrackHash = "";
let trackChangeTimeout = null;

function extractMediaData() {
    let payload = {
        Title: document.title.replace(" - Spotify", "").trim(),
        Artist: "Unknown Artist",
        Thumbnail: ""
    };

    // Approach 1: The Native Media Session API (Bulletproof for modern players)
    if ('mediaSession' in navigator && navigator.mediaSession.metadata) {
        const meta = navigator.mediaSession.metadata;
        payload.Title = meta.title || payload.Title;
        payload.Artist = meta.artist || payload.Artist;
        
        if (meta.artwork && meta.artwork.length > 0) {
            payload.Thumbnail = meta.artwork[meta.artwork.length - 1].src;
        }
    } else {
        // Approach 2: DOM Scraping Fallback specifically for Spotify's web player structure
        const titleEl = document.querySelector('[data-testid="context-item-info-title"]');
        const artistEl = document.querySelector('[data-testid="context-item-info-artist"]');
        const imgEl = document.querySelector('img[data-testid="cover-art-image"]');

        if (titleEl) payload.Title = titleEl.textContent;
        if (artistEl) payload.Artist = artistEl.textContent;
        if (imgEl) payload.Thumbnail = imgEl.src;
    }

    return payload;
}

function notifyChange(trackData) {
    if (trackData.Artist.trim() === "Unknown Artist") return;
    
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

        const currentHash = `${trackData.Title}-${trackData.Artist}`;
        
        if (currentHash !== lastTrackHash && trackData.Title.trim() !== "") {
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
        lastTrackHash = `${trackData.Title}-${trackData.Artist}`;
        if(trackData.Title) notifyChange(trackData);
    }, 1000);
} else {
    console.warn("[Thousand Eyes] No <title> element found to observe.");
}