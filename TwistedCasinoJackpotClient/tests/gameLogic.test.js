import { startGame, rollSlots, cashOut } from "src/gameLogic.js";
import {beforeEach, expect, jest, test} from '@jest/globals';

global.fetch = jest.fn(() =>
    Promise.resolve({
        json: () => Promise.resolve({ credits: 9, message: "You lost!", isWinning: false, symbols: ["A", "B", "C"] })
    })
);

// Ensure the DOM is set up before each test
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
    jest.useFakeTimers();
});

test("rollSlots() loosing roll", async () => {
    await rollSlots();
    jest.advanceTimersByTime(4000);
    expect(document.getElementById("slot1").textContent).toBe("A");
    expect(document.getElementById("slot2").textContent).toBe("B");
    expect(document.getElementById("slot3").textContent).toBe("C");
    expect(document.getElementById("credits").textContent).toBe("Credits: 9");
    expect(document.getElementById("message").textContent).toBe("You lost!");
});