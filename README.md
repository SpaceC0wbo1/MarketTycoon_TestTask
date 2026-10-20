# 🛒 Supermarket Tycoon 3D (Idle Market Simulation)

An isometric 3D Idle Tycoon management simulation built in **Unity 6**. The player develops, builds, and optimizes a bustling supermarket: purchasing abstract product shelves, managing customer flow, expanding the store perimeter, and ensuring smooth checkout operations.

The project demonstrates production-ready software engineering standards utilizing a strict **Model-View-Controller (MVC)** architectural pattern, decoupled game systems, and an entirely native Unity stack without external plugins or third-party dependencies.

---

## 📸 Media & Previews
**Screenshots**:
<img width="827" height="463" alt="Screenshot 2026-10-21 000324" src="https://github.com/user-attachments/assets/3c050bc3-f38d-4c73-ba2b-1bdbff239e3f" />

<img width="826" height="460" alt="Screenshot 2026-10-21 000355" src="https://github.com/user-attachments/assets/add8460e-a3f6-401c-87f4-3b3cc699522d" />

https://github.com/user-attachments/assets/492c7a3d-58e9-44c5-bc06-3566e0212f45

---

## ✨ Key Features & Gameplay Loop

1. **Store Setup & Placement:** 
   - Buy and construct interactive building spots across the market layout.
   - Unlock new shelves and checkout registers to scale up the business.
2. **Expansion System:**
   - Invest earned revenue into store room expansions to enlarge the total market footprint.
   - Expandable building lots provide more floor space for high-volume customer throughput.
3. **Data-Driven Shelves (Config-Based):**
   - Shelves are abstract and driven by modular configuration data.
   - Configs dictate sale values, customer interaction timings, and cash yields per product sold.
4. **Autonomous AI Customers:**
   - NavMesh-powered AI pathfinding for entering, browsing shelves, collecting products, queuing at cash registers, paying, and exiting.
   - Multi-queue balancing ensures customers automatically pick the shortest available checkout line.
5. **Robust Economy & Cashier Operations:**
   - Checkout points process customer carts and instantly credit profits to the player's balance.
   - Real-time HUD updates current funds and market indicators.
6. **Native Save & Load System:**
   - Pure native JSON file persistence tracking currency, unlocked building spots, and expansion states between sessions.
7. **RTS-Style Camera Navigation:**
   - Smooth viewport panning via right-mouse drag or screen edge scrolling, with dynamic zoom and instant reset.

---

## 🛠️ Architecture & Implementation Details

The codebase is built strictly adhering to clean code principles, **SOLID**, and **Model-View-Controller (MVC)** separation:

- **Model (`*Model.cs`):** Pure C# data containers and state machines. Models are completely detached from `UnityEngine.Object` and handle raw state changes, configuration values, event triggers, and serialization data.
- **View (`*View.cs`):** Derived from `MonoBehaviour`. Views strictly handle rendering, particle effects, UI presentation, animations, and forward native Unity engine callbacks (triggers, collisions, button clicks) directly to controllers.
- **Controller (`*Controller.cs`):** The orchestration layer mediating between Models and Views. Listens to events, validates business logic, deducts balances, updates data, and coordinates dependencies.
- **Dependency Inversion & Game Core:**
   - Native service injection and centralized registry (`MarketRegistry.cs`) without Zenject/Extenject.
   - Clean entry-point bootstrap flow initializing scene loaders, currency systems, and persistent settings.
- **Dynamic Navigation:**
   - `NavMeshRuntimeUpdater.cs` dynamically re-bakes or updates navigation surfaces at runtime whenever new shelves or expansions are placed.

---

## 🎮 Controls

| Action | Input |
| :--- | :--- |
| **Move Camera** | Hold `RMB` & Drag **OR** Move cursor to Screen Edges |
| **Zoom In / Out** | `Mouse Scroll Wheel` |
| **Reset Camera Position** | `F` |
| **Interact / Build** | `Left Mouse Button (LMB)` |
| **Pause Game / Settings** | `Escape` / In-game UI button |

---
## 🚀 How to Run the Project
1. Unity Version:
  - Open the project in Unity 6 (6000.3.17f1) or a compatible Unity 6 LTS release.

2. Opening the Scene:
  - Navigate to Assets/Scenes/01_MainMenu.unity in the Project window.
  - Crucial: Always start execution from 01_MainMenu.unity to guarantee correct bootstrap lifecycle, UI initialization, and data loading.

3. Enter Play Mode:
  - Press the Play button in the Unity Editor toolbar.
  - Click Play on the main menu to enter the supermarket simulation scene.

📄 License
This project is developed for educational and portfolio demonstration purposes.
