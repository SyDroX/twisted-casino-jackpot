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
    const defaultErrorText = "start the game";
    const data = await sendRequest("start", "GET", defaultErrorText);

    if (data.credits) {
        updateCredits(data.credits);
    } else{
        showMessage(data.message || formatString(errorFormat, { defaultErrorText }));
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
    const defaultErrorText = "cash out";
    const data = await sendRequest("cashout", "POST", defaultErrorText);

    if (data.credits) {
        clearMessage();
        setInitialSlotsValue(getSlotIds(), "X");
        alert(data.message);

        await startGame();
    } else {
        showMessage(data.message || formatString(errorFormat, { defaultErrorText:defaultErrorText }))
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
