# League of Legends Auto-Accept Tool

## Purpose

This repository provides a tool to automatically accept match ready checks in League of Legends. It interacts with the League Client API (LCU API) to detect when a match is found and automatically sends the accept command, saving you from manually clicking "Accept" each time.

The project originally used a polling approach to check the game state, but has since migrated to a more efficient and responsive WebSocket-based event listener.

---

## Install

Go to [Releases](https://github.com/HusseinHroub/LeagueAutoAccept/releases)

Download then extract *.rar (ex: 1.0.rar)

Based on your windows version choose the correct folder

Open the LOLAutoAccept.exe

Because it cost money to get a valid certificate, and am not willing to do so for this app, you will see the following screen:

![image](https://github.com/user-attachments/assets/d9d0b6d3-5256-4eab-923c-baae35ec9426)

- If you don't trust my source code, simply click "Don't run", delete the app
- Else, click on "More Info", then "Run anyway"

![image](https://github.com/user-attachments/assets/a7d6a8ab-28a4-408b-bc42-cc2483a6963d)


Now once the app is launched, you will see the console screen
![image](https://github.com/user-attachments/assets/15429183-a02d-4720-9f93-f8dc2dd6a633)

## Project File Overview

### `Program.cs`
- **Entry point of the application.**
- Waits for the League Client to start, retrieves authentication details, and launches the WebSocket listener to handle auto-accept logic.

### `LCU.cs`
- **Handles communication with the League Client API.**
- Detects if the League Client is running and extracts the authentication token and port from the client process.
- Provides methods for making authenticated HTTP requests to the LCU API.

### `MainLogic.cs`
- **Implements the legacy polling-based auto-accept logic.**
- Periodically sends HTTP requests to check the current game phase.
- If the phase is `"ReadyCheck"`, it sends a request to accept the match.
- This approach is now deprecated in favor of the WebSocket-based solution, but is kept for reference.

### `LCUWebSocketListener.cs`
- **Implements the current WebSocket-based event-driven auto-accept logic.**
- Connects to the League Client's WebSocket API and subscribes to gameflow session events.
- Listens for real-time updates and parses incoming messages to detect when the phase changes to `"ReadyCheck"`.
- Automatically sends the accept command when a match is found.
- This approach is more efficient and responsive than polling.

---

## Notes
- This project is for educational and personal use only.
