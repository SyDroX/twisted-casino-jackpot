import {
    animateSlots,
    clearMessage,
    handleRollResponse,
    setInitialSlotsValue,
    showMessage,
    toggleButtons,
    updateCredits,
    getSlotIds,
    formatString
} from "src/uiHandler.js";

import {beforeEach, expect, jest, test} from '@jest/globals';

beforeEach(() => {
    document.body.innerHTML = `
        <div id="credits"></div>
        <div id="message"></div>
        <button id="rollButton"></button>
        <button id="cashOutButton"></button>
        <div id="slot1">X</div>
        <div id="slot2">X</div>
        <div id="slot3">X</div>
    `;
});

test("animateSlots() updates slot values after delays", () => {
    jest.useFakeTimers()
    const slotIds = ["slot1", "slot2", "slot3"];
    const symbols = ["C", "L", "O"];
    const spinDelay = 1000;

    animateSlots(slotIds, symbols, spinDelay);

    jest.advanceTimersByTime(1000);
    expect(document.getElementById("slot1").textContent).toBe("C");

    jest.advanceTimersByTime(1000);
    expect(document.getElementById("slot2").textContent).toBe("L");

    jest.advanceTimersByTime(1000);
    expect(document.getElementById("slot3").textContent).toBe("O");
});

test("toggleButtons() disables and enables buttons", () => {
    document.getElementById("rollButton").disabled = false;
    document.getElementById("cashOutButton").disabled = false;

    toggleButtons();

    expect(document.getElementById("rollButton").disabled).toBe(true);
    expect(document.getElementById("cashOutButton").disabled).toBe(true);

    toggleButtons();

    expect(document.getElementById("rollButton").disabled).toBe(false);
    expect(document.getElementById("cashOutButton").disabled).toBe(false);
});

test("setInitialSlotsValue() sets all slot values", () => {
    const slotIds = ["slot1", "slot2", "slot3"];

    setInitialSlotsValue(slotIds, "X");

    expect(document.getElementById("slot1").textContent).toBe("X");
    expect(document.getElementById("slot2").textContent).toBe("X");
    expect(document.getElementById("slot3").textContent).toBe("X");
});

test("updateCredits() updates credit display", () => {
    updateCredits(15);

    expect(document.getElementById("credits").textContent).toBe("Credits: 15");
});

test("showMessage() updates message and adds win class when winning", () => {
    showMessage("You won!", true);

    expect(document.getElementById("message").textContent).toBe("You won!");
    expect(document.getElementById("message").classList.contains("win")).toBe(true);
});

test("showMessage() updates message without win class when losing", () => {
    showMessage("You lost!", false);

    expect(document.getElementById("message").textContent).toBe("You lost!");
});

test("clearMessage() hides the message", () => {
    document.getElementById("message").classList.add("win");
    document.getElementById("message").textContent = "Some text";

    clearMessage();

    expect(document.getElementById("message").classList.contains("hidden")).toBe(true);
    expect(document.getElementById("message").classList.contains("win")).toBe(false);
    expect(document.getElementById("message").textContent).toBe("");
});

test("getSlotIds() returns correct slot IDs", () => {
    expect(getSlotIds()).toEqual(["slot1", "slot2", "slot3"]);
});

test("formatString() returns the same string when no placeholders exist", () => {
    const template = "My name is {name}.";
    const params = { name: "Alice" };

    expect(formatString(template, params)).toBe("My name is Alice.");
});

