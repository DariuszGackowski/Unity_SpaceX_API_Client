🚀 Unity SpaceX API Client
A Unity (2022.3.51f1) mobile demo application that connects to the public SpaceX API, 
featuring a real-time simulation of the Tesla Roadster orbiting the Sun and a fully interactive browser for SpaceX rocket launches.

📱 Platform
Targeted for mobile devices (Android or iOS), with full support for touch input and gestures.

🎯 Core Features

🔸 Main Menu

Allows users to choose between:
- Tesla Roadster Orbital Simulation
- SpaceX Launches Browser
Ability to freely return to the main menu from either view without restarting the application.

🌞 Tesla Roadster – Orbital Simulation

Uses orbital data from the Roadster orbital elements JPL Horizon.csv file and the provided OrbitalElements.dll library.
Displays a time-accelerated simulation of the Roadster’s position orbiting the Sun.
Displays the last 20 positions as a "comet tail" behind the current position.
Shows all current orbital data on screen, including simulation date (in local time zone).
Basic camera control via touch gestures.

🚀 SpaceX Launches Browser

Loads a scrollable, interactable list of all SpaceX launches using the SpaceX API.
Each list item includes:
- Mission name
- Number of payloads
- Rocket name and country of origin
- Icon indicating whether the launch is in the past or future
- Tapping on a launch opens a popup window with detailed information on each ship involved:
  Ship name, type, home port, and number of missions
- Optimized for mobile performance and responsiveness

🛠 Technologies Used

- Unity 2022.3.51f1
- C#
- Canvas UI
- REST API (via UnityWebRequest)
- JSON parsing (Newtonsoft.Json)
- OrbitalElements.dll for orbital position calculations

🎯 Optional Objectives (partially implemented)

- Camera rotation via gestures in Roadster simulation
- Roadster interpolation for >24h time gaps
- Buttons in launch detail popups that open ship photo URLs
  
⚙️ How to Run

- Open the project in Unity 2022.3.51f1.
- Launch the MainMenu scene and choose one of the available modes.
- Make sure your device has internet access to fetch data from the SpaceX API.

🧠 About
This project was created as part of a technical recruitment challenge and serves as a showcase of real-time API integration, 
orbital mechanics visualization, and mobile-friendly UI in Unity.
