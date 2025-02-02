// Configurable API base URL and port
const apiBaseUrl = `http://localhost:5001`;
const gameApiUrl = `${apiBaseUrl}/Game`;
const errorFormat = "Failed to {defaultErrorText}. Please try again later.";


async function startGame() {
    const data = await sendRequest("start", "GET", "start the game");

    if (data.credits) {
        updateCredits(data.credits);
    }
}

async function rollSlots() {
    const slotIds = getSlotIds();

    setInitialSlotsValue(slotIds, "X");
    toggleButtons()
    clearMessage();

    const data = await sendRequest("roll", "POST", "roll the slots");

    if (data.symbols) {
        handleRollResponse(slotIds, data);
    }
}

async function cashOut() {
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
            credentials: "include", // Ensures cookies are sent with the request
            headers: {
                "Content-Type": "application/json",
            }
        });
        return await response.json();
    } catch (error) {
        showMessage(error.message || formatString(errorFormat, {defaultErrorText: defaultErrorText}));
    }
}

function getSlotIds(){
    return ["slot1", "slot2", "slot3"];
}

function formatString(template, params) {
    return template.replace(/{(\w+)}/g, (match, key) => params[key] || match);
}

function animateSlots(slotIds, symbols, spinDelay) {
    slotIds.forEach((id, index) => {
        setTimeout(() => {
            document.getElementById(id).textContent = symbols[index];
        }, (index + 1) * spinDelay);
    });
}

function toggleButtons() {
    const buttons =
        [document.getElementById("rollButton"), document.getElementById("cashOutButton")];
    buttons.forEach(button => button.disabled = !button.disabled);
}

function setInitialSlotsValue(slotIds, value) {
    slotIds.forEach((id) => (document.getElementById(id).textContent = value));
}

function handleRollResponse(slotIds, data) {
    const slotCount = slotIds.length;
    const spinDelay = 1000; // Delay for each slot (1 second per slot)
    const bufferTime = 1000; // Additional buffer after the animation
    const totalDelay = slotCount * spinDelay + bufferTime;

    animateSlots(slotIds, data.symbols, spinDelay);

    setTimeout(() => {
        updateCredits(data.credits);
        showMessage(data.message, data.isWinning);

        toggleButtons();
    }, totalDelay);
}

function updateCredits(credits) {
    document.getElementById("credits").textContent = `Credits: ${credits}`;
}

// Display a message to the user
function showMessage(message, isWinning = false) {
    const messageContainer = document.getElementById("message");

    if (isWinning) {
        messageContainer.classList.add("win");
    }

    messageContainer.textContent = message;
    messageContainer.classList.remove("hidden"); // Show the container
}

// Clear any existing message
function clearMessage() {
    const messageContainer = document.getElementById("message");
    messageContainer.classList.add("hidden");
    messageContainer.classList.remove("win");
}

// Event Listeners
document.getElementById("rollButton").addEventListener("click", rollSlots);
document.getElementById("cashOutButton").addEventListener("click", cashOut);

// Start the game on page load
startGame();
