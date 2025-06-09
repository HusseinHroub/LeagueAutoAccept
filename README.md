# League of Legends Auto-Accept Tool

## Purpose

This repository provides a tool to automatically accept match ready checks in League of Legends. It interacts with the League Client API (LCU API) to detect when a match is found and automatically sends the accept command, saving you from manually clicking "Accept" each time.

The project originally used a polling approach to check the game state, but has since migrated to a more efficient and responsive WebSocket-based event listener.

---

## File Overview

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

## Migration from Polling to WebSocket

- **Old Approach (Polling):**  
  The tool used `MainLogic.cs` to repeatedly poll the LCU API for the current game phase. This method worked but was less efficient, as it required constant HTTP requests and could introduce delays or unnecessary load.

- **New Approach (WebSocket):**  
  The tool now uses `LCUWebSocketListener.cs` to listen for real-time events from the League Client via WebSocket. This allows the tool to react instantly when a match is found, with lower resource usage and better reliability.

---

## How to Use

1. **Build the project in Visual Studio 2022 (.NET 9).**
2. **Run the generated executable.**
3. **The tool will wait for the League Client to start, then automatically accept matches when they are found.**

---

## Notes
- Because of an existing bug, only launch it when league client is opened, otherwise it will not receive any events (will be fixed in future)
- This project is for educational and personal use only.
