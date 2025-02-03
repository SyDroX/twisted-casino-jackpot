function animateSlots(slotIds, symbols, spinDelay) {
    slotIds.forEach((id, index) => {
        setTimeout(() => {
            document.getElementById(id).textContent = symbols[index];
        }, (index + 1) * spinDelay);
    });
}

export function toggleButtons() {
    const buttons = [
        document.getElementById("rollButton"),
        document.getElementById("cashOutButton")
    ];
    buttons.forEach(button => button.disabled = !button.disabled);
}

export function setInitialSlotsValue(slotIds, value) {
    slotIds.forEach((id) => (document.getElementById(id).textContent = value));
}

export function handleRollResponse(slotIds, data) {
    const slotCount = slotIds.length;
    const spinDelay = 1000;
    const bufferTime = 1000;
    const totalDelay = slotCount * spinDelay + bufferTime;

    animateSlots(slotIds, data.symbols, spinDelay);

    setTimeout(() => {
        updateCredits(data.credits);
        showMessage(data.message, data.isWinning);
        toggleButtons();
    }, totalDelay);
}

export function updateCredits(credits) {
    document.getElementById("credits").textContent = `Credits: ${credits}`;
}

export function showMessage(message, isWinning = false) {
    const messageContainer = document.getElementById("message");

    if (isWinning) {
        messageContainer.classList.add("win");
    }

    messageContainer.textContent = message;
    messageContainer.classList.remove("hidden");
}

export function clearMessage() {
    const messageContainer = document.getElementById("message");
    messageContainer.classList.add("hidden");
    messageContainer.classList.remove("win");
}

export function formatString(template, params) {
    return template.replace(/{(\w+)}/g, (match, key) => params[key] || match);
}

export function getSlotIds() {
    return ["slot1", "slot2", "slot3"];
}

