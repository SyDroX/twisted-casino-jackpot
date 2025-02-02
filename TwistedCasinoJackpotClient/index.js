// Configurable API base URL and port
const apiBaseUrl = `http://localhost:5001`;
const gameApiUrl = `${apiBaseUrl}/Game`;
const errorFormat = "Failed to {errorName}. Please try again later.";

// Start the game by initializing credits
async function startGame() {
    try {
        const response = await fetch(`${gameApiUrl}/start`, {
            method: "GET",
            credentials: "include", // Ensures cookies are sent with the request
            headers: {
                "Content-Type": "application/json",
            }
        });

        const data = await response.json();
        updateCredits(data.credits);
    } catch (error) {
        showMessage(error.message || formatString(errorFormat, {errorName: "start the game"}));
    }
}

// Roll the slot machine
async function rollSlots() {
    setInitialSlotsValue("X");
    toggleButtons()
    clearMessage();

    try {
        const response = await fetch(`${gameApiUrl}/roll`, {
            method: "POST",
            credentials: "include", // Ensures cookies are sent with the request
            headers: {
                "Content-Type": "application/json",
            }
        });

        const data = await response.json();

        if (data.symbols) {
            handleRollResponse(rollButton, slotIds, data);
        }
    } catch (error) {
        showMessage(error.message || formatString(errorFormat, {errorName: "roll"}));
        toggleButtons();
    }
}

async function cashOut() {
    try {
        const response = await fetch(`${gameApiUrl}/cashout`, {
            method: "POST",
            credentials: "include", // Ensures cookies are sent with the request
            headers: {
                "Content-Type": "application/json",
            }
        });
        const data = await response.json();
        alert(data.message);
        startGame();
    } catch (error) {
        showMessage(error.message || formatString(errorFormat, {errorName: "cash out"}));
    }
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

function toggleButtons(){
    const buttons =
        [document.getElementById("rollButton"),
        document.getElementById("cashOutButton")];
    buttons.forEach(button => button.disabled = !button.disabled);
}

function setInitialSlotsValue(value) {
    const slotIds = ["slot1", "slot2", "slot3"];
    slotIds.forEach((id) => (document.getElementById(id).textContent = value));
}

function handleRollResponse(rollButton, slotIds, data) {
    const slotCount = slotIds.length;
    const spinDelay = 1000; // Delay for each slot (1 second per slot)
    const bufferTime = 1000; // Additional buffer after the animation
    const totalDelay = slotCount * spinDelay + bufferTime;

    animateSlots(slotIds, data.symbols, spinDelay);

    setTimeout(() => {
        updateCredits(data.credits);
        showMessage(data.message, data.isWinning);

        rollButton.disabled = false;
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
