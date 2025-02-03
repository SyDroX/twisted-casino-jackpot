import {
    clearMessage,
    handleRollResponse,
    setInitialSlotsValue,
    showMessage,
    toggleButtons,
    updateCredits,
    getSlotIds,
    formatString
} from "./uiHandler.js";


const apiBaseUrl = `http://localhost:5001`;
const gameApiUrl = `${apiBaseUrl}/Game`;
const errorFormat = "Failed to {defaultErrorText}. Please try again later.";

export async function startGame() {
    const data = await sendRequest("start", "GET", "start the game");
    if (data.credits) {
        updateCredits(data.credits);
    }
}

export async function rollSlots() {
    const slotIds = getSlotIds();
    const defaultErrorText = "roll the slots";

    setInitialSlotsValue(slotIds, "X");
    toggleButtons();
    clearMessage();

    const data = await sendRequest("roll", "POST", defaultErrorText);

    if (data.symbols) {
        handleRollResponse(slotIds, data);
    } else {
        showMessage(data.message || formatString(errorFormat, { defaultErrorText }));
        toggleButtons();
    }
}

export async function cashOut() {
    const data = await sendRequest("cashout", "POST", "cash out");
    if (data.message) {
        clearMessage();
        setInitialSlotsValue(getSlotIds(), "X");
        alert(data.message);
        startGame();
    }
}

async function sendRequest(url, method, defaultErrorText) {
    try {
        const response = await fetch(`${gameApiUrl}/${url}`, {
            method: method,
            credentials: "include",
            headers: { "Content-Type": "application/json" }
        });
        return await response.json();
    } catch (error) {
        showMessage(error.message || formatString(errorFormat, { defaultErrorText }));
    }
}
