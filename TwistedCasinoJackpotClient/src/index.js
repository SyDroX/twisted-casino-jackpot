import { startGame, rollSlots, cashOut } from "./gameLogic.js";

document.getElementById("rollButton").addEventListener("click", rollSlots);
document.getElementById("cashOutButton").addEventListener("click", cashOut);

startGame();
