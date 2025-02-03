import {startGame, rollSlots, cashOut} from "src/gameLogic.js";
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

test("startGame() success", async () => {
    global.fetch = jest.fn(() =>
        Promise.resolve({
            json: () => Promise.resolve({credits: 10})
        }));

    await startGame();

    expectCredits(10);
    expectSlots(["X", "X", "X"]);
});

test("startGame() failure gracefully", async () => {
    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject(new Error("Fetch failed"))
    );

    await startGame();

    expectMessage("Fetch failed");

    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject(new Error())
    );

    await startGame();

    const errorText = "Failed to start the game. Please try again later.";
    expectMessage(errorText);

    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject()
    );

    await startGame();

    expectMessage(errorText);
});

test("cashOut() success", async () => {
    global.alert = jest.fn();
    global.fetch = jest.fn(() =>
        Promise.resolve({
            json: () => Promise.resolve({message: "Cashed out successfully!", credits: 10})
        }));

    await cashOut();

    expectCredits(10);
    expectSlots(["X", "X", "X"]);
    expect(global.alert).toHaveBeenCalledWith("Cashed out successfully!");

});

test("cashOut() failure gracefully", async () => {
    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject(new Error("Fetch failed"))
    );

    await cashOut();

    expectMessage("Fetch failed");

    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject(new Error())
    );

    await cashOut();

    const errorText = "Failed to cash out. Please try again later.";

    expectMessage(errorText);

    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject()
    );

    await cashOut();

    expectMessage(errorText);
});


test("rollSlots() loosing roll", async () => {
    jest.useFakeTimers();
    global.fetch = jest.fn(() =>
        Promise.resolve({
            json: () => Promise.resolve({credits: 9, message: "You lost!", isWinning: false, symbols: ["A", "B", "C"]})
        })
    );

    await rollSlots();

    jest.advanceTimersByTime(4000);
    expectCredits(9);
    expectSlots(["A", "B", "C"]);
    expectMessage("You lost!");
});

test("rollSlots() winning roll", async () => {
    jest.useFakeTimers();
    global.fetch = jest.fn(() =>
        Promise.resolve({
            json: () => Promise.resolve({credits: 30, message: "You won!", isWinning: true, symbols: ["W", "W", "W"]})
        })
    );

    await rollSlots();

    jest.advanceTimersByTime(4000);
    expectSlots(["W", "W", "W"]);
    expectCredits(30);
    expectMessage("You won!");
    expect(document.getElementById("message").classList.contains("win")).toBe(true);
});

test("rollSlots() failure gracefully", async () => {
    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject(new Error("Fetch failed"))
    );

    await rollSlots();

    expectMessage("Fetch failed");

    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject()
    );

    await rollSlots(new Error());

    const errorText = "Failed to roll the slots. Please try again later.";
    expectMessage(errorText);

    global.fetch = jest.fn().mockImplementationOnce(() =>
        Promise.reject()
    );

    await rollSlots();

    expectMessage(errorText);
});

function expectSlots(slotValues) {
    expect(document.getElementById("slot1").textContent).toBe(slotValues[0]);
    expect(document.getElementById("slot2").textContent).toBe(slotValues[1]);
    expect(document.getElementById("slot3").textContent).toBe(slotValues[2]);
}

function expectCredits(credits) {
    expect(document.getElementById("credits").textContent).toBe(`Credits: ${credits}`);
}

function expectMessage(message) {
    expect(document.getElementById("message").textContent).toBe(message);
}