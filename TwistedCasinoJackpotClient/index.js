// Configurable API base URL and port
const apiBaseUrl = `https://localhost:5000`;
const gameApiUrl = `${apiBaseUrl}/Game`;

// Start the game by initializing credits
async function startGame() {
    try {
        const response = await fetch(`${gameApiUrl}/start`);
        const data = await response.json();
        updateCredits(data.credits);
    } catch (error) {
        showMessage("Failed to start the game. Please try again later.");
    }
}

// Roll the slot machine
async function rollSlots() {
    const rollButton = document.getElementById("rollButton");
    rollButton.disabled = true;

    const slots = ["slot1", "slot2", "slot3"];
    slots.forEach((id) => (document.getElementById(id).textContent = "X"));
    clearMessage();

    try {
        const response = await fetch(`${gameApiUrl}/roll`, { method: "POST" });
        const data = await response.json();

        if (data.message) {
            showMessage(data.message);
            rollButton.disabled = false;
            return;
        }

        // Animate the slot machine roll
        slots.forEach((id, index) => {
            setTimeout(() => {
                document.getElementById(id).textContent = data.Symbols[index];
            }, (index + 1) * 1000);
        });

        setTimeout(() => {
            updateCredits(data.credits);
            if (data.isWinning) {
                showMessage("Congratulations! You won!");
            } else {
                showMessage("You lost. Better luck next time!");
            }
            rollButton.disabled = false;
        }, 4000);
    } catch (error) {
        showMessage("An error occurred while rolling. Please try again later.");
        rollButton.disabled = false;
    }
}

// Cash out the player's credits
async function cashOut() {
    try {
        const response = await fetch(`${gameApiUrl}/cashout`, { method: "POST" });
        const data = await response.json();
        alert(data.message);
        startGame();
    } catch (error) {
        showMessage("Failed to cash out. Please try again later.");
    }
}

// Update credits display
function updateCredits(credits) {
    document.getElementById("credits").textContent = `Credits: ${credits}`;
}

// Display a message to the user
function showMessage(message) {
    document.getElementById("message").textContent = message;
}

// Clear any existing message
function clearMessage() {
    document.getElementById("message").textContent = "";
}

// Event Listeners
document.getElementById("rollButton").addEventListener("click", rollSlots);
document.getElementById("cashOutButton").addEventListener("click", cashOut);

// Start the game on page load
startGame();
