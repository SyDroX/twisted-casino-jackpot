# 🎰 Twisted Casino Jackpot

A full-stack web application for an online slot machine game where **the house always wins**!

---

## 📌 Technologies Used

### **Frontend (Client)**
- **JavaScript (Vanilla JS)** – Simple, lightweight client logic
- **HTML & CSS** – Basic UI layout & styling
- **Fetch API** – Communicates with the backend via HTTP requests
- **Jest** – Unit testing for frontend logic

### **Backend (Server)**
- **C# (.NET 8, ASP.NET Core Web API)** – Backend logic & game rules
- **Session Storage** – Keeps track of game state per player
- **Swagger** – API documentation & testing
- **xUnit & Moq** – Unit & integration testing

---

## 🚀 How to Run the App

### **🔹 Prerequisites**
Before running the app, ensure you have the following installed:
- **.NET SDK 8+** (for the backend)
- **Node.js** (for frontend testing)
- **WebStorm** or **any Web IDE** (for frontend development)
- **Rider** or **Visual Studio** (for backend development)

---

## 🖥️ Running the Backend (`ASP.NET Core`)

### **🔹 Step 1: Clone the Repository**
```sh
git clone https://github.com/your-repo/twisted-casino.git
cd twisted-casino
```

### **🔹 Step 2: Open the Backend in Your C# IDE**
- **Using Rider or Visual Studio**
  1. Open the project **`TwistedCasinoJackpotServer`**.
  2. Set it as the **startup project**.
  3. Build the project (`Ctrl + Shift + B` in Rider).
  4. Run the project.

- **Using Command Line**
  ```sh
  dotnet run --project TwistedCasinoJackpotServer
  ```

### **🔹 Step 3: Access the API via Swagger**
Once the backend is running, open **Swagger UI** to test the API:
- Visit: [`https://localhost:5001/swagger`](https://localhost:5001/swagger)

---

## 🌐 Running the Frontend (`Vanilla JavaScript`)

### **🔹 Step 1: Navigate to the Frontend Directory**
```sh
cd TwistedCasinoJackpotClient
```

### **🔹 Step 2: Open `index.html` in a Web Browser**
- You can open the file **manually** in any browser.
- If using **WebStorm**, right-click `index.html` → Click **Open in Browser**.


## ✅ Running Backend Tests (xUnit & Moq)

### **🔹 Step 1: Navigate to the Test Directory**
```sh
cd TwistedCasinoJackpotServer.Tests
```

### **🔹 Step 2: Run All Tests**
```sh
dotnet test
```

---

## 🧪 Running Frontend Tests (Jest)

### **🔹 Step 1: Navigate to the Frontend Test Directory**
```sh
cd TwistedCasinoJackpotClient/tests
```

### **🔹 Step 2: Install Jest (if not installed)**
```sh
npm install --save-dev jest
```

### **🔹 Step 3: Run All Frontend Tests**
```sh
npm test
```

### **🔹 Step 4: Run a Specific Frontend Test**
```sh
npx jest gameLogic.test.js
```

---

## 🔗 API Endpoints

| **Method** | **Endpoint**       | **Description**                 |
|------------|-------------------|---------------------------------|
| `GET`      | `/Game/start`      | Start a new game session       |
| `POST`     | `/Game/roll`       | Roll the slot machine          |
| `POST`     | `/Game/cashout`    | Cash out winnings & reset game |

---

